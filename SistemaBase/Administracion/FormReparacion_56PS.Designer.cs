namespace SistemaBase.Administracion
{
    partial class FormReparacion_56PS
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
            this.tablas = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnsalir = new System.Windows.Forms.Button();
            this.btnrestore = new System.Windows.Forms.Button();
            this.btnrecalcular = new System.Windows.Forms.Button();
            this.lblinc = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tablas
            // 
            this.tablas.AutoSize = true;
            this.tablas.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F);
            this.tablas.Location = new System.Drawing.Point(397, 120);
            this.tablas.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.tablas.Name = "tablas";
            this.tablas.Size = new System.Drawing.Size(160, 21);
            this.tablas.TabIndex = 207;
            this.tablas.Tag = "tablasInconsistentes";
            this.tablas.Text = "Tablas inconsistentes:";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(401, 155);
            this.listBox1.Margin = new System.Windows.Forms.Padding(2);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(182, 251);
            this.listBox1.TabIndex = 206;
            // 
            // btnsalir
            // 
            this.btnsalir.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnsalir.Location = new System.Drawing.Point(200, 322);
            this.btnsalir.Margin = new System.Windows.Forms.Padding(2);
            this.btnsalir.Name = "btnsalir";
            this.btnsalir.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnsalir.Size = new System.Drawing.Size(147, 33);
            this.btnsalir.TabIndex = 205;
            this.btnsalir.Tag = "salirButton";
            this.btnsalir.Text = "Salir";
            this.btnsalir.UseVisualStyleBackColor = true;
            this.btnsalir.Click += new System.EventHandler(this.btnsalir_Click);
            // 
            // btnrestore
            // 
            this.btnrestore.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnrestore.Location = new System.Drawing.Point(200, 262);
            this.btnrestore.Margin = new System.Windows.Forms.Padding(2);
            this.btnrestore.Name = "btnrestore";
            this.btnrestore.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnrestore.Size = new System.Drawing.Size(147, 33);
            this.btnrestore.TabIndex = 204;
            this.btnrestore.Tag = "restoreBD";
            this.btnrestore.Text = "Restore BD";
            this.btnrestore.UseVisualStyleBackColor = true;
            this.btnrestore.Click += new System.EventHandler(this.btnrestore_Click);
            // 
            // btnrecalcular
            // 
            this.btnrecalcular.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnrecalcular.Location = new System.Drawing.Point(200, 205);
            this.btnrecalcular.Margin = new System.Windows.Forms.Padding(2);
            this.btnrecalcular.Name = "btnrecalcular";
            this.btnrecalcular.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnrecalcular.Size = new System.Drawing.Size(147, 33);
            this.btnrecalcular.TabIndex = 203;
            this.btnrecalcular.Tag = "recalcularDV";
            this.btnrecalcular.Text = "Recalcular DV";
            this.btnrecalcular.UseVisualStyleBackColor = true;
            this.btnrecalcular.Click += new System.EventHandler(this.btnrecalcular_Click);
            // 
            // lblinc
            // 
            this.lblinc.AutoSize = true;
            this.lblinc.Font = new System.Drawing.Font("Segoe UI Variable Display", 18F);
            this.lblinc.Location = new System.Drawing.Point(179, 45);
            this.lblinc.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblinc.Name = "lblinc";
            this.lblinc.Size = new System.Drawing.Size(442, 32);
            this.lblinc.TabIndex = 202;
            this.lblinc.Tag = "avisoWarning";
            this.lblinc.Text = "INCONSISTENCIA EN LA BASE DE DATOS";
            // 
            // FormReparacion_56PS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tablas);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnsalir);
            this.Controls.Add(this.btnrestore);
            this.Controls.Add(this.btnrecalcular);
            this.Controls.Add(this.lblinc);
            this.Name = "FormReparacion_56PS";
            this.Text = "FormReparacion_56PS";
            this.Load += new System.EventHandler(this.FormReparacion_56PS_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label tablas;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnsalir;
        private System.Windows.Forms.Button btnrestore;
        private System.Windows.Forms.Button btnrecalcular;
        private System.Windows.Forms.Label lblinc;
    }
}
