using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_625NS;
using Servicio;

namespace BLL
{
    public class BLL_Patente_56PS
    {
        DAL_Rol_56PS dalrol = new DAL_Rol_56PS();

        public List<Patente_56PS> ObtenerPatentes()
        {
            return dalrol.ObtenerPatentes();
        }
    }
}
