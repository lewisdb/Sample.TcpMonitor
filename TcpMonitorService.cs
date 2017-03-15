using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Sample.TcpMonitor
{
    public partial class TcpMonitorService : ServiceBase
    {
        AsyncTcpListener _listener;
        public TcpMonitorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if (_listener != null)
                {
                    _listener.Stop();
                    _listener = null;
                }

                _listener = new AsyncTcpListener();
                _listener.Start();
            }
            catch (Exception ex)
            {
                Logger.WriteMessage(ex.Message);
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (_listener != null)
                {
                    _listener.Stop();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteMessage(ex.Message);
            }
        }
    }
}
