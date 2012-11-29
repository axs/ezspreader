using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;


namespace EZSpreader
{

    class StrategyProc
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // This is a singleton
        private static StrategyProc instance = null;

        public  Spread     spread;
        private Signal     signal;
        private SpreadProc procSpread;
        private Market     market;

        private static int currentPosOne =0;
        private static int currentPosTwo =0;

        private Queue<Order> scratchLegs;                //list of Orders that are filled but the remaing leg has exhausted payUpCount
        private List<SpreadOrder> orders;                //list of working spreadOrders
        private Dictionary<string,SpreadOrder> fillbugs; //used for the full fill bug
        private Dictionary<string,double> lastfill;      //BuySell => price
        private Dictionary<string, Thread> closeouts;    //SOK=>thread
        private Dictionary<string, Order> offOrders;     // used when we try to cancel an off mkt spread.
                                                         // XXX this could grow with no bounds because we only
                                                         // remove them if they were too late to cancel in onFill()

        private static int spreadIDcount = 0;

        private StrategyProc(Spread spread)
        {
            this.spread = spread;

            orders     = new List<SpreadOrder>();
            lastfill   = new Dictionary<string, double>();
            closeouts  = new Dictionary<string, Thread>();
            offOrders  = new Dictionary<string, Order>();
            fillbugs   = new Dictionary<string, SpreadOrder>();

            scratchLegs = new Queue<Order>();

            procSpread = SpreadProc.getInstance();

            procSpread.OnFill   += this.OnFill;
            procSpread.OnUpdate += this.OnUpdate;

        }



        public static StrategyProc getInstance()
        {
            return (instance);
        }


        public static StrategyProc getInstance(Spread spread)
        {
            if (instance == null)
            {
                instance = new StrategyProc(spread);
            }

            return (instance);
        }


        private void OnTrade(object sender, TradeEventArgs e)
        {
            if (procSpread.Started)
            {
                for (int i = 0; i<spread.MultiplierSize;i++ )
                {
                    SendSpreadOrder(e.ACTION, true);
                }

            }
        }


        private void OnUpdate(object sender, UpdateEventArgs e)
        {
            try
            {
                market.trade(e.update.Symbol, e.update.Bid, e.update.Ask);

                attemptScratchLeg();

                //--------------XXX  make this better---
                //--------------------------------------
                List<SpreadOrder> delorders = new List<SpreadOrder>();
                            
               

                //check to see if openorders are still attractive
                foreach( SpreadOrder so in orders ){

                    double[] wkprices = so.workPrices();                    

                    if( so.buySell == "Buy" ){
                        if (wkprices[0] != market.bid(procSpread.productOne.Product) &&
                            wkprices[1] != market.ask(procSpread.productTwo.Product) &&
                            so.fillPrice() == 0
                            )
                        {
                            if (log.IsDebugEnabled) log.Debug("buy off market");
                            string[] sok = so.getSOKs();
                            Order[] legs = so.getLegs();

                            int delone = procSpread.Delete(sok[0], procSpread.productOne.ttOrderSet);
                            int deltwo = procSpread.Delete(sok[1], procSpread.productTwo.ttOrderSet);

                            int filledOne = procSpread.productOne.Size - delone;
                            int filledTwo = procSpread.productTwo.Size - deltwo;


                            /*
                             * if we couldnt get the cancel off we need to close it out but ignore
                             * if both legs are complete fills
                             */
                            if (delone != 0 || deltwo != 0)
                            {
                                if (filledOne != 0)
                                {
                                    if (log.IsDebugEnabled) log.Debug("cancel didnt go off completely. selling " + filledOne);
                                    procSpread.SendOrder("Sell", market.bid(procSpread.productOne.Product), filledOne, procSpread.productOne);
                                }
                                else
                                {
                                    offOrders[sok[0]] = legs[0];
                                }
                                if (filledTwo != 0)
                                {
                                    if (log.IsDebugEnabled) log.Debug("cancel didnt go off completely. buying " + filledTwo);
                                    procSpread.SendOrder("Buy", market.ask(procSpread.productTwo.Product), filledTwo, procSpread.productTwo);
                                }
                                else
                                {
                                    offOrders[sok[1]] = legs[1];
                                }
                            }

                            delorders.Add(so);
                            break;
                        }
                        //XXX shift the spread if necassary. 
                   //     else if (so.fillPrice() == 0)
                   //     {
                   //         checkSpreadLevel(so);
                   //     }

                    }
                    else if( so.buySell == "Sell" )
                    {
                        if( wkprices[0] != market.ask(procSpread.productOne.Product) &&
                            wkprices[1] != market.bid(procSpread.productTwo.Product) &&
                            so.fillPrice() == 0
                            )
                        {
                            if (log.IsDebugEnabled) log.Debug("sell off market");
                            string[] sok = so.getSOKs();
                            Order[] legs = so.getLegs();

                            int delone = procSpread.Delete(sok[0], procSpread.productOne.ttOrderSet);
                            int deltwo = procSpread.Delete(sok[1], procSpread.productTwo.ttOrderSet);

                            int filledOne = procSpread.productOne.Size - delone;
                            int filledTwo = procSpread.productTwo.Size - deltwo;


                            //if we couldnt get the cancel off we need to close it out
                            if (delone != 0 || deltwo != 0)
                            {

                                if (filledOne != 0)
                                {
                                    if (log.IsDebugEnabled) log.Debug("cancel didnt go off completely. buying " + filledOne);
                                    procSpread.SendOrder("Buy", market.ask(procSpread.productOne.Product), filledOne, procSpread.productOne);
                                }
                                else
                                {
                                    offOrders[sok[0]] = legs[0];
                                }
                                if (filledTwo != 0)
                                {
                                    if (log.IsDebugEnabled) log.Debug("cancel didnt go off completely. selling " + filledTwo);
                                    procSpread.SendOrder("Sell", market.bid(procSpread.productTwo.Product), filledTwo, procSpread.productTwo);
                                }
                                else
                                {
                                    offOrders[sok[1]] = legs[1];
                                }
                            }

                            delorders.Add(so);
                            break;
                        }

                        //XXX shift the spread if necassary. 
                   //     else if (so.fillPrice() == 0)
                   //     {
                  //          checkSpreadLevel(so);
                  //      }

                    }
    
                    
                }


                foreach(SpreadOrder dspo in delorders){
                    orders.Remove(dspo);
                }
                //--------------------------------------
                //--------------------------------------


                signal.trader();              
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error(ex.StackTrace);
            }
        }

        
        private void checkSpreadLevel( SpreadOrder so ){
            double[] mktprices = findPrices(so.buySell);
            double mktlevel = mktprices[2];

            Order[] o = so.getLegs();
            double[] wkprices = so.workPrices();

            //the sell leg is off mkt
            if (so.buySell == "Buy" && o[1].Price > market.ask(o[1].IPROC.Product) )
            {
                if (log.IsDebugEnabled)
                    log.Debug("BUY: sell leg is off mkt shift spread lower");

                //lower sell leg
                CancelReplace( o[1].buySell
                              ,o[1].SOK
                              ,o[1].IPROC
                              ,o[1].WkQty
                              ,1 );                

                //lower the buy leg                
                CancelReplace( o[0].buySell
                              ,o[0].SOK
                              ,o[0].IPROC
                              ,o[0].WkQty
                              ,-1 );

                o[1].Price = market.ask(o[1].IPROC.Product) - o[1].IPROC.TickPrice;
                o[0].Price = market.bid(o[0].IPROC.Product) - o[0].IPROC.TickPrice;
            }

                //the buy leg is off mkt
            else if (so.buySell == "Buy" && o[0].Price < market.bid(o[0].IPROC.Product))
            {
                if (log.IsDebugEnabled)
                    log.Debug("BUY: buy leg is off mkt shift spread higher");

                //higher sell leg
                CancelReplace(o[1].buySell
                              , o[1].SOK
                              , o[1].IPROC
                              , o[1].WkQty
                              , -1);

                //higher the buy leg
                CancelReplace(o[0].buySell
                              , o[0].SOK
                              , o[0].IPROC
                              , o[0].WkQty
                              , 1);

                o[1].Price = market.ask(o[1].IPROC.Product) + o[1].IPROC.TickPrice;
                o[0].Price = market.bid(o[0].IPROC.Product) + o[0].IPROC.TickPrice;
            }

            //the sell leg is off mkt
            else if (so.buySell == "Sell" && o[0].Price > market.ask(o[0].IPROC.Product))
            {
                if (log.IsDebugEnabled)
                    log.Debug("SELL: sell leg is off mkt shift spread lower");
                    

                //lower sell leg
                CancelReplace( o[0].buySell
                              , o[0].SOK
                              , o[0].IPROC
                              , o[0].WkQty
                              , 1);

                //lower the buy leg
                CancelReplace( o[1].buySell
                              , o[1].SOK
                              , o[1].IPROC
                              , o[1].WkQty
                              , -1);

                o[0].Price = market.ask(o[0].IPROC.Product) - o[0].IPROC.TickPrice;
                o[1].Price = market.bid(o[1].IPROC.Product) - o[1].IPROC.TickPrice;
            }

            //the buy leg is off mkt
            else if (so.buySell == "Sell" && o[1].Price < market.bid(o[1].IPROC.Product))
            {
                if (log.IsDebugEnabled)
                    log.Debug("SELL: buy leg is off mkt shift spread higher");

                //higher sell leg
                CancelReplace( o[0].buySell
                              ,o[0].SOK
                              ,o[0].IPROC
                              ,o[0].WkQty
                              ,-1);

                //higher the buy leg
                CancelReplace(o[1].buySell
                              , o[1].SOK
                              , o[1].IPROC
                              , o[1].WkQty
                              , 1);

                o[0].Price = market.ask(o[0].IPROC.Product) + o[0].IPROC.TickPrice;
                o[1].Price = market.bid(o[1].IPROC.Product) + o[1].IPROC.TickPrice;
            }
             
        }
        

        public void DeleteAll() {
            orders.Clear();
            procSpread.DeleteAll();
        }




        private SpreadOrder findSpread(string key) {

            foreach( SpreadOrder so in orders){
                string[] sok = so.getSOKs();

                if (sok[0] == key || sok[1] == key)
                {
                    return so;
                }
            }

            return null;
        }


        public int CURPOSONE { get { return currentPosOne;  }  }
        public int CURPOSTWO { get { return currentPosTwo; }   }


        private void OnFill(object sender, FillEventArgs e)
        {
            try
            {
                Fill fill = e.fill;
                string action = fill.Buy ? "Sell" : "Buy";


                //keep track of current positions
                if (fill.Product == procSpread.productOne.Product) {
                    currentPosOne += fill.Buy ? fill.Qty : -fill.Qty;
                }
                else if (fill.Product == procSpread.productTwo.Product)
                {
                    currentPosTwo += fill.Buy ? fill.Qty : -fill.Qty;
                }


                if (log.IsDebugEnabled)
                {
                    log.Debug("closeouts size= " + closeouts.Count);
                    log.Debug("fillbugs size= " + fillbugs.Count);
                    log.Debug("orders count " + orders.Count + "\n" + fill.ToString());
                    log.Debug("offorders size= " + offOrders.Count);
                }


                if (fillbugs.ContainsKey(fill.Key) && fill.Full)
                {
                    orders.Remove(fillbugs[fill.Key]);
                    fillbugs.Remove(fill.Key);
                    return;
                }


                SpreadOrder spo = findSpread(fill.Key);
                if (spo == null)
                {
                    if (offOrders.ContainsKey(fill.Key))
                    {
                        if (log.IsDebugEnabled) log.Debug("late fill in OffOrders " + fill.ToString() );

                        Order o = offOrders[fill.Key];
                        procSpread.SendOrder( o.buySell == "Buy" ? "Sell" : "Buy"
                                             ,o.buySell == "Buy" ? market.bid(o.IPROC.Product) - o.IPROC.TickPrice * 5 : market.ask(o.IPROC.Product) + o.IPROC.TickPrice * 5
                                             ,fill.Qty
                                             ,o.IPROC );

                        offOrders.Remove(fill.Key);
                    }

                    return;
                }


                spo.AddFill(fill);

                /*
                 * if we got back a full fill check to see if the whole spread is full
                 * if its not then work the open leg in a seperate thread incrementing the price by
                 * payUp params. add the thread to closeouts so we can stop it when it is full filled
                 */
                if( fill.Full )
                {
                    if( spo.IsFull() )
                    {
                        lastfill.Clear();
                        if (log.IsDebugEnabled) log.Debug("removing spread from orders");
                        orders.Remove(spo);
                        lastfill[spo.buySell] = spo.fillPrice();

                        if (log.IsInfoEnabled) 
                            log.Info( 
                                new System.Text.StringBuilder("SpreadID:").AppendFormat("{0} spreadType: {1} filled spreadcost: {2}"
                                                        ,spo.spreadID,spo.buySell,spo.fillPrice()   )                       
                                );

                        if (closeouts.ContainsKey(fill.Key))
                        {
                            try
                            {
                                closeouts[fill.Key].Abort();
                            }
                            catch(Exception exc){
                                if (log.IsErrorEnabled) log.Error(exc.StackTrace);
                            }

                            closeouts.Remove(fill.Key);
                        }
                    }
                    else
                    {
                        Order wkLeg = spo.getWorkingLeg();
                        Order filledLeg = spo.getFilledLeg();

                        //we got a Full fill but the spread itself is not full. however we are already running the cleanup
                        //thread which means TT sent back full fill error
                        if (closeouts.ContainsKey(wkLeg.SOK))
                        {
                            if (log.IsDebugEnabled) log.Debug("XXX fix for full fill bug: " + wkLeg.SOK);
                            try{
                                closeouts[wkLeg.SOK].Abort();
                            }
                            catch(Exception exc){
                                if (log.IsErrorEnabled) log.Error(exc.StackTrace);
                            }

                            closeouts.Remove(wkLeg.SOK);
                            string k = procSpread.SendOrder(wkLeg.buySell
                                                         , wkLeg.buySell == "Buy" ? market.ask(wkLeg.IPROC.Product) + wkLeg.IPROC.TickPrice * 5 : market.bid(wkLeg.IPROC.Product) - wkLeg.IPROC.TickPrice * 6
                                                         , wkLeg.WkQty
                                                         , wkLeg.IPROC);

                            fillbugs.Add(k, spo);
                        }
                        else{
                            if (log.IsDebugEnabled) log.Debug("closeout out remaing leg: " + wkLeg.SOK);

                            Thread thrdCloseout = new Thread(delegate()
                                            {
                                                int PayUpTicks = spread.PayUpTicks;
                                                for (int i = 0; i < spread.PayUpCount; i++)
                                                {
                                                    double price = action == "Buy" ? wkLeg.Price + wkLeg.IPROC.TickPrice * PayUpTicks : wkLeg.Price - wkLeg.IPROC.TickPrice * PayUpTicks;

                                                    int wkqty = wkLeg.WkQty;
                                                    if (wkqty > 0)
                                                    {
                                                        CancelReplace(action, wkLeg.SOK, wkLeg.IPROC, wkqty, price);
                                                    }
                                                    else {
                                                        break;
                                                    }

                                                    Thread.Sleep(spread.PayUpPeriod);
                                                    PayUpTicks++;
                                                }

                                                //if we exhausted payUpCount and still not filled then closeout full leg.
                                               if( wkLeg.WkQty == wkLeg.IPROC.Size ){
                                                   scratchLegs.Enqueue(filledLeg);
                                                   closeouts.Remove(wkLeg.SOK);
                                                   procSpread.Delete(wkLeg.SOK,wkLeg.IPROC.ttOrderSet);
                                               }
                                            });
                            thrdCloseout.Start();
                            closeouts.Add(wkLeg.SOK, thrdCloseout);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error(ex.StackTrace + "\n\n" + ex.Message);
            }
        }


        /*
         * close out a spread that was half filled and we no longer are chasing the remaining leg
         */
        private List<string> attemptScratchLeg(){
            List<string> soks = new List<string>();

            while( scratchLegs.Count >0 ){
                Order o = scratchLegs.Dequeue();
                string ordKey = procSpread.SendOrder( o.buySell == "Buy" ? "Sell" : "Buy"
                                                ,o.buySell == "Buy" ? market.bid(o.IPROC.Product) - o.IPROC.TickPrice * 6 : market.ask(o.IPROC.Product) + o.IPROC.TickPrice * 6
                                                ,o.IPROC.Size
                                                ,o.IPROC);

                soks.Add(ordKey);
            }

            return soks;
        }


        private bool CancelReplace(string buySell, string key, InstrumentProc iproc, int WkQty, double price)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(new System.Text.StringBuilder("CXLR ").AppendFormat("{0} price:{1} qty:{2}"
                                            ,buySell,price,WkQty)
                                            );                    
            }
            return procSpread.CxlReplace(buySell, key, iproc, price, WkQty);
        }


        
        private bool CancelReplace(string buySell, string key, InstrumentProc iproc, int WkQty, int PayUpTicks)
        {
            double price = buySell == "Buy" ? market.bid(iproc.Product) + iproc.TickPrice * PayUpTicks : market.ask(iproc.Product) - iproc.TickPrice * PayUpTicks;
            if(log.IsDebugEnabled){
                log.Debug(                    
                    new System.Text.StringBuilder("CXLR ").AppendFormat("{0} price:{1} qty:{2}"
                                            ,buySell,price,WkQty)                                            
                 );
            }
            return procSpread.CxlReplace(buySell, key, iproc, price,WkQty);
        }
        

        public void resetStrategy(Session session){
            signal.resetparams( new object[] { session.spread.Threshold
                                              ,session.spread.TrendFollow
                                              ,session.spread.PriceLength
                                              ,session.spread.Signal });
            spread = session.spread;
        }


        //called only once when we connect.
        public void init(){
            string[] tickers = new string[] { procSpread.productOne.Product
                                             ,procSpread.productTwo.Product };

            market = Market.getInstance( tickers);
            
            SignalFactory factory = new SignalFactory(spread);
            signal = factory.create();                      

            signal.TradeEvt += this.OnTrade;
        }



        public double[] findPrices(string buySell){
            double aPrice;
            double bPrice;


            if (buySell == "Buy")
            {
                aPrice = market.bid(procSpread.productOne.Product);
                bPrice = market.ask(procSpread.productTwo.Product) ;
            }
            else
            {
                aPrice = market.ask(procSpread.productOne.Product);
                bPrice = market.bid(procSpread.productTwo.Product);
            }

            double cost = aPrice * procSpread.productOne.Size * procSpread.productOne.InstrumentMultiplier
                                        - bPrice * procSpread.productTwo.Size * procSpread.productTwo.InstrumentMultiplier;

            return new double[] { aPrice, bPrice, cost };
        }



        // with check to make sure we dont keep adding to position at worse price
        private string[] SendSpreadOrder(string buySell, bool check)
        {
            double[] prices = findPrices(buySell);
            string[] sok;

            if (check)
            {
                if (!OrderCheck(buySell, prices[2]))
                {
                    if (log.IsDebugEnabled)
                    {                        
                        log.Debug(
                            new System.Text.StringBuilder("not placing order: ").AppendFormat("{0} {1} {2}"
                                                                    ,buySell,prices[2],lastfill[buySell]  )                                                
                            );
                    }
                    return null;
                }
            }

            sok = SendSpreadOrder(buySell,prices);

            return sok;
        }


        private string[] SendSpreadOrder(string buySell, double[] prices)
        {
            string action = buySell == "Buy" ? "Sell" : "Buy";


            double aPrice = prices[0];
            double bPrice = prices[1];


            string a = procSpread.SendOrder(buySell, aPrice, procSpread.productOne);
            string b = procSpread.SendOrder(action, bPrice, procSpread.productTwo);


            Order one = new Order(buySell, procSpread.instruments[0].Size, aPrice, a, procSpread.productOne);
            Order two = new Order(action, procSpread.instruments[1].Size, bPrice, b, procSpread.productTwo);
            SpreadOrder so = new SpreadOrder(new Order[] { one, two } , Interlocked.Add(ref spreadIDcount, 1) );

            orders.Add(so);

            //log the cost of the spread            
            if (log.IsInfoEnabled) 
                log.Info(
                    new System.Text.StringBuilder("spreadID:").AppendFormat("{0} orders: {1} {2} {3} @ {4}"
                                                                    ,spreadIDcount,a,b,buySell,prices[2] )                    
                );

            return new string[] { a, b };
        }


        //returns string[] of orderIds
        public string[] SendSpreadOrder(string buySell)
        {

            string action = buySell =="Buy" ? "Sell" : "Buy";

            double[] prices = findPrices(buySell);
            double aPrice = prices[0];
            double bPrice = prices[1];


            string a = procSpread.SendOrder(buySell, aPrice, procSpread.productOne);
            string b = procSpread.SendOrder(action , bPrice, procSpread.productTwo);


            Order one = new Order(buySell, procSpread.instruments[0].Size, aPrice, a, procSpread.productOne);
            Order two = new Order(action, procSpread.instruments[1].Size, bPrice, b, procSpread.productTwo);
            SpreadOrder so = new SpreadOrder(new Order[] { one, two } , Interlocked.Add(ref spreadIDcount, 1) );

            orders.Add(so);

            //log the cost of the spread
            if (log.IsInfoEnabled) 
                log.Info(
                    new System.Text.StringBuilder("spreadID:").AppendFormat("{0} orders: {1} {2} {3} @ {4}"
                                                             ,spreadIDcount,a,b,buySell,prices[2] )
                );

            return new string[] { a, b };
        }



        private bool OrderCheck(string buySell, double cost) {
            bool status = true;

            if( lastfill.ContainsKey(buySell) )
            {
                int adj = cost<0 ? -1 : 1;

                if ( (buySell == "Buy"  && cost > Math.Abs(lastfill[buySell]) * adj ) ||
                     (buySell == "Sell" && cost < Math.Abs(lastfill[buySell]) * adj )
                    ){
                        status = false;
                 }
             }

            return status;
        }


        public void reapClsThreads()
        {
            foreach( Thread thrd in closeouts.Values ){
                try
                {
                    thrd.Abort();
                }
                catch(Exception ex){
                    if (log.IsErrorEnabled) log.Error(ex.StackTrace);
                }
            }
        }

        /*
         * calculate hamming distance between vectors to determine trend
         *  a= binary vector of up and down intervals
         *  b= sorted of a or reverse sorted of a ...to create perfect trend
         *
         *  returns hamming distance: smaller distance implies closer to trend
         */
        public static int hamming(int[] a, int[] b)
        {
            int sum = 0;

            if (a.Length != b.Length)
            {
                return -1;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    sum += a[i] + b[i];
                }
            }

            return sum;
        }
    }
}
