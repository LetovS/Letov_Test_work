using MainProj.Data;
using MainProj.Models;
using System.Diagnostics;
using Faker;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Data.SqlClient;

namespace MainProj
{
    internal class Program
    {
        static void Main(string[] args )
        {
            //string[] args = { "6"};
            //string[] args = { "2", "LetovSergeyMichailovich", "30.10.1987", "Male" };
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
                        using (var db = new MainDBContext())
                        {
                            Console.WriteLine("Database was create.");
                        }
                        break;
                    case "2":
                        Console.WriteLine($"Обработка события {action}");
                        if (args.Length > 1)
                        {
                            //TODO где то нужно проверять строку на валидность
                            // распарсить входящую строку
                            string[] values = ParseInputString(args);
                            // создать персону
                            DateTime date = DateTime.Parse(values[3]);
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
                            Console.WriteLine("Без входящего параметра");
                        break;
                    case "3":
                        Console.WriteLine($"Обработка события {action}");
                        //TODO определить шаблон фильтрации записей
                        if (args.Length == 2)
                        {
                            //TODO Реализовать соритровку по дате рождения, полу
                            //TODO обдумать где выполнять сортировку (в БД\в приложение)
                            Console.WriteLine("Входящий параметр фильтра: " + args[1]);
                        }
                        else
                        {
                            Console.WriteLine("Номинальный фильтр: \"ФИО+др\"");
                            using var db = new MainDBContext();
                            var dataOrdersByLFM_birthday = db.Persons!
                                                            .Distinct()
                                                            .OrderBy(x => x.LastName)
                                                            .ThenBy(x => x.FirstName)
                                                            .ThenBy(x => x.MiddleName)
                                                            .ToList();
                            Console.WriteLine("Выводятся данные согласно фильтру");
                            foreach (var person in dataOrdersByLFM_birthday)
                            {
                                Console.WriteLine($"{person.LastName} {person.FirstName} {person.MiddleName} {person.Bithday:dd.MM.yyyy} {person.Gender} {GetFull(person.Bithday)}");
                            }
                        }
                        break;
                    case "4":
                        Console.WriteLine($"Обработка события {action}");                                                   
                        if(!(args.Length == 2 && int.TryParse(args[1], out int count)))
                        {
                            count = 1000000;
                        }
                        using (MainDBContext db = new ())
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
                            persons.Clear();
                            
                            person = null!;
                            for (int i = 0; i < 100000; i++)
                            {
                                person = new Person
                                {
                                    LastName = "F" + Guid.NewGuid().ToString()[0..5],
                                    FirstName = "F" + Guid.NewGuid().ToString()[0..5],
                                    MiddleName = "F" + Guid.NewGuid().ToString()[0..5],
                                    Gender = "Male",
                                    Bithday = DateTime.Parse(DateTime.Now.ToShortDateString()),
                                };
                                persons.Add(person);
                            }
                            db.AddRange(persons);
                            Console.WriteLine("Сохранение в БД");
                            db.SaveChanges();
                            Console.WriteLine($"Выполнено за {sw.Elapsed.TotalSeconds}");
                        }
                        break;
                    case "5":
                        Console.WriteLine($"Обработка события {action}");
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
                        Console.WriteLine($"Обработка события {action}");
                        using (var db = new MainDBContext())
                        {
                            //проверка существования процедуры
                            SqlParameter param2 = new SqlParameter()
                            {
                                ParameterName = "IsCheck"
                            };
                            try
                            {
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
                            }
                            catch (Exception)
                            {

                            }

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
                        if (MainDBContext.IsCreated())
                        {                            
                            try
                            {
                                new MainDBContext().EnsureDeleteDB();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                Console.WriteLine("Успешно.");
                            }
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

        private static string GetRndGender()
        {
            var rnd = new Random();
            var res = rnd.Next(2);
            if (res == 1)
                return "Male";
            return "Female";
        }

        private static DateTime GetDateBirthday(int year, int months, int days)
        {
            var t = DateTime.Now;
            var temp = t.AddYears(-year);
            temp = temp.AddMonths(-months);
            temp = temp.AddDays(-days);
            return temp;
        }
        private static int GetFull(DateTime bithday)
        {
            var year = DateTime.Now.Year - bithday.Year;
            if (DateTime.Now.Month - bithday.Month <= 0)
                if (DateTime.Now.Day - bithday.Day < 0)
                    year--;           
            //TODO Доделать если нужно дать возраст с учетом времени
            return year;
        }

        private static string[] ParseInputString(string[] args)
        {
            string[] temp = new string[5];
            //Person person = new Person();
            temp[4] = args[3];
            temp[3] = args[2];
            _ = ParseInputParam(args[1], temp);
            return temp;
        }

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