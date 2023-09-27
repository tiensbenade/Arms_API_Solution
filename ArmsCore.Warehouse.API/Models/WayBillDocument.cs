using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArmsCore.Warehouse.API.Models
{
    [Table("ReceivingWayBillDocuments", Schema = "dbo")]
    public class WayBillDocument
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; } = string.Empty;
    }
}
