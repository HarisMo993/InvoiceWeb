using Invoice.Extensions;
using System;
using System.Collections.Generic;
using System.Composition;

namespace Invoice.Services
{
    public class TaxCalculatorService
    {
        [ImportMany]
        public IEnumerable<Lazy<ITaxCalculator>> TaxCalculators { get; set; }

        public ITaxCalculator GetTaxCalculator(string vrstaPoreza)
        {
            ITaxCalculator taxCalculator = null;

            switch (vrstaPoreza)
            {
                case "HRV":
                    taxCalculator = new CroatianPDVTaxCalculator();
                    break;

                case "BiH":
                    taxCalculator = new BiHPDVTaxCalculator();
                    break;
            }

            return taxCalculator;
        }
    }
}
