using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KompasNetworking;
using System.Net.Sockets;

public class ServerPlayer : Player
{
    public ServerGame serverGame;
    public ServerNetworkController ServerNetworkCtrl;
    public ServerNotifier ServerNotifier;

    public override void SetInfo(TcpClient tcpClient, int index)
    {
        base.SetInfo(tcpClient, index);

        ServerNetworkCtrl.SetInfo(tcpClient);
    }
}
