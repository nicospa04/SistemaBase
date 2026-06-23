using BE_56_PS;
using BE_625NS;
using BLL;
using ClassLibrary2;
using ClassLibrary3;
using Servicio;
using SistemaBase.Usuario;
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
    public partial class FormIniciarSesion_56PS : Form, IdiomaObserver_56PS
    {

        public void actualizarIdioma()
        {
            var traductor = new BLL_Idioma_56PS();
            traductor.Traducir(this);
        }

        public FormIniciarSesion_56PS()
        {
            InitializeComponent();

            actualizarIdioma();
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

            List<Usuario_56PS> listaUsuarios = new BLL_Usuario_56PS().obtenerUsuarios();
            Usuario_56PS usuarioLogueado = listaUsuarios.Find(u => u.NombreUsuario == userName);

            //si existe usuario con ese nombre... validamos si su contraseña coincide

            bool valido = usuarioBLL.validarUsuario(userName, password); //aca se valida si el usuario y la contraseña coinciden con algun usuario de la bd

            if (!valido)
            {
                MessageBox.Show("Contraseña incorrecta");

                //registramos evento en bitacora
                Evento_56PS eventoFallido = new Evento_56PS(
                    usuarioLogueado.Dni,
                    DateTime.Now,
                    "Usuarios",
                    "Intento fallido",
                    Evento_56PS.Criticidad.Alto
                );

                BLL_BitacoraEvento_56PS bitacoraBLL = new BLL_BitacoraEvento_56PS();
                bitacoraBLL.RegistrarEvento(eventoFallido);

                //obtenemos eventos de los ultimos 5 minutos
                List<Evento_56PS> eventos = bitacoraBLL.obtenerEventos();

                int cantidadIntentos = eventos.Count(ev =>
                    ev.dni == usuarioLogueado.Dni &&
                    ev.descripcion== "Intento fallido" &&
                    ev.fecha >= DateTime.Now.AddMinutes(-5)
                );

                //si tiene 3 o mas intentos fallidos lo bloqueamos
                if (cantidadIntentos >= 3)
                {
                    usuarioBLL.bloquearUsuario(usuarioLogueado.Dni);
                    MessageBox.Show("Usuario bloqueado por demasiados intentos fallidos");
                }

                return;
            }


            //en caso de que el userName y la contraseña coincidan... verificamos que el usuario no se encuentre bloqueado

 

                if (usuarioLogueado.Bloqueado)
                {
                    MessageBox.Show("El usuario se encuentra bloqueado"); return;
                }

            if (!usuarioLogueado.Activo)
            {
                MessageBox.Show("El usuario se encuentra inactivo"); return;
            }




            SessionManager_56PS.getInstancia().iniciarSesion(usuarioLogueado);
            SessionManager_56PS.getInstancia().CambiarIdioma(usuarioLogueado.idioma);



            var sessao = SessionManager_56PS.getInstancia();
                string userNamee = sessao.getUsuarioActivo().NombreUsuario;


                MessageBox.Show("Sesión iniciada, bienvenido " + userNamee);



            var user = SessionManager_56PS.getInstancia().getUsuarioActivo();


                Evento_56PS evento = new Evento_56PS(user.Dni, DateTime.Now, "Usuarios", "Inicio de sesión", Evento_56PS.Criticidad.Bajo);

                new BLL_BitacoraEvento_56PS().RegistrarEvento(evento);



                var userr = SessionManager_56PS.getInstancia().getUsuarioActivo();
                BLL_Usuario_56PS bll = new BLL_Usuario_56PS();


                MenuPrincipal_56PS menu = Application.OpenForms["MenuPrincipal_56PS"] as MenuPrincipal_56PS;



        
            menu.MenuAdministracion.Enabled = userr.Perfil != null &&
                userr.Perfil.Contiene("P01"); // P01 = Gestión de Usuarios
            menu.MenuCambiarContraseña.Enabled = true;

            menu.MenuFamilias.Enabled = userr.Perfil.Contiene("P04");

            menu.MenuPerfiles.Enabled = userr.Perfil.Contiene("P03");
            // Verificar si el usuario debe cambiar contraseña usando la bitácora
            if (DebeCambiarContraseña(userr.Dni))
            {
                MessageBox.Show("Debe cambiar su contraseña antes de continuar.");
                FormCambiarContraseña_56PS formCambio = new FormCambiarContraseña_56PS();
                formCambio.ShowDialog();
            }

                this.Close();

                return;
            
        }

        /// <summary>
        /// Verifica en la bitácora si el usuario debe cambiar su contraseña.
        /// Un usuario debe cambiar contraseña si:
        /// 1. Fue creado y nunca cambió su contraseña (no existe evento "Cambio de contraseña" para su DNI)
        /// 2. Fue desbloqueado y no cambió su contraseña después del desbloqueo
        /// </summary>
        private bool DebeCambiarContraseña(string dniUsuario)
        {
            BLL_BitacoraEvento_56PS bitacoraBLL = new BLL_BitacoraEvento_56PS();
            List<Evento_56PS> eventos = bitacoraBLL.obtenerEventos();

            // Buscar el último evento de "Cambio de contraseña" de ESTE usuario
            var ultimoCambio = eventos
                .Where(ev => ev.dni == dniUsuario && ev.descripcion == "Cambio de contraseña")
                .OrderByDescending(ev => ev.fecha)
                .FirstOrDefault();

            // Si nunca cambió la contraseña → debe cambiarla (usuario nuevo)
            if (ultimoCambio == null)
                return true;

            // Buscar si hay un evento de desbloqueo posterior al último cambio de contraseña
            // El evento de desbloqueo lo registra el ADMIN, y la descripción contiene el DNI del usuario desbloqueado
            var desbloqueoPostCambio = eventos
                .Where(ev =>
                    ev.descripcion.Contains("Desbloqueo de usuario") &&
                    ev.descripcion.Contains(dniUsuario) &&
                    ev.fecha > ultimoCambio.fecha)
                .Any();

            return desbloqueoPostCambio;
        }

        private void FormIniciarSesion_Load(object sender, EventArgs e)
        {

        }
    }
}
