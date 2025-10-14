using System;
using System.Linq;
using System.Windows.Forms;
using CAASD.Core.Interfaces.Services;
using CAASD.Integracion.UnitOfWork;
using CAASD.Integracion.Services;
using CAASD.Caja1.Helpers;

namespace CAASD.Caja1
{
    public partial class frmFacturas : Form
    {
        private readonly IFacturaService _facturaService;
        private int _usuarioId;
        public frmFacturas()
        {
            InitializeComponent();
            var unitOfWork = new UnitOfWork();
            _facturaService = new FacturaService(unitOfWork);
            _usuarioId = SessionManager.UsuarioActual.Id;

            CargarFacturas();
        }

        private void CargarFacturas()
        {
            try
            {
                var facturas = _facturaService.ObtenerFacturasPorUsuario(_usuarioId);

                dgvFacturas.DataSource = facturas.Select(f => new
                {
                    f.Id,
                    f.NumeroFactura,
                    f.NumeroContrato,
                    f.FechaEmision,
                    f.FechaVencimiento,
                    f.MetrosConsumidos,
                    f.MontoTotal,
                    Estado = f.EstadoPago
                }).ToList();

                // Configurar columnas
                dgvFacturas.Columns["Id"].Visible = false;
                dgvFacturas.Columns["NumeroFactura"].HeaderText = "Nro. Factura";
                dgvFacturas.Columns["NumeroContrato"].HeaderText = "Contrato";
                dgvFacturas.Columns["FechaEmision"].HeaderText = "F. Emisión";
                dgvFacturas.Columns["FechaVencimiento"].HeaderText = "F. Vencimiento";
                dgvFacturas.Columns["MetrosConsumidos"].HeaderText = "M³ Consumidos";
                dgvFacturas.Columns["MontoTotal"].HeaderText = "Total";
                dgvFacturas.Columns["MontoTotal"].DefaultCellStyle.Format = "C2";

                // Calcular total adeudado
                decimal totalAdeudado = _facturaService.ObtenerTotalAdeudado(_usuarioId);
                lblTotalAdeudado.Text = $"Total Adeudado: {totalAdeudado:C2}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar facturas: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarFacturas();
        }

        private void btnPagar_Click(object sender, EventArgs e)
        {
            if (dgvFacturas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una factura", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int facturaId = Convert.ToInt32(dgvFacturas.SelectedRows[0].Cells["Id"].Value);

            var frmPago = new frmPago(facturaId);
            frmPago.ShowDialog();

            CargarFacturas(); // Actualizar después del pago
        }

        private void btnVerDetalle_Click(object sender, EventArgs e)
        {
            if (dgvFacturas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una factura", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string numeroFactura = dgvFacturas.SelectedRows[0].Cells["NumeroFactura"].Value.ToString();
            var factura = _facturaService.ObtenerFacturaPorNumero(numeroFactura);

            if (factura != null)
            {
                string detalle = $"Factura: {factura.NumeroFactura}\n" +
                                $"Contrato: {factura.NumeroContrato}\n" +
                                $"Fecha Emisión: {factura.FechaEmision:dd/MM/yyyy}\n" +
                                $"Vencimiento: {factura.FechaVencimiento:dd/MM/yyyy}\n" +
                                $"Consumo: {factura.MetrosConsumidos} m³\n" +
                                $"Monto Total: {factura.MontoTotal:C2}\n" +
                                $"Estado: {factura.EstadoPago}";

                MessageBox.Show(detalle, "Detalle de Factura",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
