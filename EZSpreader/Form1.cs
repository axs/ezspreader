using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;


namespace RWSpreader
{
    public partial class Form1 : Form
    {
        private Session session;
        private InstrumentProc procInstrument;
        private StrategyProc procStrategy;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                session = Session.getInstance();
                XmlSerializer xs = new XmlSerializer(typeof(Session));
                
                using (Stream str = File.OpenRead("Default.xml"))
                    session = (Session)xs.Deserialize(str);
                
                                
                List<Instrument> xin = new List<Instrument>();
                
                xin.Add(session.instrumentA);
                xin.Add(session.instrumentB);

                procInstrument = InstrumentProc.getInstance(xin);
                procStrategy = StrategyProc.getInstance(session.spread);
                

                pgInstrumentA.SelectedObject = session.instrumentA;
                pgInstrumentB.SelectedObject = session.instrumentB;
                pgSpread.SelectedObject = session.spread;
                 
            }
            catch(Exception ex){
                MessageBox.Show(ex.Message);
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                procInstrument.Terminate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnConnect_Click(object sender, EventArgs e)
        {
            procInstrument.Connect();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            int d = 1;
            //procInstrument.SendOrder("Buy", d, 7872, "AutoMarket");
            procInstrument.SpreadOrder("Buy");
        }
    }
}