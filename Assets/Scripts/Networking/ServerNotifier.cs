using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KompasNetworking;

public class ServerNotifier : MonoBehaviour
{
    public ServerPlayer Player;
    public ServerNetworkController ServerNetworkCtrl;
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

    private void SendPacketsAfterInverting(Packet a, Packet b, int aIndex, int bIndex)
    {
        a.InvertForController(aIndex);
        b.InvertForController(bIndex);
        SendPackets(a, b);
    }

    private void SendToBoth(Packet p)
    {
        SendPackets(p, p.Copy());
    }

    #region game start
    public void GetDecklist()
    {
        Packet p = new Packet(Packet.Command.GetDeck);
        SendPacket(p);
    }

    public void YoureFirst()
    {
        Debug.Log("Sending you're first");
        Packet p = new Packet(Packet.Command.YoureFirst);
        SendPacket(p);
    }

    public void YoureSecond()
    {
        Debug.Log("Sending you're second");
        Packet p = new Packet(Packet.Command.YoureSecond);
        SendPacket(p);
    }
    #endregion game start

    #region notify
    public void NotifyPutBack()
    {
        Packet p = new Packet(Packet.Command.PutBack);
        SendPacket(p);
    }

    public void NotifyBothPutBack()
    {
        Packet p = new Packet(Packet.Command.PutBack);
        SendToBoth(p);
    }

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
            outPacketInverted = new Packet(Packet.Command.Play, toPlay, x, y);
        }
        else
        {
            outPacketInverted = new Packet(Packet.Command.AddAsEnemy, toPlay.CardName, 
                (int)CardLocation.Field, toPlay.ID, x, y);
        }

        SendPacketsAfterInverting(outPacket, outPacketInverted, Player.index, Player.Enemy.index);
    }
    
    public void NotifyMove(Card toMove, int x, int y, bool playerInitiated)
    {
        //tell everyone to do it
        Packet friendlyPacket = new Packet(Packet.Command.Move, toMove, x, y);
        Packet enemyPacket = new Packet(Packet.Command.Move, toMove, x, y);
        SendPacketsAfterInverting(friendlyPacket, enemyPacket, Player.index, Player.Enemy.index);
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

    public void NotifyBottomdeck(Card card)
    {
        Packet packet = new Packet(Packet.Command.Bottomdeck, card);
        Packet other = null;
        if (card.Location == CardLocation.Hand || card.Location == CardLocation.Deck)
            other = new Packet(Packet.Command.Delete, card);
        SendPackets(packet, other);
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

    public void NotifyAddToDeck(Card added)
    {
        //let everyone know
        Packet outPacket = new Packet(Packet.Command.AddAsFriendly, added.CardName, (int)CardLocation.Deck, added.ID);
        Packet outPacketInverted = new Packet(Packet.Command.IncrementEnemyDeck);
        SendPackets(outPacket, outPacketInverted);
    }

    public void NotifySetPips(int pipsToSet)
    {
        //let everyone know
        Packet outPacket = new Packet(Packet.Command.SetPips, pipsToSet);
        Packet outPacketInverted = new Packet(Packet.Command.SetEnemyPips, pipsToSet);
        SendPackets(outPacket, outPacketInverted);
    }

    public void NotifySetNESW(CharacterCard card, int n, int e, int s, int w)
    {
        if (card == null) return;
        //let everyone know to set NESW
        Packet p = new Packet(Packet.Command.SetNESW, card, n, e, s, w);
        SendToBoth(p);
    }

    public void NotifySetSpellStats(SpellCard spell, int c)
    {
        if (spell == null) return;
        Packet p = new Packet(Packet.Command.SetSpellStats, spell, c);
        SendToBoth(p);
    }

    public void NotifySetNegated(Card card, bool negated)
    {
        if (card == null) return;
        Packet packet = new Packet(Packet.Command.Negate, card, negated);
        SendToBoth(packet);
    }

    public void NotifyActivate(Card card, bool activated)
    {
        if (card == null) throw new System.ArgumentNullException($"Card must not be null to notify about activating");
        Packet packet = new Packet(Packet.Command.Activate, card, activated);
        SendToBoth(packet);
    }

    public void NotifySetTurn(ServerGame sGame, int indexToSet)
    {
        Packet outPacket = new Packet(Packet.Command.EndTurn, indexToSet);
        Packet outPacketInverted = new Packet(Packet.Command.EndTurn, indexToSet);
        SendToBoth(outPacket);
    }
    #endregion notify

    #region request targets
    public void GetBoardTarget(Card source, BoardTargetSubeffect boardTargetSubeffect)
    {
        Packet outPacket = new Packet(Packet.Command.RequestBoardTarget, source, boardTargetSubeffect);
        SendPacket(outPacket);
        Debug.Log("Asking for board target");
    }

    public void GetDeckTarget(Card source, CardTargetSubeffect cardTargetSubeffect)
    {
        Packet outPacket = new Packet(Packet.Command.RequestDeckTarget, source, cardTargetSubeffect);
        SendPacket(outPacket);
        Debug.Log("Asking for deck target");
    }

    public void GetDiscardTarget(Card source, CardTargetSubeffect cardTargetSubeffect)
    {
        Packet outPacket = new Packet(Packet.Command.RequestDiscardTarget, source, cardTargetSubeffect);
        SendPacket(outPacket);
        Debug.Log("Asking for discard target");
    }

    public void GetHandTarget(Card source, CardTargetSubeffect cardTargetSubeffect)
    {
        Packet outPacket = new Packet(Packet.Command.RequestHandTarget, source, cardTargetSubeffect);
        SendPacket(outPacket);
        Debug.Log("Asking for hand target");
    }

    public void GetSpaceTarget(Card effSrc, SpaceTargetSubeffect spaceTargetSubeffect)
    {
        Packet outPacket = new Packet(Packet.Command.SpaceTarget, effSrc, spaceTargetSubeffect);
        SendPacket(outPacket);
        Debug.Log("Asking for space target");
    }

    public void GetChoicesFromList(List<Card> potentialTargets, int maxNum, ChooseFromListSubeffect src)
    {
        int[] cardIDs = new int[potentialTargets.Count];
        for(int i = 0; i < potentialTargets.Count; i++)
        {
            cardIDs[i] = potentialTargets[i].ID;
        }
        Packet packet = new Packet(Packet.Command.GetChoicesFromList, src.ThisCard, cardIDs, maxNum, src.ServerEffect.EffectIndex, src.SubeffIndex);
        SendPacket(packet);
        Debug.Log($"Asking for targets from list of cardIDs {string.Join(",", cardIDs)}");
    }

    public void ChooseEffectOption(ChooseOptionSubeffect src)
    {
        Packet packet = new Packet(Packet.Command.ChooseEffectOption, src.ThisCard, src.ServerEffect.EffectIndex, src.SubeffIndex);
        SendPacket(packet);
    }
    #endregion request targets

    #region other effect stuff
    public void EffectResolving(ServerEffect eff)
    {
        Packet p = new Packet(Packet.Command.EffectResolving, eff.thisCard, eff.EffectIndex, 
            eff.Controller == Player ? 0 : 1, 0, 0);
        Packet q = new Packet(Packet.Command.EffectResolving, eff.thisCard, eff.EffectIndex,
            eff.Controller == Player ? 1 : 0, 0, 0);
        SendPackets(p, q);
    }

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
        SendPacket(outPacket);
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

    public void AskForTrigger(ServerTrigger t, int? x, Card cardTriggerer, IServerStackable stackTriggerer, ServerPlayer triggerer)
    {
        Card cardWhoseTrigger = t.effToTrigger.thisCard;
        int effIndex = t.effToTrigger.EffectIndex;
        //TODO send info about triggerer to display on client
        Packet packet = new Packet(Packet.Command.OptionalTrigger, cardWhoseTrigger, effIndex, 0, x ?? 0, 0);
        SendPacket(packet);
    }
    #endregion other effect stuff
}
