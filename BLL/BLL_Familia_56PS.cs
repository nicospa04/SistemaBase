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
using System.Data;
namespace BLL
{
    public class BLL_Familia_56PS
    {
        DAL_Rol_56PS dalrol = new DAL_Rol_56PS();

        public void EliminarFamilia(Familia_56PS familia)
        {
            if (familia == null || string.IsNullOrWhiteSpace(familia.Codigo))
                throw new Exception("Debe seleccionar una familia.");

            if (!dalrol.VerificarFamiliaEliminada(familia))
                throw new Exception("La familia no existe o ya se encuentra inactiva.");

            if (dalrol.VerificarAsignacionFamilia(familia))
                throw new Exception("No se puede eliminar la familia mientras tenga asignaciones. Primero quite sus relaciones.");

            familia.activo = false;
            dalrol.EliminarFamilia(familia);

            var user = SessionManager_56PS.getInstancia().getUsuarioActivo();
            Evento_56PS evento = new Evento_56PS(
                user.Dni,
                DateTime.Now,
                "Familias",
                "Eliminar familia",
                Evento_56PS.Criticidad.Alto
            );

            new BLL_DigitoVerificador_56PS().CalcularDV("Familias", DAL_56PS.ConsultarTabla("Familias"));
            new BLL_BitacoraEvento_56PS().RegistrarEvento(evento);
        }

        public void ModificarFamilia(Familia_56PS f)
        {
            if (f == null || string.IsNullOrWhiteSpace(f.Codigo) || string.IsNullOrWhiteSpace(f.Nombre))
                throw new Exception("Debe indicar el código y el nombre de la familia.");

            if (dalrol.VerificarExistenciaFamiliaNombre(f))
                throw new Exception("Ya existe otra familia con ese nombre.");

            if (!dalrol.FamiliaEstaActiva(f.Codigo))
                throw new Exception("La familia no existe o se encuentra inactiva.");

            dalrol.Modificar(f);
            new BLL_DigitoVerificador_56PS().CalcularDV("Familias", DAL_56PS.ConsultarTabla("Familias"));
        }

        public void CrearFamilia(Familia_56PS fam)
        {
            CrearFamilia(fam, Enumerable.Empty<Rol_56PS>());
        }

        public void CrearFamilia(Familia_56PS fam, IEnumerable<Rol_56PS> elementos)
        {
            if (fam == null || string.IsNullOrWhiteSpace(fam.Codigo) || string.IsNullOrWhiteSpace(fam.Nombre))
                throw new Exception("Debe indicar el código y el nombre de la familia.");

            List<Rol_56PS> elementosIniciales = ValidarElementosIniciales(fam, elementos);

            if (dalrol.VerificarExistenciaFamiliaCodigo(fam))
                throw new Exception("Ya existe una familia con ese código.");
            if (dalrol.VerificarExistenciaFamiliaNombre(fam))
                throw new Exception("Ya existe una familia con ese nombre.");

            try
            {
                dalrol.CrearFamiliaConElementos(fam, elementosIniciales);
                new BLL_DigitoVerificador_56PS().CalcularDV("Familias", DAL_56PS.ConsultarTabla("Familias"));
                new BLL_DigitoVerificador_56PS().CalcularDV("FamiliaPatente", DAL_56PS.ConsultarTabla("FamiliaPatente"));
                new BLL_DigitoVerificador_56PS().CalcularDV("FamiliaFamilia", DAL_56PS.ConsultarTabla("FamiliaFamilia"));
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

        private List<Rol_56PS> ValidarElementosIniciales(Familia_56PS familia, IEnumerable<Rol_56PS> elementos)
        {
            List<Rol_56PS> elementosIniciales = elementos == null
                ? new List<Rol_56PS>()
                : elementos.Where(elemento => elemento != null).ToList();

            if (elementosIniciales.Count == 0)
                throw new Exception("No se puede crear una familia vacía. Debe seleccionar al menos una patente o familia.");

            HashSet<string> codigos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (Rol_56PS elemento in elementosIniciales)
            {
                if (string.IsNullOrWhiteSpace(elemento.Codigo))
                    throw new Exception("El elemento inicial no es válido.");

                string clave = elemento.esfamilia + ":" + elemento.Codigo;
                if (!codigos.Add(clave))
                    throw new Exception("No se puede asignar el mismo elemento más de una vez.");

                if (elemento.esfamilia)
                {
                    if (familia.Codigo == elemento.Codigo)
                        throw new Exception("Una familia no puede contenerse a sí misma.");

                    if (!dalrol.FamiliaEstaActiva(elemento.Codigo))
                        throw new Exception("La familia seleccionada no existe o se encuentra inactiva.");
                }
            }

            return elementosIniciales;
        }

        public void AsignarPermisoAFamilia(Familia_56PS f, Rol_56PS p)
        {
            if (f == null || string.IsNullOrWhiteSpace(f.Codigo))
                throw new Exception("Debe seleccionar la familia de destino.");

            if (p == null || string.IsNullOrWhiteSpace(p.Codigo))
                throw new Exception("Debe seleccionar un permiso o una familia.");

            if (!dalrol.FamiliaEstaActiva(f.Codigo))
                throw new Exception("La familia de destino no existe o se encuentra inactiva.");

            Familia_56PS familiaActual = dalrol.ObtenerFamilia(f.Codigo);

            if (familiaActual.Contiene(p.Codigo))
                throw new Exception("La familia ya contiene ese elemento.");

            if (p.esfamilia)
            {
                if (!dalrol.FamiliaEstaActiva(p.Codigo))
                    throw new Exception("La familia seleccionada no existe o se encuentra inactiva.");

                if (f.Codigo == p.Codigo)
                    throw new Exception("Una familia no puede contenerse a sí misma.");

                if (dalrol.FamiliaContiene(p.Codigo, f.Codigo))
                    throw new Exception("La asignación genera un ciclo entre familias.");
            }

            dalrol.AsignarPermisoAFamilia(p, f);
            var dv = new BLL_DigitoVerificador_56PS();
            dv.CalcularDV("FamiliaPatente", DAL_56PS.ConsultarTabla("FamiliaPatente"));
            dv.CalcularDV("FamiliaFamilia", DAL_56PS.ConsultarTabla("FamiliaFamilia"));

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

        public void EliminarPermisodeFamilia(Rol_56PS p, string cod)
        {
            if (p == null || string.IsNullOrWhiteSpace(p.Codigo) || string.IsNullOrWhiteSpace(cod))
                throw new Exception("Debe seleccionar una asignación válida.");

            dalrol.EliminarPermisodeFamilia(p, cod);
            var dv = new BLL_DigitoVerificador_56PS();
            dv.CalcularDV("FamiliaPatente", DAL_56PS.ConsultarTabla("FamiliaPatente"));
            dv.CalcularDV("FamiliaFamilia", DAL_56PS.ConsultarTabla("FamiliaFamilia"));
        }

        public Familia_56PS ObtenerFamilia(string codigo)
        {
            return dalrol.ObtenerFamilia(codigo);
        }

        public List<Familia_56PS> ObtenerFamilias()
        {
            return dalrol.ObtenerFamilias();
        }
    }
}
