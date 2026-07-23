namespace Servicio
{
    public static class Permisos_56PS
    {
        public const string ConsultarBitacora = "P02";
        public const string CambiarContrasena = "P05";
        public const string ExportarPdfBitacora = "P06";
        public const string CambiarIdioma = "P09";

        public const string ConsultarUsuarios = "P10";
        public const string CrearUsuario = "P11";
        public const string ModificarUsuario = "P12";
        public const string ActivarDesactivarUsuario = "P13";
        public const string DesbloquearUsuario = "P14";

        public const string ConsultarRoles = "P20";
        public const string CrearRol = "P21";
        public const string EliminarRol = "P22";
        public const string AsignarPatenteARol = "P23";
        public const string AsignarFamiliaARol = "P24";
        public const string QuitarPermisoDeRol = "P25";

        public const string ConsultarFamilias = "P30";
        public const string CrearFamilia = "P31";
        public const string ModificarFamilia = "P32";
        public const string EliminarFamilia = "P33";
        public const string AsignarPatenteAFamilia = "P34";
        public const string AsignarFamiliaAFamilia = "P35";
        public const string QuitarPermisoDeFamilia = "P36";

        public const string RealizarBackup = "P50";
        public const string RestaurarBackup = "P51";
        public const string RecalcularDigitosVerificadores = "P52";

        public static readonly string[] Usuarios =
        {
            ConsultarUsuarios,
            CrearUsuario,
            ModificarUsuario,
            ActivarDesactivarUsuario,
            DesbloquearUsuario
        };

        public static readonly string[] Roles =
        {
            ConsultarRoles,
            CrearRol,
            EliminarRol,
            AsignarPatenteARol,
            AsignarFamiliaARol,
            QuitarPermisoDeRol
        };

        public static readonly string[] Familias =
        {
            ConsultarFamilias,
            CrearFamilia,
            ModificarFamilia,
            EliminarFamilia,
            AsignarPatenteAFamilia,
            AsignarFamiliaAFamilia,
            QuitarPermisoDeFamilia
        };

        public static readonly string[] Auditoria =
        {
            ConsultarBitacora,
            ExportarPdfBitacora
        };

        public static readonly string[] Administracion =
        {
            ConsultarUsuarios,
            CrearUsuario,
            ModificarUsuario,
            ActivarDesactivarUsuario,
            DesbloquearUsuario,
            ConsultarRoles,
            CrearRol,
            EliminarRol,
            AsignarPatenteARol,
            AsignarFamiliaARol,
            QuitarPermisoDeRol,
            ConsultarFamilias,
            CrearFamilia,
            ModificarFamilia,
            EliminarFamilia,
            AsignarPatenteAFamilia,
            AsignarFamiliaAFamilia,
            QuitarPermisoDeFamilia,
            ConsultarBitacora,
            ExportarPdfBitacora,
            RealizarBackup,
            RestaurarBackup,
            RecalcularDigitosVerificadores
        };
    }
}
