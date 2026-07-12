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

namespace SistemaBase.Usuario
{
    public partial class FormCambiarIdioma_56PS : Form, IdiomaObserver_56PS
    {
        public FormCambiarIdioma_56PS()
        {
            InitializeComponent(); SessionManager_56PS.getInstancia().Suscribir(this);
            button1.Enabled = TienePermiso(Permisos_56PS.CambiarIdioma);

        }

        public void actualizarIdioma()
        {
            var traductor = new BLL_Idioma_56PS();
            traductor.Traducir(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un idioma"); return;
            }


            try
            {

                if (string.IsNullOrEmpty(comboBox1.SelectedItem.ToString()))
                {
                    MessageBox.Show("Selecciona un idioma");
                    return;
                }
            }
            catch
            {

                MessageBox.Show("Selecciona un idioma");
                return;
            }

            if (comboBox1.SelectedItem.ToString() != "ES" && comboBox1.SelectedItem.ToString() != "EN" && comboBox1.SelectedItem.ToString() != "POR") { MessageBox.Show("El idioma solo puede ser EN,ES o POR"); return; }


            var user = SessionManager_56PS.getInstancia().getUsuarioActivo();

            //BLL_Usuario_56PS bll = new BLL_Usuario_56PS();

            //bll.cambiarIdioma(user, comboBox1.SelectedItem.ToString());

            //MessageBox.Show("Cambio de idioma con exito, se requiere reiniciar sistema");


            SessionManager_56PS.getInstancia().CambiarIdioma(new Idioma_56PS(comboBox1.SelectedItem.ToString()));


            string a = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;

            Evento_56PS eee = new Evento_56PS(a, DateTime.Now, "Usuarios", "Cambio de idioma", Evento_56PS.Criticidad.Bajo);
            new BLL_BitacoraEvento_56PS().RegistrarEvento(eee);


            return;
        }

        private void FormCambiarIdioma_56PS_Load(object sender, EventArgs e)
        {

        }

        private bool TienePermiso(string permiso)
        {
            var usuario = SessionManager_56PS.getInstancia().getUsuarioActivo();
            return usuario?.Perfil != null && usuario.Perfil.TienePermiso(permiso);
        }
    }
}
