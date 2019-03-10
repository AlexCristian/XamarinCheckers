using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace XamarinCheckers
{
    class Connection
    {
        public Socket sock = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder message = new StringBuilder();
        public const string EndOfFileMarker = "<EOF>";
        public const string Handshake = "Checkers handshake." + EndOfFileMarker;
        XmlSerializer moveSerializer = new XmlSerializer(typeof(Move));

        public async Task<string> ReceiveMessage()
        {
            bool reachedEnd = false;
            message.Clear();

            while (!reachedEnd)
            {
                int bytesRec = sock.Receive(buffer);
                string data = Encoding.ASCII.GetString(buffer, 0, bytesRec);
                message.Append(data);
                if (data.IndexOf("<EOF>") > -1)
                {
                    reachedEnd = true;
                }
            }
            return message.ToString();
        }

        public async Task SendMessage(string message)
        {
            byte[] raw_message = Encoding.ASCII.GetBytes(message);
            sock.Send(raw_message);
        }

        public async Task SendMove(Move move)
        { 
            StringBuilder serializedMove = new StringBuilder();
            StringWriter moveWriter = new StringWriter(serializedMove);
            moveSerializer.Serialize(moveWriter, move);
            moveWriter.Close();
            await SendMessage(serializedMove.ToString() + EndOfFileMarker);
        }

        public async Task<Move> ListenForMove()
        {
            string data = await ReceiveMessage();
            byte[] byteArray = Encoding.ASCII.GetBytes(data);
            MemoryStream stream = new MemoryStream(byteArray);
            Move move = (Move) moveSerializer.Deserialize(stream);
            return move;
        }

        public async Task NotifyForfeit()
        {
            Move forfeit = new Move(true);
            await SendMove(forfeit);
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

        public async static Task<Connection> ListenForOpponent()
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
                    opponentConn = new Connection();
                    opponentConn.sock = listener.Accept();

                    string data = await opponentConn.ReceiveMessage();

                    // Establish Checkers protocol handshake.
                    if (!data.Equals(Connection.Handshake))
                    {
                        opponentConn = null;
                    }
                    else
                    {
                        Console.WriteLine("Established connection with remote client : {0}", opponentConn.sock.RemoteEndPoint.ToString());
                    }                    

                }
                await opponentConn.SendMessage(Connection.Handshake);
                return opponentConn;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public async static Task<Connection> ConnectWithOpponent(string OpponentIpAddress)
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
                await opponentConn.SendMessage(Connection.Handshake);
                string data = await opponentConn.ReceiveMessage();
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
