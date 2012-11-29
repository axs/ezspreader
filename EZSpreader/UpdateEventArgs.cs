using System;
using System.Collections.Generic;
using System.Text;

namespace EZSpreader
{
    public class UpdateEventArgs : EventArgs
    {
        public readonly Update update;

        public UpdateEventArgs(Update update)
        {
            this.update = update;
        }
    }
}
