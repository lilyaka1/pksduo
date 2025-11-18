using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseManagementSystem
{
    /// <summary>
    /// Класс для представления товара в системе складского учета
    /// </summary>
    public class Product
    {
        // Приватные поля для хранения данных о товаре
        private int id; // Уникальный идентификатор товара
        private int supplierId; // Идентификатор поставщика
        private string name; // Название товара
        private double volume; // Объем одной единицы товара в кубических метрах
        private double price; // Цена одной единицы товара
        private int daysUntilExpiry; // Количество дней до истечения срока годности

        // Свойства для доступа к полям класса
        // Свойство для получения и установки ID товара
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int SupplierId
        {
            get { return supplierId; }
            set { supplierId = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public double Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        public double Price
        {
            get { return price; }
            set { price = value; }
        }

        public int DaysUntilExpiry
        {
            get { return daysUntilExpiry; }
            set { daysUntilExpiry = value; }
        }

        // Конструкторы класса
        // Пустой конструктор для создания товара без параметров
        public Product() { }

        // Конструктор с параметрами для создания товара с заданными значениями
        public Product(int id, int supplierId, string name, double volume, double price, int daysUntilExpiry)
        {
            this.id = id;
            this.supplierId = supplierId;
            this.name = name;
            this.volume = volume;
            this.price = price;
            this.daysUntilExpiry = daysUntilExpiry;
        }

        // Методы класса
        // Метод для создания/инициализации товара с заданными параметрами
        public void CreateProduct(int id, int supplierId, string name, double volume, double price, int daysUntilExpiry)
        {
            this.id = id;
            this.supplierId = supplierId;
            this.name = name;
            this.volume = volume;
            this.price = price;
            this.daysUntilExpiry = daysUntilExpiry;
        }

        // Метод для редактирования существующего товара
        public void EditProduct(int supplierId, string name, double volume, double price, int daysUntilExpiry)
        {
            this.supplierId = supplierId;
            this.name = name;
            this.volume = volume;
            this.price = price;
            this.daysUntilExpiry = daysUntilExpiry;
        }

        // Метод для вывода информации о товаре в консоль
        public void DisplayProductInfo()
        {
            Console.WriteLine($"ID товара: {id}");
            Console.WriteLine($"ID поставщика: {supplierId}");
            Console.WriteLine($"Название: {name}");
            Console.WriteLine($"Объем: {volume}");
            Console.WriteLine($"Цена: {price}");
            Console.WriteLine($"Дней до истечения срока: {daysUntilExpiry}");
        }

        // Метод для удаления товара (обнуление всех полей)
        public void DeleteProduct()
        {
            id = 0;
            supplierId = 0;
            name = string.Empty;
            volume = 0;
            price = 0;
            daysUntilExpiry = 0;
        }
    }
}