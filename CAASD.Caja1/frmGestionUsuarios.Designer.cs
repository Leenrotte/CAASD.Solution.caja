namespace CAASD.Caja1
{
    partial class frmGestionUsuarios
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
            dgvUsuarios = new DataGridView();
            btnActualizar = new Button();
            btnActivarDesactivar = new Button();
            btnGestionarFacturas = new Button();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvUsuarios).BeginInit();
            SuspendLayout();
            // 
            // dgvUsuarios
            // 
            dgvUsuarios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvUsuarios.Location = new Point(44, 76);
            dgvUsuarios.Name = "dgvUsuarios";
            dgvUsuarios.RowHeadersWidth = 51;
            dgvUsuarios.Size = new Size(1111, 414);
            dgvUsuarios.TabIndex = 0;
            // 
            // btnActualizar
            // 
            btnActualizar.Location = new Point(44, 506);
            btnActualizar.Name = "btnActualizar";
            btnActualizar.Size = new Size(94, 29);
            btnActualizar.TabIndex = 1;
            btnActualizar.Text = "Actualizar";
            btnActualizar.UseVisualStyleBackColor = true;
            btnActualizar.Click += btnActualizar_Click;
            // 
            // btnActivarDesactivar
            // 
            btnActivarDesactivar.Location = new Point(1001, 610);
            btnActivarDesactivar.Name = "btnActivarDesactivar";
            btnActivarDesactivar.Size = new Size(154, 65);
            btnActivarDesactivar.TabIndex = 2;
            btnActivarDesactivar.Text = "Activar/Desactivar";
            btnActivarDesactivar.UseVisualStyleBackColor = true;
            btnActivarDesactivar.Click += btnActivarDesactivar_Click;
            // 
            // btnGestionarFacturas
            // 
            btnGestionarFacturas.Location = new Point(1001, 539);
            btnGestionarFacturas.Name = "btnGestionarFacturas";
            btnGestionarFacturas.Size = new Size(154, 65);
            btnGestionarFacturas.TabIndex = 3;
            btnGestionarFacturas.Text = "Gestionar Facturas";
            btnGestionarFacturas.UseVisualStyleBackColor = true;
            btnGestionarFacturas.Click += btnGestionarFacturas_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(44, 14);
            label1.Name = "label1";
            label1.Size = new Size(338, 46);
            label1.TabIndex = 4;
            label1.Text = "Gestión de Usuarios";
            // 
            // frmGestionUsuarios
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1167, 683);
            Controls.Add(label1);
            Controls.Add(btnGestionarFacturas);
            Controls.Add(btnActivarDesactivar);
            Controls.Add(btnActualizar);
            Controls.Add(dgvUsuarios);
            Name = "frmGestionUsuarios";
            Text = "frmGestionUsuarios";
            ((System.ComponentModel.ISupportInitialize)dgvUsuarios).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvUsuarios;
        private Button btnActualizar;
        private Button btnActivarDesactivar;
        private Button btnGestionarFacturas;
        private Label label1;
    }
}