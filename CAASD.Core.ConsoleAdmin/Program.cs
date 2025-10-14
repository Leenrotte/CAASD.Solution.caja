using CAASD.Core.Entities;
using CAASD.Core.Helpers;
using CAASD.Core.Interfaces;
using CAASD.Integracion.UnitOfWork;
using System;
using System.Diagnostics.Contracts;
using CAASD.Core.Constants;
using CAASD.Core.DTOs;
using CAASD.Integracion.Services;


namespace CAASD.ConsoleAdmin
{
    class Program
    {
        private static IUnitOfWork _unitOfWork;

        private static AuthService _auth;


        static void Main(string[] args)
        {
            Console.Title = "CAASD - Panel de Administración";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═══════════════════════════════════════════════╗");
            Console.WriteLine("║   CAASD - SISTEMA DE GESTIÓN DE AGUA          ║");
            Console.WriteLine("║        Panel de Administración                ║");
            Console.WriteLine("╚═══════════════════════════════════════════════╝");
            Console.ResetColor();

            _unitOfWork = new UnitOfWork();
            _unitOfWork = new UnitOfWork();
            _auth = new AuthService(_unitOfWork);

            // === LOGIN SOLO PARA CORE (Superadmin) ===
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Inicio de sesión - Acceso privado");
            Console.ResetColor();

            Console.Write("\nEmail: ");
            var emailLogin = Console.ReadLine();
            Console.Write("Password: ");
            var passLogin = ReadPassword();

            var login = _auth.Login(new LoginRequestDTO
            {
                Email = emailLogin ?? string.Empty,
                Password = passLogin ?? string.Empty,
                ModuloDestino = AppConstants.MOD_CORE
            });

            if (!login.Success || login.Usuario == null || login.Usuario.Rol != AppConstants.ROL_SUPERADMIN)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nAcceso denegado: {login.Mensaje}");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nBienvenido {login.Usuario.NombreCompleto} (Superadmin)");
            Console.ResetColor();


            bool continuar = true;
            while (continuar)
            {
                MostrarMenuPrincipal();
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        GestionarUsuarios();
                        break;
                    case "2":
                        GestionarFacturas();
                        break;
                    case "3":
                        GestionarPagos();
                        break;
                    case "4":
                        GestionarAverias();
                        break;
                    case "5":
                        GenerarReportes();
                        break;
                    case "6":
                        ConfiguracionSistema();
                        break;
                    case "0":
                        continuar = false;
                        break;
                    default:
                        Console.WriteLine("Opción inválida");
                        break;
                }

                if (continuar)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }

            Console.WriteLine("\n¡Hasta luego!");
        }

        static void MostrarMenuPrincipal()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
            Console.WriteLine("║              MENÚ PRINCIPAL                   ║");
            Console.WriteLine("╚═══════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine("\n[1] Gestión de Usuarios");
            Console.WriteLine("[2] Gestión de Facturas");
            Console.WriteLine("[3] Gestión de Pagos");
            Console.WriteLine("[4] Gestión de Averías");
            Console.WriteLine("[5] Reportes y Estadísticas");
            Console.WriteLine("[6] Configuración del Sistema");
            Console.WriteLine("[0] Salir");
            Console.Write("\nSeleccione una opción: ");
        }
        static string ReadPassword()
        {
            var pwd = string.Empty;
            ConsoleKey key;
            do
            {
                var ki = Console.ReadKey(true);
                key = ki.Key;
                if (key == ConsoleKey.Backspace && pwd.Length > 0)
                {
                    pwd = pwd[..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(ki.KeyChar))
                {
                    pwd += ki.KeyChar;
                    Console.Write("*");
                }
            } while (key != ConsoleKey.Enter);
            Console.WriteLine();
            return pwd;
        }


        // ============================================
        // GESTIÓN DE USUARIOS
        // ============================================
        static void GestionarUsuarios()
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════╗");
            Console.WriteLine("║          GESTIÓN DE USUARIOS                  ║");
            Console.WriteLine("╚═══════════════════════════════════════════════╝");
            Console.WriteLine("\n[1] Listar todos los usuarios");
            Console.WriteLine("[2] Crear nuevo usuario");
            Console.WriteLine("[3] Buscar usuario");
            Console.WriteLine("[4] Activar/Desactivar usuario");
            Console.WriteLine("[5] Cambiar rol de usuario");
            Console.WriteLine("[0] Volver");
            Console.Write("\nOpción: ");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    ListarUsuarios();
                    break;
                case "2":
                    CrearUsuario();
                    break;
                case "3":
                    BuscarUsuario();
                    break;
                case "4":
                    ActivarDesactivarUsuario();
                    break;
                case "5":
                    CambiarRolUsuario();
                    break;
            }
        }

        static void ListarUsuarios()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine("LISTADO DE USUARIOS");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");

            var usuarios = _unitOfWork.Usuarios.GetAll();

            Console.WriteLine($"{"ID",-5} {"CÉDULA",-15} {"NOMBRE",-30} {"EMAIL",-30} {"ROL",-15} {"ESTADO",-10}");
            Console.WriteLine(new string('─', 105));

            foreach (var usuario in usuarios)
            {
                string estado = usuario.Activo ? "Activo" : "Inactivo";
                Console.WriteLine($"{usuario.Id,-5} {usuario.Cedula,-15} {usuario.Nombre + " " + usuario.Apellido,-30} {usuario.Email,-30} {usuario.Rol?.Nombre ?? "N/A",-15} {estado,-10}");
            }

            Console.WriteLine($"\nTotal de usuarios: {usuarios.Count()}");
        }

        static void CrearUsuario()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("CREAR NUEVO USUARIO");
            Console.WriteLine("═══════════════════════════════════════════════");

            Console.Write("\nCédula: ");
            string cedula = Console.ReadLine();

            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();

            Console.Write("Apellido: ");
            string apellido = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Teléfono: ");
            string telefono = Console.ReadLine();

            Console.Write("Dirección: ");
            string direccion = Console.ReadLine();

            Console.Write("Contraseña: ");
            string password = Console.ReadLine();

            Console.Write("Rol (2=Admin, 4=Cajero): ");
            if (!int.TryParse(Console.ReadLine(), out int rolId) ||
                (rolId != 2 && rolId != 4))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n✗ Rol inválido. Solo 2 (Admin) o 4 (Cajero).");
                Console.ResetColor();
                return;
            }


            var usuario = new Usuario
            {
                Cedula = cedula,
                Nombre = nombre,
                Apellido = apellido,
                Email = email,
                Telefono = telefono,
                DireccionCompleta = direccion,
                PasswordHash = SecurityHelper.HashPassword(password),
                FechaRegistro = DateTime.Now,
                Activo = true,
                RolId = rolId
            };

            try
            {
                _unitOfWork.Usuarios.Add(usuario);
                _unitOfWork.SaveChanges();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n✓ Usuario creado exitosamente");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n✗ Error: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void BuscarUsuario()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("BUSCAR USUARIO");
            Console.WriteLine("═══════════════════════════════════════════════");

            Console.Write("\nBuscar por (1=Email, 2=Cédula): ");
            string tipo = Console.ReadLine();

            Usuario usuario = null;

            if (tipo == "1")
            {
                Console.Write("Email: ");
                string email = Console.ReadLine();
                usuario = _unitOfWork.Usuarios.GetByEmail(email);
            }
            else
            {
                Console.Write("Cédula: ");
                string cedula = Console.ReadLine();
                usuario = _unitOfWork.Usuarios.GetByCedula(cedula);
            }

            if (usuario != null)
            {
                Console.WriteLine("\n─────────────────────────────────────────");
                Console.WriteLine($"ID:        {usuario.Id}");
                Console.WriteLine($"Cédula:    {usuario.Cedula}");
                Console.WriteLine($"Nombre:    {usuario.Nombre} {usuario.Apellido}");
                Console.WriteLine($"Email:     {usuario.Email}");
                Console.WriteLine($"Teléfono:  {usuario.Telefono}");
                Console.WriteLine($"Dirección: {usuario.DireccionCompleta}");
                Console.WriteLine($"Rol:       {usuario.Rol?.Nombre ?? "Sin rol"}");
                Console.WriteLine($"Estado:    {(usuario.Activo ? "Activo" : "Inactivo")}");
                Console.WriteLine($"Registro:  {usuario.FechaRegistro:dd/MM/yyyy}");
                Console.WriteLine("─────────────────────────────────────────");
            }
            else
            {
                Console.WriteLine("\n✗ Usuario no encontrado");
            }
        }

        static void ActivarDesactivarUsuario()
        {
            Console.Clear();
            Console.Write("ID del usuario: ");
            int id = int.Parse(Console.ReadLine());

            var usuario = _unitOfWork.Usuarios.GetById(id);

            if (usuario != null)
            {
                usuario.Activo = !usuario.Activo;
                _unitOfWork.Usuarios.Update(usuario);
                _unitOfWork.SaveChanges();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✓ Usuario {(usuario.Activo ? "activado" : "desactivado")} exitosamente");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("\n✗ Usuario no encontrado");
            }
        }

        static void CambiarRolUsuario()
        {
            Console.Clear();
            Console.Write("ID del usuario: ");
            int id = int.Parse(Console.ReadLine());

            var usuario = _unitOfWork.Usuarios.GetById(id);

            if (usuario != null)
            {
                Console.WriteLine($"\nRol actual: {usuario.Rol?.Nombre}");
                Console.Write("Nuevo rol (2=Admin, 3=Cliente): ");
                int nuevoRolId = int.Parse(Console.ReadLine());

                usuario.RolId = nuevoRolId;
                _unitOfWork.Usuarios.Update(usuario);
                _unitOfWork.SaveChanges();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n✓ Rol actualizado exitosamente");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("\n✗ Usuario no encontrado");
            }
        }

        // ============================================
        // GESTIÓN DE FACTURAS
        // ============================================
        static void GestionarFacturas()
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════╗");
            Console.WriteLine("║          GESTIÓN DE FACTURAS                  ║");
            Console.WriteLine("╚═══════════════════════════════════════════════╝");
            Console.WriteLine("\n[1] Crear factura para usuario");
            Console.WriteLine("[2] Listar facturas pendientes");
            Console.WriteLine("[3] Buscar factura");
            Console.WriteLine("[4] Eliminar factura");
            Console.WriteLine("[0] Volver");
            Console.Write("\nOpción: ");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    CrearFactura();
                    break;
                case "2":
                    ListarFacturasPendientes();
                    break;
                case "3":
                    BuscarFactura();
                    break;
                case "4":
                    EliminarFactura();
                    break;
            }
        }

        static void CrearFactura()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("CREAR NUEVA FACTURA");
            Console.WriteLine("═══════════════════════════════════════════════");

            Console.Write("\nID del Usuario: ");
            int usuarioId = int.Parse(Console.ReadLine());

            // Buscar o crear contrato
            var contratos = _unitOfWork.Contratos.Find(c => c.UsuarioId == usuarioId && c.Activo);

            int contratoId;
            if (!contratos.Any())
            {
                Console.WriteLine("El usuario no tiene contratos. Creando uno automáticamente...");
                var usuario = _unitOfWork.Usuarios.GetById(usuarioId);

                var nuevoContrato = new Contrato
                {
                    NumeroContrato = $"CTR-{DateTime.Now:yyyyMMdd}-{usuarioId}",
                    UsuarioId = usuarioId,
                    Direccion = usuario.DireccionCompleta,
                    Sector = "Zona Central",
                    Latitud = 18.4861m,
                    Longitud = -69.9312m,
                    FechaInicio = DateTime.Now,
                    Activo = true
                };

                _unitOfWork.Contratos.Add(nuevoContrato);
                _unitOfWork.SaveChanges();
                contratoId = nuevoContrato.Id;
                Console.WriteLine($"✓ Contrato {nuevoContrato.NumeroContrato} creado");
            }
            else
            {
                contratoId = contratos.First().Id;
            }

            Console.Write("Metros cúbicos consumidos: ");
            int metrosConsumidos = int.Parse(Console.ReadLine());

            // Calcular montos
            decimal montoAgua = metrosConsumidos * 15m;
            decimal montoAlcantarillado = montoAgua * 0.30m;
            decimal montoBasura = 150m;
            decimal subtotal = montoAgua + montoAlcantarillado + montoBasura;
            decimal itbis = subtotal * 0.18m;
            decimal montoTotal = subtotal + itbis;

            var factura = new Factura
            {
                NumeroFactura = $"F-{DateTime.Now:yyyyMM}-{contratoId}-{new Random().Next(1000, 9999)}",
                ContratoId = contratoId,
                FechaEmision = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddDays(15),
                MetrosConsumidos = metrosConsumidos,
                MontoAgua = montoAgua,
                MontoAlcantarillado = montoAlcantarillado,
                MontoBasura = montoBasura,
                Itbis = itbis,
                MontoTotal = montoTotal,
                EstadoPago = "Pendiente"
            };

            try
            {
                _unitOfWork.Facturas.Add(factura);
                _unitOfWork.SaveChanges();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n✓ Factura creada exitosamente");
                Console.WriteLine($"\nNúmero: {factura.NumeroFactura}");
                Console.WriteLine($"Consumo: {metrosConsumidos} m³");
                Console.WriteLine($"Total: RD${montoTotal:N2}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n✗ Error: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void ListarFacturasPendientes()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine("FACTURAS PENDIENTES");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");

            var facturas = _unitOfWork.Facturas.GetAll()
                .Where(f => f.EstadoPago == "Pendiente")
                .OrderByDescending(f => f.FechaEmision);

            Console.WriteLine($"{"ID",-5} {"NÚMERO",-20} {"CONTRATO",-20} {"CONSUMO",-10} {"TOTAL",-15} {"VENCIMIENTO",-15}");
            Console.WriteLine(new string('─', 85));

            foreach (var factura in facturas)
            {
                Console.WriteLine($"{factura.Id,-5} {factura.NumeroFactura,-20} {factura.Contrato?.NumeroContrato ?? "N/A",-20} {factura.MetrosConsumidos + " m³",-10} {"RD$" + factura.MontoTotal.ToString("N2"),-15} {factura.FechaVencimiento:dd/MM/yyyy,-15}");
            }

            Console.WriteLine($"\nTotal pendientes: {facturas.Count()}");
        }

        static void BuscarFactura()
        {
            Console.Clear();
            Console.Write("Número de factura: ");
            string numero = Console.ReadLine();

            var factura = _unitOfWork.Facturas.GetByNumeroFactura(numero);

            if (factura != null)
            {
                Console.WriteLine("\n─────────────────────────────────────────");
                Console.WriteLine($"Número:      {factura.NumeroFactura}");
                Console.WriteLine($"Contrato:    {factura.Contrato?.NumeroContrato}");
                Console.WriteLine($"Emisión:     {factura.FechaEmision:dd/MM/yyyy}");
                Console.WriteLine($"Vencimiento: {factura.FechaVencimiento:dd/MM/yyyy}");
                Console.WriteLine($"Consumo:     {factura.MetrosConsumidos} m³");
                Console.WriteLine($"Total:       RD${factura.MontoTotal:N2}");
                Console.WriteLine($"Estado:      {factura.EstadoPago}");
                Console.WriteLine("─────────────────────────────────────────");
            }
            else
            {
                Console.WriteLine("\n✗ Factura no encontrada");
            }
        }

        static void EliminarFactura()
        {
            Console.Clear();
            Console.Write("ID de la factura: ");
            int id = int.Parse(Console.ReadLine());

            var factura = _unitOfWork.Facturas.GetById(id);

            if (factura != null)
            {
                if (factura.EstadoPago == "Pagada")
                {
                    Console.WriteLine("\n✗ No se puede eliminar una factura pagada");
                    return;
                }

                Console.Write($"¿Confirma eliminar la factura {factura.NumeroFactura}? (S/N): ");
                string confirm = Console.ReadLine();

                if (confirm.ToUpper() == "S")
                {
                    _unitOfWork.Facturas.Delete(id);
                    _unitOfWork.SaveChanges();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n✓ Factura eliminada exitosamente");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.WriteLine("\n✗ Factura no encontrada");
            }
        }

        // ============================================
        // GESTIÓN DE PAGOS
        // ============================================
        static void GestionarPagos()
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════╗");
            Console.WriteLine("║           GESTIÓN DE PAGOS                    ║");
            Console.WriteLine("╚═══════════════════════════════════════════════╝");
            Console.WriteLine("\n[1] Ver pagos del día");
            Console.WriteLine("[2] Buscar pago");
            Console.WriteLine("[3] Estadísticas de pagos");
            Console.WriteLine("[0] Volver");
            Console.Write("\nOpción: ");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    VerPagosDelDia();
                    break;
                case "2":
                    BuscarPago();
                    break;
                case "3":
                    EstadisticasPagos();
                    break;
            }
        }

        static void VerPagosDelDia()
        {
            Console.Clear();
            var hoy = DateTime.Now.Date;
            var pagos = _unitOfWork.Pagos.GetAll()
                .Where(p => p.FechaPago >= hoy && p.FechaPago < hoy.AddDays(1))
                .OrderByDescending(p => p.FechaPago);

            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"PAGOS DEL DÍA - {hoy:dd/MM/yyyy}");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");

            Console.WriteLine($"{"HORA",-10} {"FACTURA",-20} {"MÉTODO",-20} {"MONTO",-15}");
            Console.WriteLine(new string('─', 65));

            decimal totalDia = 0;
            foreach (var pago in pagos)
            {
                Console.WriteLine($"{pago.FechaPago:HH:mm:ss,-10} {pago.Factura?.NumeroFactura ?? "N/A",-20} {pago.MetodoPago,-20} {"RD$" + pago.Monto.ToString("N2"),-15}");
                totalDia += pago.Monto;
            }

            Console.WriteLine(new string('═', 65));
            Console.WriteLine($"TOTAL: RD${totalDia:N2}");
            Console.WriteLine($"CANTIDAD: {pagos.Count()} transacciones");
        }

        static void BuscarPago()
        {
            Console.Clear();
            Console.Write("Número de transacción: ");
            string numero = Console.ReadLine();

            var pago = _unitOfWork.Pagos.GetByNumeroTransaccion(numero);

            if (pago != null)
            {
                Console.WriteLine("\n─────────────────────────────────────────");
                Console.WriteLine($"Transacción: {pago.NumeroTransaccion}");
                Console.WriteLine($"Factura:     {pago.Factura?.NumeroFactura}");
                Console.WriteLine($"Fecha:       {pago.FechaPago:dd/MM/yyyy HH:mm:ss}");
                Console.WriteLine($"Método:      {pago.MetodoPago}");
                Console.WriteLine($"Monto:       RD${pago.Monto:N2}");
                Console.WriteLine($"Estado:      {pago.Estado}");
                Console.WriteLine("─────────────────────────────────────────");
            }
            else
            {
                Console.WriteLine("\n✗ Pago no encontrado");
            }
        }

        static void EstadisticasPagos()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("ESTADÍSTICAS DE PAGOS");
            Console.WriteLine("═══════════════════════════════════════════════");

            var todosPagos = _unitOfWork.Pagos.GetAll().ToList();

            var pagosPorMetodo = todosPagos
                .GroupBy(p => p.MetodoPago)
                .Select(g => new
                {
                    Metodo = g.Key,
                    Cantidad = g.Count(),
                    Total = g.Sum(p => p.Monto)
                });

            Console.WriteLine("\nPOR MÉTODO DE PAGO:");
            Console.WriteLine($"{"MÉTODO",-20} {"CANTIDAD",-15} {"TOTAL",-15}");
            Console.WriteLine(new string('─', 50));

            foreach (var grupo in pagosPorMetodo)
            {
                Console.WriteLine($"{grupo.Metodo,-20} {grupo.Cantidad,-15} {"RD$" + grupo.Total.ToString("N2"),-15}");
            }

            Console.WriteLine("\n═══════════════════════════════════════════════");
            Console.WriteLine($"TOTAL GENERAL: RD${todosPagos.Sum(p => p.Monto):N2}");
            Console.WriteLine($"TRANSACCIONES: {todosPagos.Count}");
        }

        // ============================================
        // GESTIÓN DE AVERÍAS
        // ============================================
        static void GestionarAverias()
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════╗");
            Console.WriteLine("║         GESTIÓN DE AVERÍAS                    ║");
            Console.WriteLine("╚═══════════════════════════════════════════════╝");
            Console.WriteLine("\n[1] Listar averías pendientes");
            Console.WriteLine("[2] Cambiar estado de avería");
            Console.WriteLine("[3] Ver estadísticas");
            Console.WriteLine("[0] Volver");
            Console.Write("\nOpción: ");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    ListarAveriasPendientes();
                    break;
                case "2":
                    CambiarEstadoAveria();
                    break;
                case "3":
                    EstadisticasAverias();
                    break;
            }
        }

        static void ListarAveriasPendientes()
        {
            Console.Clear();
            var averias = _unitOfWork.ReportesAveria.GetAveriasByEstado("Reportada");

            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine("AVERÍAS REPORTADAS");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");

            Console.WriteLine($"{"ID",-5} {"TIPO",-20} {"DIRECCIÓN",-30} {"FECHA",-15} {"PRIORIDAD",-15}");
            Console.WriteLine(new string('─', 85));

            foreach (var averia in averias)
            {
                Console.WriteLine($"{averia.Id,-5} {averia.TipoAveria,-20} {averia.Direccion,-30} {averia.FechaReporte:dd/MM/yyyy,-15} {averia.Prioridad,-15}");
            }

            Console.WriteLine($"\nTotal: {averias.Count()}");
        }

        static void CambiarEstadoAveria()
        {
            Console.Clear();
            Console.Write("ID de la avería: ");
            int id = int.Parse(Console.ReadLine());

            var averia = _unitOfWork.ReportesAveria.GetById(id);

            if (averia != null)
            {
                Console.WriteLine($"\nEstado actual: {averia.Estado}");
                Console.WriteLine("\nNuevo estado:");
                Console.WriteLine("1. En Proceso");
                Console.WriteLine("2. Resuelta");
                Console.WriteLine("3. Cerrada");
                Console.Write("\nOpción: ");

                string opcion = Console.ReadLine();
                string nuevoEstado = opcion switch
                {
                    "1" => "EnProceso",
                    "2" => "Resuelta",
                    "3" => "Cerrada",
                    _ => averia.Estado
                };

                Console.Write("Comentario: ");
                string comentario = Console.ReadLine();

                averia.Estado = nuevoEstado;
                averia.ComentarioResolucion = comentario;

                if (nuevoEstado == "Resuelta" || nuevoEstado == "Cerrada")
                {
                    averia.FechaResolucion = DateTime.Now;
                }

                _unitOfWork.ReportesAveria.Update(averia);
                _unitOfWork.SaveChanges();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n✓ Estado actualizado exitosamente");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("\n✗ Avería no encontrada");
            }
        }

        static void EstadisticasAverias()
        {
            Console.Clear();
            var todasAverias = _unitOfWork.ReportesAveria.GetAll().ToList();

            var porEstado = todasAverias
                .GroupBy(a => a.Estado)
                .Select(g => new { Estado = g.Key, Cantidad = g.Count() });

            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("ESTADÍSTICAS DE AVERÍAS");
            Console.WriteLine("═══════════════════════════════════════════════");

            Console.WriteLine("\nPOR ESTADO:");
            foreach (var grupo in porEstado)
            {
                Console.WriteLine($"{grupo.Estado,-20}: {grupo.Cantidad}");
            }

            Console.WriteLine($"\n═══════════════════════════════════════════════");
            Console.WriteLine($"TOTAL: {todasAverias.Count} averías reportadas");
        }

        // ============================================
        // REPORTES Y ESTADÍSTICAS
        // ============================================
        static void GenerarReportes()
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════╗");
            Console.WriteLine("║        REPORTES Y ESTADÍSTICAS                ║");
            Console.WriteLine("╚═══════════════════════════════════════════════╝");
            Console.WriteLine("\n[1] Resumen general del sistema");
            Console.WriteLine("[2] Reporte de ingresos");
            Console.WriteLine("[3] Reporte de usuarios activos");
            Console.WriteLine("[4] Exportar datos");
            Console.WriteLine("[0] Volver");
            Console.Write("\nOpción: ");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    ResumenGeneral();
                    break;
                case "2":
                    ReporteIngresos();
                    break;
                case "3":
                    ReporteUsuariosActivos();
                    break;
                case "4":
                    ExportarDatos();
                    break;
            }
        }

        static void ResumenGeneral()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║           RESUMEN GENERAL DEL SISTEMA                  ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            var usuarios = _unitOfWork.Usuarios.GetAll().ToList();
            var facturas = _unitOfWork.Facturas.GetAll().ToList();
            var pagos = _unitOfWork.Pagos.GetAll().ToList();
            var averias = _unitOfWork.ReportesAveria.GetAll().ToList();

            Console.WriteLine("\n┌─ USUARIOS ─────────────────────────────────────────┐");
            Console.WriteLine($"│ Total:                    {usuarios.Count,25} │");
            Console.WriteLine($"│ Activos:                  {usuarios.Count(u => u.Activo),25} │");
            Console.WriteLine($"│ Inactivos:                {usuarios.Count(u => !u.Activo),25} │");
            Console.WriteLine($"│ Administradores:          {usuarios.Count(u => u.RolId == 2),25} │");
            Console.WriteLine("└────────────────────────────────────────────────────┘");

            Console.WriteLine("\n┌─ FACTURAS ─────────────────────────────────────────┐");
            Console.WriteLine($"│ Total:                    {facturas.Count,25} │");
            Console.WriteLine($"│ Pendientes:               {facturas.Count(f => f.EstadoPago == "Pendiente"),25} │");
            Console.WriteLine($"│ Pagadas:                  {facturas.Count(f => f.EstadoPago == "Pagada"),25} │");
            Console.WriteLine($"│ Vencidas:                 {facturas.Count(f => f.FechaVencimiento < DateTime.Now && f.EstadoPago == "Pendiente"),25} │");
            Console.WriteLine($"│ Total por cobrar:         {("RD$" + facturas.Where(f => f.EstadoPago == "Pendiente").Sum(f => f.MontoTotal).ToString("N2")),25} │");
            Console.WriteLine("└────────────────────────────────────────────────────┘");

            Console.WriteLine("\n┌─ PAGOS ────────────────────────────────────────────┐");
            Console.WriteLine($"│ Total recibido:           {("RD$" + pagos.Sum(p => p.Monto).ToString("N2")),25} │");
            Console.WriteLine($"│ Transacciones:            {pagos.Count,25} │");
            Console.WriteLine($"│ Hoy:                      {pagos.Count(p => p.FechaPago.Date == DateTime.Now.Date),25} │");
            Console.WriteLine($"│ Este mes:                 {pagos.Count(p => p.FechaPago.Month == DateTime.Now.Month && p.FechaPago.Year == DateTime.Now.Year),25} │");
            Console.WriteLine("└────────────────────────────────────────────────────┘");

            Console.WriteLine("\n┌─ AVERÍAS ──────────────────────────────────────────┐");
            Console.WriteLine($"│ Total reportadas:         {averias.Count,25} │");
            Console.WriteLine($"│ Pendientes:               {averias.Count(a => a.Estado == "Reportada"),25} │");
            Console.WriteLine($"│ En proceso:               {averias.Count(a => a.Estado == "EnProceso"),25} │");
            Console.WriteLine($"│ Resueltas:                {averias.Count(a => a.Estado == "Resuelta"),25} │");
            Console.WriteLine("└────────────────────────────────────────────────────┘");

            Console.WriteLine($"\n{new string('═', 56)}");
            Console.WriteLine($"Reporte generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        }

        static void ReporteIngresos()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              REPORTE DE INGRESOS                       ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");

            var pagos = _unitOfWork.Pagos.GetAll().ToList();

            // Por mes
            var ingresosPorMes = pagos
                .GroupBy(p => new { p.FechaPago.Year, p.FechaPago.Month })
                .Select(g => new
                {
                    Periodo = $"{g.Key.Month:00}/{g.Key.Year}",
                    Total = g.Sum(p => p.Monto),
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Periodo)
                .Take(6);

            Console.WriteLine("\n┌─ ÚLTIMOS 6 MESES ──────────────────────────────────┐");
            Console.WriteLine($"│ {"PERIODO",-15} {"CANTIDAD",-15} {"TOTAL",-20} │");
            Console.WriteLine("├────────────────────────────────────────────────────┤");

            foreach (var mes in ingresosPorMes)
            {
                Console.WriteLine($"│ {mes.Periodo,-15} {mes.Cantidad,-15} {"RD$" + mes.Total.ToString("N2"),-20} │");
            }

            Console.WriteLine("└────────────────────────────────────────────────────┘");

            // Por método de pago
            var ingresosPorMetodo = pagos
                .GroupBy(p => p.MetodoPago)
                .Select(g => new
                {
                    Metodo = g.Key,
                    Total = g.Sum(p => p.Monto),
                    Porcentaje = (g.Sum(p => p.Monto) / pagos.Sum(p => p.Monto)) * 100
                });

            Console.WriteLine("\n┌─ POR MÉTODO DE PAGO ───────────────────────────────┐");
            Console.WriteLine($"│ {"MÉTODO",-20} {"TOTAL",-20} {"% DEL TOTAL",-12} │");
            Console.WriteLine("├────────────────────────────────────────────────────┤");

            foreach (var metodo in ingresosPorMetodo)
            {
                Console.WriteLine($"│ {metodo.Metodo,-20} {"RD$" + metodo.Total.ToString("N2"),-20} {metodo.Porcentaje.ToString("F1") + "%",-12} │");
            }

            Console.WriteLine("└────────────────────────────────────────────────────┘");

            decimal totalGeneral = pagos.Sum(p => p.Monto);
            Console.WriteLine($"\n║ TOTAL HISTÓRICO: RD${totalGeneral:N2}");
        }

        static void ReporteUsuariosActivos()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║           REPORTE DE USUARIOS ACTIVOS                  ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝\n");

            var usuarios = _unitOfWork.Usuarios.GetAll()
                .Where(u => u.Activo)
                .OrderBy(u => u.Nombre);

            Console.WriteLine($"{"CÉDULA",-15} {"NOMBRE",-30} {"EMAIL",-30} {"ROL",-15}");
            Console.WriteLine(new string('─', 90));

            foreach (var usuario in usuarios)
            {
                Console.WriteLine($"{usuario.Cedula,-15} {usuario.Nombre + " " + usuario.Apellido,-30} {usuario.Email,-30} {usuario.Rol?.Nombre ?? "N/A",-15}");
            }

            Console.WriteLine($"\nTotal de usuarios activos: {usuarios.Count()}");
        }

        static void ExportarDatos()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              EXPORTAR DATOS                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");

            Console.Write("\nRuta del archivo (ej: C:\\Reportes\\datos.txt): ");
            string ruta = Console.ReadLine();

            try
            {
                using (var writer = new System.IO.StreamWriter(ruta))
                {
                    writer.WriteLine("REPORTE COMPLETO DEL SISTEMA CAASD");
                    writer.WriteLine($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                    writer.WriteLine(new string('=', 80));
                    writer.WriteLine();

                    // Usuarios
                    writer.WriteLine("USUARIOS:");
                    writer.WriteLine(new string('-', 80));
                    foreach (var u in _unitOfWork.Usuarios.GetAll())
                    {
                        writer.WriteLine($"{u.Id}|{u.Cedula}|{u.Nombre}|{u.Apellido}|{u.Email}|{u.Activo}|{u.RolId}");
                    }
                    writer.WriteLine();

                    // Facturas
                    writer.WriteLine("FACTURAS:");
                    writer.WriteLine(new string('-', 80));
                    foreach (var f in _unitOfWork.Facturas.GetAll())
                    {
                        writer.WriteLine($"{f.Id}|{f.NumeroFactura}|{f.ContratoId}|{f.MontoTotal}|{f.EstadoPago}|{f.FechaEmision:yyyy-MM-dd}");
                    }
                    writer.WriteLine();

                    // Pagos
                    writer.WriteLine("PAGOS:");
                    writer.WriteLine(new string('-', 80));
                    foreach (var p in _unitOfWork.Pagos.GetAll())
                    {
                        writer.WriteLine($"{p.Id}|{p.NumeroTransaccion}|{p.FacturaId}|{p.Monto}|{p.MetodoPago}|{p.FechaPago:yyyy-MM-dd HH:mm:ss}");
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n✓ Datos exportados exitosamente");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n✗ Error al exportar: {ex.Message}");
                Console.ResetColor();
            }
        }

        // ============================================
        // CONFIGURACIÓN DEL SISTEMA
        // ============================================
        static void ConfiguracionSistema()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         CONFIGURACIÓN DEL SISTEMA                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.WriteLine("\n[1] Crear usuario administrador");
            Console.WriteLine("[2] Ver información de la base de datos");
            Console.WriteLine("[3] Limpiar datos de prueba");
            Console.WriteLine("[0] Volver");
            Console.Write("\nOpción: ");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    CrearAdministrador();
                    break;
                case "2":
                    VerInfoBaseDatos();
                    break;
                case "3":
                    LimpiarDatosPrueba();
                    break;
            }
        }

        static void CrearAdministrador()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("CREAR USUARIO ADMINISTRADOR");
            Console.WriteLine("═══════════════════════════════════════════════");

            Console.Write("\nCédula: ");
            string cedula = Console.ReadLine();

            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();

            Console.Write("Apellido: ");
            string apellido = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Contraseña: ");
            string password = Console.ReadLine();

            var admin = new Usuario
            {
                Cedula = cedula,
                Nombre = nombre,
                Apellido = apellido,
                Email = email,
                Telefono = "000-000-0000",
                DireccionCompleta = "Oficina Central",
                PasswordHash = SecurityHelper.HashPassword(password),
                FechaRegistro = DateTime.Now,
                Activo = true,
                RolId = 2 // Administrador
            };

            try
            {
                _unitOfWork.Usuarios.Add(admin);
                _unitOfWork.SaveChanges();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n✓ Administrador creado exitosamente");
                Console.WriteLine($"\nCredenciales:");
                Console.WriteLine($"Email: {email}");
                Console.WriteLine($"Password: {password}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n✗ Error: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void VerInfoBaseDatos()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║        INFORMACIÓN DE LA BASE DE DATOS                 ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝\n");

            Console.WriteLine($"Usuarios:            {_unitOfWork.Usuarios.GetAll().Count()}");
            Console.WriteLine($"Roles:               {_unitOfWork.Usuarios.GetAll().Select(u => u.RolId).Distinct().Count()}");
            Console.WriteLine($"Contratos:           {_unitOfWork.Contratos.GetAll().Count()}");
            Console.WriteLine($"Facturas:            {_unitOfWork.Facturas.GetAll().Count()}");
            Console.WriteLine($"Pagos:               {_unitOfWork.Pagos.GetAll().Count()}");
            Console.WriteLine($"Reportes Avería:     {_unitOfWork.ReportesAveria.GetAll().Count()}");
            Console.WriteLine($"Solicitudes:         {_unitOfWork.Solicitudes.GetAll().Count()}");
            Console.WriteLine($"Notificaciones:      {_unitOfWork.Notificaciones.GetAll().Count()}");
            Console.WriteLine($"Contenidos Educativos: {_unitOfWork.ContenidosEducativos.GetAll().Count()}");
            Console.WriteLine($"Estados Servicio:    {_unitOfWork.EstadosServicio.GetAll().Count()}");
        }

        static void LimpiarDatosPrueba()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("⚠️  ADVERTENCIA: LIMPIAR DATOS DE PRUEBA");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.ResetColor();

            Console.WriteLine("\nEsto eliminará:");
            Console.WriteLine("- Todas las facturas no pagadas");
            Console.WriteLine("- Todas las averías cerradas");
            Console.WriteLine("- Todas las notificaciones leídas");

            Console.Write("\n¿Está seguro? (SI/NO): ");
            string confirm = Console.ReadLine();

            if (confirm.ToUpper() == "SI")
            {
                try
                {
                    // Eliminar facturas pendientes
                    var facturasPendientes = _unitOfWork.Facturas.GetAll()
                        .Where(f => f.EstadoPago == "Pendiente")
                        .ToList();

                    foreach (var f in facturasPendientes)
                    {
                        _unitOfWork.Facturas.Delete(f);
                    }

                    _unitOfWork.SaveChanges();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n✓ {facturasPendientes.Count} facturas eliminadas");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n✗ Error: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
    }
}