using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace KompasCore.Networking
{
    public abstract class NetworkController : MonoBehaviour
    {
        public const int port = 8448;
        public const int bytesArrayLen = 256;
        public const char endOfTransmission = '\u0004';

        public readonly Queue<(string, string)> packets = new Queue<(string, string)>();

        protected bool awaitingInt = true;
        protected Socket socket;

        private string currPacketStr;

        public void SetInfo(Socket socket)
        {
            this.socket = socket;
        }

        public abstract Task ProcessPacket((string command, string json) packetInfo);

        protected virtual void Update()
        {
            if (socket == null || !socket.Connected) return;

            string str = ReadFromSocket(socket);
            if (str.IndexOf(endOfTransmission) > -1)
            {
                var strs = str.Split(endOfTransmission);
                int numPackets = strs.Length;
                int i = 0;
                foreach (var s in strs)
                {
                    currPacketStr += s;
                    if (i < numPackets)
                    {
                        Packet p = JsonUtility.FromJson<Packet>(currPacketStr);
                        packets.Enqueue((p.command, currPacketStr));
                        currPacketStr = "";
                    }
                }
            }
            else currPacketStr += str; 
        }

        #region serialization
        protected static byte[] Serialize(Packet packet)
        {
            string json = JsonUtility.ToJson(packet);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            return bytes;
        }

        protected static (string, string) Deserialize(byte[] bytes)
        {
            string json = Encoding.UTF8.GetString(bytes);
            try
            {
                Packet p = JsonUtility.FromJson<Packet>(json);
                return (p.command, json);
            }
            catch (System.ArgumentException argEx)
            {
                //Catch JSON parse error
                Debug.LogError($"Failed to deserialize packet from json \"{json}\", " +
                    $"argument exception with message {argEx.Message}");
                return (Packet.Invalid, null);
            }
        }
        #endregion serialization

        #region writing
        public void SendPacket(Packet packet)
        {
            if (packet == null) return;

            socket.Send(Serialize(packet));
        }
        #endregion writing

        #region reading
        private string ReadFromSocket(Socket socket)
        {
            byte[] bytes = new byte[bytesArrayLen];
            int numReceived;
            string str = "";
            while (socket.Available > 0)
            {
                numReceived = socket.Receive(bytes, bytesArrayLen, SocketFlags.None);
                str += Encoding.UTF8.GetString(bytes, 0, numReceived);
            }
            return str;
        }
        #endregion reading
    }
}
