using System;
using System.Windows.Forms;
using System.Collections.Generic;


//look for log4net config file ... <blah>.exe.config
[assembly: log4net.Config.XmlConfigurator(Watch = true)]


namespace EZSpreader
{
    public class SpreadProc
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // This is a singleton
        static private SpreadProc instance = null;

        private bool started = false;

        public List<Instrument> instruments = null;

        private XTAPI.TTGate TTGate = new XTAPI.TTGate();

        //private Session session;
        public InstrumentProc productOne;
        public InstrumentProc productTwo;


        public event EventHandler<FillEventArgs> OnFill;
        public event EventHandler<UpdateEventArgs> OnUpdate;


        XTAPI.TTInstrNotifyClass TTInstrNotify;


        private SpreadProc()
        {
            TTGate.RapidFillDelivery = 1;
            

            TTInstrNotify = new XTAPI.TTInstrNotifyClass();
            TTInstrNotify.UpdateFilter = "BID,ASK";

            if(log.IsInfoEnabled)
                log.Info("trading started for spread");
        }

        /*
        public static SpreadProc getInstance()
        {
            return (instance);
        }
        */
        public static SpreadProc getInstance()
        {
            if (instance == null)
                instance = new SpreadProc();

            return (instance);
        }


        public bool Started {
            get { return started;  }
            set { started = value; }
        }



        public void Terminate()
        {
            TTGate.XTAPITerminate();
        }


        private void OnNotifyUpdate(XTAPI.TTInstrNotify pNotify, XTAPI.TTInstrObj pInstr)
        {
            try
            {
                Array data = (Array)pInstr.get_Get("Bid#,Ask#,NetPos,PL.Z,OpenPL^");

                //get currency amount of tick
                double tickvalue = Convert.ToDouble(pInstr.get_TickPrice(0, 1, "^"));

                for (int i = 0; i < data.Length; ++i)
                    if (data.GetValue(i) == null)
                        return;

                Update update = new Update();

                update.Bid = Convert.ToDouble(data.GetValue(0));
                update.Ask = Convert.ToDouble(data.GetValue(1));
                update.NetPos = Convert.ToInt16(data.GetValue(2));
                //convert number of ticks from PL to currency
                update.PL = tickvalue * Convert.ToDouble(data.GetValue(3));
                update.OpenPL = Convert.ToDouble(data.GetValue(4));
                update.Symbol = pInstr.Product;


                if (OnUpdate != null)
                    OnUpdate(this, new UpdateEventArgs(update));
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error(ex.StackTrace);
            }
        }


        private void OnStatusUpdate(int statusChange, string desc)
        {
            try
            {
                if (log.IsInfoEnabled) log.Info("status: " + statusChange + " " + desc);
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error(ex.StackTrace);
            }
        }


        private void OnOrderFillData(XTAPI.TTFillObj pFill)
        {
            try
            {
                Fill fill = new Fill();

                Array data = (Array)pFill.get_Get("Key,FillType,BuySell,Qty,Price#,NetQty,Acct,Contract.Product");

                 
                fill.Key = (string)data.GetValue(0);
                fill.Full = ((string)data.GetValue(1)).Equals("F");
                fill.Buy = ((string)data.GetValue(2)).Equals("B");
                fill.Qty = (int)data.GetValue(3);
                fill.Price = (double)data.GetValue(4);
                fill.NetQty = (int)data.GetValue(5);
                fill.Account = (string)data.GetValue(6);
                fill.Product = (string)data.GetValue(7);

                if (OnFill != null )
                {                    
                    OnFill(this, new FillEventArgs(fill));
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error(ex.StackTrace);
            }
        }


        public void Connect(Session s)
        {
            productOne = new InstrumentProc(s.instrumentA);
            productTwo = new InstrumentProc(s.instrumentB);

            instruments = new List<Instrument>();
            instruments.Add(s.instrumentA);
            instruments.Add(s.instrumentB);

            productOne.init(TTInstrNotify);
            productTwo.init(TTInstrNotify);

            //setup the events
            TTGate.OnStatusUpdate += new XTAPI._ITTGateEvents_OnStatusUpdateEventHandler(this.OnStatusUpdate);

            productOne.ttOrderSet.OnOrderFillData += new XTAPI._ITTOrderSetEvents_OnOrderFillDataEventHandler(this.OnOrderFillData);
            TTInstrNotify.OnNotifyFound += new XTAPI._ITTInstrNotifyEvents_OnNotifyFoundEventHandler(productOne.OnNotifyFound);
            TTInstrNotify.OnNotifyFound += new XTAPI._ITTInstrNotifyEvents_OnNotifyFoundEventHandler(productTwo.OnNotifyFound);
            TTInstrNotify.OnNotifyUpdate += new XTAPI._ITTInstrNotifyEvents_OnNotifyUpdateEventHandler(this.OnNotifyUpdate);

            productTwo.ttOrderSet.OnOrderFillData += new XTAPI._ITTOrderSetEvents_OnOrderFillDataEventHandler(this.OnOrderFillData);

            if (log.IsInfoEnabled)
            {

                log.Info("trading started for spread");
            }

            setLogname();

            /*
            XTAPI.InstrCollection colInst = (XTAPI.InstrCollection)TTGate.Instruments;
            for (int i = 1; i <= colInst.Count; ++i)
            {
                XTAPI.TTInstrObj instr = (XTAPI.TTInstrObj)colInst[i];
               }
            */
        }


        public string SendOrder(string buySell, double price,int Qty, InstrumentProc instrument)
        {
            XTAPI.TTOrderProfileClass TTOrderProfile = new XTAPI.TTOrderProfileClass();

            TTOrderProfile.Instrument = instrument.ttInstrObj;

            TTOrderProfile.Customer = "<Default>";

            TTOrderProfile.Set("Acct", instrument.Account);
            TTOrderProfile.Set("AcctType", "M1");
            TTOrderProfile.Set("BuySell", buySell);
            TTOrderProfile.Set("Qty", Convert.ToString(Qty));
            TTOrderProfile.Set("OrderType", "L");
            TTOrderProfile.Set("Limit#", Convert.ToString(price));
            TTOrderProfile.Set("FFT2", "M");
            TTOrderProfile.Set("FFT3", "AutoMarket");

            try
            {
                int submittedQuantity = instrument.ttOrderSet.get_SendOrder(TTOrderProfile);
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) log.Error(ex.StackTrace);
            }

            return (Convert.ToString(TTOrderProfile.get_GetLast("SiteOrderKey")));

        }

        public string SendOrder(string buySell, double price, InstrumentProc instrument) {

            XTAPI.TTOrderProfileClass TTOrderProfile = new XTAPI.TTOrderProfileClass();

            TTOrderProfile.Instrument = instrument.ttInstrObj;

            TTOrderProfile.Customer = "<Default>";

            TTOrderProfile.Set("Acct", instrument.Account);
            TTOrderProfile.Set("AcctType", "M1");
            TTOrderProfile.Set("BuySell", buySell);
            TTOrderProfile.Set("Qty", Convert.ToString(instrument.Size));
            TTOrderProfile.Set("OrderType", "L");
            TTOrderProfile.Set("Limit#", Convert.ToString(price));
            TTOrderProfile.Set("FFT2", "M");
            TTOrderProfile.Set("FFT3", "AutoMarket");

            try
            {
                int submittedQuantity = instrument.ttOrderSet.get_SendOrder(TTOrderProfile);
            }
            catch (Exception ex) {
                if (log.IsErrorEnabled) log.Error(ex.StackTrace);
            }

            return (Convert.ToString(TTOrderProfile.get_GetLast("SiteOrderKey")));

        }


        public bool CxlReplace(string buySell, string key, InstrumentProc iproc, double price, int WkQty)
        {
            bool status = true;

            if (log.IsDebugEnabled) {
                log.Debug(
                        new System.Text.StringBuilder("cxlreplace called ")
                                        .AppendFormat("{0} {1} qty:{2}",key,price,WkQty)                        
                    );
            }


            XTAPI.TTOrderObj orderobj = (XTAPI.TTOrderObj)iproc.ttOrderSet.get_SiteKeyLookup(key);


            /*
             * http://devnetkb.tradingtechnologies.com/ViewKnowledgeEntry.aspx?KnowledgeEntryID=2387
              */

            try
            {
                while ((orderobj != null) && (Convert.ToInt32(orderobj.get_Get("ExecutionType")) != 1))
                //while ( (orderobj.IsNull == 0) && (Convert.ToInt32(orderobj.get_Get("ExecutionType")) != 1) )
                {
                    System.Threading.Thread.Sleep(5);
                    orderobj = (XTAPI.TTOrderObj)iproc.ttOrderSet.get_SiteKeyLookup(key);
                }
            }
            catch(Exception exo){
                return false;
            }


            if (orderobj.IsNull == 0 )
            {
                XTAPI.TTOrderProfile TTOrderProfile = new XTAPI.TTOrderProfile();

                TTOrderProfile = orderobj.CreateOrderProfile;

                TTOrderProfile.Instrument = iproc.ttInstrObj;
                TTOrderProfile.Customer = "<Default>";

                TTOrderProfile.Set("Acct", iproc.Account);
                TTOrderProfile.Set("AcctType", "M1");
                TTOrderProfile.Set("BuySell", buySell);
                TTOrderProfile.Set("Qty", WkQty);
                TTOrderProfile.Set("OrderType", "L");
                TTOrderProfile.Set("Limit#", Convert.ToString(price));
                TTOrderProfile.Set("FFT2", "M");
                TTOrderProfile.Set("FFT3", "CXLR");

                try
                {
                    iproc.ttOrderSet.UpdateOrder(TTOrderProfile, 0);
                }
                catch (Exception ex)
                {

                    status = false;

                    if (log.IsErrorEnabled)
                    {
                        System.Text.StringBuilder msg = new System.Text.StringBuilder("ERROR cxlreplace order ")
                                                .AppendFormat("{0} {1} {2}",key,buySell,WkQty);                                                    
                        log.Error(msg);
                        log.Error(ex.StackTrace);
                        log.Error(ex.Message);
                    }
                }
            }

            return status;
        }



        public int DeleteAll()
        {
            int res = 0;
            try
            {
                res += productOne.ttOrderSet.get_DeleteOrders(true, 0, 0, 0, null);
                res += productOne.ttOrderSet.get_DeleteOrders(false, 0, 0, 0, null);

                res += productTwo.ttOrderSet.get_DeleteOrders(true, 0, 0, 0, null);
                res += productTwo.ttOrderSet.get_DeleteOrders(false, 0, 0, 0, null);
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex.Message);
                }
            }

            return (res);
        }


        public int Delete(string key , XTAPI.TTOrderSetClass ttorderSet)
        {
            XTAPI.TTOrderObj order = (XTAPI.TTOrderObj)ttorderSet.get_SiteKeyLookup(key);
            int res = order.DeleteOrder();

            if (log.IsDebugEnabled)
                log.Debug( 
                    new System.Text.StringBuilder("Deleted: ").AppendFormat("{0} from {1}",res,key)
                    );

            return (res);
        }




        /*
         * reset the logfile name to Product_Expiration_YYYYMMDD.log
         */
        private void setLogname()
        {
            DateTime CurrTime = DateTime.Now;

            string logfile = "c:\\EZ\\logs\\spreader\\" + CurrTime.ToString("yyyyMMdd") + "_" +
                instruments[0].Product + "_" + instruments[0].Contract + "_" +
                instruments[1].Product + "_" + instruments[1].Contract + "_" +
                instruments[0].Account + ".log";


            foreach (log4net.Appender.IAppender appender in
                     log4net.LogManager.GetRepository().GetAppenders())
            {
                if (appender is log4net.Appender.FileAppender)
                {
                    ((log4net.Appender.FileAppender)appender).File = logfile;
                    ((log4net.Appender.FileAppender)appender).ActivateOptions();
                }
            }
        }


    }
}
