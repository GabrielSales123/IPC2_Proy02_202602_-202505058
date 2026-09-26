# Sistema de Gestión de Catálogo de Libros

## Introducción

Proyecto desarrollado para el curso **Introducción a la Programación y Computación 2 (IPC2)** de la Universidad de San Carlos de Guatemala.

El sistema permite administrar un catálogo de libros organizado mediante una estructura jerárquica de categorías y subcategorías. La solución está diseñada para realizar búsquedas eficientes mediante estructuras de datos desarrolladas específicamente para el proyecto.

## Descripción

El sistema simula la organización de una biblioteca, donde las categorías principales pueden contener subcategorías y estas pueden contener otras divisiones internas. Cada categoría puede estar asociada con un conjunto de libros.

Cada libro posee:

* ISBN único.
* Título.
* Autor.
* Categoría a la que pertenece.

El proyecto debe permitir trabajar con una cantidad considerable de libros y mantener tiempos de búsqueda reducidos incluso cuando el catálogo aumenta de tamaño.

## Funcionalidades

### Gestión del catálogo

* Inicializar el sistema sin información previa.
* Cargar información desde archivos XML.
* Agregar categorías.
* Mostrar la estructura de categorías y subcategorías.
* Visualizar la estructura a partir de una categoría o subcategoría específica.
* Mostrar gráficamente los libros pertenecientes a una categoría o subcategoría mediante Graphviz.

### Gestión de libros

* Registrar nuevos libros.
* Buscar un libro mediante su ISBN.
* Eliminar libros.
* Obtener el libro con el ISBN menor.
* Obtener el libro con el ISBN mayor.
* Mostrar los libros ordenados ascendentemente por ISBN.

### Validaciones

El sistema debe validar, entre otros aspectos:

* ISBN duplicados.
* Categorías inexistentes.
* Categorías duplicadas.
* Relaciones entre categorías y padres.
* Datos inválidos provenientes de archivos XML.

Los archivos XML son incrementales, por lo que pueden cargarse varios archivos durante la ejecución del sistema.

## Estructura del catálogo

La organización del catálogo sigue una estructura jerárquica:

```text
Catálogo
├── Categoría
│   ├── Subcategoría
│   │   ├── Subcategoría
│   │   └── Libros
│   └── Libros
└── Categoría
    └── ...
```

Los nombres de las categorías son únicos dentro de toda la estructura del catálogo.

## Archivos XML

El sistema permite cargar archivos XML con la configuración inicial del catálogo.

La estructura general utilizada es:

```xml
<?xml version="1.0"?>
<config>
    <listaCategorias>
        <categoria>Catalogo</categoria>
        <categoria padre="Catalogo">Ciencias</categoria>
        <categoria padre="Ciencias">Fisica</categoria>
    </listaCategorias>

    <listaLibros>
        <libro>
            <ISBN>100</ISBN>
            <titulo>Fisica General</titulo>
            <autor>Marie Curie</autor>
            <categoria>Fisica</categoria>
        </libro>
    </listaLibros>
</config>
```

Los elementos `listaCategorias` y `listaLibros` pueden ser opcionales, debido a que los archivos de entrada son incrementales.

## Estructuras de datos

El proyecto utiliza **Tipos de Datos Abstractos (TDA) desarrollados por el estudiante**, evitando el uso de estructuras de datos predefinidas de C# como:

* `List`
* `LinkedList`
* `Queue`
* `Stack`
* `Templates`

Las estructuras implementadas permiten representar la jerarquía de categorías y realizar las operaciones necesarias sobre el catálogo.

## Tecnologías

* **C#**
* **.NET**
* **Programación Orientada a Objetos**
* **XML**
* **Graphviz**
* **HTML / CSS**
* **Interfaz web**
* **Git / GitHub**

> Las tecnologías y herramientas anteriores corresponden a los requerimientos y enfoque establecidos para el proyecto. La sección será ajustada para reflejar exactamente las tecnologías presentes en la implementación actual del repositorio.

## Arquitectura

El proyecto utiliza una separación de responsabilidades para organizar la aplicación.

```text
┌───────────────────────────┐
│       Interfaz Web        │
│       HTML / CSS          │
└─────────────┬─────────────┘
              │
              ▼
┌───────────────────────────┐
│        Controladores      │
│     Lógica de aplicación  │
└─────────────┬─────────────┘
              │
              ▼
┌───────────────────────────┐
│          Modelos          │
│ Libros / Catálogo /       │
│ Categorías / Estructuras  │
└─────────────┬─────────────┘
              │
              ▼
┌───────────────────────────┐
│     Estructuras TDA       │
│ Árboles / estructuras     │
│ desarrolladas             │
└───────────────────────────┘
```

La arquitectura definitiva se documentará de acuerdo con la estructura existente en el repositorio.

## Graphviz

Graphviz se utiliza para generar representaciones gráficas de la información del catálogo.

El sistema permite visualizar los libros asociados a una categoría o subcategoría, respetando el orden ascendente por ISBN.

## Casos de validación

El proyecto contempla diferentes escenarios para verificar el comportamiento del sistema.

Entre ellos:

| Caso                            | Resultado esperado       |
| ------------------------------- | ------------------------ |
| Categoría nueva                 | Se agrega al catálogo    |
| Categoría duplicada             | Se rechaza               |
| Categoría con padre existente   | Se establece la relación |
| Categoría con padre inexistente | Se rechaza               |
| Libro con ISBN nuevo            | Se agrega                |
| Libro con ISBN duplicado        | Se rechaza               |
| Libro con categoría inexistente | Se rechaza               |
| ISBN no válido                  | Se rechaza               |

También se contempla el procesamiento de categorías declaradas antes que sus respectivos padres mediante un mecanismo de vinculación posterior.

## Requisitos

Para ejecutar el proyecto se requiere contar con:

* Sistema operativo compatible con .NET.
* SDK de .NET utilizado por el proyecto.
* IDE compatible con el desarrollo del proyecto.
* Graphviz instalado y disponible para la generación de gráficos.
* Navegador web para acceder a la interfaz.

> Las versiones exactas se indicarán de acuerdo con la configuración actual del repositorio.

## Instalación

Clonar el repositorio:

```bash
git clone https://github.com/GabrielSales123/IPC2_Proy02_202602_-202505058.git
```

Ingresar al directorio:

```bash
cd IPC2_Proy02_202602_-202505058
```

Restaurar las dependencias:

```bash
dotnet restore
```

Compilar el proyecto:

```bash
dotnet build
```

Ejecutar:

```bash
dotnet run
```

## Uso

1. Iniciar la aplicación.
2. Inicializar el catálogo.
3. Cargar un archivo XML de entrada.
4. Consultar la estructura de categorías.
5. Registrar o eliminar libros.
6. Buscar libros mediante su ISBN.
7. Consultar el libro con menor o mayor ISBN.
8. Visualizar los libros asociados a una categoría.
9. Generar las representaciones gráficas mediante Graphviz.

## Documentación

La documentación del proyecto se encuentra dentro del repositorio en la carpeta correspondiente.

Esta documentación incluye el diseño y modelado de la solución, incluyendo el diagrama de clases requerido para el proyecto.

## Versionamiento

El desarrollo del proyecto se administra mediante Git y GitHub.

Se deben mantener como mínimo cuatro releases que permitan evidenciar el avance progresivo del proyecto durante su desarrollo.

## Información académica

**Universidad de San Carlos de Guatemala**

**Facultad de Ingeniería**

**Escuela de Ciencias y Sistemas**

**Curso:** Introducción a la Programación y Computación 2

**Proyecto:** Proyecto No. 2

**Estudiante:** Gabriel Sales

**Carné:** 202505058




