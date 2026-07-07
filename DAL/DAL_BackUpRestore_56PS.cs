using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_BackUpRestore_56PS
    {
        private string connectionString = DAL_625NS.DAL_56PS.obtenerConexion();
        private string dbname = DAL_625NS.DAL_56PS.obtenerdbName();

        public void RealizarBackup(string backupPath)
        {

            string nombreArchivo = $"MiSistema.BCK_{DateTime.Now:ddMMyy_HHmm}.bak";
            string rutaCompleta = System.IO.Path.Combine(backupPath, nombreArchivo);

            string comandoBackup = $"BACKUP DATABASE [{dbname}] TO DISK = @RutaCompleta";

            var parametros = new SqlParameter[]
            {
                new SqlParameter("@RutaCompleta", rutaCompleta)
            };

            // Ejecuta a través de la DAL genérica
            DAL_625NS.DAL_56PS.ExecuteNonQuery(comandoBackup, parametros);
        }

        public void RealizarRestore(string backupFilePath)
        {
            string masterConnectionString = connectionString.Replace(dbname, "master");

            using (SqlConnection conn = new SqlConnection(masterConnectionString))
            {
                conn.Open();

                string restoreScript = $@"
            ALTER DATABASE [{dbname}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            RESTORE DATABASE [{dbname}] FROM DISK = @BackupFile WITH REPLACE;
            ALTER DATABASE [{dbname}] SET MULTI_USER;
        ";

                using (SqlCommand cmd = new SqlCommand(restoreScript, conn))
                {
                    cmd.Parameters.AddWithValue("@BackupFile", backupFilePath);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        static string Ins;
        public void RealizarRestoreIniciar(string instancia)
        {
            Ins = instancia;
            DAL_625NS.DAL_56PS.PasarleInstancia(Ins);

            string rutaPrimeraVez = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "entroInstalador.txt");

            if (File.Exists(rutaPrimeraVez))
            {

                InicializarSistema();


                File.Delete(rutaPrimeraVez);
            }
        }
        private void InicializarSistema()
        {
            if (ExisteBaseDeDatos("SistemaMedicoDB"))
                DAL_625NS.DAL_56PS.EjecutarScript("Script_Alter.sql");
            else
                DAL_625NS.DAL_56PS.EjecutarScript("Script_Create.sql");
        }

        public bool ExisteBaseDeDatos(string nombreBD)
        {
            string masterConn = $"Data Source={Ins};Database=master;Trusted_Connection=True;";

            using (var conn = new SqlConnection(masterConn))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT DB_ID(@nombre)", conn);
                cmd.Parameters.AddWithValue("@nombre", nombreBD);

                object result = cmd.ExecuteScalar();

                return result != DBNull.Value && result != null;
            }
        }


    }
}
