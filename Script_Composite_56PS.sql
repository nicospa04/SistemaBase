-- ==========================================================
-- SCRIPT SQL - Sistema Base - Patron Composite
-- ==========================================================

-- ==========================================================
-- 1. TABLAS DEL COMPOSITE (Perfiles, Familias, Patentes)
-- ==========================================================

CREATE TABLE Perfiles (
    CodigoPerfil VARCHAR(10) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1
);

CREATE TABLE Familias (
    CodigoFamilia VARCHAR(10) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Activa BIT NOT NULL DEFAULT 1
);

CREATE TABLE Patentes (
    CodigoPatente VARCHAR(10) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL
);

CREATE TABLE PerfilPatente (
    CodigoPerfil VARCHAR(10),
    CodigoPatente VARCHAR(10),
    PRIMARY KEY (CodigoPerfil, CodigoPatente),
    FOREIGN KEY (CodigoPerfil) REFERENCES Perfiles(CodigoPerfil),
    FOREIGN KEY (CodigoPatente) REFERENCES Patentes(CodigoPatente)
);

CREATE TABLE PerfilFamilia (
    CodigoPerfil VARCHAR(10),
    CodigoFamilia VARCHAR(10),
    PRIMARY KEY (CodigoPerfil, CodigoFamilia),
    FOREIGN KEY (CodigoPerfil) REFERENCES Perfiles(CodigoPerfil),
    FOREIGN KEY (CodigoFamilia) REFERENCES Familias(CodigoFamilia)
);

CREATE TABLE FamiliaPatente (
    CodigoFamilia VARCHAR(10),
    CodigoPatente VARCHAR(10),
    PRIMARY KEY (CodigoFamilia, CodigoPatente),
    FOREIGN KEY (CodigoFamilia) REFERENCES Familias(CodigoFamilia),
    FOREIGN KEY (CodigoPatente) REFERENCES Patentes(CodigoPatente)
);

CREATE TABLE FamiliaFamilia (
    CodigoFamilia VARCHAR(10),
    CodFamiliaHija VARCHAR(10),
    PRIMARY KEY (CodigoFamilia, CodFamiliaHija),
    FOREIGN KEY (CodigoFamilia) REFERENCES Familias(CodigoFamilia),
    FOREIGN KEY (CodFamiliaHija) REFERENCES Familias(CodigoFamilia)
);

CREATE TABLE DigitoVerificador (
    nombreTabla VARCHAR(128) PRIMARY KEY,
    DVH VARCHAR(64) NOT NULL,
    DVV VARCHAR(64) NOT NULL
);

-- ==========================================================
-- 2. PATENTES INICIALES POR ACCION
-- ==========================================================

INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P02', 'Consultar bitacora de eventos');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P05', 'Cambiar contrasena');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P06', 'Exportar bitacora a PDF');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P09', 'Cambiar idioma');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P10', 'Consultar usuarios');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P11', 'Crear usuario');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P12', 'Modificar usuario');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P13', 'Activar o desactivar usuario');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P14', 'Desbloquear usuario');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P20', 'Consultar perfiles');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P21', 'Crear perfil');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P22', 'Eliminar perfil');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P23', 'Asignar patente a perfil');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P24', 'Asignar familia a perfil');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P25', 'Quitar permiso de perfil');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P30', 'Consultar familias');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P31', 'Crear familia');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P32', 'Modificar familia');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P33', 'Eliminar familia');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P34', 'Asignar patente a familia');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P35', 'Asignar familia a familia');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P36', 'Quitar permiso de familia');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P50', 'Realizar backup');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P51', 'Restaurar backup');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P52', 'Recalcular digitos verificadores');
