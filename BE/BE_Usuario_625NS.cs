using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BE_625NS
{
    [Serializable]
    public class BE_Usuario_625NS
    {
        public string Apellido { get; set; }

        public bool Bloqueado { get; set; }

        public string Contraseña { get; set; }

        public string Dni { get; set; }

        public string Email { get; set; }
        [XmlIgnore]
        public string Rol_625NS { get; set; } //Perfil 


        public string Nombre { get; set; }

        public string NombreUsuario { get; set; }

        public int cantIntentosFallidos { get; set; }

        public string idioma { get; set; }

        public BE_Usuario_625NS() { }

        public BE_Usuario_625NS(string apellido, bool bloqueado, string contraseña, string dni, string email, string nombre, string nombreUsuario, int cantIntentosFallidos, string idioma)
        {
            this.Apellido = apellido;
            this.Bloqueado = bloqueado;
            this.Contraseña = contraseña;
            this.Dni = dni;
            this.Email = email;
            this.Nombre = nombre;
            this.NombreUsuario = nombreUsuario;
            this.cantIntentosFallidos = cantIntentosFallidos;
            this.idioma = idioma;
        }
    }
}
