using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseManagementSystem
{
    /// <summary>
    /// Класс для представления склада в системе
    /// </summary>
    public class Warehouse
    {
        // Приватные поля для хранения данных о складе
        private int id; // Уникальный идентификатор склада
        private string type; // Тип склада: холодный, сортировочный, общий, утилизация
        private double volume; // Общий объем склада в кубических метрах
        private string address; // Адрес расположения склада
        private List<Product> products; // Список товаров, хранящихся на складе

        // Свойства
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public double Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public List<Product> Products
        {
            get { return products; }
            set { products = value; }
        }

        // Конструкторы класса
        // Пустой конструктор, инициализирует пустой список товаров
        public Warehouse()
        {
            products = new List<Product>();
        }

        // Конструктор с параметрами для создания склада с заданными характеристиками
        public Warehouse(int id, string type, double volume, string address)
        {
            this.id = id;
            this.type = type;
            this.volume = volume;
            this.address = address;
            this.products = new List<Product>();
        }

        // Методы класса
        // Метод для создания/инициализации склада с заданными параметрами
        public void CreateWarehouse(int id, string type, double volume, string address)
        {
            this.id = id;
            this.type = type;
            this.volume = volume;
            this.address = address;
            this.products = new List<Product>();
        }

        // Метод для редактирования параметров склада
        public void EditWarehouse(string type, double volume, string address)
        {
            this.type = type;
            this.volume = volume;
            this.address = address;
        }

        // Метод для вывода полной информации о складе в консоль
        public void DisplayWarehouseInfo()
        {
            Console.WriteLine($"\nСклад ID: {id}");
            Console.WriteLine($"Тип: {type}");
            Console.WriteLine($"Объем: {volume} м³");
            Console.WriteLine($"Занято: {GetOccupiedVolume()} м³");
            Console.WriteLine($"Свободно: {GetFreeVolume()} м³");
            Console.WriteLine($"Адрес: {address}");
            Console.WriteLine($"Товаров на складе: {products.Count}");
        }

        // Метод для расчета занятого объема склада
        // Суммирует объем всех товаров на складе
        public double GetOccupiedVolume()
        {
            double occupied = 0;
            foreach (var product in products)
            {
                occupied += product.Volume;
            }
            return occupied;
        }

        // Метод для расчета свободного объема склада
        // Возвращает разницу между общим и занятым объемом
        public double GetFreeVolume()
        {
            return volume - GetOccupiedVolume();
        }

        // Метод для добавления товара на склад
        // Проверяет наличие свободного места перед добавлением
        public bool AddProduct(Product product)
        {
            if (GetFreeVolume() >= product.Volume)
            {
                products.Add(product);
                return true;
            }
            return false;
        }

        // Метод для удаления товара со склада
        // Возвращает true, если товар успешно удален
        public bool RemoveProduct(Product product)
        {
            return products.Remove(product);
        }
    }
}
