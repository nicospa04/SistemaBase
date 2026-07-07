using BE_56_PS;
using ClassLibrary2;
using Services_625NS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_625NS;
using System.Data;
namespace BLL
{
    public class BLL_Usuario_56PS
    {
        private DAL_Usuario_56PS dal = new DAL_Usuario_56PS();




        public void cambiarContraseña(string dni, string nuevaContraseña) //si
        {
            dal.cambiarContraseña(dni, nuevaContraseña);
            DataTable dt = DAL_56PS.ConsultarTabla("Usuario");
            new BLL_DigitoVerificador_56PS().CalcularDV("Usuario", dt);
        }

      

        public string crearContraseña(string nombre, string dni) //si
        {
            return CryptoManager_56PS.Encriptar(nombre + dni);
        }

        public void crearUsuario(Usuario_56PS usuario) //si
        {
            dal.crearUsuario(usuario);
            DataTable dt = DAL_56PS.ConsultarTabla("Usuario");
            new BLL_DigitoVerificador_56PS().CalcularDV("Usuario", dt);
        }

        public void desbloquearUsuario(string dni) //si
        {
            dal.desbloquearUsuario(dni);


            DataTable dt = DAL_56PS.ConsultarTabla("Usuario");
            new BLL_DigitoVerificador_56PS().CalcularDV("Usuario", dt);
        }

        public bool iniciarSesion(string nombreUsuario, string contraseña) //si
        {
            string contraseñaEncriptada = CryptoManager_56PS.Encriptar(contraseña);
            return dal.validarUsuario(nombreUsuario, contraseñaEncriptada);
        }

        public void modificarUsuario(Usuario_56PS usuario) //si
        {
            dal.modificarUsuario(usuario);
            DataTable dt = DAL_56PS.ConsultarTabla("Usuario");
            new BLL_DigitoVerificador_56PS().CalcularDV("Usuario", dt);
        }

        public List<Usuario_56PS> obtenerUsuarios() //si
        {
            return dal.obtenerUsuarios();
        }

        public bool validarUsuario(string nombreUsuario, string contraseña) //si
        {
            string contraseñaEncriptada = CryptoManager_56PS.Encriptar(contraseña);
            return dal.validarUsuario(nombreUsuario, contraseñaEncriptada);
        }

        public string verificarEstado(string nombreUsuario) //si
        {
            return dal.verificarEstado(nombreUsuario);
        }


        public Usuario_56PS obtenerUsuarioPorDni(string dni)
        {
            return dal.obtenerUsuarioPorDni(dni);
        }

  

        public bool existeUsuarioConEseUsername(string userName)
        {
            return dal.existeUsuarioConEseUsername(userName);
        }

        public void bloquearUsuario(string dni)
        {
            dal.bloquearUsuario(dni);
            DataTable dt = DAL_56PS.ConsultarTabla("Usuario");
            new BLL_DigitoVerificador_56PS().CalcularDV("Usuario", dt);
        }

        public void cambiarEstadoActivo(string dni)
        {
            dal.cambiarEstadoActivo(dni);
            DataTable dt = DAL_56PS.ConsultarTabla("Usuario");
            new BLL_DigitoVerificador_56PS().CalcularDV("Usuario", dt);
        }

        public void cambiarIdioma(string idiomaActual, string dni)
        {
            dal.cambiarIdioma(idiomaActual, dni);
            DataTable dt = DAL_56PS.ConsultarTabla("Usuario");
            new BLL_DigitoVerificador_56PS().CalcularDV("Usuario", dt);
        }
    }
}
