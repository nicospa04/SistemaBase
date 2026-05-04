using ClassLibrary2;
using GUI_625NS.Administracion;
using SistemaBase.Administracion;
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
    public partial class MenuPrincipal_56PS : Form
    {
        public MenuPrincipal_56PS()
        {
            InitializeComponent();

            MenuAdministracion.Enabled = false;
            MenuCambiarContraseña.Enabled = false;
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
            AbrirFormulario(new FormABMUsuario_625NS());

        }

        private void auditoriaDeEventosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormBitacoraEventos_625NS());

        }

        private void iniciarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormIniciarSesion());

        }

        public ToolStripMenuItem MenuAdministracion => administracionToolStripMenuItem1;
        public ToolStripMenuItem MenuCambiarContraseña => cambiarContraseñaToolStripMenuItem;



        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var instance = SessionManager_56PS.getInstancia();

            if (!instance.haySesionActiva())
            {
                MessageBox.Show("Debe iniciar sesión primero");
                return;
            }

            instance.cerrarSesion();
            MessageBox.Show("Sesión cerrada");

            MenuAdministracion.Enabled = false;
            MenuCambiarContraseña.Enabled = false;
        }

        private void administracionToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void cambiarContraseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void administracionToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
