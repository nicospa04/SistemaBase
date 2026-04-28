using BE_625NS;
using BLL;
using ClassLibrary2;
using ClassLibrary3;
using Services_625NS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaBase.Administracion
{
    public partial class FormABMUsuario_625NS : Form
    {
        string modo = "";

        public FormABMUsuario_625NS()
        {
            InitializeComponent();

            CargarRoles();






            //ActualizarIdioma_625NS();

            actualizar();
        }

        void CargarRoles()
        {

            //hardcodeamos los roles ya que aun no existen en la bd
            comboBox1.Items.Add("Administrador");
            comboBox1.Items.Add("Base");
            comboBox1.SelectedIndex = 1; //x defecto se selecciona el rol base
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                    string.IsNullOrWhiteSpace(textBox2.Text) ||
                    string.IsNullOrWhiteSpace(textBox3.Text) ||
                    string.IsNullOrWhiteSpace(textBox4.Text))
                {
                    MessageBox.Show("Todos los campos son obligatorios", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (comboBox1.SelectedItem == null)
                {
                    MessageBox.Show("Debe seleccionar un rol", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBox4.Text))
                {
                    MessageBox.Show("Debe ingresar un email");
                    return;
                }


                string dni = textBox1.Text;

                if (new BLL_Usuario_625NS().obtenerUsuarios().Any(ee => ee.Dni == dni))
                {
                    MessageBox.Show("No se puede crear un usuario con ese DNI.");
                    return;
                }


                // Crear nombre de usuario, es la combinacion de los primeros dos digitos de nombre + apellido + dni ultimos 2 digitos
                string nombreUsuario = textBox3.Text.Substring(0, 2) +
                                       textBox2.Text.Substring(0, 2) +
                                       textBox1.Text.Substring(textBox1.Text.Length - 2);

                string emailEncriptado = CryptoManager_625NS.EncriptarReversible(textBox4.Text);

                //la contraseña se crea apartir de juntar el DNI + apellido
                if (emailEncriptado == null)
                {
                    MessageBox.Show("Error al encriptar el email. Revise el valor ingresado.");
                    return;
                }


                // Crear objeto Usuario
                var usuario = new BE_Usuario_625NS(
                    apellido: textBox2.Text,
                    bloqueado: checkBox2.Checked,
                    contraseña: CryptoManager_625NS.Encriptar(textBox1.Text + textBox2.Text),
                    dni: textBox1.Text,
                    email: emailEncriptado,
                    nombre: textBox3.Text,
                    nombreUsuario: nombreUsuario,
                    cantIntentosFallidos: 0,
                    idioma: "ES" //Español es el idioma por defecto
                );

                usuario.Rol_625NS = comboBox1.SelectedValue.ToString();

                // Guardar usuario
                var usuarioBLL = new BLL_Usuario_625NS();
                usuarioBLL.crearUsuario(usuario);

               

                MessageBox.Show("Usuario creado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);


                string aa = SessionManager_625NS.getInstancia().getUsuarioActivo().Dni;

                BE_Evento_625NS eee = new BE_Evento_625NS(aa, DateTime.Now, "Usuarios", "Creacion de usuario", BE_Evento_625NS.Criticidad.Bajo);
                new BLL_BitacoraEvento_625NS().RegistrarEvento(eee);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear el usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            actualizar();
            LimpiarCampos();
            modo = "";

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Debe seleccionar un usuario de la lista.");
                return;
            }



            DataGridViewRow fila = dataGridView1.SelectedRows[0];






            // Cargar datos al formulario
            textBox1.Text = fila.Cells["DNI"].Value.ToString();
            textBox3.Text = fila.Cells["Nombre"].Value.ToString();
            textBox2.Text = fila.Cells["Apellido"].Value.ToString();
            textBox4.Text = fila.Cells["Email"].Value.ToString();
            checkBox1.Checked = true;
            checkBox2.Checked = Convert.ToBoolean(fila.Cells["Bloqueado"].Value);

            if (checkBox2.Checked)
                checkBox1.Checked = false;

            textBox1.Enabled = false;

            HabilitarCampos(true);

            modo = "modificar";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (modo == "modificar")
            {

                string dni = textBox1.Text;
                string apellido = textBox2.Text;
                string nombre = textBox3.Text;
                string email = textBox4.Text;
                bool activo = checkBox1.Checked;
                bool bloqueado = checkBox2.Checked;

                DataGridViewRow fila = dataGridView1.SelectedRows[0];

                textBox1.Text = fila.Cells["DNI"].Value.ToString();
                textBox3.Text = fila.Cells["Nombre"].Value.ToString();
                textBox2.Text = fila.Cells["Apellido"].Value.ToString();
                textBox4.Text = fila.Cells["Email"].Value.ToString();
                checkBox1.Checked = true;
                checkBox2.Checked = Convert.ToBoolean(fila.Cells["Bloqueado"].Value);



                string emailEncriptado = CryptoManager_625NS.EncriptarReversible(email);



                if (nombre.Length < 2 || apellido.Length < 2 || dni.Length < 2)
                {
                    MessageBox.Show("Nombre, Apellido y DNI deben tener al menos 2 caracteres.");
                    return;
                }
                string nombreUsuario = fila.Cells["NombreUsuario"].Value.ToString();

                if (activo && bloqueado)
                {

                    MessageBox.Show("Solo puede tener un estado");
                    return;
                }

                if (!activo && !bloqueado)
                {

                    MessageBox.Show("Solo puede tener un estado");
                    return;
                }




                BE_625NS.BE_Usuario_625NS user = new BE_625NS.BE_Usuario_625NS(apellido, bloqueado, fila.Cells["Contraseña"].Value.ToString(), dni, emailEncriptado, nombre, nombreUsuario, 0, "EN");


                if (comboBox1.SelectedValue.ToString() == "Profesional")
                {
                    MessageBox.Show("No se puede modificar un usuario y asignarle el rol de profesional"); return;
                }

                user.Rol_625NS = comboBox1.SelectedValue.ToString();



                BLL.BLL_Usuario_625NS a = new BLL.BLL_Usuario_625NS();
                a.modificarUsuario(user);

                actualizar();
                LimpiarCampos();

                modo = "";

                MessageBox.Show("Usuario modificado");


                var currentUser = SessionManager_625NS.getInstancia().getUsuarioActivo();

                BE_Evento_625NS evento = new BE_Evento_625NS(currentUser.Dni, DateTime.Now, "Usuarios", "Modificación de usuario", BE_Evento_625NS.Criticidad.Medio);


            }
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }


        private void LimpiarCampos()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            textBox1.Enabled = true;
        }


        void actualizar()
        {
            BLL_Usuario_625NS bl = new BLL_Usuario_625NS
             ();

          
            label2.Text += bl.obtenerUsuarios().Count();
        }

        //private void CargarRoles()
        ////{
        ////    var rolBLL = new BLLPerfil_625NS();
        //    var roles = rolBLL.ObtenerPerfilesSimples625NS();
        //    comboBox1.DataSource = roles;
        //    comboBox1.DisplayMember = "Nombre_625NS";
        //    comboBox1.ValueMember = "Nombre_625NS";
        //}



        private void HabilitarCampos(bool habilitar)
        {
            textBox1.Enabled = habilitar;
            textBox2.Enabled = habilitar;
            textBox3.Enabled = habilitar;
            textBox4.Enabled = habilitar;

            checkBox1.Enabled = habilitar;
            checkBox2.Enabled = habilitar;

            button5.Enabled = habilitar;
        }
        bool activado = false;
        private void button2_Click(object sender, EventArgs e)
        {
            activado = !activado;

            var bll = new BLL_Usuario_625NS();
            var usuarios = bll.obtenerUsuarios();

            if (activado)
            {
                // 🔓 Mostrar mails desencriptados
                foreach (var u in usuarios)
                {
                    if (!string.IsNullOrEmpty(u.Email))
                        u.Email = CryptoManager_625NS.DesencriptarReversible(u.Email);
                }

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = usuarios;
                button2.Text = "Ocultar mails";
            }
            else
            {
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = usuarios;
                button2.Text = "Ver mails";
            }

        }

        private void FormABMUsuario_625NS_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
