namespace SistemaBase.Usuario
{
    partial class FormCambiarIdioma_56PS
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
            this.cambioIdioma = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cambioIdioma
            // 
            this.cambioIdioma.AutoSize = true;
            this.cambioIdioma.Font = new System.Drawing.Font("Microsoft Sans Serif", 38.25F);
            this.cambioIdioma.Location = new System.Drawing.Point(196, 74);
            this.cambioIdioma.Name = "cambioIdioma";
            this.cambioIdioma.Size = new System.Drawing.Size(431, 59);
            this.cambioIdioma.TabIndex = 5;
            this.cambioIdioma.Tag = "cambioIdioma";
            this.cambioIdioma.Text = "Cambio de idioma";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(94, 276);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Tag = "AplicarButton";
            this.button1.Text = "Aplicar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "ES",
            "EN",
            "POR"});
            this.comboBox1.Location = new System.Drawing.Point(94, 222);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 3;
            // 
            // FormCambiarIdioma_56PS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cambioIdioma);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox1);
            this.Name = "FormCambiarIdioma_56PS";
            this.Text = "FormCambiarIdioma_56PS";
            this.Load += new System.EventHandler(this.FormCambiarIdioma_56PS_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label cambioIdioma;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}