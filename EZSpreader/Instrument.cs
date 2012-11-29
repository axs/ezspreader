namespace EZSpreader
{
    public class Instrument
    {
        private string exchange;
        private string product;
        private string prodType;
        private string contract;
        private string account;
        private double multiplier;
        private int size;

        public string Exchange { set { exchange = value; } get { return exchange; } }
        public string Product { set { product = value; } get { return product; } }
        public string ProductType { set { prodType = value; } get { return prodType; } }
        public double Multiplier { set { multiplier = value; } get { return multiplier; } }
        public string Contract { set { contract = value; } get { return contract; } }
        public string Account { set { account = value; } get { return account; } }
        public int Size { set { size = value; } get { return size; } }        
    }
}
