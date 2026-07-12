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
            CargarErrores();
            btnrecalcular.Enabled = TienePermiso(Permisos_56PS.RecalcularDigitosVerificadores);
            btnrestore.Enabled = TienePermiso(Permisos_56PS.RestaurarBackup);
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
                var revision = blldv.Revision();
                Errores = revision.tablasConError;

                if (revision.tablaDVVacia && Errores.Count == 0)
                    Errores.Add("Tabla DigitoVerificador sin registros");

                CargarErrores();

                if (revision.tablaDVVacia || Errores.Count > 0)
                {
                    MessageBox.Show("Se recalcularon los DV, pero todavia se detectan inconsistencias.");
                    return;
                }

                MessageBox.Show("Se recalcularon los DV correctamente. Debe iniciar sesión nuevamente.");
                CerrarSesionYVolverAlMenu();
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

                    MessageBox.Show("Restauración realizada correctamente. El sistema se cerrará para volver a iniciar con la base restaurada.");

                    string aa = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;

                    Evento_56PS ee = new Evento_56PS(aa, DateTime.Now, "Reparaciones", "Restauracion de respaldo realizada", Evento_56PS.Criticidad.MuyAlto);
                    new BLL_BitacoraEvento_56PS().RegistrarEvento(ee);

                    SessionManager_56PS.getInstancia().SolicitarCierreSistema();

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
            CerrarSesionYVolverAlMenu();
        }

        private bool TienePermiso(string permiso)
        {
            var usuario = SessionManager_56PS.getInstancia().getUsuarioActivo();
            return usuario?.Perfil != null && usuario.Perfil.TienePermiso(permiso);
        }

        private void CargarErrores()
        {
            listBox1.Items.Clear();

            if (Errores == null || Errores.Count == 0)
            {
                listBox1.Items.Add("No se detectaron tablas con error.");
                return;
            }

            foreach (string error in Errores)
            {
                listBox1.Items.Add(error);
            }
        }

        private void CerrarSesionYVolverAlMenu()
        {
            SessionManager_56PS.getInstancia().cerrarSesion();

            MenuPrincipal_56PS menu = this.MdiParent as MenuPrincipal_56PS;
            if (menu == null)
                menu = Application.OpenForms["MenuPrincipal_56PS"] as MenuPrincipal_56PS;

            if (menu != null)
                menu.AplicarEstadoSinSesion();

            this.Close();
        }
    }
}
