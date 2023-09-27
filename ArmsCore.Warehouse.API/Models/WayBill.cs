using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArmsCore.Warehouse.API.Models
{
    [Table("ReceivingWayBill", Schema = "dbo")]
    public class WayBill
    {
        [Key]
        public int Id { get; set; }
        public string WayBillNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; } = string.Empty;
    }
}

