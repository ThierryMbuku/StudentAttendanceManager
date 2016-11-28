using System;
using System.IO;
using System.IO.Ports;
using System.Linq;

namespace TemporaryListener
{
    class Program
    {
        static void Main(string[] args)
        {
            //Start the listener on localhost:portNumber
            SerialPort myport = new SerialPort();
            myport.BaudRate = 9600;
            myport.PortName = "COM3";
            myport.Open();

            while (true)
            {
                string condition = myport.ReadLine();
                Console.WriteLine(condition);
                if (condition.Contains("Card UID"))
                {
                    var cardInfo = condition.Split(':');
                    if (cardInfo.Any())
                    {
                        var file = new StreamWriter("c:\\test.txt");
                        file.WriteLine(cardInfo[1].Replace(" ", ""));
                        file.Close();
                    }
                }
            }
        }
    }
}
