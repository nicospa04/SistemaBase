SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID('dbo.DigitoVerificador', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DigitoVerificador
    (
        nombreTabla VARCHAR(128) NOT NULL PRIMARY KEY,
        DVH VARCHAR(64) NOT NULL,
        DVV VARCHAR(64) NOT NULL
    );
END;

DECLARE @Patentes TABLE
(
    CodigoPatente VARCHAR(10) NOT NULL PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL
);

INSERT INTO @Patentes (CodigoPatente, Nombre)
VALUES
    ('P02', 'Consultar bitacora de eventos'),
    ('P05', 'Cambiar contrasena'),
    ('P06', 'Exportar bitacora a PDF'),
    ('P09', 'Cambiar idioma'),
    ('P10', 'Consultar usuarios'),
    ('P11', 'Crear usuario'),
    ('P12', 'Modificar usuario'),
    ('P13', 'Activar o desactivar usuario'),
    ('P14', 'Desbloquear usuario'),
    ('P20', 'Consultar perfiles'),
    ('P21', 'Crear perfil'),
    ('P22', 'Eliminar perfil'),
    ('P23', 'Asignar patente a perfil'),
    ('P24', 'Asignar familia a perfil'),
    ('P25', 'Quitar permiso de perfil'),
    ('P30', 'Consultar familias'),
    ('P31', 'Crear familia'),
    ('P32', 'Modificar familia'),
    ('P33', 'Eliminar familia'),
    ('P34', 'Asignar patente a familia'),
    ('P35', 'Asignar familia a familia'),
    ('P36', 'Quitar permiso de familia'),
    ('P50', 'Realizar backup'),
    ('P51', 'Restaurar backup'),
    ('P52', 'Recalcular digitos verificadores');

UPDATE p
SET Nombre = np.Nombre
FROM dbo.Patentes p
INNER JOIN @Patentes np ON np.CodigoPatente = p.CodigoPatente;

INSERT INTO dbo.Patentes (CodigoPatente, Nombre)
SELECT np.CodigoPatente, np.Nombre
FROM @Patentes np
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Patentes p
    WHERE p.CodigoPatente = np.CodigoPatente
);

DECLARE @Usuarios TABLE (CodigoPatente VARCHAR(10) NOT NULL PRIMARY KEY);
INSERT INTO @Usuarios VALUES ('P10'), ('P11'), ('P12'), ('P13'), ('P14');

DECLARE @Perfiles TABLE (CodigoPatente VARCHAR(10) NOT NULL PRIMARY KEY);
INSERT INTO @Perfiles VALUES ('P20'), ('P21'), ('P22'), ('P23'), ('P24'), ('P25');

DECLARE @Familias TABLE (CodigoPatente VARCHAR(10) NOT NULL PRIMARY KEY);
INSERT INTO @Familias VALUES ('P30'), ('P31'), ('P32'), ('P33'), ('P34'), ('P35'), ('P36');

INSERT INTO dbo.PerfilPatente (CodigoPerfil, CodigoPatente)
SELECT pp.CodigoPerfil, u.CodigoPatente
FROM dbo.PerfilPatente pp
CROSS JOIN @Usuarios u
WHERE pp.CodigoPatente = 'P01'
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.PerfilPatente x
      WHERE x.CodigoPerfil = pp.CodigoPerfil
        AND x.CodigoPatente = u.CodigoPatente
  );

INSERT INTO dbo.PerfilPatente (CodigoPerfil, CodigoPatente)
SELECT pp.CodigoPerfil, p.CodigoPatente
FROM dbo.PerfilPatente pp
CROSS JOIN @Perfiles p
WHERE pp.CodigoPatente = 'P03'
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.PerfilPatente x
      WHERE x.CodigoPerfil = pp.CodigoPerfil
        AND x.CodigoPatente = p.CodigoPatente
  );

INSERT INTO dbo.PerfilPatente (CodigoPerfil, CodigoPatente)
SELECT pp.CodigoPerfil, f.CodigoPatente
FROM dbo.PerfilPatente pp
CROSS JOIN @Familias f
WHERE pp.CodigoPatente = 'P04'
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.PerfilPatente x
      WHERE x.CodigoPerfil = pp.CodigoPerfil
        AND x.CodigoPatente = f.CodigoPatente
  );

INSERT INTO dbo.FamiliaPatente (CodigoFamilia, CodigoPatente)
SELECT fp.CodigoFamilia, u.CodigoPatente
FROM dbo.FamiliaPatente fp
CROSS JOIN @Usuarios u
WHERE fp.CodigoPatente = 'P01'
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.FamiliaPatente x
      WHERE x.CodigoFamilia = fp.CodigoFamilia
        AND x.CodigoPatente = u.CodigoPatente
  );

INSERT INTO dbo.FamiliaPatente (CodigoFamilia, CodigoPatente)
SELECT fp.CodigoFamilia, p.CodigoPatente
FROM dbo.FamiliaPatente fp
CROSS JOIN @Perfiles p
WHERE fp.CodigoPatente = 'P03'
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.FamiliaPatente x
      WHERE x.CodigoFamilia = fp.CodigoFamilia
        AND x.CodigoPatente = p.CodigoPatente
  );

INSERT INTO dbo.FamiliaPatente (CodigoFamilia, CodigoPatente)
SELECT fp.CodigoFamilia, f.CodigoPatente
FROM dbo.FamiliaPatente fp
CROSS JOIN @Familias f
WHERE fp.CodigoPatente = 'P04'
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.FamiliaPatente x
      WHERE x.CodigoFamilia = fp.CodigoFamilia
        AND x.CodigoPatente = f.CodigoPatente
  );

INSERT INTO dbo.PerfilPatente (CodigoPerfil, CodigoPatente)
SELECT p.CodigoPerfil, np.CodigoPatente
FROM dbo.Perfiles p
CROSS JOIN @Patentes np
WHERE p.Nombre = 'Administrador'
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.PerfilPatente x
      WHERE x.CodigoPerfil = p.CodigoPerfil
        AND x.CodigoPatente = np.CodigoPatente
  );

INSERT INTO dbo.PerfilPatente (CodigoPerfil, CodigoPatente)
SELECT p.CodigoPerfil, sp.CodigoPatente
FROM dbo.Perfiles p
CROSS JOIN (VALUES ('P05'), ('P09')) sp(CodigoPatente)
WHERE p.Activo = 1
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.PerfilPatente x
      WHERE x.CodigoPerfil = p.CodigoPerfil
        AND x.CodigoPatente = sp.CodigoPatente
  );

DELETE FROM dbo.PerfilPatente
WHERE CodigoPatente IN ('P01', 'P03', 'P04', 'P07', 'P08');

DELETE FROM dbo.FamiliaPatente
WHERE CodigoPatente IN ('P01', 'P03', 'P04', 'P07', 'P08');

DELETE FROM dbo.Patentes
WHERE CodigoPatente IN ('P01', 'P03', 'P04', 'P07', 'P08');

COMMIT TRANSACTION;
