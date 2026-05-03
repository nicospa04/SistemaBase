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
            string query = @"INSERT INTO Evento_625NS (dni_625NS, fecha_625NS, modulo_625NS, descripcion_625NS, criticidad_625NS) 
                             VALUES (@dni, @fecha, @modulo, @descripcion, @criticidad)";

            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@dni", e.dni_625NS),
                new SqlParameter("@fecha", e.fecha_625NS),
                new SqlParameter("@modulo", e.modulo_625NS),
                new SqlParameter("@descripcion", e.descripcion_625NS),
                new SqlParameter("@criticidad", e.criticidad_625NS.ToString())
            };

            DAL_56PS.ExecuteNonQuery(query, parametros);
        }

        public List<BE_Evento_56PS> obtenerEventos()
        {
            string query = "SELECT numero, dni_625NS, fecha_625NS, modulo_625NS, descripcion_625NS, criticidad_625NS FROM Evento_625NS ORDER BY fecha_625NS DESC";

            DataSet ds = DAL_56PS.ExecuteDataSet(query, null);

            List<BE_Evento_56PS> lista = new List<BE_Evento_56PS>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                BE_Evento_56PS evento = new BE_Evento_56PS(
                    Convert.ToInt32(row["numero"]),
                    row["dni_625NS"].ToString(),
                    Convert.ToDateTime(row["fecha_625NS"]),
                    row["modulo_625NS"].ToString(),
                    row["descripcion_625NS"].ToString(),
                    (BE_Evento_56PS.Criticidad)Enum.Parse(typeof(BE_Evento_56PS.Criticidad), row["criticidad_625NS"].ToString())
                );

                lista.Add(evento);
            }

            return lista;
        }

    }
}
