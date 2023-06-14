namespace Invoice.Extensions
{
    public interface ITaxCalculator
    {
        decimal CalculateTax(decimal amount);
    }
}
