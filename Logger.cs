using System;
using System.Collections.Generic;

namespace WarehouseManagementSystem
{
	/// <summary>
	/// Класс для логирования операций в системе складского учета
	/// </summary>
	public class Logger
	{
		private List<string> logs;

		/// <summary>
		/// Конструктор логгера
		/// </summary>
		public Logger()
		{
			logs = new List<string>();
		}

		/// <summary>
		/// Записать в лог операцию перемещения товара
		/// </summary>
		/// <param name="product">Перемещаемый товар</param>
		/// <param name="volume">Объем товара</param>
		/// <param name="from">Склад-источник (null если поставка)</param>
		/// <param name="to">Склад-назначение</param>
		public void LogMovement(Product product, double volume, IWarehouse from, IWarehouse to)
		{
			string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			string fromInfo = from != null ? $"Склад {from.Id} ({from.Type})" : "Поставщик";
			string toInfo = $"Склад {to.Id} ({to.Type})";
			
			string logEntry = $"[{timestamp}] Товар: {product.Name}, Объём: {volume:F1} м³, " +
			                 $"Откуда: {fromInfo}, Куда: {toInfo}";
			
			logs.Add(logEntry);
			Console.WriteLine($"[ЛОГ] {logEntry}");
		}

		/// <summary>
		/// Записать общее сообщение в лог
		/// </summary>
		/// <param name="message">Сообщение для записи</param>
		public void Log(string message)
		{
			string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			string logEntry = $"[{timestamp}] {message}";
			logs.Add(logEntry);
			Console.WriteLine($"[ЛОГ] {logEntry}");
		}

		/// <summary>
		/// Получить все записи лога
		/// </summary>
		/// <returns>Список всех записей</returns>
		public List<string> GetLogs()
		{
			return new List<string>(logs);
		}

		/// <summary>
		/// Очистить все записи лога
		/// </summary>
		public void ClearLogs()
		{
			logs.Clear();
		}

		/// <summary>
		/// Вывести все логи в консоль
		/// </summary>
		public void DisplayLogs()
		{
			Console.WriteLine("\n=== ИСТОРИЯ ОПЕРАЦИЙ ===");
			if (logs.Count == 0)
			{
				Console.WriteLine("Логи пусты");
				return;
			}

			foreach (var log in logs)
			{
				Console.WriteLine(log);
			}
			Console.WriteLine($"\nВсего записей: {logs.Count}");
		}
	}
}
