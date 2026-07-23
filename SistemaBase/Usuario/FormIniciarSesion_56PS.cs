using BE_56_PS;
using BE_625NS;
using BLL;
using ClassLibrary2;
using ClassLibrary3;
using Servicio;
using SistemaBase.Administracion;
using SistemaBase.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SistemaBase
{
    public partial class FormIniciarSesion_56PS : Form, IdiomaObserver_56PS
    {
        public FormIniciarSesion_56PS()
        {
            InitializeComponent();
            SessionManager_56PS.getInstancia().Suscribir(this);
            actualizarIdioma();
        }

        public void actualizarIdioma()
        {
            new BLL_Idioma_56PS().Traducir(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SessionManager_56PS sesion = SessionManager_56PS.getInstancia();
            BLL_Idioma_56PS mensajes = new BLL_Idioma_56PS();

            if (sesion.haySesionActiva())
            {
                mensajes.MostrarMensaje("Ya inicio sesion, cierre sesion primero");
                return;
            }

            string nombreUsuario = textBox1.Text.Trim();
            string contraseña = textBox2.Text.Trim();
            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contraseña))
            {
                mensajes.MostrarMensaje("Complete todos los campos");
                return;
            }

            BLL_Usuario_56PS usuarios = new BLL_Usuario_56PS();
            if (!usuarios.existeUsuarioConEseUsername(nombreUsuario))
            {
                mensajes.MostrarMensaje("No existe usuario con ese nombre ");
                return;
            }

            Usuario_56PS usuario = usuarios.obtenerUsuarios()
                .FirstOrDefault(item => item.NombreUsuario == nombreUsuario);
            if (usuario == null)
            {
                mensajes.MostrarMensaje("Usuario no existe");
                return;
            }

            if (!usuarios.validarUsuario(nombreUsuario, contraseña))
            {
                RegistrarIntentoFallido(usuario, usuarios, mensajes);
                return;
            }

            if (usuario.Bloqueado)
            {
                mensajes.MostrarMensaje("El usuario se encuentra bloqueado");
                return;
            }

            if (!usuario.Activo)
            {
                mensajes.MostrarMensaje("El usuario se encuentra inactivo");
                return;
            }

            var revision = new BLL_DigitoVerificador_56PS().Revision();
            bool hayInconsistencias = revision.tablaDVVacia || revision.tablasConError.Count > 0;
            bool puedeReparar = TieneAlgunPermiso(
                usuario.Rol,
                Permisos_56PS.RecalcularDigitosVerificadores,
                Permisos_56PS.RestaurarBackup);

            if (hayInconsistencias && !puedeReparar)
            {
                mensajes.MostrarMensaje("El sistema no se encuentra disponible en estos momentos, contacte al administrador.");
                return;
            }

            MenuPrincipal_56PS menu = Application.OpenForms["MenuPrincipal_56PS"] as MenuPrincipal_56PS;
            sesion.iniciarSesion(usuario);
            sesion.CambiarIdioma(usuario.Idioma ?? new Idioma_56PS("ES"));

            if (hayInconsistencias)
            {
                if (revision.tablaDVVacia && revision.tablasConError.Count == 0)
                    revision.tablasConError.Add("Tabla DigitoVerificador sin registros");

                mensajes.MostrarMensaje("Se detectaron inconsistencias en la base de datos");
                if (menu != null)
                    menu.AplicarEstadoSesion(usuario.Rol);

                FormReparacion_56PS reparacion = new FormReparacion_56PS(revision.tablasConError);
                if (menu != null)
                {
                    reparacion.MdiParent = menu;
                    reparacion.WindowState = FormWindowState.Maximized;
                }

                reparacion.Show();
                Close();
                return;
            }

            mensajes.MostrarMensaje("Sesión iniciada, bienvenido " + usuario.NombreUsuario);
            new BLL_BitacoraEvento_56PS().RegistrarEvento(new Evento_56PS(
                usuario.Dni,
                DateTime.Now,
                "Usuarios",
                "Inicio de sesión",
                Evento_56PS.Criticidad.Bajo));

            if (menu != null)
                menu.AplicarEstadoSesion(usuario.Rol);

            if (DebeCambiarContraseña(usuario.Dni))
            {
                mensajes.MostrarMensaje("Debe cambiar su contraseña antes de continuar.");
                new FormCambiarContraseña_56PS().ShowDialog();
            }

            Close();
        }

        private void RegistrarIntentoFallido(Usuario_56PS usuario, BLL_Usuario_56PS usuarios, BLL_Idioma_56PS mensajes)
        {
            mensajes.MostrarMensaje("Contraseña incorrecta");
            BLL_BitacoraEvento_56PS bitacora = new BLL_BitacoraEvento_56PS();
            bitacora.RegistrarEvento(new Evento_56PS(
                usuario.Dni,
                DateTime.Now,
                "Usuarios",
                "Intento fallido",
                Evento_56PS.Criticidad.Alto));

            int intentos = bitacora.obtenerEventos().Count(evento =>
                evento.dni == usuario.Dni &&
                evento.descripcion == "Intento fallido" &&
                evento.fecha >= DateTime.Now.AddMinutes(-5));

            if (intentos >= 3)
            {
                usuarios.bloquearUsuario(usuario.Dni);
                mensajes.MostrarMensaje("Usuario bloqueado por demasiados intentos fallidos");
            }
        }

        private bool DebeCambiarContraseña(string dniUsuario)
        {
            List<Evento_56PS> eventos = new BLL_BitacoraEvento_56PS().obtenerEventos();
            Evento_56PS ultimoCambio = eventos
                .Where(evento => evento.dni == dniUsuario && evento.descripcion == "Cambio de contraseña")
                .OrderByDescending(evento => evento.fecha)
                .FirstOrDefault();

            if (ultimoCambio == null)
                return true;

            return eventos.Any(evento =>
                evento.descripcion.Contains("Desbloqueo de usuario") &&
                evento.dni.Contains(dniUsuario) &&
                evento.fecha > ultimoCambio.fecha);
        }

        private bool TieneAlgunPermiso(Rol_56PS rol, params string[] permisos)
        {
            return rol != null && rol.TieneAlgunPermiso(permisos);
        }

        private void FormIniciarSesion_Load(object sender, EventArgs e)
        {
        }
    }
}
