using MainProj.Data;
using MainProj.Models;
using System.Diagnostics;
using Faker;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Data.SqlClient;
using MainProj.Infrastructure.Compapeps;

namespace MainProj
{
    internal class Program
    {
        static void Main(string[] args )
        {
            // Парс входящих аргументов 
            /* Диапазон ожидаемых параметров
             * 1 - Создание (инициализация) таблицы\базы данных  реализовать создание таблицы в ручную с указанием макс длины в виде атрибутов в моделе
             * 2 - Добавление новой записи (с проверками на существование БД, дубликата), Входящий параметр
             * "ФИО ДатаРождения Пол"
             * 3 - Вывод уникальных значений по "ФИО др сортировка по ФИО" Вывод ФИО ДР пол кол-во полных лет
             * 4 - Генерация тестовых данных 1 млн записей, вывод время генерации данных ** Реализовать генерацию 100 записей с ФИО Ф... с полом male
             * 5 - Выборка по фильтру (по дефолту) выходной параметр время исполнения
             * 6 - Оптимизация БД для ускорения выполнения работы добавить индексы в БД
             */
            if (args.Length > 0)
            {
                var action = args[0];
                
                switch (action)
                {
                    case "1":
                        using (var db = new MainDBContext(true))
                        {
                            Console.WriteLine("Database was create.");
                        }
                        break;
                    case "2":
                        if (args.Length > 1)
                        {
                            //TODO где то нужно проверять строку на валидность
                            // распарсить входящую строку
                            string[] values = ParseInputString(args);                           
                            DateTime date = DateTime.Parse(values[3]);
                            // создать персону
                            var person = new Person()
                            {
                                LastName = values[0],
                                FirstName = values[1],
                                MiddleName = values[2],
                                Bithday = date,
                                Gender = values[4]
                            };
                            using var db = new MainDBContext();
                            var collections = db.Persons!.ToList();
                            db.Persons!.Add(person);
                            db.SaveChanges();
                            Console.WriteLine("New person was add.");
                        }
                        else
                            Console.WriteLine("Некоректная строка.");
                        break;
                    case "3":
                        Console.WriteLine($"Вывод уникальных записей");
                        
                        using (var db = new MainDBContext())
                        {
                            var persons = db.Persons!.ToList();
                            if (persons.Count > 0)
                            {
                                var dataOrdersByLFM_birthday = db.Persons!
                                                        .OrderBy(x => x.LastName)
                                                        .ThenBy(x => x.FirstName)
                                                        .ThenBy(x => x.MiddleName)
                                                        .ToList()
                                                        .Distinct(new PersonComparer());
                                foreach (var person in dataOrdersByLFM_birthday)
                                {
                                    Console.WriteLine($"{person.LastName} {person.FirstName} {person.MiddleName} {person.Bithday:dd.MM.yyyy} {person.Gender} {GetFull(person.Bithday)}");
                                }
                                return;
                            }
                            Console.WriteLine("Пользователей нет.");
                        }                        
                        break;
                    case "4":
                        Console.WriteLine($"Генерация тестовых данных в базе данных.");                                                   
                        if(!(args.Length == 2 && int.TryParse(args[1], out int count)))
                        {
                            count = 1000000;
                        }
                        using (MainDBContext db = new ())
                        {
                            if (db.Persons != null)
                            {
                                var sw = Stopwatch.StartNew();
                                List<Person> persons = new();
                                var rnd = new Random();
                                //TODO обдумать над использованием 
                                Person person;
                                int curent = 0;
                                while (curent < count)
                                {
                                    person = new Person
                                    {
                                        LastName = Name.Last(),
                                        FirstName = Name.First(),
                                        MiddleName = Name.Middle(),
                                        Bithday = GetDateBirthday(rnd.Next(18, 30), rnd.Next(10), rnd.Next(15)),
                                        Gender = GetRndGender()
                                    };
                                    persons.Add(person);
                                    person = null!;
                                    curent++;
                                }
                                db.Persons!.AddRange(persons);
                                db.SaveChanges();
                                persons?.Clear();
                                person = null!;
                                for (int i = 0; i < 1000; i++)
                                {
                                    person = new Person
                                    {
                                        LastName = "F" + Guid.NewGuid().ToString()[0..5],
                                        FirstName = "F" + Guid.NewGuid().ToString()[0..5],
                                        MiddleName = "F" + Guid.NewGuid().ToString()[0..5],
                                        Gender = "Male",
                                        Bithday = DateTime.Parse(DateTime.Now.ToShortDateString()),
                                    };
                                    persons!.Add(person);
                                }
                                db.AddRange(persons!);
                                Console.WriteLine("Сохранение в БД");
                                db.SaveChanges();
                                Console.WriteLine($"Выполнено за {sw.Elapsed.TotalSeconds}");
                            }
                        }
                        break;
                    case "5":
                        using (var db = new MainDBContext())
                        {
                            var sw = Stopwatch.StartNew();
                            var res = db
                                        .Persons!
                                        .Where(x =>
                                                    x.Gender == "Male" &&
                                                    x.LastName.StartsWith("F")
                                                    && x.FirstName.StartsWith("F")
                                                    && x.MiddleName.StartsWith("F"))
                                        .ToArray();
                            Console.WriteLine("Время выборки: " + sw.Elapsed.TotalSeconds);
                            Console.WriteLine("Число найденных записей" + " " + res.Length);
                        }
                        break;
                    case "6":
                        // надо как то проверить наличие функции, мб использовать SqlCommand SqlConnect
                        using (var db = new MainDBContext())
                        {
                            //проверка существования процедуры IsCheck
                            //TODO Проработать вопрос работы с функцией БД Object_Id()


                            //скрипт создания процедуры
                            db.Database.ExecuteSqlRaw(@"create procedure IsCheck
                                @res int output
                                as
                                begin
                                declare @k as int
                                select @k =Count(*) from sys.indexes as si
                                where si.name = 'ix_test'
                                if(@k = 1)
                                set @res = @k
                                else
                                set @res = @k
                                end");

                            //проверка существования составного индекса
                            // создаем параметр
                            SqlParameter param = new()
                            {
                                ParameterName = "@res",
                                SqlDbType = System.Data.SqlDbType.Int,
                                Direction = System.Data.ParameterDirection.Output
                            };
                            
                            //выполняем запрос к бд                            
                            db.Database.ExecuteSqlRaw("IsCheck @res OUT", param);
                            if (param.Value is Int32 res)
                            {
                                //value = 1 значит в табл индексов есть 1 запись с таким названием
                                if (res == 1)
                                {
                                    Console.WriteLine("Добавлен индекс");
                                }
                                else
                                {
                                    //если нет то отправялем sql на создание индекса
                                    db.Database.ExecuteSqlRaw(@"CREATE NONCLUSTERED INDEX ix_test on dbo.Persons(Gender, LastName, FirstName, MiddleName)");
                                }
                            }
                            Console.WriteLine("Успешно");
                        }
                        break;
                    case "7":
                        Console.WriteLine($"Удаление БД и {action}");
                        using (var db = new MainDBContext())
                        {
                            db.Database.EnsureDeleted();
                        }
                        break;
                    default:
                        Console.WriteLine("Вы выбрали неизвестное действие.");
                        break;
                }
            }
            else
                Console.WriteLine("No action set");
        }
        /// <summary>
        /// Генератор пола.
        /// </summary>
        /// <returns>Пол.</returns>
        private static string GetRndGender()
        {
            var rnd = new Random();
            var res = rnd.Next(2);
            if (res == 1)
                return "Male";
            return "Female";
        }
        /// <summary>
        /// Генерация даты рождения.
        /// </summary>
        /// <param name="year">Год.</param>
        /// <param name="months">Месяц.</param>
        /// <param name="days">День.</param>
        /// <returns>Полученная дата рождения.</returns>
        private static DateTime GetDateBirthday(int year, int months, int days)
        {
            var t = DateTime.Now;
            var temp = t.AddYears(-year);
            temp = temp.AddMonths(-months);
            temp = temp.AddDays(-days);
            return temp;
        }
        /// <summary>
        /// Вычисление полных лет.
        /// </summary>
        /// <param name="bithday">Дата отсчета.</param>
        /// <returns>Полных лет.</returns>
        private static int GetFull(DateTime bithday)
        {
            var year = DateTime.Now.Year - bithday.Year;
            if (DateTime.Now.Month - bithday.Month <= 0)
                if (DateTime.Now.Day - bithday.Day < 0)
                    year--;           
            //TODO Доделать если нужно дать возраст с учетом времени
            return year;
        }
        /// <summary>
        /// Парс данных.
        /// </summary>
        /// <param name="args">Даные.</param>
        /// <returns>Шаблон для генерации персоны.</returns>
        private static string[] ParseInputString(string[] args)
        {
            string[] temp = new string[5];
            //Person person = new Person();
            temp[4] = args[3];
            temp[3] = args[2];
            _ = ParseInputParam(args[1], temp);
            return temp;
        }
        /// <summary>
        /// Разбиение на ФИО
        /// </summary>
        /// <param name="v"></param>
        /// <param name="temp"></param>
        /// <returns></returns>
        private static string[] ParseInputParam(string v, string[] temp)
        {
            int index = 0;
            List<char> list = new()
            {
                v[0]
            };
            for (int i = 1; i < v.Length; i++)
            {
                if (!char.IsUpper(v[i]))
                    list.Add(v[i]);
                else
                {
                    temp[index] = string.Join("", list);
                    list.Clear();
                    list.Add(v[i]);
                    index++;
                }
            }
            temp[index] = string.Join("", list);
            return temp;
        }
    }
}