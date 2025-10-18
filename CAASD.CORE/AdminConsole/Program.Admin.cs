// CAASD.CORE/AdminConsole/Program.Admin.cs
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using CAASD.Core.Constants;
using CAASD.Core.DTOs;
using CAASD.Core.Entities;
using CAASD.Core.Helpers;
using CAASD.Core.Infrastructure;

// Repositorios (Core)
using CAASD.Core.Interfaces.Repositories;
using CAASD.Core.Repositories;

// Servicios (interfaces)
using CAASD.Core.Interfaces.Services;

namespace CAASD.Core.AdminConsole;

public class Program
{
    // Servicios (inyectados)
    private static IAuthService _auth = default!;
    private static IUsuarioService _usuarios = default!;
    private static IContratoService _contratos = default!;
    private static IFacturaService _facturas = default!;
    private static IPagoService _pagos = default!;
    private static IReporteAveriaService _averias = default!;
    private static INotificacionService _notificaciones = default!;

    public static async Task Main(string[] args)
    {
        Console.Title = "CAASD - Panel de Administración (Core)";

        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(cfg =>
            {
                cfg.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                cfg.AddEnvironmentVariables();
            })
            .ConfigureServices((ctx, services) =>
            {
                var conn = ctx.Configuration.GetConnectionString("CAASDConnection");

                // DbContext del Core
                services.AddDbContext<CaasdCoreDbContext>(opt => opt.UseSqlServer(conn));

                // Repositorios del Core
                services.AddScoped<IUsuarioRepository, UsuarioRepository>();
                services.AddScoped<IContratoRepository, ContratoRepository>();
                services.AddScoped<IFacturaRepository, FacturaRepository>();
                services.AddScoped<IPagoRepository, PagoRepository>();
                services.AddScoped<IReporteAveriaRepository, ReporteAveriaRepository>();
                services.AddScoped<INotificacionRepository, NotificacionRepository>();
                services.AddScoped<IContenidoEducativoRepository, ContenidoEducativoRepository>();
                services.AddScoped<IEstadoServicioRepository, EstadoServicioRepository>();
                services.AddScoped<ISolicitudRepository, SolicitudRepository>();

                // Servicios del Core 
                services.AddScoped<IAuthService, AuthService>();
                services.AddScoped<IUsuarioService, UsuarioService>();
                services.AddScoped<IContratoService, ContratoService>();
                services.AddScoped<IFacturaService, FacturaService>();
                services.AddScoped<IPagoService, PagoService>();
                services.AddScoped<IReporteAveriaService, ReporteAveriaService>();
                services.AddScoped<INotificacionService, NotificacionService>();
            })
            .Build();

        var sp = host.Services;
        _auth = sp.GetRequiredService<IAuthService>();
        _usuarios = sp.GetRequiredService<IUsuarioService>();
        _contratos = sp.GetRequiredService<IContratoService>();
        _facturas = sp.GetRequiredService<IFacturaService>();
        _pagos = sp.GetRequiredService<IPagoService>();
        _averias = sp.GetRequiredService<IReporteAveriaService>();
        _notificaciones = sp.GetRequiredService<INotificacionService>();

        Header();

        // ======== LOGIN ========
        Console.WriteLine();
        Yellow("Inicio de sesión - Acceso privado (CORE)");

        Console.Write("\nEmail: ");
        var email = Console.ReadLine() ?? string.Empty;
        Console.Write("Password: ");
        var pass = ReadPassword();

        var login = _auth.Login(new LoginRequestDTO
        {
            Email = email,
            Password = pass,
            ModuloDestino = AppConstants.MOD_CORE
        });

        if (!login.Success || login.Usuario == null)
        {
            Red("\nAcceso denegado: credenciales inválidas.");
            return;
        }

        // Control de acceso (AccessPolicy + Módulo CORE)
        // UsuarioDTO tiene Rol (string) y EsAdministrador (bool), no RolId.
        var usuario = login.Usuario;
        int rolId = usuario.EsAdministrador ? AppConstants.ROLID_SUPERADMIN : MapRolToId(usuario.Rol);
        if (!AccessPolicy.RolPuedeEntrarModulo(rolId, AppConstants.MOD_CORE))
        {
            Red("\nAcceso denegado: su rol no tiene permiso para el módulo CORE.");
            return;
        }

        Green($"\nBienvenido {usuario.NombreCompleto} (Rol: {usuario.Rol})");

        bool loop = true;
        while (loop)
        {
            MostrarMenuPrincipal();
            var op = Console.ReadLine();

            switch (op)
            {
                case "1": await GestionUsuarios(); break;
                case "2": await GestionFacturas(); break;
                case "3": await GestionPagos(); break;
                case "4": await GestionAverias(); break;
                case "5": await GestionNotificaciones(); break;
                case "6": await GestionContratos(); break;
                case "0": loop = false; break;
                default: Console.WriteLine("Opción inválida"); break;
            }

            if (loop)
            {
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }

        Console.WriteLine("\n¡Hasta luego!");
    }

    // ====== MENÚS ======
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
        Console.WriteLine("[5] Notificaciones");
        Console.WriteLine("[6] Gestión de Contratos");
        Console.WriteLine("[0] Salir");
        Console.Write("\nSeleccione una opción: ");
    }

    static async Task GestionUsuarios()
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║          GESTIÓN DE USUARIOS                  ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");
        Console.WriteLine("\n[1] Listar usuarios");
        Console.WriteLine("[2] Crear usuario de STAFF (Admin/Cajero)");
        Console.WriteLine("[3] Editar perfil (datos + rol)"); // <- NUEVO
        Console.WriteLine("[0] Volver");
        Console.Write("\nOpción: ");
        var op = Console.ReadLine();

        switch (op)
        {
            case "1":
                {
                    var lista = _usuarios.ObtenerTodos();

                    Console.WriteLine($"\n{"ID",-4} {"CEDULA",-12} {"NOMBRE",-32} {"EMAIL",-28} {"ROL",-18}");
                    Console.WriteLine(new string('─', 100));

                    foreach (var u in lista)
                    {
                        var nombreCompleto = ($"{u.Nombre} {u.Apellido}").Trim();
                        var rolNombre = u.Rol?.Nombre ?? MapRolIdToName(u.RolId);

                        Console.WriteLine(string.Format("{0,-4} {1,-12} {2,-32} {3,-28} {4,-18}",
                            u.Id, u.Cedula, nombreCompleto, u.Email, rolNombre));
                    }

                    Console.WriteLine($"\nTotal: {lista.Count()}");
                    break;
                }

            case "2":
                {
                    // Solo SUPERADMIN usa Core, así que no pedimos más validación aquí
                    Console.WriteLine("\nTipo de staff a crear:");
                    Console.WriteLine("[1] Administrador (acceso Web)");
                    Console.WriteLine("[2] Cajero (acceso Caja)");
                    Console.Write("Rol: ");
                    var rolSel = Console.ReadLine();

                    var rolString = rolSel == "1"
                        ? AppConstants.ROL_ADMINISTRADOR
                        : AppConstants.ROL_CAJERO; // default cajero

                    Console.Write("\nCédula: "); var ced = Console.ReadLine() ?? "";
                    Console.Write("Nombre completo: "); var nomc = Console.ReadLine() ?? "";
                    Console.Write("Email: "); var em = Console.ReadLine() ?? "";
                    Console.Write("Password: "); var pw = Console.ReadLine() ?? "";

                    var staff = new UsuarioDTO
                    {
                        Cedula = ced,
                        NombreCompleto = nomc,
                        Email = em,
                        Rol = rolString,
                        EsAdministrador = false
                    };

                    var ok = _auth.RegistrarUsuario(staff, pw);
                    Console.WriteLine(ok ? "\n✓ Usuario de staff creado" : "\n✗ No se pudo crear");
                    break;
                }

            case "3": // NUEVO: editar datos + rol
                {
                    Console.Write("\nID del usuario a modificar: ");
                    if (!int.TryParse(Console.ReadLine(), out var uid))
                    {
                        Red("\n✗ ID inválido.");
                        break;
                    }

                    var u = _usuarios.ObtenerPorId(uid);
                    if (u == null)
                    {
                        Red("\n✗ Usuario no encontrado.");
                        break;
                    }

                    // Proteger cuenta ROOT (opcional)
                    if (u.Id == 9999)
                    {
                        Red("\n✗ No se permite modificar el usuario ROOT.");
                        break;
                    }

                    // Mostrar actuales
                    Console.WriteLine("\n-- Valores actuales (Enter para mantener) --");
                    Console.WriteLine($"Cédula actual:   {u.Cedula}");
                    Console.WriteLine($"Nombre actual:   {u.Nombre} {u.Apellido}");
                    Console.WriteLine($"Email actual:    {u.Email}");
                    Console.WriteLine($"Teléfono actual: {u.Telefono}");
                    Console.WriteLine($"Dirección act.:  {u.DireccionCompleta}");
                    Console.WriteLine($"Rol actual:      {u.Rol?.Nombre ?? MapRolIdToName(u.RolId)}");

                    // Pedir nuevos (Enter mantiene)
                    Console.Write("\nNueva cédula: ");
                    var ced = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(ced)) u.Cedula = ced!.Trim();

                    Console.Write("Nuevo nombre completo: ");
                    var nomc = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(nomc))
                    {
                        var (nombre, apellido) = SplitNombreLocal(nomc!);
                        u.Nombre = nombre;
                        u.Apellido = apellido;
                    }

                    Console.Write("Nuevo email: ");
                    var em = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(em)) u.Email = em!.Trim();

                    Console.Write("Nuevo teléfono: ");
                    var tel = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(tel)) u.Telefono = tel!.Trim();

                    Console.Write("Nueva dirección: ");
                    var dir = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(dir)) u.DireccionCompleta = dir!.Trim();

                    // Rol (Enter mantiene)
                    Console.WriteLine("\nSeleccione nuevo rol (Enter para mantener):");
                    Console.WriteLine("[1] Administrador (Web)");
                    Console.WriteLine("[2] Cajero (Caja)");
                    Console.WriteLine("[3] Cliente (Web)");
                    Console.WriteLine("[4] Superadmin (Core)");
                    Console.Write("Rol: ");
                    var s = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        u.RolId = s switch
                        {
                            "1" => AppConstants.ROLID_ADMINISTRADOR,
                            "2" => AppConstants.ROLID_CAJERO,
                            "3" => AppConstants.ROLID_CLIENTE,
                            "4" => AppConstants.ROLID_SUPERADMIN,
                            _ => u.RolId
                        };
                    }

                    // Guardar cambios
                    _usuarios.Actualizar(u);
                    Green("\n✓ Usuario actualizado.");
                    break;
                }
        }

        await Task.CompletedTask;
    }



    static async Task GestionFacturas()
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║          GESTIÓN DE FACTURAS                  ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");
        Console.WriteLine("\n[1] Ver pendientes por usuario");
        Console.WriteLine("[2] Buscar por número");
        Console.WriteLine("[3] Crear nueva factura");
        Console.WriteLine("[4] Listar TODAS por usuario");     // NUEVO
        Console.WriteLine("[5] Listar por contrato");          // NUEVO
        Console.WriteLine("[6] Marcar como PAGADA");           // NUEVO
        Console.WriteLine("[7] Modificar montos");             // NUEVO
        Console.WriteLine("[8] Anular factura");               // NUEVO
        Console.WriteLine("[0] Volver");
        Console.Write("\nOpción: ");
        var op = Console.ReadLine();

        switch (op)
        {
            case "1":
                {
                    Console.Write("\nUsuarioId: ");
                    if (int.TryParse(Console.ReadLine(), out int uid))
                    {
                        var pend = _facturas.ObtenerFacturasPendientes(uid);
                        Console.WriteLine($"\n{"NUMERO",-18} {"TOTAL",-12} {"VENCE",-12}");
                        Console.WriteLine(new string('─', 48));
                        foreach (var f in pend)
                            Console.WriteLine($"{f.NumeroFactura,-18} {"RD$" + f.MontoTotal.ToString("N2"),-12} {f.FechaVencimiento:yyyy-MM-dd,-12}");
                        Console.WriteLine($"\nTotal adeudado: RD${_facturas.ObtenerTotalAdeudado(uid):N2}");
                    }
                    else Red("\n✗ UsuarioId inválido.");
                    break;
                }

            case "2":
                {
                    Console.Write("\nNúmero de factura: ");
                    var num = Console.ReadLine();
                    var fx = _facturas.ObtenerFacturaPorNumero(num ?? "");
                    if (fx == null) Console.WriteLine("\n✗ No encontrada");
                    else
                    {
                        Console.WriteLine($"\nNúmero: {fx.NumeroFactura}");
                        Console.WriteLine($"Contrato: {fx.NumeroContrato}");
                        Console.WriteLine($"Total: RD${fx.MontoTotal:N2}");
                        Console.WriteLine($"Estado: {fx.EstadoPago}");
                        Console.WriteLine($"Emisión: {fx.FechaEmision:yyyy-MM-dd}");
                        Console.WriteLine($"Vence:   {fx.FechaVencimiento:yyyy-MM-dd}");
                    }
                    break;
                }

            case "3": // Crear factura
                {
                    Console.Write("\nContratoId: ");
                    if (!int.TryParse(Console.ReadLine(), out int contratoId))
                    { Red("\n✗ ContratoId inválido."); break; }

                    Console.Write("Metros consumidos: ");
                    int.TryParse(Console.ReadLine(), out int m3);
                    Console.Write("Monto Agua RD$: ");
                    decimal.TryParse(Console.ReadLine(), out decimal mAgua);
                    Console.Write("Monto Alcantarillado RD$: ");
                    decimal.TryParse(Console.ReadLine(), out decimal mAlc);
                    Console.Write("Monto Basura RD$: ");
                    decimal.TryParse(Console.ReadLine(), out decimal mBas);

                    var nueva = new Factura
                    {
                        ContratoId = contratoId,
                        MetrosConsumidos = m3,
                        MontoAgua = mAgua,
                        MontoAlcantarillado = mAlc,
                        MontoBasura = mBas
                    };

                    try
                    {
                        var id = _facturas.CrearFactura(nueva);
                        Green($"\n✓ Factura creada: {nueva.NumeroFactura} (Id: {id})");
                        Console.WriteLine($"Emisión: {nueva.FechaEmision:yyyy-MM-dd} | Vence: {nueva.FechaVencimiento:yyyy-MM-dd}");
                        Console.WriteLine($"Total RD$: {nueva.MontoTotal:N2} | Estado: {nueva.EstadoPago}");
                    }
                    catch (Microsoft.Data.SqlClient.SqlException sqlEx)
                    {
                        Red("\n✗ Error SQL al crear la factura.");
                        Console.WriteLine(sqlEx.Message);
                        Yellow("\nSugerencia: verifique que exista el CONTRATO y que el Id sea correcto.");
                    }
                    catch (Exception ex)
                    {
                        Red("\n✗ No se pudo crear la factura.");
                        Console.WriteLine(ex.Message);
                    }
                    break;
                }

            case "4": // Listar todas por usuario
                {
                    Console.Write("\nUsuarioId: ");
                    if (!int.TryParse(Console.ReadLine(), out var uid)) { Red("\n✗ UsuarioId inválido."); break; }
                    var todas = _facturas.ObtenerFacturasPorUsuario(uid).OrderByDescending(x => x.FechaEmision);
                    if (!todas.Any()) { Yellow("\nNo hay facturas para este usuario."); break; }

                    Console.WriteLine($"\n{"NUMERO",-18} {"ESTADO",-10} {"TOTAL",-12} {"EMISION",-12} {"VENCE",-12}");
                    Console.WriteLine(new string('─', 70));
                    foreach (var f in todas)
                        Console.WriteLine($"{f.NumeroFactura,-18} {f.EstadoPago,-10} {"RD$" + f.MontoTotal.ToString("N2"),-12} {f.FechaEmision:yyyy-MM-dd,-12} {f.FechaVencimiento:yyyy-MM-dd,-12}");
                    break;
                }

            case "5": // Listar por contrato
                {
                    Console.Write("\nContratoId: ");
                    if (!int.TryParse(Console.ReadLine(), out var cid)) { Red("\n✗ ContratoId inválido."); break; }
                    var lista = _facturas.ObtenerFacturasPorContrato(cid);
                    if (!lista.Any()) { Yellow("\nNo hay facturas para este contrato."); break; }

                    Console.WriteLine($"\n{"NUMERO",-18} {"ESTADO",-10} {"TOTAL",-12} {"EMISION",-12} {"VENCE",-12}");
                    Console.WriteLine(new string('─', 70));
                    foreach (var f in lista)
                        Console.WriteLine($"{f.NumeroFactura,-18} {f.EstadoPago,-10} {"RD$" + f.MontoTotal.ToString("N2"),-12} {f.FechaEmision:yyyy-MM-dd,-12} {f.FechaVencimiento:yyyy-MM-dd,-12}");
                    break;
                }

            case "6": // Marcar como pagada
                {
                    Console.Write("\nFacturaId: ");
                    if (!int.TryParse(Console.ReadLine(), out var fid)) { Red("\n✗ FacturaId inválido."); break; }
                    var ok = _facturas.ActualizarEstado(fid, "Pagada", DateTime.Now);
                    Console.WriteLine(ok ? "\n✓ Factura marcada como Pagada" : "\n✗ No se pudo actualizar");
                    break;
                }

            case "7": // Modificar montos (recalcula)
                {
                    Console.Write("\nFacturaId: ");
                    if (!int.TryParse(Console.ReadLine(), out var fid)) { Red("\n✗ FacturaId inválido."); break; }

                    Console.Write("Metros consumidos: ");
                    int.TryParse(Console.ReadLine(), out int m3);
                    Console.Write("Monto Agua RD$: ");
                    decimal.TryParse(Console.ReadLine(), out decimal mAgua);
                    Console.Write("Monto Alcantarillado RD$: ");
                    decimal.TryParse(Console.ReadLine(), out decimal mAlc);
                    Console.Write("Monto Basura RD$: ");
                    decimal.TryParse(Console.ReadLine(), out decimal mBas);

                    var ok = _facturas.ActualizarMontos(fid, m3, mAgua, mAlc, mBas, recalcularItbis: true);
                    Console.WriteLine(ok ? "\n✓ Factura actualizada (totales recalculados)" : "\n✗ No se pudo actualizar");
                    break;
                }

            case "8": // Anular
                {
                    Console.Write("\nFacturaId: ");
                    if (!int.TryParse(Console.ReadLine(), out var fid)) { Red("\n✗ FacturaId inválido."); break; }
                    var ok = _facturas.AnularFactura(fid);
                    Console.WriteLine(ok ? "\n✓ Factura ANULADA" : "\n✗ No se pudo anular");
                    break;
                }
        }

        await Task.CompletedTask;
    }



    static async Task GestionPagos()
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║           GESTIÓN DE PAGOS                    ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");
        Console.WriteLine("\n[1] Verificar pago por transacción");
        Console.WriteLine("[2] Procesar pago rápido");
        Console.WriteLine("[0] Volver");
        Console.Write("\nOpción: ");
        var op = Console.ReadLine();

        switch (op)
        {
            case "1":
                Console.Write("\nNo. Transacción: ");
                var nt = Console.ReadLine();
                var ok = _pagos.VerificarPago(nt ?? "");
                Console.WriteLine(ok ? "\n✓ Existe/OK" : "\n✗ No encontrado");
                break;

            case "2":
                Console.Write("\nFacturaId: ");
                int.TryParse(Console.ReadLine(), out int fid);
                Console.Write("Monto: ");
                decimal.TryParse(Console.ReadLine(), out decimal monto);
                Console.Write("Método (Efectivo/Tarjeta): ");
                var metodo = Console.ReadLine() ?? "Efectivo";
                var res = _pagos.ProcesarPago(fid, monto, metodo);
                Console.WriteLine(res ? "\n✓ Pago procesado" : "\n✗ Falló el pago");
                break;
        }
        await Task.CompletedTask;
    }

    static async Task GestionAverias()
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║         GESTIÓN DE AVERÍAS                    ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");
        Console.WriteLine("\n[1] Listar reportadas");
        Console.WriteLine("[2] Cambiar estado");
        Console.WriteLine("[0] Volver");
        Console.WriteLine("\nOpción: ");
        var op = Console.ReadLine();

        switch (op)
        {
            case "1":
                var pendientes = _averias.ObtenerAveriasEnZona(18.5m, -69.9m, 50); // demo
                foreach (var a in pendientes)
                    Console.WriteLine($"#{a.Id} - {a.TipoAveria} - {a.Estado}");
                break;

            case "2":
                Console.Write("\nAveriaId: ");
                int.TryParse(Console.ReadLine(), out int aid);
                Console.Write("Nuevo estado: ");
                var ne = Console.ReadLine() ?? "EnProceso";
                Console.Write("Comentario: ");
                var com = Console.ReadLine();
                var ok = _averias.ActualizarEstadoAveria(aid, ne, com ?? "");
                Console.WriteLine(ok ? "\n✓ Actualizada" : "\n✗ No se pudo actualizar");
                break;
        }
        await Task.CompletedTask;
    }

    static async Task GestionNotificaciones()
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║         NOTIFICACIONES                        ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");
        Console.WriteLine("\n[1] Enviar notificación a usuario");
        Console.WriteLine("[2] Ver notificaciones de un usuario");
        Console.WriteLine("[0] Volver");
        Console.Write("\nOpción: ");
        var op = Console.ReadLine();

        switch (op)
        {
            case "1":
                Console.Write("\nUsuarioId: "); int.TryParse(Console.ReadLine(), out int uid);
                Console.Write("Título: "); var t = Console.ReadLine() ?? "Aviso";
                Console.Write("Mensaje: "); var m = Console.ReadLine() ?? "Mensaje de prueba";
                Console.Write("Tipo: "); var tipo = Console.ReadLine() ?? "INFO";
                _notificaciones.EnviarNotificacion(uid, t, m, tipo);
                Console.WriteLine("\n✓ Enviada");
                break;

            case "2":
                Console.Write("\nUsuarioId: "); int.TryParse(Console.ReadLine(), out int uidx);
                var list = _notificaciones.ObtenerNotificacionesUsuario(uidx);
                foreach (var n in list)
                    Console.WriteLine($"[{n.Tipo}] {n.Titulo} - {(n.Leida ? "Leída" : "No leída")}");
                break;
        }
        await Task.CompletedTask;
    }

    static async Task GestionContratos()
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║          GESTIÓN DE CONTRATOS                 ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");
        Console.WriteLine("\n[1] Crear contrato");
        Console.WriteLine("[2] Listar contratos por usuario");
        Console.WriteLine("[0] Volver");
        Console.Write("\nOpción: ");
        var op = Console.ReadLine();

        switch (op)
        {
            case "1":
                {
                    Console.Write("\nUsuarioId: ");
                    if (!int.TryParse(Console.ReadLine(), out var usuarioId))
                    {
                        Red("\n✗ UsuarioId inválido.");
                        break;
                    }

                    Console.Write("Dirección: ");
                    var dir = Console.ReadLine() ?? "";
                    Console.Write("Sector: ");
                    var sector = Console.ReadLine() ?? "";
                    Console.Write("Latitud (opcional): ");
                    decimal.TryParse(Console.ReadLine(), out var lat);
                    Console.Write("Longitud (opcional): ");
                    decimal.TryParse(Console.ReadLine(), out var lon);

                    // Generar número de contrato
                    var sufijo = DateTime.UtcNow.ToString("yyyyMM");
                    var aleatorio = Guid.NewGuid().ToString("N")[..5].ToUpperInvariant();
                    var numeroContrato = $"CTR-{sufijo}-{aleatorio}";

                    var contrato = new Contrato
                    {
                        NumeroContrato = numeroContrato,
                        UsuarioId = usuarioId,
                        Direccion = dir,
                        Sector = sector,
                        Latitud = lat,
                        Longitud = lon,
                        FechaInicio = DateTime.Now,
                        Activo = true
                    };

                    _contratos.Crear(contrato);
                    Green($"\n✓ Contrato creado: {numeroContrato} (Id: {contrato.Id})");
                    break;
                }

            case "2":
                {
                    Console.Write("\nUsuarioId: ");
                    if (!int.TryParse(Console.ReadLine(), out var usuarioId))
                    {
                        Red("\n✗ UsuarioId inválido.");
                        break;
                    }

                    var contratos = _contratos.ObtenerPorUsuario(usuarioId, soloActivos: false);
                    if (!contratos.Any())
                    {
                        Yellow("\nNo hay contratos para este usuario.");
                        break;
                    }

                    Console.WriteLine($"\n{"ID",-4} {"NUMERO",-18} {"SECTOR",-18} {"ACTIVO",-8} {"F.INICIO",-12}");
                    Console.WriteLine(new string('─', 70));
                    foreach (var c in contratos)
                        Console.WriteLine($"{c.Id,-4} {c.NumeroContrato,-18} {c.Sector,-18} {(c.Activo ? "Sí" : "No"),-8} {c.FechaInicio:yyyy-MM-dd,-12}");
                    break;
                }
        }

        await Task.CompletedTask;
    }

    // ===== helpers UI =====
    static void Header()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║   CAASD - SISTEMA DE GESTIÓN DE AGUA          ║");
        Console.WriteLine("║        Panel de Administración (Core)         ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");
        Console.ResetColor();
    }
    static void Green(string s) { Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(s); Console.ResetColor(); }
    static void Red(string s) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(s); Console.ResetColor(); }
    static void Yellow(string s) { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(s); Console.ResetColor(); }
    static string ReadPassword()
    {
        var pwd = string.Empty;
        ConsoleKey key;
        do
        {
            var ki = Console.ReadKey(true);
            key = ki.Key;
            if (key == ConsoleKey.Backspace && pwd.Length > 0)
            { pwd = pwd[..^1]; Console.Write("\b \b"); }
            else if (!char.IsControl(ki.KeyChar))
            { pwd += ki.KeyChar; Console.Write("*"); }
        } while (key != ConsoleKey.Enter);
        Console.WriteLine();
        return pwd;
    }

    // Mapea el rol string de UsuarioDTO a los IDs esperados por AccessPolicy
    static int MapRolToId(string rol)
    {
        var r = (rol ?? "").Trim().ToUpperInvariant();
        return r switch
        {
            "SUPERADMIN" => AppConstants.ROLID_SUPERADMIN,
            "ADMIN" => AppConstants.ROLID_ADMINISTRADOR,
            "CAJERO" => AppConstants.ROLID_CAJERO,
            "CLIENTE" => AppConstants.ROLID_CLIENTE,
            _ => AppConstants.ROLID_CLIENTE
        };
    }
    // Mapea RolId (entero) al nombre de rol legible
    static string MapRolIdToName(int rolId)
    {
        return rolId switch
        {
            var x when x == AppConstants.ROLID_SUPERADMIN => AppConstants.ROL_SUPERADMIN,
            var x when x == AppConstants.ROLID_ADMINISTRADOR => AppConstants.ROL_ADMINISTRADOR,
            var x when x == AppConstants.ROLID_CAJERO => AppConstants.ROL_CAJERO,
            _ => AppConstants.ROL_CLIENTE
        };
    }
    static (string nombre, string apellido) SplitNombreLocal(string nombreCompleto)
    {
        var n = (nombreCompleto ?? "").Trim();
        if (string.IsNullOrEmpty(n)) return ("", "");
        var parts = n.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1) return (parts[0], "");
        return (string.Join(" ", parts[..^1]), parts[^1]);
    }
}

/* ===========================
   IMPLEMENTACIONES MÍNIMAS DE SERVICIOS (Core)
   - Envuelven tus repos del Core
   - Usan tus DTOs reales (UsuarioDTO, FacturaDTO, PagoDTO)
   - Cuando tengas tus servicios oficiales, cambia el DI y elimina estas clases
=============================*/

internal class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarios;
    public AuthService(IUsuarioRepository usuarios) => _usuarios = usuarios;

    public LoginResponseDTO Login(LoginRequestDTO dto)
    {
        var user = _usuarios.GetByEmail(dto.Email);
        var ok = user != null && SecurityHelper.VerifyPassword(dto.Password, user.PasswordHash);

        return new LoginResponseDTO
        {
            Success = ok && user!.Activo,
            Mensaje = ok ? "OK" : "Credenciales inválidas",
            Usuario = ok ? new UsuarioDTO
            {
                Id = user!.Id,
                Cedula = user.Cedula,
                NombreCompleto = $"{user.Nombre} {user.Apellido}",
                Email = user.Email,
                Direccion = user.DireccionCompleta ?? "",
                Telefono = user.Telefono ?? "",
                Rol = user.Rol?.Nombre?.ToUpperInvariant() ?? "CLIENTE",
                EsAdministrador = (user.Rol?.Nombre?.ToUpperInvariant() ?? "") == "SUPERADMIN" || (user.RolId == AppConstants.ROLID_SUPERADMIN)
            } : null
        };
    }

    public bool RegistrarUsuario(UsuarioDTO usuario, string password)
    {
        if (_usuarios.GetByEmail(usuario.Email) != null) return false;

        // descomponer nombre completo
        var (nombre, apellido) = SplitNombre(usuario.NombreCompleto);

        
        var rolId = ResolveRolId(usuario.Rol, usuario.EsAdministrador);

        var u = new Usuario
        {
            Cedula = usuario.Cedula,
            Nombre = nombre,
            Apellido = apellido,
            Email = usuario.Email,
            DireccionCompleta = usuario.Direccion,
            Telefono = usuario.Telefono,
            RolId = rolId,
            Activo = true,
            FechaRegistro = DateTime.Now,
            PasswordHash = SecurityHelper.HashPassword(password)
        };

        _usuarios.Add(u);
        return true;
    }

  
    private static int ResolveRolId(string? rol, bool esAdminFlag)
    {
        if (esAdminFlag) return AppConstants.ROLID_SUPERADMIN;

        var r = (rol ?? "").Trim().ToUpperInvariant();

        // acepta equivalentes
        if (r is "ADMINISTRADOR" or "ADMIN" ||
            r.Equals(AppConstants.ROL_ADMINISTRADOR, StringComparison.OrdinalIgnoreCase))
            return AppConstants.ROLID_ADMINISTRADOR;

        if (r is "CAJERO" ||
            r.Equals(AppConstants.ROL_CAJERO, StringComparison.OrdinalIgnoreCase))
            return AppConstants.ROLID_CAJERO;

        if (r is "SUPERADMIN" ||
            r.Equals(AppConstants.ROL_SUPERADMIN, StringComparison.OrdinalIgnoreCase))
            return AppConstants.ROLID_SUPERADMIN;

        // default → CLIENTE
        return AppConstants.ROLID_CLIENTE;
    }


    public bool CambiarPassword(int usuarioId, string passwordActual, string passwordNuevo)
    {
        var u = _usuarios.GetById(usuarioId);
        if (u == null) return false;
        if (!SecurityHelper.VerifyPassword(passwordActual, u.PasswordHash)) return false;
        u.PasswordHash = SecurityHelper.HashPassword(passwordNuevo);
        _usuarios.Update(u);
        return true;
    }

    public bool RecuperarPassword(string email) => _usuarios.GetByEmail(email) != null;

    private static (string nombre, string apellido) SplitNombre(string nombreCompleto)
    {
        var n = (nombreCompleto ?? "").Trim();
        if (string.IsNullOrEmpty(n)) return ("", "");
        var parts = n.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1) return (parts[0], "");
        return (string.Join(" ", parts[..^1]), parts[^1]);
    }
  
    public bool RegistrarClienteDesdeWeb(UsuarioDTO dto, string password)
    {
        dto.Rol = AppConstants.ROL_CLIENTE;
        dto.EsAdministrador = false;
        return RegistrarUsuario(dto, password);
    }


    static int MapRolToId(string rol)
    {
        var r = (rol ?? "").Trim().ToUpperInvariant();
        return r switch
        {
            "SUPERADMIN" => AppConstants.ROLID_SUPERADMIN,
            "ADMIN" => AppConstants.ROLID_ADMINISTRADOR,
            "CAJERO" => AppConstants.ROLID_CAJERO,
            "CLIENTE" => AppConstants.ROLID_CLIENTE,
            _ => AppConstants.ROLID_CLIENTE
        };
    }
}

internal class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repo;
    public UsuarioService(IUsuarioRepository repo) => _repo = repo;

    public IEnumerable<Usuario> ObtenerTodos() => _repo.GetAll();
    public Usuario? ObtenerPorId(int id) => _repo.GetById(id);
    public Usuario? ObtenerPorEmail(string email) => _repo.GetByEmail(email);

    public void Crear(Usuario usuario, string passwordPlano)
    {
        usuario.PasswordHash = SecurityHelper.HashPassword(passwordPlano);
        usuario.Activo = true;
        usuario.FechaRegistro = DateTime.Now;
        _repo.Add(usuario);
    }

    public void Actualizar(Usuario usuario) => _repo.Update(usuario);
}

internal class ContratoService : IContratoService
{
    private readonly IContratoRepository _repo;
    public ContratoService(IContratoRepository repo) => _repo = repo;

    public IEnumerable<Contrato> ObtenerPorUsuario(int usuarioId, bool soloActivos = true)
        => _repo.GetAll().Where(c => c.UsuarioId == usuarioId && (!soloActivos || c.Activo));

    public void Crear(Contrato contrato) => _repo.Add(contrato);
}

internal class FacturaService : IFacturaService
{
    private readonly IFacturaRepository _repo;
    public FacturaService(IFacturaRepository repo) => _repo = repo;

    public IEnumerable<FacturaDTO> ObtenerFacturasPendientes(int usuarioId)
        => _repo.GetFacturasPendientesByUsuario(usuarioId).Select(Map);

    public IEnumerable<FacturaDTO> ObtenerFacturasPorUsuario(int usuarioId)
        => _repo.GetAll()
                .Where(f => f.Contrato != null && f.Contrato.UsuarioId == usuarioId)
                .Select(Map);

    public IEnumerable<FacturaDTO> ObtenerFacturasPorContrato(int contratoId)
        => _repo.GetAll().Where(f => f.ContratoId == contratoId)
            .OrderByDescending(f => f.FechaEmision).Select(Map);

    public FacturaDTO? ObtenerFacturaPorNumero(string numeroFactura)
        => _repo.GetByNumeroFactura(numeroFactura) is { } f ? Map(f) : null;

    public decimal ObtenerTotalAdeudado(int usuarioId)
        => _repo.GetFacturasPendientesByUsuario(usuarioId).Sum(f => f.MontoTotal);

    public int CrearFactura(Factura nueva)
    {
        if (string.IsNullOrWhiteSpace(nueva.NumeroFactura))
        {
            var sufijo = DateTime.UtcNow.ToString("yyyyMM");
            var aleatorio = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
            nueva.NumeroFactura = $"FAC-{sufijo}-{aleatorio}";
        }
        if (nueva.FechaEmision == default) nueva.FechaEmision = DateTime.Now;
        if (nueva.FechaVencimiento == default) nueva.FechaVencimiento = nueva.FechaEmision.AddDays(AppConstants.DIAS_VENCIMIENTO_FACTURA);

        var subtotal = (nueva.MontoAgua) + (nueva.MontoAlcantarillado) + (nueva.MontoBasura);
        if (nueva.Itbis <= 0) nueva.Itbis = Math.Round(subtotal * AppConstants.ITBIS_PORCENTAJE, 2, MidpointRounding.AwayFromZero);
        nueva.MontoTotal = Math.Round(subtotal + nueva.Itbis, 2, MidpointRounding.AwayFromZero);

        if (string.IsNullOrWhiteSpace(nueva.EstadoPago)) nueva.EstadoPago = "Pendiente";

        _repo.Add(nueva);
        return nueva.Id;
    }

    public bool ActualizarEstado(int facturaId, string nuevoEstado, DateTime? fechaPago = null)
    {
        var f = _repo.GetById(facturaId);
        if (f == null) return false;
        f.EstadoPago = nuevoEstado;
        if (fechaPago.HasValue) f.FechaPago = fechaPago.Value;
        _repo.Update(f);
        return true;
    }

    public bool ActualizarMontos(int facturaId, int metrosConsumidos, decimal montoAgua, decimal montoAlcantarillado, decimal montoBasura, bool recalcularItbis = true)
    {
        var f = _repo.GetById(facturaId);
        if (f == null) return false;

        f.MetrosConsumidos = metrosConsumidos;
        f.MontoAgua = montoAgua;
        f.MontoAlcantarillado = montoAlcantarillado;
        f.MontoBasura = montoBasura;

        var subtotal = montoAgua + montoAlcantarillado + montoBasura;
        if (recalcularItbis) f.Itbis = Math.Round(subtotal * AppConstants.ITBIS_PORCENTAJE, 2, MidpointRounding.AwayFromZero);
        f.MontoTotal = Math.Round(subtotal + f.Itbis, 2, MidpointRounding.AwayFromZero);

        _repo.Update(f);
        return true;
    }

    public bool AnularFactura(int facturaId)
    {
        var f = _repo.GetById(facturaId);
        if (f == null) return false;
        f.EstadoPago = "Anulada";
        _repo.Update(f);
        return true;
    }

    private static FacturaDTO Map(Factura f) => new()
    {
        Id = f.Id,
        NumeroFactura = f.NumeroFactura,
        NumeroContrato = f.Contrato?.NumeroContrato ?? "",
        FechaEmision = f.FechaEmision,
        FechaVencimiento = f.FechaVencimiento,
        MetrosConsumidos = f.MetrosConsumidos,
        MontoTotal = f.MontoTotal,
        EstadoPago = f.EstadoPago,
        EstaPagada = string.Equals(f.EstadoPago, "Pagada", StringComparison.OrdinalIgnoreCase)
    };
}

internal class PagoService : IPagoService
{
    private readonly IPagoRepository _repo;
    private readonly IFacturaRepository _facturas;
    public PagoService(IPagoRepository repo, IFacturaRepository facturas) { _repo = repo; _facturas = facturas; }

    public PagoDTO? ObtenerDetallePago(int pagoId)
        => _repo.GetById(pagoId) is { } p ? Map(p) : null;

    public bool VerificarPago(string numeroTransaccion)
        => _repo.GetByNumeroTransaccion(numeroTransaccion) != null;

    public bool ProcesarPago(int facturaId, decimal monto, string metodoPago)
    {
        var factura = _facturas.GetById(facturaId);
        if (factura == null) return false;

        var pago = new Pago
        {
            FacturaId = facturaId,
            Monto = monto,
            MetodoPago = metodoPago,
            NumeroTransaccion = $"TX-{Guid.NewGuid():N}".Substring(0, 12),
            FechaPago = DateTime.Now,
            Estado = "Completado"
        };
        _repo.Add(pago);

        // actualizar estado factura
        if (monto >= factura.MontoTotal)
        {
            factura.EstadoPago = "Pagada";
            _facturas.Update(factura);
        }
        return true;
    }

    private static PagoDTO Map(Pago p) => new()
    {
        Id = p.Id,
        NumeroFactura = p.Factura?.NumeroFactura ?? "",
        MetodoPago = p.MetodoPago,
        Monto = p.Monto,
        NumeroTransaccion = p.NumeroTransaccion,
        FechaPago = p.FechaPago,
        Estado = p.Estado
    };
}

internal class ReporteAveriaService : IReporteAveriaService
{
    private readonly IReporteAveriaRepository _repo;
    public ReporteAveriaService(IReporteAveriaRepository repo) => _repo = repo;

    public IEnumerable<ReporteAveria> ObtenerAveriasEnZona(decimal lat, decimal lon, double radioKm)
        => _repo.GetAveriasByZona(lat, lon, radioKm);

    public IEnumerable<ReporteAveria> ObtenerAveriasPorUsuario(int usuarioId)
        => _repo.GetAveriasByUsuario(usuarioId);

    public bool ActualizarEstadoAveria(int averiaId, string nuevoEstado, string comentario)
    {
        var a = _repo.GetById(averiaId);
        if (a == null) return false;
        a.Estado = nuevoEstado;
        a.ComentarioResolucion = comentario;
        if (nuevoEstado == "Resuelta" || nuevoEstado == "Cerrada")
            a.FechaResolucion = DateTime.Now;
        _repo.Update(a);
        return true;
    }

    public int CrearReporteAveria(ReporteAveria reporte)
    {
        _repo.Add(reporte);
        return reporte.Id;
    }
}

internal class NotificacionService : INotificacionService
{
    private readonly INotificacionRepository _repo;
    public NotificacionService(INotificacionRepository repo) => _repo = repo;

    public void EnviarNotificacion(int usuarioId, string titulo, string mensaje, string tipo)
        => _repo.Add(new Notificacion { UsuarioId = usuarioId, Titulo = titulo, Mensaje = mensaje, Tipo = tipo, FechaEnvio = DateTime.Now, Leida = false });

    public IEnumerable<Notificacion> ObtenerNotificacionesUsuario(int usuarioId)
        => _repo.GetByUsuario(usuarioId);

    public void MarcarComoLeida(int notificacionId)
    {
        var n = _repo.GetById(notificacionId);
        if (n == null) return;
        n.Leida = true;
        _repo.Update(n);
    }

    public int ContarNotificacionesNoLeidas(int usuarioId)
        => _repo.ContarNoLeidasByUsuario(usuarioId);
}
