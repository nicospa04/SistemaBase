using BE_625NS;
using BLL;
using ClassLibrary2;
using ClassLibrary3;
using Services_625NS;
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
            var traductor = new BLL_Idioma_56PS();
            traductor.Traducir(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string actual = textBox1.Text.Trim();
            string nueva = textBox2.Text.Trim();
            string confirmacion = textBox3.Text.Trim();

            if (string.IsNullOrEmpty(actual) || string.IsNullOrEmpty(nueva) || string.IsNullOrEmpty(confirmacion))
            {
                MessageBox.Show("Debe completar todos los campos.");
                return;
            }

            if (nueva != confirmacion)
            {
                MessageBox.Show("La nueva contraseña no coincide con la confirmación.");
                return;
            }

            var usuario = SessionManager_56PS.getInstancia().getUsuarioActivo();

            if (usuario == null)
            {
                MessageBox.Show("No hay sesión activa.");
                return;
            }


            string hashActualIngresada = CryptoManager_56PS.Encriptar(actual);

            if (usuario.Contraseña != hashActualIngresada)
            {
                MessageBox.Show("La contraseña actual es incorrecta.");
                return;
            }

            if (nueva == actual)
            {
                MessageBox.Show("La contraseña nueva no puede ser igual a la actual"); return;
            }



            string nuevaHash = CryptoManager_56PS.Encriptar(nueva);

            try
            {
                BLL_Usuario_56PS bll = new BLL_Usuario_56PS();
                bll.cambiarContraseña(usuario.Dni, nuevaHash);

                // Actualizar en memoria también
                usuario.Contraseña = nuevaHash;

                MessageBox.Show("La contraseña se cambió correctamente.");

                // Registrar evento en bitácora
                Evento_56PS evento = new Evento_56PS(
                    usuario.Dni,
                    DateTime.Now,
                    "Usuarios",
                    "Cambio de contraseña",
                    Evento_56PS.Criticidad.Alto
                );
                new BLL_BitacoraEvento_56PS().RegistrarEvento(evento);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void FormCambiarContraseña_56PS_Load(object sender, EventArgs e)
        {

        }

        private bool TienePermiso(string permiso)
        {
            var usuario = SessionManager_56PS.getInstancia().getUsuarioActivo();
            return usuario?.Perfil != null && usuario.Perfil.TienePermiso(permiso);
        }
    }
}
