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
using System.Data;

namespace BLL
{
    public class BLL_Rol_56PS
    {
        DAL_Rol_56PS dalrol = new DAL_Rol_56PS();

        public void CrearRol(Rol_56PS p)
        {
            CrearRol(p, Enumerable.Empty<Rol_56PS>());
        }

        public void CrearRol(Rol_56PS p, IEnumerable<Rol_56PS> elementos)
        {
            if (p == null || string.IsNullOrWhiteSpace(p.Codigo) || string.IsNullOrWhiteSpace(p.Nombre))
                throw new Exception("Debe indicar el código y el nombre del rol.");

            List<Rol_56PS> elementosIniciales = ValidarElementosIniciales(elementos);

            if (dalrol.VerificarExistenciaRolCodigo(p))
            {
                throw new Exception("Ya existe rol con ese codigo");
            }

            if (dalrol.VerificarExistenciaRolNombre(p))
            {
                throw new Exception("Ya existe rol con ese nombre");
            }

            dalrol.CrearRolConElementos(p, elementosIniciales);

            var user = SessionManager_56PS.getInstancia().getUsuarioActivo();
            Evento_56PS evento = new Evento_56PS(
                user.Dni,
                DateTime.Now,
                "Roles",
                "Crear rol",
                Evento_56PS.Criticidad.Alto
            );
            new BLL_BitacoraEvento_56PS().RegistrarEvento(evento);

            BLL_DigitoVerificador_56PS dv = new BLL_DigitoVerificador_56PS();
            dv.CalcularDV("Roles", DAL_56PS.ConsultarTabla("Roles"));
            dv.CalcularDV("RolPatente", DAL_56PS.ConsultarTabla("RolPatente"));
            dv.CalcularDV("RolFamilia", DAL_56PS.ConsultarTabla("RolFamilia"));
        }

        private List<Rol_56PS> ValidarElementosIniciales(IEnumerable<Rol_56PS> elementos)
        {
            List<Rol_56PS> elementosIniciales = elementos == null
                ? new List<Rol_56PS>()
                : elementos.Where(elemento => elemento != null).ToList();

            if (elementosIniciales.Count == 0)
                throw new Exception("No se puede crear un rol vacío. Debe seleccionar al menos una patente o familia.");

            HashSet<string> codigos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (Rol_56PS elemento in elementosIniciales)
            {
                if (string.IsNullOrWhiteSpace(elemento.Codigo))
                    throw new Exception("El elemento inicial no es válido.");

                string clave = elemento.esfamilia + ":" + elemento.Codigo;
                if (!codigos.Add(clave))
                    throw new Exception("No se puede asignar el mismo elemento más de una vez.");

                if (elemento.esfamilia && !dalrol.FamiliaEstaActiva(elemento.Codigo))
                    throw new Exception("La familia seleccionada no existe o se encuentra inactiva.");
            }

            return elementosIniciales;
        }

        public string ObtenerNombredeRol(string cod)
        {
            return dalrol.ObtenerNombredeRol(cod);
        }

        public Rol_56PS ObtenerRol(string cod)
        {
            return dalrol.ObtenerRol(cod);
        }

        public Rol_56PS ObtenerPermiso(string nombre)
        {
            return dalrol.ObtenerPermiso(nombre);
        }

        public void EliminarPermisodeRol(Rol_56PS p, string codrol)
        {
            if (p == null || string.IsNullOrWhiteSpace(p.Codigo) || string.IsNullOrWhiteSpace(codrol))
                throw new Exception("Debe seleccionar una asignación válida.");

            dalrol.EliminarPermisodeRol(p, codrol);

            var dv = new BLL_DigitoVerificador_56PS();
            dv.CalcularDV("RolPatente", DAL_56PS.ConsultarTabla("RolPatente"));
            dv.CalcularDV("RolFamilia", DAL_56PS.ConsultarTabla("RolFamilia"));
        }

        public string ObtenerCodigoRol(Rol_56PS p)
        {
            return dalrol.ObtenerCodigoRol(p);
        }

        public void EliminarRol(Rol_56PS rol)
        {
            if (dalrol.RolEstaActivo(rol))
            {
                if (dalrol.VerificarExistenciaRolCodigo(rol))
                {
                    if (!dalrol.VerificarAsignacion(rol))
                    {
                        rol.activo = false;
                        dalrol.EliminarRol(rol);
                        new BLL_DigitoVerificador_56PS().CalcularDV("Roles", DAL_56PS.ConsultarTabla("Roles"));

                        var user = SessionManager_56PS.getInstancia().getUsuarioActivo();
                        Evento_56PS evento = new Evento_56PS(
                            user.Dni,
                            DateTime.Now,
                            "Roles",
                            "Eliminar Rol",
                            Evento_56PS.Criticidad.Alto
                        );
                        new BLL_BitacoraEvento_56PS().RegistrarEvento(evento);
                    }
                    else
                    {
                        throw new Exception("No puede borrarlo pq un usuario tiene este rol");
                    }
                }
                else
                {
                    throw new Exception("error");
                }
            }
            else
            {
                throw new Exception("error, el rol no esta activo");

            }
        }

        public void AsignarPermisosARol(Rol_56PS p, Rol_56PS permiso)
        {
            if (p == null || string.IsNullOrWhiteSpace(p.Codigo))
                throw new Exception("Debe seleccionar el rol de destino.");

            if (permiso == null || string.IsNullOrWhiteSpace(permiso.Codigo))
                throw new Exception("Debe seleccionar un permiso o una familia.");

            Rol_56PS rolActual = dalrol.ObtenerRol(p.Codigo);
            if (rolActual.Contiene(permiso.Codigo))
                throw new Exception("El rol ya contiene ese elemento.");

            if (permiso.esfamilia && !dalrol.FamiliaEstaActiva(permiso.Codigo))
                throw new Exception("La familia seleccionada no existe o se encuentra inactiva.");

            dalrol.AsignarPermisosARol(p, permiso);

            var user = SessionManager_56PS.getInstancia().getUsuarioActivo();
            Evento_56PS evento = new Evento_56PS(
                user.Dni,
                DateTime.Now,
                "Roles",
                "Asignar permisos a rol",
                Evento_56PS.Criticidad.Medio
            );
            new BLL_BitacoraEvento_56PS().RegistrarEvento(evento);
            var dv = new BLL_DigitoVerificador_56PS();
            dv.CalcularDV("RolPatente", DAL_56PS.ConsultarTabla("RolPatente"));
            dv.CalcularDV("RolFamilia", DAL_56PS.ConsultarTabla("RolFamilia"));
        }

        public List<Rol_56PS> ObtenerTodosLosRoles()
        {
            return dalrol.ObtenerTodosLosRoles();
        }
    }
}
