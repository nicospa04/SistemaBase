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
        DAL_Perfil_56PS dalperfil = new DAL_Perfil_56PS();

        public List<Patente_56PS> ObtenerPatentes()
        {
            return dalperfil.ObtenerPatentes();
        }
    }
}
