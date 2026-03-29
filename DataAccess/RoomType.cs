// =============================================================================
// ROOMTYPE.CS - Oda Tipi Sinifi
// CONCEPT HIERARCHY (Kavram Hiyerarsisi) ornegi bu sinif
// Single < Double < Suite < Penthouse seklinde bir hiyerarsi var
// Her tipin farkli kapasitesi ve fiyati var, kategorilendirme yapiyoruz yani
// =============================================================================

namespace DataAccess
{
    using System;
    using System.Collections.Generic;

    // Oda tipi entity (varlik) sinifi
    // Kavram hiyerarsisi icin guzel bir ornek, farkli seviyelerde oda tipleri tanimliyoruz
    // Single Room -> Double Room -> Suite -> Penthouse (ucuzdan pahaliya dogru)
    public partial class RoomType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RoomType()
        {
            // Bu tipteki odalar - One-to-Many (Bire-Cok) iliski
            this.Rooms = new HashSet<Room>();
        }

        // Primary Key (Birincil Anahtar)
        public int RoomTypeID { get; set; }

        // Tip adi - Single, Double, Suite, Penthouse falan
        public string TypeName { get; set; }

        // Kapasite - kac kisi kalabilir odada
        // Concept Hierarchy (Kavram Hiyerarsisi) icin onemli, her seviyede farkli
        public int Capacity { get; set; }

        // Gecelik fiyat - decimal kullaniyoruz para birimi icin
        // Hiyerarsideki her seviye icin farkli fiyat var tabii ki
        public decimal PricePerNight { get; set; }

        // Oda tipi aciklamasi
        public string Description { get; set; }

        // Tarih alanlari
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        // Navigation Property (Gezinme Ozelligi) - Bu tipteki tum odalar
        // Include (Dahil Etme) ile bu koleksiyonu doldurabiliriz sorgularda
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
