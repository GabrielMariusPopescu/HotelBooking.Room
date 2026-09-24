namespace Room.Persistence;

public class RoomDbContext(DbContextOptions<RoomDbContext> options): DbContext(options)
{
    public DbSet<Domain.Models.Room> Rooms => Set<Domain.Models.Room>();

    public DbSet<Booking> Bookings => Set<Booking>();

    public DbSet<BookingItem> BookingItems => Set<BookingItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new RoomConfiguration());
        modelBuilder.ApplyConfiguration(new BookingConfiguration());
        modelBuilder.ApplyConfiguration(new BookingItemConfiguration());
    }
}