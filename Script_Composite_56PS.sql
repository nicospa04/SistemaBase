-- ==========================================================
-- SCRIPT SQL - Sistema Base - Patrón Composite
-- ==========================================================

-- ==========================================================
-- 1. TABLAS DEL COMPOSITE (Perfiles, Familias, Patentes)
-- ==========================================================

-- Tabla de Perfiles (raíz del composite)
CREATE TABLE Perfiles (
    CodigoPerfil VARCHAR(10) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1
);

-- Tabla de Familias (nodo composite)
CREATE TABLE Familias (
    CodigoFamilia VARCHAR(10) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Activa BIT NOT NULL DEFAULT 1
);

-- Tabla de Patentes (hoja del composite)
CREATE TABLE Patentes (
    CodigoPatente VARCHAR(10) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL
);

-- Relación Perfil → Patente
CREATE TABLE PerfilPatente (
    CodigoPerfil VARCHAR(10),
    CodigoPatente VARCHAR(10),
    PRIMARY KEY (CodigoPerfil, CodigoPatente),
    FOREIGN KEY (CodigoPerfil) REFERENCES Perfiles(CodigoPerfil),
    FOREIGN KEY (CodigoPatente) REFERENCES Patentes(CodigoPatente)
);

-- Relación Perfil → Familia
CREATE TABLE PerfilFamilia (
    CodigoPerfil VARCHAR(10),
    CodigoFamilia VARCHAR(10),
    PRIMARY KEY (CodigoPerfil, CodigoFamilia),
    FOREIGN KEY (CodigoPerfil) REFERENCES Perfiles(CodigoPerfil),
    FOREIGN KEY (CodigoFamilia) REFERENCES Familias(CodigoFamilia)
);

-- Relación Familia → Patente
CREATE TABLE FamiliaPatente (
    CodigoFamilia VARCHAR(10),
    CodigoPatente VARCHAR(10),
    PRIMARY KEY (CodigoFamilia, CodigoPatente),
    FOREIGN KEY (CodigoFamilia) REFERENCES Familias(CodigoFamilia),
    FOREIGN KEY (CodigoPatente) REFERENCES Patentes(CodigoPatente)
);

-- Relación Familia → Familia (recursiva)
CREATE TABLE FamiliaFamilia (
    CodigoFamilia VARCHAR(10),
    CodFamiliaHija VARCHAR(10),
    PRIMARY KEY (CodigoFamilia, CodFamiliaHija),
    FOREIGN KEY (CodigoFamilia) REFERENCES Familias(CodigoFamilia),
    FOREIGN KEY (CodFamiliaHija) REFERENCES Familias(CodigoFamilia)
);

-- ==========================================================
-- 2. PATENTES INICIALES (módulos de SistemaBase)
-- ==========================================================

INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P01', 'Gestión de Usuarios');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P02', 'Consultar Bitácora de Eventos');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P03', 'Gestión de Perfiles');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P04', 'Gestión de Familias');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P05', 'Cambiar Contraseña');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P06', 'Exportar PDF');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P07', 'Iniciar Sesión');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P08', 'Cerrar Sesión');
INSERT INTO Patentes (CodigoPatente, Nombre) VALUES ('P09', 'Cambiar Idioma');
