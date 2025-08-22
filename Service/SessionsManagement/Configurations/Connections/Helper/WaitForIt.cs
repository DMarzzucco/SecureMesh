using System;
using Npgsql;

namespace SessionsManagement.Configurations.Connections.Helper;

public static class WaitForIt
{
    private const int DefaultMaxRetries = 10;
    private static readonly TimeSpan DefaultDelay = TimeSpan.FromSeconds(6);

    public static async Task<bool> WaitForDatabaseAsync
    (
        string connectionString,
        ILogger logger,
        int maxRetries = DefaultMaxRetries,
        TimeSpan? delay = null
    )
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString), "Connection string not be null or empty");

        delay ??= DefaultDelay;

        for (int i = 1; i <= maxRetries; i++)
        {
            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                logger.LogInformation("connection successufully");
                return true;
            }
            catch (NpgsqlException)
            {
                logger.LogWarning($"Attempt {i}/{maxRetries} fails: Retry in {delay.Value.TotalSeconds} secons.");
                await Task.Delay(delay.Value);
            }
            catch (Exception ex)
            {
                logger.LogError($"Attemp {i}/{maxRetries} fails: Exceptions: {ex.Message}");
                throw;
            }
        }
        logger.LogError($"Could not connected to database after {maxRetries} retries.");
        return false;
    }
}
