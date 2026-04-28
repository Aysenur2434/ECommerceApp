using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests.IntegrationTests
{
    // ==========================================
    // 4. INTEGRATION TESTS
    // Tüm katmanların birlikte çalışmasını test ediyoruz
    // Ürün → Sepet → Sipariş → Ödeme akışı
    // ==========================================
    [TestFixture]
    public class ECommerceIntegrationTests
    {
        private OrderService _orderService;
        private Cart _cart;
        private Product _laptop;
        private Product _phone;
        private Product _outOfStock;

        [SetUp]
        public void Setup()
        {
            _orderService = new OrderService();
            _cart = new Cart();
            _laptop = new Product(1, "Laptop", 15000m, 5);
            _phone = new Product(2, "Telefon", 8000m, 10);
            _outOfStock = new Product(3, "Tablet", 5000m, 0); // stok yok
        }

        // TC-15: INTEGRATION - Tam başarılı alışveriş akışı
        [Test]
        public void FullPurchaseFlow_ValidProducts_ShouldSucceed()
        {
            // Arrange & Act
            _cart.AddItem(_laptop, 1);
            _cart.AddItem(_phone, 1);

            decimal expectedTotal = 15000m + 8000m; // 23000 TL
            Assert.That(_cart.GetTotal(), Is.EqualTo(expectedTotal), "Sepet toplamı hatalı.");

            var order = _orderService.PlaceOrder(_cart);
            Assert.That(order, Is.Not.Null, "Sipariş oluşturulmalı.");
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Pending), "Yeni sipariş Pending olmalı.");
            Assert.That(_cart.Items.Count, Is.EqualTo(0), "Sipariş sonrası sepet temizlenmeli.");

            bool paid = _orderService.ProcessPayment(order, expectedTotal);
            Assert.That(paid, Is.True, "Ödeme başarılı olmalı.");
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Confirmed), "Ödeme sonrası Confirmed olmalı.");

            bool shipped = _orderService.ShipOrder(order.OrderId);
            Assert.That(shipped, Is.True, "Confirmed sipariş kargoya verilebilmeli.");
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Shipped), "Kargo durumu Shipped olmalı.");
        }

        // TC-16: INTEGRATION - Stokta olmayan ürün sipariş edilememeli
        [Test]
        public void PurchaseFlow_OutOfStockProduct_ShouldFailAtCartLevel()
        {
            // Act & Assert
            // BUG #4 ile ilgili: stok kontrolü cart'ta yapılmıyor
            Assert.Throws<InvalidOperationException>(
                () => _cart.AddItem(_outOfStock, 1),
                "Stokta olmayan ürün sepete eklenince hata fırlatılmalı."
            );
        }

        // TC-17: INTEGRATION - Sipariş stoku azaltmalı
        [Test]
        public void PlaceOrder_ShouldDecreaseProductStock()
        {
            // Arrange
            int initialStock = _laptop.Stock; // 5
            _cart.AddItem(_laptop, 2);

            // Act
            _orderService.PlaceOrder(_cart);

            // Assert
            // Sipariş verilince ürün stoğu düşmeli, bu entegrasyon şu an eksik
            Assert.That(_laptop.Stock, Is.EqualTo(initialStock - 2),
                "Sipariş verilince ürün stoğu azalmalı.");
        }

        // TC-18: INTEGRATION - Birden fazla sipariş bağımsız yönetilmeli
        [Test]
        public void MultipleOrders_ShouldBeTrackedIndependently()
        {
            // Arrange - İlk sipariş
            _cart.AddItem(_laptop, 1);
            var order1 = _orderService.PlaceOrder(_cart);

            // İkinci sipariş
            _cart.AddItem(_phone, 1);
            var order2 = _orderService.PlaceOrder(_cart);

            // Assert
            Assert.That(order1.OrderId, Is.Not.EqualTo(order2.OrderId),
                "Farklı siparişlerin ID'leri farklı olmalı.");
            Assert.That(_orderService.GetAllOrders().Count, Is.EqualTo(2),
                "İki sipariş kayıtlı olmalı.");

            // Birini iptal et, diğeri etkilenmemeli
            _orderService.CancelOrder(order1.OrderId);
            Assert.That(order1.Status, Is.EqualTo(OrderStatus.Cancelled));
            Assert.That(order2.Status, Is.EqualTo(OrderStatus.Pending),
                "Bir siparişin iptali diğerini etkilememeli.");
        }
    }
}
