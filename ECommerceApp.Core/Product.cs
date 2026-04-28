namespace ECommerceApp.Core
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public Product(int id, string name, decimal price, int stock)
        {
            Id = id;
            Name = name;
            Price = price;
            Stock = stock;
        }

        // BUG #1: Negatif fiyata izin veriyor, validation yok
        public void SetPrice(decimal newPrice)
        {
            Price = newPrice; // negatif fiyat set edilebilir!
        }

        // BUG #2: Stok azaltma sıfırın altına düşebilir
        public bool DecreaseStock(int quantity)
        {
            Stock -= quantity; // stok negatife düşebilir!
            return true;       // her zaman true döner, hatalı
        }

        public bool IsAvailable()
        {
            return Stock > 0;
        }
    }
}
