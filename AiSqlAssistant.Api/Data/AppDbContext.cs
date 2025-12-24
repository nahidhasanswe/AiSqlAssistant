using AiSqlAssistant.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AiSqlAssistant.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
}