using System;
using System.Collections.Generic;
using System.Text;

namespace EZSpreader
{
    public class ZscoreEventArgs : EventArgs
    {

        public double[] zscores;

        public ZscoreEventArgs(double[] z)
        {
            this.zscores = z;
        }
    }
}
