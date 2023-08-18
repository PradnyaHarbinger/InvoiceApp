using System.ComponentModel;

namespace InvoiceApp.Models
{
    public class Invoice
    {
        [DisplayName("Id")]
        public int InvoiceId { get; set; }
        [DisplayName("Invoice Amount")]
        public double InvoiceAmount { get; set; }
        [DisplayName("Invoice Month")]
        public string InvoiceMonth { get; set; }
        [DisplayName("Invoice Owner")]
        public string InvoiceOwner { get; set; }
        [DisplayName("Creator Id")]
        public string CreatorId { get; set; }
        [DisplayName("Status")]
        public InvoiceStatus Status { get; set; }
    }

    public enum InvoiceStatus
    {
        Submitted,
        Approved,
        Rejected
    }
}
