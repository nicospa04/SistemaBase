using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_625NS
{
    public class BE_Evento_56PS
    {

        public enum Criticidad
        {
            Bajo,
            Medio,
            Alto,
            MuyAlto
        }



        public int numero { get; set; }

        public string dni{ get; set; }

        public DateTime fecha{ get; set; }

        public string modulo{ get; set; }

        public string descripcion{ get; set; }

        public Criticidad criticidad{ get; set; }

        public BE_Evento_56PS(int numero_625NS, string dni_625NS, DateTime fecha_625NS, string modulo_625NS, string descripcion_625NS, Criticidad criticidad_625NS)
        {
            this.numero = numero_625NS;
            this.dni= dni_625NS;
            this.fecha= fecha_625NS;
            this.modulo= modulo_625NS;
            this.descripcion= descripcion_625NS;
            this.criticidad= criticidad_625NS;
        }

        public BE_Evento_56PS(string dni_625NS, DateTime fecha_625NS, string modulo_625NS, string descripcion_625NS, Criticidad criticidad_625NS)
        {

            this.dni= dni_625NS;
            this.fecha= fecha_625NS;
            this.modulo= modulo_625NS;
            this.descripcion= descripcion_625NS;
            this.criticidad= criticidad_625NS;
        }

    }
}
