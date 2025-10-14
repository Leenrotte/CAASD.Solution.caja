namespace CAASD.Caja1
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            lblRol = new Label();
            lblUsuario = new Label();
            panel2 = new Panel();
            btnCerrarSesion = new Button();
            btnCuadre = new Button();
            btnGestionUsuarios = new Button();
            btnReportes = new Button();
            btnCobro = new Button();
            btnPagos = new Button();
            btnFacturas = new Button();
            MainPanel = new Panel();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ActiveCaption;
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(lblRol);
            panel1.Controls.Add(lblUsuario);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1093, 86);
            panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.logo_caasd;
            pictureBox1.Location = new Point(396, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(451, 83);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // lblRol
            // 
            lblRol.AutoSize = true;
            lblRol.Location = new Point(25, 32);
            lblRol.Name = "lblRol";
            lblRol.Size = new Size(35, 23);
            lblRol.TabIndex = 1;
            lblRol.Text = "Rol";
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(25, 9);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(68, 23);
            lblUsuario.TabIndex = 0;
            lblUsuario.Text = "Usuario";
            // 
            // panel2
            // 
            panel2.AutoScroll = true;
            panel2.BackColor = SystemColors.ActiveCaption;
            panel2.Controls.Add(btnCerrarSesion);
            panel2.Controls.Add(btnCuadre);
            panel2.Controls.Add(btnGestionUsuarios);
            panel2.Controls.Add(btnReportes);
            panel2.Controls.Add(btnCobro);
            panel2.Controls.Add(btnPagos);
            panel2.Controls.Add(btnFacturas);
            panel2.Dock = DockStyle.Left;
            panel2.Location = new Point(0, 86);
            panel2.Name = "panel2";
            panel2.Size = new Size(201, 536);
            panel2.TabIndex = 1;
            // 
            // btnCerrarSesion
            // 
            btnCerrarSesion.Dock = DockStyle.Top;
            btnCerrarSesion.Location = new Point(0, 372);
            btnCerrarSesion.Name = "btnCerrarSesion";
            btnCerrarSesion.Size = new Size(201, 62);
            btnCerrarSesion.TabIndex = 6;
            btnCerrarSesion.Text = "Cerrar Sesion";
            btnCerrarSesion.UseVisualStyleBackColor = true;
            btnCerrarSesion.Click += btnCerrarSesion_Click;
            // 
            // btnCuadre
            // 
            btnCuadre.Dock = DockStyle.Top;
            btnCuadre.Location = new Point(0, 310);
            btnCuadre.Name = "btnCuadre";
            btnCuadre.Size = new Size(201, 62);
            btnCuadre.TabIndex = 5;
            btnCuadre.Text = "Cuadre";
            btnCuadre.UseVisualStyleBackColor = true;
            // 
            // btnGestionUsuarios
            // 
            btnGestionUsuarios.Dock = DockStyle.Top;
            btnGestionUsuarios.Location = new Point(0, 248);
            btnGestionUsuarios.Name = "btnGestionUsuarios";
            btnGestionUsuarios.Size = new Size(201, 62);
            btnGestionUsuarios.TabIndex = 4;
            btnGestionUsuarios.Text = "Gestion de Usuarios";
            btnGestionUsuarios.UseVisualStyleBackColor = true;
            btnGestionUsuarios.Click += btnGestionUsuarios_Click;
            // 
            // btnReportes
            // 
            btnReportes.Dock = DockStyle.Top;
            btnReportes.Location = new Point(0, 186);
            btnReportes.Name = "btnReportes";
            btnReportes.Size = new Size(201, 62);
            btnReportes.TabIndex = 3;
            btnReportes.Text = "Reportes";
            btnReportes.UseVisualStyleBackColor = true;
            btnReportes.Click += btnReportes_Click;
            // 
            // btnCobro
            // 
            btnCobro.Dock = DockStyle.Top;
            btnCobro.Location = new Point(0, 124);
            btnCobro.Name = "btnCobro";
            btnCobro.Size = new Size(201, 62);
            btnCobro.TabIndex = 2;
            btnCobro.Text = "Cobro";
            btnCobro.UseVisualStyleBackColor = true;
            btnCobro.Click += btnCobro_Click;
            // 
            // btnPagos
            // 
            btnPagos.Dock = DockStyle.Top;
            btnPagos.Location = new Point(0, 62);
            btnPagos.Name = "btnPagos";
            btnPagos.Size = new Size(201, 62);
            btnPagos.TabIndex = 1;
            btnPagos.Text = "Pagos";
            btnPagos.UseVisualStyleBackColor = true;
            btnPagos.Click += btnPagos_Click;
            // 
            // btnFacturas
            // 
            btnFacturas.Dock = DockStyle.Top;
            btnFacturas.Location = new Point(0, 0);
            btnFacturas.Name = "btnFacturas";
            btnFacturas.Size = new Size(201, 62);
            btnFacturas.TabIndex = 0;
            btnFacturas.Text = "Facturas";
            btnFacturas.UseVisualStyleBackColor = true;
            btnFacturas.Click += btnFacturas_Click;
            // 
            // MainPanel
            // 
            MainPanel.BackColor = SystemColors.ControlDark;
            MainPanel.Dock = DockStyle.Fill;
            MainPanel.Location = new Point(201, 86);
            MainPanel.Name = "MainPanel";
            MainPanel.Size = new Size(892, 536);
            MainPanel.TabIndex = 2;
            MainPanel.Paint += MainPanel_Paint;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1093, 622);
            Controls.Add(MainPanel);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Margin = new Padding(4, 3, 4, 3);
            Name = "MainForm";
            Text = "CAJA CAASD";
            WindowState = FormWindowState.Maximized;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Button btnCuadre;
        private Button btnGestionUsuarios;
        private Button btnReportes;
        private Button btnCobro;
        private Button btnPagos;
        private Button btnFacturas;
        private PictureBox pictureBox1;
        private Label lblRol;
        private Label lblUsuario;
        private Button btnCerrarSesion;
        private Panel MainPanel;
    }
}