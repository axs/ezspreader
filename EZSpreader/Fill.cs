using System;
using System.Collections.Generic;
using System.Text;

namespace EZSpreader
{
    public class Fill
    {
        private string key;
        private bool full;
        private bool buy;
        private int qty;
        private int netqty;
        private double price;
        private string account;
        private string product;

        public string Product { set { product = value; } get { return product; } }
        public string Key { set { key = value; } get { return key; } }
        public bool Full { set { full = value; } get { return full; } }
        public bool Buy { set { buy = value; } get { return buy; } }
        public int Qty { set { qty = value; } get { return qty; } }
        public int NetQty { set { netqty = value; } get { return netqty; } }
        public double Price { set { price = value; } get { return price; } }

        public string Account { set { account = value; } get { return account; } }


        public override string ToString()
        {
            string x = "Key=" + key + " Full=" + full + " Product= " + product 
                     + " Buy=" + buy + " Qty=" + qty
                     + " NetQty=" + netqty + " Price=" + price + " Acct=" + account;

            return x;
        }
    }
}
