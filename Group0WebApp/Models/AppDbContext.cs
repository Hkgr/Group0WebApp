using Group_WebApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public virtual DbSet<User>? Users { get; set; }
    public virtual DbSet<Group_WebApp.Models.File>? Files { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Group0WebApp_DB;Integrated Security=SSPI; TrustServerCertificate=true");
    //}
}

 
