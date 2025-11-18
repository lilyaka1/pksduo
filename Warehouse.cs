using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseManagement
{
    /// <summary>
    /// Класс для описания склада
    /// </summary>
    public class Warehouse
    {
        // Приватные поля
        private int id;
        private string type; // холодный, сортировочный, общий, утилизация
        private double volume;
        private string address;
        private List<Product> products;

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

        // Конструктор для создания склада
        public Warehouse(int id, string type, double volume, string address)
        {
            this.id = id;
            this.type = type;
            this.volume = volume;
            this.address = address;
            this.products = new List<Product>();
        }

        // Метод для редактирования склада
        public void EditWarehouse(string newType, double newVolume, string newAddress)
        {
            this.type = newType;
            this.volume = newVolume;
            this.address = newAddress;
        }

        // Метод для вывода информации о складе
        public void PrintInfo()
        {
            Console.WriteLine($"\n=== Склад ID: {id} ===");
            Console.WriteLine($"Тип: {type}");
            Console.WriteLine($"Общий объем: {volume} м³");
            Console.WriteLine($"Занято: {GetOccupiedVolume()} м³");
            Console.WriteLine($"Свободно: {GetFreeVolume()} м³");
            Console.WriteLine($"Адрес: {address}");
            Console.WriteLine($"Количество товаров: {products.Count}");
            Console.WriteLine("============================\n");
        }

        // Метод для подсчета занятого объема
        public double GetOccupiedVolume()
        {
            double occupied = 0;
            foreach (var product in products)
            {
                occupied += product.VolumePerUnit;
            }
            return occupied;
        }

        // Метод для подсчета свободного объема
        public double GetFreeVolume()
        {
            return volume - GetOccupiedVolume();
        }

        // Метод для добавления товара на склад
        public bool AddProduct(Product product)
        {
            // Проверяем, есть ли свободное место
            if (GetFreeVolume() >= product.VolumePerUnit)
            {
                products.Add(product);
                return true;
            }
            return false;
        }

        // Метод для удаления товара со склада
        public bool RemoveProduct(Product product)
        {
            return products.Remove(product);
        }

        // Метод для подсчета стоимости всех товаров на складе
        public double CalculateTotalValue()
        {
            double total = 0;
            foreach (var product in products)
            {
                total += product.PricePerUnit;
            }
            return total;
        }
    }
}
