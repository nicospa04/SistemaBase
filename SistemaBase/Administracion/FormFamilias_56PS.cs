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

namespace SistemaBase.Administracion
{
    public partial class FormFamilias_56PS : Form, IdiomaObserver_56PS
    {
        BLL_Familia_56PS bllfamilia = new BLL_Familia_56PS();
        BLL_Perfil_56PS bllperfil = new BLL_Perfil_56PS();
        BLL_Patente_56PS bllpatentes = new BLL_Patente_56PS();
        List<Perfil_56PS> listapermisos = new List<Perfil_56PS>();
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
            button3.Click += button3_Click;
            button6.Click += button6_Click;
            button4.Click += button4_Click;
            btncancelar.Click += btncancelar_Click;
            btnbuscar.Click += btnbuscar_Click;
            button2.Click += button2_Click;
            btnmodificar.Click += btnmodificar_Click;

            SessionManager_56PS.getInstancia().Suscribir(this);
            actualizarIdioma();
        }

        // --- TRADUCCIONES ---

        public void actualizarIdioma()
        {
            var traductor = new BLL_Idioma_56PS();
            traductor.Traducir(this);
        }

        // --- DATOS ---

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
            foreach (Familia_56PS f in familias)
            {
                listapermisos.Add(f);
                cmbfa.Items.Add(f.Nombre);
            }
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
                foreach (Patente_56PS pat in patentes)
                {
                    listapermisos.Add(pat);
                    cmbpermiso.Items.Add(pat.Nombre);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en MostrarPermisos: " + ex.Message);
            }
        }

        // --- BOTONES ---

        // BOTON ASIGNAR PERMISO
        private void button3_Click(object sender, EventArgs e)
        {

        }

        // BOTON CREAR FAMILIA
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
                cmbfa.Text = null;
                cmbpermiso.Text = null;
                MostrarFamiliaEnTreeView(p.Codigo);
                MostrarFamiliasenDatagrid();
                cmbfa.Items.Clear();
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

        // BOTON ASIGNAR FAMILIA
        private void button2_Click(object sender, EventArgs e)
        {
            p = new Familia_56PS();
            if (cmbfa.SelectedItem == null)
            {
                MessageBox.Show("Seleccionar una familia");
                return;
            }

            string nombre = cmbfa.SelectedItem.ToString();
            string codigo = listapermisos.Where(x => x.Nombre.Equals(nombre)).Select(x => x.Codigo).FirstOrDefault();

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
            }

            Familia_56PS familia = new Familia_56PS();
            familia.esfamilia = true;
            familia.Nombre = cmbfa.SelectedItem.ToString();
            familia.Codigo = codigo;
            familia = bllfamilia.ObtenerFamilia(codigo);
            p.esfamilia = true;
            p.Codigo = codigofamilia;

            try
            {
                p.Agregar(familia);
                bllfamilia.AsignarPermisoAFamilia(p, familia);
                MostrarFamiliaEnTreeView(p.Codigo);
                cmbpermiso.SelectedIndex = -1;
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

        // BOTON CANCELAR
        private void button4_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            Limpiar();
        }

        public void Limpiar()
        {
            txtcod.Text = "";
            txtnomb.Text = "";
            cmbfa.Text = "";
            cmbpermiso.Text = "";
            p = new Familia_56PS();
        }

        // BOTON ELIMINAR FAMILIA
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

        // BOTON ELIMINAR PERMISO/FAMILIA DE FAMILIA
        private void button6_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("Debe seleccionar un permiso o familia para eliminar.");
                return;
            }

            TreeNode nodoseleccionado = treeView1.SelectedNode;
            TreeNode raiz = treeView1.Nodes[0];
            if (nodoseleccionado.Parent != raiz)
            {
                MessageBox.Show("Solo se pueden eliminar permisos directos de la familia.");
                return;
            }

            Perfil_56PS permiso = nodoseleccionado.Tag as Perfil_56PS;

            if (permiso == null)
            {
                MessageBox.Show("Error: No se pudo obtener la información del elemento seleccionado.");
                return;
            }

            bllfamilia.EliminarPermisodeFamilia(permiso, txtcod.Text);
            MessageBox.Show($"'{permiso.Nombre}' fue eliminado correctamente.");

            string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
            Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Familias", "Eliminación de permiso/familia de familia", Evento_56PS.Criticidad.Medio);
            new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);

            MostrarFamiliaEnTreeView(txtcod.Text);
        }

        // BOTON MODIFICAR NOMBRE
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
                cmbfa.Items.Clear();
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

        // CLICK EN DATAGRIDVIEW
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
            //BOTON ASIGNAR PERMISO
            p = new Familia_56PS();
            if (cmbpermiso.SelectedItem == null)
            {
                //MessageBox.Show("Seleccionar un permiso");
                MessageBox.Show("Seleccionar un permiso");
                return;
            }

            string nombre = cmbpermiso.SelectedItem.ToString();
            string codigo = listapermisos.Where(p => p.Nombre.Equals(nombre)).Select(p => p.Codigo).FirstOrDefault();
            if (string.IsNullOrEmpty(txtcod.Text))
            {
                //MessageBox.Show("Ingresar el código de la familia, seleccionar del datagrid");
                MessageBox.Show("Ingresar el código de la familia, seleccionar del datagrid");
                return;
            }

            string codigofamilia = txtcod.Text;
            //MessageBox.Show("el codigo es: " + codigo);
            try
            {
                p = bllfamilia.ObtenerFamilia(codigofamilia);
            }
            catch (Exception ex)
            {
                MessageBox.Show((ex.Message));
            }
            Patente_56PS patente = new Patente_56PS();
            patente.Nombre = nombre;
            patente.Codigo = codigo;
            patente.esfamilia = false;
            p.esfamilia = true;
            p.Codigo = codigofamilia;

            try
            {
                p.Agregar(patente);

                bllfamilia.AsignarPermisoAFamilia(p, patente);
                p = bllfamilia.ObtenerFamilia(codigofamilia);
                MostrarFamiliaEnTreeView(p.Codigo);
                cmbpermiso.SelectedIndex = -1;


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
                cmbfa.Text = null;
                cmbpermiso.Text = null;
                MostrarFamiliaEnTreeView(p.Codigo);
                MostrarFamiliasenDatagrid();
                cmbfa.Items.Clear();
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
                cmbfa.Items.Clear();
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
    }
}
