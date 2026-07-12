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
        DAL_Perfil_56PS dalperfil = new DAL_Perfil_56PS();

        public void EliminarFamilia(Familia_56PS familia)
        {
            if (familia == null || string.IsNullOrWhiteSpace(familia.Codigo))
                throw new Exception("Debe seleccionar una familia.");

            if (!dalperfil.VerificarFamiliaEliminada(familia))
                throw new Exception("La familia no existe o ya se encuentra inactiva.");

            if (dalperfil.VerificarAsignacionFamilia(familia))
                throw new Exception("No se puede eliminar la familia mientras tenga asignaciones. Primero quite sus relaciones.");

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

            new BLL_DigitoVerificador_56PS().CalcularDV("Familias", DAL_56PS.ConsultarTabla("Familias"));
            new BLL_BitacoraEvento_56PS().RegistrarEvento(evento);
        }

        public void ModificarFamilia(Familia_56PS f)
        {
            if (f == null || string.IsNullOrWhiteSpace(f.Codigo) || string.IsNullOrWhiteSpace(f.Nombre))
                throw new Exception("Debe indicar el código y el nombre de la familia.");

            if (dalperfil.VerificarExistenciaFamiliaNombre(f))
                throw new Exception("Ya existe otra familia con ese nombre.");

            if (!dalperfil.FamiliaEstaActiva(f.Codigo))
                throw new Exception("La familia no existe o se encuentra inactiva.");

            dalperfil.Modificar(f);
            new BLL_DigitoVerificador_56PS().CalcularDV("Familias", DAL_56PS.ConsultarTabla("Familias"));
        }

        public void CrearFamilia(Familia_56PS fam)
        {
            if (fam == null || string.IsNullOrWhiteSpace(fam.Codigo) || string.IsNullOrWhiteSpace(fam.Nombre))
                throw new Exception("Debe indicar el código y el nombre de la familia.");

            if (dalperfil.VerificarExistenciaFamiliaCodigo(fam))
                throw new Exception("Ya existe una familia con ese código.");
            if (dalperfil.VerificarExistenciaFamiliaNombre(fam))
                throw new Exception("Ya existe una familia con ese nombre.");

            try
            {
                dalperfil.CrearFamilia(fam);
                new BLL_DigitoVerificador_56PS().CalcularDV("Familias", DAL_56PS.ConsultarTabla("Familias"));
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
            if (f == null || string.IsNullOrWhiteSpace(f.Codigo))
                throw new Exception("Debe seleccionar la familia de destino.");

            if (p == null || string.IsNullOrWhiteSpace(p.Codigo))
                throw new Exception("Debe seleccionar un permiso o una familia.");

            if (!dalperfil.FamiliaEstaActiva(f.Codigo))
                throw new Exception("La familia de destino no existe o se encuentra inactiva.");

            Familia_56PS familiaActual = dalperfil.ObtenerFamilia(f.Codigo);

            if (familiaActual.Contiene(p.Codigo))
                throw new Exception("La familia ya contiene ese elemento.");

            if (p.esfamilia)
            {
                if (!dalperfil.FamiliaEstaActiva(p.Codigo))
                    throw new Exception("La familia seleccionada no existe o se encuentra inactiva.");

                if (f.Codigo == p.Codigo)
                    throw new Exception("Una familia no puede contenerse a sí misma.");

                if (dalperfil.FamiliaContiene(p.Codigo, f.Codigo))
                    throw new Exception("La asignación genera un ciclo entre familias.");
            }

            dalperfil.AsignarPermisoAFamilia(p, f);
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

        public void EliminarPermisodeFamilia(Perfil_56PS p, string cod)
        {
            if (p == null || string.IsNullOrWhiteSpace(p.Codigo) || string.IsNullOrWhiteSpace(cod))
                throw new Exception("Debe seleccionar una asignación válida.");

            dalperfil.EliminarPermisodeFamilia(p, cod);
            var dv = new BLL_DigitoVerificador_56PS();
            dv.CalcularDV("FamiliaPatente", DAL_56PS.ConsultarTabla("FamiliaPatente"));
            dv.CalcularDV("FamiliaFamilia", DAL_56PS.ConsultarTabla("FamiliaFamilia"));
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
