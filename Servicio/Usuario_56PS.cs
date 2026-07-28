using System;
using Servicio;

namespace BE_56_PS
{
    [Serializable]
    public class Usuario_56PS
    {
        public string Apellido { get; set; }
        public string Contraseña { get; set; }
        public string Dni { get; set; }
        public string Email { get; set; }
        public Rol_56PS Rol { get; set; }
        public string Nombre { get; set; }
        public string NombreUsuario { get; set; }
        public Idioma_56PS Idioma { get; set; }
        public bool Bloqueado { get; set; }
        public bool Activo { get; set; }

        public Usuario_56PS()
        {
        }

        public Usuario_56PS(string apellido, string contraseña, string dni, string email, string nombre, string nombreUsuario, Idioma_56PS idioma, bool bloqueado, bool activo, Rol_56PS rol)
        {
            Apellido = apellido;
            Contraseña = contraseña;
            Dni = dni;
            Email = email;
            Nombre = nombre;
            NombreUsuario = nombreUsuario;
            Idioma = idioma;
            Bloqueado = bloqueado;
            Activo = activo;
            Rol = rol;
        }
    }
}
