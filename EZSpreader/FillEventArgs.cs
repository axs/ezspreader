using System;
using System.Collections.Generic;
using System.Text;

namespace EZSpreader
{
    public class FillEventArgs : EventArgs
    {
        public readonly Fill fill;

        public FillEventArgs(Fill fill)
        {
            this.fill = fill;
        }
    }
}
