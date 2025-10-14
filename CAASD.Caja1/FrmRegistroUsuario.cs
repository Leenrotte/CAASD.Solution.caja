using System;
using System.Windows.Forms;
using CAASD.Core.DTOs;
using CAASD.Core.Interfaces.Services;
using CAASD.Integracion.UnitOfWork;
using CAASD.Integracion.Services;

namespace CAASD.Caja1
{
    public partial class FrmRegistroUsuario : Form
    {
        private readonly IAuthService _authService;
        public FrmRegistroUsuario()
        {
            InitializeComponent();
            var unitOfWork = new UnitOfWork();
            _authService = new AuthService(unitOfWork);
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(txtCedula.Text) ||
                    string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtApellido.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Text) ||
                    string.IsNullOrWhiteSpace(txtConfirmarPassword.Text))
                {
                    MessageBox.Show("Por favor complete todos los campos",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtPassword.Text != txtConfirmarPassword.Text)
                {
                    MessageBox.Show("Las contraseñas no coinciden",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var usuario = new UsuarioDTO
                {
                    Cedula = txtCedula.Text.Trim(),
                    NombreCompleto = $"{txtNombre.Text.Trim()} {txtApellido.Text.Trim()}",
                    Email = txtEmail.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    Direccion = txtDireccion.Text.Trim()
                };

                bool resultado = _authService.RegistrarUsuario(usuario, txtPassword.Text);

                if (resultado)
                {
                    MessageBox.Show("Usuario registrado exitosamente",
                        "Registro Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar usuario: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrmRegistroUsuario_Load(object sender, EventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
