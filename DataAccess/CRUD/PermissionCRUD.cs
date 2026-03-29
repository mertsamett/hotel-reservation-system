using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace DataAccess.CRUD
{
    public class PermissionCRUD
    {
        private HotelManagementEntities db = new HotelManagementEntities();

        // GET ALL
        public List<Permission> GetAllPermissions()
        {
            return db.Permissions
                .OrderBy(p => p.PerRole)
                .ThenBy(p => p.PerName)
                .ToList();
        }

        // GET BY ID
        public Permission GetPermissionById(string id)
        {
            return db.Permissions.Find(id);
        }

        // CREATE
        public void CreatePermission(Permission permission)
        {
            db.Permissions.Add(permission);
            db.SaveChanges();
        }

        // UPDATE
        public void UpdatePermission(Permission permission)
        {
            var existing = db.Permissions.Find(permission.PerID);
            if (existing != null)
            {
                existing.PerRole = permission.PerRole;
                existing.PerName = permission.PerName;
                db.SaveChanges();
            }
        }

        // DELETE
        public bool DeletePermission(string id)
        {
            var permission = db.Permissions.Find(id);
            if (permission != null)
            {
                db.Permissions.Remove(permission);
                db.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
