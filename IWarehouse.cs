using System;
using System.Collections.Generic;

namespace WarehouseManagementSystem
{
	/// <summary>
	/// Интерфейс для всех типов складов, определяющий базовое поведение
	/// </summary>
	public interface IWarehouse
	{
		/// <summary>
		/// Уникальный идентификатор склада
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Тип склада
		/// </summary>
		WarehouseType Type { get; }

		/// <summary>
		/// Общий объем склада в кубических метрах
		/// </summary>
		double Volume { get; }

		/// <summary>
		/// Адрес склада
		/// </summary>
		string Address { get; }

		/// <summary>
		/// Список товаров на складе
		/// </summary>
		List<Product> Products { get; }

		/// <summary>
		/// Добавить товар на склад
		/// </summary>
		/// <param name="product">Товар для добавления</param>
		/// <returns>true если товар добавлен, false если нет места</returns>
		bool AddProduct(Product product);

		/// <summary>
		/// Удалить товар со склада
		/// </summary>
		/// <param name="productId">ID товара для удаления</param>
		/// <returns>true если товар удален, false если товар не найден</returns>
		bool RemoveProduct(int productId);

		/// <summary>
		/// Получить занятый объем на складе
		/// </summary>
		/// <returns>Занятый объем в м³</returns>
		double GetOccupiedVolume();

		/// <summary>
		/// Получить свободный объем на складе
		/// </summary>
		/// <returns>Свободный объем в м³</returns>
		double GetFreeVolume();

		/// <summary>
		/// Вывести информацию о складе
		/// </summary>
		void DisplayWarehouseInfo();

		/// <summary>
		/// Рассчитать общую стоимость товаров на складе
		/// </summary>
		/// <returns>Общая стоимость в рублях</returns>
		double GetTotalValue();
	}
}
