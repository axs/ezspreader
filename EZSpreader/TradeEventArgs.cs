using System;
using System.Collections.Generic;
using System.Text;

namespace EZSpreader
{
    public class TradeEventArgs : EventArgs
    {

          private string action;

        public TradeEventArgs(string Action)
        {
            this.action = Action;
        }

        public string ACTION {
            get { return action; }
        }
    }
}
