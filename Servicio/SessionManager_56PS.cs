using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE_56_PS;
using Servicio;

namespace ClassLibrary2
{
    public class SessionManager_56PS : IObservable_56PS
    {
        private static SessionManager_56PS instancia;
        private Usuario_56PS usuarioActivo;

        public Idioma_56PS idiomaActual { get; private set; }
        public event EventHandler CierreSistemaSolicitado;
        private List<IdiomaObserver_56PS> Observadores;


        private SessionManager_56PS()
        {
            Observadores = new List<IdiomaObserver_56PS>();
            idiomaActual = new Idioma_56PS("ES");
        }


        public static SessionManager_56PS getInstancia()
        {
            if (instancia == null)
                instancia = new SessionManager_56PS();

            return instancia;
        }

        public void CambiarIdioma(Idioma_56PS idiomaNuevo)
        {
            if (idiomaNuevo == null || string.IsNullOrWhiteSpace(idiomaNuevo.tipo))
                throw new ArgumentException("Debe indicar un idioma válido.", nameof(idiomaNuevo));

            idiomaActual = idiomaNuevo;
            Notificar();
        }

        public void Suscribir(IdiomaObserver_56PS obs)
        {
            if (!Observadores.Contains(obs))
                Observadores.Add(obs);
        }

        public void Desuscribir(IdiomaObserver_56PS obs)
        {
            Observadores.Remove(obs);
        }

        public void Notificar()
        {
            foreach (var obs in Observadores)
                obs.actualizarIdioma();
        }


        public void iniciarSesion(Usuario_56PS usuario)
        {
            usuarioActivo = usuario;
        }

        public void cerrarSesion()
        {
            usuarioActivo = null;
            idiomaActual = new Idioma_56PS("ES");
            Notificar();
        }

        public Usuario_56PS getUsuarioActivo()
        {
            return usuarioActivo;
        }

        public bool haySesionActiva()
        {
            return usuarioActivo != null;
        }

        public void SolicitarCierreSistema()
        {
            CierreSistemaSolicitado?.Invoke(this, EventArgs.Empty);
        }

 
    }
}
