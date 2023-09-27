using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArmsCore.Warehouse.API.Models
{
    [Table("ReceivingHeader", Schema = "dbo")]
    public class ReceivingHeader
    {
        [Key]
        public int Id { get; set; }
        public string InternalReference { get; set; } = string.Empty;
        public string ExternalReference { get; set; } = string.Empty;
        public int BatchQuantity { get; set; }  
        public int WayBillId { get; set; }
        public int CourierId { get; set; }
        public bool IsNewDevices { get; set; }
        public bool BatchCompleted { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; } = string.Empty;

      
    }
}
