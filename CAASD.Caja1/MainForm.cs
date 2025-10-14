using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CAASD.Caja1.Helpers;

namespace CAASD.Caja1
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            ConfigurarMenu();
        }

        private void ConfigurarMenu()
        {
            lblUsuario.Text = $"Usuario: {SessionManager.UsuarioActual.NombreCompleto}";
            lblRol.Text = $"Rol: {SessionManager.UsuarioActual.Rol}";

            if (!SessionManager.EsAdministrador)
            {
                btnGestionUsuarios.Visible = false;
                btnReportes.Visible = false;
            }
        }

        private void facturasToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {

        }
        private void OpenFormInPanel(Form formToOpen)
        {
            MainPanel.Controls.Clear();

            formToOpen.TopLevel = false;
            formToOpen.FormBorderStyle = FormBorderStyle.None;
            formToOpen.Dock = DockStyle.Fill;

            MainPanel.Controls.Add(formToOpen);
            formToOpen.Show();
        }

        private void btnFacturas_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new frmFacturas());
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            SessionManager.CerrarSesion();
            this.Close();
        }

        private void btnPagos_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new frmPago());
        }

        private void btnCobro_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new frmCobro());
        }

        private void btnReportes_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new frmAverias());
        }

        private void btnGestionUsuarios_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new frmGestionUsuarios());
        }
    }
}
