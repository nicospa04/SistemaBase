using BE_625NS;
using BLL;
using ClassLibrary2;
using ClassLibrary3;
using GUI_625NS.Administracion;
using SistemaBase.Administracion;
using SistemaBase.Usuario;
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

namespace SistemaBase
{
    public partial class MenuPrincipal_56PS : Form, IdiomaObserver_56PS
    {
        public MenuPrincipal_56PS()
        {
            InitializeComponent();
            SessionManager_56PS.getInstancia().Suscribir(this);
            actualizarIdioma();

            MenuAdministracion.Enabled = false;
            MenuAuditoria.Enabled = false;
            MenuFamilias.Enabled = false;
            MenuGestionRespaldo.Enabled = false;
            MenuPerfiles.Enabled = false;
            MenuCambiarContraseña.Enabled = false;
            MenuCambiarIdioma.Enabled = false;
        }

        public void actualizarIdioma()
        {
            var traductor = new BLL_Idioma_56PS();
            traductor.Traducir(this);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SessionManager_56PS.getInstancia().Desuscribir(this);
            base.OnFormClosed(e);
        }

        private void MenuPrincipal_56PS_Load(object sender, EventArgs e)
        {

        }

        private void AbrirFormulario(Form form)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f.GetType() == form.GetType())
                {
                    f.Activate();
                    return;
                }
            }

            form.MdiParent = this;
            form.WindowState = FormWindowState.Maximized;

            form.Show();
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormABMUsuario_56PS());

        }

        private void auditoriaDeEventosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormBitacoraEventos_56PS());

        }

        private void iniciarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormIniciarSesion_56PS());

        }

        public ToolStripMenuItem MenuAdministracion => usuariosToolStripMenuItem;
        public ToolStripMenuItem MenuCambiarContraseña => cambiarContraseñaToolStripMenuItem;

        public ToolStripMenuItem MenuPerfiles => perfilesToolStripMenuItem;

        public ToolStripMenuItem MenuFamilias => familiasToolStripMenuItem;

        public ToolStripMenuItem MenuGestionRespaldo => gestionDeRespaldoToolStripMenuItem;

        public ToolStripMenuItem MenuAuditoria => auditoriaDeEventosToolStripMenuItem;

        public ToolStripMenuItem MenuCambiarIdioma => cambiarIdiomaToolStripMenuItem;

        public ToolStripMenuItem menuAdmin => administracionToolStripMenuItem;
        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var instance = SessionManager_56PS.getInstancia();

            if (!instance.haySesionActiva())
            {
                MessageBox.Show("Debe iniciar sesión primero");
                return;
            }


            var idiomaActual = instance.idiomaActual;

            var idiomaOriginal = instance.getUsuarioActivo().idioma;

            if(idiomaActual != idiomaOriginal) //si se cierra sesion con un idioma distinto al original del usuario se lo cambiamos en la db
            {
                new BLL_Usuario_56PS().cambiarIdioma(idiomaActual, instance.getUsuarioActivo().Dni);

                Evento_56PS evento = new Evento_56PS(instance.getUsuarioActivo().Dni, DateTime.Now, "Usuarios", "Cambio de idioma", Evento_56PS.Criticidad.Bajo);
                new BLL_BitacoraEvento_56PS().RegistrarEvento(evento);

            }

            instance.cerrarSesion();
            MessageBox.Show("Sesión cerrada");

            foreach (Form form in this.MdiChildren)
            {
                form.Close();
            }

            MenuAdministracion.Enabled = false;
            MenuCambiarContraseña.Enabled = false;

            MenuCambiarIdioma.Enabled = false;
            MenuAuditoria.Enabled = false;
            MenuPerfiles.Enabled = false;
            MenuFamilias.Enabled = false;
            MenuGestionRespaldo.Enabled = false;


        }

        private void administracionToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void cambiarContraseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormCambiarContraseña_56PS());
        }

        private void administracionToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void perfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormPerfiles_56PS());
        }

        private void familiasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormFamilias_56PS());
        }

        private void cambiarIdiomaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormCambiarIdioma_56PS());
        }

        private void gestionDeRespaldoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormGestionBackUpRestore_56PS());
        }
    }
}
