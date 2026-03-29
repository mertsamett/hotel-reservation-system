namespace DataAccess.CRUD.Models
{
    /// <summary>
    /// Model for customer distribution by country - used in dashboard charts
    /// </summary>
    public class CustomerCountryStat
    {
        public string Country { get; set; }
        public int CustomerCount { get; set; }
    }
}
