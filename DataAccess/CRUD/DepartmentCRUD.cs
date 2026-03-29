using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace DataAccess.CRUD
{
    public class DepartmentCRUD
    {
        private HotelManagementEntities db = new HotelManagementEntities();

        // GET ALL
        public List<Department> GetAllDepartments()
        {
            return db.Departments
                .Include("Permission")
                .OrderBy(d => d.DepartmentName)
                .ToList();
        }

        // GET BY ID
        public Department GetDepartmentById(int id)
        {
            return db.Departments
                .Include("Permission")
                .Include("Staff")
                .FirstOrDefault(d => d.DepartmentID == id);
        }

        // CREATE
        public void CreateDepartment(Department department)
        {
            department.CreatedDate = DateTime.Now;
            department.ModifiedDate = DateTime.Now;
            department.NumberOfEmployees = 0;
            db.Departments.Add(department);
            db.SaveChanges();
        }

        // UPDATE
        public void UpdateDepartment(Department department)
        {
            var existing = db.Departments.Find(department.DepartmentID);
            if (existing != null)
            {
                existing.DepartmentName = department.DepartmentName;
                existing.PerID = department.PerID;
                existing.ModifiedDate = DateTime.Now;
                db.SaveChanges();
            }
        }

        // DELETE
        public bool DeleteDepartment(int id)
        {
            var department = db.Departments.Find(id);
            if (department != null)
            {
                // Check if department has staff
                var hasStaff = db.Staffs.Any(s => s.DepartmentID == id);
                if (hasStaff)
                {
                    return false; // Cannot delete department with staff
                }

                db.Departments.Remove(department);
                db.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
