using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio
{
    public class LoginAttemptManager_56PS
    {
        private static LoginAttemptManager_56PS instancia;

        private Dictionary<string, IntentosLogin_56PS> intentosPorUsuario;

        private readonly int MAX_INTENTOS = 3;
        private readonly int MINUTOS_BLOQUEO = 5;

        private LoginAttemptManager_56PS()
        {
            intentosPorUsuario = new Dictionary<string, IntentosLogin_56PS>();
        }

        public static LoginAttemptManager_56PS GetInstancia()
        {
            if (instancia == null)
                instancia = new LoginAttemptManager_56PS();

            return instancia;
        }

        public bool RegistrarIntentoFallido(string username)
        {
            if (!intentosPorUsuario.ContainsKey(username))
            {
                intentosPorUsuario[username] = new IntentosLogin_56PS
                {
                    Cantidad = 1,
                    PrimerIntento = DateTime.Now
                };
                return false;
            }

            var intento = intentosPorUsuario[username];

            // Si ya pasó el tiempo, reseteamos
            if ((DateTime.Now - intento.PrimerIntento).TotalMinutes > MINUTOS_BLOQUEO)
            {
                intento.Cantidad = 1;
                intento.PrimerIntento = DateTime.Now;
                return false;
            }

            intento.Cantidad++;

            return intento.Cantidad >= MAX_INTENTOS;
        }

        public void ResetearIntentos(string username)
        {
            if (intentosPorUsuario.ContainsKey(username))
                intentosPorUsuario.Remove(username);
        }
    }
}
