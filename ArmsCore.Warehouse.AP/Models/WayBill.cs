using System.ComponentModel.DataAnnotations;

namespace ARMS_Warehouse_API.Models
{
    public class WayBill
    {
        [Key]
        public int Id { get; set; }
        public string WayBillNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; } = string.Empty;
    }
}

