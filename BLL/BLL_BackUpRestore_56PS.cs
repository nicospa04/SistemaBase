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
