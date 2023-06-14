using System.Composition;

namespace Invoice.Extensions
{
    [Export(typeof(ITaxCalculator))]
    public class CroatianPDVTaxCalculator : ITaxCalculator
    {
        private const decimal TaxRate = 0.25m;

        public decimal CalculateTax(decimal amount)
        {
            return amount * TaxRate;
        }
    }
}
