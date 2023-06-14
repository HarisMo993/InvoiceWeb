using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Invoice.ViewModels
{
    public class BillInvoiceViewModel
    {
        public string InvoiceNumber { get; set; }

        [Display(Name = "Datum dospijeća")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Display(Name = "Naziv primatelja")]
        public string RecipientName { get; set; }
        public string TaxType { get; set; }
        public decimal TotalAmountExcludingTax { get; set; }
        public decimal TotalAmountIncludingTax { get; set; }
        public string CreatorUserName { get; set; }
        public int? NumberOfItems { get; set; }
        public bool? CanDelete { get; set; }

        public List<InvoiceItemViewModel> InvoiceItems { get; set; }
    }
}
