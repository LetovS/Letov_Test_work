using MainProj.Data;

namespace MainProj
{
    internal class Program
    {
        static void Main(string[] args )
        {
            //string[] args = { "7" };
            
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
                        if (args.Length == 2)
                        {
                            Console.WriteLine("Входящий параметр есть: " + args[1]);
                        }
                        else
                            Console.WriteLine("Без входящего параметра");
                        break;
                    case "3":
                        Console.WriteLine($"Обработка события {action}");
                        //TODO определить шаблон фильтрации записей
                        if (args.Length == 2)
                        {
                            Console.WriteLine("Входящий параметр фильтра: " + args[1]);
                        }
                        else
                            Console.WriteLine("Номинальный фильтр: \"ФИО+др\"");
                        Console.WriteLine("Выводятся данные согласно фильтру");
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

        private static void CreatedDB()
        {
            using var db = new MainDBContext();
        }
    }
}