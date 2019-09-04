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
                gamesByConnectionID.Add(c, currentGameToMatchmake);
                currentGameToMatchmake.AddPlayer(c);
            }
            else
            {
                uiCtrl.CurrentStateString = "Two Players Connected";
                gamesByConnectionID.Add(c, currentGameToMatchmake);
                currentGameToMatchmake.AddPlayer(c);
                SendPackets(new Packet(Packet.Command.YoureFirst), new Packet(Packet.Command.YoureSecond), currentGameToMatchmake, mConnections[mConnections.Length - 1]);
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
    }
    
    public void SendPackets(Packet outPacket, Packet outPacketInverted, ServerGame serverGame, NetworkConnection connectionID)
    {
        if (outPacketInverted == null)
            Debug.Log("Sending only one packet with command " + outPacket.command);
        else if(outPacket.command != Packet.Command.Nothing)
            Debug.Log("Sending packets with commands " + outPacket.command + " and " + outPacketInverted.command);
        //if it's not null, send the normal packet to the player that queried you
        if (outPacket != null)
            Send(outPacket, mDriver, connectionID, mPipeline);


        //if the inverted packet isn't null, send the packet to the correct player
        if (outPacketInverted == null || !serverGame.HasPlayer2()) return;
        //if the one that queried you is player 0,
        if (connectionID == serverGame.Players[0].ConnectionID)
        {
            //send the inverted one to player 1.
            Send(outPacketInverted, mDriver, serverGame.Players[1].ConnectionID, mPipeline);
        }
        //if the one that queried you is player 1,
        else
        {
            //send the inverted one to player 0.
            Send(outPacketInverted, mDriver, serverGame.Players[0].ConnectionID, mPipeline);
        }

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
        //packet.InvertForController(playerIndex);
        //Debug.Log("packet command is " + packet.command + " for player index " + playerIndex);

        //switch between all the possible requests for the server to handle.
        switch (packet.command)
        {
            case Packet.Command.Nothing: //keeps the connection alive
                SendPackets(new Packet(Packet.Command.Nothing), new Packet(Packet.Command.Nothing), ServerGame.mainServerGame, connectionID);
                break;
            case Packet.Command.AddToDeck:
                AddCardToDeck(serverGame, playerIndex, packet.CardName, connectionID);
                break;
            case Packet.Command.Augment:
                Augment(serverGame, playerIndex, packet.cardID, packet.X, packet.Y, connectionID);
                break;
            case Packet.Command.Play: //TODO push to stack
                Play(serverGame, playerIndex, packet.cardID, packet.X, packet.Y, connectionID);
                break;
            case Packet.Command.Move:
                Move(serverGame, playerIndex, packet.cardID, packet.X, packet.Y, connectionID);
                break;
            case Packet.Command.Attack:
                Attack(serverGame, playerIndex, packet.cardID, packet.X, packet.Y, connectionID);
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
                SetXForEffect(serverGame, packet.EffectX);
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
                DebugTopdeck(serverGame, packet.cardID, connectionID);
                break;
            case Packet.Command.Discard:
                if (!uiCtrl.DebugMode) break;
                DebugDiscard(serverGame, packet.cardID, connectionID);
                break;
            case Packet.Command.Rehand:
                if (!uiCtrl.DebugMode) break;
                DebugRehand(serverGame, packet.cardID, connectionID);
                break;
            case Packet.Command.Draw:
                if (!uiCtrl.DebugMode) break;
                DebugDraw(serverGame, playerIndex, connectionID);
                break;
            case Packet.Command.SetNESW:
                if (!uiCtrl.DebugMode) break;
                DebugSetNESW(serverGame, packet.cardID, connectionID, packet.N, packet.E, packet.S, packet.W);
                break;
            case Packet.Command.SetPips:
                if (!uiCtrl.DebugMode) break;
                DebugSetPips(serverGame, playerIndex, connectionID, packet.Pips);
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

    public void AddCardToDeck(ServerGame sGame, int playerIndex, string cardName, NetworkConnection sourceID)
    {
        //figure out who's getting the card to their deck
        Player owner = sGame.Players[playerIndex];
        //add the card in, with the cardCount being the card id, then increment the card count
        Card added = owner.deckCtrl.AddCard(cardName, sGame.cardCount, playerIndex);
        sGame.cardCount++;
        //let everyone know
        Packet outPacket = new Packet(Packet.Command.AddAsFriendly, cardName, (int)Card.CardLocation.Deck, added.ID);
        Packet outPacketInverted = new Packet(Packet.Command.IncrementEnemyDeck);
        SendPackets(outPacket, outPacketInverted, sGame, sourceID);
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
    public void Augment(ServerGame sGame, int playerIndex, int cardID, int x, int y, NetworkConnection sourceID)
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
            if (toAugment.Location == Card.CardLocation.Discard || toAugment.Location == Card.CardLocation.Field)
                outPacketInverted = new Packet(Packet.Command.Augment, toAugment, x, y, true);
            else
                outPacketInverted = new Packet(Packet.Command.AddAsEnemy, toAugment.CardName, (int)Card.CardLocation.Field, toAugment.ID, x, y, true);

            //play the card here
            toAugment.Play(invertedX, invertedY, playerIndex);
        }
        else
        {
            outPacket = new Packet(Packet.Command.PutBack);
        }
        SendPackets(outPacket, outPacketInverted, sGame, sourceID);
    }

    public void NotifyPlay(ServerGame sGame, int controller, Card toPlay, int x, int y, NetworkConnection sourceID)
    {
        //tell everyone to do it
        Packet outPacket = new Packet(Packet.Command.Play, toPlay, x, y);
        Packet outPacketInverted;
        if (toPlay.Location == Card.CardLocation.Discard || toPlay.Location == Card.CardLocation.Field)
            outPacketInverted = new Packet(Packet.Command.Play, toPlay, x, y, true);
        else
            outPacketInverted = new Packet(Packet.Command.AddAsEnemy, toPlay.CardName, (int)Card.CardLocation.Field, toPlay.ID, x, y, true);

        outPacket.InvertForController(controller);
        outPacketInverted.InvertForController(controller);
        SendPackets(outPacket, outPacketInverted, sGame, sourceID);
    }

    public void Play(ServerGame sGame, int playerIndex, int cardID, int x, int y, NetworkConnection sourceID)
    {
        //get the card to play
        Card toPlay = sGame.GetCardFromID(cardID);
        int invertedX = InvertIndexForController(x, playerIndex);
        int invertedY = InvertIndexForController(y, playerIndex);
        //if it's not a valid place to do, return
        if (uiCtrl.DebugMode || sGame.ValidBoardPlay(toPlay, invertedX, invertedY))
        {
            NotifyPlay(sGame, playerIndex, toPlay, invertedX, invertedY, sourceID);
            //play the card here
            toPlay.Play(invertedX, invertedY, playerIndex);
        }
        else
        {
            Packet outPacket = new Packet(Packet.Command.PutBack);
            SendPackets(outPacket, null, sGame, sourceID);
        }
    }
    
    public void NotifyMove(ServerGame sGame, Card toMove, int x, int y, NetworkConnection sourceID)
    {
        //tell everyone to do it
        Packet outPacket = new Packet(Packet.Command.Move, toMove, x, y);
        Packet outPacketInverted = new Packet(Packet.Command.Move, toMove, x, y, true);
        SendPackets(outPacket, outPacketInverted, sGame, sourceID);
    }

    public void Move(ServerGame sGame, int playerIndex, int cardID, int x, int y, NetworkConnection sourceID)
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
            NotifyMove(sGame, toMove, x, y, sourceID);
        }
        else
        {
            Debug.Log("putback");
            Packet outPacket = new Packet(Packet.Command.PutBack);
            SendPackets(outPacket, null, sGame, sourceID);
        }
    }

    public void Attack(ServerGame sGame, int playerIndex, int cardID, int x, int y, NetworkConnection sourceID)
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
            SendPackets(outPacket, outPacketInverted, sGame, sourceID);
            //then resolve the attack
            //TODO allow for activation of abilities, fast cards
            CharacterCard attacker = toMove as CharacterCard;
            CharacterCard defender = sGame.boardCtrl.GetCharAt(invertedX, invertedY);
            attacker.Attack(defender);

            outPacket = new Packet(Packet.Command.SetNESW, attacker, attacker.N, attacker.E, attacker.S, attacker.W);
            outPacketInverted = new Packet(Packet.Command.SetNESW, attacker, attacker.N, attacker.E, attacker.S, attacker.W);
            SendPackets(outPacket, outPacketInverted, sGame, sourceID);
            outPacket = new Packet(Packet.Command.SetNESW, defender, defender.N, defender.E, defender.S, defender.W);
            outPacketInverted = new Packet(Packet.Command.SetNESW, defender, defender.N, defender.E, defender.S, defender.W);
            SendPackets(outPacket, outPacketInverted, sGame, sourceID);
        }
        else
        {
            Debug.Log("putback");
            outPacket = new Packet(Packet.Command.PutBack);
            SendPackets(outPacket, outPacketInverted, sGame, sourceID);
        }
    }

    public void NotifyTopdeck(ServerGame sGame, Card card, NetworkConnection sourceID)
    {
        Packet outPacket = null;
        Packet outPacketInverted = null;
        //and let everyone know
        outPacket = new Packet(Packet.Command.Topdeck, card);
        if (card.Location == Card.CardLocation.Hand || card.Location == Card.CardLocation.Deck)
            outPacketInverted = new Packet(Packet.Command.Delete, card);
        SendPackets(outPacket, outPacketInverted, sGame, sourceID);
    }

    public void DebugTopdeck(ServerGame sGame, int cardID, NetworkConnection sourceID)
    {
        Card toTopdeck = sGame.GetCardFromID(cardID);
        toTopdeck.Topdeck();
        NotifyTopdeck(sGame, toTopdeck, sourceID);
    }

    public void NotifyDiscard(ServerGame sGame, Card toDiscard, NetworkConnection sourceID)
    {
        Packet outPacket = null;
        Packet outPacketInverted = null;
        //and let everyone know
        outPacket = new Packet(Packet.Command.Discard, toDiscard);
        if (toDiscard.Location == Card.CardLocation.Discard || toDiscard.Location == Card.CardLocation.Field)
            outPacketInverted = new Packet(Packet.Command.Discard, toDiscard);
        else outPacketInverted = new Packet(Packet.Command.AddAsEnemy, toDiscard.CardName, (int)Card.CardLocation.Discard, toDiscard.ID);
        SendPackets(outPacket, outPacketInverted,sGame, sourceID);
    }

    public void DebugDiscard(ServerGame sGame, int cardID, NetworkConnection sourceID)
    {
        Card toDiscard = sGame.GetCardFromID(cardID);
        toDiscard.Discard();
        NotifyDiscard(sGame, toDiscard, sourceID);
    }

    public void NotifyRehand(ServerGame sGame, Card toRehand, NetworkConnection sourceID)
    {
        Packet outPacket = null;
        Packet outPacketInverted = null;
        //and let everyone know
        outPacket = new Packet(Packet.Command.Rehand, toRehand);
        if (toRehand.Location == Card.CardLocation.Hand || toRehand.Location == Card.CardLocation.Deck)
            outPacketInverted = new Packet(Packet.Command.Delete, toRehand);
        else outPacketInverted = null; //TODO make this add a blank card
                                       //eventually, this won't be necessary, because the player won't initiate this action
        SendPackets(outPacket, outPacketInverted, sGame, sourceID);
    }

    public void DebugRehand(ServerGame sGame, int cardID, NetworkConnection sourceID)
    {
        Card toRehand = sGame.GetCardFromID(cardID);
        toRehand.Rehand();
        NotifyRehand(sGame, toRehand, sourceID);
    }

    public void NotifyReshuffle(ServerGame sGame, Card toReshuffle, NetworkConnection sourceID)
    {
        Packet outPacket = null;
        Packet outPacketInverted = null;
        //and let everyone know
        outPacket = new Packet(Packet.Command.Reshuffle, toReshuffle);
        if (toReshuffle.Location == Card.CardLocation.Hand || toReshuffle.Location == Card.CardLocation.Deck)
            outPacketInverted = new Packet(Packet.Command.Delete, toReshuffle);
        SendPackets(outPacket, outPacketInverted, sGame, sourceID);
    }

    public void NotifyDraw(ServerGame sGame, Card toDraw, NetworkConnection sourceID)
    {
        //let everyone know to add that character to the correct hand
        Packet outPacket = new Packet(Packet.Command.Rehand, toDraw);
        //Packet outPacketInverted = new Packet(Packet.Command.Rehand, toDraw);
        Packet outPacketInverted = null;
        SendPackets(outPacket, outPacketInverted, sGame, sourceID);
    }

    public void DebugDraw(ServerGame sGame, int playerIndex, NetworkConnection connectionID)
    {
        //draw and store what was drawn
        Card toDraw = sGame.Draw(playerIndex);
        if (toDraw == null) return; //deck was empty
        NotifyDraw(sGame, toDraw, connectionID);
    }

    public void NotifySetPips(ServerGame sGame, int playerIndex, int pipsToSet, NetworkConnection sourceID)
    {
        //let everyone know
        Packet outPacket = new Packet(Packet.Command.SetPips, pipsToSet);
        Packet outPacketInverted = new Packet(Packet.Command.SetEnemyPips, pipsToSet);
        SendPackets(outPacket, outPacketInverted,sGame, sourceID);
    }

    public void DebugSetPips(ServerGame sGame, int playerIndex, NetworkConnection connectionID, int pipsToSet)
    {
        sGame.Players[playerIndex].pips = pipsToSet;
        if (playerIndex == 0) uiCtrl.UpdateFriendlyPips(pipsToSet);
        else uiCtrl.UpdateEnemyPips(pipsToSet);
        NotifySetPips(sGame, playerIndex, pipsToSet, connectionID);
    }

    public void SetTurn(ServerGame sGame, NetworkConnection connectionID, int indexToSet)
    {
        Packet outPacket = new Packet(Packet.Command.EndTurn, indexToSet);
        Packet outPacketInverted = new Packet(Packet.Command.EndTurn, indexToSet);
        SendPackets(outPacket, outPacketInverted, sGame, connectionID);
    }

    public void GetBoardTarget(ServerGame sGame, int playerIndex, Card effectSource, int effectIndex, int subeffectIndex)
    {
        Packet outPacket = new Packet(Packet.Command.RequestBoardTarget, effectSource, effectIndex, subeffectIndex);
        SendPackets(outPacket, null, sGame, sGame.Players[playerIndex].ConnectionID);
        Debug.Log("Asking for board target");
    }

    public void GetDeckTarget(ServerGame sGame, int playerIndex, Card effectSource, int effectIndex, int subeffectIndex)
    {
        Packet outPacket = new Packet(Packet.Command.RequestDeckTarget, effectSource, effectIndex, subeffectIndex);
        SendPackets(outPacket, null, sGame, sGame.Players[playerIndex].ConnectionID);
        Debug.Log("Asking for deck target");
        uiCtrl.CurrentStateString = "Asking for deck target";
    }

    public void GetDiscardTarget(ServerGame sGame, int playerIndex, Card effectSource, int effectIndex, int subeffectIndex)
    {
        Packet outPacket = new Packet(Packet.Command.RequestDiscardTarget, effectSource, effectIndex, subeffectIndex);
        SendPackets(outPacket, null, sGame, sGame.Players[playerIndex].ConnectionID);
        Debug.Log("Asking for discard target");
    }

    public void GetHandTarget(ServerGame sGame, int playerIndex, Card effectSource, int effectIndex, int subeffectIndex)
    {
        Packet outPacket = new Packet(Packet.Command.RequestHandTarget, effectSource, effectIndex, subeffectIndex);
        SendPackets(outPacket, null, sGame, sGame.Players[playerIndex].ConnectionID);
        Debug.Log("Asking for hand target");
    }

    public void GetSpaceTarget(ServerGame sGame, int playerIndex, Card effSrc, int effIndex, int subeffIndex)
    {
        Packet outPacket = new Packet(Packet.Command.SpaceTarget, effSrc, effIndex, subeffIndex);
        SendPackets(outPacket, null, sGame, sGame.Players[playerIndex].ConnectionID);
        Debug.Log("Asking for space target");
    }

    public void DebugSetNESW(ServerGame sGame, int cardID, NetworkConnection connectionID, int n, int e, int s, int w)
    {
        Card toSet = sGame.GetCardFromID(cardID);
        if(!(toSet is CharacterCard charToSet)) return;
        charToSet.SetNESW(n, e, s, w);
        NotifySetNESW(sGame, charToSet);
    }

    public void NotifySetNESW(ServerGame sGame, CharacterCard card)
    {
        if (card == null) return;
        //let everyone know to set NESW
        Packet outPacket = new Packet(Packet.Command.SetNESW, card, card.N, card.E, card.S, card.W);
        Packet outPacketInverted = new Packet(Packet.Command.SetNESW, card, card.N, card.E, card.S, card.W);
        SendPackets(outPacket, outPacketInverted, sGame, sGame.Players[card.ControllerIndex].ConnectionID);
    }

    public void RequestResponse(ServerGame sGame, int playerIndex)
    {
        Packet outPacket = new Packet(Packet.Command.Response);
    }

    /// <summary>
    /// Lets that player know their target has been accepted. called if the Target method returns True
    /// </summary>
    public void AcceptTarget(ServerGame sGame, Card target, NetworkConnection connectionID)
    {
        SendPackets(new Packet(Packet.Command.TargetAccepted, target), null, sGame, connectionID);
    }

    public void AcceptSpaceTarget(ServerGame sGame, int x, int y, NetworkConnection connectionID)
    {
        SendPackets(new Packet(Packet.Command.SpaceTargetAccepted, x, y), null, sGame, connectionID);
    }

    public void GetXForEffect(ServerGame sGame, int playerIndex, Card effSource, int effIndex, int subeffIndex)
    {
        Packet outPacket = new Packet(Packet.Command.PlayerSetX, effSource, effIndex, subeffIndex);
        SendPackets(outPacket, null, sGame, sGame.Players[playerIndex].ConnectionID);
        Debug.Log("Asking for X");
    }

    public void SetXForEffect(ServerGame sGame, int x)
    {
        //TODO sanitize
        sGame.CurrEffect.X = x;
        sGame.CurrEffect.ResolveNextSubeffect();
    }

    public void NotifyEffectX(ServerGame sGame, Card effSrc, int effIndex, int x)
    {
        //this puts the cardid in the right place, eff index in right place, x in packet.X
        Packet outPacket = new Packet(Packet.Command.SetEffectsX, effSrc, effIndex, 0, x, 0);
        SendPackets(outPacket, outPacket, sGame, effSrc.Controller.ConnectionID);
        Debug.Log("Sending value of X to client");
    }

    public void EnableDecliningTarget(ServerGame sGame, int playerIndex)
    {
        Packet packet = new Packet(Packet.Command.EnableDecliningTarget);
        Debug.Log("Enabling declining target");
        SendPackets(packet, null, sGame, sGame.Players[playerIndex].ConnectionID);
    }

    public void DisableDecliningTarget(ServerGame sGame, int playerIndex)
    {
        Packet packet = new Packet(Packet.Command.DisableDecliningTarget);
        Debug.Log("Disabling declining target");
        SendPackets(packet, null, sGame, sGame.Players[playerIndex].ConnectionID);
    }
}
