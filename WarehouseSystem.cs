using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseManagement
{
    /// <summary>
    /// Класс для управления системой складов
    /// </summary>
    public class WarehouseSystem
    {
        // Список всех складов в системе
        private List<Warehouse> warehouses;

        // Свойство для доступа к складам
        public List<Warehouse> Warehouses
        {
            get { return warehouses; }
        }

        // Конструктор
        public WarehouseSystem()
        {
            warehouses = new List<Warehouse>();
        }

        // Метод для добавления склада в систему
        public void AddWarehouse(Warehouse warehouse)
        {
            warehouses.Add(warehouse);
            Console.WriteLine($"Склад ID {warehouse.Id} добавлен в систему.");
        }

        // Метод поставки товаров с автоматической оптимизацией
        public void DeliverProducts(List<Product> delivery)
        {
            Console.WriteLine("\n=== ПОСТАВКА ТОВАРОВ ===");
            
            // Проверяем сроки годности всех товаров в поставке
            bool allLongTerm = true;  // все товары с долгим сроком (>=30)
            bool allShortTerm = true; // все товары с коротким сроком (<30)

            foreach (var product in delivery)
            {
                if (product.DaysUntilExpiry < 30)
                {
                    allLongTerm = false;
                }
                if (product.DaysUntilExpiry >= 30)
                {
                    allShortTerm = false;
                }
            }

            string targetType;
            
            // Определяем тип склада для поставки
            if (allLongTerm)
            {
                targetType = "общий";
                Console.WriteLine("Все товары с долгим сроком годности (>=30 дней) → направляем на общий склад");
            }
            else if (allShortTerm)
            {
                targetType = "холодный";
                Console.WriteLine("Все товары с коротким сроком годности (<30 дней) → направляем на холодный склад");
            }
            else
            {
                targetType = "сортировочный";
                Console.WriteLine("Товары с разными сроками годности → направляем на сортировочный склад");
            }

            // Распределяем товары по подходящим складам
            foreach (var product in delivery)
            {
                bool placed = false;
                
                // Ищем подходящий склад с достаточным свободным местом
                foreach (var warehouse in warehouses)
                {
                    if (warehouse.Type == targetType && warehouse.GetFreeVolume() >= product.VolumePerUnit)
                    {
                        warehouse.AddProduct(product);
                        Console.WriteLine($"[ЛОГ] Товар '{product.Name}' (объем: {product.VolumePerUnit} м³) → от Поставщика → на склад ID {warehouse.Id} ({warehouse.Type})");
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    Console.WriteLine($"[ОШИБКА] Не найден подходящий склад для товара '{product.Name}'");
                }
            }
        }

        // Метод оптимизационного перемещения товаров с сортировочных складов
        public void OptimizeFromSortingWarehouses(int? warehouseId = null)
        {
            Console.WriteLine("\n=== ОПТИМИЗАЦИЯ СОРТИРОВОЧНЫХ СКЛАДОВ ===");
            
            // Получаем список сортировочных складов для обработки
            List<Warehouse> sortingWarehouses;
            if (warehouseId.HasValue)
            {
                sortingWarehouses = warehouses.Where(w => w.Id == warehouseId.Value && w.Type == "сортировочный").ToList();
            }
            else
            {
                sortingWarehouses = warehouses.Where(w => w.Type == "сортировочный").ToList();
            }

            // Обрабатываем каждый сортировочный склад
            foreach (var sortWarehouse in sortingWarehouses)
            {
                Console.WriteLine($"\nАнализируем склад ID {sortWarehouse.Id}...");
                
                // Создаем копию списка товаров для безопасной итерации
                List<Product> productsToMove = new List<Product>(sortWarehouse.Products);

                foreach (var product in productsToMove)
                {
                    string targetType;
                    
                    // Определяем целевой тип склада по сроку годности
                    if (product.DaysUntilExpiry >= 30)
                    {
                        targetType = "общий";
                    }
                    else
                    {
                        targetType = "холодный";
                    }

                    // Ищем подходящий склад
                    foreach (var targetWarehouse in warehouses)
                    {
                        if (targetWarehouse.Type == targetType && targetWarehouse.GetFreeVolume() >= product.VolumePerUnit)
                        {
                            sortWarehouse.RemoveProduct(product);
                            targetWarehouse.AddProduct(product);
                            Console.WriteLine($"[ЛОГ] Товар '{product.Name}' (объем: {product.VolumePerUnit} м³) → со склада ID {sortWarehouse.Id} → на склад ID {targetWarehouse.Id} ({targetType})");
                            break;
                        }
                    }
                }
            }
        }

        // Метод перемещения товаров между складами вручную
        public void MoveProducts(int fromWarehouseId, int toWarehouseId, List<Product> productsToMove)
        {
            Console.WriteLine("\n=== ПЕРЕМЕЩЕНИЕ ТОВАРОВ ===");
            
            // Находим склады
            Warehouse fromWarehouse = warehouses.FirstOrDefault(w => w.Id == fromWarehouseId);
            Warehouse toWarehouse = warehouses.FirstOrDefault(w => w.Id == toWarehouseId);

            if (fromWarehouse == null || toWarehouse == null)
            {
                Console.WriteLine("[ОШИБКА] Склад не найден");
                return;
            }

            // Перемещаем товары
            foreach (var product in productsToMove)
            {
                if (toWarehouse.GetFreeVolume() >= product.VolumePerUnit)
                {
                    fromWarehouse.RemoveProduct(product);
                    toWarehouse.AddProduct(product);
                    Console.WriteLine($"[ЛОГ] Товар '{product.Name}' (объем: {product.VolumePerUnit} м³) → со склада ID {fromWarehouseId} → на склад ID {toWarehouseId}");
                }
                else
                {
                    Console.WriteLine($"[ОШИБКА] Недостаточно места на складе ID {toWarehouseId} для товара '{product.Name}'");
                }
            }
        }

        // Метод перемещения товаров с истекшим сроком годности
        public void MoveExpiredProducts(int? warehouseId = null)
        {
            Console.WriteLine("\n=== ПЕРЕМЕЩЕНИЕ ПРОСРОЧЕННЫХ ТОВАРОВ ===");
            
            // Находим склад утилизации
            Warehouse disposalWarehouse = warehouses.FirstOrDefault(w => w.Type == "утилизация");
            if (disposalWarehouse == null)
            {
                Console.WriteLine("[ОШИБКА] Склад утилизации не найден");
                return;
            }

            // Получаем список складов для проверки
            List<Warehouse> warehousesToCheck;
            if (warehouseId.HasValue)
            {
                warehousesToCheck = warehouses.Where(w => w.Id == warehouseId.Value).ToList();
            }
            else
            {
                warehousesToCheck = warehouses.Where(w => w.Type != "утилизация").ToList();
            }
        }

        // Метод анализа складской сети
        public void AnalyzeWarehouseNetwork()
        {
            Console.WriteLine("\n=== АНАЛИЗ СКЛАДСКОЙ СЕТИ ===");

            foreach (var warehouse in warehouses)
            {
                Console.WriteLine($"\nСклад ID {warehouse.Id} ({warehouse.Type}):");
                
                bool hasViolations = false;
                List<string> recommendations = new List<string>();

                // Проверяем наличие просроченных товаров
                var expiredProducts = warehouse.Products.Where(p => p.DaysUntilExpiry <= 0).ToList();
                if (expiredProducts.Count > 0 && warehouse.Type != "утилизация")
                {
                    hasViolations = true;
                    recommendations.Add("- Переместить просроченные товары (MoveExpiredProducts)");
                }

                // Проверяем сортировочные склады
                if (warehouse.Type == "сортировочный" && warehouse.Products.Count > 0)
                {
                    hasViolations = true;
                    recommendations.Add("- Провести оптимизацию товаров (OptimizeFromSortingWarehouses)");
                }

                // Проверяем соответствие товаров типу склада
                if (warehouse.Type == "холодный")
                {
                    var longTermProducts = warehouse.Products.Where(p => p.DaysUntilExpiry >= 30).ToList();
                    if (longTermProducts.Count > 0)
                    {
                        hasViolations = true;
                        recommendations.Add("- Переместить товары с долгим сроком на общий склад");
                    }
                }
                else if (warehouse.Type == "общий")
                {
                    var shortTermProducts = warehouse.Products.Where(p => p.DaysUntilExpiry < 30 && p.DaysUntilExpiry > 0).ToList();
                    if (shortTermProducts.Count > 0)
                    {
                        hasViolations = true;
                        recommendations.Add("- Переместить товары с коротким сроком на холодный склад");
                    }
                }

                // Выводим результат
                if (hasViolations)
                {
                    Console.WriteLine("Статус: НАРУШЕНИЯ ЕСТЬ");
                    Console.WriteLine("Рекомендации:");
                    foreach (var rec in recommendations)
                    {
                        Console.WriteLine(rec);
                    }
                }
                else
                {
                    Console.WriteLine("Статус: НАРУШЕНИЙ НЕТ");
                }
            }
        }

