using MainProj.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;

namespace MainProj.Data
{
    internal class MainDBContext : DbContext
    {
        private bool isCreated { get; set; }
        public DbSet<Person>? Persons { get; set; } 
        public MainDBContext()
        {
            isCreated = IsCreated();
            if (!isCreated)
            {
                Database.EnsureCreated();                
                ChangeCreatedDBStatus(isCreated: true);
            }
        }
        

        private void ChangeCreatedDBStatus(bool isCreated)
        {
            if (isCreated)
            {
                isCreated = true;
                string path = "settings.txt";
                using var sw = new StreamWriter(path);
                sw.Write("true");
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=LetovTestDB;");
        }

        internal bool EnsureDeleteDB()
        {
            if (isCreated)
            {
                //удалить бд
                Database.EnsureDeleted();
                // изменить флаг
                isCreated = false;
                // записать false в файл
                using var sw = new StreamWriter("settings.txt", false);
                sw.Write("false");
                return true;
            }
            else
                throw new ArgumentException("Database wasn't creating.");
        }

        internal static bool IsCreated()
        {
            string path = "settings.txt";
            // прочесть настройки, если нет или false создаем
            if (File.Exists(path) && File.ReadAllText(path) == "true")
                return true;            
            // если true не создаем
            return false;
        }

        
    }
}
