using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;
using ClassLibrary2;


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

                if (ctrl.HasChildren)
                    TraducirControles(ctrl.Controls, traducciones);
            }
        }

        private string ObtenerTipoDesdeSession()
        {
            string idiomaActual = SessionManager_56PS.getInstancia().idiomaActual;
            return string.IsNullOrEmpty(idiomaActual) ? "ES" : idiomaActual;
        }

    }
}
