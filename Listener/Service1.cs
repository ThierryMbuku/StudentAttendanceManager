using System.ServiceProcess;
using System.IO.Ports;
using System.Web.Services.Description;
using SAM1;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System;

namespace Listener
{
    public partial class svcScannerListener : ServiceBase
    {
        SerialPort myport = new SerialPort();
        public svcScannerListener()
        {
            InitializeComponent();
            myport.BaudRate = 9600;
            myport.PortName = "COM3";

            myport.DataReceived += Myport_DataReceived;
        }

        private void Myport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;

            var bytes = new byte[sp.ReadBufferSize];
            //byte[] data = new byte[sp.BytesToRead];

             sp.Read(bytes, 0, bytes.Length);
            //sp.Read(data, 0, data.Length);

            var ascii = Encoding.ASCII.GetString(bytes);
            //var utf8 = Encoding.UTF8.GetString(bytes);

            File.WriteAllText(@"C:\log_ascii.txt", ascii);
            //File.WriteAllText(@"C:\log_utf8", utf8);

            //using (var fs = new FileStream(@"C:\log.log", FileMode.Create, FileAccess.Write))
            //{
            //    fs.Write(bytes, 0, bytes.Length);
            //    fs.Flush();
            //}
        }

        protected override void OnStart(string[] args)
        {
            myport.Open();
        }

        protected override void OnStop()
        {
            myport.Close();
         // Any files that have been created, delete them
        }
    }
}
