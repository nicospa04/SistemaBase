using Servicio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_625NS
{
    public class DAL_Rol_56PS
    {
        public void Modificar(Familia_56PS f)
        {
            string query = "UPDATE Familias SET Nombre = @nom WHERE CodigoFamilia = @cod";

            SqlParameter[] parametros = {
                new SqlParameter("@cod", f.Codigo),
                new SqlParameter("@nom", f.Nombre)
            };

            DAL_56PS.ExecuteNonQuery(query, parametros);
        }

        public string ObtenerNombredeRol(string codigo)
        {
            string query = "SELECT Nombre FROM Roles WHERE CodigoRol = @c";
            SqlParameter[] parametros = { new SqlParameter("@c", codigo) };

            DataSet ds = DAL_56PS.ExecuteDataSet(query, parametros);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            return ds.Tables[0].Rows[0]["Nombre"].ToString();
        }

        public void AsignarPermisosARol(Rol_56PS rol, Rol_56PS permiso)
        {
            int rowsaf = 0;

            if (permiso.esfamilia)
            {
                string query = "INSERT INTO RolFamilia(CodigoRol, CodigoFamilia) VALUES (@codrol,@codfamilia)";

                SqlParameter[] parametros = {
                    new SqlParameter("@codrol", rol.Codigo),
                    new SqlParameter("@codfamilia", permiso.Codigo)
                };

                rowsaf = DAL_56PS.ExecuteNonQuery(query, parametros);
            }
            else
            {
                string query = "INSERT INTO RolPatente(CodigoRol, CodigoPatente) VALUES (@codrol,@codpatente)";

                SqlParameter[] parametros = {
                    new SqlParameter("@codrol", rol.Codigo),
                    new SqlParameter("@codpatente", permiso.Codigo)
                };

                rowsaf = DAL_56PS.ExecuteNonQuery(query, parametros);
            }

            if (rowsaf <= 0)
            {
                throw new Exception("ex1");
            }
        }

        public void AsignarPermisoAFamilia(Rol_56PS permiso, Familia_56PS familia)
        {
            int filasAfectadas;

            if (permiso.esfamilia)
            {
                string query = "INSERT INTO FamiliaFamilia (CodigoFamilia, CodFamiliaHija) VALUES (@codfam, @codhija)";

                SqlParameter[] parametros = {
                    new SqlParameter("@codfam", familia.Codigo),
                    new SqlParameter("@codhija", permiso.Codigo)
                };

                filasAfectadas = DAL_56PS.ExecuteNonQuery(query, parametros);
            }
            else
            {
                string query = "INSERT INTO FamiliaPatente (CodigoFamilia, CodigoPatente) VALUES (@codfam, @codpat)";

                SqlParameter[] parametros = {
                    new SqlParameter("@codfam", familia.Codigo),
                    new SqlParameter("@codpat", permiso.Codigo)
                };

                filasAfectadas = DAL_56PS.ExecuteNonQuery(query, parametros);
            }

            if (filasAfectadas != 1)
                throw new Exception("No se pudo asignar el elemento a la familia.");
        }

        public List<Patente_56PS> ObtenerPatentes()
        {
            string query = "SELECT * FROM Patentes";

            DataSet ds = DAL_56PS.ExecuteDataSet(query, null);

            List<Patente_56PS> patentes = new List<Patente_56PS>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Patente_56PS p = new Patente_56PS();
                p.Nombre = row["Nombre"].ToString();
                p.Codigo = row["CodigoPatente"].ToString();
                patentes.Add(p);
            }

            return patentes;
        }

        public bool VerificarAsignacion(Rol_56PS p)
        {
            string query = "SELECT COUNT(*) FROM Usuario_56PS WHERE Rol = @cod";
            SqlParameter[] parametros = { new SqlParameter("@cod", p.Codigo) };
            int resultado = Convert.ToInt32(DAL_56PS.ExecuteScalar(query, parametros));
            return resultado > 0;
        }

        public bool VerificarAsignacionFamilia(Familia_56PS f)
        {
            string query = @"SELECT
                (SELECT COUNT(*) FROM RolFamilia WHERE CodigoFamilia = @cod) +
                (SELECT COUNT(*) FROM FamiliaPatente WHERE CodigoFamilia = @cod) +
                (SELECT COUNT(*) FROM FamiliaFamilia WHERE CodigoFamilia = @cod OR CodFamiliaHija = @cod)";
            SqlParameter[] parametros = { new SqlParameter("@cod", f.Codigo) };

            int resultado = Convert.ToInt32(DAL_56PS.ExecuteScalar(query, parametros));
            return resultado > 0;
        }

        public bool FamiliaEstaActiva(string codigo)
        {
            string query = "SELECT COUNT(*) FROM Familias WHERE CodigoFamilia = @cod AND Activa = 1";
            SqlParameter[] parametros = { new SqlParameter("@cod", codigo) };
            return Convert.ToInt32(DAL_56PS.ExecuteScalar(query, parametros)) > 0;
        }

        public bool FamiliaContiene(string codigoFamilia, string codigoBuscado)
        {
            string query = @"WITH Jerarquia AS
            (
                SELECT CodFamiliaHija,
                    CAST('|' + CodigoFamilia + '|' + CodFamiliaHija + '|' AS VARCHAR(MAX)) AS Ruta
                FROM FamiliaFamilia
                WHERE CodigoFamilia = @origen

                UNION ALL

                SELECT ff.CodFamiliaHija,
                    CAST(j.Ruta + ff.CodFamiliaHija + '|' AS VARCHAR(MAX))
                FROM FamiliaFamilia ff
                INNER JOIN Jerarquia j ON ff.CodigoFamilia = j.CodFamiliaHija
                WHERE j.Ruta NOT LIKE '%|' + ff.CodFamiliaHija + '|%'
            )
            SELECT COUNT(*) FROM Jerarquia WHERE CodFamiliaHija = @destino OPTION (MAXRECURSION 32767);";

            SqlParameter[] parametros = {
                new SqlParameter("@origen", codigoFamilia),
                new SqlParameter("@destino", codigoBuscado)
            };

            return Convert.ToInt32(DAL_56PS.ExecuteScalar(query, parametros)) > 0;
        }

        public List<Rol_56PS> ObtenerTodosLosRoles()
        {
            string query = "SELECT * FROM Roles WHERE Activo = 1";

            DataSet ds = DAL_56PS.ExecuteDataSet(query, null);

            List<Rol_56PS> roles = new List<Rol_56PS>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Rol_56PS p = new Rol_56PS();
                p.Nombre = row["Nombre"].ToString();
                p.Codigo = row["CodigoRol"].ToString();
                p.activo = Convert.ToBoolean(row["Activo"]);
                p.esfamilia = true;
                roles.Add(p);
            }

            return roles;
        }

        public void EliminarFamilia(Familia_56PS f)
        {
            string query = "UPDATE Familias SET Activa = @act WHERE CodigoFamilia = @cod";

            SqlParameter[] parametros = {
                new SqlParameter("@act", f.activo),
                new SqlParameter("@cod", f.Codigo)
            };

            int rowsaf = DAL_56PS.ExecuteNonQuery(query, parametros);

            if (rowsaf <= 0)
            {
                throw new Exception("ex2");
            }
        }

        public void EliminarRol(Rol_56PS rol)
        {
            string query = "UPDATE Roles SET Activo = @act WHERE CodigoRol = @cod";

            SqlParameter[] parametros = {
                new SqlParameter("@act", rol.activo),
                new SqlParameter("@cod", rol.Codigo)
            };

            int rowsaf = DAL_56PS.ExecuteNonQuery(query, parametros);

            if (rowsaf <= 0)
            {
                throw new Exception("ex3");
            }
        }

        public List<Familia_56PS> ObtenerFamilias()
        {
            string query = "SELECT * FROM Familias WHERE Activa=1";

            DataSet ds = DAL_56PS.ExecuteDataSet(query, null);

            List<Familia_56PS> familias = new List<Familia_56PS>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Familia_56PS f = new Familia_56PS();
                f.Nombre = row["Nombre"].ToString();
                f.Codigo = row["CodigoFamilia"].ToString();
                f.esfamilia = true;
                f.activo = true;
                familias.Add(f);
            }

            return familias;
        }

        public bool VerificarExistenciaRolCodigo(Rol_56PS p)
        {
            string query = "SELECT COUNT(*) FROM Roles WHERE CodigoRol = @cod";
            SqlParameter[] parametros = { new SqlParameter("@cod", p.Codigo) };

            int resultado = Convert.ToInt32(DAL_56PS.ExecuteScalar(query, parametros));
            return resultado > 0;
        }

        public bool VerificarExistenciaRolNombre(Rol_56PS p)
        {
            string query = "SELECT COUNT(*) FROM Roles WHERE Nombre = @nom";
            SqlParameter[] parametros = { new SqlParameter("@nom", p.Nombre) };

            int resultado = Convert.ToInt32(DAL_56PS.ExecuteScalar(query, parametros));
            return resultado > 0;
        }

        public bool VerificarExistenciaFamiliaCodigo(Familia_56PS f)
        {
            string query = "SELECT COUNT(*) FROM Familias WHERE CodigoFamilia = @cod";
            SqlParameter[] parametros = { new SqlParameter("@cod", f.Codigo) };

            int resultado = Convert.ToInt32(DAL_56PS.ExecuteScalar(query, parametros));
            return resultado > 0;
        }

        public bool VerificarExistenciaFamiliaNombre(Familia_56PS f)
        {
            string query = "SELECT COUNT(*) FROM Familias WHERE Nombre = @nom AND CodigoFamilia <> @cod";
            SqlParameter[] parametros = {
                new SqlParameter("@nom", f.Nombre),
                new SqlParameter("@cod", f.Codigo)
            };

            int resultado = Convert.ToInt32(DAL_56PS.ExecuteScalar(query, parametros));
            return resultado > 0;
        }

        public Rol_56PS ObtenerPermiso(string nombre)
        {
            string query = "SELECT Nombre, CodigoPatente FROM Patentes WHERE Nombre = @nom";
            SqlParameter[] parametros = { new SqlParameter("@nom", nombre) };

            DataSet ds = DAL_56PS.ExecuteDataSet(query, parametros);

            Rol_56PS permiso = new Rol_56PS();

            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                permiso.Nombre = row["Nombre"].ToString();
                permiso.Codigo = row["CodigoPatente"].ToString();
            }

            return permiso;
        }

        public void EliminarPermisodeRol(Rol_56PS p, string codrol)
        {
            int filasAfectadas;

            if (p.esfamilia)
            {
                string query = "DELETE FROM RolFamilia WHERE CodigoRol = @codperf AND CodigoFamilia = @codfam";

                SqlParameter[] parametros = {
                    new SqlParameter("@codfam", p.Codigo),
                    new SqlParameter("@codperf", codrol)
                };

                filasAfectadas = DAL_56PS.ExecuteNonQuery(query, parametros);
            }
            else
            {
                string query = "DELETE FROM RolPatente WHERE CodigoRol = @codperf AND CodigoPatente = @codpat";

                SqlParameter[] parametros = {
                    new SqlParameter("@codpat", p.Codigo),
                    new SqlParameter("@codperf", codrol)
                };

                filasAfectadas = DAL_56PS.ExecuteNonQuery(query, parametros);
            }

            if (filasAfectadas != 1)
                throw new Exception("La asignación seleccionada ya no existe en el rol.");
        }

        public void EliminarPermisodeFamilia(Rol_56PS p, string codrol)
        {
            int filasAfectadas;

            if (p.esfamilia)
            {
                string query = "DELETE FROM FamiliaFamilia WHERE CodigoFamilia = @codfam AND CodFamiliaHija = @codfamhija";

                SqlParameter[] parametros = {
                    new SqlParameter("@codfamhija", p.Codigo),
                    new SqlParameter("@codfam", codrol)
                };

                filasAfectadas = DAL_56PS.ExecuteNonQuery(query, parametros);
            }
            else
            {
                string query = "DELETE FROM FamiliaPatente WHERE CodigoFamilia = @codfam AND CodigoPatente = @codpat";

                SqlParameter[] parametros = {
                    new SqlParameter("@codpat", p.Codigo),
                    new SqlParameter("@codfam", codrol)
                };

                filasAfectadas = DAL_56PS.ExecuteNonQuery(query, parametros);
            }

            if (filasAfectadas != 1)
                throw new Exception("La asignación seleccionada ya no existe en la familia.");
        }

        public string ObtenerCodigoRol(Rol_56PS p)
        {
            string query = "SELECT CodigoRol FROM Roles WHERE Nombre = @nom";
            SqlParameter[] parametros = { new SqlParameter("@nom", p.Nombre) };

            DataSet ds = DAL_56PS.ExecuteDataSet(query, parametros);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            return ds.Tables[0].Rows[0]["CodigoRol"].ToString();
        }

        public bool CrearFamilia(Familia_56PS fam)
        {
            string query = "INSERT INTO Familias(Nombre,CodigoFamilia,Activa) VALUES(@nom,@cod,@act)";

            SqlParameter[] parametros = {
                new SqlParameter("@nom", fam.Nombre),
                new SqlParameter("@cod", fam.Codigo),
                new SqlParameter("@act", fam.activo)
            };

            int rowsaf = DAL_56PS.ExecuteNonQuery(query, parametros);

            if (rowsaf <= 0)
            {
                throw new Exception("ex4");
            }

            return true;
        }

        public void CrearRol(Rol_56PS rol)
        {
            string query = "INSERT INTO Roles(Nombre,CodigoRol,Activo) VALUES(@nom,@cod,@act)";

            SqlParameter[] parametros = {
                new SqlParameter("@nom", rol.Nombre),
                new SqlParameter("@cod", rol.Codigo),
                new SqlParameter("@act", rol.activo)
            };

            int rowsaff = DAL_56PS.ExecuteNonQuery(query, parametros);

            if (rowsaff <= 0)
            {
                throw new Exception("ex5");
            }
        }

        public void CrearFamiliaConElementos(Familia_56PS familia, IEnumerable<Rol_56PS> elementos)
        {
            using (SqlConnection conexion = new SqlConnection(DAL_56PS.obtenerConexion()))
            {
                conexion.Open();
                using (SqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand comando = new SqlCommand("INSERT INTO Familias(Nombre, CodigoFamilia, Activa) VALUES(@nom, @cod, @act)", conexion, transaccion))
                        {
                            comando.Parameters.AddWithValue("@nom", familia.Nombre);
                            comando.Parameters.AddWithValue("@cod", familia.Codigo);
                            comando.Parameters.AddWithValue("@act", familia.activo);
                            if (comando.ExecuteNonQuery() != 1)
                                throw new Exception("No se pudo crear la familia.");
                        }

                        foreach (Rol_56PS elemento in elementos)
                        {
                            string consulta = elemento.esfamilia
                                ? "INSERT INTO FamiliaFamilia(CodigoFamilia, CodFamiliaHija) VALUES(@padre, @hijo)"
                                : "INSERT INTO FamiliaPatente(CodigoFamilia, CodigoPatente) VALUES(@padre, @hijo)";

                            using (SqlCommand comando = new SqlCommand(consulta, conexion, transaccion))
                            {
                                comando.Parameters.AddWithValue("@padre", familia.Codigo);
                                comando.Parameters.AddWithValue("@hijo", elemento.Codigo);
                                if (comando.ExecuteNonQuery() != 1)
                                    throw new Exception("No se pudo asignar un elemento inicial a la familia.");
                            }
                        }

                        transaccion.Commit();
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        public void CrearRolConElementos(Rol_56PS rol, IEnumerable<Rol_56PS> elementos)
        {
            using (SqlConnection conexion = new SqlConnection(DAL_56PS.obtenerConexion()))
            {
                conexion.Open();
                using (SqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand comando = new SqlCommand("INSERT INTO Roles(Nombre, CodigoRol, Activo) VALUES(@nom, @cod, @act)", conexion, transaccion))
                        {
                            comando.Parameters.AddWithValue("@nom", rol.Nombre);
                            comando.Parameters.AddWithValue("@cod", rol.Codigo);
                            comando.Parameters.AddWithValue("@act", rol.activo);
                            if (comando.ExecuteNonQuery() != 1)
                                throw new Exception("No se pudo crear el rol.");
                        }

                        foreach (Rol_56PS elemento in elementos)
                        {
                            string consulta = elemento.esfamilia
                                ? "INSERT INTO RolFamilia(CodigoRol, CodigoFamilia) VALUES(@padre, @hijo)"
                                : "INSERT INTO RolPatente(CodigoRol, CodigoPatente) VALUES(@padre, @hijo)";

                            using (SqlCommand comando = new SqlCommand(consulta, conexion, transaccion))
                            {
                                comando.Parameters.AddWithValue("@padre", rol.Codigo);
                                comando.Parameters.AddWithValue("@hijo", elemento.Codigo);
                                if (comando.ExecuteNonQuery() != 1)
                                    throw new Exception("No se pudo asignar un elemento inicial al rol.");
                            }
                        }

                        transaccion.Commit();
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        public Rol_56PS ObtenerRol(string codigo)
        {
            string queryNom = "SELECT Nombre FROM Roles WHERE CodigoRol = @cod AND Activo = 1";
            SqlParameter[] paramNom = { new SqlParameter("@cod", codigo) };

            object result = DAL_56PS.ExecuteScalar(queryNom, paramNom);
            if (result == null)
                throw new Exception("El rol no existe o se encuentra inactivo.");

            string nombreRol = result.ToString();

            Rol_56PS raiz = new Rol_56PS
            {
                Codigo = codigo,
                Nombre = nombreRol,
                esfamilia = true
            };

            string queryFam = "SELECT f.CodigoFamilia, f.Nombre FROM Familias f " +
                "INNER JOIN RolFamilia pf ON f.CodigoFamilia = pf.CodigoFamilia " +
                "WHERE pf.CodigoRol = @cod AND f.Activa = 1";
            SqlParameter[] paramFam = { new SqlParameter("@cod", codigo) };

            DataSet dsFam = DAL_56PS.ExecuteDataSet(queryFam, paramFam);

            List<Familia_56PS> familias = new List<Familia_56PS>();
            foreach (DataRow row in dsFam.Tables[0].Rows)
            {
                var fam = new Familia_56PS
                {
                    Codigo = row["CodigoFamilia"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    esfamilia = true
                };
                familias.Add(fam);
            }

            foreach (var fam in familias)
            {
                CargarHijosFamilia(fam, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { fam.Codigo });
                raiz.Agregar(fam);
            }

            string queryPat = "SELECT p.CodigoPatente, p.Nombre FROM Patentes p " +
                "INNER JOIN RolPatente pp ON p.CodigoPatente = pp.CodigoPatente " +
                "WHERE pp.CodigoRol = @cod";
            SqlParameter[] paramPat = { new SqlParameter("@cod", codigo) };

            DataSet dsPat = DAL_56PS.ExecuteDataSet(queryPat, paramPat);

            foreach (DataRow row in dsPat.Tables[0].Rows)
            {
                var pat = new Patente_56PS
                {
                    Codigo = row["CodigoPatente"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    esfamilia = false
                };
                raiz.Agregar(pat);
            }

            return raiz;
        }

        public Familia_56PS ObtenerFamilia(string codigo)
        {
            string query = "SELECT * FROM Familias WHERE CodigoFamilia = @cod AND Activa = 1";
            SqlParameter[] parametros = { new SqlParameter("@cod", codigo) };

            DataSet ds = DAL_56PS.ExecuteDataSet(query, parametros);

            if (ds.Tables[0].Rows.Count == 0)
            {
                throw new Exception("La familia no existe o se encuentra inactiva.");
            }

            DataRow row = ds.Tables[0].Rows[0];

            Familia_56PS familia = new Familia_56PS
            {
                Codigo = row["CodigoFamilia"].ToString(),
                Nombre = row["Nombre"].ToString(),
                esfamilia = true,
                activo = true
            };

            CargarHijosFamilia(familia, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { familia.Codigo });

            return familia;
        }

        private void CargarHijosFamilia(Familia_56PS familia, HashSet<string> ruta)
        {
            string queryHijas = "SELECT f.CodigoFamilia, f.Nombre FROM FamiliaFamilia ff " +
                "INNER JOIN Familias f ON ff.CodFamiliaHija = f.CodigoFamilia " +
                "WHERE ff.CodigoFamilia = @cod AND f.Activa = 1";
            SqlParameter[] paramHijas = { new SqlParameter("@cod", familia.Codigo) };

            DataSet dsHijas = DAL_56PS.ExecuteDataSet(queryHijas, paramHijas);

            List<Familia_56PS> familiasHijas = new List<Familia_56PS>();
            foreach (DataRow row in dsHijas.Tables[0].Rows)
            {
                var hija = new Familia_56PS();
                hija.Codigo = row["CodigoFamilia"].ToString();
                hija.Nombre = row["Nombre"].ToString();
                hija.esfamilia = true;
                familiasHijas.Add(hija);
            }

            foreach (var hija in familiasHijas)
            {
                if (!ruta.Add(hija.Codigo))
                    continue;

                CargarHijosFamilia(hija, ruta);
                familia.Agregar(hija);
                ruta.Remove(hija.Codigo);
            }

            string queryPats = "SELECT p.CodigoPatente, p.Nombre FROM FamiliaPatente fp " +
                "INNER JOIN Patentes p ON fp.CodigoPatente = p.CodigoPatente " +
                "WHERE fp.CodigoFamilia = @cod";
            SqlParameter[] paramPats = { new SqlParameter("@cod", familia.Codigo) };

            DataSet dsPats = DAL_56PS.ExecuteDataSet(queryPats, paramPats);

            foreach (DataRow row in dsPats.Tables[0].Rows)
            {
                var patente = new Patente_56PS();
                patente.Codigo = row["CodigoPatente"].ToString();
                patente.Nombre = row["Nombre"].ToString();
                patente.esfamilia = false;
                familia.Agregar(patente);
            }
        }

        public bool VerificarFamiliaEliminada(Familia_56PS f)
        {
            string query = "SELECT Activa FROM Familias WHERE CodigoFamilia = @cod";
            SqlParameter[] parametros = { new SqlParameter("@cod", f.Codigo) };

            DataSet ds = DAL_56PS.ExecuteDataSet(query, parametros);

            if (ds.Tables[0].Rows.Count > 0)
            {
                f.activo = Convert.ToBoolean(ds.Tables[0].Rows[0]["Activa"]);
            }

            return f.activo;
        }

        public bool RolEstaActivo(Rol_56PS p)
        {
            string query = "SELECT Activo FROM Roles WHERE CodigoRol = @cod";
            SqlParameter[] parametros = { new SqlParameter("@cod", p.Codigo) };

            DataSet ds = DAL_56PS.ExecuteDataSet(query, parametros);

            if (ds.Tables[0].Rows.Count > 0)
            {
                p.activo = Convert.ToBoolean(ds.Tables[0].Rows[0]["Activo"]);
            }

            return p.activo;
        }
    }
}
