using Npgsql;

namespace MVC.Data;

public sealed class DatabaseConnection
{
    private readonly NpgsqlDataSource _dataSource;

    public DatabaseConnection(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public ValueTask<NpgsqlConnection> OpenConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        return _dataSource.OpenConnectionAsync(cancellationToken);
    }
}