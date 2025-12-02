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
		private List<IWarehouse> warehouses;
		private Logger logger;

		public List<IWarehouse> Warehouses
		{
			get { return warehouses; }
		}

		// Конструктор системы, принимает Logger через DI
		public WarehouseSystem(Logger logger)
		{
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			warehouses = new List<IWarehouse>();
		}
		
		// Конструктор для обратной совместимости
		public WarehouseSystem()
		{
			this.logger = new Logger();
			warehouses = new List<IWarehouse>();
		}

		// Метод для добавления склада в систему
		public void AddWarehouse(IWarehouse warehouse)
		{
			warehouses.Add(warehouse);
			logger.Log($"Склад ID {warehouse.Id} добавлен в систему");
			Console.WriteLine($"Склад ID {warehouse.Id} добавлен в систему");
		}

		// Метод поставки товаров с автоматической логистической оптимизацией
		// Использует класс Supply для анализа сроков годности и автоматического распределения
		public void DeliverProducts(List<Product> delivery)
		{
			Console.WriteLine("\n--- ПОСТАВКА ТОВАРОВ ---");
			
			var supply = new Supply(delivery, logger);
			supply.Process(warehouses);
		}

		// Метод оптимизационного перемещения товаров с сортировочных складов
		// Анализирует товары и распределяет их по складам согласно срокам хранения
		public void OptimizeFromSortingWarehouses()
		{
			Console.WriteLine("\n--- ОПТИМИЗАЦИЯ СОРТИРОВОЧНЫХ СКЛАДОВ ---");
			logger.Log("Запущена оптимизация сортировочных складов");

			var sortingWarehouses = warehouses.Where(w => w.Type == WarehouseType.Sorting).ToList();

			foreach (var sortWarehouse in sortingWarehouses)
			{
				Console.WriteLine($"Анализ склада ID {sortWarehouse.Id}");
				List<Product> productsToMove = new List<Product>(sortWarehouse.Products);

				// Перебираем товары и определяем целевой склад
				foreach (var product in productsToMove)
				{
					// Товары с долгим сроком (>=30) → общий склад, с коротким (<30) → холодный
					WarehouseType targetType = product.DaysUntilExpiry >= 30 ? WarehouseType.General : WarehouseType.Cold;

					foreach (var targetWarehouse in warehouses)
					{
						if (targetWarehouse.Type == targetType && targetWarehouse.GetFreeVolume() >= product.Volume)
						{
							sortWarehouse.RemoveProduct(product.Id);
							targetWarehouse.AddProduct(product);
							logger.LogMovement(product, product.Volume, sortWarehouse, targetWarehouse);
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
			logger.Log("Запущена проверка просроченных товаров");

			var disposalWarehouse = warehouses.FirstOrDefault(w => w.Type == WarehouseType.Disposal);
			if (disposalWarehouse == null)
			{
				Console.WriteLine("[ОШИБКА] Склад утилизации не найден");
				logger.Log("ОШИБКА: Склад утилизации не найден");
				return;
			}

			// Проверяем все склады кроме склада утилизации
			foreach (var warehouse in warehouses.Where(w => w.Type != WarehouseType.Disposal))
			{
				// Находим все просроченные товары (срок <= 0 дней)
				var expiredProducts = warehouse.Products.Where(p => p.DaysUntilExpiry <= 0).ToList();

				foreach (var product in expiredProducts)
				{
					if (disposalWarehouse.GetFreeVolume() >= product.Volume)
					{
						warehouse.RemoveProduct(product.Id);
						disposalWarehouse.AddProduct(product);
						logger.LogMovement(product, product.Volume, warehouse, disposalWarehouse);
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
			logger.Log("Запущен анализ складской сети");

			foreach (var warehouse in warehouses)
			{
				Console.WriteLine($"\nСклад ID {warehouse.Id} ({warehouse.Type}):");

				bool hasViolations = false;
				List<string> recommendations = new List<string>();

				var expiredProducts = warehouse.Products.Where(p => p.DaysUntilExpiry <= 0).ToList();
				if (expiredProducts.Count > 0 && warehouse.Type != WarehouseType.Disposal)
				{
					hasViolations = true;
					recommendations.Add("- Переместить просроченные товары");
				}

				if (warehouse.Type == WarehouseType.Sorting && warehouse.Products.Count > 0)
				{
					hasViolations = true;
					recommendations.Add("- Провести оптимизацию товаров");
				}

				if (warehouse.Type == WarehouseType.Cold)
				{
					var longTermProducts = warehouse.Products.Where(p => p.DaysUntilExpiry >= 30).ToList();
					if (longTermProducts.Count > 0)
					{
						hasViolations = true;
						recommendations.Add("- Переместить товары с долгим сроком на общий склад");
					}
				}

				if (warehouse.Type == WarehouseType.General)
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
			logger.Log($"Запущено ручное перемещение со склада {fromId} на склад {toId}");

			var fromWarehouse = warehouses.FirstOrDefault(w => w.Id == fromId);
			var toWarehouse = warehouses.FirstOrDefault(w => w.Id == toId);

			if (fromWarehouse == null || toWarehouse == null)
			{
				Console.WriteLine("[ОШИБКА] Склад не найден");
				logger.Log("ОШИБКА: Склад не найден");
				return;
			}

			foreach (var product in productsToMove)
			{
				if (toWarehouse.GetFreeVolume() >= product.Volume)
				{
					fromWarehouse.RemoveProduct(product.Id);
					toWarehouse.AddProduct(product);
					logger.LogMovement(product, product.Volume, fromWarehouse, toWarehouse);
					Console.WriteLine($"[ЛОГ] Товар '{product.Name}' ({product.Volume} м³) → склад ID {fromId} → склад ID {toId}");
				}
				else
				{
					Console.WriteLine($"[ОШИБКА] Недостаточно места для товара '{product.Name}'");
					logger.Log($"ОШИБКА: Недостаточно места для товара '{product.Name}'");
				}
			}
		}
		
		// Метод для получения логов
		public Logger GetLogger()
		{
			return logger;
		}
	}
}