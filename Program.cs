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

       