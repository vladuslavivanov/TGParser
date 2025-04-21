using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TGParser.DAL.Models;

namespace TGParser.DAL;

public class DataContext : DbContext
{
    public DataContext() : base() { }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }
    }

    #region DbSets

    /// <summary>
    /// Пользователи.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Товары.
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// Пресеты.
    /// </summary>
    public DbSet<Preset> Presets { get; set; }

    /// <summary>
    /// Пользовательские пресеты.
    /// </summary>
    public DbSet<UserPreset> UserPresets { get; set; }

    /// <summary>
    /// Просмотренные товары.
    /// </summary>
    public DbSet<UserViewedItems> UserViewedItems { get; set; }

    /// <summary>
    /// Пользовательские прокси.
    /// </summary>
    public DbSet<Proxy> Proxies { get; set; }

    /// <summary>
    /// Пользовательские прокси.
    /// </summary>
    public DbSet<UserProxy> UserProxies { get; set; }

    /// <summary>
    /// Платежи.
    /// </summary>
    public DbSet<Invoice> Invoices { get; set; }

    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var configNamespace = "TGParser.DAL.ModelConfigs";
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),
            type => type.Namespace != null && type.Namespace.StartsWith(configNamespace));

        base.OnModelCreating(modelBuilder);

        //var currentNamespace = GetType().Namespace!;
        //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(), type => type.Namespace!.Contains(currentNamespace));
        //base.OnModelCreating(modelBuilder);
    }
}
