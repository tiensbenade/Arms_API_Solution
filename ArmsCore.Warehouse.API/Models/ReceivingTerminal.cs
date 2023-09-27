using System.ComponentModel.DataAnnotations;

namespace ArmsCore.Warehouse.API.Models
{
    public class ReceivingTerminal
    {
        [Key]
        public int Id { get; set; }
        public int ReceivingHeaderId { get; set; }
        public Int64 TerminalId { get; set; }
        public Int64 Sim1Id { get; set; }
        public Int64 Sim2Id { get; set; }
        public string BaseSerialNumberId { get; set; } = string.Empty;
        public string ProvisionalSerialNumber { get; set; } = string.Empty;
        public bool ValidationSuccess { get; set; }
        public bool HasCharger { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; } = string.Empty;

    }
}
