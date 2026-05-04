using BE_625NS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL_625NS
{
    public class DAL_BitacoraEventos_56PS
    {

        public void RegistrarEvento(BE_Evento_56PS e)
        {
            string query = @"INSERT INTO Evento_56PS (dni, fecha, modulo, descripcion, criticidad) 
                             VALUES (@dni, @fecha, @modulo, @descripcion, @criticidad)";

            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@dni", e.dni),
                new SqlParameter("@fecha", e.fecha),
                new SqlParameter("@modulo", e.modulo),
                new SqlParameter("@descripcion", e.descripcion),
                new SqlParameter("@criticidad", e.criticidad.ToString())
            };

            DAL_56PS.ExecuteNonQuery(query, parametros);
        }

        public List<BE_Evento_56PS> obtenerEventos()
        {
            string query = "SELECT numero, dni, fecha, modulo, descripcion, criticidad FROM Evento_56PS ORDER BY fecha DESC";

            DataSet ds = DAL_56PS.ExecuteDataSet(query, null);

            List<BE_Evento_56PS> lista = new List<BE_Evento_56PS>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                BE_Evento_56PS evento = new BE_Evento_56PS(
                    Convert.ToInt32(row["numero"]),
                    row["dni"].ToString(),
                    Convert.ToDateTime(row["fecha"]),
                    row["modulo"].ToString(),
                    row["descripcion"].ToString(),
                    (BE_Evento_56PS.Criticidad)Enum.Parse(typeof(BE_Evento_56PS.Criticidad), row["criticidad"].ToString())
                );

                lista.Add(evento);
            }

            return lista;
        }

    }
}
