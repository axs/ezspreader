using System;
using System.Collections.Generic;
using System.Text;

namespace EZSpreader
{

    class Quote
    {
        public double bid = 0;
        public double ask = 0;
        public string symbol = null;
    }


    class Market
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // This is a singleton
        private static Market instance = null;
        private Dictionary<string, Quote> mktdict = null;
        private string[] tickers;


        private Market(string[] tickers)
        {
            this.tickers = tickers;
            this.mktdict = new Dictionary<string, Quote>();
        }


        public static Market getInstance()
        {
            return (instance);
        }


        public static Market getInstance(string[] tickers )
        {
            if (instance == null)
            {
                instance = new Market(tickers);
            }

            return (instance);
        }


        public bool IsPopulated(){
            bool status = false;

            if (mktdict.ContainsKey(tickers[0]) && mktdict.ContainsKey(tickers[1]))
            {
                if (mktdict[tickers[0]].ask > 0 && mktdict[tickers[0]].bid > 0 &&
                    mktdict[tickers[1]].ask > 0 && mktdict[tickers[1]].bid > 0)
                {
                        status = true;
                }
            }

            return status;
        }


        public void trade(string sym, double bid, double ask)
        {
            if (!mktdict.ContainsKey(sym))
            {
                Quote quote = new Quote();
                quote.bid = bid;
                quote.ask = ask;
                mktdict[sym] = quote;
            }
            else
            {
                mktdict[sym].bid = bid;
                mktdict[sym].ask = ask;
            }
        }


        public double bid(string sym) {
            double price=0;

            try
            {
                price = mktdict[sym].bid;
            }
            catch (Exception ex){
                if (log.IsErrorEnabled) log.Error(ex.StackTrace);                
            }

            return price;
        }

        public double ask(string sym)
        {
            double price=0;

            try
            {
                price = mktdict[sym].ask;
            }
            catch (Exception ex)
            {
                if(log.IsErrorEnabled) log.Error(ex.StackTrace);                
            }

            return price;
        }
    }
}
