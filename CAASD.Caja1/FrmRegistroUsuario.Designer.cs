namespace CAASD.Caja1
{
    partial class FrmRegistroUsuario
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
            label1 = new Label();
            label2 = new Label();
            txtCedula = new TextBox();
            txtNombre = new TextBox();
            label3 = new Label();
            txtEmail = new TextBox();
            label4 = new Label();
            txtApellido = new TextBox();
            label5 = new Label();
            txtConfirmarPassword = new TextBox();
            label6 = new Label();
            txtPassword = new TextBox();
            label7 = new Label();
            btnRegistrar = new Button();
            btnSalir = new Button();
            txtTelefono = new TextBox();
            label8 = new Label();
            txtDireccion = new TextBox();
            label9 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ActiveCaption;
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.ForeColor = SystemColors.ActiveCaption;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(952, 88);
            panel1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(307, 20);
            label1.Name = "label1";
            label1.Size = new Size(334, 46);
            label1.TabIndex = 0;
            label1.Text = "Registro de Usuario";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(114, 160);
            label2.Name = "label2";
            label2.Size = new Size(67, 23);
            label2.TabIndex = 1;
            label2.Text = "Cedula:";
            // 
            // txtCedula
            // 
            txtCedula.Location = new Point(187, 159);
            txtCedula.Name = "txtCedula";
            txtCedula.PlaceholderText = "000-0000000-0";
            txtCedula.Size = new Size(313, 27);
            txtCedula.TabIndex = 2;
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(187, 202);
            txtNombre.Name = "txtNombre";
            txtNombre.PlaceholderText = "John";
            txtNombre.Size = new Size(313, 27);
            txtNombre.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(104, 202);
            label3.Name = "label3";
            label3.Size = new Size(77, 23);
            label3.TabIndex = 3;
            label3.Text = "Nombre:";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(187, 361);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "platanomaduro@gmail.com";
            txtEmail.Size = new Size(313, 27);
            txtEmail.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(126, 362);
            label4.Name = "label4";
            label4.Size = new Size(55, 23);
            label4.TabIndex = 7;
            label4.Text = "Email:";
            // 
            // txtApellido
            // 
            txtApellido.Location = new Point(187, 244);
            txtApellido.Name = "txtApellido";
            txtApellido.PlaceholderText = "Doe";
            txtApellido.Size = new Size(313, 27);
            txtApellido.TabIndex = 6;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(105, 245);
            label5.Name = "label5";
            label5.Size = new Size(76, 23);
            label5.TabIndex = 5;
            label5.Text = "Apellido:";
            // 
            // txtConfirmarPassword
            // 
            txtConfirmarPassword.Location = new Point(187, 445);
            txtConfirmarPassword.Name = "txtConfirmarPassword";
            txtConfirmarPassword.PasswordChar = '*';
            txtConfirmarPassword.Size = new Size(313, 27);
            txtConfirmarPassword.TabIndex = 12;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(13, 445);
            label6.Name = "label6";
            label6.Size = new Size(168, 23);
            label6.TabIndex = 11;
            label6.Text = "Confirmar Password:";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(187, 402);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(313, 27);
            txtPassword.TabIndex = 10;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.Location = new Point(95, 403);
            label7.Name = "label7";
            label7.Size = new Size(86, 23);
            label7.TabIndex = 9;
            label7.Text = "Password:";
            // 
            // btnRegistrar
            // 
            btnRegistrar.Location = new Point(708, 413);
            btnRegistrar.Name = "btnRegistrar";
            btnRegistrar.Size = new Size(131, 46);
            btnRegistrar.TabIndex = 13;
            btnRegistrar.Text = "Registrar";
            btnRegistrar.UseVisualStyleBackColor = true;
            btnRegistrar.Click += btnRegistrar_Click;
            // 
            // btnSalir
            // 
            btnSalir.Location = new Point(708, 465);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(131, 46);
            btnSalir.TabIndex = 14;
            btnSalir.Text = "Cancelar";
            btnSalir.UseVisualStyleBackColor = true;
            btnSalir.Click += btnSalir_Click;
            // 
            // txtTelefono
            // 
            txtTelefono.Location = new Point(187, 283);
            txtTelefono.Name = "txtTelefono";
            txtTelefono.PlaceholderText = "809-000-0000";
            txtTelefono.Size = new Size(313, 27);
            txtTelefono.TabIndex = 16;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.Location = new Point(105, 284);
            label8.Name = "label8";
            label8.Size = new Size(79, 23);
            label8.TabIndex = 15;
            label8.Text = "Telefono:";
            // 
            // txtDireccion
            // 
            txtDireccion.Location = new Point(187, 321);
            txtDireccion.Name = "txtDireccion";
            txtDireccion.PlaceholderText = "Calle #01";
            txtDireccion.Size = new Size(313, 27);
            txtDireccion.TabIndex = 18;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.Location = new Point(95, 322);
            label9.Name = "label9";
            label9.Size = new Size(85, 23);
            label9.TabIndex = 17;
            label9.Text = "Direccion:";
            // 
            // FrmRegistroUsuario
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(952, 537);
            Controls.Add(txtDireccion);
            Controls.Add(label9);
            Controls.Add(txtTelefono);
            Controls.Add(label8);
            Controls.Add(btnSalir);
            Controls.Add(btnRegistrar);
            Controls.Add(txtConfirmarPassword);
            Controls.Add(label6);
            Controls.Add(txtPassword);
            Controls.Add(label7);
            Controls.Add(txtEmail);
            Controls.Add(label4);
            Controls.Add(txtApellido);
            Controls.Add(label5);
            Controls.Add(txtNombre);
            Controls.Add(label3);
            Controls.Add(txtCedula);
            Controls.Add(label2);
            Controls.Add(panel1);
            Name = "FrmRegistroUsuario";
            Text = "CAJA CAASD";
            Load += FrmRegistroUsuario_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Label label2;
        private TextBox txtCedula;
        private TextBox txtNombre;
        private Label label3;
        private TextBox txtEmail;
        private Label label4;
        private TextBox txtApellido;
        private Label label5;
        private TextBox txtConfirmarPassword;
        private Label label6;
        private TextBox txtPassword;
        private Label label7;
        private Button btnRegistrar;
        private Button btnSalir;
        private TextBox txtTelefono;
        private Label label8;
        private TextBox txtDireccion;
        private Label label9;
    }
}