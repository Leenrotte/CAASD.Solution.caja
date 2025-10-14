using System;
using System.Windows.Forms;
using CAASD.Core.Interfaces.Services;
using CAASD.Integracion.UnitOfWork;
using CAASD.Integracion.Services;

namespace CAASD.Caja1
{
    public partial class frmPago : Form
    {
        private readonly IPagoService _pagoService;
        private readonly IFacturaService _facturaService;
        private readonly int _facturaId;
        public frmPago(int facturaId = 0)
        {
            InitializeComponent();
            var unitOfWork = new UnitOfWork();
            _pagoService = new PagoService(unitOfWork);
            _facturaService = new FacturaService(unitOfWork);
            _facturaId = facturaId;

            CargarMetodosPago();

            if (_facturaId > 0)
            {
                CargarDatosFactura();
            }
        }
        private void CargarMetodosPago()
        {
            cmbMetodoPago.Items.AddRange(new object[]
            {
                "Efectivo",
                "Tarjeta de Crédito",
                "Tarjeta de Débito",
                "Transferencia"
            });
            cmbMetodoPago.SelectedIndex = 0;
        }

        private void CargarDatosFactura()
        {
            try
            {
                // Aquí cargarías los datos de la factura
                // Por simplicidad, usamos valores de ejemplo
                lblFacturaInfo.Text = $"Factura ID: {_facturaId}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnProcesarPago_Click(object sender, EventArgs e)
        {
            try
            {
                if (_facturaId == 0)
                {
                    MessageBox.Show("No se ha seleccionado una factura",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtMonto.Text))
                {
                    MessageBox.Show("Ingrese el monto a pagar",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal monto = decimal.Parse(txtMonto.Text);
                string metodoPago = cmbMetodoPago.SelectedItem.ToString();

                // Simular procesamiento
                var resultado = DialogResult.Yes == MessageBox.Show(
                    $"¿Confirma el pago de {monto:C2} mediante {metodoPago}?",
                    "Confirmar Pago",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado)
                {
                    bool pagoExitoso = _pagoService.ProcesarPago(_facturaId, monto, metodoPago);

                    if (pagoExitoso)
                    {
                        MessageBox.Show("Pago procesado exitosamente",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Error al procesar el pago",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
