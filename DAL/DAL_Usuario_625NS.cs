using BE_625NS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL_625NS
{
    public class DAL_Usuario_625NS //hola
    {
        public void crearUsuario(BE_Usuario_625NS usuario)
        {
            if (validarUsuarioDNI(usuario.Dni))
                throw new Exception("Usuario ya existe");

            string queryRol = "SELECT CodRol_625NS FROM Rol_625NS WHERE Nombre_625NS = @NombreRol";
            object result = DAL_625NS.ExecuteScalar(queryRol, new SqlParameter[] { new SqlParameter("@NombreRol", usuario.Rol_625NS) });

            if (result == null || result == DBNull.Value)
                throw new Exception("El rol especificado no existe.");

            int codRol = Convert.ToInt32(result);

            string query = @"INSERT INTO Usuario_625NS 
                    (DNI_625NS, Nombre_625NS, Apellido_625NS, Email_625NS, 
                     Bloqueado_625NS, NombreUsuario_625NS, Contraseña_625NS, 
                     IntentosFallidos_625NS, Idioma_625NS, CodRol_625NS) 
                     VALUES 
                    (@DNI, @Nombre, @Apellido, @Email, 
                     @Bloqueado, @NombreUsuario, @Contraseña, 
                     @IntentosFallidos, @Idioma, @CodRol)";

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
        new SqlParameter("@CodRol", codRol)
    };

            DAL_625NS.ExecuteNonQuery(query, parameters);
        }

        public bool validarUsuarioDNI(string dni)
        {
            string query = @"SELECT COUNT(*) FROM Usuario_625NS 
                             WHERE DNI_625NS = @Dni";

            SqlParameter[] parameters = { new SqlParameter("@Dni", dni) };

            int count = Convert.ToInt32(DAL_625NS.ExecuteScalar(query, parameters));
            return count > 0;
        }

        public bool validarUsuario(string nombreUsuario, string contraseña)
        {
            string query = @"SELECT COUNT(*) FROM Usuario_625NS 
                             WHERE NombreUsuario_625NS = @NombreUsuario 
                               AND Contraseña_625NS = @Contraseña 
                               ";

            SqlParameter[] parameters = {
                new SqlParameter("@NombreUsuario", nombreUsuario),
                new SqlParameter("@Contraseña", contraseña)
            };

            int count = Convert.ToInt32(DAL_625NS.ExecuteScalar(query, parameters));
            return count > 0;
        }

        public void cambiarContraseña(string dni, string nuevaContraseña)
        {
            string query = "UPDATE Usuario_625NS SET Contraseña_625NS = @Contraseña WHERE DNI_625NS = @DNI";

            SqlParameter[] parameters = {
                new SqlParameter("@Contraseña", nuevaContraseña),
                new SqlParameter("@DNI", dni)
            };

            DAL_625NS.ExecuteNonQuery(query, parameters);
        }

        public void desbloquearUsuario(string dni)
        {
            string query = "UPDATE Usuario_625NS SET Bloqueado_625NS = 0 WHERE DNI_625NS = @DNI";

            SqlParameter[] parameters = { new SqlParameter("@DNI", dni) };

            DAL_625NS.ExecuteNonQuery(query, parameters);
        }

        public List<BE_Usuario_625NS> obtenerUsuarios()
        {
            string query = @"SELECT u.*, r.CodRol_625NS, r.Nombre_625NS AS NombreRol
                     FROM Usuario_625NS u
                     LEFT JOIN Rol_625NS r ON u.CodRol_625NS = r.CodRol_625NS";

            DataSet ds = DAL_625NS.ExecuteDataSet(query, null);

            List<BE_Usuario_625NS> lista = new List<BE_Usuario_625NS>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var usuario = new BE_Usuario_625NS(
                    apellido: row["Apellido_625NS"].ToString(),
                    bloqueado: Convert.ToBoolean(row["Bloqueado_625NS"]),
                    contraseña: row["Contraseña_625NS"].ToString(),
                    dni: row["DNI_625NS"].ToString(),
                    email: row["Email_625NS"].ToString(),
                    nombre: row["Nombre_625NS"].ToString(),
                    nombreUsuario: row["NombreUsuario_625NS"].ToString(),
                    cantIntentosFallidos: Convert.ToInt32(row["IntentosFallidos_625NS"]),
                    idioma: row["Idioma_625NS"].ToString()
                );

                if (row["NombreRol"] != DBNull.Value)
                {
                    usuario.Rol_625NS = row["NombreRol"].ToString();
                }


                lista.Add(usuario);
            }

            return lista;
        }



        public void modificarUsuario(BE_Usuario_625NS usuario)
        {
            string queryRol = "SELECT CodRol_625NS FROM Rol_625NS WHERE Nombre_625NS = @NombreRol";
            object result = DAL_625NS.ExecuteScalar(queryRol, new SqlParameter[] { new SqlParameter("@NombreRol", usuario.Rol_625NS) });

            if (result == null || result == DBNull.Value)
                throw new Exception("El rol especificado no existe.");

            int codRol = Convert.ToInt32(result);


            string query = @"UPDATE Usuario_625NS SET 
                             Nombre_625NS = @Nombre,
                             Apellido_625NS = @Apellido,
                             Email_625NS = @Email,
                             NombreUsuario_625NS = @NombreUsuario,
                             Bloqueado_625NS = @Bloqueado,
                             IntentosFallidos_625NS = @IntentosFallidos,
                             Idioma_625NS = @Idioma,
                             CodRol_625NS = @CodRol
                             WHERE DNI_625NS = @DNI";

            SqlParameter[] parameters = {
                new SqlParameter("@Nombre", usuario.Nombre),
                new SqlParameter("@Apellido", usuario.Apellido),
                new SqlParameter("@Email", usuario.Email),
                new SqlParameter("@NombreUsuario", usuario.NombreUsuario),
                new SqlParameter("@Bloqueado", usuario.Bloqueado),
                new SqlParameter("@IntentosFallidos", usuario.cantIntentosFallidos),
                new SqlParameter("@Idioma", usuario.idioma),
                new SqlParameter("@DNI", usuario.Dni),
                new SqlParameter("@CodRol", codRol)

            };

            DAL_625NS.ExecuteNonQuery(query, parameters);
        }

        public string verificarEstado(string nombreUsuario)
        {
            string query = "SELECT Bloqueado_625NS FROM Usuario_625NS WHERE NombreUsuario_625NS = @NombreUsuario";

            SqlParameter[] parameters = { new SqlParameter("@NombreUsuario", nombreUsuario) };

            object result = DAL_625NS.ExecuteScalar(query, parameters);
            return Convert.ToBoolean(result) ? "Bloqueado" : "Activo";
        }

        public void sumarIntentoFallido(string nombreUsuario)
        {
            string query = @"UPDATE Usuario_625NS 
                             SET IntentosFallidos_625NS = ISNULL(IntentosFallidos_625NS, 0) + 1 
                             WHERE NombreUsuario_625NS = @NombreUsuario";

            SqlParameter[] parameters = { new SqlParameter("@NombreUsuario", nombreUsuario) };

            DAL_625NS.ExecuteNonQuery(query, parameters);
        }

        public void validarCantIntentos(string nombreUsuario)
        {
            string query = @"SELECT IntentosFallidos_625NS 
                             FROM Usuario_625NS 
                             WHERE NombreUsuario_625NS = @NombreUsuario";

            SqlParameter[] parameters = { new SqlParameter("@NombreUsuario", nombreUsuario) };

            object result = DAL_625NS.ExecuteScalar(query, parameters);

            if (result != null && result != DBNull.Value)
            {
                int intentosFallidos = Convert.ToInt32(result);

                if (intentosFallidos >= 3)
                {
                    string bloquearQuery = @"UPDATE Usuario_625NS 
                                             SET Bloqueado_625NS = 1, IntentosFallidos_625NS = 0 
                                             WHERE NombreUsuario_625NS = @NombreUsuario";

                    DAL_625NS.ExecuteNonQuery(bloquearQuery, parameters);
                }
            }
        }

        public void cambiarIdioma(BE_Usuario_625NS user, string v)
        {
            string query = @"UPDATE Usuario_625NS 
                             SET Idioma_625NS = @Idioma
                            WHERE DNI_625NS = @dni";

            SqlParameter[] parameters = { new SqlParameter("@dni", user.Dni), new SqlParameter("@Idioma", v) };

            DAL_625NS.ExecuteNonQuery(query, parameters);
        }
    }
}
