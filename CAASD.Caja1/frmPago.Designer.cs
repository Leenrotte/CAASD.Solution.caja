namespace CAASD.Caja1
{
    partial class frmPago
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
            lblblbl = new Label();
            lblFacturaInfo = new Label();
            lblMonto = new Label();
            label1 = new Label();
            cmbMetodoPago = new ComboBox();
            btnProcesarPago = new Button();
            btnCancelar = new Button();
            txtMonto = new TextBox();
            SuspendLayout();
            // 
            // lblblbl
            // 
            lblblbl.AutoSize = true;
            lblblbl.Font = new Font("Segoe UI Semibold", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblblbl.Location = new Point(29, 23);
            lblblbl.Name = "lblblbl";
            lblblbl.Size = new Size(112, 46);
            lblblbl.TabIndex = 0;
            lblblbl.Text = "Pagos";
            // 
            // lblFacturaInfo
            // 
            lblFacturaInfo.AutoSize = true;
            lblFacturaInfo.Location = new Point(577, 106);
            lblFacturaInfo.Name = "lblFacturaInfo";
            lblFacturaInfo.Size = new Size(161, 20);
            lblFacturaInfo.TabIndex = 1;
            lblFacturaInfo.Text = "Informacion de Factura";
            // 
            // lblMonto
            // 
            lblMonto.AutoSize = true;
            lblMonto.Location = new Point(32, 153);
            lblMonto.Name = "lblMonto";
            lblMonto.Size = new Size(109, 20);
            lblMonto.TabIndex = 2;
            lblMonto.Text = "Monto a Pagar:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 200);
            label1.Name = "label1";
            label1.Size = new Size(123, 20);
            label1.TabIndex = 3;
            label1.Text = "Metodo de Pago:";
            // 
            // cmbMetodoPago
            // 
            cmbMetodoPago.FormattingEnabled = true;
            cmbMetodoPago.Location = new Point(169, 197);
            cmbMetodoPago.Name = "cmbMetodoPago";
            cmbMetodoPago.Size = new Size(248, 28);
            cmbMetodoPago.TabIndex = 4;
            // 
            // btnProcesarPago
            // 
            btnProcesarPago.Location = new Point(815, 425);
            btnProcesarPago.Name = "btnProcesarPago";
            btnProcesarPago.Size = new Size(153, 65);
            btnProcesarPago.TabIndex = 5;
            btnProcesarPago.Text = "Pagar";
            btnProcesarPago.UseVisualStyleBackColor = true;
            btnProcesarPago.Click += btnProcesarPago_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(815, 496);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(153, 65);
            btnCancelar.TabIndex = 6;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // txtMonto
            // 
            txtMonto.Location = new Point(169, 150);
            txtMonto.Name = "txtMonto";
            txtMonto.Size = new Size(248, 27);
            txtMonto.TabIndex = 7;
            // 
            // frmPago
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(999, 584);
            Controls.Add(txtMonto);
            Controls.Add(btnCancelar);
            Controls.Add(btnProcesarPago);
            Controls.Add(cmbMetodoPago);
            Controls.Add(label1);
            Controls.Add(lblMonto);
            Controls.Add(lblFacturaInfo);
            Controls.Add(lblblbl);
            Name = "frmPago";
            Text = "Pagos";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblblbl;
        private Label lblFacturaInfo;
        private Label lblMonto;
        private Label label1;
        private ComboBox cmbMetodoPago;
        private Button btnProcesarPago;
        private Button btnCancelar;
        private TextBox txtMonto;
    }
}