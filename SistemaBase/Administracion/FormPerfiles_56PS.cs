using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL;
using BE_625NS;
using ClassLibrary2;
using Servicio;

namespace SistemaBase.Administracion
{
    public partial class FormPerfiles_56PS : Form, IdiomaObserver_56PS
    {
        BLL_Perfil_56PS bllperfil = new BLL_Perfil_56PS();
        List<Perfil_56PS> listapermisos = new List<Perfil_56PS>();
        BLL_Patente_56PS bllpatente = new BLL_Patente_56PS();
        BLL_Familia_56PS bllfamilia = new BLL_Familia_56PS();
        Perfil_56PS p = new Perfil_56PS();

        public FormPerfiles_56PS()
        {
            InitializeComponent();

            cmbpermiso.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbfa.DropDownStyle = ComboBoxStyle.DropDownList;

            MostrarFamilias();
            MostrarTodosLosPerfiles();
            MostrarPermisos();

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

            SessionManager_56PS.getInstancia().Suscribir(this);
            actualizarIdioma();
        }

        // --- TRADUCCIONES ---

        public void actualizarIdioma()
        {
            var traductor = new BLL_Idioma_56PS();
            traductor.Traducir_625NS(this);
        }

        // --- DATOS ---

        public void MostrarTodosLosPerfiles()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bllperfil.ObtenerTodosLosPerfiles();
            OcultarColumnas();
        }

        public void OcultarColumnas()
        {
            if (dataGridView1.Columns.Contains("esfamilia"))
            {
                dataGridView1.Columns["esfamilia"].Visible = false;
            }
            if (dataGridView1.Columns.Contains("hijos"))
            {
                dataGridView1.Columns["hijos"].Visible = false;
            }
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

        public void MostrarPermisos()
        {
            List<Patente_56PS> patentes = bllpatente.ObtenerPatentes();
            foreach (Patente_56PS pat in patentes)
            {
                listapermisos.Add(pat);
                cmbpermiso.Items.Add(pat.Nombre);
            }
        }

        private void MostrarPerfilEnTreeView(string cod)
        {
            treeView1.Nodes.Clear();
            var perfil = bllperfil.ObtenerPerfil(cod);

            TreeNode raiz = new TreeNode(perfil.Nombre);
            raiz.Tag = perfil;

            AgregarNodosRecursivos(raiz, perfil.hijos);

            treeView1.Nodes.Add(raiz);
            treeView1.ExpandAll();
        }

        private void AgregarNodosRecursivos(TreeNode nodoPadre, List<Perfil_56PS> hijos)
        {
            foreach (var item in hijos)
            {
                TreeNode nuevoNodo = new TreeNode(item.Nombre);
                nuevoNodo.Tag = item;
                nodoPadre.Nodes.Add(nuevoNodo);

                if (item.esfamilia && item.hijos != null)
                {
                    AgregarNodosRecursivos(nuevoNodo, item.hijos);
                }
            }
        }

        // --- BOTONES ---

        // BOTON ASIGNAR PERMISO
        private void button3_Click(object sender, EventArgs e)
        {
            p = new Perfil_56PS();
            if (cmbpermiso.SelectedItem == null)
            {
                MessageBox.Show("Seleccionar un permiso");
                return;
            }

            string nombre = cmbpermiso.SelectedItem.ToString();
            string codigo = listapermisos.Where(x => x.Nombre.Equals(nombre)).Select(x => x.Codigo).FirstOrDefault();

            if (string.IsNullOrEmpty(txtcod.Text))
            {
                MessageBox.Show("Ingresar el código del perfil, seleccionar del datagrid");
                return;
            }

            string codperfil = txtcod.Text;
            try
            {
                p = bllperfil.ObtenerPerfil(codperfil);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            Patente_56PS patente = new Patente_56PS();
            patente.Nombre = nombre;
            patente.Codigo = codigo;
            patente.esfamilia = false;
            p.esfamilia = true;
            p.Codigo = codperfil;

            try
            {
                p.Agregar(patente);
                bllperfil.AsignarPermisosAPerfil(p, patente);
                MostrarPerfilEnTreeView(p.Codigo);

                string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
                Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Perfiles", "Asignación de permiso a perfil", Evento_56PS.Criticidad.Medio);
                new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Limpiar();
        }

        // BOTON CREAR PERFIL
        private void btnbuscar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtnomb.Text) || string.IsNullOrEmpty(txtcod.Text))
            {
                MessageBox.Show("Ingresar el nombre y código del perfil");
                return;
            }

            p.Codigo = txtcod.Text;
            p.Nombre = txtnomb.Text;
            p.activo = true;

            try
            {
                bllperfil.CrearPerfil(p);
                cmbfa.Text = null;
                cmbpermiso.Text = null;
                MostrarTodosLosPerfiles();
                MostrarPerfilEnTreeView(p.Codigo);
                Limpiar();

                string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
                Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Perfiles", "Creación de perfil", Evento_56PS.Criticidad.Medio);
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
            p = new Perfil_56PS();
            if (cmbfa.SelectedItem == null)
            {
                MessageBox.Show("Seleccionar una familia");
                return;
            }

            string nombre = cmbfa.SelectedItem.ToString();
            string codigo = listapermisos.Where(x => x.Nombre.Equals(nombre)).Select(x => x.Codigo).FirstOrDefault();

            if (string.IsNullOrEmpty(txtcod.Text))
            {
                MessageBox.Show("Ingresar el código del perfil, seleccionar del datagrid");
                return;
            }

            string codigoperfil = txtcod.Text;
            try
            {
                p = bllperfil.ObtenerPerfil(codigoperfil);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Familia_56PS familia = new Familia_56PS();
            familia.esfamilia = true;
            familia.Nombre = cmbfa.SelectedItem.ToString();
            familia.Codigo = codigo;
            p.esfamilia = true;
            p.Codigo = txtcod.Text;

            try
            {
                p.Agregar(familia);
                bllperfil.AsignarPermisosAPerfil(p, familia);
                MostrarPerfilEnTreeView(p.Codigo);

                string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
                Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Perfiles", "Asignación de familia a perfil", Evento_56PS.Criticidad.Medio);
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
            Limpiar();
        }

        public void Limpiar()
        {
            p = new Perfil_56PS();
            txtcod.Text = null;
            txtnomb.Text = null;
            cmbfa.Text = "";
            cmbpermiso.Text = "";
        }

        // BOTON ELIMINAR PERFIL
        private void btncancelar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtcod.Text))
            {
                MessageBox.Show("Ingresar el código del perfil");
                return;
            }
            Perfil_56PS perfil = new Perfil_56PS();
            perfil.Codigo = txtcod.Text;
            try
            {
                bllperfil.EliminarPerfil(perfil);

                string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
                Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Perfiles", "Eliminación de perfil", Evento_56PS.Criticidad.Alto);
                new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MostrarTodosLosPerfiles();
            Limpiar();
        }

        // BOTON ELIMINAR PERMISO/FAMILIA DE PERFIL
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
                MessageBox.Show("Solo se pueden eliminar permisos directos del perfil.");
                return;
            }

            Perfil_56PS permiso = treeView1.SelectedNode.Tag as Perfil_56PS;

            if (permiso == null)
            {
                MessageBox.Show("Error: No se pudo obtener la información del elemento seleccionado.");
                return;
            }

            bllperfil.EliminarPermisodePerfil(permiso, txtcod.Text);
            MessageBox.Show($"Permiso/Familia: '{permiso.Nombre}' eliminada correctamente.");

            string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
            Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Perfiles", "Eliminación de permiso/familia de perfil", Evento_56PS.Criticidad.Medio);
            new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);

            treeView1.SelectedNode.Remove();
        }

        // CLICK EN DATAGRIDVIEW
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string cod = row.Cells["Codigo"].Value.ToString();
                txtcod.Text = cod;
                MostrarPerfilEnTreeView(cod);
            }
        }

        private void cmbfa_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FormPerfiles_56PS_Load(object sender, EventArgs e)
        {

        }
    }
}
