// =============================================================================
// PERMISSION.CS - Yetki/Izin Sinifi
// Personelin hangi yetkilere sahip oldugunu tutuyoruz burada
// Her departmanin ve personelin bir yetkisi olmak zorunda, foreign key (yabanci anahtar) ile bagliyoruz
// =============================================================================

namespace DataAccess
{
    using System;
    using System.Collections.Generic;

    // Yetki sinifi - Admin, Manager, Staff gibi roller tanimlaniyor burda
    // Primary key (birincil anahtar) olarak PerID kullaniliyor, string tipinde
    public partial class Permission
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Permission()
        {
            // Constructor (yapilandirici) icinde bos koleksiyonlar olusturuyoruz
            // HashSet kullaniyoruz cunku ayni departman iki kere eklenmesin diye
            this.Departments = new HashSet<Department>();
            this.Staff = new HashSet<Staff>();
        }

        // Primary Key (Birincil Anahtar) - P01, P02 gibi degerler aliyor
        public string PerID { get; set; }

        // Rol adi - Admin, Manager, Receptionist falan
        public string PerRole { get; set; }

        // Yetki aciklamasi - Full Access, Limited Access gibi
        public string PerName { get; set; }

        // Navigation Property (Gezinme Ozelligi) - Bu yetkiye sahip departmanlar
        // One-to-Many (Bire-Cok) iliski var burada, bir yetki birden fazla departmana atanabilir
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Department> Departments { get; set; }

        // Bu yetkiye sahip personeller - yine One-to-Many (Bire-Cok)
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Staff> Staff { get; set; }
    }
}
