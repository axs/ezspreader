using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Timers;

 
namespace EZSpreader
{
    public partial class EZSpreader : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Session session;
        private SpreadProc procSpread;
        private StrategyProc procStrategy;
        private string Config;

        private System.Timers.Timer RatioVerifyTimer;
        private double testRatio;

        public EZSpreader(string cfg)
        {
            this.Config = cfg;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                session = Session.getInstance();
                XmlSerializer xs = new XmlSerializer(typeof(Session));

                using (Stream str = File.OpenRead(this.Config))
                    session = (Session)xs.Deserialize(str);

                            
                procSpread = SpreadProc.getInstance();
                procSpread.OnUpdate += new EventHandler<UpdateEventArgs>(procSpread_OnUpdate);
                
                procStrategy = StrategyProc.getInstance(session.spread);

                //procStrategy.OnZscoreEvt += this.OnZscore;
                
                pgInstrumentA.SelectedObject = session.instrumentA;
                pgInstrumentB.SelectedObject = session.instrumentB;
                pgSpread.SelectedObject = session.spread;

            }
            catch(Exception ex){
                MessageBox.Show(ex.Message);
            }
        }

        private void procSpread_OnUpdate(object sender, UpdateEventArgs e)
        {
            double[] b = procStrategy.findPrices("Buy");
            double[] s = procStrategy.findPrices("Sell");

            this.textBoxBid.Text = Convert.ToString(b[2]);
            this.textBoxAsk.Text = Convert.ToString(s[2]);
        }

        /*
        private void OnZscore(object sender, ZscoreEventArgs e )
        {
            try
            {
                zscoreBid.Text = e.zscores[0].ToString();
                zscoreAsk.Text = e.zscores[1].ToString();
            }
            catch (Exception ex) { 
                
            }
        }
        */

        private void OnFound(object sender, FoundEventArgs e)
        {        
            btnConnect.Enabled = false;

            pgInstrumentA.Enabled = false;
            pgInstrumentB.Enabled = false;
            
            btnApplyStrat.Enabled = true;
            btnStart.Enabled = true;
            btnOpen.Enabled = false;

           this.Text = getInstrumentLine();

            Application.DoEvents();
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                procSpread.Terminate();
                //procStrategy.reapClsThreads();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnConnect_Click(object sender, EventArgs e)
        {
             
            btnConnect.Enabled = false;
             
            procSpread.Connect(session);
            procSpread.productOne.OnFound += this.OnFound;
            procStrategy.init();



            this.testRatio = Convert.ToDouble(procSpread.productOne.Size) / Convert.ToDouble(procSpread.productTwo.Size);

            RatioVerifyTimer = new System.Timers.Timer();
            RatioVerifyTimer.Elapsed += new ElapsedEventHandler(RatioVerifyEvent);
            RatioVerifyTimer.Interval = 1000 * 60 * 1;              
            RatioVerifyTimer.Start();
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            procSpread.Started = true;
            btnStart.BackColor = Color.Green;
            btnStart.Enabled = false;
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            procStrategy.SendSpreadOrder("Buy");
        }

        private void Sellbtn_Click(object sender, EventArgs e)
        {
            procStrategy.SendSpreadOrder("Sell");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            procStrategy.DeleteAll();
            procSpread.Started = false;
            btnStart.BackColor = Color.Red;
            btnStart.Enabled = true;
        }

        private void btnApplyStrat_Click(object sender, EventArgs e)
        {
            procStrategy.resetStrategy(session);
        }



        private void RatioVerifyEvent(object source, ElapsedEventArgs e)
        {
            int openone = procStrategy.CURPOSONE;
            int opentwo = procStrategy.CURPOSTWO;

            if (openone == 0 || opentwo == 0) { return ; }

            //log.Warn(openone + " <- open1 ~ open2 -> " + opentwo);

            double realRatio = Convert.ToDouble(openone) / Convert.ToDouble(opentwo) *  -1;

            //log.Warn(realRatio + " <- real ~ test -> " + testRatio);

            if( testRatio == realRatio ) 
            {                
                setRatioTextBox(true, 0);                
                //log.Warn("RatioVerifyEvent is good");                
            }
            else
            {
                double gap = (testRatio * opentwo * -1) - openone;                
                setRatioTextBox(false, gap);                
                log.Warn("RatioVerifyEvent is off");
            }
        }


        //set the btnconnect text to the gap of the first leg
        private void setRatioTextBox(bool v,double gap ) {
            if (v)
            {                
                this.btnConnect.Text = "Connect";                
                this.btnConnect.BackColor = System.Drawing.SystemColors.Control;
            }
            else
            {                                
                this.btnConnect.Text = Convert.ToString(gap);                
                this.btnConnect.BackColor = Color.BlueViolet;                
            }
        }

        private string getInstrumentLine()
        {

            return procSpread.instruments[0].Product + "_" + procSpread.instruments[0].Contract + "_" +
                procSpread.instruments[1].Product + "_" + procSpread.instruments[1].Contract + "_" +
                procSpread.instruments[0].Account;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                Stream str = null;
                XmlSerializer xs = new XmlSerializer(typeof(Session));

                ofd.Filter = "spread profiles (*.xml)|*.xml";
                ofd.FilterIndex = 1;
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if ((str = ofd.OpenFile()) != null)
                    {
                        using (str)
                            session = (Session)xs.Deserialize(str);
                    

                        pgInstrumentA.SelectedObject = session.instrumentA;
                        pgInstrumentB.SelectedObject = session.instrumentB;
                        pgSpread.SelectedObject = session.spread;                         
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
    }
}