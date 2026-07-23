using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;
using ClassLibrary2;
using DAL_625NS;
using Servicio;


namespace BLL
{
    public class BLL_Idioma_56PS
    {
        private readonly DAL_Idioma_56PS dalIdioma = new DAL_Idioma_56PS();

        public List<Idioma_56PS> ObtenerIdiomas()
        {
            return dalIdioma.ObtenerIdiomas();
        }

        public Idioma_56PS ObtenerIdioma(string tipo)
        {
            Idioma_56PS idioma = dalIdioma.ObtenerIdioma(tipo);
            return idioma ?? new Idioma_56PS("ES");
        }

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

        public string TraducirMensaje(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
                return mensaje;

            string mensajeNormalizado = NormalizarTexto(mensaje).Trim();
            Dictionary<string, string> traducciones = obtenerIdioma(ObtenerTipoDesdeSession());
            if (traducciones.ContainsKey(mensajeNormalizado))
                return traducciones[mensajeNormalizado];

            string mensajeConDetalle = TraducirMensajeConDetalle(mensajeNormalizado, traducciones);
            return string.IsNullOrEmpty(mensajeConDetalle)
                ? mensajeNormalizado
                : mensajeConDetalle;
        }

        public string TraducirFormato(string plantilla, params object[] argumentos)
        {
            return string.Format(TraducirMensaje(plantilla), argumentos);
        }

        public DialogResult MostrarMensaje(string texto)
        {
            return MessageBox.Show(TraducirMensaje(texto));
        }

        public DialogResult MostrarMensaje(string texto, string titulo, MessageBoxButtons botones, MessageBoxIcon icono)
        {
            return MessageBox.Show(TraducirMensaje(texto), TraducirMensaje(titulo), botones, icono);
        }

        private string TraducirMensajeConDetalle(string mensaje, Dictionary<string, string> traducciones)
        {
            const string inicioInicializacion = "No se pudo inicializar la base de datos. Instancia: ";
            const string separadorInicializacion = ". Verifique que SQL Server este iniciado y que el nombre sea correcto. Detalle: ";
            const string plantillaInicializacion = "No se pudo inicializar la base de datos. Instancia: {0}. Verifique que SQL Server este iniciado y que el nombre sea correcto. Detalle: {1}";

            if (mensaje.StartsWith(inicioInicializacion) && mensaje.Contains(separadorInicializacion))
            {
                int indiceDetalle = mensaje.IndexOf(separadorInicializacion, StringComparison.Ordinal);
                string instancia = mensaje.Substring(inicioInicializacion.Length, indiceDetalle - inicioInicializacion.Length);
                string detalle = mensaje.Substring(indiceDetalle + separadorInicializacion.Length);

                if (traducciones.ContainsKey(plantillaInicializacion))
                    return string.Format(traducciones[plantillaInicializacion], instancia, TraducirMensaje(detalle));
            }

            const string inicioTablaDv = "La tabla ";
            const string finTablaDv = " no esta habilitada para calculo de DV.";
            const string plantillaTablaDv = "La tabla {0} no esta habilitada para calculo de DV.";
            if (mensaje.StartsWith(inicioTablaDv) && mensaje.EndsWith(finTablaDv) && traducciones.ContainsKey(plantillaTablaDv))
            {
                string tabla = mensaje.Substring(inicioTablaDv.Length, mensaje.Length - inicioTablaDv.Length - finTablaDv.Length);
                return string.Format(traducciones[plantillaTablaDv], tabla);
            }

            const string inicioConsultaTabla = "Error al consultar la tabla ";
            const string separadorConsultaTabla = ": ";
            const string plantillaConsultaTabla = "Error al consultar la tabla {0}: {1}";
            if (mensaje.StartsWith(inicioConsultaTabla) && mensaje.Contains(separadorConsultaTabla) && traducciones.ContainsKey(plantillaConsultaTabla))
            {
                int indiceDetalle = mensaje.IndexOf(separadorConsultaTabla, inicioConsultaTabla.Length, StringComparison.Ordinal);
                string tabla = mensaje.Substring(inicioConsultaTabla.Length, indiceDetalle - inicioConsultaTabla.Length);
                string detalle = mensaje.Substring(indiceDetalle + separadorConsultaTabla.Length);
                return string.Format(traducciones[plantillaConsultaTabla], tabla, TraducirMensaje(detalle));
            }

            const string inicioGuardarBackup = "No se pudo guardar el backup en '";
            const string separadorDetalleBackup = "'. Detalle: ";
            const string separadorPrimerIntento = ". Primer intento: ";
            const string plantillaGuardarBackup = "No se pudo guardar el backup en '{0}'. Detalle: {1}. Primer intento: {2}";
            if (mensaje.StartsWith(inicioGuardarBackup) && mensaje.Contains(separadorDetalleBackup) && mensaje.Contains(separadorPrimerIntento) && traducciones.ContainsKey(plantillaGuardarBackup))
            {
                int indiceDetalle = mensaje.IndexOf(separadorDetalleBackup, StringComparison.Ordinal);
                int indicePrimerIntento = mensaje.IndexOf(separadorPrimerIntento, StringComparison.Ordinal);
                string ruta = mensaje.Substring(inicioGuardarBackup.Length, indiceDetalle - inicioGuardarBackup.Length);
                string detalle = mensaje.Substring(indiceDetalle + separadorDetalleBackup.Length, indicePrimerIntento - indiceDetalle - separadorDetalleBackup.Length);
                string primerIntento = mensaje.Substring(indicePrimerIntento + separadorPrimerIntento.Length);
                return string.Format(traducciones[plantillaGuardarBackup], ruta, TraducirMensaje(detalle), TraducirMensaje(primerIntento));
            }

            const string inicioScript = "No se encontro el script de instalacion '";
            const string separadorScript = "' en '";
            const string finScript = "'.";
            const string plantillaScript = "No se encontro el script de instalacion '{0}' en '{1}'.";
            if (mensaje.StartsWith(inicioScript) && mensaje.Contains(separadorScript) && mensaje.EndsWith(finScript) && traducciones.ContainsKey(plantillaScript))
            {
                int indiceRuta = mensaje.IndexOf(separadorScript, inicioScript.Length, StringComparison.Ordinal);
                string script = mensaje.Substring(inicioScript.Length, indiceRuta - inicioScript.Length);
                string ruta = mensaje.Substring(indiceRuta + separadorScript.Length, mensaje.Length - indiceRuta - separadorScript.Length - finScript.Length);
                return string.Format(traducciones[plantillaScript], script, ruta);
            }

            string[] prefijos =
            {
                "Error: ",
                "Error al cargar roles: ",
                "Error al crear el usuario: ",
                "Error al exportar PDF: ",
                "Error al realizar el backup: ",
                "Error al restaurar la base de datos: ",
                "Error en MostrarPermisos: ",
                "Sesión iniciada, bienvenido "
            };

            foreach (string prefijo in prefijos)
            {
                if (!mensaje.StartsWith(prefijo))
                    continue;

                string plantilla = prefijo + "{0}";
                if (!traducciones.ContainsKey(plantilla))
                    return string.Empty;

                return string.Format(traducciones[plantilla], TraducirMensaje(mensaje.Substring(prefijo.Length)));
            }

            const string inicioEliminacionFamilia = "'";
            const string finEliminacionFamilia = "' fue eliminado correctamente.";
            if (mensaje.StartsWith(inicioEliminacionFamilia) && mensaje.EndsWith(finEliminacionFamilia))
            {
                string nombre = mensaje.Substring(1, mensaje.Length - finEliminacionFamilia.Length - 1);
                const string plantillaEliminacionFamilia = "'{0}' fue eliminado correctamente.";
                if (traducciones.ContainsKey(plantillaEliminacionFamilia))
                    return string.Format(traducciones[plantillaEliminacionFamilia], nombre);
            }

            const string inicioEliminacionRol = "Permiso/Familia: '";
            const string finEliminacionRol = "' eliminada correctamente.";
            if (mensaje.StartsWith(inicioEliminacionRol) && mensaje.EndsWith(finEliminacionRol))
            {
                string nombre = mensaje.Substring(inicioEliminacionRol.Length, mensaje.Length - inicioEliminacionRol.Length - finEliminacionRol.Length);
                const string plantillaEliminacionRol = "Permiso/Familia: '{0}' eliminada correctamente.";
                if (traducciones.ContainsKey(plantillaEliminacionRol))
                    return string.Format(traducciones[plantillaEliminacionRol], nombre);
            }

            return string.Empty;
        }

        private string NormalizarTexto(string texto)
        {
            if (!texto.Contains("Ã") && !texto.Contains("Â"))
                return texto;

            return Encoding.UTF8.GetString(Encoding.GetEncoding(1252).GetBytes(texto));
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
