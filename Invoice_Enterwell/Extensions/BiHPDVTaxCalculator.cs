using System.Composition;

namespace Invoice.Extensions
{
    [Export(typeof(ITaxCalculator))]
    public class BiHPDVTaxCalculator : ITaxCalculator
    {
        private const decimal TaxRate = 0.17m;

        public decimal CalculateTax(decimal amount)
        {
            return amount * TaxRate;
        }
    }
}
