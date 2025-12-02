using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseManagementSystem
{
	/// <summary>
	/// Класс для представления склада в системе (реализует интерфейс IWarehouse)
	/// </summary>
	public class Warehouse : IWarehouse
	{
		// Приватные поля
		private int id;
		private WarehouseType type;
		private double volume;
		private string address;
		private List<Product> products;
		private Logger logger;

		// Свойства интерфейса IWarehouse
		public int Id => id;
		public WarehouseType Type => type;
		public double Volume => volume;
		public string Address => address;
		public List<Product> Products => products;

		/// <summary>
		/// Конструктор склада с валидацией
		/// </summary>
		public Warehouse(int id, WarehouseType type, double volume, string address, Logger logger)
		{
			if (id <= 0)
				throw new ArgumentException("ID склада должен быть положительным числом", nameof(id));
			if (volume <= 0)
				throw new ArgumentException("Объем склада должен быть положительным числом", nameof(volume));
			if (string.IsNullOrWhiteSpace(address))
				throw new ArgumentException("Адрес склада не может быть пустым", nameof(address));

			this.id = id;
			this.type = type;
			this.volume = volume;
			this.address = address;
			this.products = new List<Product>();
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		// Конструктор для обратной совместимости (конвертирует string в enum)
		public Warehouse(int id, string typeString, double volume, string address)
		{
			if (id <= 0)
				throw new ArgumentException("ID склада должен быть положительным числом", nameof(id));
			if (volume <= 0)
				throw new ArgumentException("Объем склада должен быть положительным числом", nameof(volume));
			if (string.IsNullOrWhiteSpace(address))
				throw new ArgumentException("Адрес склада не может быть пустым", nameof(address));

			this.id = id;
			this.type = ConvertStringToType(typeString);
			this.volume = volume;
			this.address = address;
			this.products = new List<Product>();
			this.logger = new Logger(); // Создаем логгер по умолчанию
		}

		/// <summary>
		/// Конвертировать строку в enum WarehouseType
		/// </summary>
		private WarehouseType ConvertStringToType(string typeString)
		{
			switch (typeString?.ToLower())
			{
				case "холодный":
				case "cold":
					return WarehouseType.Cold;
				case "сортировочный":
				case "sorting":
					return WarehouseType.Sorting;
				case "общий":
				case "general":
					return WarehouseType.General;
				case "утилизация":
				case "disposal":
					return WarehouseType.Disposal;
				default:
					throw new ArgumentException($"Неизвестный тип склада: {typeString}");
			}
		}

		/// <summary>
		/// Конвертировать enum в строку на русском
		/// </summary>
		public string GetTypeString()
		{
			switch (type)
			{
				case WarehouseType.Cold:
					return "холодный";
				case WarehouseType.Sorting:
					return "сортировочный";
				case WarehouseType.General:
					return "общий";
				case WarehouseType.Disposal:
					return "утилизация";
				default:
					return type.ToString();
			}
		}

		public bool AddProduct(Product product)
		{
			if (product == null)
				throw new ArgumentNullException(nameof(product));

			// Проверка на переполнение
			double currentVolume = GetOccupiedVolume();
			if (currentVolume + product.Volume > volume)
			{
				return false; // Недостаточно места
			}

			products.Add(product);
			return true;
		}

		public bool RemoveProduct(int productId)
		{
			var product = products.FirstOrDefault(p => p.Id == productId);
			if (product == null)
			{
				return false;
			}

			products.Remove(product);
			return true;
		}

		public double GetOccupiedVolume()
		{
			return products.Sum(p => p.Volume);
		}

		public double GetFreeVolume()
		{
			return volume - GetOccupiedVolume();
		}

		public double GetTotalValue()
		{
			return products.Sum(p => p.Price);
		}

		public void DisplayWarehouseInfo()
		{
			Console.WriteLine($"\nСклад ID {id}:");
			Console.WriteLine($"Тип: {GetTypeString()}");
			Console.WriteLine($"Адрес: {address}");
			Console.WriteLine($"Объем: {GetOccupiedVolume():F1}/{volume:F1} м³");
			Console.WriteLine($"Свободно: {GetFreeVolume():F1} м³");
			Console.WriteLine($"Товаров: {products.Count}");
			Console.WriteLine($"Общая стоимость: {GetTotalValue():F2} руб.");
		}
	}
}
