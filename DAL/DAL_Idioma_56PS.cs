using Servicio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL_625NS
{
    public class DAL_Idioma_56PS
    {
        public List<Idioma_56PS> ObtenerIdiomas()
        {
            DataSet datos = DAL_56PS.ExecuteDataSet(
                "SELECT Nombre, Tipo FROM Idioma_56PS ORDER BY Nombre",
                null);

            List<Idioma_56PS> idiomas = new List<Idioma_56PS>();
            foreach (DataRow fila in datos.Tables[0].Rows)
            {
                idiomas.Add(new Idioma_56PS
                {
                    nombre = fila["Nombre"].ToString(),
                    tipo = fila["Tipo"].ToString()
                });
            }

            return idiomas;
        }

        public Idioma_56PS ObtenerIdioma(string tipo)
        {
            DataSet datos = DAL_56PS.ExecuteDataSet(
                "SELECT Nombre, Tipo FROM Idioma_56PS WHERE Tipo = @Tipo",
                new[] { new SqlParameter("@Tipo", tipo) });

            if (datos.Tables[0].Rows.Count == 0)
                return null;

            DataRow fila = datos.Tables[0].Rows[0];
            return new Idioma_56PS
            {
                nombre = fila["Nombre"].ToString(),
                tipo = fila["Tipo"].ToString()
            };
        }
    }
}
