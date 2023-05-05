﻿namespace MainProj
{
    internal class Program
    {
        static void Main(string[] args)
        {
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
                        Console.WriteLine($"Обработка события {action}");
                        break;
                    case "2":
                        Console.WriteLine($"Обработка события {action}");
                        break;
                    case "3":
                        Console.WriteLine($"Обработка события {action}");
                        break;
                    case "4":
                        Console.WriteLine($"Обработка события {action}");
                        break;
                    case "5":
                        Console.WriteLine($"Обработка события {action}");
                        break;
                    case "6":
                        Console.WriteLine($"Обработка события {action}");
                        break;
                    default:
                        Console.WriteLine("Вы выбрали неизвестное действие.");
                        break;
                }
            }
            else
                Console.WriteLine("No action set");

        }
    }
}