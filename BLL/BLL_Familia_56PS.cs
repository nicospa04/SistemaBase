using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_625NS;
using Servicio;
using ClassLibrary2;
using ClassLibrary3;
using BE_56_PS;
using BE_625NS;
namespace BLL
{
    public class BLL_Familia_56PS
    {
        DAL_Perfil_56PS dalperfil = new DAL_Perfil_56PS();

        public void EliminarFamilia(Familia_56PS familia)
        {
            if (dalperfil.VerificarFamiliaEliminada(familia))
            {
                if (!dalperfil.VerificarAsignacionFamilia(familia))
                {
                    familia.activo = false;
                    dalperfil.EliminarFamilia(familia);

                    var user = SessionManager_56PS.getInstancia().getUsuarioActivo();
                    Evento_56PS evento = new Evento_56PS(
                        user.Dni,
                        DateTime.Now,
                        "Familias",
                        "Eliminar familia",
                        Evento_56PS.Criticidad.Alto
                    );
                    new BLL_BitacoraEvento_56PS().RegistrarEvento(evento);
                }
                else
                {
                    throw new Exception("asignar");
                }
            }
            else
            {
                throw new Exception("ex1");
            }
        }

        public void ModificarFamilia(Familia_56PS f)
        {
            if (dalperfil.VerificarExistenciaFamiliaNombre(f))
            {
                throw new Exception("ex3");
            }
            else
            {
                if (dalperfil.VerificarExistenciaFamiliaCodigo(f))
                {
                    dalperfil.Modificar(f);
                }
            }
        }

        public void CrearFamilia(Familia_56PS fam)
        {
            if (dalperfil.VerificarExistenciaFamiliaCodigo(fam))
            {
                throw new Exception("ex2");
            }
            if (dalperfil.VerificarExistenciaFamiliaNombre(fam))
            {
                throw new Exception("ex3");
            }

            try
            {
                dalperfil.CrearFamilia(fam);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

            var user = SessionManager_56PS.getInstancia().getUsuarioActivo();
            Evento_56PS evento = new Evento_56PS(
                user.Dni,
                DateTime.Now,
                "Familias",
                "Crear familia",
                Evento_56PS.Criticidad.Alto
            );
            new BLL_BitacoraEvento_56PS().RegistrarEvento(evento);
        }

        public void AsignarPermisoAFamilia(Familia_56PS f, Perfil_56PS p)
        {
            dalperfil.AsignarPermisoAFamilia(p, f);

            var user = SessionManager_56PS.getInstancia().getUsuarioActivo();
            Evento_56PS evento = new Evento_56PS(
                user.Dni,
                DateTime.Now,
                "Familias",
                "Asignar permisos a familia",
                Evento_56PS.Criticidad.Medio
            );
            new BLL_BitacoraEvento_56PS().RegistrarEvento(evento);
        }

        public void EliminarPermisodeFamilia(Perfil_56PS p, string cod)
        {
            try
            {
                dalperfil.EliminarPermisodeFamilia(p, cod);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Familia_56PS ObtenerFamilia(string codigo)
        {
            return dalperfil.ObtenerFamilia(codigo);
        }

        public List<Familia_56PS> ObtenerFamilias()
        {
            return dalperfil.ObtenerFamilias();
        }
    }
}
