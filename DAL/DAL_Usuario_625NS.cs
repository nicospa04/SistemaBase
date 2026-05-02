using BE_56_PS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.NetworkInformation;

namespace DAL_625NS
{
    public class DAL_Usuario_625NS 
    {
        public void crearUsuario(BE_Usuario_56_PS usuario)
        {
            if (validarUsuarioDNI(usuario.Dni))
                throw new Exception("Usuario ya existe");

            string queryRol = "SELECT CodRol_625NS FROM Rol_625NS WHERE Nombre_625NS = @NombreRol";
            object result = DAL_625NS.ExecuteScalar(queryRol, new SqlParameter[] { new SqlParameter("@NombreRol", usuario.Rol_56_PS) });

            if (result == null || result == DBNull.Value)
                throw new Exception("El rol especificado no existe.");

            int codRol = Convert.ToInt32(result);

            string query = @"INSERT INTO Usuario_625NS 
                    (DNI, Nombre, Apellido, Email, Bloqueado, 
                     NombreUsuario, Contraseña, IntentosFallidos, 
                     Idioma, NombreRol, Activo) 
                     VALUES 
                    (@DNI, @Nombre, @Apellido, @Email, @Bloqueado, 
                     @NombreUsuario, @Contraseña, @IntentosFallidos, 
                     @Idioma, @NombreRol, @Activo)";

            SqlParameter[] parameters = {
                new SqlParameter("@DNI", usuario.Dni),
                new SqlParameter("@Nombre", usuario.Nombre),
                new SqlParameter("@Apellido", usuario.Apellido),
                new SqlParameter("@Email", usuario.Email),
                new SqlParameter("@Bloqueado", usuario.Bloqueado),
                new SqlParameter("@NombreUsuario", usuario.NombreUsuario),
                new SqlParameter("@Contraseña", usuario.Contraseña),
                new SqlParameter("@IntentosFallidos", usuario.cantIntentosFallidos),
                new SqlParameter("@Idioma", usuario.idioma),
                new SqlParameter("@NombreRol", usuario.Rol_56_PS),
                new SqlParameter("@Activo", usuario.Activo)
            };

            DAL_625NS.ExecuteNonQuery(query, parameters);
        }

        public bool validarUsuarioDNI(string dni)
        {
            string query = "SELECT COUNT(*) FROM dbo.Usuario WHERE DNI = @Dni";
            SqlParameter[] parameters = { new SqlParameter("@Dni", dni) };

            int count = Convert.ToInt32(DAL_625NS.ExecuteScalar(query, parameters));
            return count > 0;
        }

        public bool validarUsuario(string nombreUsuario, string contraseña)
        {
            string query = @"SELECT COUNT(*) FROM dbo.Usuario 
                             WHERE NombreUsuario = @NombreUsuario AND Contraseña = @Contraseña";

            SqlParameter[] parameters = {
                new SqlParameter("@NombreUsuario", nombreUsuario),
                new SqlParameter("@Contraseña", contraseña)
            };

            int count = Convert.ToInt32(DAL_625NS.ExecuteScalar(query, parameters));
            return count > 0;
        }

        public void cambiarContraseña(string dni, string nuevaContraseña)
        {
            string query = "UPDATE dbo.Usuario SET Contraseña = @Contraseña WHERE DNI = @DNI";

            SqlParameter[] parameters = {
                new SqlParameter("@Contraseña", nuevaContraseña),
                new SqlParameter("@DNI", dni)
            };

            DAL_625NS.ExecuteNonQuery(query, parameters);
        }

        public void desbloquearUsuario(string dni)
        {
            string query = "UPDATE dbo.Usuario SET Bloqueado = 0, IntentosFallidos = 0, Activo = 1 WHERE DNI = @DNI";
            DAL_625NS.ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@DNI", dni) });
        }

        public List<BE_Usuario_56_PS> obtenerUsuarios()
        {
            string query = "SELECT * FROM dbo.Usuario";

            DataSet ds = DAL_625NS.ExecuteDataSet(query, null);

            List<BE_Usuario_56_PS> lista = new List<BE_Usuario_56_PS>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var usuario = new BE_Usuario_56_PS(
                    apellido: row["Apellido"].ToString(),
                    contraseña: row["Contraseña"].ToString(),
                    dni: row["DNI"].ToString(),
                    email: row["Email"].ToString(),
                    nombre: row["Nombre"].ToString(),
                    nombreUsuario: row["NombreUsuario"].ToString(),
                    cantIntentosFallidos: Convert.ToInt32(row["IntentosFallidos"]),
                    idioma: row["Idioma"].ToString(),
                    bloqueado: Convert.ToBoolean(row["Bloqueado"]),
                    activo: Convert.ToBoolean(row["Activo"])
                );

                if (row["NombreRol"] != DBNull.Value)
                {
                    usuario.Rol_56_PS = row["NombreRol"].ToString();
                }


                lista.Add(usuario);
            }

            return lista;
        }



        public void modificarUsuario(BE_Usuario_56_PS usuario)
        {
            string queryRol = "SELECT CodRol FROM Rol WHERE NombreRol = @NombreRol";
            object result = DAL_625NS.ExecuteScalar(queryRol, new SqlParameter[] { new SqlParameter("@NombreRol", usuario.Rol_56_PS) });

            if (result == null || result == DBNull.Value)
                throw new Exception("El rol especificado no existe.");

            int codRol = Convert.ToInt32(result);


            string query = @"UPDATE dbo.Usuario SET 
                     Nombre = @Nombre,
                     Apellido = @Apellido,
                     Email = @Email,
                     Bloqueado = @Bloqueado,
                     NombreUsuario = @NombreUsuario,
                     Contraseña = @Contraseña,
                     IntentosFallidos = @IntentosFallidos,
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
                new SqlParameter("@IntentosFallidos", usuario.cantIntentosFallidos),
                new SqlParameter("@Idioma", usuario.idioma),
                new SqlParameter("@CodRol", codRol), // Pasamos el ID obtenido en el paso 1
                new SqlParameter("@Activo", usuario.Activo),
                new SqlParameter("@DNI", usuario.Dni)
            }; 

            DAL_625NS.ExecuteNonQuery(query, parameters);
        }

        public string verificarEstado(string nombreUsuario)
        {
            string query = "SELECT Bloqueado FROM dbo.Usuario WHERE NombreUsuario = @NombreUsuario";

            object result = DAL_625NS.ExecuteScalar(query, new SqlParameter[] { new SqlParameter("@NombreUsuario", nombreUsuario) });

            if (result == null) return "Inexistente";
            return Convert.ToBoolean(result) ? "Bloqueado" : "Activo";
        }

        public void sumarIntentoFallido(string nombreUsuario)
        {
            string query = "UPDATE dbo.Usuario SET IntentosFallidos = ISNULL(IntentosFallidos, 0) + 1 WHERE NombreUsuario = @NombreUsuario";
            DAL_625NS.ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@NombreUsuario", nombreUsuario) });
        }

        public void validarCantIntentos(string nombreUsuario)
        {
            string query = "SELECT IntentosFallidos FROM dbo.Usuario WHERE NombreUsuario = @NombreUsuario";

            object result = DAL_625NS.ExecuteScalar(query, new SqlParameter[] { new SqlParameter("@NombreUsuario", nombreUsuario) });

            if (result != null && Convert.ToInt32(result) >= 3)
            {
                string bloquearQuery = "UPDATE dbo.Usuario SET Bloqueado = 1, Activo = 0, IntentosFallidos = 0 WHERE NombreUsuario = @NombreUsuario";
                DAL_625NS.ExecuteNonQuery(bloquearQuery, new SqlParameter[] { new SqlParameter("@NombreUsuario", nombreUsuario) });
            }
        }

        public void CambiarEstadoActivo(string dni, bool nuevoEstado)
        {
            string query = "UPDATE dbo.Usuario SET Activo = @estado, Bloqueado = @bloqueado WHERE DNI = @dni";

            SqlParameter[] parametros = {
                new SqlParameter("@estado", nuevoEstado),
                new SqlParameter("@bloqueado", !nuevoEstado),
                new SqlParameter("@dni", dni)
            };

            DAL_625NS.ExecuteNonQuery(query, parametros);
        }

        public void cambiarIdioma(BE_Usuario_56_PS user, string nuevoIdioma)
        {
            string query = "UPDATE dbo.Usuario SET Idioma = @Idioma WHERE DNI = @dni";

            SqlParameter[] parameters = {
                new SqlParameter("@dni", user.Dni),
                new SqlParameter("@Idioma", nuevoIdioma)
            };

            DAL_625NS.ExecuteNonQuery(query, parameters);
        }
    }
}
