using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests.UnitTests
{
    // ==========================================
    // 1. WHITE BOX (UNIT) TESTS - Product
    // İç mantığı bilerek test ediyoruz
    // ==========================================
    [TestFixture]
    public class ProductWhiteBoxTests
    {
        private Product _product;

        [SetUp]
        public void Setup()
        {
            _product = new Product(1, "Test Ürünü", 100m, 10);
        }

        // TC-01: WHITE BOX - SetPrice negatif değer kabul etmemeli
        [Test]
        public void SetPrice_NegativePrice_ShouldThrowException()
        {
            // Arrange
            decimal negativePrice = -50m;

            // Act & Assert
            // BUG #1: Bu test FAIL olacak çünkü negatif fiyata izin veriliyor
            Assert.Throws<ArgumentException>(() => _product.SetPrice(negativePrice),
                "Negatif fiyat set edildiğinde ArgumentException fırlatılmalı.");
        }

        // TC-02: WHITE BOX - DecreaseStock stoku negatife düşürmemeli
        [Test]
        public void DecreaseStock_ExceedsStock_ShouldReturnFalse()
        {
            // Arrange
            int excessiveQuantity = 100; // Stok sadece 10

            // Act
            // BUG #2: Bu test FAIL olacak çünkü stok negatife düşüyor ve true dönüyor
            bool result = _product.DecreaseStock(excessiveQuantity);

            // Assert
            Assert.That(result, Is.False, "Stok yetersizse false dönmeli.");
            Assert.That(_product.Stock, Is.GreaterThanOrEqualTo(0), "Stok negatif olmamalı.");
        }

        // TC-03: WHITE BOX - Geçerli stok azaltma başarılı olmalı
        [Test]
        public void DecreaseStock_ValidQuantity_ShouldSucceed()
        {
            // Arrange
            int quantity = 3;

            // Act
            bool result = _product.DecreaseStock(quantity);

            // Assert
            Assert.That(result, Is.True, "Geçerli miktarda stok azaltma başarılı olmalı.");
            Assert.That(_product.Stock, Is.EqualTo(7), "Stok doğru hesaplanmalı.");
        }

        // TC-04: WHITE BOX - IsAvailable stok 0 iken false dönmeli
        [Test]
        public void IsAvailable_StockIsZero_ShouldReturnFalse()
        {
            // Arrange
            var emptyStockProduct = new Product(2, "Tükendi", 50m, 0);

            // Act
            bool available = emptyStockProduct.IsAvailable();

            // Assert
            Assert.That(available, Is.False, "Stok 0 iken IsAvailable false dönmeli.");
        }
    }

    // ==========================================
    // 2. BLACK BOX TESTS - Cart
    // Sadece giriş/çıkış davranışını test ediyoruz
    // ==========================================
    [TestFixture]
    public class CartBlackBoxTests
    {
        private Cart _cart;
        private Product _product1;
        private Product _product2;

        [SetUp]
        public void Setup()
        {
            _cart = new Cart();
            _product1 = new Product(1, "Laptop", 15000m, 5);
            _product2 = new Product(2, "Mouse", 500m, 20);
        }

        // TC-05: BLACK BOX - Aynı ürün iki kez eklenince quantity birleşmeli
        [Test]
        public void AddItem_SameProductTwice_ShouldMergeQuantities()
        {
            // Act
            _cart.AddItem(_product1, 1);
            _cart.AddItem(_product1, 2);

            // Assert
            // BUG #3: Bu test FAIL olacak çünkü aynı ürün iki ayrı item olarak ekleniyor
            Assert.That(_cart.Items.Count, Is.EqualTo(1),
                "Aynı ürün tekrar eklenince item sayısı artmamalı, quantity güncellenmalı.");
            Assert.That(_cart.Items[0].Quantity, Is.EqualTo(3),
                "Aynı ürün eklenince toplam quantity 3 olmalı.");
        }

        // TC-06: BLACK BOX - Stoktan fazla ürün eklenemez
        [Test]
        public void AddItem_ExceedsStock_ShouldThrowException()
        {
            // Act & Assert
            // BUG #4: Bu test FAIL olacak çünkü stok kontrolü yapılmıyor
            Assert.Throws<InvalidOperationException>(() => _cart.AddItem(_product1, 100),
                "Stoktan fazla ürün eklenince exception fırlatılmalı.");
        }

        // TC-07: BLACK BOX - GetItemCount toplam adet sayısını vermeli
        [Test]
        public void GetItemCount_ReturnsCorrectTotalQuantity()
        {
            // Arrange
            _cart.AddItem(_product1, 3);
            _cart.AddItem(_product2, 2);

            // Act
            int count = _cart.GetItemCount();

            // Assert
            // BUG #6: Bu test FAIL olacak çünkü GetItemCount unique ürün sayısı dönüyor (2), toplam 5 değil
            Assert.That(count, Is.EqualTo(5),
                "GetItemCount toplam ürün adedini (quantity toplamı) dönmeli.");
        }

        // TC-08: BLACK BOX - Sıfır quantity ile ürün eklenemez
        [Test]
        public void AddItem_ZeroQuantity_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => _cart.AddItem(_product1, 0),
                "Sıfır quantity ile ürün eklenince ArgumentException fırlatılmalı.");
        }

        // TC-09: BLACK BOX - Ürün kaldırıldıktan sonra sepetten silinmeli
        [Test]
        public void RemoveItem_ExistingProduct_ShouldBeRemovedFromCart()
        {
            // Arrange
            _cart.AddItem(_product1, 1);

            // Act
            _cart.RemoveItem(_product1.Id);

            // Assert
            Assert.That(_cart.Items.Count, Is.EqualTo(0), "Ürün kaldırıldıktan sonra sepet boş olmalı.");
        }
    }

    // ==========================================
    // 3. GRAY BOX TESTS - OrderService
    // Kısmi iç bilgiyle test ediyoruz
    // ==========================================
    [TestFixture]
    public class OrderServiceGrayBoxTests
    {
        private OrderService _orderService;
        private Cart _cart;
        private Product _product;

        [SetUp]
        public void Setup()
        {
            _orderService = new OrderService();
            _cart = new Cart();
            _product = new Product(1, "Laptop", 15000m, 5);
        }

        // TC-10: GRAY BOX - Boş sepete sipariş verilemez
        [Test]
        public void PlaceOrder_EmptyCart_ShouldThrowException()
        {
            // Act & Assert
            // BUG #7: Bu test FAIL olacak çünkü boş sepet kontrolü yok
            Assert.Throws<InvalidOperationException>(() => _orderService.PlaceOrder(_cart),
                "Boş sepete sipariş verilince InvalidOperationException fırlatılmalı.");
        }

        // TC-11: GRAY BOX - Eksik ödeme tutarıyla sipariş onaylanmamalı
        [Test]
        public void ProcessPayment_InsufficientAmount_ShouldReturnFalse()
        {
            // Arrange
            _cart.AddItem(_product, 1);
            var order = _orderService.PlaceOrder(_cart);
            decimal insufficientAmount = order.TotalAmount - 1m; // 1 TL eksik

            // Act
            // BUG #8: Bu test FAIL olacak çünkü tutar kontrolü yapılmıyor
            bool result = _orderService.ProcessPayment(order, insufficientAmount);

            // Assert
            Assert.That(result, Is.False, "Eksik ödeme tutarı kabul edilmemeli.");
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Pending),
                "Başarısız ödemede sipariş Pending kalmalı.");
        }

        // TC-12: GRAY BOX - İptal edilmiş sipariş tekrar iptal edilemez
        [Test]
        public void CancelOrder_AlreadyCancelled_ShouldReturnFalse()
        {
            // Arrange
            _cart.AddItem(_product, 1);
            var order = _orderService.PlaceOrder(_cart);
            _orderService.CancelOrder(order.OrderId);

            // Act
            // BUG #9: Bu test FAIL olacak çünkü durum kontrolü yapılmıyor
            bool result = _orderService.CancelOrder(order.OrderId);

            // Assert
            Assert.That(result, Is.False,
                "Zaten iptal edilmiş sipariş tekrar iptal edilemez, false dönmeli.");
        }

        // TC-13: GRAY BOX - Confirmed olmayan sipariş kargoya verilemez
        [Test]
        public void ShipOrder_PendingOrder_ShouldReturnFalse()
        {
            // Arrange
            _cart.AddItem(_product, 1);
            var order = _orderService.PlaceOrder(_cart);
            // Ödeme yapılmadan (hala Pending durumunda)

            // Act
            // BUG #10: Bu test FAIL olacak çünkü Confirmed kontrolü yapılmıyor
            bool result = _orderService.ShipOrder(order.OrderId);

            // Assert
            Assert.That(result, Is.False,
                "Pending durumundaki sipariş kargoya verilemez.");
        }

        // TC-14: GRAY BOX - Başarılı tam ödeme sonrası sipariş Confirmed olmalı
        [Test]
        public void ProcessPayment_ExactAmount_ShouldConfirmOrder()
        {
            // Arrange
            _cart.AddItem(_product, 1);
            var order = _orderService.PlaceOrder(_cart);

            // Act
            bool result = _orderService.ProcessPayment(order, order.TotalAmount);

            // Assert
            Assert.That(result, Is.True, "Tam tutar ödenince ödeme başarılı olmalı.");
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Confirmed),
                "Ödeme sonrası sipariş Confirmed olmalı.");
        }
    }
}
