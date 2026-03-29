// =============================================================================
// BOOKING.CS - Rezervasyon Sinifi
// TERNARY RELATIONSHIP (Uclu Iliski) ornegi bu sinif
// Musteri + Oda + Personel = Rezervasyon seklinde uc tabloyu birlestiriyoruz
// Bir musteri bir oda icin rezervasyon yapiyor, bir personel bu rezervasyonu aliyor
// =============================================================================

namespace DataAccess
{
    using System;
    using System.Collections.Generic;

    // Rezervasyon entity (varlik) sinifi
    // TERNARY RELATIONSHIP (Uclu Iliski) - Uc farkli tabloyu bagliyor bu entity
    // Customer (Musteri) + Room (Oda) + Staff (Personel) bir araya geliyor
    public partial class Booking
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Booking()
        {
            // Rezervasyona ait oda servisleri - Weak Entity (Zayif Varlik)
            this.RoomServices = new HashSet<RoomService>();
        }

        // Primary Key (Birincil Anahtar)
        public int BookingID { get; set; }

        // Foreign Key (Yabanci Anahtar) - Hangi musteri
        // TERNARY (Uclu) iliskinin birinci bacagi
        public int CustomerID { get; set; }

        // Foreign Key (Yabanci Anahtar) - Hangi oda
        // TERNARY (Uclu) iliskinin ikinci bacagi
        public int RoomID { get; set; }

        // Foreign Key (Yabanci Anahtar) - Hangi personel aldi bu rezervasyonu
        // TERNARY (Uclu) iliskinin ucuncu bacagi
        // Nullable cunku online rezervasyonda personel olmayabilir
        public Nullable<int> StaffID { get; set; }

        // Giris tarihi
        public System.DateTime CheckInDate { get; set; }

        // Cikis tarihi - CheckInDate'den buyuk olmak zorunda (Constraint ile kontrol ediliyor)
        public System.DateTime CheckOutDate { get; set; }

        // Rezervasyon yapilma tarihi
        public Nullable<System.DateTime> BookingDate { get; set; }

        // Toplam ucret - gece sayisi * gecelik fiyat
        public Nullable<decimal> TotalPrice { get; set; }

        // Rezervasyon durumu - Active, Completed, Cancelled
        // CHECK constraint (kisitlama) ile sadece bu degerler kabul ediliyor
        public string BookingStatus { get; set; }

        // Notlar
        public string Notes { get; set; }

        // Tarih alanlari
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        // Computed Property (Hesaplanmis Ozellik) - Kac gece kaliyor
        // Cikis - Giris = Gece sayisi, basit matematik
        public int NightCount => (CheckOutDate - CheckInDate).Days;

        // Navigation Properties (Gezinme Ozellikleri)
        // TERNARY RELATIONSHIP icin uc tablo da burada
        // Include("Customer").Include("Room").Include("Staff") diyerek hepsini cekebiliriz
        public virtual Customer Customer { get; set; }
        public virtual Room Room { get; set; }
        public virtual Staff Staff { get; set; }

        // Weak Entity (Zayif Varlik) - Oda servisleri
        // ON DELETE CASCADE ile rezervasyon silinince servisler de siliniyor
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoomService> RoomServices { get; set; }
    }
}
