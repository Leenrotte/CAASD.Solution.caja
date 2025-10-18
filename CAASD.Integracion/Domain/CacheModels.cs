namespace CAASD.Integracion;

public class UsuarioCache
{
    public int Id { get; set; }
    public string Cedula { get; set; } = "";
    public string NombreCompleto { get; set; } = "";
    public string Email { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Direccion { get; set; } = "";
    public string Rol { get; set; } = "CLIENTE"; // CLIENTE por defecto
}

public class FacturaCache
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string NumeroFactura { get; set; } = "";
    public decimal MontoTotal { get; set; }
    public bool EstaPagada { get; set; }
}

public class PagoCache
{
    public int Id { get; set; }
    public int FacturaId { get; set; }
    public string NumeroTransaccion { get; set; } = "";
    public decimal Monto { get; set; }
    public string Metodo { get; set; } = "";
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string Estado { get; set; } = "Pendiente";
}
