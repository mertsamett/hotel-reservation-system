// =============================================================================
// HOTELMODEL.CONTEXT.CS - DbContext Sinifi
// Entity Framework 6 ile veritabani baglantisi burada yapiliyor
// EDMX (Entity Data Model) dosyasindan turetilen DbContext
// Tum tablolar DbSet olarak tanimli, CRUD islemleri buradan yapiliyor
// =============================================================================

namespace DataAccess
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    // DbContext sinifi - veritabani islemleri icin ana sinif
    // Connection string (baglanti dizesi) App.config'den okunuyor
    // "name=HotelManagementEntities" kisminda hangi baglanti kullanilacak yazili
    public partial class HotelManagementEntities : DbContext
    {
        // Constructor (Yapilandirici) - base class'a baglanti adini geciriyoruz
        public HotelManagementEntities()
            : base("name=HotelManagementEntities")
        {
        }

        // OnModelCreating - Code First (Kod Oncelikli) yaklasimda kullanilir
        // Biz Database First (Veritabani Oncelikli) kullandigimiz icin exception firlatiyoruz
        // EDMX dosyasi modeli tanimliyor, bu metoda gerek yok
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        // DbSet'ler - Her tablo icin bir DbSet tanimliyoruz
        // virtual keyword (anahtar kelime) Lazy Loading (Tembel Yukleme) icin gerekli

        // Yetki tablosu
        public virtual DbSet<Permission> Permissions { get; set; }

        // Departman tablosu
        public virtual DbSet<Department> Departments { get; set; }

        // Personel tablosu
        public virtual DbSet<Staff> Staffs { get; set; }

        // Oda tipi tablosu - CONCEPT HIERARCHY (Kavram Hiyerarsisi)
        public virtual DbSet<RoomType> RoomTypes { get; set; }

        // Oda tablosu
        public virtual DbSet<Room> Rooms { get; set; }

        // Musteri tablosu
        public virtual DbSet<Customer> Customers { get; set; }

        // Rezervasyon tablosu - TERNARY RELATIONSHIP (Uclu Iliski)
        public virtual DbSet<Booking> Bookings { get; set; }

        // Oda servisi tablosu - WEAK ENTITY (Zayif Varlik)
        public virtual DbSet<RoomService> RoomServices { get; set; }
    }
}
