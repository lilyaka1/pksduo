using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WarehouseManagementSystem
{
    class Program
    {
        static WarehouseSystem system;
        static int productIdCounter = 1;
        static Logger logger;

        static void Main(string[] args)
        {
            Console.WriteLine("СИСТЕМА СКЛАДСКОГО УЧЕТА\n");

            // Создание логгера
            logger = new Logger();

            // Создание системы с логгером
            system = new WarehouseSystem(logger);

            // Создание складов с логгером
            Warehouse coldWarehouse = new Warehouse(1, WarehouseType.Cold, 1000, "ул. Холодная, 1", logger);
            Warehouse sortingWarehouse = new Warehouse(2, WarehouseType.Sorting, 1500, "ул. Центральная, 2", logger);
            Warehouse generalWarehouse = new Warehouse(3, WarehouseType.General, 2000, "ул. Складская, 3", logger);
            Warehouse disposalWarehouse = new Warehouse(4, WarehouseType.Disposal, 500, "ул. Утилизации, 4", logger);

            system.AddWarehouse(coldWarehouse);
            system.AddWarehouse(sortingWarehouse);
            system.AddWarehouse(generalWarehouse);
            system.AddWarehouse(disposalWarehouse);

            Console.WriteLine("Система инициализирована\n");

            // Загрузка тестовых данных
            LoadTestData();

            // Главное меню
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
                        ViewProducts();
                        break;
                    case "3":
                        AddDelivery();
                        break;
                    case "4":
                        DeleteProductFromWarehouse();
                        break;
                    case "5":
                        system.OptimizeFromSortingWarehouses();
                        break;
                    case "6":
                        system.MoveExpiredProducts();
                        break;
                    case "7":
                        system.AnalyzeWarehouseNetwork();
                        break;
                    case "8":
                        CalculateValues();
                        break;
                    case "9":
                        ExportLogsToJson();
                        break;
                    case "10":
                        ExportWarehousesToJson();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("\nЗавершение работы");
                        break;
                    default:
                        Console.WriteLine("\nНеверный выбор");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nНажмите Enter для продолжения...");
                    Console.ReadLine();
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("\n--- МЕНЮ ---");
            Console.WriteLine("1. Просмотр складов и добавление");
            Console.WriteLine("2. Просмотр товаров на складах");
            Console.WriteLine("3. Добавить поставку товаров");
            Console.WriteLine("4. Удалить товар со склада");
            Console.WriteLine("5. Оптимизация сортировочных складов");
            Console.WriteLine("6. Переместить просроченные товары");
            Console.WriteLine("7. Анализ складской сети");
            Console.WriteLine("8. Расчет стоимости товаров");
            Console.WriteLine("9. Экспорт логов в JSON");
            Console.WriteLine("10. Экспорт данных складов в JSON");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите действие: ");
        }

        static void ViewWarehouses()
        {
            Console.WriteLine("\n--- ИНФОРМАЦИЯ О СКЛАДАХ ---");
            foreach (var warehouse in system.Warehouses)
            {
                warehouse.DisplayWarehouseInfo();
            }
            
            Console.Write("\nДобавить новый склад? (y/n): ");
            string answer = Console.ReadLine()?.ToLower();
            
            if (answer == "y" || answer == "yes" || answer == "д" || answer == "да")
            {
                AddNewWarehouse();
            }
        }
        
        static void AddNewWarehouse()
        {
            Console.WriteLine("\n--- ДОБАВЛЕНИЕ НОВОГО СКЛАДА ---");
            
            try
            {
                Console.Write("ID склада: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("[ОШИБКА] Неверный формат ID");
                    return;
                }
                
                // Проверка на дубликат ID
                if (system.Warehouses.Any(w => w.Id == id))
                {
                    Console.WriteLine($"[ОШИБКА] Склад с ID {id} уже существует");
                    return;
                }
                
                Console.WriteLine("\nТип склада:");
                Console.WriteLine("1. Холодный (Cold)");
                Console.WriteLine("2. Сортировочный (Sorting)");
                Console.WriteLine("3. Общий (General)");
                Console.WriteLine("4. Утилизация (Disposal)");
                Console.Write("Выберите тип (1-4): ");
                
                if (!int.TryParse(Console.ReadLine(), out int typeChoice) || typeChoice < 1 || typeChoice > 4)
                {
                    Console.WriteLine("[ОШИБКА] Неверный выбор типа");
                    return;
                }
                
                WarehouseType type = typeChoice switch
                {
                    1 => WarehouseType.Cold,
                    2 => WarehouseType.Sorting,
                    3 => WarehouseType.General,
                    4 => WarehouseType.Disposal,
                    _ => WarehouseType.General
                };
                
                Console.Write("Объем склада (м³): ");
                if (!double.TryParse(Console.ReadLine(), out double volume) || volume <= 0)
                {
                    Console.WriteLine("[ОШИБКА] Неверный формат объема");
                    return;
                }
                
                Console.Write("Адрес склада: ");
                string address = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(address))
                {
                    Console.WriteLine("[ОШИБКА] Адрес не может быть пустым");
                    return;
                }
                
                var newWarehouse = new Warehouse(id, type, volume, address, logger);
                system.AddWarehouse(newWarehouse);
                
                Console.WriteLine($"\n[УСПЕХ] Склад ID {id} успешно добавлен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ОШИБКА] Не удалось добавить склад: {ex.Message}");
            }
        }

        static void ViewProducts()
        {
            Console.WriteLine("\n--- ТОВАРЫ НА СКЛАДАХ ---");
            foreach (var warehouse in system.Warehouses)
            {
                Console.WriteLine($"\nСклад ID {warehouse.Id} ({warehouse.Type}):");
                if (warehouse.Products.Count == 0)
                {
                    Console.WriteLine("  Товаров нет");
                }
                else
                {
                    for (int i = 0; i < warehouse.Products.Count; i++)
                    {
                        var product = warehouse.Products[i];
                        Console.WriteLine($"  {i + 1}. {product.Name} - ID:{product.Id}, Объем:{product.Volume} м³, Цена:{product.Price} руб., Срок:{product.DaysUntilExpiry} дней");
                    }
                }
            }
        }

        static void AddDelivery()
        {
            Console.WriteLine("\n--- ДОБАВЛЕНИЕ ПОСТАВКИ ---");
            Console.WriteLine("1. Ввести товары вручную");
            Console.WriteLine("2. Загрузить из JSON файла");
            Console.Write("Выберите способ: ");
            
            string choice = Console.ReadLine();
            List<Product> delivery = new List<Product>();
            
            if (choice == "2")
            {
                // Импорт из JSON
                Console.Write("Введите путь к JSON файлу (или имя файла в текущей папке): ");
                string fileName = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    Console.WriteLine("[ОШИБКА] Путь к файлу не может быть пустым");
                    return;
                }
                
                // Если указано только имя файла, используем текущую директорию
                string filePath = fileName;
                if (!Path.IsPathRooted(fileName))
                {
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                }
                
                delivery = DataExporter.LoadDeliveryFromJson(filePath);
                
                if (delivery.Count == 0)
                {
                    Console.WriteLine("[ОШИБКА] Не удалось загрузить товары из файла");
                    return;
                }
            }
            else if (choice == "1")
            {
                // Ручной ввод
                Console.Write("Количество товаров в поставке: ");
                
                if (!int.TryParse(Console.ReadLine(), out int count) || count <= 0)
                {
                    Console.WriteLine("Неверное количество");
                    return;
                }

                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine($"\nТовар {i + 1}:");
                    
                    Console.Write("Название: ");
                    string name = Console.ReadLine();

                    Console.Write("ID поставщика: ");
                    if (!int.TryParse(Console.ReadLine(), out int supplierId))
                    {
                        Console.WriteLine("Неверный ID поставщика");
                        return;
                    }

                    Console.Write("Объем (м³): ");
                    if (!double.TryParse(Console.ReadLine(), out double volume) || volume <= 0)
                    {
                        Console.WriteLine("Неверный объем");
                        return;
                    }

                    Console.Write("Цена (руб): ");
                    if (!double.TryParse(Console.ReadLine(), out double price) || price <= 0)
                    {
                        Console.WriteLine("Неверная цена");
                        return;
                    }

                    Console.Write("Дней до истечения срока годности: ");
                    if (!int.TryParse(Console.ReadLine(), out int days))
                    {
                        Console.WriteLine("Неверное количество дней");
                        return;
                    }

                    delivery.Add(new Product(productIdCounter++, supplierId, name, volume, price, days));
                }
            }
            else
            {
                Console.WriteLine("Неверный выбор");
                return;
            }

            system.DeliverProducts(delivery);
        }

        static void DeleteProductFromWarehouse()
        {
            Console.WriteLine("\n--- УДАЛЕНИЕ ТОВАРА СО СКЛАДА ---");
            Console.Write("ID склада: ");
            
            if (!int.TryParse(Console.ReadLine(), out int warehouseId))
            {
                Console.WriteLine("Неверный ID склада");
                return;
            }

            var warehouse = system.Warehouses.FirstOrDefault(w => w.Id == warehouseId);
            if (warehouse == null)
            {
                Console.WriteLine("Склад не найден");
                return;
            }

            if (warehouse.Products.Count == 0)
            {
                Console.WriteLine("На складе нет товаров");
                return;
            }

            Console.WriteLine("\nТовары на складе:");
            for (int i = 0; i < warehouse.Products.Count; i++)
            {
                var product = warehouse.Products[i];
                Console.WriteLine($"{i + 1}. {product.Name} (ID: {product.Id}, Объем: {product.Volume} м³)");
            }

            Console.Write("\nНомер товара для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int productNum) || productNum < 1 || productNum > warehouse.Products.Count)
            {
                Console.WriteLine("Неверный номер товара");
                return;
            }

            var productToDelete = warehouse.Products[productNum - 1];
            warehouse.RemoveProduct(productToDelete.Id);
            productToDelete.DeleteProduct();
            
            Console.WriteLine($"[ЛОГ] Товар '{productToDelete.Name}' (объем: {productToDelete.Volume} м³) удален со склада ID {warehouseId}");
        }

        static void ViewLogs()
        {
            Console.WriteLine("\n--- ПРОСМОТР ЛОГОВ ОПЕРАЦИЙ ---");
            logger.DisplayLogs();
        }
        
        static void ExportLogsToJson()
        {
            Console.WriteLine("\n--- ЭКСПОРТ ЛОГОВ В JSON ---");
            Console.Write("Введите имя файла (или нажмите Enter для 'logs.json'): ");
            string fileName = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "logs.json";
            }
            
            if (!fileName.EndsWith(".json"))
            {
                fileName += ".json";
            }
            
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            DataExporter.SaveLogsToJson(logger, filePath);
        }
        
        static void ExportWarehousesToJson()
        {
            Console.WriteLine("\n--- ЭКСПОРТ ДАННЫХ СКЛАДОВ В JSON ---");
            Console.Write("Введите имя файла (или нажмите Enter для 'warehouses.json'): ");
            string fileName = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "warehouses.json";
            }
            
            if (!fileName.EndsWith(".json"))
            {
                fileName += ".json";
            }
            
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            DataExporter.SaveWarehousesToJson(system, filePath);
        }

        static void CalculateValues()
        {
            Console.WriteLine("\n--- РАСЧЕТ СТОИМОСТИ ---");
            foreach (var warehouse in system.Warehouses)
            {
                system.CalculateWarehouseValue(warehouse.Id);
            }
        }

        // Метод для загрузки тестовых данных
        static void LoadTestData()
        {
            Console.WriteLine("Загрузка тестовых данных...\n");

            // Тестовая поставка 1: Товары с долгим сроком годности (>= 30 дней)
            // Должны попасть на общий склад
            List<Product> delivery1 = new List<Product>
            {
                new Product(productIdCounter++, 101, "Консервы тушенка", 50, 250, 365),
                new Product(productIdCounter++, 101, "Крупа гречневая", 30, 120, 180),
                new Product(productIdCounter++, 102, "Макароны спагетти", 40, 85, 90),
                new Product(productIdCounter++, 102, "Сахар", 45, 95, 730),
                new Product(productIdCounter++, 103, "Мука пшеничная", 35, 70, 120)
            };
            Console.WriteLine("=== Поставка 1: Товары длительного хранения ===");
            system.DeliverProducts(delivery1);

            // Тестовая поставка 2: Товары с коротким сроком годности (< 30 дней)
            // Должны попасть на холодный склад
            List<Product> delivery2 = new List<Product>
            {
                new Product(productIdCounter++, 104, "Молоко 3.2%", 20, 85, 7),
                new Product(productIdCounter++, 104, "Йогурт натуральный", 15, 65, 14),
                new Product(productIdCounter++, 105, "Мясо говядина", 25, 650, 5),
                new Product(productIdCounter++, 105, "Курица охлажденная", 18, 320, 10),
                new Product(productIdCounter++, 106, "Рыба форель", 22, 580, 4),
                new Product(productIdCounter++, 106, "Сметана 20%", 12, 95, 15)
            };
            Console.WriteLine("\n=== Поставка 2: Скоропортящиеся продукты ===");
            system.DeliverProducts(delivery2);

            // Тестовая поставка 3: Смешанные товары (и короткий, и долгий срок)
            // Должны попасть на сортировочный склад
            List<Product> delivery3 = new List<Product>
            {
                new Product(productIdCounter++, 107, "Сыр твердый", 10, 420, 25),
                new Product(productIdCounter++, 107, "Печенье овсяное", 20, 140, 60),
                new Product(productIdCounter++, 108, "Хлеб белый", 5, 45, 3),
                new Product(productIdCounter++, 108, "Колбаса вареная", 8, 280, 12),
                new Product(productIdCounter++, 109, "Чай черный", 15, 180, 365)
            };
            Console.WriteLine("\n=== Поставка 3: Смешанные сроки хранения ===");
            system.DeliverProducts(delivery3);

            // Тестовая поставка 4: Еще долгосрочные товары
            List<Product> delivery4 = new List<Product>
            {
                new Product(productIdCounter++, 110, "Растительное масло", 28, 150, 180),
                new Product(productIdCounter++, 110, "Соль поваренная", 25, 35, 1825),
                new Product(productIdCounter++, 111, "Кофе молотый", 12, 320, 240)
            };
            Console.WriteLine("\n=== Поставка 4: Дополнительные товары длительного хранения ===");
            system.DeliverProducts(delivery4);

            // Тестовая поставка 5: Критически свежие продукты
            List<Product> delivery5 = new List<Product>
            {
                new Product(productIdCounter++, 112, "Зелень укроп", 2, 55, 2),
                new Product(productIdCounter++, 112, "Салат айсберг", 3, 75, 5),
                new Product(productIdCounter++, 113, "Творог 9%", 10, 125, 7)
            };
            Console.WriteLine("\n=== Поставка 5: Критически свежие продукты ===");
            system.DeliverProducts(delivery5);

            // Добавляем просроченные товары для тестирования утилизации
            var warehouses = system.Warehouses;
            var generalWarehouse = warehouses.Find(w => w.Type == WarehouseType.General);
            var coldWarehouse = warehouses.Find(w => w.Type == WarehouseType.Cold);

            if (generalWarehouse != null)
            {
                Product expired1 = new Product(productIdCounter++, 114, "Просроченные консервы", 15, 200, -10);
                Product expired2 = new Product(productIdCounter++, 114, "Просроченная мука", 20, 60, -5);
                generalWarehouse.AddProduct(expired1);
                generalWarehouse.AddProduct(expired2);
                Console.WriteLine("\n[ТЕСТ] Добавлены просроченные товары на общий склад для тестирования");
            }

            if (coldWarehouse != null)
            {
                Product expired3 = new Product(productIdCounter++, 115, "Испорченное молоко", 18, 80, -3);
                Product expired4 = new Product(productIdCounter++, 115, "Протухшая рыба", 12, 400, -1);
                coldWarehouse.AddProduct(expired3);
                coldWarehouse.AddProduct(expired4);
                Console.WriteLine("[ТЕСТ] Добавлены испорченные продукты на холодный склад для тестирования");
            }

            // Тестовая поставка 6: Граничные значения (ровно 30 дней)
            List<Product> delivery6 = new List<Product>
            {
                new Product(productIdCounter++, 116, "Сок апельсиновый", 15, 95, 30),
                new Product(productIdCounter++, 116, "Джем клубничный", 10, 145, 30)
            };
            Console.WriteLine("\n=== Поставка 6: Граничные значения (30 дней) ===");
            system.DeliverProducts(delivery6);

            // Тестовая поставка 7: Товары с нулевым сроком (на грани)
            List<Product> delivery7 = new List<Product>
            {
                new Product(productIdCounter++, 117, "Торт бисквитный", 8, 380, 0),
                new Product(productIdCounter++, 117, "Пирожное эклер", 6, 120, 0)
            };
            Console.WriteLine("\n=== Поставка 7: Товары с нулевым сроком годности ===");
            system.DeliverProducts(delivery7);

            Console.WriteLine("\n========================================");
            Console.WriteLine("Тестовые данные загружены успешно!");
            Console.WriteLine("========================================");
        }
    }
}
