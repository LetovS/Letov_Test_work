using MainProj.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace MainProj.Data
{
    internal class MainDBContext : DbContext
    {
        private bool IsCreated { get; set; } = false;
        public DbSet<Person> Persons { get; set; }
        public MainDBContext()
        {
            IsCreated = CheckIsCreatedDB();
            if (!IsCreated)
            {
                Database.EnsureCreated();
                //TODO записать в файл настроек тру
                ChangeCreatedDBStatus(isCreated:true);
            }
        }
        /// <summary>
        /// Проверка создания БД.
        /// </summary>
        /// <returns>bool</returns>
        private bool CheckIsCreatedDB()
        {
            if (File.Exists("settings.txt"))
            {
                using var sr = new StreamReader("settings.txt");
                if (sr.ReadToEnd() == "true") return true;
            }
            return false;
        }

        private void ChangeCreatedDBStatus(bool isCreated)
        {
            if (isCreated)
            {                
                string path = "settings.txt";
                using var sw = new StreamWriter(path);
                sw.Write("true");
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=LetovTestDB;");
        }

        internal void EnsureDeleteDB()
        {
            //удалить бд
            Database.EnsureDeleted();
            // изменить флаг
            IsCreated = false;
            // записать false в файл
            using var sw = new StreamWriter("settings.txt");
            sw.Write("false");
        }
    }
}
