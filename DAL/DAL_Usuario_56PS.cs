using BE_56_PS;
using Servicio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL_625NS
{
    public class DAL_Usuario_56PS
    {
        public void crearUsuario(Usuario_56PS usuario)
        {
            if (validarUsuarioDNI(usuario.Dni))
                throw new Exception("Usuario ya existe");

            string query = @"INSERT INTO Usuario_56PS
                (DNI, Nombre, Apellido, Email, Bloqueado, NombreUsuario, Contraseña, Idioma, Rol, Activo)
                VALUES
                (@DNI, @Nombre, @Apellido, @Email, @Bloqueado, @NombreUsuario, @Contraseña, @Idioma, @Rol, @Activo)";

            SqlParameter[] parametros =
            {
                new SqlParameter("@DNI", usuario.Dni),
                new SqlParameter("@Nombre", usuario.Nombre),
                new SqlParameter("@Apellido", usuario.Apellido),
                new SqlParameter("@Email", (object)usuario.Email ?? DBNull.Value),
                new SqlParameter("@Bloqueado", usuario.Bloqueado),
                new SqlParameter("@NombreUsuario", usuario.NombreUsuario),
                new SqlParameter("@Contraseña", usuario.Contraseña),
                new SqlParameter("@Idioma", usuario.Idioma?.tipo ?? "ES"),
                new SqlParameter("@Rol", (object)usuario.Rol?.Codigo ?? DBNull.Value),
                new SqlParameter("@Activo", usuario.Activo)
            };

            DAL_56PS.ExecuteNonQuery(query, parametros);
        }

        public bool validarUsuarioDNI(string dni)
        {
            string query = "SELECT COUNT(*) FROM dbo.Usuario_56PS WHERE DNI = @Dni";
            int cantidad = Convert.ToInt32(DAL_56PS.ExecuteScalar(
                query,
                new[] { new SqlParameter("@Dni", dni) }));
            return cantidad > 0;
        }

        public bool validarUsuario(string nombreUsuario, string contraseña)
        {
            string query = @"SELECT COUNT(*) FROM dbo.Usuario_56PS
                             WHERE NombreUsuario = @NombreUsuario AND Contraseña = @Contraseña";
            SqlParameter[] parametros =
            {
                new SqlParameter("@NombreUsuario", nombreUsuario),
                new SqlParameter("@Contraseña", contraseña)
            };
            return Convert.ToInt32(DAL_56PS.ExecuteScalar(query, parametros)) > 0;
        }

        public void cambiarContraseña(string dni, string nuevaContraseña)
        {
            string query = @"UPDATE dbo.Usuario_56PS
                             SET Contraseña = @Contraseña
                             WHERE DNI = @DNI";
            DAL_56PS.ExecuteNonQuery(query, new[]
            {
                new SqlParameter("@Contraseña", nuevaContraseña),
                new SqlParameter("@DNI", dni)
            });
        }

        public void desbloquearUsuario(string dni)
        {
            string query = @"UPDATE dbo.Usuario_56PS
                             SET Bloqueado = 0
                             WHERE DNI = @DNI";
            DAL_56PS.ExecuteNonQuery(query, new[] { new SqlParameter("@DNI", dni) });
        }

        public List<Usuario_56PS> obtenerUsuarios()
        {
            string query = @"SELECT U.*, R.Nombre AS NombreRol, I.Nombre AS NombreIdioma
                             FROM Usuario_56PS U
                             LEFT JOIN Roles R ON U.Rol = R.CodigoRol
                             LEFT JOIN Idioma_56PS I ON U.Idioma = I.Tipo";
            DataSet datos = DAL_56PS.ExecuteDataSet(query, null);
            List<Usuario_56PS> usuarios = new List<Usuario_56PS>();

            foreach (DataRow fila in datos.Tables[0].Rows)
                usuarios.Add(MapearUsuario(fila));

            return usuarios;
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
                             Rol = @Rol,
                             Activo = @Activo
                             WHERE DNI = @DNI";

            SqlParameter[] parametros =
            {
                new SqlParameter("@Nombre", usuario.Nombre),
                new SqlParameter("@Apellido", usuario.Apellido),
                new SqlParameter("@Email", (object)usuario.Email ?? DBNull.Value),
                new SqlParameter("@Bloqueado", usuario.Bloqueado),
                new SqlParameter("@NombreUsuario", usuario.NombreUsuario),
                new SqlParameter("@Contraseña", usuario.Contraseña),
                new SqlParameter("@Idioma", usuario.Idioma?.tipo ?? "ES"),
                new SqlParameter("@Rol", (object)usuario.Rol?.Codigo ?? DBNull.Value),
                new SqlParameter("@Activo", usuario.Activo),
                new SqlParameter("@DNI", usuario.Dni)
            };
            DAL_56PS.ExecuteNonQuery(query, parametros);
        }

        public string verificarEstado(string nombreUsuario)
        {
            object resultado = DAL_56PS.ExecuteScalar(
                "SELECT Bloqueado FROM dbo.Usuario_56PS WHERE NombreUsuario = @NombreUsuario",
                new[] { new SqlParameter("@NombreUsuario", nombreUsuario) });
            if (resultado == null)
                return "Inexistente";
            return Convert.ToBoolean(resultado) ? "Bloqueado" : "Activo";
        }

        public bool existeUsuarioConEseUsername(string userName)
        {
            int cantidad = Convert.ToInt32(DAL_56PS.ExecuteScalar(
                "SELECT COUNT(*) FROM dbo.Usuario_56PS WHERE NombreUsuario = @UserName",
                new[] { new SqlParameter("@UserName", userName) }));
            return cantidad > 0;
        }

        public void bloquearUsuario(string dni)
        {
            DAL_56PS.ExecuteNonQuery(
                "UPDATE dbo.Usuario_56PS SET Bloqueado = 1 WHERE DNI = @DNI",
                new[] { new SqlParameter("@DNI", dni) });
        }

        public Usuario_56PS obtenerUsuarioPorDni(string dni)
        {
            string query = @"SELECT U.*, R.Nombre AS NombreRol, I.Nombre AS NombreIdioma
                             FROM Usuario_56PS U
                             LEFT JOIN Roles R ON U.Rol = R.CodigoRol
                             LEFT JOIN Idioma_56PS I ON U.Idioma = I.Tipo
                             WHERE U.DNI = @DNI";
            DataSet datos = DAL_56PS.ExecuteDataSet(
                query,
                new[] { new SqlParameter("@DNI", dni) });
            return datos.Tables[0].Rows.Count == 0 ? null : MapearUsuario(datos.Tables[0].Rows[0]);
        }

        public void cambiarEstadoActivo(string dni)
        {
            string query = @"UPDATE dbo.Usuario_56PS
                             SET Activo = CASE WHEN Activo = 1 THEN 0 ELSE 1 END
                             WHERE DNI = @DNI";
            DAL_56PS.ExecuteNonQuery(query, new[] { new SqlParameter("@DNI", dni) });
        }

        public void cambiarIdioma(string idioma, string dni)
        {
            string query = @"UPDATE dbo.Usuario_56PS
                             SET Idioma = @Idioma
                             WHERE DNI = @DNI";
            DAL_56PS.ExecuteNonQuery(query, new[]
            {
                new SqlParameter("@Idioma", idioma),
                new SqlParameter("@DNI", dni)
            });
        }

        private Usuario_56PS MapearUsuario(DataRow fila)
        {
            string codigoRol = fila["Rol"] == DBNull.Value ? null : fila["Rol"].ToString();
            Rol_56PS rol = string.IsNullOrEmpty(codigoRol)
                ? null
                : new DAL_Rol_56PS().ObtenerRol(codigoRol);

            Idioma_56PS idioma = new Idioma_56PS
            {
                tipo = fila["Idioma"].ToString(),
                nombre = fila["NombreIdioma"] == DBNull.Value
                    ? new Idioma_56PS(fila["Idioma"].ToString()).nombre
                    : fila["NombreIdioma"].ToString()
            };

            return new Usuario_56PS(
                apellido: fila["Apellido"].ToString(),
                contraseña: fila["Contraseña"].ToString(),
                dni: fila["DNI"].ToString(),
                email: fila["Email"] == DBNull.Value ? null : fila["Email"].ToString(),
                nombre: fila["Nombre"].ToString(),
                nombreUsuario: fila["NombreUsuario"].ToString(),
                idioma: idioma,
                bloqueado: Convert.ToBoolean(fila["Bloqueado"]),
                activo: Convert.ToBoolean(fila["Activo"]),
                rol: rol);
        }
    }
}
