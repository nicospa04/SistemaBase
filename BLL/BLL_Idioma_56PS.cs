using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;
using ClassLibrary2;
using Servicio;


namespace BLL
{
    public class BLL_Idioma_56PS
    {
        public Dictionary<string, string> obtenerIdioma(string tipo)
        {
            if (tipo == "POR") { tipo = "PT"; }

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{tipo}.json");
            if (!File.Exists(path)) return new Dictionary<string, string>();

            string json = File.ReadAllText(path, Encoding.UTF8);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public void Traducir(Form formulario)
        {
            string tipo = ObtenerTipoDesdeSession();
            var traducciones = obtenerIdioma(tipo);
            TraducirControles(formulario.Controls, traducciones);
        }

        private void TraducirControles(Control.ControlCollection controls, Dictionary<string, string> traducciones)
        {
            foreach (Control ctrl in controls)
            {
                if (!string.IsNullOrEmpty(ctrl.Tag?.ToString()) && traducciones.ContainsKey(ctrl.Tag.ToString()))
                    ctrl.Text = traducciones[ctrl.Tag.ToString()];

                if (ctrl is MenuStrip menuStrip)
                    TraducirItemsMenu(menuStrip.Items, traducciones);

                if (ctrl is DataGridView dataGridView)
                {
                    TraducirColumnas(dataGridView, traducciones);
                    dataGridView.DataBindingComplete -= DataGridView_DataBindingComplete;
                    dataGridView.DataBindingComplete += DataGridView_DataBindingComplete;
                }

                if (ctrl.HasChildren)
                    TraducirControles(ctrl.Controls, traducciones);
            }
        }

        private void TraducirColumnas(DataGridView dataGridView, Dictionary<string, string> traducciones)
        {
            foreach (DataGridViewColumn columna in dataGridView.Columns)
            {
                string nombre = string.IsNullOrWhiteSpace(columna.DataPropertyName)
                    ? columna.Name
                    : columna.DataPropertyName;
                string clave = "grid" + nombre;

                if (traducciones.ContainsKey(clave))
                    columna.HeaderText = traducciones[clave];
                else
                {
                    KeyValuePair<string, string> traduccion = traducciones.FirstOrDefault(item => string.Equals(item.Key, clave, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrEmpty(traduccion.Key))
                        columna.HeaderText = traduccion.Value;
                }
            }
        }

        private static void DataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;
            if (dataGridView == null)
                return;

            BLL_Idioma_56PS traductor = new BLL_Idioma_56PS();
            traductor.TraducirColumnas(dataGridView, traductor.obtenerIdioma(traductor.ObtenerTipoDesdeSession()));
        }

        private void TraducirItemsMenu(ToolStripItemCollection items, Dictionary<string, string> traducciones)
        {
            foreach (ToolStripItem item in items)
            {
                if (!string.IsNullOrEmpty(item.Tag?.ToString()) && traducciones.ContainsKey(item.Tag.ToString()))
                    item.Text = traducciones[item.Tag.ToString()];

                if (item is ToolStripMenuItem menuItem && menuItem.DropDownItems.Count > 0)
                    TraducirItemsMenu(menuItem.DropDownItems, traducciones);
            }
        }

        private string ObtenerTipoDesdeSession()
        {
            Idioma_56PS idiomaActual = SessionManager_56PS.getInstancia().idiomaActual;
            return idiomaActual == null || string.IsNullOrEmpty(idiomaActual.tipo) ? "ES" : idiomaActual.tipo;
        }

    }
}
