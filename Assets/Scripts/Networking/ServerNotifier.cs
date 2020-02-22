using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KompasNetworking;

public class ServerNotifier : MonoBehaviour
{
    public Player Player;
    public KompasNetworking.ServerNetworkController ServerNetworkCtrl;
    public ServerNotifier OtherNotifier;

    public void SendPacket(Packet packet)
    {
        ServerNetworkCtrl.SendPacket(packet);
    }

    private void SendPackets(Packet a, Packet b)
    {
        SendPacket(a);
        OtherNotifier.SendPacket(b);
    }

    private void SendPacketsAfterInverting(Packet a, Packet b)
    {
        a.InvertForController(Player.index);
        b.InvertForController(Player.index);
        SendPackets(a, b);
    }

    private void SendToBoth(Packet p)
    {
        SendPackets(p, p.Copy());
    }

    #region notify
    /// <summary>
    /// Notifies that the Player corresponding to this notifier played a given card
    /// </summary>
    public void NotifyPlay(Card toPlay, int x, int y)
    {
        //tell everyone to do it
        Packet outPacket = new Packet(Packet.Command.Play, toPlay, x, y);
        Packet outPacketInverted;
        if (toPlay.Location == CardLocation.Discard || toPlay.Location == CardLocation.Field)
        {
            outPacketInverted = new Packet(Packet.Command.Play, toPlay, x, y, true);
        }
        else
        {
            outPacketInverted = new Packet(Packet.Command.AddAsEnemy, toPlay.CardName, 
                (int)CardLocation.Field, toPlay.ID, x, y, true);
        }

        SendPacketsAfterInverting(outPacket, outPacketInverted);
    }
    
    public void NotifyMove(Card toMove, int x, int y)
    {
        //tell everyone to do it
        Packet outPacket = new Packet(Packet.Command.Move, toMove, x, y);
        Packet outPacketInverted = new Packet(Packet.Command.Move, toMove, x, y, true);
        SendPackets(outPacket, outPacketInverted);
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
        SendPackets(outPacket, outPacketInverted);
    }

    public void NotifyRehand(Card toRehand)
    {
        Packet outPacketInverted = null;
        //and let everyone know
        Packet outPacket = new Packet(Packet.Command.Rehand, toRehand);
        if (toRehand.Location == CardLocation.Discard || toRehand.Location == CardLocation.Field)
            outPacketInverted = new Packet(Packet.Command.Delete, toRehand);
        else outPacketInverted = null; //TODO make this add a blank card
        SendPackets(outPacket, outPacketInverted);
    }

    public void NotifyTopdeck(Card card)
    {
        Packet outPacketInverted = null;
        //and let everyone know
        Packet outPacket = new Packet(Packet.Command.Topdeck, card);
        if (card.Location == CardLocation.Hand || card.Location == CardLocation.Deck)
            outPacketInverted = new Packet(Packet.Command.Delete, card);
        SendPackets(outPacket, outPacketInverted);
    }

    public void NotifyReshuffle(Card toReshuffle)
    {
        Packet outPacketInverted = null;
        //and let everyone know
        Packet outPacket = new Packet(Packet.Command.Reshuffle, toReshuffle);
        if (toReshuffle.Location == CardLocation.Hand || toReshuffle.Location == CardLocation.Deck)
            outPacketInverted = new Packet(Packet.Command.Delete, toReshuffle);
        SendPackets(outPacket, outPacketInverted);
    }

    public void NotifyDraw(Card toDraw)
    {
        //I think it's equivalent?
        NotifyRehand(toDraw);
    }

    public void NotifySetPips(int pipsToSet)
    {
        //let everyone know
        Packet outPacket = new Packet(Packet.Command.SetPips, pipsToSet);
        Packet outPacketInverted = new Packet(Packet.Command.SetEnemyPips, pipsToSet);
        SendPackets(outPacket, outPacketInverted);
    }

    public void NotifySetNESW(CharacterCard card)
    {
        if (card == null) return;
        //let everyone know to set NESW
        Packet outPacket = new Packet(Packet.Command.SetNESW, card, card.N, card.E, card.S, card.W);
        Packet outPacketInverted = new Packet(Packet.Command.SetNESW, card, card.N, card.E, card.S, card.W);
        SendPackets(outPacket, outPacketInverted);
    }

    public void NotifySetTurn(ServerGame sGame, int indexToSet)
    {
        Packet outPacket = new Packet(Packet.Command.EndTurn, indexToSet);
        Packet outPacketInverted = new Packet(Packet.Command.EndTurn, indexToSet);
        SendToBoth(outPacket);
    }
    #endregion notify

    #region request targets

    public void GetBoardTarget(Card effectSource, int effectIndex, int subeffectIndex)
    {
        Packet outPacket = new Packet(Packet.Command.RequestBoardTarget, effectSource, effectIndex, subeffectIndex);
        SendPacket(outPacket);
        Debug.Log("Asking for board target");
    }

    public void GetDeckTarget(Card effectSource, int effectIndex, int subeffectIndex)
    {
        Packet outPacket = new Packet(Packet.Command.RequestDeckTarget, effectSource, effectIndex, subeffectIndex);
        SendPacket(outPacket);
        Debug.Log("Asking for deck target");
    }

    public void GetDiscardTarget(Card effectSource, int effectIndex, int subeffectIndex)
    {
        Packet outPacket = new Packet(Packet.Command.RequestDiscardTarget, effectSource, effectIndex, subeffectIndex);
        SendPacket(outPacket);
        Debug.Log("Asking for discard target");
    }

    public void GetHandTarget(Card effectSource, int effectIndex, int subeffectIndex)
    {
        Packet outPacket = new Packet(Packet.Command.RequestHandTarget, effectSource, effectIndex, subeffectIndex);
        SendPacket(outPacket);
        Debug.Log("Asking for hand target");
    }

    public void GetSpaceTarget(Card effSrc, int effIndex, int subeffIndex)
    {
        Packet outPacket = new Packet(Packet.Command.SpaceTarget, effSrc, effIndex, subeffIndex);
        SendPacket(outPacket);
        Debug.Log("Asking for space target");
    }
    #endregion request targets

    #region other effect stuff
    public void RequestResponse()
    {
        Packet outPacket = new Packet(Packet.Command.Response);
        SendPacket(outPacket);
    }

    /// <summary>
    /// Lets that player know their target has been accepted. called if the Target method returns True
    /// </summary>
    public void AcceptTarget()
    {
        Packet p = new Packet(Packet.Command.TargetAccepted);
        SendPacket(p);
    }

    public void GetXForEffect(Card effSource, int effIndex, int subeffIndex)
    {
        Packet outPacket = new Packet(Packet.Command.PlayerSetX, effSource, effIndex, subeffIndex);
        SendToBoth(outPacket);
    }

    public void NotifyEffectX(Card effSrc, int effIndex, int x)
    {
        //this puts the cardid in the right place, eff index in right place, x in packet.X
        Packet outPacket = new Packet(Packet.Command.SetEffectsX, effSrc, effIndex, 0, x, 0);
        SendToBoth(outPacket);
    }

    public void EnableDecliningTarget()
    {
        Packet packet = new Packet(Packet.Command.EnableDecliningTarget);
        Debug.Log("Enabling declining target");
        SendPacket(packet);
    }

    public void DisableDecliningTarget()
    {
        Packet packet = new Packet(Packet.Command.DisableDecliningTarget);
        Debug.Log("Disabling declining target");
        SendPacket(packet);
    }

    public void DiscardSimples()
    {
        Packet packet = new Packet(Packet.Command.DiscardSimples);
        SendToBoth(packet);
    }
    #endregion other effect stuff
}
