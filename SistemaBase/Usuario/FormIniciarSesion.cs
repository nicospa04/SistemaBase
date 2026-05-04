using BE_56_PS;
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
            var instance = SessionManager_56PS.getInstancia();

            if (instance.haySesionActiva()) //validamos que no exista una sesion ya iniciada
            {
                MessageBox.Show("Ya inicio sesion, cierre sesion primero");
                return;
            }



            string userName = (string)textBox1.Text.Trim();
            string password = (string)textBox2.Text.Trim();

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Complete todos los campos"); return;
            }


            BLL_Usuario_56PS usuarioBLL = new BLL_Usuario_56PS();


            //antes de validar si la contraseña y el usuario coinciden primero validamos si existen algun usuario con ese nombre de usuario

            bool existeUsuarioConEseUsername = usuarioBLL.existeUsuarioConEseUsername(userName);

            if (!existeUsuarioConEseUsername) //si el userName directamente no existe
            {
                MessageBox.Show("No existe usuario con ese nombre ");
                return;
            }

            //si existe usuario con ese nombre... validamos si su contraseña coincide

            bool valido = usuarioBLL.validarUsuario(userName, password); //aca se valida si el usuario y la contraseña coinciden con algun usuario de la bd

            if (!valido) //si la contraseña NO coincide con el usuario ingresado sumamos un intento en el loginattemptmanager
            {
                MessageBox.Show("Contraseña incorrecta"); //notificamos que la contraseña es incorrecta

                var attemptManager = LoginAttemptManager_56PS.GetInstancia();

                bool bloqueado = attemptManager.RegistrarIntentoFallido(userName); //sumamos intento fallido

                if (bloqueado) //si los intentos excedieron lo definido (cant/tiempo) entonces bloqueamos el usuario
                {
                    Console.WriteLine("Su cuenta ha sido bloqueada por demasiados intentos fallidos");
                    usuarioBLL.bloquearUsuario(userName);
                }
                return; //terminamos ejecucion
            }


            //en caso de que el userName y la contraseña coincidan... verificamos que el usuario no se encuentre bloqueado

            List<Usuario_56PS> listaUsuarios = usuarioBLL.obtenerUsuarios();
            Usuario_56PS usuarioLogueado = listaUsuarios.Find(u => u.NombreUsuario == userName);

                if (usuarioLogueado.Bloqueado)
                {
                    MessageBox.Show("El usuario se encuentra bloqueado"); return;
                }



                SessionManager_56PS.getInstancia().iniciarSesion(usuarioLogueado);


                var sessao = SessionManager_56PS.getInstancia();
                string userNamee = sessao.getUsuarioActivo().NombreUsuario;


                MessageBox.Show("Sesión iniciada, bienvenido " + userNamee);

            LoginAttemptManager_56PS.GetInstancia().ResetearIntentos(userName); //reseteamos intentos para el usuario que inicio sesion


            var user = SessionManager_56PS.getInstancia().getUsuarioActivo();


                BE_Evento_56PS evento = new BE_Evento_56PS(user.Dni, DateTime.Now, "Usuarios", "Inicio de sesión", BE_Evento_56PS.Criticidad.Bajo);

                new BLL_BitacoraEvento_56PS().RegistrarEvento(evento);



                var userr = SessionManager_56PS.getInstancia().getUsuarioActivo();
                BLL_Usuario_56PS bll = new BLL_Usuario_56PS();


                MenuPrincipal_56PS menu = Application.OpenForms["MenuPrincipal_56PS"] as MenuPrincipal_56PS;



                menu.MenuAdministracion.Enabled = userr.Rol == "Administrador";
            menu.MenuCambiarContraseña.Enabled = true;
                

                this.Close();

                return;
            
        }

        private void FormIniciarSesion_Load(object sender, EventArgs e)
        {

        }
    }
}
