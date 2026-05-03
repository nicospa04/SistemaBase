using BE_625NS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_625NS;
namespace ClassLibrary3
{
    public class BLL_BitacoraEvento_56PS
    {

        DAL_BitacoraEventos_56PS dal = new DAL_BitacoraEventos_56PS();

        public void RegistrarEvento(BE_Evento_56PS e)
        {
            dal.RegistrarEvento(e);

            return;
        }

        public BLL_BitacoraEvento_56PS() { dal = new DAL_BitacoraEventos_56PS(); }

        public List<BE_Evento_56PS> obtenerEventos()
        {
            return dal.obtenerEventos();
        }







    }
}
