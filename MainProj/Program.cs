﻿using MainProj.Data;
using MainProj.Models;
using System.Diagnostics;
using Faker;

namespace MainProj
{
    internal class Program
    {
        static void Main(string[] args )
        {
            //string [] args = { "5" };
            //string[] args = { "7"};
            // Парс входящих аргументов 
            /* Диапазон ожидаемых параметров
             * 1 - Создание (инициализация) таблицы\базы данных
             * 2 - Добавление новой записи (с проверками на существование БД, дубликата), Входящий параметр
             * "ФИО ДатаРождения Пол"
             * 3 - Вывод уникальных значений по "ФИО др сортировка по ФИО" Вывод ФИО ДР пол кол-во полных лет
             * 4 - Генерация тестовых данных 1 млн записей, вывод время генерации данных
             * 5 - Выборка по фильтру (по дефолту) выходной параметр время исполнения
             * 6 - Оптимизация БД для ускорения выполнения работы.
             */
            if (args.Length > 0)
            {
                var action = args[0];
                
                switch (action)
                {
                    case "1":
                        if (!MainDBContext.IsCreated())
                        {
                            var db = new MainDBContext();
                            Console.WriteLine("Database was create.");
                        }
                        else
                            Console.WriteLine("Database already create.");
                        break;
                    case "2":
                        Console.WriteLine($"Обработка события {action}");
                        if (args.Length > 1)
                        {
                            if (MainDBContext.IsCreated())
                            {
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
                                // проверить есть ли в БД
                                using var db = new MainDBContext();

                                var collections = db.Persons!.ToList();

                                if (!collections.Contains(person))
                                {
                                    db.Persons!.Add(person);
                                    db.SaveChanges();
                                    Console.WriteLine("New person was add.");
                                    return;
                                }
                                else
                                    Console.WriteLine("Person already exsist in DB.");
                            }
                            else
                                Console.WriteLine("Database wasn't create.");
                            // ответ успешно\не успешно
                        }
                        else
                            Console.WriteLine("Без входящего параметра");
                        break;
                    case "3":
                        Console.WriteLine($"Обработка события {action}");
                        //TODO определить шаблон фильтрации записей
                        if (MainDBContext.IsCreated())
                        {
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
                        }                         
                        else
                            Console.WriteLine("Database wasn't create.");
                        break;
                    case "4":
                        Console.WriteLine($"Обработка события {action}");
                        if (MainDBContext.IsCreated())
                        {                            
                            if(!(args.Length == 2 && int.TryParse(args[1], out int count)))
                            {
                                count = 1000000;
                            }                      
                            using var db = new MainDBContext();
                            var sw = Stopwatch.StartNew();
                            List<Person> persons = new List<Person>();
                            var rnd = new Random();

                            Person person;
                            int curent = 0; 
                            while(curent < count)
                            {
                                var prefix = Faker.Name.Prefix();
                                switch (prefix)
                                {
                                    case "Mr.":
                                        prefix = "Male";
                                        break;
                                    case "Mrs.":
                                    case "Miss":
                                    case "Ms.":
                                        prefix = "Female";
                                        break;
                                    default:
                                        prefix = "Unknow";
                                        break;
                                }
                                person = new Person
                                {
                                    LastName = Name.Last(),
                                    FirstName = Name.First(),
                                    MiddleName = Name.Middle(),
                                    Bithday = DateTime.Now.AddDays(-rnd.Next(6570, 36500)),
                                    Gender = prefix
                                };
                                persons.Add(person);
                                person = null!;
                                curent++;
                                //TODO Проверка на дубликат при добавлении в список выкл
                                //if (!persons.Contains(person))
                                //{
                                //    persons.Add(person);
                                //    person = null!;
                                //    curent++;
                                //}

                            }
                            db.Persons!.AddRange(persons);
                            db.SaveChanges();
                            Console.WriteLine($"Выполнено за {sw.Elapsed.TotalSeconds}");
                        }
                        else
                            Console.WriteLine("Database wasn't create.");

                        break;
                    case "5":
                        Console.WriteLine($"Обработка события {action}");
                        if (MainDBContext.IsCreated())
                        {
                            using var db = new MainDBContext();
                            var sw = Stopwatch.StartNew();
                            
                            var res = db.Persons!.Where(x => x.Gender == "Male" && x.LastName.StartsWith("F") && x.FirstName.StartsWith("F") && x.MiddleName.StartsWith("F")).ToArray();
                            Console.WriteLine("Время выборки: " + sw.Elapsed.TotalSeconds);
                            Console.WriteLine("Число найденных записей" + " " + res.Length);
                        }
                        else
                            Console.WriteLine("Database wasn't create.");
                        break;
                    case "6":
                        Console.WriteLine($"Обработка события {action}");
                        break;
                    case "7":
                        Console.WriteLine($"Удаление БД и {action}");
                        if (MainDBContext.IsCreated())
                        {                            
                            try
                            {
                                new MainDBContext().EnsureDeleteDB();
                                Console.WriteLine("Успешно.");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
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

        private static int GetFull(DateTime bithday)
        {
            var year = DateTime.Now.Year - bithday.Year;
            if (DateTime.Now.Month - bithday.Month <= 0)
                if (DateTime.Now.Day - bithday.Day < 0)
                    year--;           
            //TODO Доделать если нужно дать возраст с учетом времени
            return year;
        }

        private static string [] ParseInputString(string[] args)
        {
            string[] temp = new string[5];
            Person person = new Person();
            temp[4] = args[3];
            temp[3] = args[2];
            string[] FIO = ParseInputParam(args[1], temp);
            return temp;
        }

        private static string[] ParseInputParam(string v, string[] temp)
        {
            int index = 0;
            
            List<char> list = new List<char>();
            list.Add(v[0]);
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