using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_625NS;
using Servicio;
using ClassLibrary2;
using ClassLibrary3;
using BE_625NS;

namespace BLL
{
    public class BLL_Perfil_56PS
    {
        DAL_Perfil_56PS dalperfil = new DAL_Perfil_56PS();

        public void CrearPerfil(Perfil_56PS p)
        {
            if (dalperfil.VerificarExistenciaPerfilCodigo(p))
            {
                throw new Exception("ex7");
            }

            if (dalperfil.VerificarExistenciaPerfilNombre(p))
            {
                throw new Exception("ex8");
            }

            dalperfil.CrearPerfil(p);

            var user = SessionManager_56PS.getInstancia().getUsuarioActivo();
            Evento_56PS evento = new Evento_56PS(
                user.Dni,
                DateTime.Now,
                "Perfiles",
                "Crear perfil",
                Evento_56PS.Criticidad.Alto
            );
            new BLL_BitacoraEvento_56PS().RegistrarEvento(evento);
        }

        public string ObtenerNombredePerfil(string cod)
        {
            return dalperfil.ObtenerNombredePerfil(cod);
        }

        public Perfil_56PS ObtenerPerfil(string cod)
        {
            return dalperfil.ObtenerPerfil(cod);
        }

        public Perfil_56PS ObtenerPermiso(string nombre)
        {
            return dalperfil.ObtenerPermiso(nombre);
        }

        public void EliminarPermisodePerfil(Perfil_56PS p, string codperfil)
        {
            try
            {
                dalperfil.EliminarPermisodePerfil(p, codperfil);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string ObtenerCodigoPerfil(Perfil_56PS p)
        {
            return dalperfil.ObtenerCodigoPerfil(p);
        }

        public void EliminarPerfil(Perfil_56PS perfil)
        {
            if (dalperfil.PerfilEstaActivo(perfil))
            {
                if (dalperfil.VerificarExistenciaPerfilCodigo(perfil))
                {
                    if (!dalperfil.VerificarAsignacion(perfil))
                    {
                        perfil.activo = false;
                        dalperfil.EliminarPerfil(perfil);

                        var user = SessionManager_56PS.getInstancia().getUsuarioActivo();
                        Evento_56PS evento = new Evento_56PS(
                            user.Dni,
                            DateTime.Now,
                            "Perfiles",
                            "Eliminar Perfil",
                            Evento_56PS.Criticidad.Alto
                        );
                        new BLL_BitacoraEvento_56PS().RegistrarEvento(evento);
                    }
                    else
                    {
                        throw new Exception("ex9");
                    }
                }
                else
                {
                    throw new Exception("ex10");
                }
            }
            else
            {
                throw new Exception("ex11");
            }
        }

        public void AsignarPermisosAPerfil(Perfil_56PS p, Perfil_56PS permiso)
        {
            try
            {
                dalperfil.AsignarPermisosAPerfil(p, permiso);

                var user = SessionManager_56PS.getInstancia().getUsuarioActivo();
                Evento_56PS evento = new Evento_56PS(
                    user.Dni,
                    DateTime.Now,
                    "Perfiles",
                    "Asignar permisos a perfil",
                    Evento_56PS.Criticidad.Medio
                );
                new BLL_BitacoraEvento_56PS().RegistrarEvento(evento);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Perfil_56PS> ObtenerTodosLosPerfiles()
        {
            return dalperfil.ObtenerTodosLosPerfiles();
        }
    }
}
