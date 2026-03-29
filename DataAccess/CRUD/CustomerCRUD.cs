// =============================================================================
// CUSTOMERCRUD.CS - Musteri CRUD Islemleri
// Create, Read, Update, Delete islemlerini yapiyoruz musteriler icin
// LINQ sorgulari burada - Where, OrderBy, GroupBy, Include falan
// Istatistik metodlari da var, dashboard icin kullaniyoruz
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using DataAccess.CRUD.Models;

namespace DataAccess.CRUD
{
    public class CustomerCRUD
    {
        // DbContext instance'i - veritabani islemleri icin kullaniyoruz
        private HotelManagementEntities db = new HotelManagementEntities();

        // =====================================================================
        // GET ALL - Tum musterileri getir
        // LINQ ORDER BY kullaniyoruz - soyada gore siralama
        // ThenBy ile ikincil siralama - ayni soyadlilar ada gore siralaniyor
        // =====================================================================
        public List<Customer> GetAllCustomers()
        {
            return db.Customers
                .OrderBy(c => c.LastName)        // Soyada gore sirala
                .ThenBy(c => c.FirstName)         // Sonra ada gore
                .ToList();                        // Listeye cevir
        }

        // =====================================================================
        // GET BY ID - ID ile musteri bul
        // Include kullaniyoruz, iliskili rezervasyonlari da cekiyor
        // EF6'da Include string parametre aliyor, EF Core'dan farkli
        // =====================================================================
        public Customer GetCustomerById(int id)
        {
            return db.Customers
                .Include("Bookings")              // JOIN islemi - rezervasyonlari da getir
                .FirstOrDefault(c => c.CustomerID == id);
        }

        // =====================================================================
        // GET BY EMAIL - Email ile musteri bul
        // Email unique (benzersiz) oldugu icin tek sonuc donuyor
        // =====================================================================
        public Customer GetCustomerByEmail(string email)
        {
            return db.Customers
                .FirstOrDefault(c => c.Email == email);
        }

        // =====================================================================
        // SEARCH - Musteri arama
        // LINQ WHERE ve Contains kullaniyoruz - SQL'deki LIKE gibi
        // Ad, soyad veya email'de arama yapiliyor
        // =====================================================================
        public List<Customer> SearchCustomers(string searchTerm)
        {
            searchTerm = searchTerm.ToLower();   // Kucuk harfe ceviriyoruz karsilastirma icin
            return db.Customers
                .Where(c => c.FirstName.ToLower().Contains(searchTerm) ||
                           c.LastName.ToLower().Contains(searchTerm) ||
                           c.Email.ToLower().Contains(searchTerm))
                .OrderBy(c => c.LastName)
                .ToList();
        }

        // =====================================================================
        // CREATE - Yeni musteri olustur
        // Tarih alanlari otomatik dolduruluyor
        // SaveChanges cagrilinca veritabanina yaziliyor
        // =====================================================================
        public void CreateCustomer(Customer customer)
        {
            customer.CreatedDate = DateTime.Now;
            customer.ModifiedDate = DateTime.Now;
            customer.TotalBookings = 0;           // Baslangicta 0 rezervasyon
            db.Customers.Add(customer);           // Context'e ekle
            db.SaveChanges();                      // Veritabanina yaz
        }

        // =====================================================================
        // UPDATE - Musteri guncelle
        // Once Find ile mevcut kaydi buluyoruz
        // Sonra alanlari tek tek guncelliyoruz
        // =====================================================================
        public void UpdateCustomer(Customer customer)
        {
            var existing = db.Customers.Find(customer.CustomerID);
            if (existing != null)
            {
                existing.FirstName = customer.FirstName;
                existing.LastName = customer.LastName;
                existing.Email = customer.Email;
                existing.Phone = customer.Phone;
                existing.Address = customer.Address;
                existing.City = customer.City;
                existing.Country = customer.Country;
                existing.DateOfBirth = customer.DateOfBirth;
                existing.ModifiedDate = DateTime.Now;  // Guncelleme tarihi
                db.SaveChanges();
            }
        }

        // =====================================================================
        // DELETE - Musteri sil
        // Dikkat! Aktif rezervasyonu olan musteri silinemiyor
        // Once kontrol ediyoruz, sonra siliyoruz
        // =====================================================================
        public bool DeleteCustomer(int id)
        {
            var customer = db.Customers.Find(id);
            if (customer != null)
            {
                // Aktif rezervasyon kontrolu - LINQ Any kullaniyoruz
                var hasActiveBookings = db.Bookings
                    .Any(b => b.CustomerID == id && b.BookingStatus == "Active");

                if (hasActiveBookings)
                {
                    return false; // Aktif rezervasyonu olan silinemez
                }

                db.Customers.Remove(customer);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        // =====================================================================
        // GET TOP CUSTOMERS - En cok rezervasyon yapan musteriler
        // LINQ ORDER BY DESC + TAKE kullaniyoruz
        // Dashboard'da "VIP musteriler" listesi icin
        // =====================================================================
        public List<Customer> GetTopCustomers(int count = 10)
        {
            return db.Customers
                .OrderByDescending(c => c.TotalBookings)  // Coktan aza
                .Take(count)                               // Ilk N tane
                .ToList();
        }

        // =====================================================================
        // FILTER BY COUNTRY - Ulkeye gore filtrele
        // LINQ WHERE kullaniyoruz
        // =====================================================================
        public List<Customer> GetCustomersByCountry(string country)
        {
            return db.Customers
                .Where(c => c.Country == country)
                .OrderBy(c => c.LastName)
                .ToList();
        }

        // =====================================================================
        // GET COUNT - Toplam musteri sayisi
        // Basit COUNT sorgusu
        // =====================================================================
        public int GetTotalCustomerCount()
        {
            return db.Customers.Count();
        }

        // =====================================================================
        // GROUP BY COUNTRY - Ulkeye gore gruplama (Istatistik icin)
        // LINQ GROUPBY kullaniyoruz - Dashboard grafikleri icin
        // Her ulkeden kac musteri var gosteriyor
        // SELECT Country, COUNT(*) FROM Customers GROUP BY Country gibi
        // =====================================================================
        public List<CustomerCountryStat> GetCustomerCountByCountry()
        {
            return db.Customers
                .GroupBy(c => c.Country)                  // Ulkeye gore grupla
                .Select(g => new CustomerCountryStat      // Sonucu model'e don
                {
                    Country = g.Key,                       // Grup anahtari (ulke)
                    CustomerCount = g.Count()              // Gruptaki eleman sayisi
                })
                .OrderByDescending(x => x.CustomerCount)   // Coktan aza sirala
                .ToList();
        }
    }
}
