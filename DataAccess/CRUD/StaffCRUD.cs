using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace DataAccess.CRUD
{
    public class StaffCRUD
    {
        private HotelManagementEntities db = new HotelManagementEntities();

        // GET ALL - with JOINs
        public List<Staff> GetAllStaff()
        {
            return db.Staffs
                .Include("Department")
                .Include("Permission")
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToList();
        }

        // GET BY ID
        public Staff GetStaffById(int id)
        {
            return db.Staffs
                .Include("Department")
                .Include("Permission")
                .FirstOrDefault(s => s.StaffID == id);
        }

        // GET BY DEPARTMENT
        public List<Staff> GetStaffByDepartment(int departmentId)
        {
            return db.Staffs
                .Include("Department")
                .Where(s => s.DepartmentID == departmentId)
                .OrderBy(s => s.LastName)
                .ToList();
        }

        // CREATE (Trigger increments department employee count)
        public void CreateStaff(Staff staff)
        {
            staff.CreatedDate = DateTime.Now;
            staff.ModifiedDate = DateTime.Now;
            staff.HireDate = DateTime.Now;
            db.Staffs.Add(staff);
            db.SaveChanges();
            // Note: SQL Trigger will increment Department.NumberOfEmployees
        }

        // UPDATE
        public void UpdateStaff(Staff staff)
        {
            var existing = db.Staffs.Find(staff.StaffID);
            if (existing != null)
            {
                existing.FirstName = staff.FirstName;
                existing.LastName = staff.LastName;
                existing.Email = staff.Email;
                existing.Phone = staff.Phone;
                existing.DateOfBirth = staff.DateOfBirth;
                existing.Salary = staff.Salary;
                existing.DepartmentID = staff.DepartmentID;
                existing.PerID = staff.PerID;
                existing.ModifiedDate = DateTime.Now;
                db.SaveChanges();
            }
        }

        // DELETE (Trigger decrements department employee count)
        public bool DeleteStaff(int id)
        {
            var staff = db.Staffs.Find(id);
            if (staff != null)
            {
                db.Staffs.Remove(staff);
                db.SaveChanges();
                // Note: SQL Trigger will decrement Department.NumberOfEmployees
                return true;
            }
            return false;
        }

        // SEARCH
        public List<Staff> SearchStaff(string searchTerm)
        {
            searchTerm = searchTerm.ToLower();
            return db.Staffs
                .Include("Department")
                .Where(s => s.FirstName.ToLower().Contains(searchTerm) ||
                           s.LastName.ToLower().Contains(searchTerm) ||
                           s.Email.ToLower().Contains(searchTerm))
                .ToList();
        }

        // GET COUNT
        public int GetTotalStaffCount()
        {
            return db.Staffs.Count();
        }
    }
}
