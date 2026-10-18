using Newtonsoft.Json;
using Npgsql;
using PricingService.Domain;

namespace PricingService.Data;

public sealed class PostgresDataStore : IDataStore, ITariffRepository
{
    private readonly NpgsqlDataSource dataSource;

    public PostgresDataStore(NpgsqlDataSource dataSource)
    {
        this.dataSource = dataSource;
    }

    public ITariffRepository Tariffs => this;
    public Task<Tariff> this[string productCode] => FindAsync(productCode);

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        await using (var create = new NpgsqlCommand("""
            CREATE TABLE IF NOT EXISTS pricing_tariffs (
                code text PRIMARY KEY,
                definition jsonb NOT NULL
            );
            """, connection, transaction))
        {
            await create.ExecuteNonQueryAsync(cancellationToken);
        }

        foreach (var tariff in DefaultTariffs.Create())
        {
            await using var insert = new NpgsqlCommand("""
                INSERT INTO pricing_tariffs (code, definition)
                VALUES (@code, CAST(@definition AS jsonb))
                ON CONFLICT (code) DO NOTHING;
                """, connection, transaction);
            insert.Parameters.AddWithValue("code", tariff.Code);
            insert.Parameters.AddWithValue("definition", JsonConvert.SerializeObject(tariff));
            await insert.ExecuteNonQueryAsync(cancellationToken);
        }
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task<Tariff> FindAsync(string productCode)
    {
        await using var command = dataSource.CreateCommand(
            "SELECT definition::text FROM pricing_tariffs WHERE code = @code;");
        command.Parameters.AddWithValue("code", productCode);
        var definition = await command.ExecuteScalarAsync() as string;
        if (definition is null)
            throw new KeyNotFoundException($"No tariff exists for product '{productCode}'.");
        return JsonConvert.DeserializeObject<Tariff>(definition)
            ?? throw new InvalidOperationException($"Invalid tariff document for '{productCode}'.");
    }
}

public sealed class TariffDatabaseInitializer : IHostedService
{
    private readonly PostgresDataStore dataStore;

    public TariffDatabaseInitializer(PostgresDataStore dataStore)
    {
        this.dataStore = dataStore;
    }

    public Task StartAsync(CancellationToken cancellationToken) => dataStore.InitializeAsync(cancellationToken);
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
