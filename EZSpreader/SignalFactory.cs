using System;
using System.Collections.Generic;
using System.Text;

namespace EZSpreader
{
    public class SignalFactory
    {
        Spread spread;
        string signalName;

        public SignalFactory(Spread spread) {
            this.spread = spread;
            this.signalName = spread.Signal.ToUpper();
        }


        public Signal create() {
            if (this.signalName == "ZSCORE") 
            {
                return new Zscore(this.spread);
            }
            else if (this.signalName == "CONVERGE")
            {
                return new Zscore(this.spread);
            }
            else 
            {
                return new Zscore(this.spread);
            }
        }
    }
}
