using System;
using System.Collections.Generic;
using System.Text;

namespace EZSpreader
{
    public class Update
    {
        private double bid;
        private double ask;
        private int netpos;
        private double pl;
        private double openpl;
        private string symbol;

        public double Bid { set { bid = value; } get { return bid; } }
        public double Ask { set { ask = value; } get { return ask; } }
        public int NetPos { set { netpos = value; } get { return netpos; } }
        public double PL { set { pl = value; } get { return pl; } }
        public double OpenPL { set { openpl = value; } get { return openpl; } }
        public string Symbol { set { symbol = value; } get { return symbol; } }
    }
}
