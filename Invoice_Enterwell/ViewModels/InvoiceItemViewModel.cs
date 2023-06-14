using System.ComponentModel.DataAnnotations;

namespace Invoice.ViewModels
{
    public class InvoiceItemViewModel
    {
        public string Description { get; set; }

        public decimal Quantity { get; set; }

        [Display(Name = "Jedinična cijena bez poreza")]
        public decimal UnitPriceExcludingTax { get; set; }
    }
}
