using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL;
using BE_625NS;
using ClassLibrary2;
using Servicio;
using ClassLibrary3;
using DAL_625NS;

namespace SistemaBase.Administracion
{
    public partial class FormFamilias_56PS : Form, IdiomaObserver_56PS
    {
        BLL_Familia_56PS bllfamilia = new BLL_Familia_56PS();
        BLL_Perfil_56PS bllperfil = new BLL_Perfil_56PS();
        BLL_Patente_56PS bllpatentes = new BLL_Patente_56PS();
        Familia_56PS p = new Familia_56PS();

        public FormFamilias_56PS()
        {
            InitializeComponent();

            cmbfa.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbpermiso.DropDownStyle = ComboBoxStyle.DropDownList;
            this.AutoScroll = true;
            MostrarFamilias();
            MostrarPermisos();
            MostrarFamiliasenDatagrid();

            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToOrderColumns = false;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ClearSelection();

            dataGridView1.CellClick += dataGridView1_CellClick;

            SessionManager_56PS.getInstancia().Suscribir(this);
            actualizarIdioma();
            AplicarPermisosAcciones();
        }

 
        public void actualizarIdioma()
        {
            var traductor = new BLL_Idioma_56PS();
            traductor.Traducir(this);
        }

 
        public void OcultarColumnas()
        {
            if (dataGridView1.Columns.Contains("esfamilia"))
            {
                dataGridView1.Columns["esfamilia"].Visible = false;
            }
            if (dataGridView1.Columns.Contains("activo"))
            {
                dataGridView1.Columns["activo"].Visible = false;
            }
            if (dataGridView1.Columns.Contains("hijos"))
            {
                dataGridView1.Columns["hijos"].Visible = false;
            }
        }

        public void MostrarFamiliasenDatagrid()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bllfamilia.ObtenerFamilias();
            OcultarColumnas();
        }

        public void MostrarFamilias()
        {
            List<Familia_56PS> familias = bllfamilia.ObtenerFamilias();
            cmbfa.DataSource = null;
            cmbfa.DisplayMember = "Nombre";
            cmbfa.ValueMember = "Codigo";
            cmbfa.DataSource = familias;
            cmbfa.SelectedIndex = -1;
        }

        private void MostrarFamiliaEnTreeView(string codigoFamilia)
        {
            treeView1.Nodes.Clear();
            try
            {
                var familiaRaiz = bllfamilia.ObtenerFamilia(codigoFamilia);
                TreeNode nodoRaiz = new TreeNode(familiaRaiz.Nombre);
                nodoRaiz.Tag = familiaRaiz;
                AgregarNodosRecursivos(nodoRaiz, familiaRaiz.hijos);
                treeView1.Nodes.Add(nodoRaiz);
                treeView1.ExpandAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AgregarNodosRecursivos(TreeNode nodoPadre, List<Perfil_56PS> hijos)
        {
            foreach (var hijo in hijos)
            {
                TreeNode nuevoNodo = new TreeNode(hijo.Nombre);
                nuevoNodo.Tag = hijo;
                nodoPadre.Nodes.Add(nuevoNodo);

                if (hijo.esfamilia && hijo.hijos != null && hijo.hijos.Count > 0)
                {
                    AgregarNodosRecursivos(nuevoNodo, hijo.hijos);
                }
            }
        }

        public void MostrarPermisos()
        {
            try
            {
                List<Patente_56PS> patentes = bllpatentes.ObtenerPatentes();
                cmbpermiso.DataSource = null;
                cmbpermiso.DisplayMember = "Nombre";
                cmbpermiso.ValueMember = "Codigo";
                cmbpermiso.DataSource = patentes;
                cmbpermiso.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en MostrarPermisos: " + ex.Message);
            }
        }

      
        private void button3_Click(object sender, EventArgs e)
        {

        }

         private void btnbuscar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtnomb.Text) || string.IsNullOrEmpty(txtcod.Text))
            {
                MessageBox.Show("Ingresar el nombre y código de la familia");
                return;
            }

            string pattern = @"^.{1,10}$";
            if (!Regex.IsMatch(txtcod.Text, pattern))
            {
                MessageBox.Show("El código no puede tener más de 10 caracteres.");
                return;
            }

            p.Codigo = txtcod.Text;
            p.activo = true;
            p.Nombre = txtnomb.Text;

            try
            {
                bllfamilia.CrearFamilia(p);
                MostrarFamiliaEnTreeView(p.Codigo);
                MostrarFamiliasenDatagrid();
                MostrarFamilias();
                Limpiar();

                string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
                Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Familias", "Creación de familia", Evento_56PS.Criticidad.Medio);
                new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

         private void button2_Click(object sender, EventArgs e)
        {
            p = new Familia_56PS();
            if (cmbfa.SelectedItem == null)
            {
                MessageBox.Show("Seleccionar una familia");
                return;
            }

            Familia_56PS familiaSeleccionada = cmbfa.SelectedItem as Familia_56PS;
            if (familiaSeleccionada == null)
            {
                MessageBox.Show("Seleccionar una familia");
                return;
            }

            if (string.IsNullOrEmpty(txtcod.Text))
            {
                MessageBox.Show("Ingresar el código de la familia, seleccionar del datagrid");
                return;
            }

            string codigofamilia = txtcod.Text;
            try
            {
                p = bllfamilia.ObtenerFamilia(codigofamilia);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            Familia_56PS familia;
            try
            {
                familia = bllfamilia.ObtenerFamilia(familiaSeleccionada.Codigo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            try
            {
                p.Agregar(familia);
                bllfamilia.AsignarPermisoAFamilia(p, familia);
                MostrarFamiliaEnTreeView(p.Codigo);
                cmbfa.SelectedIndex = -1;

                string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
                Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Familias", "Asignación de familia a familia", Evento_56PS.Criticidad.Medio);
                new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Limpiar();
        }

         private void button4_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            Limpiar();
        }

        public void Limpiar()
        {
            txtcod.Text = "";
            txtnomb.Text = "";
            cmbfa.SelectedIndex = -1;
            cmbpermiso.SelectedIndex = -1;
            p = new Familia_56PS();
        }

         private void btncancelar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtcod.Text))
            {
                MessageBox.Show("Ingresar el código de la familia");
                return;
            }

            Familia_56PS f = new Familia_56PS();
            f.Codigo = txtcod.Text;
            try
            {
                bllfamilia.EliminarFamilia(f);
                cmbfa.Items.Clear();
                MostrarFamilias();
                MostrarFamiliasenDatagrid();
                treeView1.Nodes.Clear();

                string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
                Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Familias", "Eliminación de familia", Evento_56PS.Criticidad.Alto);
                new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Limpiar();
        }

         private void button6_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("Debe seleccionar un permiso o familia para eliminar.");
                return;
            }

            TreeNode nodoseleccionado = treeView1.SelectedNode;
            if (nodoseleccionado.Parent == null)
            {
                MessageBox.Show("No se puede eliminar la familia raíz desde el árbol.");
                return;
            }

            Perfil_56PS permiso = nodoseleccionado.Tag as Perfil_56PS;

            if (permiso == null)
            {
                MessageBox.Show("Error: No se pudo obtener la información del elemento seleccionado.");
                return;
            }

            Familia_56PS familiaPadre = nodoseleccionado.Parent.Tag as Familia_56PS;
            if (familiaPadre == null)
            {
                MessageBox.Show("No se pudo obtener la familia que contiene el elemento seleccionado.");
                return;
            }

            try
            {
            bllfamilia.EliminarPermisodeFamilia(permiso, familiaPadre.Codigo);
            MessageBox.Show($"'{permiso.Nombre}' fue eliminado correctamente.");

            string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
            Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Familias", "Eliminación de permiso/familia de familia", Evento_56PS.Criticidad.Medio);
            new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);


            MostrarFamiliaEnTreeView(txtcod.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

         private void btnmodificar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtnomb.Text) || string.IsNullOrEmpty(txtcod.Text))
            {
                MessageBox.Show("Ingresar el nombre y código de la familia");
                return;
            }

            Familia_56PS f = new Familia_56PS();
            f.Nombre = txtnomb.Text;
            f.Codigo = txtcod.Text;
            try
            {
                bllfamilia.ModificarFamilia(f);
                MostrarFamiliaEnTreeView(f.Codigo);
                MostrarFamiliasenDatagrid();
                MostrarFamilias();
                treeView1.Nodes.Clear();
                txtcod.Text = "";
                txtnomb.Text = "";

                string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
                Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Familias", "Modificación de familia", Evento_56PS.Criticidad.Medio);
                new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);

           
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

         private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string cod = row.Cells["Codigo"].Value.ToString();
                string nom = row.Cells["Nombre"].Value.ToString();
                txtcod.Text = cod;
                txtnomb.Text = nom;
                MostrarFamiliaEnTreeView(cod);
            }
        }

        private void FormFamilias_56PS_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
             p = new Familia_56PS();
            if (cmbpermiso.SelectedItem == null)
            {
                 MessageBox.Show("Seleccionar un permiso");
                return;
            }

            Patente_56PS patenteSeleccionada = cmbpermiso.SelectedItem as Patente_56PS;
            if (patenteSeleccionada == null)
            {
                MessageBox.Show("Seleccionar un permiso");
                return;
            }
            if (string.IsNullOrEmpty(txtcod.Text))
            {
                 MessageBox.Show("Ingresar el código de la familia, seleccionar del datagrid");
                return;
            }

            string codigofamilia = txtcod.Text;
             try
            {
                p = bllfamilia.ObtenerFamilia(codigofamilia);
            }
            catch (Exception ex)
            {
                MessageBox.Show((ex.Message));
                return;
            }
            Patente_56PS patente = new Patente_56PS();
            patente.Nombre = patenteSeleccionada.Nombre;
            patente.Codigo = patenteSeleccionada.Codigo;
            patente.esfamilia = false;

            try
            {
                p.Agregar(patente);

                bllfamilia.AsignarPermisoAFamilia(p, patente);
                p = bllfamilia.ObtenerFamilia(codigofamilia);
                MostrarFamiliaEnTreeView(p.Codigo);
            }
            catch (Exception ex)
            {
                MessageBox.Show((ex.Message));
            }

            Limpiar();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnbuscar_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtnomb.Text) || string.IsNullOrEmpty(txtcod.Text))
            {
                MessageBox.Show("Ingresar el nombre y código de la familia");
                return;
            }

            string pattern = @"^.{1,10}$";
            if (!Regex.IsMatch(txtcod.Text, pattern))
            {
                MessageBox.Show("El código no puede tener más de 10 caracteres.");
                return;
            }

            p.Codigo = txtcod.Text;
            p.activo = true;
            p.Nombre = txtnomb.Text;

            try
            {
                bllfamilia.CrearFamilia(p);
                MostrarFamiliaEnTreeView(p.Codigo);
                MostrarFamiliasenDatagrid();
                MostrarFamilias();
                Limpiar();

                string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
                Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Familias", "Creación de familia", Evento_56PS.Criticidad.Medio);
                new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnmodificar_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtnomb.Text) || string.IsNullOrEmpty(txtcod.Text))
            {
                MessageBox.Show("Ingresar el nombre y código de la familia");
                return;
            }

            Familia_56PS f = new Familia_56PS();
            f.Nombre = txtnomb.Text;
            f.Codigo = txtcod.Text;
            try
            {
                bllfamilia.ModificarFamilia(f);
                MostrarFamiliaEnTreeView(f.Codigo);
                MostrarFamiliasenDatagrid();
                MostrarFamilias();
                treeView1.Nodes.Clear();
                txtcod.Text = "";
                txtnomb.Text = "";

                string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
                Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Familias", "Modificación de familia", Evento_56PS.Criticidad.Medio);
                new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btncancelar_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtcod.Text))
            {
                MessageBox.Show("Ingresar el código de la familia");
                return;
            }

            Familia_56PS f = new Familia_56PS();
            f.Codigo = txtcod.Text;
            try
            {
                bllfamilia.EliminarFamilia(f);
                MostrarFamilias();
                MostrarFamiliasenDatagrid();
                treeView1.Nodes.Clear();

                string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
                Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Familias", "Eliminación de familia", Evento_56PS.Criticidad.Alto);
                new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Limpiar();
        }

        private void AplicarPermisosAcciones()
        {
            btnbuscar.Enabled = TienePermiso(Permisos_56PS.CrearFamilia);
            btnmodificar.Enabled = TienePermiso(Permisos_56PS.ModificarFamilia);
            btncancelar.Enabled = TienePermiso(Permisos_56PS.EliminarFamilia);
            button1.Enabled = TienePermiso(Permisos_56PS.AsignarPatenteAFamilia);
            button2.Enabled = TienePermiso(Permisos_56PS.AsignarFamiliaAFamilia);
            button6.Enabled = TienePermiso(Permisos_56PS.QuitarPermisoDeFamilia);
        }

        private bool TienePermiso(string permiso)
        {
            var usuario = SessionManager_56PS.getInstancia().getUsuarioActivo();
            return usuario?.Perfil != null && usuario.Perfil.TienePermiso(permiso);
        }
    }
}
