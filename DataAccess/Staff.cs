// =============================================================================
// STAFF.CS - Personel Sinifi
// Otel calisanlarinin bilgilerini tutuyoruz burada
// Departman ve Permission tablolariyla iliskili, Foreign Key (Yabanci Anahtar) var
// Ayrica Booking tablosuyla da iliskisi var - hangi personel hangi rezervasyonu aldiysa
// =============================================================================

namespace DataAccess
{
    using System;
    using System.Collections.Generic;

    // Personel entity (varlik) sinifi
    // STAFF tablosunun C# tarafindaki karsiligi bu
    public partial class Staff
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Staff()
        {
            // Personelin aldigi rezervasyonlar icin bos koleksiyon
            this.Bookings = new HashSet<Booking>();
        }

        // Primary Key (Birincil Anahtar) - Otomatik artan
        public int StaffID { get; set; }

        // Ad
        public string FirstName { get; set; }

        // Soyad
        public string LastName { get; set; }

        // E-posta - iletisim icin
        public string Email { get; set; }

        // Telefon numarasi
        public string Phone { get; set; }

        // Dogum tarihi - Nullable (Bos Olabilir) cunku zorunlu degil
        public Nullable<System.DateTime> DateOfBirth { get; set; }

        // Ise giris tarihi
        public Nullable<System.DateTime> HireDate { get; set; }

        // Maas - decimal kullaniyoruz para icin, hassasiyet onemli
        public Nullable<decimal> Salary { get; set; }

        // Foreign Key (Yabanci Anahtar) - Hangi departmanda calisiyor
        public Nullable<int> DepartmentID { get; set; }

        // Foreign Key (Yabanci Anahtar) - Hangi yetkiye sahip
        public string PerID { get; set; }

        // Kayit tarihleri
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        // Computed Property (Hesaplanmis Ozellik) - Ad ve soyadi birlestiriyoruz
        // Lambda expression (ifade) kullanarak tek satirda yaziyoruz
        public string FullName => $"{FirstName} {LastName}";

        // Navigation Properties (Gezinme Ozellikleri)
        // EF (Entity Framework) bunlari kullanarak JOIN islemleri yapiyor arka planda
        public virtual Department Department { get; set; }
        public virtual Permission Permission { get; set; }

        // Bu personelin aldigi rezervasyonlar
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
