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
    public class DAL_Perfil_56PS
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

        public string ObtenerNombredePerfil(string codigo)
        {
            string query = "SELECT Nombre FROM Perfiles WHERE CodigoPerfil = @c";
            SqlParameter[] parametros = { new SqlParameter("@c", codigo) };

            DataSet ds = DAL_56PS.ExecuteDataSet(query, parametros);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            return ds.Tables[0].Rows[0]["Nombre"].ToString();
        }

        public void AsignarPermisosAPerfil(Perfil_56PS perfil, Perfil_56PS permiso)
        {
            int rowsaf = 0;

            if (permiso.esfamilia)
            {
                string query = "INSERT INTO PerfilFamilia(CodigoPerfil, CodigoFamilia) VALUES (@codperfil,@codfamilia)";

                SqlParameter[] parametros = {
                    new SqlParameter("@codperfil", perfil.Codigo),
                    new SqlParameter("@codfamilia", permiso.Codigo)
                };

                rowsaf = DAL_56PS.ExecuteNonQuery(query, parametros);
            }
            else
            {
                string query = "INSERT INTO PerfilPatente(CodigoPerfil, CodigoPatente) VALUES (@codperfil,@codpatente)";

                SqlParameter[] parametros = {
                    new SqlParameter("@codperfil", perfil.Codigo),
                    new SqlParameter("@codpatente", permiso.Codigo)
                };

                rowsaf = DAL_56PS.ExecuteNonQuery(query, parametros);
            }

            if (rowsaf <= 0)
            {
                throw new Exception("ex1");
            }
        }

        public void AsignarPermisoAFamilia(Perfil_56PS permiso, Familia_56PS familia)
        {
            if (permiso.esfamilia)
            {
                string query = "INSERT INTO FamiliaFamilia (CodigoFamilia, CodFamiliaHija) VALUES (@codfam, @codhija)";

                SqlParameter[] parametros = {
                    new SqlParameter("@codfam", familia.Codigo),
                    new SqlParameter("@codhija", permiso.Codigo)
                };

                DAL_56PS.ExecuteNonQuery(query, parametros);
            }
            else
            {
                string query = "INSERT INTO FamiliaPatente (CodigoFamilia, CodigoPatente) VALUES (@codfam, @codpat)";

                SqlParameter[] parametros = {
                    new SqlParameter("@codfam", familia.Codigo),
                    new SqlParameter("@codpat", permiso.Codigo)
                };

                DAL_56PS.ExecuteNonQuery(query, parametros);
            }
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

        public bool VerificarAsignacion(Perfil_56PS p)
        {
            string query = "SELECT COUNT(*) FROM Usuario WHERE Perfil = @cod";
            SqlParameter[] parametros = { new SqlParameter("@cod", p.Codigo) };

            int resultado = Convert.ToInt32(DAL_56PS.ExecuteScalar(query, parametros));
            return resultado > 0;
        }

        public bool VerificarAsignacionFamilia(Familia_56PS f)
        {
            string query = "SELECT COUNT(*) FROM PerfilFamilia WHERE CodigoFamilia = @cod";
            SqlParameter[] parametros = { new SqlParameter("@cod", f.Codigo) };

            int resultado = Convert.ToInt32(DAL_56PS.ExecuteScalar(query, parametros));
            return resultado > 0;
        }

        public List<Perfil_56PS> ObtenerTodosLosPerfiles()
        {
            string query = "SELECT * FROM Perfiles";

            DataSet ds = DAL_56PS.ExecuteDataSet(query, null);

            List<Perfil_56PS> perfiles = new List<Perfil_56PS>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Perfil_56PS p = new Perfil_56PS();
                p.Nombre = row["Nombre"].ToString();
                p.Codigo = row["CodigoPerfil"].ToString();
                p.activo = Convert.ToBoolean(row["Activo"]);
                perfiles.Add(p);
            }

            return perfiles;
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

        public void EliminarPerfil(Perfil_56PS perfil)
        {
            string query = "UPDATE Perfiles SET Activo = @act WHERE CodigoPerfil = @cod";

            SqlParameter[] parametros = {
                new SqlParameter("@act", perfil.activo),
                new SqlParameter("@cod", perfil.Codigo)
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
                familias.Add(f);
            }

            return familias;
        }

        public bool VerificarExistenciaPerfilCodigo(Perfil_56PS p)
        {
            string query = "SELECT COUNT(*) FROM Perfiles WHERE CodigoPerfil = @cod";
            SqlParameter[] parametros = { new SqlParameter("@cod", p.Codigo) };

            int resultado = Convert.ToInt32(DAL_56PS.ExecuteScalar(query, parametros));
            return resultado > 0;
        }

        public bool VerificarExistenciaPerfilNombre(Perfil_56PS p)
        {
            string query = "SELECT COUNT(*) FROM Perfiles WHERE Nombre = @nom";
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
            string query = "SELECT COUNT(*) FROM Familias WHERE Nombre = @nom";
            SqlParameter[] parametros = { new SqlParameter("@nom", f.Nombre) };

            int resultado = Convert.ToInt32(DAL_56PS.ExecuteScalar(query, parametros));
            return resultado > 0;
        }

        public Perfil_56PS ObtenerPermiso(string nombre)
        {
            string query = "SELECT Nombre, CodigoPatente FROM Patentes WHERE Nombre = @nom";
            SqlParameter[] parametros = { new SqlParameter("@nom", nombre) };

            DataSet ds = DAL_56PS.ExecuteDataSet(query, parametros);

            Perfil_56PS permiso = new Perfil_56PS();

            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                permiso.Nombre = row["Nombre"].ToString();
                permiso.Codigo = row["CodigoPatente"].ToString();
            }

            return permiso;
        }

        public void EliminarPermisodePerfil(Perfil_56PS p, string codperfil)
        {
            if (p.esfamilia)
            {
                string query = "DELETE FROM PerfilFamilia WHERE CodigoPerfil = @codperf AND CodigoFamilia = @codfam";

                SqlParameter[] parametros = {
                    new SqlParameter("@codfam", p.Codigo),
                    new SqlParameter("@codperf", codperfil)
                };

                DAL_56PS.ExecuteNonQuery(query, parametros);
            }
            else
            {
                string query = "DELETE FROM PerfilPatente WHERE CodigoPerfil = @codperf AND CodigoPatente = @codpat";

                SqlParameter[] parametros = {
                    new SqlParameter("@codpat", p.Codigo),
                    new SqlParameter("@codperf", codperfil)
                };

                DAL_56PS.ExecuteNonQuery(query, parametros);
            }
        }

        public void EliminarPermisodeFamilia(Perfil_56PS p, string codperfil)
        {
            if (p.esfamilia)
            {
                string query = "DELETE FROM FamiliaFamilia WHERE CodigoFamilia = @codfam AND CodFamiliaHija = @codfamhija";

                SqlParameter[] parametros = {
                    new SqlParameter("@codfamhija", p.Codigo),
                    new SqlParameter("@codfam", codperfil)
                };

                DAL_56PS.ExecuteNonQuery(query, parametros);
            }
            else
            {
                string query = "DELETE FROM FamiliaPatente WHERE CodigoFamilia = @codfam AND CodigoPatente = @codpat";

                SqlParameter[] parametros = {
                    new SqlParameter("@codpat", p.Codigo),
                    new SqlParameter("@codfam", codperfil)
                };

                DAL_56PS.ExecuteNonQuery(query, parametros);
            }
        }

        public string ObtenerCodigoPerfil(Perfil_56PS p)
        {
            string query = "SELECT CodigoPerfil FROM Perfiles WHERE Nombre = @nom";
            SqlParameter[] parametros = { new SqlParameter("@nom", p.Nombre) };

            DataSet ds = DAL_56PS.ExecuteDataSet(query, parametros);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            return ds.Tables[0].Rows[0]["CodigoPerfil"].ToString();
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

        public void CrearPerfil(Perfil_56PS perfil)
        {
            string query = "INSERT INTO Perfiles(Nombre,CodigoPerfil,Activo) VALUES(@nom,@cod,@act)";

            SqlParameter[] parametros = {
                new SqlParameter("@nom", perfil.Nombre),
                new SqlParameter("@cod", perfil.Codigo),
                new SqlParameter("@act", perfil.activo)
            };

            int rowsaff = DAL_56PS.ExecuteNonQuery(query, parametros);

            if (rowsaff <= 0)
            {
                throw new Exception("ex5");
            }
        }

        public Perfil_56PS ObtenerPerfil(string codigo)
        {
            // Obtener nombre del perfil
            string queryNom = "SELECT Nombre FROM Perfiles WHERE CodigoPerfil = @cod";
            SqlParameter[] paramNom = { new SqlParameter("@cod", codigo) };

            object result = DAL_56PS.ExecuteScalar(queryNom, paramNom);
            string nombrePerfil = result != null ? result.ToString() : "(Desconocido)";

            Perfil_56PS raiz = new Perfil_56PS
            {
                Codigo = codigo,
                Nombre = nombrePerfil,
                esfamilia = true
            };

            // Obtener familias del perfil
            string queryFam = "SELECT f.CodigoFamilia, f.Nombre FROM Familias f " +
                "INNER JOIN PerfilFamilia pf ON f.CodigoFamilia = pf.CodigoFamilia " +
                "WHERE pf.CodigoPerfil = @cod";
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
                CargarHijosFamilia(fam);
                raiz.Agregar(fam);
            }

            // Obtener patentes del perfil
            string queryPat = "SELECT p.CodigoPatente, p.Nombre FROM Patentes p " +
                "INNER JOIN PerfilPatente pp ON p.CodigoPatente = pp.CodigoPatente " +
                "WHERE pp.CodigoPerfil = @cod";
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
            string query = "SELECT * FROM Familias WHERE CodigoFamilia = @cod";
            SqlParameter[] parametros = { new SqlParameter("@cod", codigo) };

            DataSet ds = DAL_56PS.ExecuteDataSet(query, parametros);

            if (ds.Tables[0].Rows.Count == 0)
            {
                throw new Exception("ex6");
            }

            DataRow row = ds.Tables[0].Rows[0];

            Familia_56PS familia = new Familia_56PS
            {
                Codigo = row["CodigoFamilia"].ToString(),
                Nombre = row["Nombre"].ToString(),
                esfamilia = true
            };

            CargarHijosFamilia(familia);

            return familia;
        }

        private void CargarHijosFamilia(Familia_56PS familia)
        {
            // Cargar familias hijas
            string queryHijas = "SELECT f.CodigoFamilia, f.Nombre FROM FamiliaFamilia ff " +
                "INNER JOIN Familias f ON ff.CodFamiliaHija = f.CodigoFamilia " +
                "WHERE ff.CodigoFamilia = @cod";
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
                CargarHijosFamilia(hija);
                familia.Agregar(hija);
            }

            // Cargar patentes
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

        public bool PerfilEstaActivo(Perfil_56PS p)
        {
            string query = "SELECT Activo FROM Perfiles WHERE CodigoPerfil = @cod";
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
