using BE_625NS;
using BLL;
using ClassLibrary2;
using ClassLibrary3;
using Services_625NS;
using Servicio;
using System;
using System.Windows.Forms;

namespace SistemaBase.Usuario
{
    public partial class FormCambiarContraseña_56PS : Form, IdiomaObserver_56PS
    {
        public FormCambiarContraseña_56PS()
        {
            InitializeComponent();
            actualizarIdioma();
            button1.Enabled = TienePermiso(Permisos_56PS.CambiarContrasena);
        }

        public void actualizarIdioma()
        {
            new BLL_Idioma_56PS().Traducir(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string actual = textBox1.Text.Trim();
            string nueva = textBox2.Text.Trim();
            string confirmacion = textBox3.Text.Trim();
            BLL_Idioma_56PS mensajes = new BLL_Idioma_56PS();

            if (string.IsNullOrEmpty(actual) || string.IsNullOrEmpty(nueva) || string.IsNullOrEmpty(confirmacion))
            {
                mensajes.MostrarMensaje("Debe completar todos los campos.");
                return;
            }

            if (nueva != confirmacion)
            {
                mensajes.MostrarMensaje("La nueva contraseña no coincide con la confirmación.");
                return;
            }

            var usuario = SessionManager_56PS.getInstancia().getUsuarioActivo();
            if (usuario == null)
            {
                mensajes.MostrarMensaje("No hay sesión activa.");
                return;
            }

            if (usuario.Contraseña != CryptoManager_56PS.Encriptar(actual))
            {
                mensajes.MostrarMensaje("La contraseña actual es incorrecta.");
                return;
            }

            if (nueva == actual)
            {
                mensajes.MostrarMensaje("La contraseña nueva no puede ser igual a la actual");
                return;
            }

            string nuevaHash = CryptoManager_56PS.Encriptar(nueva);
            try
            {
                new BLL_Usuario_56PS().cambiarContraseña(usuario.Dni, nuevaHash);
                usuario.Contraseña = nuevaHash;
                mensajes.MostrarMensaje("La contraseña se cambió correctamente.");

                new BLL_BitacoraEvento_56PS().RegistrarEvento(new Evento_56PS(
                    usuario.Dni,
                    DateTime.Now,
                    "Usuarios",
                    "Cambio de contraseña",
                    Evento_56PS.Criticidad.Alto));

                Close();
            }
            catch (Exception ex)
            {
                mensajes.MostrarMensaje("Error: " + ex.Message);
            }
        }

        private void FormCambiarContraseña_56PS_Load(object sender, EventArgs e)
        {
        }

        private bool TienePermiso(string permiso)
        {
            var usuario = SessionManager_56PS.getInstancia().getUsuarioActivo();
            return usuario?.Rol != null && usuario.Rol.TienePermiso(permiso);
        }
    }
}
