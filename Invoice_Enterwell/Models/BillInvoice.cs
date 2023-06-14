using System;
using System.Collections.Generic;

namespace Invoice.Models
{
    public class BillInvoice : BaseEntity
    {
        public string InvoiceNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        public ICollection<InvoiceItem> InvoiceItems { get; set; }
        public decimal TotalAmountExcludingTax { get; set; }
        public decimal TotalAmountIncludingTax { get; set; }
        public string CreatorUserId { get; set; }
        public ApplicationUser CreatorUser { get; set; }
        public string RecipientName { get; set; }
    }
}
