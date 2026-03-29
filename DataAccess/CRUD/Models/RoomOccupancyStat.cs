namespace DataAccess.CRUD.Models
{
    /// <summary>
    /// Model for room occupancy statistics - used in dashboard charts
    /// </summary>
    public class RoomOccupancyStat
    {
        public string TypeName { get; set; }
        public int TotalCount { get; set; }
        public int AvailableCount { get; set; }
        public int OccupiedCount { get; set; }
    }
}
