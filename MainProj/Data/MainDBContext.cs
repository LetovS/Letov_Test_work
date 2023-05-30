using MainProj.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace MainProj.Data
{
    internal class MainDBContext : DbContext
    {
        public DbSet<Person>? Persons { get; set; } 
        public MainDBContext(bool isCreatedDB = false)
        {
            if (isCreatedDB)
            {
                Database.EnsureCreated();                
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=LetovTestDB;");
        }
    }
}
