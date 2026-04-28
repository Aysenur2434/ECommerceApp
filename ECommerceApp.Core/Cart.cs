namespace ECommerceApp.Core
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }

    public class Cart
    {
        private List<CartItem> _items = new List<CartItem>();

        public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

        // BUG #3: Aynı ürün tekrar eklenirse yeni item olarak ekleniyor (güncellenmeli)
        public void AddItem(Product product, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive.");

            // Stok kontrolü yok! BUG #4
            _items.Add(new CartItem(product, quantity));
        }

        public void RemoveItem(int productId)
        {
            var item = _items.FirstOrDefault(i => i.Product.Id == productId);
            if (item != null)
                _items.Remove(item);
        }

        // BUG #5: Toplam hesaplama yanlış - discount sonrası negatif değer verebilir
        public decimal GetTotal()
        {
            decimal total = 0;
            foreach (var item in _items)
            {
                total += item.Product.Price * item.Quantity;
            }
            return total; // vergi ve indirim uygulanmıyor ama uygulandığı varsayılıyor
        }

        public int GetItemCount()
        {
            return _items.Count; // BUG #6: quantity toplamı değil, unique ürün sayısı
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}
