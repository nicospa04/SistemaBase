using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BE_56_PS
{
    [Serializable]
    public class BE_Usuario_56_PS
    {
        public string Apellido { get; set; }
        public string Contraseña { get; set; }
        public string Dni { get; set; }
        public string Email { get; set; }

        [XmlIgnore]
        public string Rol_56_PS { get; set; } //Perfil 


        public string Nombre { get; set; }
        public string NombreUsuario { get; set; }
        public int cantIntentosFallidos { get; set; }
        public string idioma { get; set; }
        public bool Bloqueado { get; set; }
        public bool Activo { get; set; }

        public BE_Usuario_56_PS() { }

        public BE_Usuario_56_PS(string apellido, string contraseña, string dni, string email, string nombre, string nombreUsuario, int cantIntentosFallidos, string idioma, bool bloqueado ,bool activo)
        {
            this.Apellido = apellido;
            this.Contraseña = contraseña;
            this.Dni = dni;
            this.Email = email;
            this.Nombre = nombre;
            this.NombreUsuario = nombreUsuario;
            this.cantIntentosFallidos = cantIntentosFallidos;
            this.idioma = idioma;
            this.Bloqueado = bloqueado;
            this.Activo = activo;
        }
    }
}
