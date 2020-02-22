using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;


namespace KompasNetworking
{
    // got a lot of the basic code from https://gist.github.com/VisualMelon/9e1e8425b0e44012c79d932c2f1ca92b
    public class NetworkController : MonoBehaviour
    {
        public const int port = 8888;

        public Queue<Packet> Packets { get; private set; }

        private bool awaitingInt = true;
        private int numBytesToRead;
        private int numBytesRead;
        private byte[] bytesRead = new byte[sizeof(int)];
        private TcpClient tcpClient;

        private void Awake()
        {
            Packets = new Queue<Packet>();
        }

        public void SetInfo(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
        }

        #region serialization
        protected static byte[] Serialize(Packet packet)
        {
            string json = JsonUtility.ToJson(packet);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            return bytes;
        }

        protected static Packet Deserialize(byte[] bytes)
        {
            string json = Encoding.UTF8.GetString(bytes);
            try
            {
                Packet p = JsonUtility.FromJson<Packet>(json);
                return p;
            }
            catch (System.ArgumentException argEx)
            {
                //Catch JSON parse error
                Debug.LogError($"Failed to deserialize packet from json {json}, " +
                    $"argument exception with message {argEx.Message}");
                return null;
            }
        }
        #endregion serialization

        #region writing
        public void SendPacket(Packet packet)
        {
            NetworkStream networkStream = tcpClient.GetStream();
            // we won't use a binary writer, because the endianness is unhelpful

            // turn the string message into a byte[] (encode)
            byte[] messageBytes = Serialize(packet);
            // determine length of message
            int length = messageBytes.Length;

            // convert the length into bytes using BitConverter (encode)
            byte[] lengthBytes = System.BitConverter.GetBytes(length);

            // flip the bytes if we are a little-endian system: reverse the bytes in lengthBytes to do so
            if (System.BitConverter.IsLittleEndian) System.Array.Reverse(lengthBytes);

            // send length
            networkStream.Write(lengthBytes, 0, lengthBytes.Length);
            // send message
            networkStream.Write(messageBytes, 0, length);
        }
        #endregion writing

        #region reading
        private void ReadInt(NetworkStream networkStream)
        {
            numBytesRead += networkStream.Read(bytesRead, numBytesRead, sizeof(int) - numBytesRead);
            if (numBytesRead == sizeof(int))
            {
                //if this system is little-endian, reverse the bytes read
                if (System.BitConverter.IsLittleEndian) System.Array.Reverse(bytesRead);
                // get length from bytes
                numBytesToRead = System.BitConverter.ToInt32(bytesRead, 0);
                awaitingInt = false;
                bytesRead = new byte[numBytesToRead];
            }
            else if (numBytesRead == 0)
            {
                throw new System.IO.IOException("Lost Connection during read");
            }
        }

        private void ReadPacket(NetworkStream networkStream)
        {
            numBytesRead += networkStream.Read(bytesRead, numBytesRead, numBytesToRead - numBytesRead);
            if (numBytesRead == numBytesToRead)
            {
                Packet p = Deserialize(bytesRead);
                if(p != null) Packets.Enqueue(p);
                awaitingInt = true;
                bytesRead = new byte[sizeof(int)];
            }
            else if (numBytesRead == 0)
            {
                throw new System.IO.IOException("Lost Connection during read");
            }
        }

        public virtual void Update()
        {
            NetworkStream networkStream = tcpClient.GetStream();
            //if there's nothing to be read, return
            if (!tcpClient.GetStream().DataAvailable) return;

            if (awaitingInt) ReadInt(networkStream);
            else ReadPacket(networkStream);
        }
        #endregion reading
    }
}
