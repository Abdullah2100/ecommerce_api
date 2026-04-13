using api.application;
using api.domain.Interface;
using api.Presentation.dto;
using api.Presentation.dto.Response;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace api.Infrastructure.Repositories;

public class AnalyseRepository(
    AppDbContext context
) : IAnalyseRepository
{
    public async Task<AnalyzesOrderDto?> GetMonthAnalysis()
    {
        try
        {
            using (var cmd = context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM get_monthly_stats()";
                await context.Database.OpenConnectionAsync();
                AnalyzesOrderDto? info = null;
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
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
                    }
                }

                return info;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"this error from get anaylise data {ex.Message}");
            return null;
        }
    }
}