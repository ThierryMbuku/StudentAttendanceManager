using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                        var cardId = cardInfo[1].Replace(Environment.NewLine, string.Empty);
                        cardId = cardInfo[1].Replace("\r", string.Empty);
                        cardId = cardId.Replace(" ", string.Empty);

                        var cardType = -1;
                        if (cardId == "75FD17A6") //admin card id here
                        {
                            cardType = 1;
                        }
                        else
                        {
                            cardType = 2;
                        }
                        using (var data = new SAMEntities())
                        {
                            data.AccessCards.Add(new AccessCard()
                            {
                                CardType = (int)cardType,
                                CardId = cardId,
                                CreateDate = DateTime.Now,
                                SignedIn = false,
                                UserId = null
                            });
                            data.SaveChanges();
                            Console.WriteLine("Written card: " + cardId);
                        }
                    }
                }
            }
        }
    }
}
