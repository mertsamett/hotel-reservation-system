// =============================================================================
// ROOM.CS - Oda Sinifi
// Oteldeki odalarin bilgilerini tutuyoruz - oda numarasi, kat, musaitlik durumu
// RoomType ile iliskili, hangi tipte bir oda oldugunu oradan aliyoruz
// IsAvailable alani Trigger (Tetikleyici) ile otomatik guncelleniyor
// =============================================================================

namespace DataAccess
{
    using System;
    using System.Collections.Generic;

    // Oda entity (varlik) sinifi
    // ROOM tablosunun C# tarafindaki karsiligi
    public partial class Room
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Room()
        {
            // Bu odaya ait rezervasyonlar
            this.Bookings = new HashSet<Booking>();
        }

        // Primary Key (Birincil Anahtar)
        public int RoomID { get; set; }

        // Oda numarasi - 101, 102, 201 gibi
        // Unique (Benzersiz) olmak zorunda, ayni numara iki kere olamaz
        public string RoomNumber { get; set; }

        // Foreign Key (Yabanci Anahtar) - Hangi tipte oda bu
        // RoomType tablosuna referans veriyor
        public int RoomTypeID { get; set; }

        // Kac. kat - nullable (bos olabilir) cunku bazi otellerde kat kavrami olmayabilir
        public Nullable<int> Floor { get; set; }

        // Musaitlik durumu - true ise bos, false ise dolu
        // Trigger (Tetikleyici) ile guncelleniyor bu alan:
        // - Rezervasyon yapilinca false oluyor
        // - Check-out yapilinca veya iptal edilince true oluyor
        public Nullable<bool> IsAvailable { get; set; }

        // Notlar - ozel durumlar icin
        public string Notes { get; set; }

        // Tarih alanlari
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        // Navigation Property (Gezinme Ozelligi) - Odanin tipi
        // Include("RoomType") diyerek sorguda cekebiliriz
        public virtual RoomType RoomType { get; set; }

        // Bu odaya yapilan rezervasyonlar
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
