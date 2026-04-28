using BE_625NS;
using BLL;
using ClassLibrary2;
using ClassLibrary3;
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
    public partial class FormIniciarSesion : Form
    {
        public FormIniciarSesion()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var instance = SessionManager_625NS.getInstancia();

            if (instance.haySesionActiva())
            {
                MessageBox.Show("Ya inicio sesion, cierre sesion primero");
                return;
            }



            string userName = (string)textBox1.Text.Trim();
            string password = (string)textBox2.Text.Trim();

            if (userName.Length == 0 || password.Length == 0)
            {
                MessageBox.Show("Complete todos los campos");
                return;
            }

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Complete todos los campos"); return;
            }


            BLL_Usuario_625NS usuarioBLL = new BLL_Usuario_625NS();
            bool valido = usuarioBLL.validarUsuario(userName, password);

            if (valido)
            {
                List<BE_Usuario_625NS> listaUsuarios = usuarioBLL.obtenerUsuarios();
                BE_Usuario_625NS usuarioLogueado = listaUsuarios.Find(u => u.NombreUsuario == userName);

                if (usuarioLogueado.Bloqueado)
                {
                    MessageBox.Show("El usuario se encuentra bloqueado"); return;
                }


                ////var perfilBLL = new BLLPerfil_625NS();
                //usuarioLogueado.Rol_625NS = perfilBLL.ObtenerPerfilCompleto625NS(usuarioLogueado.Rol_625NS.Nombre_625NS);




                //var revision = new BLL_DigitoVerificador_625NS().Revision();

                //bool tablavacia = revision.tablaDVVacia;
                //List<string> errores = revision.tablasConError;

                //if (tablavacia)
                //{
                //    MessageBox.Show("No existen registros en la tabla DigitoVerificador.");
                //}
                //else if (errores.Count > 0)
                //{
                //    MessageBox.Show("Se detectaron inconsistencias en la base de datos");

                //    if (usuarioLogueado.Rol_625NS.Nombre_625NS == "Administrador")
                //    {

                //        FormReparacion_625NS form = new FormReparacion_625NS(errores);
                //        form.Show();
                //        this.Hide();
                //        return;
                //    }
                //    else
                //    {
                //        MessageBox.Show("El sistema no se encuentra disponible en estos momentos, contacte al administrador.");
                //        return;
                //    }
                //}





                SessionManager_625NS.getInstancia().iniciarSesion(usuarioLogueado);

               // SessionManager_625NS.getInstancia().CambiarIdioma_625NS(usuarioLogueado.idioma);


                var sessao = SessionManager_625NS.getInstancia();
                string userNamee = sessao.getUsuarioActivo().NombreUsuario;


                MessageBox.Show("Sesión iniciada, bienvenido " + userNamee);


                var user = SessionManager_625NS.getInstancia().getUsuarioActivo();


                BE_Evento_625NS evento = new BE_Evento_625NS(user.Dni, DateTime.Now, "Usuarios", "Inicio de sesión", BE_Evento_625NS.Criticidad.Bajo);

                new BLL_BitacoraEvento_625NS().RegistrarEvento(evento);



                //var perfil = new BLLPerfil_625NS().ObtenerPerfilCompleto625NS(usuarioLogueado.Rol_625NS.Nombre_625NS);




                var userr = SessionManager_625NS.getInstancia().getUsuarioActivo();
                BLL_Usuario_625NS bll = new BLL_Usuario_625NS();


                MenuPrincipal menu = Application.OpenForms["FormPrincipal"] as MenuPrincipal;



                menu.MenuAdministracion.Enabled = userr.Rol_625NS == "Administrador";
                


                //codigo que se usara cuando exista composite
                //menu.MenuAdministracion.Enabled = bll.TienePermiso("Administrador", userr);
                //menu.MenuTurnos.Enabled = bll.TienePermiso("Turnos", userr);
                //menu.Reportes.Enabled = bll.TienePermiso("Reportes", userr);
                //menu.MenuConsulta.Enabled = bll.TienePermiso("ConsultaMedica", userr);
                //menu.MenuMaestro.Enabled = bll.TienePermiso("Maestro", userr);

                //menu.MenuCambiarIdioma.Enabled = true;

                //menu.MenuCambiarContraseña.Enabled = true;

                //menu.MenuCerrarSesion.Enabled = true;

                //menu.Ayuda.Enabled = true;

                this.Close();

                return;
            }
            else
            {




                MessageBox.Show("Usuario o contraseña incorrectos");




                usuarioBLL.sumarIntentoFallido(userName);
            }
        }
    }
}
