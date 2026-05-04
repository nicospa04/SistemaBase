using BE_56_PS;
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
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaBase.Administracion
{
    public partial class FormABMUsuario_625NS : Form
    {
        string modo = "";

        List<Usuario_56PS> listaGeneral;

        public FormABMUsuario_625NS()
        {
            InitializeComponent();

            CargarRoles();

            listaGeneral = new BLL_Usuario_56PS().obtenerUsuarios();

            dataGridView1.DataSource = listaGeneral;


            deshabilitarBotonCancelar();
            deshabilitarBotonAplicar();


            //ActualizarIdioma_625NS();

            actualizar();

            textBox5.Text = textBox5.Text = "Mensaje: \nModo consulta";
        }

        void deshabilitarBotonAplicar()
        {
            button5.Enabled = false;
        }


        void habilitarBotonAplicar()
        {
            button5.Enabled = true;
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

            modo = "crear";

            textBox5.Text = "Mensaje: \nModo crear";

            habilitarBotonAplicar();
            habilitarBotonCancelar();

            deshabilitarBotonActivarDesactivar();
            deshabilitarBotonDesbloquear();
            deshabilitarBotonModificar();


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

            textBox5.Text = "Mensaje: \nModo modificar";

            habilitarBotonAplicar();
            habilitarBotonCancelar();

        }

        private void button5_Click(object sender, EventArgs e)
        {

            if(modo == "desbloquear")
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {


                    MessageBox.Show("Debe seleccionar un usuario de la tabla");
                    return;
                }

                var fila = dataGridView1.SelectedRows[0];

                string dni = fila.Cells["DNI"].Value.ToString();
                bool bloqueadoActual = Convert.ToBoolean(fila.Cells["Bloqueado"].Value);

                var bll = new BLL_Usuario_56PS();
                // Invertimos: si está bloqueado, lo desbloqueamos (false) y viceversa
                    bll.CambiarEstadoBloqueado(dni, !bloqueadoActual);

                MessageBox.Show("Estado actualizado con éxito.");
                actualizar(); // Esto refresca la lista
            }


            if (modo == "activar/desactivar")
            {
                if (dataGridView1.SelectedRows.Count == 0) {


                    MessageBox.Show("Debe seleccionar un usuario de la tabla");
                    return;
                }

                var fila = dataGridView1.SelectedRows[0];

                string dni = fila.Cells["DNI_625NS"].Value.ToString();
                bool ActivoActual = Convert.ToBoolean(fila.Cells["Activo"].Value);

                  var bll = new BLL_Usuario_56PS();
                // Invertimos: si está bloqueado, lo desbloqueamos (false) y viceversa


                if (ActivoActual) 
                {
                    bll.bloquearUsuario(dni); //hacemos lo opuesto a lo que ya existe

                }
                else
                {
                    bll.desbloquearUsuario(dni);
                }
                    MessageBox.Show("Estado actualizado con éxito.");
                    actualizar(); // Esto refresca la lista
             
            }



            if (modo == "desbloquear")
            {
                try
                {

                    DataGridViewRow fila = dataGridView1.SelectedRows[0];


                    string dni = fila.Cells["DNI"].Value.ToString();

                    Usuario_56PS usuarioADesbloquear = (Usuario_56PS)new BLL_Usuario_56PS().obtenerUsuarioPorDni(dni); // to do

                    if (!usuarioADesbloquear.Bloqueado)
                    {
                        MessageBox.Show("No puede desbloquear un usuario que no se encuentra bloqueado");
                        return;
                    }

                    new BLL_Usuario_56PS().desbloquearUsuario(dni);

                    MessageBox.Show($"Usuario {usuarioADesbloquear.NombreUsuario} desbloqueado con exito");


                }
                catch
                {
                    MessageBox.Show("Debe seleccionar un usuario para poder desbloquearlo");
                }


            }

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



                string emailEncriptado = CryptoManager_56PS.EncriptarReversible(email);



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




                Usuario_56PS user = new Usuario_56PS( //ARREGLAR ESTOOOOOOOO !!!!!
                    apellido,
                    bloqueado,
                    fila.Cells["Contraseña"].Value.ToString(),
                    dni,
                    emailEncriptado,
                    nombre,
                    nombreUsuario,
                    activo:true,
                    idioma:"EN",
                    rol: comboBox1.SelectedItem.ToString());


              

                user.Rol = comboBox1.SelectedValue.ToString();



                BLL.BLL_Usuario_56PS a = new BLL.BLL_Usuario_56PS();
                a.modificarUsuario(user);

                actualizar();
                LimpiarCampos();

                modo = "";

                MessageBox.Show("Usuario modificado");


                var currentUser = SessionManager_56PS.getInstancia().getUsuarioActivo();

                BE_Evento_56PS evento = new BE_Evento_56PS(currentUser.Dni, DateTime.Now, "Usuarios", "Modificación de usuario", BE_Evento_56PS.Criticidad.Medio);


            }
            if(modo == "crear")
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

                    if (new BLL_Usuario_56PS().obtenerUsuarios().Any(ee => ee.Dni == dni))
                    {
                        MessageBox.Show("No se puede crear un usuario con ese DNI.");
                        return;
                    }


                    // Crear nombre de usuario, es la combinacion de los primeros dos digitos de nombre + apellido + dni ultimos 2 digitos
                    string nombreUsuario = textBox3.Text.Substring(0, 2) +
                                           textBox2.Text.Substring(0, 2) +
                                           textBox1.Text.Substring(textBox1.Text.Length - 2);

                    string emailEncriptado = CryptoManager_56PS.EncriptarReversible(textBox4.Text);

                    //la contraseña se crea apartir de juntar el DNI + apellido
                    if (emailEncriptado == null)
                    {
                        MessageBox.Show("Error al encriptar el email. Revise el valor ingresado.");
                        return;
                    }


                    // Crear objeto Usuario
                    var usuario = new Usuario_56PS(
                        apellido: textBox2.Text,
                        bloqueado: checkBox2.Checked,
                        contraseña: CryptoManager_56PS.Encriptar(textBox1.Text + textBox2.Text),
                        dni: textBox1.Text,
                        email: emailEncriptado,
                        nombre: textBox3.Text,
                        nombreUsuario: nombreUsuario,
                        idioma: "ES", //Español es el idioma por defecto,
                        activo: true,
                        rol: comboBox1.SelectedItem.ToString()
                    );

                    usuario.Rol = comboBox1.SelectedValue.ToString();

                    // Guardar usuario
                    var usuarioBLL = new BLL_Usuario_56PS();
                    usuarioBLL.crearUsuario(usuario);



                    MessageBox.Show("Usuario creado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    string aa = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;

                    BE_Evento_56PS eee = new BE_Evento_56PS(aa, DateTime.Now, "Usuarios", "Creacion de usuario", BE_Evento_56PS.Criticidad.Bajo);
                    new BLL_BitacoraEvento_56PS().RegistrarEvento(eee);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al crear el usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                actualizar();
                LimpiarCampos();
                modo = "";
            }
        }

        void habilitarBotonCancelar()
        {
            button6.Enabled = true;
        }

        void deshabilitarBotonCancelar()
        {
            button6.Enabled = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //se cancela la operacion actual
            modo = "";

            LimpiarCampos();

            MessageBox.Show("Operacion cancelada");
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
            BLL_Usuario_56PS bl = new BLL_Usuario_56PS
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


        void habilitarBotonCrear()
        {
            button1.Enabled = true;
        }

        void deshabilitarBotonCrear()
        {
            button1.Enabled = false;
        }

        void habilitarBotonModificar()
        {
            button3.Enabled = true;
        }

        void deshabilitarBotonModificar()
        {
            button3 .Enabled = false;
        }

        void habilitarBotonActivarDesactivar()
        {
            btnActivarDesactivar.Enabled = true;
        }

        void deshabilitarBotonActivarDesactivar()
        {
            btnActivarDesactivar.Enabled= false;
        }

        void habilitarBotonDesbloquear()
        {
            button2.Enabled = true;
            
        }

        void deshabilitarBotonDesbloquear()
        {
            button2.Enabled = false;
        }




        private void FormABMUsuario_625NS_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnActivarDesactivar_Click(object sender, EventArgs e)
        {

            modo = "activar/desactivar";

            textBox5.Text = textBox5.Text = "Mensaje: \nModo activar/desactivar";

            habilitarBotonAplicar();
            habilitarBotonCancelar();


        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton1.Checked)
            {
                radioButton2.Checked = true;
            }
            else
            {
                radioButton2.Checked = false;
                dataGridView1.DataSource = listaGeneral.Where(x => x.Activo == true).ToList(); //mostramos solo los usuarios activos, no bloqueados

            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton2.Checked)
            {
            }
            else
            {
                radioButton1.Checked = false;
                radioButton3.Checked = false;
                dataGridView1.DataSource = listaGeneral;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {

                radioButton1.Checked= false;
                radioButton2.Checked= false;

                dataGridView1.DataSource = listaGeneral.Where(x => x.Bloqueado);


            }
            else
            {
                dataGridView1.DataSource= listaGeneral;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            modo = "desbloquear";

            textBox5.Text = "Mensaje: \nModo desbloquear";

            habilitarBotonAplicar();
            habilitarBotonCancelar();


        }
    }
}
