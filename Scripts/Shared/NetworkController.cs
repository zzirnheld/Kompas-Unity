using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkController : MonoBehaviour {

    protected const int BUFFER_SIZE = 1024;
    protected Vector3 absCardScale = new Vector3(1f / 9f, 1f / 9f, 1f / 9f);

    //flags
    public bool hosting = false;
    public bool connected = false;

    //IDs
    protected int channelID;
    protected int hostID;

    protected byte error;
    protected NetworkEventType recData;
    //holds the data for the buffer
    protected byte[] recBuffer = new byte[BUFFER_SIZE];
    protected int dataSize;
    protected string recievedMessage;
    protected string[] msgTokens;

    //buffer used for serialization
    protected byte[] reusableBuffer = new byte[BUFFER_SIZE];

    //prefabs
    public GameObject characterPrefab;
    public GameObject spellPrefab;
    public GameObject augmentPrefab;

    //When any controller is initialized, initiate Unity's transport layer API
    private void Awake() { NetworkTransport.Init(); }

    /// <summary>
    /// Opens your port to allow you to connect. 
    /// If you're hosting a server, no further action is needed.
    /// If you're connecting to a server, you still need to connect.
    /// </summary>
    /// <param name="socket"></param>
    protected void Host(int socket)
    {
        //then make a config to add whatever channels you wanna do
        ConnectionConfig config = new ConnectionConfig();
        channelID = config.AddChannel(QosType.Reliable);

        //then you create a topology, which defines how your networking is gonna work
        HostTopology topology = new HostTopology(config, 1);

        //adds a new host on port "socket" (currently 8888), with the defined topology, for any ip addresses to connect to
        hostID = NetworkTransport.AddHost(topology, socket);
        hosting = true;

        Debug.Log("Hosting on " + socket);
    }

    #region serialization
    protected byte[] Serialize(Packet packet)
    {
        //create a stream on top of the resuable buffer you have
        Stream stream = new MemoryStream(reusableBuffer);
        //the binary formatter serializes the packet into the stream (in the resuable buffer)
        new BinaryFormatter().Serialize(stream, packet);
        //once the stream has been filled with the serialized thing, flush it
        stream.Flush();
        //then create a new byte array only as large as the serialzied object is
        byte[] arr = new byte[stream.Position + 1];
        //close the stream now that you're done with it
        stream.Close();
        //copy the contents of the reusable buffer into your new array
        Array.Copy(reusableBuffer, arr, arr.Length);
        //return the new array
        return arr;
    }

    protected static Packet Deserialize(byte[] buffer)
    {
        //make sure the buffer isn't null for some dumb reason
        if (buffer == null) return null;
        //make sure that the buffer has nonzero length
        if (buffer.Length == 0) return null;
        //create a stream on top of the buffer passed in
        Stream stream = new MemoryStream(buffer);
        //and a binarry formatter to do the deserialziation
        BinaryFormatter formatter = new BinaryFormatter();
        //deserialize the contents of the buffer
        object o;
        try { o = formatter.Deserialize(stream); }
        catch(SerializationException e) { Debug.Log("Failed to deserialize"); throw; }
        //close the stream
        stream.Close();
        //if the object you deserialized is a packet, great! return it
        if (o is Packet) return o as Packet;
        //otherwise don't return anything
        return null;
    }
    #endregion serialization

    /// <summary>
    /// Sends the packet to the other computer.
    /// </summary>
    protected void Send(Packet packet, int connectionID)
    {
        if (!connected) return;
        byte[] arr = Serialize(packet);
        NetworkTransport.Send(hostID, connectionID, channelID, arr, arr.Length, out error);
    }

}
