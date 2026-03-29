// =============================================================================
// CUSTOMER.CS - Musteri Sinifi
// Otelde konaklayan musterilerin bilgilerini tutuyoruz
// Ad, soyad, iletisim bilgileri, adres falan var
// TotalBookings alani kac kere rezervasyon yaptigini gosteriyor
// =============================================================================

namespace DataAccess
{
    using System;
    using System.Collections.Generic;

    // Musteri entity (varlik) sinifi
    // CUSTOMER tablosunun karsiligi bu sinif
    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            // Musterinin yaptigi rezervasyonlar
            this.Bookings = new HashSet<Booking>();
        }

        // Primary Key (Birincil Anahtar) - Identity (Otomatik Artan)
        public int CustomerID { get; set; }

        // Ad
        public string FirstName { get; set; }

        // Soyad
        public string LastName { get; set; }

        // E-posta - Unique (Benzersiz) olmali, ayni email iki kere kayit olamaz
        public string Email { get; set; }

        // Telefon numarasi
        public string Phone { get; set; }

        // Adres bilgileri
        public string Address { get; set; }
        public string City { get; set; }

        // Ulke - GroupBy (Gruplama) sorgularinda kullaniyoruz
        // Hangi ulkeden kac musteri var diye istatistik cikartiyoruz
        public string Country { get; set; }

        // Dogum tarihi
        public Nullable<System.DateTime> DateOfBirth { get; set; }

        // Toplam rezervasyon sayisi
        public Nullable<int> TotalBookings { get; set; }

        // Tarih alanlari
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        // Computed Property (Hesaplanmis Ozellik) - Ad soyad birlestirme
        // Veritabaninda tutmuyoruz, runtime (calisma zamani) da hesaplaniyor
        public string FullName => $"{FirstName} {LastName}";

        // Navigation Property (Gezinme Ozelligi) - Musterinin rezervasyonlari
        // Include("Bookings") diyerek JOIN yapabiliriz
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
