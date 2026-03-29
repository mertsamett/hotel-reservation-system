// =============================================================================
// DEPARTMENT.CS - Departman Sinifi
// Oteldeki departmanlari tutuyoruz - Resepsiyon, Housekeeping, Yonetim falan
// Her departmanin bir yetkisi var, Permission tablosuyla iliskilendiriyoruz
// =============================================================================

namespace DataAccess
{
    using System;
    using System.Collections.Generic;

    // Departman entity (varlik) sinifi
    // Veritabanindaki DEPARTMENT tablosuna karsilik geliyor bu
    public partial class Department
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Department()
        {
            // Bos bir HashSet olusturuyoruz personeller icin
            // Lazy Loading (Tembel Yukleme) sayesinde ihtiyac oldugunda yuklenecek
            this.Staff = new HashSet<Staff>();
        }

        // Primary Key (Birincil Anahtar) - Identity (Otomatik Artan) olarak tanimli veritabaninda
        public int DepartmentID { get; set; }

        // Departman adi - Reception, Housekeeping, Management gibi
        public string DepartmentName { get; set; }

        // Foreign Key (Yabanci Anahtar) - Permission tablosuna baglaniyor
        public string PerID { get; set; }

        // Calisan sayisi - Trigger (Tetikleyici) ile otomatik guncelleniyor bu alan
        // Insert yapilinca artiyor, delete yapilinca azaliyor
        public Nullable<int> NumberOfEmployees { get; set; }

        // Olusturulma tarihi
        public Nullable<System.DateTime> CreatedDate { get; set; }

        // Son guncelleme tarihi
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        // Navigation Property (Gezinme Ozelligi) - Departmanin yetkisi
        // virtual keyword (anahtar kelime) Lazy Loading icin gerekli
        public virtual Permission Permission { get; set; }

        // Bu departmandaki personeller - One-to-Many (Bire-Cok) iliski
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Staff> Staff { get; set; }
    }
}
