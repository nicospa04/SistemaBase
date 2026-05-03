using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE_625NS;
namespace ClassLibrary2
{
    public class SessionManager_56PS //: IObservable_625NS
    {
        private static SessionManager_56PS instancia;
        private Usuario_56PS usuarioActivo;


      

        public static SessionManager_56PS getInstancia()
        {
            if (instancia == null)
                instancia = new SessionManager_56PS();

            return instancia;
        }

        //public void CambiarIdioma_625NS(string idiomaNuevo)
        //{
        //    idiomaActual_625NS = idiomaNuevo;
        //    //Notificar_625NS();
        //}


        public void iniciarSesion(Usuario_56PS usuario)
        {
            usuarioActivo = usuario;
        }

        public void cerrarSesion()
        {
            usuarioActivo = null;
        }

        public Usuario_56PS getUsuarioActivo()
        {
            return usuarioActivo;
        }

        public bool haySesionActiva()
        {
            return usuarioActivo != null;
        }

 
    }
}
