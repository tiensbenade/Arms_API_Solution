using System.ComponentModel.DataAnnotations;

namespace ARMS_Warehouse_API.Models
{
    public class ReceivingTerminals
    {
        [Key]
        public int Id { get; set; }
        public int TerminalId { get; set; }
        public int SimId1 { get; set; }
        public int SimId2 { get; set; }
        public string BaseSerialNumberId { get; set; } = string.Empty;
        public string ProvisionalSerialNumber { get; set; } = string.Empty;
        public bool ValidationSuccess { get; set; }
        public bool HasCharger { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; } = string.Empty;

    }
}
