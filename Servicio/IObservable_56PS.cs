using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio
{
    public interface IObservable_56PS
    {
        void Suscribir(IdiomaObserver_56PS obs);
        void Desuscribir(IdiomaObserver_56PS obs);
        void Notificar();
    }
}
