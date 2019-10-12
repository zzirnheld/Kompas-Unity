//using System.Collections;
//using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using Unity.Networking.Transport.LowLevel.Unsafe;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
using System.Collections.Generic;
//using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

public class ServerNetworkController : NetworkController {

    public GameObject ServerGamePrefab;
    public ServerGame currentGameToMatchmake;

    public ServerNotifier serverNotifier;

    public UIController uiCtrl;
    public MouseController mouseCtrl;

    public UdpNetworkDriver mDriver;
    public NetworkPipeline mPipeline;
    public NativeList<NetworkConnection> mConnections;
    private bool Hosting = false;

    private Dictionary<NetworkConnection, ServerGame> gamesByConnectionID;

    public void Host(ushort port)
    {
        currentGameToMatchmake = null;
        gamesByConnectionID = new Dictionary<NetworkConnection, ServerGame>();

        mDriver = new UdpNetworkDriver(new ReliableUtility.Parameters { WindowSize = 32 });
        mPipeline = mDriver.CreatePipeline(typeof(ReliableSequencedPipelineStage));

        var endpoint = new NetworkEndPoint();
        endpoint = NetworkEndPoint.Parse("0.0.0.0", port);
        if (mDriver.Bind(endpoint) != 0) Debug.Log("Failed to bind to port 8888");
        else { mDriver.Listen(); Debug.Log("listening"); }

        mConnections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        Hosting = true;
        uiCtrl.CurrentStateString = "Hosting";
    }

    public void HostIntPort(int port)
    {
        Host((ushort) port);
    }

    void OnDestroy()
    {
        mDriver.Dispose();
        mConnections.Dispose();
    }

    void Update()
    {
        if (!Hosting) return;
        mDriver.ScheduleUpdate().Complete();

        //remove dead connections
        for(int i = 0; i < mConnections.Length; i++)
        {
            if (!mConnections[i].IsCreated)
            {
                mConnections.RemoveAtSwapBack(i);
                i--;
            }
        }

        //add any and all new connections to accept
        NetworkConnection c;
        while((c = mDriver.Accept()) != default(NetworkConnection))
        {
            mConnections.Add(c);
            Debug.Log("Accepted connection");
            //add the player. if it's the second player, do i need to tell each player the other is here?
            if(currentGameToMatchmake == null)
            {
                uiCtrl.CurrentStateString = "One Player Connected";
                currentGameToMatchmake = GameObject.Instantiate(ServerGamePrefab).GetComponent<ServerGame>();
                currentGameToMatchmake.uiCtrl = uiCtrl;
                currentGameToMatchmake.mouseCtrl = mouseCtrl;
                currentGameToMatchmake.networkCtrl = this;
                currentGameToMatchmake.serverNetworkCtrl = this;
                currentGameToMatchmake.serverNotifier = serverNotifier;
                gamesByConnectionID.Add(c, currentGameToMatchmake);
                currentGameToMatchmake.AddPlayer(c);
            }
            else
            {
                uiCtrl.CurrentStateString = "Two Players Connected";
                gamesByConnectionID.Add(c, currentGameToMatchmake);
                currentGameToMatchmake.AddPlayer(c);
                SendPackets(new Packet(Packet.Command.YoureFirst), 
                    new Packet(Packet.Command.YoureSecond), 
                    currentGameToMatchmake.Players[0].ConnectionID, 
                    currentGameToMatchmake.Players[1].ConnectionID);
                currentGameToMatchmake = null;
            }
        }

        //query driver for events
        DataStreamReader reader;
        //loop through each connection
        for(int i = 0; i < mConnections.Length; i++)
        {
            if (i > 2) Debug.Log("WEE WOO " + mConnections.Length);
            //if the connection doesn't exist, skip to the next connection
            if (!mConnections[i].IsCreated) continue;

            NetworkEvent.Type cmd;
            //for each event for this connection (until you get an empty one), do stuff
            while((cmd = mDriver.PopEventForConnection(mConnections[i], out reader)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data) {
                    //Debug.Log("Recieved Data Event");
                    var readerCtxt = default(DataStreamReader.Context);
                    byte[] packetBuffer = reader.ReadBytesAsArray(ref readerCtxt, BUFFER_SIZE);
                    ParseRequest(packetBuffer, mConnections[i]);
                }
                else if(cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    uiCtrl.CurrentStateString = "Player " + i + " disconnected";
                    mConnections[i] = default(NetworkConnection); //default gets the default value of whatever type
                }
            }
        }

        //keep connection alive by sending packets to each connection
        timeChange += Time.deltaTime;
        if(timeChange >= 1f)
        {
            for(int i = 0; i < mConnections.Length; i++)
            {
                Send(new Packet(Packet.Command.Nothing), mDriver, mConnections[i], mPipeline);
            }
            timeChange = 0f;
        }
    }
    
    public void SendPackets(Packet outPacket, Packet outPacketInverted, NetworkConnection connectionID, NetworkConnection connectionIDInverted)
    {
        Debug.Log("Sending packets with commands " + outPacket?.command + " and " + outPacketInverted?.command);

        //if it's not null, send the normal packet to the player that queried you
        if (outPacket != null)
            Send(outPacket, mDriver, connectionID, mPipeline);
        if (outPacketInverted != null)
            Send(outPacketInverted, mDriver, connectionIDInverted, mPipeline);

    }

    //TODO: move all these to separate methods.
    //TODO make code that checks if ready to resolve the stack (both players have no responses/have declined priority in a row)
    private void ParseRequest(byte[] buffer, NetworkConnection connectionID)
    {
        //Debug.Log("recieved packet");
        Packet packet = Deserialize(buffer);
        if (packet == null) return;

        ServerGame serverGame = gamesByConnectionID[connectionID];
        int playerIndex = serverGame.GetPlayerIndexFromID(connectionID);
        Player playerFrom = serverGame.Players[playerIndex];
        //packet.InvertForController(playerIndex);
        if(packet.command != Packet.Command.Nothing) Debug.Log("packet command is " + packet.command + " for player index " + playerIndex);

        //switch between all the possible requests for the server to handle.
        switch (packet.command)
        {
            case Packet.Command.Nothing: 
                //do nothing, keeps the connection alive
                break;
            case Packet.Command.AddToDeck:
                AddCardToDeck(serverGame, playerIndex, packet.CardName);
                break;
            case Packet.Command.Augment:
                Augment(serverGame, playerIndex, packet.cardID, packet.X, packet.Y);
                break;
            case Packet.Command.Play: //TODO push to stack
                Play(serverGame, playerIndex, packet.cardID, packet.X, packet.Y);
                break;
            case Packet.Command.Move:
                Move(serverGame, playerIndex, packet.cardID, packet.X, packet.Y);
                break;
            case Packet.Command.Attack:
                Attack(serverGame, playerIndex, packet.cardID, packet.X, packet.Y);
                break;
            case Packet.Command.EndTurn:
                //TODO check to see if it was their turn bewfore swapping turns
                serverGame.SwitchTurn();
                break;
#region effect commands
            case Packet.Command.Target:
                if (serverGame.CurrEffect != null && serverGame.CurrEffect.CurrSubeffect is CardTargetSubeffect targetEff)
                {
                    targetEff.AddTargetIfLegal(serverGame.GetCardFromID(packet.cardID));
                }
                break;
            case Packet.Command.SpaceTarget:
                Debug.Log("Receieved space target " + packet.X + packet.Y);
                if (serverGame.CurrEffect != null && serverGame.CurrEffect.CurrSubeffect is SpaceTargetSubeffect spaceEff)
                {
                    packet.InvertForController(playerIndex);
                    spaceEff.SetTargetIfValid(packet.X, packet.Y);
                }
                else Debug.Log("curr effect null? " + (serverGame.CurrEffect == null) + " or not spacetgtsubeff? " + (serverGame.CurrEffect.CurrSubeffect is SpaceTargetSubeffect));
                break;
            case Packet.Command.PlayerSetX:
                serverNotifier.SetXForEffect(serverGame, packet.EffectX);
                break;
            case Packet.Command.DeclineAnotherTarget:
                if(serverGame.CurrEffect != null)
                {
                    serverGame.CurrEffect.DeclineAnotherTarget();
                }
                break;
#endregion
#region debug commands
            case Packet.Command.Topdeck:
                if (!uiCtrl.DebugMode) break;
                DebugTopdeck(serverGame, packet.cardID);
                break;
            case Packet.Command.Discard:
                if (!uiCtrl.DebugMode) break;
                DebugDiscard(serverGame, packet.cardID);
                break;
            case Packet.Command.Rehand:
                if (!uiCtrl.DebugMode) break;
                DebugRehand(serverGame, packet.cardID);
                break;
            case Packet.Command.Draw:
                if (!uiCtrl.DebugMode) break;
                DebugDraw(serverGame, playerIndex);
                break;
            case Packet.Command.SetNESW:
                if (!uiCtrl.DebugMode) break;
                DebugSetNESW(serverGame, packet.cardID, packet.N, packet.E, packet.S, packet.W);
                break;
            case Packet.Command.SetPips:
                if (!uiCtrl.DebugMode) break;
                DebugSetPips(serverGame.Players[playerIndex], packet.Pips);
                break;
            case Packet.Command.TestTargetEffect:
                Card whoseEffToTest = serverGame.GetCardFromID(packet.cardID);
                Debug.Log("Running eff of " + whoseEffToTest.CardName);
                serverGame.PushToStack(whoseEffToTest.Effects[0], playerIndex);
                serverGame.CheckForResponse();
                break;
#endregion
            default:
                Debug.Log("Invalid command " + packet.command + " to server from " + connectionID);
                break;
        }
    }

    public void AddCardToDeck(ServerGame sGame, int playerIndex, string cardName)
    {
        //figure out who's getting the card to their deck
        Player owner = sGame.Players[playerIndex];
        //add the card in, with the cardCount being the card id, then increment the card count
        Card added = owner.deckCtrl.AddCard(cardName, sGame.cardCount, playerIndex);
        sGame.cardCount++;
        foreach(Effect eff in added.Effects)
        {
            if (eff.Trigger != null)
            {
                Debug.Log("registering trigger for " + eff.Trigger.triggerCondition);
                sGame.RegisterTrigger(eff.Trigger.triggerCondition, eff.Trigger);
            }
            else Debug.Log("trigger is null");
        }
        //let everyone know
        Packet outPacket = new Packet(Packet.Command.AddAsFriendly, cardName, (int)CardLocation.Deck, added.ID);
        Packet outPacketInverted = new Packet(Packet.Command.IncrementEnemyDeck);
        SendPackets(outPacket, outPacketInverted, sGame.Players[playerIndex].ConnectionID, sGame.Players[1 - playerIndex].ConnectionID);
    }

    public static int InvertIndexForController(int index, int controller)
    {
        if (controller == 0) return index;
        else return 6 - index;
    }

    /// <summary>
    /// x and y here are from playerIndex's perspective
    /// </summary>
    /// <param name="sGame"></param>
    /// <param name="playerIndex"></param>
    /// <param name="cardID"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="sourceID"></param>
    public void Augment(ServerGame sGame, int playerIndex, int cardID, int x, int y)
    {
        Card toAugment = sGame.GetCardFromID(cardID);
        int invertedX = InvertIndexForController(x, playerIndex);
        int invertedY = InvertIndexForController(y, playerIndex);
        Packet outPacket = null;
        Packet outPacketInverted = null;
        //if it's not a valid place to do, return
        if (uiCtrl.DebugMode || sGame.ValidAugment(toAugment, invertedX, invertedY))
        {
            //tell everyone to do it
            outPacket = new Packet(Packet.Command.Augment, toAugment, x, y);
            if (toAugment.Location == CardLocation.Discard || toAugment.Location == CardLocation.Field)
                outPacketInverted = new Packet(Packet.Command.Augment, toAugment, x, y, true);
            else
                outPacketInverted = new Packet(Packet.Command.AddAsEnemy, toAugment.CardName, (int)CardLocation.Field, toAugment.ID, x, y, true);

            //play the card here
            toAugment.Play(invertedX, invertedY, playerIndex);
        }
        else
        {
            outPacket = new Packet(Packet.Command.PutBack);
        }
        SendPackets(outPacket, outPacketInverted, sGame.Players[playerIndex].ConnectionID, sGame.Players[1 - playerIndex].ConnectionID);
    }

    public void Play(ServerGame sGame, int playerIndex, int cardID, int x, int y)
    {
        //get the card to play
        Card toPlay = sGame.GetCardFromID(cardID);
        int invertedX = InvertIndexForController(x, playerIndex);
        int invertedY = InvertIndexForController(y, playerIndex);
        //if it's not a valid place to do, return
        if (uiCtrl.DebugMode || sGame.ValidBoardPlay(toPlay, invertedX, invertedY))
        {
            serverNotifier.NotifyPlay(toPlay, sGame.Players[playerIndex], invertedX, invertedY);
            //play the card here
            toPlay.Play(invertedX, invertedY, playerIndex);
            //trigger effects
            sGame.Trigger(TriggerCondition.Play, toPlay, null, null, null);
            sGame.CheckForResponse();
        }
        else
        {
            Packet outPacket = new Packet(Packet.Command.PutBack);
            SendPackets(outPacket, null, sGame.Players[playerIndex].ConnectionID, default);
        }
    }

    public void Move(ServerGame sGame, int playerIndex, int cardID, int x, int y)
    {
        //get the card to move
        Card toMove = sGame.GetCardFromID(cardID);
        int invertedX = InvertIndexForController(x, playerIndex);
        int invertedY = InvertIndexForController(y, playerIndex);
        //if it's not a valid place to do, return
        //NOTE: there is no debug to override moves because of how checking if attack works
        if (uiCtrl.DebugMode || sGame.ValidMove(toMove, invertedX, invertedY))
        {
            Debug.Log("move");
            //move the card there
            toMove.MoveOnBoard(invertedX, invertedY);
            serverNotifier.NotifyMove(toMove, x, y);
        }
        else
        {
            Debug.Log("putback");
            Packet outPacket = new Packet(Packet.Command.PutBack);
            SendPackets(outPacket, null, sGame.Players[playerIndex].ConnectionID, default);
        }
    }

    public void Attack(ServerGame sGame, int playerIndex, int cardID, int x, int y)
    {
        //get the card to move
        Card toMove = sGame.GetCardFromID(cardID);
        int invertedX = InvertIndexForController(x, playerIndex);
        int invertedY = InvertIndexForController(y, playerIndex);
        Packet outPacket = null;
        Packet outPacketInverted = null;
        if (uiCtrl.DebugMode || sGame.ValidAttack(toMove, invertedX, invertedY))
        {
            Debug.Log("attack");
            //tell the players to put cards down where they were
            outPacket = new Packet(Packet.Command.PutBack);
            outPacketInverted = new Packet(Packet.Command.PutBack);
            SendPackets(outPacket, outPacketInverted, sGame.Players[playerIndex].ConnectionID, sGame.Players[1 - playerIndex].ConnectionID);
            //then push the attack tothe stack
            CharacterCard attacker = toMove as CharacterCard;
            CharacterCard defender = sGame.boardCtrl.GetCharAt(invertedX, invertedY);
            //push the attack to the stack, then check if any player wants to respond before resolving it
            sGame.PushToStack(new Attack(sGame, attacker, defender), playerIndex);
            sGame.CheckForResponse();
        }
        else
        {
            Debug.Log("putback");
            outPacket = new Packet(Packet.Command.PutBack);
            SendPackets(outPacket, null, sGame.Players[playerIndex].ConnectionID, default);
        }
    }

    public void DebugTopdeck(ServerGame sGame, int cardID)
    {
        Card toTopdeck = sGame.GetCardFromID(cardID);
        serverNotifier.NotifyTopdeck(toTopdeck);
        toTopdeck.Topdeck();
    }

    public void DebugDiscard(ServerGame sGame, int cardID)
    {
        Card toDiscard = sGame.GetCardFromID(cardID);
        serverNotifier.NotifyDiscard(toDiscard);
        toDiscard.Discard();
        sGame.CheckForResponse();
    }

    public void DebugRehand(ServerGame sGame, int cardID)
    {
        Card toRehand = sGame.GetCardFromID(cardID);
        serverNotifier.NotifyRehand(toRehand);
        toRehand.Rehand();
    }

    public void DebugDraw(ServerGame sGame, int playerIndex)
    {
        //draw and store what was drawn
        Card toDraw = sGame.Draw(playerIndex);
        if (toDraw == null) return; //deck was empty
        serverNotifier.NotifyDraw(toDraw);
    }

    public void DebugSetPips(Player toSetPips, int pipsToSet)
    {
        toSetPips.pips = pipsToSet;
        if (toSetPips.index == 0) uiCtrl.UpdateFriendlyPips(pipsToSet);
        else uiCtrl.UpdateEnemyPips(pipsToSet);
        serverNotifier.NotifySetPips(toSetPips, pipsToSet);
    }

    public void DebugSetNESW(ServerGame sGame, int cardID, int n, int e, int s, int w)
    {
        Card toSet = sGame.GetCardFromID(cardID);
        if(!(toSet is CharacterCard charToSet)) return;
        charToSet.SetNESW(n, e, s, w);
        serverNotifier.NotifySetNESW(charToSet);
    }
}
