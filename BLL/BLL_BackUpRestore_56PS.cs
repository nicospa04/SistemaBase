using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_BackUpRestore_56PS
    {
        private DAL_BackUpRestore_56PS dalbackup = new DAL_BackUpRestore_56PS();

        public static void Instalador(string instancia)
        {
            DAL_BackUpRestore_56PS a = new DAL_BackUpRestore_56PS();
            if (a.RealizarRestoreIniciar(instancia))
                new BLL_DigitoVerificador_56PS().RecalcularDV();
        }

        public void RealizarBackup(string backupPath)
        {
            dalbackup.RealizarBackup(backupPath);
        }
        public void RealizarRestore(string restorePath)
        {
            dalbackup.RealizarRestore(restorePath);
        }
    }
}
