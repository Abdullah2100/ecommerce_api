using api.application;
using data.Interface;
using data.dto.Response;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace data.Repositories;

/// <summary>
/// Repository implementation for performing data analysis and retrieving statistics.
/// </summary>
/// <param name="context">The database context used for data access.</param>
public class AnalyseRepository(
    AppDbContext context
) : IAnalyseRepository
{
    /// <summary>
    /// Retrieves statistical data for the current month by executing the <c>get_monthly_stats()</c> database function.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation, returning an <see cref="AnalyzesOrderDto"/>
    /// containing monthly statistics, or <c>null</c> if an error occurs or no data is found.
    /// </returns>
    public async Task<AnalyzesOrderDto?> GetMonthAnalysis()
    {
        try
        {
            await using var cmd = context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT * FROM get_monthly_stats()";
            await context.Database.OpenConnectionAsync();
            AnalyzesOrderDto? info = null;
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows) return info;
            if (await reader.ReadAsync())
            {
                info = new AnalyzesOrderDto
                {
                    TotalFee = (decimal?)reader["totalFee"],
                    TotalOrders = (long?)reader["totalOrder"],
                    TotalDeliveryDistance = (decimal?)reader["totalDeliveryDistance"],
                    UsersCount = (long)reader["userCount"],
                    ProductCount = (long)reader["productcount"],
                };
            }

            return info;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"this error from get anaylise data {ex.Message}");
            return null;
        }
    }
}