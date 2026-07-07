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

namespace SistemaBase.Administracion
{
    public partial class FormReparacion_56PS : Form, IdiomaObserver_56PS
    {
        BLL_BackUpRestore_56PS bllrespaldos = new BLL_BackUpRestore_56PS();
        BLL_Usuario_56PS bllusuario = new BLL_Usuario_56PS();
        BLL_DigitoVerificador_56PS blldv = new BLL_DigitoVerificador_56PS();
        private List<string> Errores;

        public FormReparacion_56PS(List<string> errores)
        {
            Errores = errores;

            InitializeComponent();



            SessionManager_56PS.getInstancia().Suscribir(this);
            actualizarIdioma();
        }

        public void actualizarIdioma()
        {
            var traductor = new BLL_Idioma_56PS();
            traductor.Traducir(this);
        }

        private void FormReparacion_56PS_Load(object sender, EventArgs e)
        {

        }

        private void btnrecalcular_Click(object sender, EventArgs e)
        {
            try
            {
                blldv.RecalcularDV();
                MessageBox.Show("Se guardaron los DV en la tabla");//""



                var a = SessionManager_56PS.getInstancia();
                a.cerrarSesion();
                MenuPrincipal_56PS frm = new MenuPrincipal_56PS();
                frm.Show();
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnrestore_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Filter = "Archivos de respaldo (*.bak)|*.bak";
                openFile.Title = "Seleccionar archivo de respaldo";

                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    string ruta = openFile.FileName;

                    new BLL_BackUpRestore_56PS().RealizarRestore(ruta);

                    MessageBox.Show("Restauración realizada correctamente");

                    string aa = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;

                    Evento_56PS ee = new Evento_56PS(aa, DateTime.Now, "Reparaciones", "Restauracion de respaldo realizada", Evento_56PS.Criticidad.MuyAlto);
                    new BLL_BitacoraEvento_56PS().RegistrarEvento(ee);


                    var a = SessionManager_56PS.getInstancia();
                    a.cerrarSesion();
                    MenuPrincipal_56PS frm = new MenuPrincipal_56PS();
                    frm.Show();
                    this.Close();

                }
                else
                {
                    MessageBox.Show("Debe seleccionar un archivo de respaldo"); // 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnsalir_Click(object sender, EventArgs e)
        {
            var a = SessionManager_56PS.getInstancia();
            a.cerrarSesion();
            MenuPrincipal_56PS frm = new MenuPrincipal_56PS();
            frm.Show();
            this.Close();
        }
    }
}
