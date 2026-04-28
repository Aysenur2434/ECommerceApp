namespace ECommerceApp.Core
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Shipped,
        Delivered,
        Cancelled
    }

    public class Order
    {
        public int OrderId { get; set; }
        public List<CartItem> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public Order(int orderId, List<CartItem> items, decimal totalAmount)
        {
            OrderId = orderId;
            Items = items;
            TotalAmount = totalAmount;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.Now;
        }
    }

    public class OrderService
    {
        private List<Order> _orders = new List<Order>();
        private int _nextOrderId = 1;

        // BUG #7: Sepet boşsa sipariş oluşturulabiliyor
        public Order PlaceOrder(Cart cart)
        {
            // Boş sepet kontrolü yok!
            var items = cart.Items.ToList();
            decimal total = cart.GetTotal();

            var order = new Order(_nextOrderId++, items, total);
            _orders.Add(order);
            cart.Clear();

            return order;
        }

        // BUG #8: Ödeme tutarı sipariş tutarıyla eşleşmeden ödeme onaylanıyor
        public bool ProcessPayment(Order order, decimal paymentAmount)
        {
            if (paymentAmount <= 0)
                return false;

            // paymentAmount == order.TotalAmount kontrolü yapılmıyor!
            order.Status = OrderStatus.Confirmed;
            return true;
        }

        public Order? GetOrder(int orderId)
        {
            return _orders.FirstOrDefault(o => o.OrderId == orderId);
        }

        public List<Order> GetAllOrders()
        {
            return _orders;
        }

        // BUG #9: İptal edilmiş siparişler de iptal edilebilir (durum kontrolü yok)
        public bool CancelOrder(int orderId)
        {
            var order = GetOrder(orderId);
            if (order == null) return false;

            order.Status = OrderStatus.Cancelled;
            return true;
        }

        // BUG #10: Kargo durumuna geçiş için Confirmed kontrolü yapılmıyor
        public bool ShipOrder(int orderId)
        {
            var order = GetOrder(orderId);
            if (order == null) return false;

            order.Status = OrderStatus.Shipped;
            return true; // Pending durumundaki sipariş de kargoya verilebilir!
        }
    }
}
