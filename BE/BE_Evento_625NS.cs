using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_625NS
{
    public class BE_Evento_625NS
    {

        public enum Criticidad
        {
            Bajo,
            Medio,
            Alto,
            MuyAlto
        }



        public int numero_625NS { get; set; }

        public string dni_625NS { get; set; }

        public DateTime fecha_625NS { get; set; }

        public string modulo_625NS { get; set; }

        public string descripcion_625NS { get; set; }

        public Criticidad criticidad_625NS { get; set; }

        public BE_Evento_625NS(int numero_625NS, string dni_625NS, DateTime fecha_625NS, string modulo_625NS, string descripcion_625NS, Criticidad criticidad_625NS)
        {
            this.numero_625NS = numero_625NS;
            this.dni_625NS = dni_625NS;
            this.fecha_625NS = fecha_625NS;
            this.modulo_625NS = modulo_625NS;
            this.descripcion_625NS = descripcion_625NS;
            this.criticidad_625NS = criticidad_625NS;
        }

        public BE_Evento_625NS(string dni_625NS, DateTime fecha_625NS, string modulo_625NS, string descripcion_625NS, Criticidad criticidad_625NS)
        {

            this.dni_625NS = dni_625NS;
            this.fecha_625NS = fecha_625NS;
            this.modulo_625NS = modulo_625NS;
            this.descripcion_625NS = descripcion_625NS;
            this.criticidad_625NS = criticidad_625NS;
        }

    }
}
