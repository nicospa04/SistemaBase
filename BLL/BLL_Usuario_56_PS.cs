using BE_56_PS;
using ClassLibrary2;
using Services_56_PS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_56_PS;
namespace BLL
{
    public class BLL_Usuario_56_PS
    {
        private DAL_Usuario_56_PS dal = new DAL_Usuario_56_PS();


        //public bool TienePermiso(string nombrePermiso, BE_Usuario_625NS a)
        //{


        //    //if (a.Rol_625NS == null) return false; codigo usado para cuando exista composite

        //    //var permisos = a.Rol_625NS.obtenerPermisos625NS();
        //    //return permisos.Any(p => p.Nombre_625NS.Equals(nombrePermiso, StringComparison.OrdinalIgnoreCase));

        //  
        //}



        public void cambiarContraseña(string dni, string nuevaContraseña) //si
        {
            dal.cambiarContraseña(dni, nuevaContraseña);
        }

        public void cerrarSesion() //si
        {
            SessionManager_56_PS.getInstancia().cerrarSesion();
        }

        public string crearContraseña(string nombre, string dni) //si
        {
            return CryptoManager_56_PS.Encriptar(nombre + dni);
        }

        public void crearUsuario(BE_Usuario_56_PS usuario) //si
        {
            dal.crearUsuario(usuario);
        }

        public void desbloquearUsuario(string dni) //si
        {
            dal.desbloquearUsuario(dni);
        }

        public bool iniciarSesion(string nombreUsuario, string contraseña) //si
        {
            string contraseñaEncriptada = CryptoManager_56_PS.Encriptar(contraseña);
            return dal.validarUsuario(nombreUsuario, contraseñaEncriptada);
        }

        public void modificarUsuario(BE_Usuario_56_PS usuario) //si
        {
            dal.modificarUsuario(usuario);
        }

        public List<BE_Usuario_56_PS> obtenerUsuarios() //si
        {
            return dal.obtenerUsuarios();
        }

        public bool validarUsuario(string nombreUsuario, string contraseña) //si
        {
            string contraseñaEncriptada = CryptoManager_56_PS.Encriptar(contraseña);
            return dal.validarUsuario(nombreUsuario, contraseñaEncriptada);
        }

        public string verificarEstado(string nombreUsuario) //si
        {
            return dal.verificarEstado(nombreUsuario);
        }

        public void sumarIntentoFallido(string nombreUsuario) //si
        {
            dal.sumarIntentoFallido(nombreUsuario);

            dal.validarCantIntentos(nombreUsuario);
        }

        public void cambiarIdioma(BE_Usuario_56_PS user, string v)  //si
        {
            dal.cambiarIdioma(user, v);
        }

        public void CambiarEstadoActivo(string dni, bool nuevoEstado)
        {
            dal.CambiarEstadoActivo(dni, nuevoEstado);
        }

        public object obtenerUsuarioPorDni(string dni)
        {
            throw new NotImplementedException();
        }

        public void CambiarEstadoBloqueado(string dni, bool v)
        {
            throw new NotImplementedException();
        }
    }
}
