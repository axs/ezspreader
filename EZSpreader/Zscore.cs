using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EZSpreader
{    

    class Zscore : Signal
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                
        private List<Quote> spreads = null;
        private List<double> zslist = null;

        private double[] zscores;
        private double[] multipliers;
        private int[] ratio;
        private string[] tickers;
        private double threshold;
        private int trendfollow;
        private int pricelength;
        private string signame;
        
        private Market market;

        private SpreadProc procSpread;
       
        
        public Zscore(Spread spread)
        {
            procSpread = SpreadProc.getInstance();

            this.tickers = new string[] { procSpread.productOne.Product
                                             ,procSpread.productTwo.Product };
            this.multipliers = new double[] {procSpread.productOne.InstrumentMultiplier
                                              ,procSpread.productTwo.InstrumentMultiplier };
            this.ratio = new int[] { procSpread.productOne.Size, procSpread.productTwo.Size };


            market  = Market.getInstance(tickers);
            this.spreads = new List<Quote>();
            this.zslist  = new List<double>();

            this.threshold = spread.Threshold;
            this.trendfollow  = spread.TrendFollow;
            this.pricelength  = spread.PriceLength;
            this.signame      = spread.Signal;

            this.signame = signame.ToUpper();
        }

        

        public override void resetparams(object[] o){ 
            this.threshold   = (double)o[0];
            this.trendfollow = (int)o[1];
            this.pricelength = (int)o[2];
            this.signame = (string)o[3];

            this.signame = signame.ToUpper();            
        }            
       

        public override void trader() {
            if( market.IsPopulated() )
            {
                CalcSpreadQuote();                                
                if (spreads.Count >= pricelength) CalcZScore();
            }
            else {
                if(log.IsDebugEnabled)
                    log.Debug("market object not populated yet");
            }
        }


        //worst case. buy offer and sell bid
        private void CalcSpreadQuote()
        {
            Quote spreadQuote = new Quote();  
            spreadQuote.bid = market.ask(tickers[0]) * multipliers[0] * ratio[0] 
                                    - market.bid(tickers[1]) * multipliers[1] * ratio[1];
            spreadQuote.ask = market.bid(tickers[0]) * multipliers[0] * ratio[0]
                                    - market.ask(tickers[1]) * multipliers[1] * ratio[1];
            spreadQuote.symbol = "spread";

            spreads.Add(spreadQuote);
           
            
            if (spreads.Count > pricelength)
            {
                spreads.RemoveAt(0);
            }
        }
        


        private void CalcZScore()
        {                     
            // calculate bid mean
            double bidmean = 0.0;
            double askmean = 0.0;
            for (int i = 0; i < spreads.Count; i++)
            {
                bidmean += spreads[i].bid;
                askmean += spreads[i].ask;
            }
            askmean /= spreads.Count;
            bidmean /= spreads.Count;
            

            // calculate deviation
            double bdev = 0.0;
            double bprc = 0.0;
            double adev = 0.0;
            double aprc = 0.0;

            for (int i = 0; i < spreads.Count; i++)
            {
                bprc = spreads[i].bid;
                bdev += (bprc - bidmean) * (bprc - bidmean);

                aprc = spreads[i].ask;
                adev += (aprc - askmean) * (aprc - askmean);
            }

            adev = Math.Sqrt(adev / (spreads.Count - 1));
            bdev = Math.Sqrt(bdev / (spreads.Count - 1));


            // z-score for the last price                       
             zscores = new double[] { (bprc - bidmean) / bdev
                                            ,(aprc - askmean) / adev };


            /*
             * XXX make this a factory or subclass this with execute() method
             * 
             * determine which signal to run
             */ 
             if ( signame == "CONVERGE")
             {                                  
                 zslist.Add(zscores[0]);
                 if (zslist.Count > pricelength)
                 {
                     zslist.RemoveAt(0);
                 }

                 convergence();
             }
             else {
                 overshoot();
             }
        }



        /* 
         * fire overshoot signal when zscore gets pass threshold
         */ 
        private void overshoot(){
            
           if (zscores[1] < -1*threshold )
           {               
               string action = trendfollow == 1 ? "Sell" : "Buy";
               if (log.IsDebugEnabled)
               {
                   log.Debug(
                       new System.Text.StringBuilder(action)
                                .AppendFormat(" zscore: {0} ~ {1} mkt: {2} ~ {3}",zscores[0],zscores[1],spreads[spreads.Count - 1].bid,spreads[spreads.Count - 1].ask)                       
                       );
               }
               
               OnTradeEvt(new TradeEventArgs(action));                
           }
           else if (zscores[0] > threshold )
           {                
               string action = trendfollow == 1 ? "Buy" : "Sell";

               if (log.IsDebugEnabled)
               {
                   log.Debug(
                       new System.Text.StringBuilder(action)
                                 .AppendFormat(" zscore: {0} ~ {1} mkt: {2} ~ {3}", zscores[0], zscores[1], spreads[spreads.Count - 1].bid, spreads[spreads.Count - 1].ask)
                        );
               }
               
               OnTradeEvt(new TradeEventArgs(action));                
           }                     
        }



        /*
         *  fire signal when the extremas and check to see if we are diverging/ converging
         */
        private void convergence(){                         
             Convergence zlistConv = LocalExtrema.releventWrap(zslist);
             Convergence mktConv   = LocalExtrema.releventWrap(spreads);


            /*
             if (zlistConv.IsHi)
             {
                 if (log.IsDebugEnabled) log.Debug("zlisdt HI");
            }
            if (zlistConv.IsLow)
            {
                if (log.IsDebugEnabled) log.Debug("zlisdt Low");
            }
            if (mktConv.IsHi)
            {
                if (log.IsDebugEnabled) log.Debug("mktConv HI");
            }
            if (mktConv.IsLow)
            {
                if (log.IsDebugEnabled) log.Debug("mktConv Low");
            }
            */

            if(zlistConv.IsHi && mktConv.IsLow){
                if (log.IsDebugEnabled)
                {
                    log.Debug(
                        new System.Text.StringBuilder("Buy convergence: mkt: ")
                                .AppendFormat("{0} ~ {1}", spreads[spreads.Count - 1].bid, spreads[spreads.Count - 1].ask)
                         );
                }             
                OnTradeEvt(new TradeEventArgs("Buy")); 
            }
            else if (zlistConv.IsLow && mktConv.IsHi)
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug(
                                new System.Text.StringBuilder("Sell convergence: mkt: ")
                                    .AppendFormat("{0} ~ {1}", spreads[spreads.Count - 1].bid, spreads[spreads.Count - 1].ask)                        
                         );
                }             
                OnTradeEvt(new TradeEventArgs("Sell"));
            }
                /*
            else if (zlistConv.IsLow && mktConv.IsLow)
            {
                action = "same low";
            }
            else if (zlistConv.IsHi && mktConv.IsHi)
            {
                action = "same hi";
            }
            */
                        
        }

        public double[] ZSCORES{
            get { return zscores; }
        }

    }
}
