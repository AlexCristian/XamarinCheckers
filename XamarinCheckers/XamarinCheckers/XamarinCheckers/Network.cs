using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace XamarinCheckers
{
    public class Connection
    {
        public Socket sock = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder message = new StringBuilder();
        public const string EndOfFileMarker = "<EOF>";
        public const string Handshake = "Checkers handshake." + EndOfFileMarker;
        XmlSerializer moveSerializer = new XmlSerializer(typeof(Move));

        public string ReceiveMessage()
        {
            bool reachedEnd = false;
            message.Clear();

            while (!reachedEnd)
            {
                int bytesRec = sock.Receive(buffer);
                string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);
                message.Append(data);
                if (data.IndexOf("<EOF>") > -1)
                {
                    reachedEnd = true;
                }
            }
            return message.ToString();
        }

        public void SendMessage(string message)
        {
            byte[] raw_message = Encoding.UTF8.GetBytes(message);
            sock.Send(raw_message);
        }

        public void SendMove(Move move)
        { 
            StringBuilder serializedMove = new StringBuilder();
            StringWriter moveWriter = new StringWriter(serializedMove);
            moveSerializer.Serialize(moveWriter, move);
            moveWriter.Close();
            SendMessage(serializedMove.ToString() + EndOfFileMarker);
        }

        public Move ListenForMove()
        {
            string data = ReceiveMessage();
            data = data.Remove(data.LastIndexOf(EndOfFileMarker));
            XmlReader reader = XmlReader.Create(new StringReader(data));
            Move move = (Move) moveSerializer.Deserialize(reader);
            return move;
        }

        public void NotifyForfeit()
        {
            Move forfeit = new Move(true);
            SendMove(forfeit);
        }

        public void CloseConnection()
        {
            sock.Close();
        }

    }
    class Network
    {
        public Network()
        {
        }

        public static string GetDeviceIPAddress()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            return ipHostInfo.AddressList[0].ToString();
        }

        public static Connection ListenForOpponent()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            Connection opponentConn = null;

            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (opponentConn == null)
                {
                    Console.WriteLine("Waiting for a connection...");
                    opponentConn = new Connection
                    {
                        sock = listener.Accept()
                    };

                    string data = opponentConn.ReceiveMessage();

                    // Establish Checkers protocol handshake.
                    if (!data.Equals(Connection.Handshake))
                    {
                        opponentConn = null;
                    }
                    else
                    {
                        //Console.WriteLine("Established connection with remote client : {0}", opponentConn.sock.RemoteEndPoint.ToString());
                    }                    

                }
                opponentConn.SendMessage(Connection.Handshake);
                return opponentConn;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public static Connection ConnectWithOpponent(string OpponentIpAddress)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(OpponentIpAddress);
                IPEndPoint opponent = new IPEndPoint(ipAddress, 11000);

                Connection opponentConn = new Connection();
                opponentConn.sock = new Socket(ipAddress.AddressFamily,
                 SocketType.Stream, ProtocolType.Tcp);
                opponentConn.sock.Connect(opponent);

                // Establish Checkers protocol handshake.
                opponentConn.SendMessage(Connection.Handshake);
                string data = opponentConn.ReceiveMessage();
                if (!data.Equals(Connection.Handshake))
                {
                    opponentConn = null;
                }
                else
                {
                    Console.WriteLine("Established connection with remote client : {0}", opponentConn.sock.RemoteEndPoint.ToString());
                }
                return opponentConn;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

    }
}
