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
            if (string.IsNullOrWhiteSpace(backupPath))
                throw new Exception("Debe seleccionar una carpeta para guardar el backup.");

            string nombreArchivo = $"MiSistema.BCK_{DateTime.Now:ddMMyy_HHmm}.bak";
            string rutaDestinoUsuario = Path.Combine(backupPath, nombreArchivo);
            string carpetaBackupServidor = ObtenerCarpetaBackupServidor();
            string rutaBackupServidor = Path.Combine(carpetaBackupServidor, nombreArchivo);

            string comandoBackup = $"BACKUP DATABASE [{dbname}] TO DISK = @RutaCompleta WITH INIT";

            var parametros = new SqlParameter[]
            {
                new SqlParameter("@RutaCompleta", rutaBackupServidor)
            };

            DAL_625NS.DAL_56PS.ExecuteNonQuery(comandoBackup, parametros);

            try
            {
                if (!RutasIguales(rutaBackupServidor, rutaDestinoUsuario))
                {
                    File.Copy(rutaBackupServidor, rutaDestinoUsuario, true);
                    TryDelete(rutaBackupServidor);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"El backup se generó en '{rutaBackupServidor}', pero no se pudo copiar a '{rutaDestinoUsuario}'. Detalle: {ex.Message}",
                    ex
                );
            }
        }

        public void RealizarRestore(string backupFilePath)
        {
            if (string.IsNullOrWhiteSpace(backupFilePath))
                throw new Exception("Debe seleccionar un archivo de respaldo.");

            if (!File.Exists(backupFilePath))
                throw new Exception("El archivo de respaldo seleccionado no existe.");

            string masterConnectionString = connectionString.Replace(dbname, "master");
            string rutaRestoreServidor = PrepararArchivoRestoreParaServidor(backupFilePath);

            try
            {
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
                        cmd.Parameters.AddWithValue("@BackupFile", rutaRestoreServidor);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            finally
            {
                if (!RutasIguales(backupFilePath, rutaRestoreServidor))
                {
                    TryDelete(rutaRestoreServidor);
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

        private string ObtenerCarpetaBackupServidor()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
DECLARE @path NVARCHAR(4000) = CAST(SERVERPROPERTY('InstanceDefaultBackupPath') AS NVARCHAR(4000));

IF (@path IS NULL OR LEN(@path) = 0)
BEGIN
    SELECT TOP 1 @path = LEFT(physical_name, LEN(physical_name) - CHARINDEX('\', REVERSE(physical_name)) + 1)
    FROM sys.master_files
    WHERE database_id = DB_ID('master') AND file_id = 1;
END

SELECT @path;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    string path = result?.ToString();

                    if (string.IsNullOrWhiteSpace(path))
                        throw new Exception("No se pudo obtener la carpeta de backup del servidor SQL.");

                    return path;
                }
            }
        }

        private string PrepararArchivoRestoreParaServidor(string backupFilePath)
        {
            string carpetaBackupServidor = ObtenerCarpetaBackupServidor();
            string rutaRestoreServidor = Path.Combine(carpetaBackupServidor, Path.GetFileName(backupFilePath));

            if (!RutasIguales(backupFilePath, rutaRestoreServidor))
            {
                File.Copy(backupFilePath, rutaRestoreServidor, true);
            }

            return rutaRestoreServidor;
        }

        private bool RutasIguales(string primera, string segunda)
        {
            string ruta1 = Path.GetFullPath(primera).TrimEnd(Path.DirectorySeparatorChar);
            string ruta2 = Path.GetFullPath(segunda).TrimEnd(Path.DirectorySeparatorChar);
            return string.Equals(ruta1, ruta2, StringComparison.OrdinalIgnoreCase);
        }

        private void TryDelete(string archivo)
        {
            try
            {
                if (File.Exists(archivo))
                    File.Delete(archivo);
            }
            catch
            {
            }
        }


    }
}
