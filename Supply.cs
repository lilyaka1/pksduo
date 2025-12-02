using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseManagementSystem
{
	/// <summary>
	/// Класс для представления поставки товаров
	/// </summary>
	public class Supply
	{
		private List<Product> items;
		private Logger logger;

		/// <summary>
		/// Конструктор поставки
		/// </summary>
		/// <param name="products">Список товаров в поставке</param>
		/// <param name="logger">Логгер для записи операций</param>
		public Supply(List<Product> products, Logger logger)
		{
			if (products == null || products.Count == 0)
			{
				throw new ArgumentException("Поставка не может быть пустой");
			}

			this.items = new List<Product>(products);
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Обработать поставку и распределить товары по складам
		/// </summary>
		/// <param name="warehouses">Список всех складов в системе</param>
		public void Process(List<IWarehouse> warehouses)
		{
			if (warehouses == null || warehouses.Count == 0)
			{
				throw new ArgumentException("Список складов пуст");
			}

			// Анализируем сроки годности всех товаров в поставке
			var expiryDays = items.Select(p => p.DaysUntilExpiry).ToList();
			bool allLongTerm = expiryDays.All(days => days >= 30);
			bool allShortTerm = expiryDays.All(days => days < 30);

			WarehouseType targetType;
			string reason;

			// Определяем тип склада по правилам
			if (allLongTerm)
			{
				targetType = WarehouseType.General;
				reason = "Товары с долгим сроком (>=30 дней) → общий склад";
			}
			else if (allShortTerm)
			{
				targetType = WarehouseType.Cold;
				reason = "Товары с коротким сроком (<30 дней) → холодный склад";
			}
			else
			{
				targetType = WarehouseType.Sorting;
				reason = "Товары со смешанными сроками → сортировочный склад";
			}

			Console.WriteLine("\n--- ПОСТАВКА ТОВАРОВ ---");
			Console.WriteLine(reason);

			// Находим подходящий склад с достаточным свободным местом
			double totalVolume = items.Sum(p => p.Volume);
			var suitableWarehouses = warehouses
				.Where(w => w.Type == targetType && w.GetFreeVolume() >= totalVolume)
				.OrderByDescending(w => w.GetFreeVolume())
				.ToList();

			if (suitableWarehouses.Count == 0)
			{
				logger.Log($"ОШИБКА: Нет складов типа {targetType} с достаточным объемом {totalVolume:F1} м³");
				Console.WriteLine($"[ОШИБКА] Нет подходящих складов типа {targetType}!");
				return;
			}

			// Добавляем товары на первый подходящий склад
			var targetWarehouse = suitableWarehouses[0];
			foreach (var product in items)
			{
				if (targetWarehouse.AddProduct(product))
				{
					logger.LogMovement(product, product.Volume, null, targetWarehouse);
				}
				else
				{
					logger.Log($"ОШИБКА: Не удалось добавить товар {product.Name} на склад {targetWarehouse.Id}");
				}
			}
		}

		/// <summary>
		/// Получить список товаров в поставке
		/// </summary>
		public List<Product> GetItems()
		{
			return new List<Product>(items);
		}

		/// <summary>
		/// Получить общий объем поставки
		/// </summary>
		public double GetTotalVolume()
		{
			return items.Sum(p => p.Volume);
		}

		/// <summary>
		/// Получить количество товаров в поставке
		/// </summary>
		public int GetItemCount()
		{
			return items.Count;
		}
	}
}
