// =============================================================================
// BOOKINGCRUD.CS - Rezervasyon CRUD Islemleri
// En onemli CRUD sinifi bu, TRANSACTION (Islem) yonetimi burada
// TERNARY RELATIONSHIP (Uclu Iliski) sorgulari var - Customer, Room, Staff JOIN
// Check-in, Check-out, Cancel islemleri de burada
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using DataAccess.CRUD.Models;

namespace DataAccess.CRUD
{
    public class BookingCRUD
    {
        // DbContext instance'i
        private HotelManagementEntities db = new HotelManagementEntities();

        // =====================================================================
        // GET ALL - Tum rezervasyonlari getir
        // Include ile JOIN yapiyoruz - Customer, Room, RoomType, Staff
        // EF6'da Include string parametre aliyor dikkat et
        // Nested (ic ice) Include icin "Room.RoomType" yaziyoruz
        // =====================================================================
        public List<Booking> GetAllBookings()
        {
            return db.Bookings
                .Include("Customer")           // Musteri bilgisi
                .Include("Room")               // Oda bilgisi
                .Include("Room.RoomType")      // Oda tipi (nested include)
                .Include("Staff")              // Personel bilgisi
                .OrderByDescending(b => b.BookingDate)  // Yeniden eskiye
                .ToList();
        }

        // =====================================================================
        // GET ACTIVE - Aktif rezervasyonlar
        // LINQ WHERE kullaniyoruz - BookingStatus filtreleme
        // =====================================================================
        public List<Booking> GetActiveBookings()
        {
            return db.Bookings
                .Include("Customer")
                .Include("Room")
                .Include("Room.RoomType")
                .Include("Staff")
                .Where(b => b.BookingStatus == "Active")  // Sadece aktifler
                .OrderBy(b => b.CheckInDate)              // Yakin tarihe gore sirala
                .ToList();
        }

        // =====================================================================
        // GET BY STATUS - Duruma gore filtrele
        // Active, Completed, Cancelled olabilir
        // =====================================================================
        public List<Booking> GetBookingsByStatus(string status)
        {
            return db.Bookings
                .Include("Customer")
                .Include("Room")
                .Include("Room.RoomType")
                .Where(b => b.BookingStatus == status)
                .OrderByDescending(b => b.BookingDate)
                .ToList();
        }

        // =====================================================================
        // GET BY ID - ID ile rezervasyon bul
        // FirstOrDefault null donebilir, kontrol etmek lazim
        // =====================================================================
        public Booking GetBookingById(int id)
        {
            return db.Bookings
                .Include("Customer")
                .Include("Room")
                .Include("Room.RoomType")
                .Include("Staff")
                .FirstOrDefault(b => b.BookingID == id);
        }

        // =====================================================================
        // GET BY CUSTOMER - Musteriye ait rezervasyonlar
        // Musterinin gecmis ve aktif tum rezervasyonlari
        // =====================================================================
        public List<Booking> GetBookingsByCustomer(int customerId)
        {
            return db.Bookings
                .Include("Room")
                .Include("Room.RoomType")
                .Where(b => b.CustomerID == customerId)
                .OrderByDescending(b => b.BookingDate)
                .ToList();
        }

        // =====================================================================
        // GET TODAY'S CHECK-INS - Bugunun girisleri
        // DbFunctions.TruncateTime ile sadece tarih kismini karsilastiriyoruz
        // Saat kismi olmadan karsilastirma yapiliyor boylece
        // =====================================================================
        public List<Booking> GetTodayCheckIns()
        {
            var today = DateTime.Today;
            return db.Bookings
                .Include("Customer")
                .Include("Room")
                .Where(b => DbFunctions.TruncateTime(b.CheckInDate) == today && b.BookingStatus == "Active")
                .ToList();
        }

        // =====================================================================
        // GET TODAY'S CHECK-OUTS - Bugunun cikislari
        // Ayni mantik, CheckOutDate'e bakiyoruz
        // =====================================================================
        public List<Booking> GetTodayCheckOuts()
        {
            var today = DateTime.Today;
            return db.Bookings
                .Include("Customer")
                .Include("Room")
                .Where(b => DbFunctions.TruncateTime(b.CheckOutDate) == today && b.BookingStatus == "Active")
                .ToList();
        }

        // =====================================================================
        // CREATE - Yeni rezervasyon olustur
        // TRANSACTION (Islem) kullaniyoruz - ya hep ya hic
        // Birden fazla tabloda degisiklik yapiyoruz atomik olmasi lazim
        // Hata olursa Rollback, basarili olursa Commit
        // =====================================================================
        public bool CreateBooking(Booking booking)
        {
            // Transaction (Islem) baslat
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // 1. Oda musait mi kontrol et (double-check)
                    // SQL Trigger da yapar ama guvenlik icin C# tarafinda da kontrol ediyoruz
                    var room = db.Rooms.Find(booking.RoomID);
                    if (room == null || room.IsAvailable != true)
                    {
                        transaction.Rollback();  // Geri al
                        return false;            // Oda musait degil
                    }

                    // 2. Rezervasyonu kaydet
                    booking.BookingDate = DateTime.Now;
                    booking.CreatedDate = DateTime.Now;
                    booking.ModifiedDate = DateTime.Now;
                    booking.BookingStatus = "Active";
                    db.Bookings.Add(booking);
                    db.SaveChanges();

                    // 3. Odayi dolu olarak isaretle
                    // SQL Trigger da yapar ama consistency (tutarlilik) icin burada da yapiyoruz
                    room.IsAvailable = false;
                    room.ModifiedDate = DateTime.Now;
                    db.SaveChanges();

                    // 4. Musterinin toplam rezervasyon sayisini artir
                    var customer = db.Customers.Find(booking.CustomerID);
                    if (customer != null)
                    {
                        customer.TotalBookings = (customer.TotalBookings ?? 0) + 1;
                        customer.ModifiedDate = DateTime.Now;
                        db.SaveChanges();
                    }

                    transaction.Commit();  // Her sey basarili, onayla
                    return true;
                }
                catch
                {
                    transaction.Rollback();  // Hata oldu, geri al
                    return false;
                }
            }
        }

        // =====================================================================
        // UPDATE - Rezervasyon guncelle
        // Tarih ve fiyat degisiklikleri icin
        // =====================================================================
        public void UpdateBooking(Booking booking)
        {
            var existing = db.Bookings.Find(booking.BookingID);
            if (existing != null)
            {
                existing.CheckInDate = booking.CheckInDate;
                existing.CheckOutDate = booking.CheckOutDate;
                existing.TotalPrice = booking.TotalPrice;
                existing.Notes = booking.Notes;
                existing.StaffID = booking.StaffID;
                existing.ModifiedDate = DateTime.Now;
                db.SaveChanges();
            }
        }

        // =====================================================================
        // CHECKOUT - Check-out islemi
        // TRANSACTION (Islem) ile yapiyoruz
        // Booking'i Completed yap, odayi musait yap
        // =====================================================================
        public bool CheckoutBooking(int bookingId)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var booking = db.Bookings.Find(bookingId);
                    if (booking == null || booking.BookingStatus != "Active")
                    {
                        transaction.Rollback();
                        return false;
                    }

                    // 1. Rezervasyon durumunu Completed yap
                    booking.BookingStatus = "Completed";
                    booking.ModifiedDate = DateTime.Now;
                    db.SaveChanges();

                    // 2. Odayi musait yap
                    // Trigger da yapar ama burada da yapiyoruz guvenlik icin
                    var room = db.Rooms.Find(booking.RoomID);
                    if (room != null)
                    {
                        room.IsAvailable = true;
                        room.ModifiedDate = DateTime.Now;
                        db.SaveChanges();
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        // =====================================================================
        // CANCEL - Rezervasyon iptal
        // Checkout'a benziyor ama durum Cancelled oluyor
        // =====================================================================
        public bool CancelBooking(int bookingId)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var booking = db.Bookings.Find(bookingId);
                    if (booking == null || booking.BookingStatus != "Active")
                    {
                        transaction.Rollback();
                        return false;
                    }

                    // 1. Durumu Cancelled yap
                    booking.BookingStatus = "Cancelled";
                    booking.ModifiedDate = DateTime.Now;
                    db.SaveChanges();

                    // 2. Odayi musait yap
                    var room = db.Rooms.Find(booking.RoomID);
                    if (room != null)
                    {
                        room.IsAvailable = true;
                        room.ModifiedDate = DateTime.Now;
                        db.SaveChanges();
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        // =====================================================================
        // DELETE - Rezervasyon sil
        // Aktif ise once iptal et sonra sil
        // =====================================================================
        public bool DeleteBooking(int id)
        {
            var booking = db.Bookings.Find(id);
            if (booking != null)
            {
                // Aktif ise once iptal et ki oda serbest kalsin
                if (booking.BookingStatus == "Active")
                {
                    CancelBooking(id);
                }
                db.Bookings.Remove(booking);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        // =====================================================================
        // STATISTICS - Istatistik metodlari
        // Dashboard icin kullaniyoruz bunlari
        // =====================================================================

        // Aktif rezervasyon sayisi - COUNT ile WHERE
        public int GetActiveBookingCount()
        {
            return db.Bookings.Count(b => b.BookingStatus == "Active");
        }

        // =====================================================================
        // MONTHLY REVENUE - Aylik gelir
        // LINQ SUM kullaniyoruz - toplam hesaplama
        // Iptal edilmemis rezervasyonlarin toplam fiyati
        // =====================================================================
        public decimal GetMonthlyRevenue(int month, int year)
        {
            var revenue = db.Bookings
                .Where(b => b.BookingDate.HasValue &&
                           b.BookingDate.Value.Month == month &&
                           b.BookingDate.Value.Year == year &&
                           b.BookingStatus != "Cancelled")
                .Sum(b => b.TotalPrice);
            return revenue ?? 0;
        }

        // =====================================================================
        // ROOM OCCUPANCY STATS - Oda doluluk istatistikleri
        // LINQ GROUPBY kullaniyoruz - Dashboard grafikleri icin
        // Her oda tipinden kac tane var, kaci dolu kaci bos
        // =====================================================================
        public List<RoomOccupancyStat> GetRoomOccupancyStats()
        {
            return db.Rooms
                .Include("RoomType")
                .GroupBy(r => r.RoomType.TypeName)  // Oda tipine gore grupla
                .Select(g => new RoomOccupancyStat
                {
                    TypeName = g.Key,                                   // Grup anahtari
                    TotalCount = g.Count(),                             // Toplam oda
                    AvailableCount = g.Count(r => r.IsAvailable == true),  // Musait
                    OccupiedCount = g.Count(r => r.IsAvailable == false)   // Dolu
                })
                .ToList();
        }

        // =====================================================================
        // FILTER BY DATE RANGE - Tarih araligina gore filtrele
        // Raporlama icin kullaniyoruz
        // =====================================================================
        public List<Booking> GetBookingsByDateRange(DateTime startDate, DateTime endDate)
        {
            return db.Bookings
                .Include("Customer")
                .Include("Room")
                .Where(b => b.CheckInDate >= startDate && b.CheckOutDate <= endDate)
                .OrderBy(b => b.CheckInDate)
                .ToList();
        }
    }
}
