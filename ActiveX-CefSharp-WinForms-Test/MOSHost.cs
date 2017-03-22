using ActiveX_CefSharp_WinForms_Test.DataSources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ActiveX_CefSharp_WinForms_Test
{
    public class MOSHost
    {
        private Form host;

        public MOSHost(Form host)
        {
            this.host = host;
            LogMessages = new BindingList<LogMessage>();
        }

        public BindingList<LogMessage> LogMessages
        {
            get; private set;
        }

        public void receiveMessageFromJavascript(string message)
        {
            host.Invoke((MethodInvoker)delegate
            {
               LogMessages.Add(new LogMessage(message));
            });

        }
    }
}
