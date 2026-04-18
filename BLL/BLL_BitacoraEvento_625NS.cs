using BE_625NS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_625NS;
namespace ClassLibrary3
{
    public class BLL_BitacoraEvento_625NS
    {

        DAL_BitacoraEventos_625NS dal = new DAL_BitacoraEventos_625NS();

        public void RegistrarEvento(BE_Evento_625NS e)
        {
            dal.RegistrarEvento(e);

            return;
        }

        public BLL_BitacoraEvento_625NS() { dal = new DAL_BitacoraEventos_625NS(); }

        public List<BE_Evento_625NS> obtenerEventos()
        {
            return dal.obtenerEventos();
        }







    }
}
