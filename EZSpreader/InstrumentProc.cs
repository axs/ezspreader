using System;
using System.Windows.Forms;
using System.Collections.Generic;


namespace EZSpreader
{
    public class InstrumentProc
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool started = false;

        Instrument instrument = null;

        private double tickPrice = 0.0;
        private int tickIncr = 0;
        //private int wkQty = 0;


        private XTAPI.TTInstrObjClass TTInstrObj = null;
        private XTAPI.TTOrderSetClass TTOrderSet = null;

        public event EventHandler<FoundEventArgs> OnFound;


        public XTAPI.TTInstrObjClass ttInstrObj
        {
            get { return TTInstrObj;  }
        }


        public double TickPrice{
             get {
            return tickPrice;
            }
        }

        public XTAPI.TTOrderSetClass ttOrderSet
        {
            get { return TTOrderSet; }
        }

       

        public InstrumentProc(Instrument instrument)
        {
            this.instrument = instrument;
        }


        public bool Started { set { started = value; } get { return started; } }


        public Instrument Instru
        {
            set { this.instrument = value;  }
        }


        public double InstrumentMultiplier
        {
            get {return instrument.Multiplier;}
        }

        public int Size
        {
            get{ return instrument.Size;}
            set{ instrument.Size = value;}
        }
        public string Account
        {
            get { return instrument.Account; }
        }

        public string Product
        {
            get { return instrument.Product; }
        }


        public void init( XTAPI.TTInstrNotifyClass TTInstrNotify)
        {
            if (TTOrderSet != null && TTInstrObj != null && TTInstrNotify != null )
            {
                TTInstrNotify.DetachInstrument(TTInstrObj);
            }

            TTOrderSet = new XTAPI.TTOrderSetClass();

            TTOrderSet.Alias = instrument.Product;
            TTOrderSet.EnableFillCaching = 1;
            TTOrderSet.EnableOrderSend = 1;
            TTOrderSet.Set("NetLimits", false);
            TTOrderSet.EnableOrderSetUpdates = 1;
            TTOrderSet.EnableOrderFillData = 1;
            TTOrderSet.EnableOrderAutoDelete = 1;
            TTOrderSet.ClearQuotePosition(0,0,0);

            TTInstrObj = new XTAPI.TTInstrObjClass();
            TTInstrObj.Exchange = instrument.Exchange;
            TTInstrObj.Product = instrument.Product;
            TTInstrObj.Contract = instrument.Contract;
            TTInstrObj.ProdType = instrument.ProductType;
            TTInstrObj.Alias = instrument.Product;


            TTInstrNotify.AttachInstrument(TTInstrObj);


            XTAPI.TTOrderSelector orderSelector = TTInstrObj.CreateOrderSelector;


            //match orders per account
            XTAPI.TTOrderSelector acctSelector = new XTAPI.TTOrderSelectorClass();
            acctSelector.AddTest("Acct", instrument.Account);
            acctSelector.AllMatchesRequired = 1;

            orderSelector.AddSelector(acctSelector);
            orderSelector.AllMatchesRequired = 1;


            TTOrderSet.OrderSelector = orderSelector;

            TTOrderSet.Open(true);

            TTInstrObj.OrderSet = TTOrderSet;
            TTInstrObj.Open(1);



            if (log.IsInfoEnabled)
            {
                System.Text.StringBuilder msg = new System.Text.StringBuilder("trading started for ")
                                                .AppendFormat("{0} {1} {2} {3}"
                                                , instrument.Account, instrument.Contract,instrument.Product,instrument.Exchange);
                log.Info(msg);
            }
        }


        public Array numOpenOrders()
        {

            return (Array)TTOrderSet.get_Get("BuyWrk,SellWrk");
        }

        public int NetOPos()
        {
            return (int)TTOrderSet.get_Get("NetOPos");
        }

        public int NetPos()
        {   
            return (int)TTOrderSet.get_Get("NetPos");
        }

        public void OnNotifyFound(XTAPI.TTInstrNotify pNotify, XTAPI.TTInstrObj pInstr)
        {
            try
            {
                //we use the same TTInstrNotify.OnNotifyFound event so we need to check the Product
                if( Convert.ToString(pInstr.get_Get("Product")) == instrument.Product ) 
                {
                    tickPrice = Convert.ToDouble(pInstr.get_TickPrice(0, 1, "#"));
                    tickIncr = Convert.ToInt16(pInstr.get_Get("TickIncrement"));
                }
               

                if (OnFound != null)
                    OnFound(this, new FoundEventArgs(tickPrice));

                Application.DoEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
