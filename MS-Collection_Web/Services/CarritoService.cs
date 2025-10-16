using MS_Collection_WbApi.Models;
using MS_Collection_Web.Models;

public class CarritoService
{
    public List<CarritoItem> Carrito { get; set; } = new();

    public void AgregarItem(Producto producto)
    {
        var existente = Carrito.FirstOrDefault(p => p.ProductoId == producto.Id);
        int cantidadEnCarrito = existente?.Cantidad ?? 0;

        // ✅ No permitir agregar si ya se alcanzó el stock disponible
        if (cantidadEnCarrito >= producto.StockActual)
        {
            return;
        }

        if (existente != null)
        {
            existente.Cantidad++;
        }
        else
        {
            Carrito.Add(new CarritoItem
            {
                Producto = producto,
                ProductoId = producto.Id,
                Nombre = producto.Nombre,
                PrecioUnitario = (decimal)producto.PrecioVenta,
                Cantidad = 1,
                ImagenURL = producto.ImagenURL,
                Codigo = producto.Codigo
            });
        }
    }

    public int ObtenerCantidadEnCarrito(int productoId)
    {
        return Carrito.FirstOrDefault(p => p.ProductoId == productoId)?.Cantidad ?? 0;
    }


    public void EliminarProducto(int productoId)
    {
        if (Carrito == null)
        {
            Console.WriteLine("Carrito es null");
            return;
        }

        var item = Carrito.FirstOrDefault(p => p.ProductoId == productoId);
        if (item != null)
        {
            Carrito.Remove(item);
        }
    }

    public List<CarritoItem> ObtenerProductos()
    {
        return Carrito;
    }

    public decimal CalcularTotal()
    {
        return Carrito.Sum(item => item.PrecioUnitario * item.Cantidad);
    }

    public decimal TotalCarrito()
    {
        return CalcularTotal(); // o eliminá este método si no lo usás
    }

    public void VaciarCarrito() => Carrito.Clear();
}

public class CarritoItem
{
    public Producto Producto { get; set; }
    public int ProductoId { get; set; }
    public string Nombre { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string ImagenURL { get; set; } = null!;
    public string Codigo { get; set; } = null!;
    public decimal Subtotal => Cantidad * PrecioUnitario;
}
