// =============================================================================
// ROOMSERVICE.CS - Oda Servisi Sinifi
// WEAK ENTITY (Zayif Varlik) ornegi bu sinif
// Booking olmadan var olamaz, rezervasyona bagimli bir entity
// ON DELETE CASCADE - Rezervasyon silinince servisler de otomatik siliniyor
// =============================================================================

namespace DataAccess
{
    using System;
    using System.Collections.Generic;

    // Oda servisi entity (varlik) sinifi
    // WEAK ENTITY (Zayif Varlik) - Guclu entity (Booking) olmadan var olamaz
    // Oda servisi dedigimiz sey: kahvalti, minibar, oda temizligi gibi ekstra hizmetler
    public partial class RoomService
    {
        // Primary Key (Birincil Anahtar) - ama Weak Entity oldugu icin
        // tek basina anlam ifade etmiyor, BookingID ile birlikte dusunmeli
        public int ServiceID { get; set; }

        // Foreign Key (Yabanci Anahtar) - Hangi rezervasyona ait
        // WEAK ENTITY oldugu icin bu alan zorunlu (NOT NULL)
        // ON DELETE CASCADE - Parent (ana kayit) silinince bu da siliniyor
        public int BookingID { get; set; }

        // Servis tipi - Breakfast, Minibar, Laundry, Room Cleaning falan
        public string ServiceType { get; set; }

        // Servis aciklamasi
        public string Description { get; set; }

        // Servis ucreti - decimal cunku para
        public decimal Price { get; set; }

        // Servisin verildigi tarih
        public Nullable<System.DateTime> ServiceDate { get; set; }

        // Olusturulma tarihi
        public Nullable<System.DateTime> CreatedDate { get; set; }

        // Navigation Property (Gezinme Ozelligi) - Ana rezervasyon
        // Weak Entity (Zayif Varlik) her zaman Parent'a (ebeveyn) referans verir
        public virtual Booking Booking { get; set; }
    }
}
