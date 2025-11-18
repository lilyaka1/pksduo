using System;
using System.Collections.Generic;

namespace WarehouseManagement
{
    class Program
    {
        static WarehouseSystem system;
        static Warehouse coldWarehouse;
        static Warehouse sortingWarehouse;
        static Warehouse generalWarehouse;
        static Warehouse disposalWarehouse;
        static int productIdCounter = 1;

        static void Main(string[] args)
        {
            Console.WriteLine("СИСТЕМА СКЛАДСКОГО УЧЕТА\n");

            // Инициализация системы
            InitializeSystem();

            // Основной цикл меню
            bool running = true;
            while (running)
            {
                ShowMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewWarehouses();
                        break;
                    case "2":
                        DeliverProducts();
                        break;
                    case "3":
                        OptimizeWarehouses();
                        break;
                    case "4":
                        MoveExpiredProducts();
                        break;
                    case "5":
                        AnalyzeNetwork();
                        break;
                    case "6":
                        CalculateValues();
                        break;
                    case "7":
                        ManualMove();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("\nЗавершение работы программы.");
                        break;
                    default:
                        Console.WriteLine("\nНеверный выбор. Попробуйте снова.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }

        static void InitializeSystem()
        {
            system = new WarehouseSystem();

            // Создаем склады
            coldWarehouse = new Warehouse(1, "холодный", 1000, "ул. Холодная, д. 1");
            sortingWarehouse = new Warehouse(2, "сортировочный", 1500, "ул. Центральная, д. 2");
            generalWarehouse = new Warehouse(3, "общий", 2000, "ул. Складская, д. 3");
            disposalWarehouse = new Warehouse(4, "утилизация", 500, "ул. Утилизации, д. 4");

            system.AddWarehouse(coldWarehouse);
            system.AddWarehouse(sortingWarehouse);
            system.AddWarehouse(generalWarehouse);
            system.AddWarehouse(disposalWarehouse);

            Console.WriteLine("Система инициализирована. Создано 4 склада.\n");
        }

        static void ShowMenu()
        {
            Console.WriteLine("\n--- МЕНЮ ---");
            Console.WriteLine("1. Просмотр складов");
            Console.WriteLine("2. Создать поставку товаров");
            Console.WriteLine("3. Оптимизация сортировочных складов");
            Console.WriteLine("4. Переместить просроченные товары");
            Console.WriteLine("5. Анализ складской сети");
            Console.WriteLine("6. Расчет стоимости товаров");
            Console.WriteLine("7. Ручное перемещение товаров");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите действие: ");
        }

        static void ViewWarehouses()
        {
            Console.WriteLine("\n--- ИНФОРМАЦИЯ О СКЛАДАХ ---");
            coldWarehouse.PrintInfo();
            sortingWarehouse.PrintInfo();
            generalWarehouse.PrintInfo();
            disposalWarehouse.PrintInfo();
        }

        static void DeliverProducts()
        {
            Console.WriteLine("\n--- СОЗДАНИЕ ПОСТАВКИ ---");
            Console.Write("Количество товаров в поставке: ");
            if (!int.TryParse(Console.ReadLine(), out int count) || count <= 0)
            {
                Console.WriteLine("Неверное количество.");
                return;
            }

            List<Product> delivery = new List<Product>();

            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"\nТовар {i + 1}:");
                Console.Write("Название: ");
                string name = Console.ReadLine();
                
                Console.Write("ID поставщика: ");
                if (!int.TryParse(Console.ReadLine(), out int supplierId))
                {
                    Console.WriteLine("Неверный ID поставщика.");
                    return;
                }

                Console.Write("Объем (м³): ");
                if (!double.TryParse(Console.ReadLine(), out double volume))
                {
                    Console.WriteLine("Неверный объем.");
                    return;
                }

                Console.Write("Цена (руб): ");
                if (!double.TryParse(Console.ReadLine(), out double price))
                {
                    Console.WriteLine("Неверная цена.");
                    return;
                }

                Console.Write("Дней до истечения срока годности: ");
                if (!int.TryParse(Console.ReadLine(), out int days))
                {
                    Console.WriteLine("Неверное количество дней.");
                    return;
                }

                delivery.Add(new Product(productIdCounter++, supplierId, name, volume, price, days));
            }

            system.DeliverProducts(delivery);
        }

        static void OptimizeWarehouses()
        {
            Console.WriteLine("\n--- ОПТИМИЗАЦИЯ ---");
            Console.Write("Оптимизировать все склады? (y/n): ");
            string answer = Console.ReadLine();

            if (answer.ToLower() == "y")
            {
                system.OptimizeFromSortingWarehouses();
            }
            else
            {
                Console.Write("ID склада для оптимизации: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    system.OptimizeFromSortingWarehouses(id);
                }
                else
                {
                    Console.WriteLine("Неверный ID.");
                }
            }
        }

        static void MoveExpiredProducts()
        {
            system.MoveExpiredProducts();
        }

        static void AnalyzeNetwork()
        {
            system.AnalyzeWarehouseNetwork();
        }

        static void CalculateValues()
        {
            Console.WriteLine("\n--- РАСЧЕТ СТОИМОСТИ ---");
            system.CalculateWarehouseValue(1);
            system.CalculateWarehouseValue(2);
            system.CalculateWarehouseValue(3);
            system.CalculateWarehouseValue(4);
        }

        static void ManualMove()
        {
            Console.WriteLine("\n--- РУЧНОЕ ПЕРЕМЕЩЕНИЕ ---");
            Console.Write("ID склада-источника: ");
            if (!int.TryParse(Console.ReadLine(), out int fromId))
            {
                Console.WriteLine("Неверный ID.");
                return;
            }

            Console.Write("ID склада-назначения: ");
            if (!int.TryParse(Console.ReadLine(), out int toId))
            {
                Console.WriteLine("Неверный ID.");
                return;
            }

            Warehouse fromWarehouse = system.Warehouses.Find(w => w.Id == fromId);
            if (fromWarehouse == null || fromWarehouse.Products.Count == 0)
            {
                Console.WriteLine("Склад не найден или пуст.");
                return;
            }

            Console.WriteLine("\nТовары на складе:");
            for (int i = 0; i < fromWarehouse.Products.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {fromWarehouse.Products[i].Name}");
            }

            Console.Write("\nНомер товара для перемещения: ");
            if (!int.TryParse(Console.ReadLine(), out int productNum) || productNum < 1 || productNum > fromWarehouse.Products.Count)
            {
                Console.WriteLine("Неверный номер товара.");
                return;
            }

            List<Product> toMove = new List<Product> { fromWarehouse.Products[productNum - 1] };
            system.MoveProducts(fromId, toId, toMove);
        }
    }
}
