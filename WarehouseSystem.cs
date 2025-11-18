using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseManagementSystem
{
    /// <summary>
    /// Класс для управления всей системой складов
    /// Содержит методы для автоматизации логистических операций
    /// </summary>
    public class WarehouseSystem
    {
        // Список всех складов в системе
        private List<Warehouse> warehouses;

        public List<Warehouse> Warehouses
        {
            get { return warehouses; }
        }

        // Конструктор системы, инициализирует пустой список складов
        public WarehouseSystem()
        {
            warehouses = new List<Warehouse>();
        }

        // Метод для добавления склада в систему
        public void AddWarehouse(Warehouse warehouse)
        {
            warehouses.Add(warehouse);
            Console.WriteLine($"Склад ID {warehouse.Id} добавлен в систему");
        }

        // Метод поставки товаров с автоматической логистической оптимизацией
        // Анализирует сроки годности и направляет товары на подходящий тип склада
        public void DeliverProducts(List<Product> delivery)
        {
            Console.WriteLine("\n--- ПОСТАВКА ТОВАРОВ ---");

            // Проверяем сроки годности всех товаров в поставке
            bool allLongTerm = true; // Все товары с долгим сроком (>=30 дней)
            bool allShortTerm = true; // Все товары с коротким сроком (<30 дней)

            foreach (var product in delivery)
            {
                if (product.DaysUntilExpiry < 30)
                    allLongTerm = false;
                if (product.DaysUntilExpiry >= 30)
                    allShortTerm = false;
            }

            // Определяем целевой тип склада на основе анализа сроков годности
            string targetType;
            if (allLongTerm)
            {
                targetType = "общий";
                Console.WriteLine("Товары с долгим сроком (>=30 дней) → общий склад");
            }
            else if (allShortTerm)
            {
                targetType = "холодный";
                Console.WriteLine("Товары с коротким сроком (<30 дней) → холодный склад");
            }
            else
            {
                targetType = "сортировочный";
                Console.WriteLine("Товары со смешанными сроками → сортировочный склад");
            }

            foreach (var product in delivery)
            {
                bool placed = false;
                foreach (var warehouse in warehouses)
                {
                    if (warehouse.Type == targetType && warehouse.GetFreeVolume() >= product.Volume)
                    {
                        warehouse.AddProduct(product);
                        Console.WriteLine($"[ЛОГ] Товар '{product.Name}' ({product.Volume} м³) → от Поставщика → склад ID {warehouse.Id} ({warehouse.Type})");
                        placed = true;
                        break;
                    }
                }
                if (!placed)
                {
                    Console.WriteLine($"[ОШИБКА] Нет места для товара '{product.Name}'");
                }
            }
        }

        // Метод оптимизационного перемещения товаров с сортировочных складов
        // Анализирует товары и распределяет их по складам согласно срокам хранения
        public void OptimizeFromSortingWarehouses()
        {
            Console.WriteLine("\n--- ОПТИМИЗАЦИЯ СОРТИРОВОЧНЫХ СКЛАДОВ ---");

            var sortingWarehouses = warehouses.Where(w => w.Type == "сортировочный").ToList();

            foreach (var sortWarehouse in sortingWarehouses)
            {
                Console.WriteLine($"Анализ склада ID {sortWarehouse.Id}");
                List<Product> productsToMove = new List<Product>(sortWarehouse.Products);

                // Перебираем товары и определяем целевой склад
                foreach (var product in productsToMove)
                {
                    // Товары с долгим сроком (>=30) → общий склад, с коротким (<30) → холодный
                    string targetType = product.DaysUntilExpiry >= 30 ? "общий" : "холодный";

                    foreach (var targetWarehouse in warehouses)
                    {
                        if (targetWarehouse.Type == targetType && targetWarehouse.GetFreeVolume() >= product.Volume)
                        {
                            sortWarehouse.RemoveProduct(product);
                            targetWarehouse.AddProduct(product);
                            Console.WriteLine($"[ЛОГ] Товар '{product.Name}' ({product.Volume} м³) → склад ID {sortWarehouse.Id} → склад ID {targetWarehouse.Id} ({targetType})");
                            break;
                        }
                    }
                }
            }
        }

        // Метод перемещения товаров с истекшим сроком годности на склад утилизации
        // Проверяет все склады и перемещает товары со сроком годности <= 0
        public void MoveExpiredProducts()
        {
            Console.WriteLine("\n--- ПЕРЕМЕЩЕНИЕ ПРОСРОЧЕННЫХ ТОВАРОВ ---");

            var disposalWarehouse = warehouses.FirstOrDefault(w => w.Type == "утилизация");
            if (disposalWarehouse == null)
            {
                Console.WriteLine("[ОШИБКА] Склад утилизации не найден");
                return;
            }

            // Проверяем все склады кроме склада утилизации
            foreach (var warehouse in warehouses.Where(w => w.Type != "утилизация"))
            {
                // Находим все просроченные товары (срок <= 0 дней)
                var expiredProducts = warehouse.Products.Where(p => p.DaysUntilExpiry <= 0).ToList();

                foreach (var product in expiredProducts)
                {
                    if (disposalWarehouse.GetFreeVolume() >= product.Volume)
                    {
                        warehouse.RemoveProduct(product);
                        disposalWarehouse.AddProduct(product);
                        Console.WriteLine($"[ЛОГ] Просроченный товар '{product.Name}' ({product.Volume} м³) → склад ID {warehouse.Id} → утилизация ID {disposalWarehouse.Id}");
                    }
                }
            }
        }

        // Метод анализа складской сети
        // Выдает статус каждого склада и рекомендации по оптимизации
        public void AnalyzeWarehouseNetwork()
        {
            Console.WriteLine("\n--- АНАЛИЗ СКЛАДСКОЙ СЕТИ ---");

            foreach (var warehouse in warehouses)
            {
                Console.WriteLine($"\nСклад ID {warehouse.Id} ({warehouse.Type}):");

                bool hasViolations = false;
                List<string> recommendations = new List<string>();

                var expiredProducts = warehouse.Products.Where(p => p.DaysUntilExpiry <= 0).ToList();
                if (expiredProducts.Count > 0 && warehouse.Type != "утилизация")
                {
                    hasViolations = true;
                    recommendations.Add("- Переместить просроченные товары");
                }

                if (warehouse.Type == "сортировочный" && warehouse.Products.Count > 0)
                {
                    hasViolations = true;
                    recommendations.Add("- Провести оптимизацию товаров");
                }

                if (warehouse.Type == "холодный")
                {
                    var longTermProducts = warehouse.Products.Where(p => p.DaysUntilExpiry >= 30).ToList();
                    if (longTermProducts.Count > 0)
                    {
                        hasViolations = true;
                        recommendations.Add("- Переместить товары с долгим сроком на общий склад");
                    }
                }

                if (warehouse.Type == "общий")
                {
                    var shortTermProducts = warehouse.Products.Where(p => p.DaysUntilExpiry < 30 && p.DaysUntilExpiry > 0).ToList();
                    if (shortTermProducts.Count > 0)
                    {
                        hasViolations = true;
                        recommendations.Add("- Переместить товары с коротким сроком на холодный склад");
                    }
                }

                if (hasViolations)
                {
                    Console.WriteLine("Статус: НАРУШЕНИЯ ЕСТЬ");
                    Console.WriteLine("Рекомендации:");
                    foreach (var rec in recommendations)
                        Console.WriteLine(rec);
                }
                else
                {
                    Console.WriteLine("Статус: НАРУШЕНИЙ НЕТ");
                }
            }
        }

        // Метод подсчета общей стоимости всех товаров на конкретном складе
        public void CalculateWarehouseValue(int warehouseId)
        {
            var warehouse = warehouses.FirstOrDefault(w => w.Id == warehouseId);
            if (warehouse == null)
            {
                Console.WriteLine($"[ОШИБКА] Склад ID {warehouseId} не найден");
                return;
            }

            // Суммируем цены всех товаров на складе
            double total = 0;
            foreach (var product in warehouse.Products)
            {
                total += product.Price;
            }

            Console.WriteLine($"Стоимость товаров на складе ID {warehouseId}: {total} руб.");
        }

        // Метод ручного перемещения товаров между складами
        // Позволяет переместить выбранные товары с одного склада на другой
        public void MoveProducts(int fromId, int toId, List<Product> productsToMove)
        {
            Console.WriteLine("\n--- РУЧНОЕ ПЕРЕМЕЩЕНИЕ ---");

            var fromWarehouse = warehouses.FirstOrDefault(w => w.Id == fromId);
            var toWarehouse = warehouses.FirstOrDefault(w => w.Id == toId);

            if (fromWarehouse == null || toWarehouse == null)
            {
                Console.WriteLine("[ОШИБКА] Склад не найден");
                return;
            }

            foreach (var product in productsToMove)
            {
                if (toWarehouse.GetFreeVolume() >= product.Volume)
                {
                    fromWarehouse.RemoveProduct(product);
                    toWarehouse.AddProduct(product);
                    Console.WriteLine($"[ЛОГ] Товар '{product.Name}' ({product.Volume} м³) → склад ID {fromId} → склад ID {toId}");
                }
                else
                {
                    Console.WriteLine($"[ОШИБКА] Недостаточно места для товара '{product.Name}'");
                }
            }
        }
    }
}
