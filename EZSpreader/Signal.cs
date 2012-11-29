using System;
using System.Collections.Generic;
using System.Text;

namespace EZSpreader
{
    public abstract class Signal
    {
        public abstract void trader();        
        public event EventHandler<TradeEventArgs> TradeEvt;


        
        //The event-invoking method that derived classes can override.
        protected virtual void OnTradeEvt(TradeEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<TradeEventArgs> handler = TradeEvt;
            if (handler != null)
            {
                handler(this, e);
            }
        }


        public virtual void resetparams(object o){}
        public virtual void resetparams(object[] o) { }
    }
}
