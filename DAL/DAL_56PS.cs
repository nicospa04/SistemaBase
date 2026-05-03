using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace DAL_625NS
{
    public static class DAL_56PS
    {
        private static string dbname = "SistemaMedicoDB";
        private static string conexion = $@"Data Source=COMPURELOCA;Initial Catalog={dbname};Integrated Security=True";


        static string Ins;

        public static SqlConnection con = new SqlConnection($"Data Source={Ins};Initial Catalog=HotelParis;Integrated Security=True;");

        static internal void PasarleInstancia(string ins)
        {
            Ins = ins;

        }

        public static void EjecutarScript(string archivo)
        {
            string connStr = $"Data Source={Ins};Trusted_Connection=True;";

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                string script = File.ReadAllText(archivo);

                // Partir por GO
                var comandos = script.Split(
                    new[] { "\r\nGO\r\n", "\nGO\n", "\rGO\r" },
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (string comando in comandos)
                {
                    if (string.IsNullOrWhiteSpace(comando)) continue;

                    using (var cmd = new SqlCommand(comando, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static int ExecuteNonQuery(string query, SqlParameter[] parametros)
        {
            using (SqlConnection conn = new SqlConnection(conexion))
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


        public static DataTable ConsultarTabla(string nombreTabla)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(conexion))
                {
                    string query = $"SELECT * FROM {nombreTabla}";
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





        public static string obtenerdbName() { return dbname; }
        public static string obtenerConexion() { return conexion; }

        public static object ExecuteScalar(string query, SqlParameter[] parametros)
        {
            using (SqlConnection conn = new SqlConnection(conexion))
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
            using (SqlConnection conn = new SqlConnection(conexion))
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
