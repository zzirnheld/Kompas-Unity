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
        public void SendPacket(Packet packet, TcpClient tcpClient)
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
        /// <summary> Reads count bytes from the stream from tcpClient </summary>
        private byte[] ReadBytes(TcpClient tcpClient, int count)
        {
            NetworkStream networkStream = tcpClient.GetStream();

            //this buffer will eventually be returned
            byte[] bytes = new byte[count];
            int readCount = 0;

            //loop until we've read "count" bytes
            while (readCount < count)
            {
                //want to read however many bytes are available, up to "count"
                int askFor = count - readCount;
                int numRead = networkStream.Read(bytes, readCount, askFor);

                //if read 0 bytes, connection has been lost
                if (numRead == 0)
                {
                    throw new System.IO.IOException("Lost Connection during read");
                }

                readCount += numRead; // advance by however many bytes we read
            }

            return bytes;
        }

        /// <summary> Reads the next message from the stream from tcpClient </summary>
        public Packet ReadPacket(TcpClient tcpClient)
        {
            //first get the length of the message to be read
            byte[] lengthBytes = ReadBytes(tcpClient, sizeof(int));
            //if this system is little-endian, reverse the bytes read
            if (System.BitConverter.IsLittleEndian) System.Array.Reverse(lengthBytes);
            // get length from bytes
            int length = System.BitConverter.ToInt32(lengthBytes, 0);
            //TODO a sanity check on length? is that a security risk? 

            // read the requested number of bytes
            byte[] messageBytes = ReadBytes(tcpClient, length);
            // get the packet that was sent
            return Deserialize(messageBytes);
        }
        #endregion reading
    }
}
