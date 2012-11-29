using System;
using System.Collections.Generic;
using System.Text;

namespace EZSpreader
{
    public class FoundEventArgs : EventArgs
    {
        private double tickPrice;

        public FoundEventArgs(double tickPrice)
        {
            this.tickPrice = tickPrice;
        }

        public double TICKPRICE {
            get { return tickPrice;  }
        }
    }
}
