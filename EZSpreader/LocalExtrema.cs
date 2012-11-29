using System;
using System.Collections.Generic;


namespace EZSpreader
{


    public struct Convergence
    {
        public Convergence(double latest, double mx, double nextmx, double mn, double nextmn)
        {
            Latest = latest;
            HiOne = mx;
            HiTwo = nextmx;
            LowOne = mn;
            LowTwo = nextmn;
        }

        private readonly double Latest;
        private readonly double HiOne;
        private readonly double HiTwo;
        private readonly double LowOne;
        private readonly double LowTwo;

        public bool IsLow {
            get { return Latest == LowOne; }
        }

        public bool IsHi
        {
            get { return Latest == HiOne; }
        }
    }


    class LocalExtrema
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<double> extremas(List<Quote> spreads)
        {
            List<double> mids = new List<double>();

            foreach(Quote q in spreads){
                mids.Add( (q.bid + q.ask ) * 0.5 );
            }

            return LocalExtrema.extremas(mids);
        } 
      

        /*
         * determine inflection points of a list by finding sign changes
         */ 
        public static List<double> extremas(List<double> inlist)
        {            
            double[] diffs = new double[inlist.Count - 1];            
            List<double> inflections = new List<double>();

            //element by element diffs
            for (int i = 1; i < inlist.Count; i++)
            {
                diffs[i - 1] = inlist[i] - inlist[i - 1];
            }

            /*
             *  multiply vectors [ 0 : listLength-1] and [1 : listLength]
             *  if we are passed the first element and we're negative
             *  then add the next element to the extremes List
             */
            for (int i = 0; i < diffs.Length - 1; i++)
            { 
                double x = diffs[i] * diffs[i + 1];

                if (i>0 && x<0)
                        inflections.Add(inlist[i + 1]);
            }
             

            return inflections;
        }
         

        /*
         * take a list of inflection points and set the convergence struct with data
         */ 
        public static Convergence relevant(List<double> extremas)
        {
            if (extremas.Count <= 1)
            {
                if(log.IsDebugEnabled) log.Debug("list is too small");
                return new Convergence();
            }


            //latest,max,nextmax,min,nextmin
            double latest = extremas[extremas.Count - 1];

            extremas.Sort();
            double mn = extremas[0];
            double nextmn = extremas[1];

            extremas.Reverse();
            double mx = extremas[0];
            double nextmx = extremas[1];

            //if (log.IsDebugEnabled) log.Debug("Convergence: " + latest + " " + mx + " " + nextmx + " " + mn + " " +  nextmn);
            Console.WriteLine("Convergence: " + latest + " " + mx + " " + nextmx + " " + mn + " " + nextmn);

            return new Convergence(latest, mx, nextmx, mn, nextmn);
        }


        public static Convergence releventWrap(List<Quote> spreads)
        {             
            List<double> ext = LocalExtrema.extremas(spreads);
            return LocalExtrema.relevant(ext);
        }


        public static Convergence releventWrap(List<double> zlist)
        {            
            List<double> ext = LocalExtrema.extremas(zlist);         
            return LocalExtrema.relevant(ext);
        }

    }
}
