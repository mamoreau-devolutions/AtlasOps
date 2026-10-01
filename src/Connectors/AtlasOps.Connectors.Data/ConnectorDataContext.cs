namespace AtlasOps.Connectors.Data;

using Microsoft.EntityFrameworkCore;

public sealed class ConnectorDataContext(DbContextOptions<ConnectorDataContext> options) : DbContext(options)
{
    public DbSet<ConnectorStateEntity> ConnectorStates => this.Set<ConnectorStateEntity>();

    public DbSet<ConnectorCheckpointEntity> ConnectorCheckpoints => this.Set<ConnectorCheckpointEntity>();

    public DbSet<ConnectorOutboxEntity> ConnectorOutbox => this.Set<ConnectorOutboxEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConnectorStateEntity>(
            entity =>
            {
                entity.HasKey(static item => item.Id);
                entity.HasIndex(static item => item.ConnectorId).IsUnique();
                entity.Property(static item => item.ConnectorId).HasMaxLength(120);
                entity.Property(static item => item.DisplayName).HasMaxLength(160);
                entity.Property(static item => item.ConfigurationJson).HasColumnType("TEXT");
            });

        modelBuilder.Entity<ConnectorCheckpointEntity>(
            entity =>
            {
                entity.HasKey(static item => item.Id);
                entity.HasIndex(static item => new { item.ConnectorId, item.Partition }).IsUnique();
                entity.Property(static item => item.ConnectorId).HasMaxLength(120);
                entity.Property(static item => item.Partition).HasMaxLength(200);
                entity.Property(static item => item.Cursor).HasMaxLength(2_000);
            });

        modelBuilder.Entity<ConnectorOutboxEntity>(
            entity =>
            {
                entity.HasKey(static item => item.Id);
                entity.HasIndex(static item => new { item.ProcessedAt, item.OccurredAt });
                entity.Property(static item => item.EventType).HasMaxLength(200);
                entity.Property(static item => item.PayloadJson).HasColumnType("TEXT");
            });
    }
}

public sealed class ConnectorStateEntity
{
    public Guid Id { get; set; }

    public required string ConnectorId { get; set; }

    public required string DisplayName { get; set; }

    public required string ConfigurationJson { get; set; }

    public bool Enabled { get; set; }

    public long Revision { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class ConnectorCheckpointEntity
{
    public Guid Id { get; set; }

    public required string ConnectorId { get; set; }

    public required string Partition { get; set; }

    public required string Cursor { get; set; }

    public long Revision { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class ConnectorOutboxEntity
{
    public Guid Id { get; set; }

    public required string EventType { get; set; }

    public required string PayloadJson { get; set; }

    public DateTimeOffset OccurredAt { get; set; }

    public DateTimeOffset? ProcessedAt { get; set; }

    public int AttemptCount { get; set; }
}
