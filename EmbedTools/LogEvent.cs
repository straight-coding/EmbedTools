/*
  LogEvent.cs
  Author: Straight Coder<simpleisrobust@gmail.com>
  Date: Nov 13, 2020
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmbedTools
{
    public class LogEventArgs : EventArgs
    {
        public DateTime Timestamp { get; set; }
        public String Message { get; set; }
    }
}
