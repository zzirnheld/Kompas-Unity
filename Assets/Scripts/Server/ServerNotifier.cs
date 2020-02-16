using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using KompasNetworking;

public class ServerNotifier : MonoBehaviour
{
    public ServerNetworkController serverNetworkController;

    #region packet sending overloads
    public void SendPackets(Packet packetOne, Packet packetTwo, NetworkConnection connectionOne, NetworkConnection connectionTwo)
    {
        serverNetworkController.SendPackets(packetOne, packetTwo, connectionOne, connectionTwo);
    }

    public void SendPackets(Packet packet, NetworkConnection connection)
    {
        serverNetworkController.SendPackets(packet, null, connection, default);
    }

    public void SendPackets(Packet packetOne, Packet packetTwo, ServerGame sGame, int playerOne)
    {
        serverNetworkController.SendPackets(packetOne, packetTwo, 
            sGame.Players[playerOne].ConnectionID, 
            sGame.Players[1 - playerOne].ConnectionID);
    }

    public void SendPacketToBoth(Packet packet, ServerGame serverGame)
    {
        serverNetworkController.SendPackets(packet, packet, serverGame.Players[0].ConnectionID, serverGame.Players[1].ConnectionID);
    }

    public void SendPackets(Packet packetOne, Packet packetTwo, Player playerOne)
    {
        serverNetworkController.SendPackets(packetOne, packetTwo, playerOne.ConnectionID, playerOne.enemy.ConnectionID);
    }
    #endregion packet sending overloads

    #region notify
    public void NotifyPlay(Card toPlay, Player controller, int x, int y)
    {
        //tell everyone to do it
        Packet outPacket = new Packet(Packet.Command.Play, toPlay, x, y);
        Packet outPacketInverted;
        if (toPlay.Location == CardLocation.Discard || toPlay.Location == CardLocation.Field)
            outPacketInverted = new Packet(Packet.Command.Play, toPlay, x, y, true);
        else
            outPacketInverted = new Packet(Packet.Command.AddAsEnemy, toPlay.CardName, (int)CardLocation.Field, toPlay.ID, x, y, true);

        outPacket.InvertForController(controller.index);
        outPacketInverted.InvertForController(controller.index);
        SendPackets(outPacket, outPacketInverted, controller);
    }
    
    public void NotifyMove(Card toMove, int x, int y)
    {
        //tell everyone to do it
        Packet outPacket = new Packet(Packet.Command.Move, toMove, x, y);
        Packet outPacketInverted = new Packet(Packet.Command.Move, toMove, x, y, true);
        SendPackets(outPacket, outPacketInverted, toMove.Controller);
    }

    public void NotifyDiscard(Card toDiscard)
    {
        Packet outPacket = null;
        Packet outPacketInverted = null;
        //and let everyone know
        outPacket = new Packet(Packet.Command.Discard, toDiscard);
        if (toDiscard.Location == CardLocation.Discard || toDiscard.Location == CardLocation.Field)
            outPacketInverted = new Packet(Packet.Command.Discard, toDiscard);
        else outPacketInverted = new Packet(Packet.Command.AddAsEnemy, toDiscard.CardName, (int)CardLocation.Discard, toDiscard.ID);
        SendPackets(outPacket, outPacketInverted, toDiscard.Owner);
    }

    public void NotifyRehand(Card toRehand)
    {
        Packet outPacket = null;
        Packet outPacketInverted = null;
        //and let everyone know
        outPacket = new Packet(Packet.Command.Rehand, toRehand);
        if (toRehand.Location == CardLocation.Discard || toRehand.Location == CardLocation.Field)
            outPacketInverted = new Packet(Packet.Command.Delete, toRehand);
        else outPacketInverted = null; //TODO make this add a blank card
        SendPackets(outPacket, outPacketInverted, toRehand.Owner);
    }

    public void NotifyTopdeck(Card card)
    {
        Packet outPacket = null;
        Packet outPacketInverted = null;
        //and let everyone know
        outPacket = new Packet(Packet.Command.Topdeck, card);
        if (card.Location == CardLocation.Hand || card.Location == CardLocation.Deck)
            outPacketInverted = new Packet(Packet.Command.Delete, card);
        SendPackets(outPacket, outPacketInverted, card.Owner);
    }

    public void NotifyReshuffle(Card toReshuffle)
    {
        Packet outPacket = null;
        Packet outPacketInverted = null;
        //and let everyone know
        outPacket = new Packet(Packet.Command.Reshuffle, toReshuffle);
        if (toReshuffle.Location == CardLocation.Hand || toReshuffle.Location == CardLocation.Deck)
            outPacketInverted = new Packet(Packet.Command.Delete, toReshuffle);
        SendPackets(outPacket, outPacketInverted, toReshuffle.Owner);
    }

    public void NotifyDraw(Card toDraw)
    {
        //let everyone know to add that character to the correct hand
        Packet outPacket = new Packet(Packet.Command.Rehand, toDraw);
        SendPackets(outPacket, toDraw.Controller.ConnectionID);
    }

    public void NotifySetPips(Player toSetPips, int pipsToSet)
    {
        //let everyone know
        Packet outPacket = new Packet(Packet.Command.SetPips, pipsToSet);
        Packet outPacketInverted = new Packet(Packet.Command.SetEnemyPips, pipsToSet);
        SendPackets(outPacket, outPacketInverted, toSetPips);
    }

    public void NotifySetNESW(CharacterCard card)
    {
        if (card == null) return;
        //let everyone know to set NESW
        Packet outPacket = new Packet(Packet.Command.SetNESW, card, card.N, card.E, card.S, card.W);
        Packet outPacketInverted = new Packet(Packet.Command.SetNESW, card, card.N, card.E, card.S, card.W);
        SendPackets(outPacket, outPacketInverted, card.Controller);
    }

    public void NotifySetTurn(ServerGame sGame, int indexToSet)
    {
        Packet outPacket = new Packet(Packet.Command.EndTurn, indexToSet);
        Packet outPacketInverted = new Packet(Packet.Command.EndTurn, indexToSet);
        SendPacketToBoth(outPacket, sGame);
    }
    #endregion notify

    #region request targets

    public void GetBoardTarget(Player toGetTarget, Card effectSource, int effectIndex, int subeffectIndex)
    {
        Packet outPacket = new Packet(Packet.Command.RequestBoardTarget, effectSource, effectIndex, subeffectIndex);
        SendPackets(outPacket, toGetTarget.ConnectionID);
        Debug.Log("Asking for board target");
    }

    public void GetDeckTarget(Player toGetTarget, Card effectSource, int effectIndex, int subeffectIndex)
    {
        Packet outPacket = new Packet(Packet.Command.RequestDeckTarget, effectSource, effectIndex, subeffectIndex);
        SendPackets(outPacket, toGetTarget.ConnectionID);
        Debug.Log("Asking for deck target");
    }

    public void GetDiscardTarget(Player toGetTarget, Card effectSource, int effectIndex, int subeffectIndex)
    {
        Packet outPacket = new Packet(Packet.Command.RequestDiscardTarget, effectSource, effectIndex, subeffectIndex);
        SendPackets(outPacket, toGetTarget.ConnectionID);
        Debug.Log("Asking for discard target");
    }

    public void GetHandTarget(Player toGetTarget, Card effectSource, int effectIndex, int subeffectIndex)
    {
        Packet outPacket = new Packet(Packet.Command.RequestHandTarget, effectSource, effectIndex, subeffectIndex);
        SendPackets(outPacket, toGetTarget.ConnectionID);
        Debug.Log("Asking for hand target");
    }

    public void GetSpaceTarget(Player toGetTarget, Card effSrc, int effIndex, int subeffIndex)
    {
        Packet outPacket = new Packet(Packet.Command.SpaceTarget, effSrc, effIndex, subeffIndex);
        SendPackets(outPacket, toGetTarget.ConnectionID);
        Debug.Log("Asking for space target");
    }
    #endregion request targets

    #region other effect stuff
    public void RequestResponse(ServerGame sGame, int playerIndex)
    {
        Packet outPacket = new Packet(Packet.Command.Response);
    }

    /// <summary>
    /// Lets that player know their target has been accepted. called if the Target method returns True
    /// </summary>
    public void AcceptTarget(NetworkConnection connectionID)
    {
        SendPackets(new Packet(Packet.Command.TargetAccepted), connectionID);
    }

    public void GetXForEffect(Player toGetX, Card effSource, int effIndex, int subeffIndex)
    {
        Packet outPacket = new Packet(Packet.Command.PlayerSetX, effSource, effIndex, subeffIndex);
        SendPackets(outPacket, toGetX.ConnectionID);
    }

    public void SetXForEffect(ServerGame sGame, int x)
    {
        sGame.CurrEffect.X = x;
        sGame.CurrEffect.ResolveNextSubeffect();
    }

    public void NotifyEffectX(ServerGame sGame, Card effSrc, int effIndex, int x)
    {
        //this puts the cardid in the right place, eff index in right place, x in packet.X
        Packet outPacket = new Packet(Packet.Command.SetEffectsX, effSrc, effIndex, 0, x, 0);
        SendPacketToBoth(outPacket, sGame);
    }

    public void EnableDecliningTarget(Player toEnable)
    {
        Packet packet = new Packet(Packet.Command.EnableDecliningTarget);
        Debug.Log("Enabling declining target");
        SendPackets(packet, toEnable.ConnectionID);
    }

    public void DisableDecliningTarget(Player toDisable)
    {
        Packet packet = new Packet(Packet.Command.DisableDecliningTarget);
        Debug.Log("Disabling declining target");
        SendPackets(packet, toDisable.ConnectionID);
    }

    public void DiscardSimples(ServerGame inThisGame)
    {
        Packet packet = new Packet(Packet.Command.DiscardSimples);
        SendPacketToBoth(packet, inThisGame);
    }
    #endregion other effect stuff
}
