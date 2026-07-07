using BE_625NS;
using BLL;
using ClassLibrary2;
using ClassLibrary3;
using Servicio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaBase.Administracion
{
    public partial class FormGestionBackUpRestore_56PS : Form, IdiomaObserver_56PS
    {
        public void actualizarIdioma()
        {
            var traductor = new BLL_Idioma_56PS();
            traductor.Traducir(this);
        }

        public FormGestionBackUpRestore_56PS()
        {
            InitializeComponent();

            SessionManager_56PS.getInstancia().Suscribir(this);
            actualizarIdioma();
        }

        private void FormGestionBackUpRestore_56PS_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BLL_BackUpRestore_56PS bll = new BLL_BackUpRestore_56PS();

            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un item");
                return;
            }

            if (comboBox1.SelectedItem.ToString() == "Backup")
            {
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    try
                    {
                        bll.RealizarBackup(textBox1.Text);
                        MessageBox.Show("Backup realizado con éxito.");
                        textBox1.Text = "";

                        string a = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;

                        Evento_56PS ee = new Evento_56PS(a, DateTime.Now, "Base de datos", "Realizacion de backup", Evento_56PS.Criticidad.Alto);
                        new BLL_BitacoraEvento_56PS().RegistrarEvento(ee);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al realizar el backup: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione una ubicación para el backup.");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    try
                    {
                        bll.RealizarRestore(textBox1.Text);
                        MessageBox.Show("Restauración realizada con éxito.");

                        string a = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;

                        Evento_56PS ee = new Evento_56PS(a, DateTime.Now, "Base de datos", "Realizacion de restore", Evento_56PS.Criticidad.Alto);
                        new BLL_BitacoraEvento_56PS().RegistrarEvento(ee);
                        textBox1.Text = "";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al restaurar la base de datos: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione un archivo de restore.");
                }
            }
        }
    }
}
