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
            InitializeComponent();
            SessionManager_56PS.getInstancia().Suscribir(this);
            CargarIdiomas();
            button1.Enabled = TienePermiso(Permisos_56PS.CambiarIdioma);
        }

        public void actualizarIdioma()
        {
            var traductor = new BLL_Idioma_56PS();
            traductor.Traducir(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Idioma_56PS idiomaSeleccionado = comboBox1.SelectedItem as Idioma_56PS;
            if (idiomaSeleccionado == null)
            {
                new BLL_Idioma_56PS().MostrarMensaje("Selecciona un idioma");
                return;
            }

            SessionManager_56PS.getInstancia().CambiarIdioma(idiomaSeleccionado);
            string a = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
            Evento_56PS eee = new Evento_56PS(a, DateTime.Now, "Usuarios", "Cambio de idioma", Evento_56PS.Criticidad.Bajo);
            new BLL_BitacoraEvento_56PS().RegistrarEvento(eee);
        }

        private void CargarIdiomas()
        {
            List<Idioma_56PS> idiomas = new BLL_Idioma_56PS().ObtenerIdiomas();
            comboBox1.DataSource = idiomas;
            comboBox1.DisplayMember = "nombre";
            comboBox1.ValueMember = "tipo";

            string tipoActual = SessionManager_56PS.getInstancia().idiomaActual?.tipo;
            int indice = idiomas.FindIndex(idioma => idioma.tipo == tipoActual);
            comboBox1.SelectedIndex = indice >= 0 ? indice : 0;
        }

        private void FormCambiarIdioma_56PS_Load(object sender, EventArgs e)
        {

        }

        private bool TienePermiso(string permiso)
        {
            var usuario = SessionManager_56PS.getInstancia().getUsuarioActivo();
            return usuario?.Rol != null && usuario.Rol.TienePermiso(permiso);
        }
    }
}
