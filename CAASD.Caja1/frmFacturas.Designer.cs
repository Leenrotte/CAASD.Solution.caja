namespace CAASD.Caja1
{
    partial class frmFacturas
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
            dgvFacturas = new DataGridView();
            lblTotalAdeudado = new Label();
            btnPagar = new Button();
            btnVerDetalle = new Button();
            label1 = new Label();
            btnActualizar = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvFacturas).BeginInit();
            SuspendLayout();
            // 
            // dgvFacturas
            // 
            dgvFacturas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvFacturas.Location = new Point(29, 67);
            dgvFacturas.Name = "dgvFacturas";
            dgvFacturas.RowHeadersWidth = 51;
            dgvFacturas.Size = new Size(1046, 404);
            dgvFacturas.TabIndex = 0;
            // 
            // lblTotalAdeudado
            // 
            lblTotalAdeudado.AutoSize = true;
            lblTotalAdeudado.Location = new Point(29, 501);
            lblTotalAdeudado.Name = "lblTotalAdeudado";
            lblTotalAdeudado.Size = new Size(116, 20);
            lblTotalAdeudado.TabIndex = 1;
            lblTotalAdeudado.Text = "Total Adeudado";
            // 
            // btnPagar
            // 
            btnPagar.Location = new Point(895, 537);
            btnPagar.Name = "btnPagar";
            btnPagar.Size = new Size(180, 68);
            btnPagar.TabIndex = 2;
            btnPagar.Text = "Pagar";
            btnPagar.UseVisualStyleBackColor = true;
            btnPagar.Click += btnPagar_Click;
            // 
            // btnVerDetalle
            // 
            btnVerDetalle.Location = new Point(895, 611);
            btnVerDetalle.Name = "btnVerDetalle";
            btnVerDetalle.Size = new Size(180, 68);
            btnVerDetalle.TabIndex = 3;
            btnVerDetalle.Text = "Detalles";
            btnVerDetalle.UseVisualStyleBackColor = true;
            btnVerDetalle.Click += btnVerDetalle_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(29, 9);
            label1.Name = "label1";
            label1.Size = new Size(152, 46);
            label1.TabIndex = 4;
            label1.Text = "Facturas";
            // 
            // btnActualizar
            // 
            btnActualizar.Location = new Point(981, 477);
            btnActualizar.Name = "btnActualizar";
            btnActualizar.Size = new Size(94, 29);
            btnActualizar.TabIndex = 5;
            btnActualizar.Text = "Actualizar";
            btnActualizar.UseVisualStyleBackColor = true;
            btnActualizar.Click += btnActualizar_Click;
            // 
            // frmFacturas
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1159, 692);
            Controls.Add(btnActualizar);
            Controls.Add(label1);
            Controls.Add(btnVerDetalle);
            Controls.Add(btnPagar);
            Controls.Add(lblTotalAdeudado);
            Controls.Add(dgvFacturas);
            Name = "frmFacturas";
            Text = "Facturas";
            ((System.ComponentModel.ISupportInitialize)dgvFacturas).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvFacturas;
        private Label lblTotalAdeudado;
        private Button btnPagar;
        private Button btnVerDetalle;
        private Label label1;
        private Button btnActualizar;
    }
}