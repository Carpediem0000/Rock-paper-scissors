using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    internal class Program
    {
        private static UdpClient _server;
        private static List<IPEndPoint> _clients = new List<IPEndPoint>();
        private static List<string> _choices = new List<string>();
        private static int _connectedClients = 0;
        private static int _rounds = 0;
        private const int MaxRounds = 5;

        static void Main(string[] args)
        {
            try
            {
                _server = new UdpClient(53333);
                Console.WriteLine("Server start listening...");

                while (_rounds < MaxRounds)
                {
                    if (_connectedClients < 2)
                    {
                        ReceiveChoice();
                    }

                    if (_choices.Count == 2)
                    {
                        SendChoices();
                        ResetRound();
                    }
                }

                Console.WriteLine("GameOver");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _server.Close();
            }

            Environment.Exit(0);
        }

        private static void ReceiveChoice()
        {
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] buffer = _server.Receive(ref clientEndPoint);
            string choice = Encoding.UTF8.GetString(buffer);
            //Console.WriteLine($"Получено от клиента {clientEndPoint}: {choice}");

            _clients.Add(clientEndPoint);
            _choices.Add(choice);
            _connectedClients++;
        }

        private static void SendChoices()
        {
            for (int i = 0; i < 2; i++)
            {
                string opponentChoice = _choices[1 - i];
                byte[] choiceBytes = Encoding.UTF8.GetBytes(opponentChoice);
                _server.Send(choiceBytes, choiceBytes.Length, _clients[i]);
            }
        }

        private static void ResetRound()
        {
            _choices.Clear();
            _clients.Clear();
            _connectedClients = 0;
            _rounds++;
        }
    }
}
