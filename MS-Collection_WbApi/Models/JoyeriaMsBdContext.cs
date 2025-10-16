using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MS_Collection_WbApi.Models;

public partial class JoyeriaMsBdContext : DbContext
{
    public JoyeriaMsBdContext()
    {
    }

    public JoyeriaMsBdContext(DbContextOptions<JoyeriaMsBdContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categorium> Categoria { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<DetalleCompra> DetalleCompras { get; set; }

    public virtual DbSet<DetallePedido> DetallePedidos { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<Inventario> Inventarios { get; set; }

    public virtual DbSet<MetodosDePago> MetodosDePagos { get; set; }

    public virtual DbSet<PagoFactura> PagoFacturas { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedore> Proveedores { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categorium>(entity =>
        {
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.ToTable("Compra");

            entity.Property(e => e.FechaCompra).HasColumnType("datetime");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.Compras)
                .HasForeignKey(d => d.ProveedorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Compra_Proveedores");
        });

        modelBuilder.Entity<DetalleCompra>(entity =>
        {
            entity.ToTable("DetalleCompra");

            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Compra).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.CompraId)
                .HasConstraintName("FK_DetalleCompra_Compra");

            entity.HasOne(d => d.Producto).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("FK_DetalleCompra_Producto");
        });


        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.ToTable("DetallePedido");

            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Pedido).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetallePedido_Pedidos");

            entity.HasOne(d => d.Producto).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetallePedido_Producto");
        });

        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.ToTable("Inventario");

            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");

            entity.HasOne(d => d.Producto).WithMany(p => p.Inventarios)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventario_Producto");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Inventarios)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventario_Usuarios");
        });

        modelBuilder.Entity<MetodosDePago>(entity =>
        {
            entity.ToTable("Metodos de pago");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PagoFactura>(entity =>
        {
            entity.ToTable("PagoFactura");

            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Factura).WithMany(p => p.PagoFacturas)
                .HasForeignKey(d => d.FacturaId)
                .HasConstraintName("FK_PagoFactura_Factura");

            entity.HasOne(d => d.MetodoPago).WithMany(p => p.PagoFacturas)
                .HasForeignKey(d => d.MetodoPagoId)
                .HasConstraintName("FK_PagoFactura_Metodos de pago1");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Producto");

            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Descripción).HasColumnType("text");
            entity.Property(e => e.Material)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PrecioVenta).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ImagenURL)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_Categoria1");
        });

        modelBuilder.Entity<Proveedore>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Contacto)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.Property(e => e.Contraseña)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Rol)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);

        // Relación entre Pedido y Usuario
        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.Usuario)      // Un Pedido tiene un Usuario
            .WithMany(u => u.Pedidos)    // Un Usuario tiene muchos Pedidos
            .HasForeignKey(p => p.ClienteId)  // Relación por la columna ClienteId
            .OnDelete(DeleteBehavior.SetNull);  // Si se elimina el Usuario, no se elimina el Pedido
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
