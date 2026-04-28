using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE_625NS;
namespace ClassLibrary2
{
    public class SessionManager_625NS //: IObservable_625NS
    {
        private static SessionManager_625NS instancia;
        private BE_Usuario_625NS usuarioActivo;


        public string idiomaActual_625NS { get; private set; }
       // private List<IdiomaObserver_625NS> Observadores_625NS;

        //private SessionManager_625NS() { Observadores_625NS = new List<IdiomaObserver_625NS>(); }

        public static SessionManager_625NS getInstancia()
        {
            if (instancia == null)
                instancia = new SessionManager_625NS();

            return instancia;
        }

        //public void CambiarIdioma_625NS(string idiomaNuevo)
        //{
        //    idiomaActual_625NS = idiomaNuevo;
        //    //Notificar_625NS();
        //}


        public void iniciarSesion(BE_Usuario_625NS usuario)
        {
            usuarioActivo = usuario;
        }

        public void cerrarSesion()
        {
            usuarioActivo = null;
        }

        public BE_Usuario_625NS getUsuarioActivo()
        {
            return usuarioActivo;
        }

        public bool haySesionActiva()
        {
            return usuarioActivo != null;
        }

        //public void Suscribir_625NS(IdiomaObserver_625NS obs)
        //{
        //    if (!Observadores_625NS.Contains(obs))
        //        Observadores_625NS.Add(obs);
        //}

        //public void Desuscribir_625NS(IdiomaObserver_625NS obs)
        //{
        //    Observadores_625NS.Remove(obs);
        //}

        //public void Notificar_625NS()
        //{
        //    foreach (var obs in Observadores_625NS)
        //        obs.ActualizarIdioma_625NS();
        //}
    }
}
