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
            button1.Enabled = TieneAlgunPermiso(Permisos_56PS.RealizarBackup, Permisos_56PS.RestaurarBackup);
        }

        private void FormGestionBackUpRestore_56PS_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                new BLL_Idioma_56PS().MostrarMensaje("Selecciona Backup o Restore antes de buscar.");
                return;
            }

            if (comboBox1.SelectedItem.ToString() == "Backup")
            {
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        textBox1.Text = folderDialog.SelectedPath;
                    }
                }
            }
            else
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Archivos de respaldo (*.bak)|*.bak|Todos los archivos (*.*)|*.*";
                    openFileDialog.Title = "Seleccionar archivo de respaldo";
                    openFileDialog.CheckFileExists = true;
                    openFileDialog.CheckPathExists = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        textBox1.Text = openFileDialog.FileName;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BLL_BackUpRestore_56PS bll = new BLL_BackUpRestore_56PS();

            if (comboBox1.SelectedItem == null)
            {
                new BLL_Idioma_56PS().MostrarMensaje("Selecciona un item");
                return;
            }

            if (comboBox1.SelectedItem.ToString() == "Backup")
            {
                if (!TienePermiso(Permisos_56PS.RealizarBackup))
                {
                    new BLL_Idioma_56PS().MostrarMensaje("No tiene permiso para realizar backup.");
                    return;
                }

                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    try
                    {
                        bll.RealizarBackup(textBox1.Text);
                        new BLL_Idioma_56PS().MostrarMensaje("Backup realizado con éxito.");
                        textBox1.Text = "";

                        string a = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;

                        Evento_56PS ee = new Evento_56PS(a, DateTime.Now, "Base de datos", "Realizacion de backup", Evento_56PS.Criticidad.Alto);
                        new BLL_BitacoraEvento_56PS().RegistrarEvento(ee);
                    }
                    catch (Exception ex)
                    {
                        new BLL_Idioma_56PS().MostrarMensaje($"Error al realizar el backup: {ex.Message}");
                    }
                }
                else
                {
                    new BLL_Idioma_56PS().MostrarMensaje("Seleccione una ubicación para el backup.");
                }
            }
            else
            {
                if (!TienePermiso(Permisos_56PS.RestaurarBackup))
                {
                    new BLL_Idioma_56PS().MostrarMensaje("No tiene permiso para realizar restore.");
                    return;
                }

                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    try
                    {
                        string dniUsuario = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
                        bll.RealizarRestore(textBox1.Text);
                        new BLL_Idioma_56PS().MostrarMensaje("Restauración realizada con éxito. El sistema se cerrará para volver a iniciar con la base restaurada.");

                        try
                        {
                            Evento_56PS ee = new Evento_56PS(dniUsuario, DateTime.Now, "Base de datos", "Realizacion de restore", Evento_56PS.Criticidad.Alto);
                            new BLL_BitacoraEvento_56PS().RegistrarEvento(ee);
                        }
                        catch
                        {
                        }

                        textBox1.Text = "";
                        SalirDelSistemaDespuesDeRestore();
                    }
                    catch (Exception ex)
                    {
                        new BLL_Idioma_56PS().MostrarMensaje($"Error al restaurar la base de datos: {ex.Message}");
                    }
                }
                else
                {
                    new BLL_Idioma_56PS().MostrarMensaje("Seleccione un archivo de restore.");
                }
            }
        }

        private bool TienePermiso(string permiso)
        {
            var usuario = SessionManager_56PS.getInstancia().getUsuarioActivo();
            return usuario?.Rol != null && usuario.Rol.TienePermiso(permiso);
        }

        private bool TieneAlgunPermiso(params string[] permisos)
        {
            var usuario = SessionManager_56PS.getInstancia().getUsuarioActivo();
            return usuario?.Rol != null && usuario.Rol.TieneAlgunPermiso(permisos);
        }

        private void SalirDelSistemaDespuesDeRestore()
        {
            SessionManager_56PS.getInstancia().SolicitarCierreSistema();
        }
    }
}
