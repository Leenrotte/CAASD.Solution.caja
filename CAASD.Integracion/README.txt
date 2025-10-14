====================================================
  CONFIGURACIÓN DEL PROYECTO CAASD - INTEGRACIÓN
====================================================

IMPORTANTE: ANTES DE COMPILAR O EJECUTAR

1. Abrir SQL Server Management Studio (SSMS)
2. Conectarte y ver el nombre de tu servidor
   Ejemplos:
   - localhost\SQLEXPRESS
   - localhost
   - TU_PC\SQLEXPRESS

3. Abrir el archivo: CAASD.Integracion\appsettings.json

4. Cambiar esta línea:
   "CAASDConnection": "CONFIGURAR_AQUI_TU_SERVIDOR"

   Por tu servidor, ejemplo:
   "CAASDConnection": "Server=localhost\\SQLEXPRESS;Database=CAASD_DB;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True"

5. En Package Manager Console ejecutar:
   Update-Database

6. ¡Listo! Ya puedes trabajar.

====================================================