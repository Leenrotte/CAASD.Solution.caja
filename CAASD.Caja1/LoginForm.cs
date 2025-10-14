using System;
using System.Windows.Forms;
using CAASD.Core.DTOs;
using CAASD.Core.Interfaces.Services;
using CAASD.Integracion.UnitOfWork;
using CAASD.Integracion.Services;
using CAASD.Caja1.Helpers;

namespace CAASD.Caja1
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService;
        public LoginForm()
        {
            InitializeComponent();
            var unitOfWork = new UnitOfWork();
            _authService = new AuthService(unitOfWork);
            txtPassword.KeyPress += txtPassword_KeyPress;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Por favor complete todos los campos",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var request = new LoginRequestDTO
                {
                    Email = txtEmail.Text.Trim(),
                    Password = txtPassword.Text
                };

                var resultado = _authService.Login(request);

                if (resultado.Success)
                {
                    SessionManager.UsuarioActual = resultado.Usuario;

                    // depurando admin stuff
                    //string debugInfo = $"=== INFORMACIÓN DE DEBUG ===\n\n" +
                    //         $"ID: {resultado.Usuario.Id}\n" +
                    //         $"Nombre: {resultado.Usuario.NombreCompleto}\n" +
                    //         $"Email: {resultado.Usuario.Email}\n" +
                    //         $"Rol: {resultado.Usuario.Rol}\n" +
                    //         $"EsAdministrador (DTO): {resultado.Usuario.EsAdministrador}\n" +
                    //         $"SessionManager.EsAdministrador: {SessionManager.EsAdministrador}\n" +
                    //         $"SessionManager.UsuarioActual != null: {SessionManager.UsuarioActual != null}";

                    //MessageBox.Show(debugInfo, "Debug Login", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // fin de depuracion
                    MessageBox.Show($"Bienvenido {resultado.Usuario.NombreCompleto}",
                        "Login Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    var frmMenu = new MainForm();
                    this.Hide();
                    frmMenu.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(resultado.Mensaje, "Error de Login",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar sesión: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            var frmRegistro = new FrmRegistroUsuario();
            frmRegistro.ShowDialog();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
