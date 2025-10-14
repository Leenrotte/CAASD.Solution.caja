using System;
using System.Linq;
using System.Windows.Forms;
using CAASD.Core.Interfaces;
using CAASD.Core.Entities;
using CAASD.Integracion.UnitOfWork;

namespace CAASD.Caja1
{
    public partial class FrmCrearFactura : Form
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _usuarioId;
        private int? _facturaSeleccionadaId = null;
        public FrmCrearFactura(int usuarioId)
        {
            InitializeComponent();
            _unitOfWork = new UnitOfWork();
            _usuarioId = usuarioId;

            CargarContratos();
            CargarFacturas();
            ConfigurarDataGridView();
        }
        private void ConfigurarDataGridView()
        {
            dgvFacturas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvFacturas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFacturas.MultiSelect = false;
            dgvFacturas.ReadOnly = true;
        }

        private void CargarContratos()
        {
            try
            {
                var contratos = _unitOfWork.Contratos.Find(c => c.UsuarioId == _usuarioId && c.Activo);

                if (!contratos.Any())
                {
                    var resultado = MessageBox.Show(
                        "Este usuario no tiene contratos activos. ¿Desea crear uno ahora?",
                        "Sin Contratos",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (resultado == DialogResult.Yes)
                    {
                        CrearContratoAutomatico();
                        contratos = _unitOfWork.Contratos.Find(c => c.UsuarioId == _usuarioId && c.Activo);
                    }
                }

                cmbContrato.DataSource = contratos.ToList();
                cmbContrato.DisplayMember = "NumeroContrato";
                cmbContrato.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar contratos: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CrearContratoAutomatico()
        {
            var usuario = _unitOfWork.Usuarios.GetById(_usuarioId);

            var contrato = new Contrato
            {
                NumeroContrato = $"CTR-{DateTime.Now:yyyyMMdd}-{_usuarioId}",
                UsuarioId = _usuarioId,
                Direccion = usuario.DireccionCompleta,
                Sector = "Zona Central",
                Latitud = 18.4861m,
                Longitud = -69.9312m,
                FechaInicio = DateTime.Now,
                Activo = true
            };

            _unitOfWork.Contratos.Add(contrato);
            _unitOfWork.SaveChanges();
        }

        private void ActualizarVistaPrevia()
        {
            int metros = (int)numMetrosConsumidos.Value;
            decimal agua = metros * 15m;
            decimal alcantarillado = agua * 0.30m;
            decimal basura = 150m;
            decimal subtotal = agua + alcantarillado + basura;
            decimal itbis = subtotal * 0.18m;
            decimal total = subtotal + itbis;

            lblVistaPrevia.Text = $"Agua: RD${agua:N2}\n" +
                                 $"Alcantarillado: RD${alcantarillado:N2}\n" +
                                 $"Basura: RD${basura:N2}\n" +
                                 $"Subtotal: RD${subtotal:N2}\n" +
                                 $"ITBIS (18%): RD${itbis:N2}\n" +
                                 $"───────────────\n" +
                                 $"TOTAL: RD${total:N2}";
        }

        private void numMetrosConsumidos_ValueChanged(object sender, EventArgs e)
        {
            ActualizarVistaPrevia();
        }

        private void CargarFacturas()
        {
            try
            {
                var facturas = _unitOfWork.Facturas.GetFacturasPendientesByUsuario(_usuarioId);

                dgvFacturas.DataSource = facturas.Select(f => new
                {
                    f.Id,
                    f.NumeroFactura,
                    f.FechaEmision,
                    f.FechaVencimiento,
                    f.MetrosConsumidos,
                    f.MontoTotal,
                    Estado = f.EstadoPago
                }).ToList();

                dgvFacturas.Columns["Id"].Visible = false;
                dgvFacturas.Columns["NumeroFactura"].HeaderText = "Nro. Factura";
                dgvFacturas.Columns["FechaEmision"].HeaderText = "F. Emisión";
                dgvFacturas.Columns["FechaVencimiento"].HeaderText = "Vencimiento";
                dgvFacturas.Columns["MetrosConsumidos"].HeaderText = "Consumo (m³)";
                dgvFacturas.Columns["MontoTotal"].HeaderText = "Total";
                dgvFacturas.Columns["MontoTotal"].DefaultCellStyle.Format = "C2";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar facturas: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFormulario()
        {
            numMetrosConsumidos.Value = 10;
            dtpFechaEmision.Value = DateTime.Now;
            lblVistaPrevia.Text = "";
            _facturaSeleccionadaId = null;
            btnGuardar.Text = "Crear Factura";
        }

        private void btnCrearNueva_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            _facturaSeleccionadaId = null;
            grpDatosFactura.Enabled = true;
            btnGuardar.Text = "Crear Factura";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbContrato.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione un contrato", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int contratoId = (int)cmbContrato.SelectedValue;
                int metrosConsumidos = (int)numMetrosConsumidos.Value;

                // Calcular montos
                decimal montoAgua = metrosConsumidos * 15m;
                decimal montoAlcantarillado = montoAgua * 0.30m;
                decimal montoBasura = 150m;
                decimal subtotal = montoAgua + montoAlcantarillado + montoBasura;
                decimal itbis = subtotal * 0.18m;
                decimal montoTotal = subtotal + itbis;

                if (_facturaSeleccionadaId == null)
                {
                    // CREAR NUEVA
                    var factura = new Factura
                    {
                        NumeroFactura = $"F-{DateTime.Now:yyyyMM}-{contratoId}-{new Random().Next(1000, 9999)}",
                        ContratoId = contratoId,
                        FechaEmision = dtpFechaEmision.Value,
                        FechaVencimiento = dtpFechaEmision.Value.AddDays(15),
                        MetrosConsumidos = metrosConsumidos,
                        MontoAgua = montoAgua,
                        MontoAlcantarillado = montoAlcantarillado,
                        MontoBasura = montoBasura,
                        Itbis = itbis,
                        MontoTotal = montoTotal,
                        EstadoPago = "Pendiente"
                    };

                    _unitOfWork.Facturas.Add(factura);
                    _unitOfWork.SaveChanges();

                    MessageBox.Show("Factura creada exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // RECTIFICAR EXISTENTE
                    var factura = _unitOfWork.Facturas.GetById(_facturaSeleccionadaId.Value);

                    if (factura != null)
                    {
                        if (factura.EstadoPago == "Pagada")
                        {
                            MessageBox.Show("No se puede rectificar una factura pagada",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        factura.MetrosConsumidos = metrosConsumidos;
                        factura.MontoAgua = montoAgua;
                        factura.MontoAlcantarillado = montoAlcantarillado;
                        factura.MontoBasura = montoBasura;
                        factura.Itbis = itbis;
                        factura.MontoTotal = montoTotal;
                        factura.FechaEmision = dtpFechaEmision.Value;
                        factura.FechaVencimiento = dtpFechaEmision.Value.AddDays(15);

                        _unitOfWork.Facturas.Update(factura);
                        _unitOfWork.SaveChanges();

                        MessageBox.Show("Factura rectificada exitosamente", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                CargarFacturas();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRectificar_Click(object sender, EventArgs e)
        {
            if (dgvFacturas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una factura para rectificar",
                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int facturaId = Convert.ToInt32(dgvFacturas.SelectedRows[0].Cells["Id"].Value);
                var factura = _unitOfWork.Facturas.GetById(facturaId);

                if (factura == null)
                {
                    MessageBox.Show("Factura no encontrada", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (factura.EstadoPago == "Pagada")
                {
                    MessageBox.Show("No se puede rectificar una factura pagada",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _facturaSeleccionadaId = facturaId;
                cmbContrato.SelectedValue = factura.ContratoId;
                numMetrosConsumidos.Value = factura.MetrosConsumidos;
                dtpFechaEmision.Value = factura.FechaEmision;

                grpDatosFactura.Enabled = true;
                btnGuardar.Text = "Guardar Cambios";

                MessageBox.Show("Modifique los datos y presione 'Guardar Cambios'",
                    "Modo Rectificación", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvFacturas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una factura para eliminar",
                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int facturaId = Convert.ToInt32(dgvFacturas.SelectedRows[0].Cells["Id"].Value);
                var factura = _unitOfWork.Facturas.GetById(facturaId);

                if (factura.EstadoPago == "Pagada")
                {
                    MessageBox.Show("No se puede eliminar una factura pagada",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var resultado = MessageBox.Show(
                    $"¿Está seguro de eliminar la factura {factura.NumeroFactura}?",
                    "Confirmar Eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (resultado == DialogResult.Yes)
                {
                    _unitOfWork.Facturas.Delete(facturaId);
                    _unitOfWork.SaveChanges();

                    MessageBox.Show("Factura eliminada exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarFacturas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
