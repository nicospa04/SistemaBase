using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicio;
using DAL_625NS;

namespace DAL
{
    public class DAL_DigitoVerificador_56PS
    {
        public List<DigitoVerificador_56PS> ConsultarDV()
        {
            string query = "SELECT nombreTabla, DVH, DVV FROM DigitoVerificador";
            DataSet ds = DAL_56PS.ExecuteDataSet(query, null);

            List<DigitoVerificador_56PS> lista = new List<DigitoVerificador_56PS>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DigitoVerificador_56PS dv = new DigitoVerificador_56PS
                {
                    tabla = row["nombreTabla"].ToString(),
                    DVH = row["DVH"].ToString(),
                    DVV = row["DVV"].ToString()
                };
                lista.Add(dv);
            }

            return lista;
        }

         public void GuardarDV(DigitoVerificador_56PS dv)
        {
             string queryExiste = "SELECT COUNT(*) FROM DigitoVerificador WHERE nombreTabla = @nombreTabla";
            SqlParameter[] paramExiste = { new SqlParameter("@nombreTabla", dv.tabla) };
            int existe = Convert.ToInt32(DAL_56PS.ExecuteScalar(queryExiste, paramExiste));

             string query = existe > 0
                ? "UPDATE DigitoVerificador SET DVH = @DVH, DVV = @DVV WHERE nombreTabla = @nombreTabla"
                : "INSERT INTO DigitoVerificador (nombreTabla, DVH, DVV) VALUES (@nombreTabla, @DVH, @DVV)";

            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@nombreTabla", dv.tabla),
                new SqlParameter("@DVH", (object)dv.DVH ?? DBNull.Value),
                new SqlParameter("@DVV", (object)dv.DVV ?? DBNull.Value)
            };

            DAL_56PS.ExecuteNonQuery(query, parametros);
        }
    }
}
