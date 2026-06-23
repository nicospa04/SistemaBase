using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio
{
    public class Perfil_56PS
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool esfamilia { get; set; }
        public bool activo { get; set; }

        public virtual List<Perfil_56PS> hijos { get; set; } = new List<Perfil_56PS>();

        public virtual void Agregar(Perfil_56PS p)
        {
            if (this.Codigo == p.Codigo)
            {
                if (this.esfamilia)
                {
                    throw new Exception("exagregar");
                    //"No se puede agregar una familia/perfil a sí misma"



                }
            }

            if (Contiene(p.Codigo))
            {
                throw new Exception("exagregar1");
                //"Este perfil/familia ya contiene ese permiso o familia"
            }

            if (p.Contiene(this.Codigo))
            {
                throw new Exception("exagregar2");
            }

            hijos.Add(p);

        }

        public bool Contiene(string codigo)
        {
            if (hijos == null)
                return false;
            foreach (var hijo in hijos)
            {
                if (hijo.Codigo == codigo)
                    return true;

                if (hijo.esfamilia && hijo.Contiene(codigo))
                    return true;
            }
            return false;
        }

        public virtual void Eliminar(Perfil_56PS p)
        {
            hijos.Remove(p);
        }

    }
}
