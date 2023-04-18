namespace States.Models
{
    [GenerateSerializer]
    public class StockState
    {
        [Id(0)]
        public string Symbol { get; set; }
        [Id(1)]
        public decimal Price { get; set; }
    }
}
