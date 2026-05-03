using BE_56_PS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_56_PS;
namespace ClassLibrary3
{
    public class BLL_BitacoraEvento_56_PS
    {

        DAL_BitacoraEventos_56_PS dal = new DAL_BitacoraEventos_56_PS();

        public void RegistrarEvento(BE_Evento_56_PS e)
        {
            dal.RegistrarEvento(e);

            return;
        }

        public BLL_BitacoraEvento_56_PS() { dal = new DAL_BitacoraEventos_56_PS(); }

        public List<BE_Evento_56_PS> obtenerEventos()
        {
            return dal.obtenerEventos();
        }

    }
}
