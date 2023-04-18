namespace States.Models
{
    [GenerateSerializer]
    public class CustomerState
    {
        [Id(0)]
        public int Id { get; set; }

        [Id(1)]
        public string Name { get; set; }

        [Id(2)]
        public string Description { get; set; }

        [Id(3)]
        public string Address { get; set; }

        [Id(4)]
        public string City { get; set; }

        [Id(5)]
        public string Region { get; set; }

        [Id(6)]
        public string PostalCode { get; set; }

        [Id(7)]
        public string Country { get; set; }
        
        [Id(8)]
        public Dictionary<string, StockState> Stocks { get; set; }
    }
}
