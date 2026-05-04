using BE_56_PS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.NetworkInformation;

namespace DAL_625NS
{
    public class DAL_Usuario_56PS 
    {
        public void crearUsuario(Usuario_56PS usuario)
        {
            if (validarUsuarioDNI(usuario.Dni))
                throw new Exception("Usuario ya existe");

 

            string query = @"INSERT INTO Usuario_56PS 
                    (DNI, Nombre, Apellido, Email, Bloqueado, 
                     NombreUsuario, Contraseña, 
                     Idioma, NombreRol, Activo) 
                     VALUES 
                    (@DNI, @Nombre, @Apellido, @Email, @Bloqueado, 
                     @NombreUsuario, @Contraseña, 
                     @Idioma, @NombreRol, @Activo)";

            SqlParameter[] parameters = {
                new SqlParameter("@DNI", usuario.Dni),
                new SqlParameter("@Nombre", usuario.Nombre),
                new SqlParameter("@Apellido", usuario.Apellido),
                new SqlParameter("@Email", usuario.Email),
                new SqlParameter("@Bloqueado", usuario.Bloqueado),
                new SqlParameter("@NombreUsuario", usuario.NombreUsuario),
                new SqlParameter("@Contraseña", usuario.Contraseña),
                new SqlParameter("@Idioma", usuario.idioma),
                new SqlParameter("@NombreRol", usuario.Rol),
                new SqlParameter("@Activo", usuario.Activo)
            };

            DAL_56PS.ExecuteNonQuery(query, parameters);
        }

        public bool validarUsuarioDNI(string dni)
        {
            string query = "SELECT COUNT(*) FROM dbo.Usuario_56PS WHERE DNI = @Dni";
            SqlParameter[] parameters = { new SqlParameter("@Dni", dni) };

            int count = Convert.ToInt32(DAL_56PS.ExecuteScalar(query, parameters));
            return count > 0;
        }

        public bool validarUsuario(string nombreUsuario, string contraseña)
        {
            string query = @"SELECT COUNT(*) FROM dbo.Usuario_56PS
                             WHERE NombreUsuario = @NombreUsuario AND Contraseña = @Contraseña";

            SqlParameter[] parameters = {
                new SqlParameter("@NombreUsuario", nombreUsuario),
                new SqlParameter("@Contraseña", contraseña)
            };

            int count = Convert.ToInt32(DAL_56PS.ExecuteScalar(query, parameters));
            return count > 0;
        }

        public void cambiarContraseña(string dni, string nuevaContraseña)
        {
            string query = "UPDATE dbo.Usuario_56PS SET Contraseña = @Contraseña WHERE DNI = @DNI";

            SqlParameter[] parameters = {
                new SqlParameter("@Contraseña", nuevaContraseña),
                new SqlParameter("@DNI", dni)
            };

            DAL_56PS.ExecuteNonQuery(query, parameters);
        }

        public void desbloquearUsuario(string dni)
        {
            string query = "UPDATE dbo.Usuario_56PS SET Bloqueado = 0, IntentosFallidos = 0, Activo = 1 WHERE DNI = @DNI";
            DAL_56PS.ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@DNI", dni) });
        }

        public List<Usuario_56PS> obtenerUsuarios()
        {
            string query = "SELECT * FROM dbo.Usuario_56PS";

            DataSet ds = DAL_56PS.ExecuteDataSet(query, null);

            List<Usuario_56PS> lista = new List<Usuario_56PS>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var usuario = new Usuario_56PS(
                    apellido: row["Apellido"].ToString(),
                    contraseña: row["Contraseña"].ToString(),
                    dni: row["DNI"].ToString(),
                    email: row["Email"].ToString(),
                    nombre: row["Nombre"].ToString(),
                    nombreUsuario: row["NombreUsuario"].ToString(),
                    idioma: row["Idioma"].ToString(),
                    bloqueado: Convert.ToBoolean(row["Bloqueado"]),
                    activo: Convert.ToBoolean(row["Activo"]),
                    rol: Convert.ToString(row["NombreRol"])
                );

               


                lista.Add(usuario);
            }

            return lista;
        }



        public void modificarUsuario(Usuario_56PS usuario)
        {
        

            string query = @"UPDATE dbo.Usuario_56PS SET 
                     Nombre = @Nombre,
                     Apellido = @Apellido,
                     Email = @Email,
                     Bloqueado = @Bloqueado,
                     NombreUsuario = @NombreUsuario,
                     Contraseña = @Contraseña,
                     Idioma = @Idioma,
                     NombreRol = @CodRol, 
                     Activo = @Activo
                     WHERE DNI = @DNI";

            SqlParameter[] parameters = {
                new SqlParameter("@Nombre", usuario.Nombre),
                new SqlParameter("@Apellido", usuario.Apellido),
                new SqlParameter("@Email", (object)usuario.Email ?? DBNull.Value),
                new SqlParameter("@Bloqueado", usuario.Bloqueado),
                new SqlParameter("@NombreUsuario", usuario.NombreUsuario),
                new SqlParameter("@Contraseña", usuario.Contraseña),
                new SqlParameter("@Idioma", usuario.idioma),
                new SqlParameter("@CodRol", usuario.Rol),  
                new SqlParameter("@Activo", usuario.Activo),
                new SqlParameter("@DNI", usuario.Dni)
            }; 

            DAL_56PS.ExecuteNonQuery(query, parameters);
        }

        public string verificarEstado(string nombreUsuario)
        {
            string query = "SELECT Bloqueado FROM dbo.Usuario_56PS WHERE NombreUsuario = @NombreUsuario";

            object result = DAL_56PS.ExecuteScalar(query, new SqlParameter[] { new SqlParameter("@NombreUsuario", nombreUsuario) });

            if (result == null) return "Inexistente";
            return Convert.ToBoolean(result) ? "Bloqueado" : "Activo";
        }

        public bool existeUsuarioConEseUsername(string userName)
        {
            string query = "SELECT COUNT(*) FROM dbo.Usuario_56PS WHERE NombreUsuario = @UserName";

            SqlParameter[] parameters = {
        new SqlParameter("@UserName", userName)
    };

            int count = Convert.ToInt32(DAL_56PS.ExecuteScalar(query, parameters));
            return count > 0;
        }

        public void bloquearUsuario(string dni)
        {
            string query = "UPDATE dbo.Usuario_56PS SET Bloqueado = 1, Activo = 0 WHERE DNI = @DNI";
            DAL_56PS.ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@DNI", dni) });
        }

        public Usuario_56PS obtenerUsuarioPorDni(string dni)
        {
            string query = "SELECT * FROM dbo.Usuario_56PS WHERE DNI = @DNI";

            SqlParameter[] parameters = {
        new SqlParameter("@DNI", dni)
    };

            DataSet ds = DAL_56PS.ExecuteDataSet(query, parameters);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow row = ds.Tables[0].Rows[0];

            return new Usuario_56PS(
                apellido: row["Apellido"].ToString(),
                contraseña: row["Contraseña"].ToString(),
                dni: row["DNI"].ToString(),
                email: row["Email"].ToString(),
                nombre: row["Nombre"].ToString(),
                nombreUsuario: row["NombreUsuario"].ToString(),
                idioma: row["Idioma"].ToString(),
                bloqueado: Convert.ToBoolean(row["Bloqueado"]),
                activo: Convert.ToBoolean(row["Activo"]),
                rol: Convert.ToString(row["NombreRol"])
            );
        }
    }
}
