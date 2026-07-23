using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_BackUpRestore_56PS
    {
        private string dbname = DAL_625NS.DAL_56PS.obtenerdbName();
        private string connectionString => DAL_625NS.DAL_56PS.obtenerConexion();
        private string masterConnectionString => DAL_625NS.DAL_56PS.obtenerConexionMaster();

        public void RealizarBackup(string backupPath)
        {
            if (string.IsNullOrWhiteSpace(backupPath))
                throw new Exception("Debe seleccionar una carpeta para guardar el backup.");

            string nombreArchivo = $"MiSistema.BCK_{DateTime.Now:ddMMyy_HHmm}.bak";
            string rutaDestinoUsuario = Path.Combine(backupPath, nombreArchivo);
            Directory.CreateDirectory(backupPath);

            Exception errorBackupDirecto = null;
            try
            {
                EjecutarBackupEnRuta(rutaDestinoUsuario);
                return;
            }
            catch (Exception ex)
            {
                errorBackupDirecto = ex;
            }

            string rutaBackupTrabajo = Path.Combine(ObtenerCarpetaTrabajoServidor(), nombreArchivo);
            try
            {
                EjecutarBackupEnRuta(rutaBackupTrabajo);
                File.Copy(rutaBackupTrabajo, rutaDestinoUsuario, true);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"No se pudo guardar el backup en '{rutaDestinoUsuario}'. Detalle: {ex.Message}. Primer intento: {errorBackupDirecto.Message}",
                    ex
                );
            }
            finally
            {
                TryDelete(rutaBackupTrabajo);
            }
        }

        public void RealizarRestore(string backupFilePath)
        {
            if (string.IsNullOrWhiteSpace(backupFilePath))
                throw new Exception("Debe seleccionar un archivo de respaldo.");

            if (!File.Exists(backupFilePath))
                throw new Exception("El archivo de respaldo seleccionado no existe.");

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

        public bool RealizarRestoreIniciar(string instancia)
        {
            DAL_625NS.DAL_56PS.PasarleInstancia(instancia);

            string rutaPrimeraVez = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SistemaBase",
                "entroInstalador.txt");
            bool primeraEjecucionInstalador = File.Exists(rutaPrimeraVez);
            bool existeBase = ExisteBaseDeDatos(dbname);
            bool baseInicializada = existeBase && BaseDatosInicializada();
            bool scriptEjecutado = false;

            if (!existeBase || !baseInicializada)
            {
                DAL_625NS.DAL_56PS.EjecutarScript(ObtenerRutaScript("Script_Create.sql"));
                scriptEjecutado = true;
            }
            else if (primeraEjecucionInstalador)
            {
                DAL_625NS.DAL_56PS.EjecutarScript(ObtenerRutaScript("Script_Alter.sql"));
                scriptEjecutado = true;
            }

            if (primeraEjecucionInstalador)
            {
                File.Delete(rutaPrimeraVez);
            }

            return scriptEjecutado;
        }

        public bool ExisteBaseDeDatos(string nombreBD)
        {
            using (var conn = new SqlConnection(masterConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT DB_ID(@nombre)", conn);
                cmd.Parameters.AddWithValue("@nombre", nombreBD);

                object result = cmd.ExecuteScalar();

                return result != DBNull.Value && result != null;
            }
        }

        private bool BaseDatosInicializada()
        {
            if (!ExisteBaseDeDatos(dbname))
                return false;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT OBJECT_ID(N'dbo.Usuario_56PS', N'U')", conn))
                {
                    object result = cmd.ExecuteScalar();
                    return result != DBNull.Value && result != null;
                }
            }
        }

        private string ObtenerRutaScript(string nombreScript)
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nombreScript);
            if (!File.Exists(ruta))
                throw new Exception($"No se encontro el script de instalacion '{nombreScript}' en '{AppDomain.CurrentDomain.BaseDirectory}'.");

            return ruta;
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
            string carpetaTrabajoServidor = ObtenerCarpetaTrabajoServidor();
            string rutaRestoreServidor = Path.Combine(carpetaTrabajoServidor, Path.GetFileName(backupFilePath));

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

        private void EjecutarBackupEnRuta(string rutaCompleta)
        {
            string comandoBackup = $"BACKUP DATABASE [{dbname}] TO DISK = @RutaCompleta WITH INIT";

            var parametros = new SqlParameter[]
            {
                new SqlParameter("@RutaCompleta", rutaCompleta)
            };

            DAL_625NS.DAL_56PS.ExecuteNonQuery(comandoBackup, parametros);
        }

        private string ObtenerCarpetaTrabajoServidor()
        {
            string carpeta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "SistemaBase",
                "Backups");

            Directory.CreateDirectory(carpeta);
            DarPermisosCarpeta(carpeta);
            return carpeta;
        }

        private void DarPermisosCarpeta(string carpeta)
        {
            try
            {
                DirectorySecurity seguridad = Directory.GetAccessControl(carpeta);
                SecurityIdentifier todos = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                FileSystemAccessRule regla = new FileSystemAccessRule(
                    todos,
                    FileSystemRights.Modify,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow);

                seguridad.SetAccessRule(regla);
                Directory.SetAccessControl(carpeta, seguridad);
            }
            catch
            {
            }
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
