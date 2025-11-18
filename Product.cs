using System;

namespace WarehouseManagement
{
    /// <summary>
    /// Класс для описания товара
    /// </summary>
    public class Product
    {
        // Приватные поля для хранения данных
        private int id;
        private int supplierId;
        private string name;
        private double volumePerUnit;
        private double pricePerUnit;
        private int daysUntilExpiry;

        // Свойства с геттерами и сеттерами
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

        public double VolumePerUnit
        {
            get { return volumePerUnit; }
            set { volumePerUnit = value; }
        }

        public double PricePerUnit
        {
            get { return pricePerUnit; }
            set { pricePerUnit = value; }
        }

        public int DaysUntilExpiry
        {
            get { return daysUntilExpiry; }
            set { daysUntilExpiry = value; }
        }

        // Конструктор для создания товара
        public Product(int id, int supplierId, string name, double volumePerUnit, double pricePerUnit, int daysUntilExpiry)
        {
            this.id = id;
            this.supplierId = supplierId;
            this.name = name;
            this.volumePerUnit = volumePerUnit;
            this.pricePerUnit = pricePerUnit;
            this.daysUntilExpiry = daysUntilExpiry;
        }

        // Метод для изменения информации о товаре
        public void EditProduct(string newName, double newVolume, double newPrice, int newDaysUntilExpiry)
        {
            this.name = newName;
            this.volumePerUnit = newVolume;
            this.pricePerUnit = newPrice;
            this.daysUntilExpiry = newDaysUntilExpiry;
        }

        // Метод для вывода информации о товаре
        public void PrintInfo()
        {
            Console.WriteLine($"Товар ID: {id}");
            Console.WriteLine($"Поставщик ID: {supplierId}");
            Console.WriteLine($"Название: {name}");
            Console.WriteLine($"Объем единицы: {volumePerUnit} м³");
            Console.WriteLine($"Цена единицы: {pricePerUnit} руб.");
            Console.WriteLine($"Дней до истечения срока годности: {daysUntilExpiry}");
            Console.WriteLine("-----------------------------------");
        }
    }
}
