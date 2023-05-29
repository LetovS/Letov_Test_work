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
            isCreated = true;
            var ttt = Environment.CurrentDirectory.Split('\\');
            var k = ttt.Take(ttt.Length - 3).ToArray();
            string path = "\\settings.txt";
            var res = string.Join("\\", k) + path;
            using var sw = new StreamWriter(res);
            sw.Write("true");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=LetovTestDB;");
        }

        internal void EnsureDeleteDB()
        {
            if (isCreated)
            {
                //удалить бд
                Database.EnsureDeleted();
                // изменить флаг
                isCreated = false;
                // записать false в файл
                var ttt = Environment.CurrentDirectory.Split('\\');
                var k = ttt.Take(ttt.Length - 3).ToArray();
                string path = "\\settings.txt";
                var res = string.Join("\\", k) + path;
                using var sw = new StreamWriter(res, false);
                sw.Write("false");
            }
            else
                throw new ArgumentException("Database wasn't creating.");
        }

        internal static bool IsCreated()
        {
            var ttt = Environment.CurrentDirectory.Split('\\');
            var k = ttt.Take(ttt.Length - 3).ToArray();
            string path = "\\settings.txt";
            var res = string.Join("\\", k) + path;
            // прочесть настройки, если нет или false создаем
            if (File.Exists(res) && File.ReadAllText(res) == "true")
                return true;            
            // если true не создаем
            return false;
        }
    }
}
