using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace WarehouseManagementSystem
{
	/// <summary>
	/// Класс для экспорта и импорта данных в JSON формате
	/// </summary>
	public static class DataExporter
	{
		/// <summary>
		/// Класс для десериализации товара из JSON
		/// </summary>
		public class ProductJson
		{
			public int Id { get; set; }
			public int SupplierId { get; set; }
			public string Name { get; set; }
			public double Volume { get; set; }
			public double Price { get; set; }
			public int DaysUntilExpiry { get; set; }
		}
		
		/// <summary>
		/// Сохраняет логи в JSON файл
		/// </summary>
		public static void SaveLogsToJson(Logger logger, string filePath)
		{
			try
			{
				var logs = logger.GetLogs();
				var options = new JsonSerializerOptions
				{
					WriteIndented = true,
					Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
				};
				
				string jsonString = JsonSerializer.Serialize(logs, options);
				File.WriteAllText(filePath, jsonString);
				
				Console.WriteLine($"[УСПЕХ] Логи сохранены в файл: {filePath}");
				logger.Log($"Логи экспортированы в JSON: {filePath}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[ОШИБКА] Не удалось сохранить логи: {ex.Message}");
			}
		}
		
		/// <summary>
		/// Загружает поставку товаров из JSON файла
		/// </summary>
		public static List<Product> LoadDeliveryFromJson(string filePath)
		{
			try
			{
				if (!File.Exists(filePath))
				{
					Console.WriteLine($"[ОШИБКА] Файл не найден: {filePath}");
					return new List<Product>();
				}
				
				string jsonString = File.ReadAllText(filePath);
				var productsJson = JsonSerializer.Deserialize<List<ProductJson>>(jsonString);
				
				if (productsJson == null || productsJson.Count == 0)
				{
					Console.WriteLine($"[ОШИБКА] Файл пуст или неверный формат");
					return new List<Product>();
				}
				
				var products = new List<Product>();
				foreach (var pj in productsJson)
				{
					try
					{
						var product = new Product(pj.Id, pj.SupplierId, pj.Name, pj.Volume, pj.Price, pj.DaysUntilExpiry);
						products.Add(product);
					}
					catch (Exception ex)
					{
						Console.WriteLine($"[ОШИБКА] Не удалось создать товар '{pj.Name}': {ex.Message}");
					}
				}
				
				Console.WriteLine($"[УСПЕХ] Загружено {products.Count} товаров из файла: {filePath}");
				return products;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[ОШИБКА] Не удалось загрузить поставку: {ex.Message}");
				return new List<Product>();
			}
		}
		
		/// <summary>
		/// Сохраняет данные о складах в JSON файл
		/// </summary>
		public static void SaveWarehousesToJson(WarehouseSystem system, string filePath)
		{
			try
			{
				var warehousesData = new List<object>();
				
				foreach (var warehouse in system.Warehouses)
				{
					var warehouseInfo = new
					{
						Id = warehouse.Id,
						Type = warehouse.Type.ToString(),
						Volume = warehouse.Volume,
						Address = warehouse.Address,
						OccupiedVolume = warehouse.GetOccupiedVolume(),
						FreeVolume = warehouse.GetFreeVolume(),
						ProductsCount = warehouse.Products.Count,
						Products = warehouse.Products.Select(p => new
						{
							p.Id,
							p.SupplierId,
							p.Name,
							p.Volume,
							p.Price,
							p.DaysUntilExpiry
						}).ToList()
					};
					
					warehousesData.Add(warehouseInfo);
				}
				
				var options = new JsonSerializerOptions
				{
					WriteIndented = true,
					Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
				};
				
				string jsonString = JsonSerializer.Serialize(warehousesData, options);
				File.WriteAllText(filePath, jsonString);
				
				Console.WriteLine($"[УСПЕХ] Данные о складах сохранены в файл: {filePath}");
				system.GetLogger()?.Log($"Данные складов экспортированы в JSON: {filePath}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[ОШИБКА] Не удалось сохранить данные складов: {ex.Message}");
			}
		}
	}
}
