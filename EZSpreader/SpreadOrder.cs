using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace EZSpreader
{
    public class Order {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string BuySell = null;
        private int wkqty = 0;
        private double price = 0.0;
        private string orderID = null;
       

        private InstrumentProc iproc;
        private List<Fill> fills;

        public Order( string action, int wkqty, double Price, string SOK, InstrumentProc iproc){
            this.BuySell = action;
            this.wkqty   = wkqty;
            this.price = Price;
            this.orderID = SOK;
            this.iproc = iproc;

            fills = new List<Fill>();
        }

        public string buySell
        {
            get { return BuySell; }
        }

        public double Price
        {
            get { return price; }
            set { price = value; }
        }

        public InstrumentProc IPROC
        {
            get { return iproc; }
        }

        public int WkQty
        {
           get { //return (int)Interlocked.Read(ref wkqty );
               return wkqty;
                }
        }

        public string SOK
        {
            set { orderID = value; }
            get { return orderID; }
        }


        public void AddFill( Fill f ){
            fills.Add(f);
            Interlocked.Add(ref wkqty, -f.Qty);
            if (log.IsDebugEnabled) log.Debug(
                new System.Text.StringBuilder("setting wkqty: ")
                                    .AppendFormat("{0} filled: {1} {2}",wkqty,f.Qty,f.Key)
                       );
        }

        public List<Fill> getFills(){
            return  fills;
        }

        public bool IsFull() {
            return this.WkQty == 0 ? true : false;
        }

    }



    /*
     * SpreadOrder contains number of individual orders
     */
    public class SpreadOrder
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Order[] orders;
        private double level;
        private int spreadid;

        public SpreadOrder( Order[] orders, int spreadid ){
            this.orders = orders;
            this.level = Math.Abs(
                            orders[0].Price * orders[0].IPROC.Size - orders[1].Price * orders[0].IPROC.Size
                         );
            this.spreadid = spreadid;
        }

        public int spreadID
        {
            get { return spreadid; }
        }

        public string buySell
        {
            get { return orders[0].buySell; }
        }


        public double fillPrice()
        {
            double buycost  = 0;
            double sellcost = 0;

            foreach( Order o in orders ){
                foreach( Fill fill in o.getFills() )
                {
                    if( fill.NetQty > 0 )
                    {
                        buycost += fill.NetQty * fill.Price * o.IPROC.InstrumentMultiplier;
                    }
                    else
                    {
                        sellcost += fill.NetQty * fill.Price * o.IPROC.InstrumentMultiplier;
                    }
                }
            }

            return buycost + sellcost;
        }

        public double[] workPrices()
        {
            return new double[] { orders[0].Price, orders[1].Price };
        }

        
        public double Level {
            get { return level; }
        }


        public string[] getSOKs(){
            return new string[] { orders[0].SOK, orders[1].SOK };
        }


        public void AddFill(Fill f){
            foreach(Order o in orders){
                if( o.SOK == f.Key ){
                    o.AddFill(f);
                    break;
                }
            }
        }

        public Order[] getLegs(){
            return orders;
        }

        public Order getWorkingLeg(){
            foreach (Order o in orders) {
                if(! o.IsFull() ){
                    return o;
                }
            }
            return null;
        }


        public Order getFilledLeg()
        {
            foreach (Order o in orders)
            {
                if( o.IsFull() )
                {
                    return o;
                }
            }
            return null;
        }

        public bool IsFull() {
            return orders[0].IsFull() && orders[1].IsFull(); 
        }
    }
}
