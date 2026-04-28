# ECommerceApp - Test Raporu

Öğrenci: Ayşenur Er

Numara: 20230108030 

Ders: Yazılım Test ve Kalitesi  

Teslim Tarihi: 29.04.2026

---

## Proje Yapısı

```
ECommerceApp/
├── Core/
│   ├── Product.cs        → Ürün modeli (kasıtlı buglar içerir)
│   ├── Cart.cs           → Sepet işlemleri (kasıtlı buglar içerir)
│   └── OrderService.cs   → Sipariş & ödeme servisi (kasıtlı buglar içerir)
├── Tests/
│   ├── UnitTests/
│   │   └── ECommerceTests.cs   → Unit, Black Box, Gray Box testleri
│   └── IntegrationTests/
│       └── IntegrationTests.cs → Entegrasyon testleri
├── Program.cs
└── README.md ← Bu dosya
```

---

## Nasıl Çalıştırılır

```bash
# Projeyi klonla
git clone https://github.com/Aysenur2434
cd ECommerceApp

# Uygulamayı çalıştır
dotnet run --project ECommerceApp/ECommerceApp.csproj

# Testleri çalıştır
dotnet test ECommerceApp/Tests/ECommerceApp.Tests.csproj
```

---

## Sistemdeki Kasıtlı Buglar

| Bug # | Dosya | Metot | Açıklama |
|-------|-------|-------|----------|
| BUG-01 | Product.cs | `SetPrice()` | Negatif fiyat set edilebiliyor |
| BUG-02 | Product.cs | `DecreaseStock()` | Stok negatife düşebiliyor, her zaman `true` dönüyor |
| BUG-03 | Cart.cs | `AddItem()` | Aynı ürün tekrar eklenince ayrı item olarak ekleniyor |
| BUG-04 | Cart.cs | `AddItem()` | Stok kontrolü yapılmıyor |
| BUG-05 | Cart.cs | `GetTotal()` | İndirim/vergi uygulanmıyor ama uygulandığı varsayılıyor |
| BUG-06 | Cart.cs | `GetItemCount()` | Toplam adet yerine unique ürün sayısı dönüyor |
| BUG-07 | OrderService.cs | `PlaceOrder()` | Boş sepete sipariş verilebiliyor |
| BUG-08 | OrderService.cs | `ProcessPayment()` | Eksik ödeme tutarıyla sipariş onaylanıyor |
| BUG-09 | OrderService.cs | `CancelOrder()` | İptal edilmiş sipariş tekrar iptal edilebiliyor |
| BUG-10 | OrderService.cs | `ShipOrder()` | Pending durumdaki sipariş kargoya verilebiliyor |

---

## Test Case Tablosu

| TC # | Test Adı | Test Tipi | Beklenen | Gerçek Sonuç | Durum |
|------|----------|-----------|----------|--------------|-------|
| TC-01 | `SetPrice_NegativePrice_ShouldThrowException` | **White Box** | ArgumentException fırlatılmalı | Exception fırlatılmıyor | ❌ FAIL |
| TC-02 | `DecreaseStock_ExceedsStock_ShouldReturnFalse` | **White Box** | `false` dönmeli, stok ≥ 0 | `true` dönüyor, stok negatif | ❌ FAIL |
| TC-03 | `DecreaseStock_ValidQuantity_ShouldSucceed` | **White Box** | `true`, stok=7 | `true`, stok=7 | ✅ PASS |
| TC-04 | `IsAvailable_StockIsZero_ShouldReturnFalse` | **White Box** | `false` | `false` | ✅ PASS |
| TC-05 | `AddItem_SameProductTwice_ShouldMergeQuantities` | **Black Box** | 1 item, quantity=3 | 2 ayrı item | ❌ FAIL |
| TC-06 | `AddItem_ExceedsStock_ShouldThrowException` | **Black Box** | Exception fırlatılmalı | Exception yok | ❌ FAIL |
| TC-07 | `GetItemCount_ReturnsCorrectTotalQuantity` | **Black Box** | 5 (3+2) | 2 (unique ürün sayısı) | ❌ FAIL |
| TC-08 | `AddItem_ZeroQuantity_ShouldThrowException` | **Black Box** | ArgumentException | ArgumentException | ✅ PASS |
| TC-09 | `RemoveItem_ExistingProduct_ShouldBeRemovedFromCart` | **Black Box** | Sepet boş | Sepet boş | ✅ PASS |
| TC-10 | `PlaceOrder_EmptyCart_ShouldThrowException` | **Gray Box** | Exception fırlatılmalı | Exception yok | ❌ FAIL |
| TC-11 | `ProcessPayment_InsufficientAmount_ShouldReturnFalse` | **Gray Box** | `false`, status=Pending | `true`, status=Confirmed | ❌ FAIL |
| TC-12 | `CancelOrder_AlreadyCancelled_ShouldReturnFalse` | **Gray Box** | `false` | `true` | ❌ FAIL |
| TC-13 | `ShipOrder_PendingOrder_ShouldReturnFalse` | **Gray Box** | `false` | `true` | ❌ FAIL |
| TC-14 | `ProcessPayment_ExactAmount_ShouldConfirmOrder` | **Gray Box** | `true`, Confirmed | `true`, Confirmed | ✅ PASS |
| TC-15 | `FullPurchaseFlow_ValidProducts_ShouldSucceed` | **Integration** | Tüm adımlar başarılı | ShipOrder Confirmed kontrolü yok | ❌ FAIL |
| TC-16 | `PurchaseFlow_OutOfStockProduct_ShouldFailAtCartLevel` | **Integration** | Exception fırlatılmalı | Exception yok | ❌ FAIL |
| TC-17 | `PlaceOrder_ShouldDecreaseProductStock` | **Integration** | Stok azalmalı | Stok azalmıyor | ❌ FAIL |
| TC-18 | `MultipleOrders_ShouldBeTrackedIndependently` | **Integration** | Bağımsız yönetim | Bağımsız yönetim | ✅ PASS |

---

## Test Sonuç Özeti

| Test Tipi | Toplam | PASS | FAIL |
|-----------|--------|------|------|
| White Box (Unit) | 4 | 2 | 2 |
| Black Box | 5 | 2 | 3 |
| Gray Box | 5 | 1 | 4 |
| Integration | 4 | 1 | 3 |
| **TOPLAM** | **18** | **6** | **12** |

---

## FAIL Olan Testlerin Detaylı Analizi

### TC-01 — SetPrice_NegativePrice_ShouldThrowException ❌ FAIL
**Test Tipi:** White Box  
**Neden Fail?** `Product.SetPrice()` metodunda hiçbir validasyon bulunmuyor. Negatif değer direkt olarak `Price` property'sine atanıyor. Gerçek dünyada fiyat negatif olamaz; bu ciddi bir iş mantığı hatasıdır.  
**Düzeltme:** `if (newPrice < 0) throw new ArgumentException("Price cannot be negative.");`

---

### TC-02 — DecreaseStock_ExceedsStock_ShouldReturnFalse ❌ FAIL
**Test Tipi:** White Box  
**Neden Fail?** `DecreaseStock()` yeterli stok olup olmadığını kontrol etmeden quantity'yi çıkartıyor. Bu negatif stoka (örn: -90) yol açıyor. Ayrıca her koşulda `true` dönüyor.  
**Düzeltme:** Önce `if (quantity > Stock) return false;` kontrolü eklenmalı.

---

### TC-05 — AddItem_SameProductTwice_ShouldMergeQuantities ❌ FAIL
**Test Tipi:** Black Box  
**Neden Fail?** `Cart.AddItem()` mevcut ürün var mı diye kontrol etmiyor. Aynı ürün iki kez eklenince `_items` listesine iki ayrı `CartItem` ekleniyor. Kullanıcı aynı ürünü iki kez ekleyince çift fatura görür.  
**Düzeltme:** Mevcut item bulunursa `item.Quantity += quantity` yapılmalı, yoksa yeni item eklenmalı.

---

### TC-06 — AddItem_ExceedsStock_ShouldThrowException ❌ FAIL
**Test Tipi:** Black Box  
**Neden Fail?** `AddItem()` ürünün stok durumuna bakmıyor. Stok 0 olan ürün bile 100 adet eklenebiliyor. Bu sipariş aşamasında tutarsızlığa yol açar.  
**Düzeltme:** `if (quantity > product.Stock) throw new InvalidOperationException("Insufficient stock.");`

---

### TC-07 — GetItemCount_ReturnsCorrectTotalQuantity ❌ FAIL
**Test Tipi:** Black Box  
**Neden Fail?** `GetItemCount()` `_items.Count` dönüyor. Bu unique ürün (CartItem) sayısıdır. 3 adet laptop + 2 adet mouse için 5 dönmeli, 2 dönüyor.  
**Düzeltme:** `return _items.Sum(i => i.Quantity);`

---

### TC-10 — PlaceOrder_EmptyCart_ShouldThrowException ❌ FAIL
**Test Tipi:** Gray Box  
**Neden Fail?** `OrderService.PlaceOrder()` sepet boş mu diye kontrol etmiyor. Boş sepete sipariş oluşturuluyor, 0 TL tutarlı geçersiz bir sipariş kayıt ediliyor.  
**Düzeltme:** `if (!cart.Items.Any()) throw new InvalidOperationException("Cart is empty.");`

---

### TC-11 — ProcessPayment_InsufficientAmount_ShouldReturnFalse ❌ FAIL
**Test Tipi:** Gray Box  
**Neden Fail?** `ProcessPayment()` sadece `paymentAmount > 0` kontrolü yapıyor. Sipariş tutarından 1 kuruş bile eksik ödeme yapılsa siparişi onaylıyor. Bu ciddi bir finansal güvenlik açığıdır.  
**Düzeltme:** `if (paymentAmount < order.TotalAmount) return false;`

---

### TC-12 — CancelOrder_AlreadyCancelled_ShouldReturnFalse ❌ FAIL
**Test Tipi:** Gray Box  
**Neden Fail?** `CancelOrder()` siparişin mevcut durumunu kontrol etmiyor. Zaten iptal edilmiş bir sipariş tekrar "iptal ediliyor" ve `true` dönüyor. Bu idempotency ihlalidir.  
**Düzeltme:** `if (order.Status == OrderStatus.Cancelled) return false;`

---

### TC-13 — ShipOrder_PendingOrder_ShouldReturnFalse ❌ FAIL
**Test Tipi:** Gray Box  
**Neden Fail?** `ShipOrder()` siparişin `Confirmed` olup olmadığını kontrol etmiyor. Ödeme yapılmamış (`Pending`) bir sipariş kargoya verilebiliyor. Bu hem iş süreci hem de finansal açıdan kritik bir hatadır.  
**Düzeltme:** `if (order.Status != OrderStatus.Confirmed) return false;`

---

### TC-15 — FullPurchaseFlow_ValidProducts_ShouldSucceed ❌ FAIL
**Test Tipi:** Integration  
**Neden Fail?** Tam akış testi sırasında `ShipOrder()` çağrısı `true` dönse de `Confirmed` durumu kontrolü olmadığı için akış tutarsız ilerleyebiliyor. TC-13'teki bug entegrasyon seviyesinde de kendini gösteriyor.

---

### TC-16 — PurchaseFlow_OutOfStockProduct_ShouldFailAtCartLevel ❌ FAIL
**Test Tipi:** Integration  
**Neden Fail?** TC-06'daki stok kontrolü eksikliği entegrasyon seviyesinde de fail veriyor. Stok 0 olan ürün sepete eklenebiliyor.

---

### TC-17 — PlaceOrder_ShouldDecreaseProductStock ❌ FAIL
**Test Tipi:** Integration  
**Neden Fail?** `PlaceOrder()` sadece Cart'ı temizleyip Order oluşturuyor. Sipariş verilen ürünlerin stoklarını düşürmüyor. Bu veri tutarsızlığına yol açar: aynı stoklu ürün defalarca satılabilir.  
**Düzeltme:** `PlaceOrder()` içinde her item için `item.Product.DecreaseStock(item.Quantity)` çağrılmalı.

---

## Öğrenilen Dersler

1. **Validasyon eksikliği** en yaygın bug kaynağıdır (BUG-01, 04, 07, 08).
2. **İş mantığı kuralları** (durum geçişleri) kodda açıkça uygulanmalıdır (BUG-09, 10).
3. **Entegrasyon katmanı** bağımsız modüllerde gözükmeyen hataları ortaya çıkarır (TC-17).
4. **Black Box testler** implementasyonu bilmeden davranış tutarsızlıklarını yakalar.
5. **Unit testler** en küçük birimi izole ederek hata kaynağını kesin olarak belirler.
