using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace DataAccess.CRUD
{
    /// <summary>
    /// WEAK ENTITY CRUD - RoomService depends on Booking
    /// ON DELETE CASCADE: When booking is deleted, all services are deleted
    /// </summary>
    public class RoomServiceCRUD
    {
        private HotelManagementEntities db = new HotelManagementEntities();

        // GET ALL
        public List<RoomService> GetAllRoomServices()
        {
            return db.RoomServices
                .Include("Booking")
                .Include("Booking.Room")
                .Include("Booking.Customer")
                .ToList();
        }

        // GET BY ID
        public RoomService GetRoomServiceById(int id)
        {
            return db.RoomServices
                .Include("Booking")
                .FirstOrDefault(rs => rs.ServiceID == id);
        }

        // GET BY BOOKING - Get all services for a specific booking
        public List<RoomService> GetServicesByBooking(int bookingId)
        {
            return db.RoomServices
                .Where(rs => rs.BookingID == bookingId)
                .OrderByDescending(rs => rs.ServiceDate)
                .ToList();
        }

        // CREATE
        public void CreateRoomService(RoomService entity)
        {
            entity.ServiceDate = DateTime.Now;
            entity.CreatedDate = DateTime.Now;
            db.RoomServices.Add(entity);
            db.SaveChanges();
        }

        // UPDATE
        public void UpdateRoomService(RoomService entity)
        {
            var existing = db.RoomServices.Find(entity.ServiceID);
            if (existing != null)
            {
                existing.ServiceType = entity.ServiceType;
                existing.Description = entity.Description;
                existing.Price = entity.Price;
                db.SaveChanges();
            }
        }

        // DELETE
        public bool DeleteRoomService(int id)
        {
            var entity = db.RoomServices.Find(id);
            if (entity != null)
            {
                db.RoomServices.Remove(entity);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        // Analytics - SUM of services for a booking
        public decimal GetTotalServicesPrice(int bookingId)
        {
            return db.RoomServices
                .Where(rs => rs.BookingID == bookingId)
                .Sum(rs => rs.Price);
        }
    }
}
