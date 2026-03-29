using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace DataAccess.CRUD
{
    public class RoomTypeCRUD
    {
        private HotelManagementEntities db = new HotelManagementEntities();

        // GET ALL
        public List<RoomType> GetAllRoomTypes()
        {
            return db.RoomTypes
                .OrderBy(rt => rt.TypeName)
                .ToList();
        }

        // GET BY ID
        public RoomType GetRoomTypeById(int id)
        {
            return db.RoomTypes.Find(id);
        }

        // CREATE
        public void CreateRoomType(RoomType roomType)
        {
            roomType.CreatedDate = DateTime.Now;
            roomType.ModifiedDate = DateTime.Now;
            db.RoomTypes.Add(roomType);
            db.SaveChanges();
        }

        // UPDATE
        public void UpdateRoomType(RoomType roomType)
        {
            var existing = db.RoomTypes.Find(roomType.RoomTypeID);
            if (existing != null)
            {
                existing.TypeName = roomType.TypeName;
                existing.Capacity = roomType.Capacity;
                existing.PricePerNight = roomType.PricePerNight;
                existing.Description = roomType.Description;
                existing.ModifiedDate = DateTime.Now;
                db.SaveChanges();
            }
        }

        // DELETE
        public bool DeleteRoomType(int id)
        {
            var roomType = db.RoomTypes.Find(id);
            if (roomType != null)
            {
                // Check if any rooms use this type
                var hasRooms = db.Rooms.Any(r => r.RoomTypeID == id);
                if (hasRooms)
                {
                    return false; // Cannot delete room type with existing rooms
                }

                db.RoomTypes.Remove(roomType);
                db.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
