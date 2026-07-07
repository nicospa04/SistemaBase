using DAL;
using DAL_625NS;
using Services_625NS;
using Servicio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_DigitoVerificador_56PS
    {
        DAL_DigitoVerificador_56PS daldv = new DAL_DigitoVerificador_56PS();


        public void CalcularDV(string tabla, DataTable registros)
        {
            if (registros == null || registros.Rows.Count == 0)
                return;

            long totalDVH = CalcularDVH(registros);

            long totalDVV = CalcularDVV(registros);

            string dvhHash = CryptoManager_56PS.Encriptar(totalDVH.ToString());
            string dvvHash = CryptoManager_56PS.Encriptar(totalDVV.ToString());

            DigitoVerificador_56PS dv = new DigitoVerificador_56PS();
            dv.tabla = tabla;
            dv.DVH = dvhHash;
            dv.DVV = dvvHash;

            daldv.GuardarDV(dv);
        }

        public long CalcularDVH(DataTable registros)
        {
            long sumaDVH = 0;

            foreach (DataRow fila in registros.Rows)
            {
                string concatenado = string.Join("", fila.ItemArray.Select(x => x.ToString()));
                string hex = ConvertirAHexadecimal(concatenado);
                sumaDVH += SumarHexadecimal(hex);
            }

            return sumaDVH;
        }

        public long CalcularDVV(DataTable registros)
        {
            long sumaDVV = 0;

            foreach (DataColumn columna in registros.Columns)
            {
                string concatenado = string.Join("", registros.AsEnumerable().Select(r => r[columna].ToString()));
                string hex = ConvertirAHexadecimal(concatenado);
                sumaDVV += SumarHexadecimal(hex);
            }

            return sumaDVV;
        }

        private string ConvertirAHexadecimal(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:X2}", b);
            return hex.ToString();
        }

        private long SumarHexadecimal(string hex)
        {
            long suma = 0;
            foreach (char c in hex)
            {
                if (int.TryParse(c.ToString(), System.Globalization.NumberStyles.HexNumber, null, out int valor))
                    suma += valor;
            }
            return suma;
        }
        public void RecalcularDV()
        {
            List<DigitoVerificador_56PS> listaDV = daldv.ConsultarDV();

            if (listaDV == null || listaDV.Count == 0)
            {
                throw new Exception("No existen registros en la tabla de Digitos Verificadores. No es posible verificar la integridad.");
            }

            foreach (var dv in listaDV)
            {
                DataTable registros = DAL_625NS.DAL_56PS.ConsultarTabla(dv.tabla);


                long nuevoDVV = CalcularDVV(registros);
                long nuevoDVH = CalcularDVH(registros);

                string nuevoDVVHash = CryptoManager_56PS.Encriptar(nuevoDVV.ToString());
                string nuevoDVHHash = CryptoManager_56PS.Encriptar(nuevoDVH.ToString());

                DigitoVerificador_56PS digitonuevo = new DigitoVerificador_56PS();
                dv.tabla = dv.tabla;
                dv.DVH = nuevoDVHHash;
                dv.DVV = nuevoDVVHash;

                daldv.GuardarDV(dv);
            }

        }


        public (bool tablaDVVacia, List<string> tablasConError) Revision()
        {
            List<string> errores = new List<string>();
            List<DigitoVerificador_56PS> listaDV = daldv.ConsultarDV();

            if (listaDV == null || listaDV.Count == 0)
            {
                return (true, errores);
            }

            foreach (var dv in listaDV)
            {
                DataTable registros = DAL_56PS.ConsultarTabla(dv.tabla);

                long nuevoDVV = CalcularDVV(registros);
                long nuevoDVH = CalcularDVH(registros);

                string nuevoDVVHash = CryptoManager_56PS.Encriptar(nuevoDVV.ToString());
                string nuevoDVHHash = CryptoManager_56PS.Encriptar(nuevoDVH.ToString());

                if (nuevoDVVHash != dv.DVV || nuevoDVHHash != dv.DVH)
                {
                    errores.Add(dv.tabla);
                }
            }

            return (false, errores);
        }
    }
}
