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
using ClassLibrary3;
using DAL_625NS;

namespace SistemaBase.Administracion
{
    public partial class FormRoles_56PS : Form, IdiomaObserver_56PS
    {
        BLL_Rol_56PS bllrol = new BLL_Rol_56PS();
        BLL_Patente_56PS bllpatente = new BLL_Patente_56PS();
        BLL_Familia_56PS bllfamilia = new BLL_Familia_56PS();
        Rol_56PS p = new Rol_56PS();

        public FormRoles_56PS()
        {
            InitializeComponent();

            cmbpermiso.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbfa.DropDownStyle = ComboBoxStyle.DropDownList;
            this.AutoScroll = true;

            MostrarFamilias();
            MostrarTodosLosRoles();
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

            SessionManager_56PS.getInstancia().Suscribir(this);
            actualizarIdioma();
            AplicarPermisosAcciones();
        }

 
        public void actualizarIdioma()
        {
            var traductor = new BLL_Idioma_56PS();
            traductor.Traducir(this);
        }

 
        public void MostrarTodosLosRoles()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bllrol.ObtenerTodosLosRoles();
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
            cmbfa.DataSource = null;
            cmbfa.DisplayMember = "Nombre";
            cmbfa.ValueMember = "Codigo";
            cmbfa.DataSource = familias;
            cmbfa.SelectedIndex = -1;
        }

        public void MostrarPermisos()
        {
            List<Patente_56PS> patentes = bllpatente.ObtenerPatentes();
            cmbpermiso.DataSource = null;
            cmbpermiso.DisplayMember = "Nombre";
            cmbpermiso.ValueMember = "Codigo";
            cmbpermiso.DataSource = patentes;
            cmbpermiso.SelectedIndex = -1;
        }

        private void MostrarRolEnTreeView(string cod)
        {
            treeView1.Nodes.Clear();
            var rol = bllrol.ObtenerRol(cod);

            TreeNode raiz = new TreeNode(rol.Nombre);
            raiz.Tag = rol;

            AgregarNodosRecursivos(raiz, rol.hijos);

            treeView1.Nodes.Add(raiz);
            treeView1.ExpandAll();
        }

        private void AgregarNodosRecursivos(TreeNode nodoPadre, List<Rol_56PS> hijos)
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

   
        private void button3_Click(object sender, EventArgs e)
        {
            p = new Rol_56PS();
            if (cmbpermiso.SelectedItem == null)
            {
                new BLL_Idioma_56PS().MostrarMensaje("Seleccionar un permiso");
                return;
            }

            Patente_56PS patenteSeleccionada = cmbpermiso.SelectedItem as Patente_56PS;
            if (patenteSeleccionada == null)
            {
                new BLL_Idioma_56PS().MostrarMensaje("Seleccionar un permiso");
                return;
            }

            if (string.IsNullOrEmpty(txtcod.Text))
            {
                new BLL_Idioma_56PS().MostrarMensaje("Ingresar el código del rol, seleccionar del datagrid");
                return;
            }

            string codrol = txtcod.Text;
            try
            {
                p = bllrol.ObtenerRol(codrol);
            }
            catch (Exception ex)
            {
                new BLL_Idioma_56PS().MostrarMensaje(ex.Message);
                return;
            }

            Patente_56PS patente = new Patente_56PS();
            patente.Nombre = patenteSeleccionada.Nombre;
            patente.Codigo = patenteSeleccionada.Codigo;
            patente.esfamilia = false;
            p.esfamilia = true;
            p.Codigo = codrol;

            try
            {
                p.Agregar(patente);
                bllrol.AsignarPermisosARol(p, patente);
                MostrarRolEnTreeView(p.Codigo);

                string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
                Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Roles", "Asignación de permiso a rol", Evento_56PS.Criticidad.Medio);
                new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);
            }
            catch (Exception ex)
            {
                new BLL_Idioma_56PS().MostrarMensaje(ex.Message);
            }
            Limpiar();
        }

        private void btnbuscar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtnomb.Text) || string.IsNullOrEmpty(txtcod.Text))
            {
                new BLL_Idioma_56PS().MostrarMensaje("Ingresar el nombre y código del rol");
                return;
            }

            List<Rol_56PS> elementosIniciales = ObtenerElementosIniciales();
            if (elementosIniciales.Count == 0)
            {
                new BLL_Idioma_56PS().MostrarMensaje("Debe seleccionar al menos un permiso o una familia para crearlo.");
                return;
            }

            p.Codigo = txtcod.Text;
            p.Nombre = txtnomb.Text;
            p.activo = true;

            try
            {
                bllrol.CrearRol(p, elementosIniciales);
                cmbfa.Text = null;
                cmbpermiso.Text = null;
                MostrarTodosLosRoles();
                MostrarRolEnTreeView(p.Codigo);
                Limpiar();

                string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
                Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Roles", "Creación de rol", Evento_56PS.Criticidad.Medio);
                new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);
            }
            catch (Exception ex)
            {
                new BLL_Idioma_56PS().MostrarMensaje(ex.Message);
            }
        }

        private List<Rol_56PS> ObtenerElementosIniciales()
        {
            List<Rol_56PS> elementos = new List<Rol_56PS>();

            Patente_56PS patente = cmbpermiso.SelectedItem as Patente_56PS;
            if (patente != null)
            {
                elementos.Add(new Patente_56PS
                {
                    Codigo = patente.Codigo,
                    Nombre = patente.Nombre,
                    esfamilia = false
                });
            }

            Familia_56PS familia = cmbfa.SelectedItem as Familia_56PS;
            if (familia != null)
            {
                elementos.Add(new Familia_56PS
                {
                    Codigo = familia.Codigo,
                    Nombre = familia.Nombre,
                    esfamilia = true
                });
            }

            return elementos;
        }

         private void button2_Click(object sender, EventArgs e)
        {
            p = new Rol_56PS();
            if (cmbfa.SelectedItem == null)
            {
                new BLL_Idioma_56PS().MostrarMensaje("Seleccionar una familia");
                return;
            }

            Familia_56PS familiaSeleccionada = cmbfa.SelectedItem as Familia_56PS;
            if (familiaSeleccionada == null)
            {
                new BLL_Idioma_56PS().MostrarMensaje("Seleccionar una familia");
                return;
            }

            if (string.IsNullOrEmpty(txtcod.Text))
            {
                new BLL_Idioma_56PS().MostrarMensaje("Ingresar el código del rol, seleccionar del datagrid");
                return;
            }

            string codigorol = txtcod.Text;
            try
            {
                p = bllrol.ObtenerRol(codigorol);
            }
            catch (Exception ex)
            {
                new BLL_Idioma_56PS().MostrarMensaje(ex.Message);
                return;
            }

            Familia_56PS familia = new Familia_56PS
            {
                esfamilia = true,
                Nombre = familiaSeleccionada.Nombre,
                Codigo = familiaSeleccionada.Codigo
            };

            try
            {
                p.Agregar(familia);
                bllrol.AsignarPermisosARol(p, familia);
                MostrarRolEnTreeView(p.Codigo);

                string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
                Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Roles", "Asignación de familia a rol", Evento_56PS.Criticidad.Medio);
                new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);
            }
            catch (Exception ex)
            {
                new BLL_Idioma_56PS().MostrarMensaje(ex.Message);
            }
            Limpiar();
        }

         private void button4_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        public void Limpiar()
        {
            p = new Rol_56PS();
            txtcod.Text = null;
            txtnomb.Text = null;
            cmbfa.SelectedIndex = -1;
            cmbpermiso.SelectedIndex = -1;
        }

         private void btncancelar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtcod.Text))
            {
                new BLL_Idioma_56PS().MostrarMensaje("Ingresar el código del rol");
                return;
            }
            Rol_56PS rol = new Rol_56PS();
            rol.Codigo = txtcod.Text;
            try
            {
                bllrol.EliminarRol(rol);

                string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
                Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Roles", "Eliminación de rol", Evento_56PS.Criticidad.Alto);
                new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);
            }   
            catch (Exception ex)
            {
                new BLL_Idioma_56PS().MostrarMensaje(ex.Message);
            }
            MostrarTodosLosRoles();
            Limpiar();
        }

         private void button6_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                new BLL_Idioma_56PS().MostrarMensaje("Debe seleccionar un permiso o familia para eliminar.");
                return;
            }

            TreeNode nodoseleccionado = treeView1.SelectedNode;
            TreeNode raiz = treeView1.Nodes[0];
            if (nodoseleccionado.Parent != raiz)
            {
                new BLL_Idioma_56PS().MostrarMensaje("Solo se pueden eliminar permisos directos del rol.");
                return;
            }

            Rol_56PS permiso = treeView1.SelectedNode.Tag as Rol_56PS;

            if (permiso == null)
            {
                new BLL_Idioma_56PS().MostrarMensaje("Error: No se pudo obtener la información del elemento seleccionado.");
                return;
            }

            try
            {
            bllrol.EliminarPermisodeRol(permiso, txtcod.Text);
            new BLL_Idioma_56PS().MostrarMensaje($"Permiso/Familia: '{permiso.Nombre}' eliminada correctamente.");

            string dniUser = SessionManager_56PS.getInstancia().getUsuarioActivo().Dni;
            Evento_56PS ev = new Evento_56PS(dniUser, DateTime.Now, "Roles", "Eliminación de permiso/familia de rol", Evento_56PS.Criticidad.Medio);
            new BLL_BitacoraEvento_56PS().RegistrarEvento(ev);



            treeView1.SelectedNode.Remove();
            }
            catch (Exception ex)
            {
                new BLL_Idioma_56PS().MostrarMensaje(ex.Message);
            }
        }

         private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string cod = row.Cells["Codigo"].Value.ToString();
                txtcod.Text = cod;
                MostrarRolEnTreeView(cod);
            }
        }

        private void cmbfa_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FormRoles_56PS_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
             p = new Rol_56PS();
            if (cmbpermiso.SelectedItem == null)
            {
                new BLL_Idioma_56PS().MostrarMensaje("Seleccionar un permiso");

                return;
            }

            Patente_56PS patenteSeleccionada = cmbpermiso.SelectedItem as Patente_56PS;
            if (patenteSeleccionada == null)
            {
                new BLL_Idioma_56PS().MostrarMensaje("Seleccionar un permiso");
                return;
            }

            if (string.IsNullOrEmpty(txtcod.Text))
            {
                new BLL_Idioma_56PS().MostrarMensaje("Ingresar el código del rol, seleccionar del datagrid");
                return;
            }

            string codrol = txtcod.Text;
            try
            {
                p = bllrol.ObtenerRol(codrol);
            }
            catch (Exception ex)
            {
                new BLL_Idioma_56PS().MostrarMensaje(ex.Message);
                return;
            }

 
            Patente_56PS patente = new Patente_56PS();
            patente.Nombre = patenteSeleccionada.Nombre;
            patente.Codigo = patenteSeleccionada.Codigo;
            patente.esfamilia = false;
            p.esfamilia = true;
            p.Codigo = codrol;
            try
            {
                p.Agregar(patente);
                bllrol.AsignarPermisosARol(p, patente);
                MostrarRolEnTreeView(p.Codigo);

             
            }
            catch (Exception ex)
            {
                new BLL_Idioma_56PS().MostrarMensaje((ex.Message));
            }
            Limpiar();
        }

        private void cmbpermiso_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void AplicarPermisosAcciones()
        {
            btnbuscar.Enabled = TienePermiso(Permisos_56PS.CrearRol);
            btncancelar.Enabled = TienePermiso(Permisos_56PS.EliminarRol);
            button3.Enabled = TienePermiso(Permisos_56PS.AsignarPatenteARol);
            button2.Enabled = TienePermiso(Permisos_56PS.AsignarFamiliaARol);
            button6.Enabled = TienePermiso(Permisos_56PS.QuitarPermisoDeRol);
        }

        private bool TienePermiso(string permiso)
        {
            var usuario = SessionManager_56PS.getInstancia().getUsuarioActivo();
            return usuario?.Rol != null && usuario.Rol.TienePermiso(permiso);
        }
    }
}
