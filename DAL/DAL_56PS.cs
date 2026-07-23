using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace DAL_625NS
{
    public static class DAL_56PS
    {
        private static string dbname = "SistemaBase";
        private static readonly Dictionary<string, string> ordenTablas = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Evento_56PS", "Numero" },
            { "FamiliaFamilia", "CodigoFamilia, CodFamiliaHija" },
            { "FamiliaPatente", "CodigoFamilia, CodigoPatente" },
            { "Familias", "CodigoFamilia" },
            { "Idioma_56PS", "Tipo" },
            { "Patentes", "CodigoPatente" },
            { "Roles", "CodigoRol" },
            { "RolFamilia", "CodigoRol, CodigoFamilia" },
            { "RolPatente", "CodigoRol, CodigoPatente" },
            { "Usuario_56PS", "DNI" }
        };


        static string Ins;

        public static SqlConnection con = new SqlConnection(obtenerConexion());

        public static void PasarleInstancia(string ins)
        {
            if (string.IsNullOrWhiteSpace(ins))
                return;

            Ins = ins.Trim();
            con.ConnectionString = obtenerConexion();
        }

        public static DataTable ConsultarTabla(string nombreTabla)
        {
            DataTable dt = new DataTable();

            try
            {
                if (!ordenTablas.ContainsKey(nombreTabla))
                {
                    throw new Exception($"La tabla {nombreTabla} no esta habilitada para calculo de DV.");
                }

                using (SqlConnection conn = new SqlConnection(obtenerConexion()))
                {
                    string query = $"SELECT * FROM dbo.[{nombreTabla}] ORDER BY {ordenTablas[nombreTabla]}";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al consultar la tabla {nombreTabla}: {ex.Message}");
            }

            return dt;
        }

        public static void EjecutarScript(string archivo)
        {
            using (var conn = new SqlConnection(obtenerConexionMaster()))
            {
                conn.Open();
                string script = File.ReadAllText(archivo);

                var comandos = Regex.Split(
                    script,
                    @"^[ \t]*GO[ \t]*(?:\r\n|\n|\r|$)",
                    RegexOptions.Multiline | RegexOptions.IgnoreCase);

                foreach (string comando in comandos)
                {
                    if (string.IsNullOrWhiteSpace(comando)) continue;

                    string comandoSql = PrepararComandoScript(comando);
                    if (string.IsNullOrWhiteSpace(comandoSql)) continue;

                    using (var cmd = new SqlCommand(comandoSql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static string PrepararComandoScript(string comando)
        {
            if (comando.IndexOf($"CREATE DATABASE [{dbname}]", StringComparison.OrdinalIgnoreCase) >= 0)
                return $"IF DB_ID(N'{dbname}') IS NULL CREATE DATABASE [{dbname}]";

            if (comando.TrimStart().StartsWith($"ALTER DATABASE [{dbname}]", StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            return comando;
        }

        public static int ExecuteNonQuery(string query, SqlParameter[] parametros)
        {
            using (SqlConnection conn = new SqlConnection(obtenerConexion()))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (parametros != null)
                {
                    foreach (SqlParameter original in parametros)
                    {
                        var copy = new SqlParameter
                        {
                            ParameterName = original.ParameterName,
                            SqlDbType = original.SqlDbType,
                            Size = original.Size,
                            Direction = original.Direction,
                            Value = original.Value ?? DBNull.Value
                        };

                        cmd.Parameters.Add(copy);
                    }
                }

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static string obtenerdbName() { return dbname; }

        public static string obtenerInstancia()
        {
            if (!string.IsNullOrWhiteSpace(Ins))
                return Ins.Trim();

            foreach (string archivoInstancia in new[] { "instancia.txt", "sqlserver.txt" })
            {
                string rutaInstancia = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, archivoInstancia);
                if (File.Exists(rutaInstancia))
                {
                    string instanciaGuardada = File.ReadAllText(rutaInstancia).Trim();
                    if (!string.IsNullOrWhiteSpace(instanciaGuardada))
                    {
                        Ins = instanciaGuardada;
                        return Ins;
                    }
                }
            }

            return ".";
        }

        public static string obtenerConexion()
        {
            return crearConexion(dbname);
        }

        public static string obtenerConexionMaster()
        {
            return crearConexion("master");
        }

        private static string crearConexion(string catalogo)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = obtenerInstancia(),
                IntegratedSecurity = true,
                ConnectTimeout = 15
            };

            if (!string.IsNullOrWhiteSpace(catalogo))
                builder.InitialCatalog = catalogo;

            return builder.ConnectionString;
        }

        public static object ExecuteScalar(string query, SqlParameter[] parametros)
        {
            using (SqlConnection conn = new SqlConnection(obtenerConexion()))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                if (parametros != null)
                    cmd.Parameters.AddRange(parametros);

                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        public static DataSet ExecuteDataSet(string query, SqlParameter[] parametros)
        {
            using (SqlConnection conn = new SqlConnection(obtenerConexion()))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                if (parametros != null)
                    adapter.SelectCommand.Parameters.AddRange(parametros);

                DataSet ds = new DataSet();
                adapter.Fill(ds);
                return ds;
            }
        }
    }
}
