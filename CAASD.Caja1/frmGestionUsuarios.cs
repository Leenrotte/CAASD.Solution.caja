using System;
using System.Linq;
using System.Windows.Forms;
using CAASD.Core.Interfaces;
using CAASD.Integracion.UnitOfWork;
using CAASD.Caja1.Helpers;

namespace CAASD.Caja1
{
    public partial class frmGestionUsuarios : Form
    {
        private readonly IUnitOfWork _unitOfWork;
        public frmGestionUsuarios()
        {
            InitializeComponent();
            if (!SessionManager.EsAdministrador)
            {
                MessageBox.Show("Acceso denegado", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            _unitOfWork = new UnitOfWork();
            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            try
            {
                var usuarios = _unitOfWork.Usuarios.GetAll();

                dgvUsuarios.DataSource = usuarios.Select(u => new
                {
                    u.Id,
                    u.Cedula,
                    Nombre = $"{u.Nombre} {u.Apellido}",
                    u.Email,
                    u.Telefono,
                    Rol = u.Rol?.Nombre ?? "Sin rol",
                    Estado = u.Activo ? "Activo" : "Inactivo"
                }).ToList();

                dgvUsuarios.Columns["Id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActivarDesactivar_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un usuario", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int usuarioId = Convert.ToInt32(dgvUsuarios.SelectedRows[0].Cells["Id"].Value);
                var usuario = _unitOfWork.Usuarios.GetById(usuarioId);

                if (usuario != null)
                {
                    usuario.Activo = !usuario.Activo;
                    _unitOfWork.Usuarios.Update(usuario);
                    _unitOfWork.SaveChanges();

                    MessageBox.Show($"Usuario {(usuario.Activo ? "activado" : "desactivado")} exitosamente",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarUsuarios();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarUsuarios();
        }

        private void btnGestionarFacturas_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un usuario", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int usuarioId = Convert.ToInt32(dgvUsuarios.SelectedRows[0].Cells["Id"].Value);
                FrmCrearFactura frmCrearFactura = new FrmCrearFactura(usuarioId);
                frmCrearFactura.ShowDialog();

                MessageBox.Show("Factura creada exitosamente", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
