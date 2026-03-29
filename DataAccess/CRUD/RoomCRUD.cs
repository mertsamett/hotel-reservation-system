// =============================================================================
// ROOMCRUD.CS - Oda CRUD Islemleri
// Oda ekleme, guncelleme, silme, listeleme islemleri
// Include ile RoomType bilgisini de cekiyoruz (JOIN)
// IsAvailable durumu Trigger ile otomatik guncelleniyor veritabaninda
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace DataAccess.CRUD
{
    public class RoomCRUD
    {
        // DbContext instance'i
        private HotelManagementEntities db = new HotelManagementEntities();

        // =====================================================================
        // GET ALL - Tum odalari getir
        // Include ile RoomType'i da cekiyoruz - JOIN islemi
        // Oda numarasina gore siraliyoruz
        // =====================================================================
        public List<Room> GetAllRooms()
        {
            return db.Rooms
                .Include("RoomType")          // JOIN - Oda tipi bilgisi
                .OrderBy(r => r.RoomNumber)   // Oda numarasina gore sirala
                .ToList();
        }

        // =====================================================================
        // GET AVAILABLE - Sadece musait odalari getir
        // LINQ WHERE kullaniyoruz - IsAvailable = true olan odalar
        // Rezervasyon yaparken bu metodu kullaniyoruz
        // =====================================================================
        public List<Room> GetAvailableRooms()
        {
            return db.Rooms
                .Include("RoomType")
                .Where(r => r.IsAvailable == true)  // Sadece musait olanlar
                .OrderBy(r => r.RoomNumber)
                .ToList();
        }

        // =====================================================================
        // GET BY ID - ID ile oda bul
        // FirstOrDefault null donebilir
        // =====================================================================
        public Room GetRoomById(int id)
        {
            return db.Rooms
                .Include("RoomType")
                .FirstOrDefault(r => r.RoomID == id);
        }

        // =====================================================================
        // GET BY ROOM NUMBER - Oda numarasi ile bul
        // Oda numarasi unique (benzersiz) oldugu icin tek sonuc donuyor
        // =====================================================================
        public Room GetRoomByNumber(string roomNumber)
        {
            return db.Rooms
                .Include("RoomType")
                .FirstOrDefault(r => r.RoomNumber == roomNumber);
        }

        // =====================================================================
        // CREATE - Yeni oda ekle
        // Yeni oda varsayilan olarak musait (IsAvailable = true)
        // =====================================================================
        public void CreateRoom(Room room)
        {
            room.CreatedDate = DateTime.Now;
            room.ModifiedDate = DateTime.Now;
            room.IsAvailable = true;           // Yeni oda musait olarak baslar
            db.Rooms.Add(room);
            db.SaveChanges();
        }

        // =====================================================================
        // UPDATE - Oda guncelle
        // Once Find ile mevcut kaydi buluyoruz
        // Sonra alanlari tek tek guncelliyoruz
        // =====================================================================
        public void UpdateRoom(Room room)
        {
            var existing = db.Rooms.Find(room.RoomID);
            if (existing != null)
            {
                existing.RoomNumber = room.RoomNumber;
                existing.RoomTypeID = room.RoomTypeID;
                existing.Floor = room.Floor;
                existing.IsAvailable = room.IsAvailable;
                existing.Notes = room.Notes;
                existing.ModifiedDate = DateTime.Now;
                db.SaveChanges();
            }
        }

        // =====================================================================
        // UPDATE AVAILABILITY - Musaitlik durumunu guncelle
        // BookingCRUD'dan cagrilabilir ama genelde Trigger hallediyor
        // =====================================================================
        public void UpdateRoomAvailability(int roomId, bool isAvailable)
        {
            var room = db.Rooms.Find(roomId);
            if (room != null)
            {
                room.IsAvailable = isAvailable;
                room.ModifiedDate = DateTime.Now;
                db.SaveChanges();
            }
        }

        // =====================================================================
        // DELETE - Oda sil
        // Dikkat! Aktif rezervasyonu olan oda silinemiyor
        // Once kontrol ediyoruz, sonra siliyoruz
        // =====================================================================
        public bool DeleteRoom(int id)
        {
            var room = db.Rooms.Find(id);
            if (room != null)
            {
                // Aktif rezervasyon kontrolu - LINQ Any
                var hasActiveBookings = db.Bookings
                    .Any(b => b.RoomID == id && b.BookingStatus == "Active");

                if (hasActiveBookings)
                {
                    return false;  // Aktif rezervasyonu olan silinemez
                }

                db.Rooms.Remove(room);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        // =====================================================================
        // FILTER BY TYPE - Oda tipine gore filtrele
        // LINQ WHERE kullaniyoruz
        // =====================================================================
        public List<Room> GetRoomsByType(int roomTypeId)
        {
            return db.Rooms
                .Include("RoomType")
                .Where(r => r.RoomTypeID == roomTypeId)
                .OrderBy(r => r.RoomNumber)
                .ToList();
        }

        // =====================================================================
        // FILTER BY FLOOR - Kata gore filtrele
        // Katlari ayri ayri listelemek icin
        // =====================================================================
        public List<Room> GetRoomsByFloor(int floor)
        {
            return db.Rooms
                .Include("RoomType")
                .Where(r => r.Floor == floor)
                .OrderBy(r => r.RoomNumber)
                .ToList();
        }

        // =====================================================================
        // COUNT METHODS - Sayim metodlari
        // Dashboard istatistikleri icin kullaniyoruz
        // =====================================================================

        // Toplam oda sayisi
        public int GetTotalRoomCount()
        {
            return db.Rooms.Count();
        }

        // Musait oda sayisi - COUNT with WHERE
        public int GetAvailableRoomCount()
        {
            return db.Rooms.Count(r => r.IsAvailable == true);
        }

        // Dolu oda sayisi
        public int GetOccupiedRoomCount()
        {
            return db.Rooms.Count(r => r.IsAvailable == false);
        }
    }
}
