using System;
using System.Collections.Generic;
using System.Text;

namespace EZSpreader
{
    public class Spread
    {
        private int payupticks;
        private int payupperiod;
        private int payupcount;
        private double threshold;
        private int multipliersize;
        private int trendfollow;
        private int pricelength;
        private string signal;

        public int PayUpTicks { set { payupticks = value; } get { return payupticks; } }
        public int PayUpPeriod { set { payupperiod = value; } get { return payupperiod; } }
        public int PayUpCount { set { payupcount = value; } get { return payupcount; } }        
        public double Threshold { set { threshold = value; } get { return threshold; } }
        public int MultiplierSize { set { multipliersize = value; } get { return multipliersize; } }
        public int TrendFollow { set { trendfollow = value; } get { return trendfollow; } }
        public int PriceLength { set { pricelength = value; } get { return pricelength; } }
        public string Signal { set { signal = value; } get { return signal; } }        

    }
}
