namespace CAASD.Caja1
{
    partial class FrmCrearFactura
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
            label1 = new Label();
            label2 = new Label();
            cmbContrato = new ComboBox();
            numMetrosConsumidos = new NumericUpDown();
            label3 = new Label();
            label4 = new Label();
            dtpFechaEmision = new DateTimePicker();
            panel1 = new Panel();
            lblVistaPrevia = new Label();
            btnCrearNueva = new Button();
            btnCancelar = new Button();
            dgvFacturas = new DataGridView();
            grpDatosFactura = new GroupBox();
            btnRectificar = new Button();
            btnEliminar = new Button();
            btnGuardar = new Button();
            btnActualizar = new Button();
            ((System.ComponentModel.ISupportInitialize)numMetrosConsumidos).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvFacturas).BeginInit();
            grpDatosFactura.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(331, 46);
            label1.TabIndex = 5;
            label1.Text = "Gestión de Facturas";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(77, 47);
            label2.Name = "label2";
            label2.Size = new Size(70, 20);
            label2.TabIndex = 6;
            label2.Text = "Contrato:";
            // 
            // cmbContrato
            // 
            cmbContrato.FormattingEnabled = true;
            cmbContrato.Location = new Point(153, 44);
            cmbContrato.Name = "cmbContrato";
            cmbContrato.Size = new Size(222, 28);
            cmbContrato.TabIndex = 7;
            // 
            // numMetrosConsumidos
            // 
            numMetrosConsumidos.Location = new Point(153, 78);
            numMetrosConsumidos.Name = "numMetrosConsumidos";
            numMetrosConsumidos.Size = new Size(222, 27);
            numMetrosConsumidos.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(4, 80);
            label3.Name = "label3";
            label3.Size = new Size(143, 20);
            label3.TabIndex = 9;
            label3.Text = "Metros Consumidos:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(20, 114);
            label4.Name = "label4";
            label4.Size = new Size(127, 20);
            label4.TabIndex = 10;
            label4.Text = "Fecha de Emisión:";
            // 
            // dtpFechaEmision
            // 
            dtpFechaEmision.Location = new Point(153, 111);
            dtpFechaEmision.Name = "dtpFechaEmision";
            dtpFechaEmision.Size = new Size(222, 27);
            dtpFechaEmision.TabIndex = 11;
            // 
            // panel1
            // 
            panel1.Controls.Add(lblVistaPrevia);
            panel1.Location = new Point(824, 89);
            panel1.Name = "panel1";
            panel1.Size = new Size(328, 261);
            panel1.TabIndex = 12;
            // 
            // lblVistaPrevia
            // 
            lblVistaPrevia.AutoSize = true;
            lblVistaPrevia.Dock = DockStyle.Fill;
            lblVistaPrevia.Font = new Font("Segoe UI Semibold", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblVistaPrevia.Location = new Point(0, 0);
            lblVistaPrevia.Name = "lblVistaPrevia";
            lblVistaPrevia.Size = new Size(109, 25);
            lblVistaPrevia.TabIndex = 0;
            lblVistaPrevia.Text = "Vista Previa";
            // 
            // btnCrearNueva
            // 
            btnCrearNueva.Location = new Point(417, 452);
            btnCrearNueva.Name = "btnCrearNueva";
            btnCrearNueva.Size = new Size(167, 63);
            btnCrearNueva.TabIndex = 13;
            btnCrearNueva.Text = "Crear Factura";
            btnCrearNueva.UseVisualStyleBackColor = true;
            btnCrearNueva.Click += btnCrearNueva_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(985, 604);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(167, 63);
            btnCancelar.TabIndex = 14;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // dgvFacturas
            // 
            dgvFacturas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvFacturas.Location = new Point(23, 72);
            dgvFacturas.Name = "dgvFacturas";
            dgvFacturas.RowHeadersWidth = 51;
            dgvFacturas.Size = new Size(795, 344);
            dgvFacturas.TabIndex = 15;
            // 
            // grpDatosFactura
            // 
            grpDatosFactura.Controls.Add(numMetrosConsumidos);
            grpDatosFactura.Controls.Add(label2);
            grpDatosFactura.Controls.Add(cmbContrato);
            grpDatosFactura.Controls.Add(label3);
            grpDatosFactura.Controls.Add(label4);
            grpDatosFactura.Controls.Add(dtpFechaEmision);
            grpDatosFactura.Location = new Point(23, 443);
            grpDatosFactura.Name = "grpDatosFactura";
            grpDatosFactura.Size = new Size(388, 206);
            grpDatosFactura.TabIndex = 16;
            grpDatosFactura.TabStop = false;
            grpDatosFactura.Text = "Datos de Factura";
            // 
            // btnRectificar
            // 
            btnRectificar.Location = new Point(417, 521);
            btnRectificar.Name = "btnRectificar";
            btnRectificar.Size = new Size(167, 63);
            btnRectificar.TabIndex = 17;
            btnRectificar.Text = "Rectificar Factura";
            btnRectificar.UseVisualStyleBackColor = true;
            btnRectificar.Click += btnRectificar_Click;
            // 
            // btnEliminar
            // 
            btnEliminar.Location = new Point(417, 590);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(167, 63);
            btnEliminar.TabIndex = 18;
            btnEliminar.Text = "Eliminar Factura";
            btnEliminar.UseVisualStyleBackColor = true;
            btnEliminar.Click += btnEliminar_Click;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(985, 536);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(167, 63);
            btnGuardar.TabIndex = 19;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnActualizar
            // 
            btnActualizar.Location = new Point(731, 422);
            btnActualizar.Name = "btnActualizar";
            btnActualizar.Size = new Size(87, 34);
            btnActualizar.TabIndex = 20;
            btnActualizar.Text = "Actualizar";
            btnActualizar.UseVisualStyleBackColor = true;
            // 
            // FrmCrearFactura
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1164, 688);
            Controls.Add(btnActualizar);
            Controls.Add(btnGuardar);
            Controls.Add(btnEliminar);
            Controls.Add(btnRectificar);
            Controls.Add(grpDatosFactura);
            Controls.Add(dgvFacturas);
            Controls.Add(btnCancelar);
            Controls.Add(btnCrearNueva);
            Controls.Add(panel1);
            Controls.Add(label1);
            Name = "FrmCrearFactura";
            Text = "FrmCrearFactura";
            ((System.ComponentModel.ISupportInitialize)numMetrosConsumidos).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvFacturas).EndInit();
            grpDatosFactura.ResumeLayout(false);
            grpDatosFactura.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private ComboBox cmbContrato;
        private NumericUpDown numMetrosConsumidos;
        private Label label3;
        private Label label4;
        private DateTimePicker dtpFechaEmision;
        private Panel panel1;
        private Label lblVistaPrevia;
        private Button btnCrearNueva;
        private Button btnCancelar;
        private DataGridView dgvFacturas;
        private GroupBox grpDatosFactura;
        private Button btnRectificar;
        private Button btnEliminar;
        private Button btnGuardar;
        private Button btnActualizar;
    }
}