namespace SistemaBase.Administracion
{
    partial class FormPerfiles_56PS
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
            this.button6 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.button4 = new System.Windows.Forms.Button();
            this.cmbfa = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.cmbpermiso = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.btncancelar = new System.Windows.Forms.Button();
            this.btnbuscar = new System.Windows.Forms.Button();
            this.lblperfiles = new System.Windows.Forms.Label();
            this.lblnombre = new System.Windows.Forms.Label();
            this.txtnomb = new System.Windows.Forms.TextBox();
            this.lblcod = new System.Windows.Forms.Label();
            this.txtcod = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.Location = new System.Drawing.Point(56, 130);
            this.button6.Margin = new System.Windows.Forms.Padding(2);
            this.button6.Name = "button6";
            this.button6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button6.Size = new System.Drawing.Size(277, 32);
            this.button6.TabIndex = 162;
            this.button6.Tag = "eliminarPermiso/familiaDePerfil";
            this.button6.Text = "Eliminar permiso/familia de perfil";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Variable Display", 10F);
            this.label1.Location = new System.Drawing.Point(911, -25);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 19);
            this.label1.TabIndex = 161;
            this.label1.Text = "Todos los perfiles";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(859, 24);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(434, 310);
            this.dataGridView1.TabIndex = 160;
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(370, 24);
            this.treeView1.Margin = new System.Windows.Forms.Padding(2);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(444, 311);
            this.treeView1.TabIndex = 159;
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(56, 92);
            this.button4.Margin = new System.Windows.Forms.Padding(2);
            this.button4.Name = "button4";
            this.button4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button4.Size = new System.Drawing.Size(277, 33);
            this.button4.TabIndex = 158;
            this.button4.Tag = "cancelar";
            this.button4.Text = "Cancelar";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // cmbfa
            // 
            this.cmbfa.FormattingEnabled = true;
            this.cmbfa.Location = new System.Drawing.Point(316, 505);
            this.cmbfa.Margin = new System.Windows.Forms.Padding(2);
            this.cmbfa.Name = "cmbfa";
            this.cmbfa.Size = new System.Drawing.Size(204, 21);
            this.cmbfa.TabIndex = 157;
            this.cmbfa.SelectedIndexChanged += new System.EventHandler(this.cmbfa_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(316, 450);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button2.Size = new System.Drawing.Size(202, 33);
            this.button2.TabIndex = 156;
            this.button2.Tag = "asignarFamilia";
            this.button2.Text = "Asignar familia";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cmbpermiso
            // 
            this.cmbpermiso.FormattingEnabled = true;
            this.cmbpermiso.Location = new System.Drawing.Point(39, 505);
            this.cmbpermiso.Margin = new System.Windows.Forms.Padding(2);
            this.cmbpermiso.Name = "cmbpermiso";
            this.cmbpermiso.Size = new System.Drawing.Size(204, 21);
            this.cmbpermiso.TabIndex = 155;
            this.cmbpermiso.SelectedIndexChanged += new System.EventHandler(this.cmbpermiso_SelectedIndexChanged);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(39, 450);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button3.Size = new System.Drawing.Size(202, 33);
            this.button3.TabIndex = 154;
            this.button3.Tag = "asignarPermiso";
            this.button3.Text = "Asignar permiso";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // btncancelar
            // 
            this.btncancelar.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btncancelar.Location = new System.Drawing.Point(56, 170);
            this.btncancelar.Margin = new System.Windows.Forms.Padding(2);
            this.btncancelar.Name = "btncancelar";
            this.btncancelar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btncancelar.Size = new System.Drawing.Size(277, 33);
            this.btncancelar.TabIndex = 151;
            this.btncancelar.Tag = "eliminarPerfil";
            this.btncancelar.Text = "Eliminar Perfil";
            this.btncancelar.UseVisualStyleBackColor = true;
            this.btncancelar.Click += new System.EventHandler(this.btncancelar_Click);
            // 
            // btnbuscar
            // 
            this.btnbuscar.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnbuscar.Location = new System.Drawing.Point(56, 214);
            this.btnbuscar.Margin = new System.Windows.Forms.Padding(2);
            this.btnbuscar.Name = "btnbuscar";
            this.btnbuscar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnbuscar.Size = new System.Drawing.Size(277, 33);
            this.btnbuscar.TabIndex = 150;
            this.btnbuscar.Tag = "crearPerfil";
            this.btnbuscar.Text = "Crear Perfil";
            this.btnbuscar.UseVisualStyleBackColor = true;
            this.btnbuscar.Click += new System.EventHandler(this.btnbuscar_Click);
            // 
            // lblperfiles
            // 
            this.lblperfiles.AutoSize = true;
            this.lblperfiles.Font = new System.Drawing.Font("Segoe UI Variable Display", 20F);
            this.lblperfiles.Location = new System.Drawing.Point(255, -62);
            this.lblperfiles.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblperfiles.Name = "lblperfiles";
            this.lblperfiles.Size = new System.Drawing.Size(99, 36);
            this.lblperfiles.TabIndex = 147;
            this.lblperfiles.Text = "Perfiles";
            // 
            // lblnombre
            // 
            this.lblnombre.AutoSize = true;
            this.lblnombre.Font = new System.Drawing.Font("Segoe UI Variable Display", 10F);
            this.lblnombre.Location = new System.Drawing.Point(225, 346);
            this.lblnombre.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblnombre.Name = "lblnombre";
            this.lblnombre.Size = new System.Drawing.Size(59, 19);
            this.lblnombre.TabIndex = 166;
            this.lblnombre.Text = "Nombre";
            // 
            // txtnomb
            // 
            this.txtnomb.Location = new System.Drawing.Point(56, 348);
            this.txtnomb.Margin = new System.Windows.Forms.Padding(2);
            this.txtnomb.Name = "txtnomb";
            this.txtnomb.Size = new System.Drawing.Size(158, 20);
            this.txtnomb.TabIndex = 165;
            // 
            // lblcod
            // 
            this.lblcod.AutoSize = true;
            this.lblcod.Font = new System.Drawing.Font("Segoe UI Variable Display", 10F);
            this.lblcod.Location = new System.Drawing.Point(225, 314);
            this.lblcod.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblcod.Name = "lblcod";
            this.lblcod.Size = new System.Drawing.Size(54, 19);
            this.lblcod.TabIndex = 164;
            this.lblcod.Text = "Código";
            // 
            // txtcod
            // 
            this.txtcod.Location = new System.Drawing.Point(56, 315);
            this.txtcod.Margin = new System.Windows.Forms.Padding(2);
            this.txtcod.Name = "txtcod";
            this.txtcod.Size = new System.Drawing.Size(158, 20);
            this.txtcod.TabIndex = 163;
            // 
            // FormPerfiles_56PS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 589);
            this.Controls.Add(this.lblnombre);
            this.Controls.Add(this.txtnomb);
            this.Controls.Add(this.lblcod);
            this.Controls.Add(this.txtcod);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.cmbfa);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.cmbpermiso);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btncancelar);
            this.Controls.Add(this.btnbuscar);
            this.Controls.Add(this.lblperfiles);
            this.Name = "FormPerfiles_56PS";
            this.Text = "FormPerfiles_56PS";
            this.Load += new System.EventHandler(this.FormPerfiles_56PS_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ComboBox cmbfa;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox cmbpermiso;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btncancelar;
        private System.Windows.Forms.Button btnbuscar;
        private System.Windows.Forms.Label lblperfiles;
        private System.Windows.Forms.Label lblnombre;
        private System.Windows.Forms.TextBox txtnomb;
        private System.Windows.Forms.Label lblcod;
        private System.Windows.Forms.TextBox txtcod;
    }
}