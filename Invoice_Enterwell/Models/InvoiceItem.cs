namespace Invoice.Models
{
    public class InvoiceItem : BaseEntity
    {
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPriceExcludingTax { get; set; }
        public decimal TotalPriceExcludingTax { get; set; }
        public int InvoiceId { get; set; }
        public BillInvoice Invoice { get; set; }
    }
}
