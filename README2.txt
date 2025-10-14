---------- REQUISITOS PREVIOS --------------

- Visual Studio 2022 Community o superior
- .NET 8.0 SDK
- SQL Server 2019 o superior (Express es suficiente)
- SQL Server Management Studio (SSMS) - recomendado

------------- CONFIGURACIÓN INICIAL ---------------
- Paso 1: Copiar el Proyecto

Copia la carpeta completa CAASD.Solution a tu PC
Abre CAASD.Solution.sln con Visual Studio

- Paso 2: Configurar Connection String (USEN SQL EXPRESS)
IMPORTANTE: Antes de compilar o ejecutar cualquier cosa.

Abre SQL Server Management Studio (SSMS).

Conéctate y anota el nombre de tu servidor. Ejemplos:

  - Vlocalhost\SQLEXPRESS
  - localhost
  - TU_PC\SQLEXPRESS
  - . o (local)

Ve al proyecto CAASD.Integracion.

Abre el archivo appsettings.json.

Cambia la línea del CAASDConnection:
json{
  "ConnectionStrings": {
    "CAASDConnection": "Server=TU_SERVIDOR_AQUI;Database=CAASD_DB;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }

---------  Paso 3: Crear la Base de Datos -----------------

En Visual Studio: Tools → NuGet Package Manager → Package Manager Console.

Asegúrate que "Default project" esté en CAASD.Integracion.

Ejecuta:


    powershell   Update-Database

Verifica en SSMS que se creó la base de datos CAASD_DB con 10 tablas

------------ CÓMO INTEGRAR TU CAPA (WEB o CAJA) -------------
1. Crear el Proyecto
Clic derecho en la Solution → Add → New Project

Tipo: ASP.NET Web Application (.NET Framework)

Nombre: CAASD.Web

Framework: .NET Framework .NET 8.0 

Template: ESCOGE

2. Agregar Referencias

Clic derecho en CAASD.Web o caja → Add → Project Reference
Marcar:
  CAASD.Core
  CAASD.Integracion