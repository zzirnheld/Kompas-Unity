using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KompasNetworking;
using System.Linq;

public class ServerNotifier : MonoBehaviour
{
    public ServerPlayer Player;
    public ServerNetworkController ServerNetworkCtrl;
    public ServerNotifier OtherNotifier;

    public void SendPacket(Packet packet)
    {
        if (packet != null) Debug.Log($"Sending packet to {Player.index} with command {packet.command}, normal args {string.Join(",", packet.normalArgs)}, " +
            $"special args {string.Join(",", packet.specialArgs)}, string arg {packet.stringArg}");
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
        b?.InvertForController(bIndex);
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

    public void DeckAccepted()
    {
        Packet p = new Packet(Packet.Command.DeckAccepted);
        SendPacket(p);
    }

    public void SetFriendlyAvatar(string cardName, int cardID)
    {
        Packet p = new Packet(Packet.Command.SetFriendlyAvatar, cardName) { cardID = cardID };
        Packet q = new Packet(Packet.Command.SetEnemyAvatar, cardName) { cardID = cardID };
        SendPackets(p, q);
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

    public void NotifyAttach(GameCard toAttach, int x, int y)
    {
        Packet p = new Packet(Packet.Command.Attach, toAttach, x, y);
        Packet q = toAttach.Location == CardLocation.Discard || toAttach.Location == CardLocation.Field ?
            new Packet(Packet.Command.Attach, toAttach, x, y) : 
            new Packet(Packet.Command.AddAsEnemyAndAttach, toAttach.CardName, (int)CardLocation.Field, toAttach.ID, x, y);
        SendPacketsAfterInverting(p, q, Player.index, Player.Enemy.index);
    }

    /// <summary>
    /// Notifies that the Player corresponding to this notifier played a given card
    /// </summary>
    public void NotifyPlay(GameCard toPlay, int x, int y)
    {
        //if this card is an augment, don't bother notifying about it. attach will take care of it.
        if (toPlay.CardType == 'A') return;

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

    public void NotifyMove(GameCard toMove, int x, int y, bool playerInitiated)
    {
        //tell everyone to do it
        Packet friendlyPacket = new Packet(Packet.Command.Move, toMove, x, y);
        Packet enemyPacket = new Packet(Packet.Command.Move, toMove, x, y);
        SendPacketsAfterInverting(friendlyPacket, enemyPacket, Player.index, Player.Enemy.index);
    }

    public void NotifyDiscard(GameCard toDiscard)
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

    public void NotifyRehand(GameCard toRehand)
    {
        Packet outPacketInverted = null;
        //and let everyone know
        Packet outPacket = new Packet(Packet.Command.Rehand, toRehand);
        if (toRehand.Location == CardLocation.Discard || toRehand.Location == CardLocation.Field)
            outPacketInverted = new Packet(Packet.Command.Delete, toRehand);
        else outPacketInverted = null; //TODO make this add a blank card
        SendPackets(outPacket, outPacketInverted);
    }

    public void NotifyAnnhilate(GameCard toAnnhilate)
    {
        var p = new Packet(Packet.Command.Annihilate, toAnnhilate);
        var q = toAnnhilate.Location == CardLocation.Discard || toAnnhilate.Location == CardLocation.Field ?
            new Packet(Packet.Command.Annihilate, toAnnhilate) :
        new Packet(Packet.Command.AddAsEnemy, toAnnhilate.CardName, (int)CardLocation.Annihilation, toAnnhilate.ID);
        SendPackets(p, q);
    }

    public void NotifyTopdeck(GameCard card)
    {
        Packet outPacketInverted = null;
        //and let everyone know
        Packet outPacket = new Packet(Packet.Command.Topdeck, card);
        if (card.Location == CardLocation.Hand || card.Location == CardLocation.Deck)
            outPacketInverted = new Packet(Packet.Command.Delete, card);
        SendPackets(outPacket, outPacketInverted);
    }

    public void NotifyBottomdeck(GameCard card)
    {
        Packet packet = new Packet(Packet.Command.Bottomdeck, card);
        Packet other = null;
        if (card.Location == CardLocation.Hand || card.Location == CardLocation.Deck)
            other = new Packet(Packet.Command.Delete, card);
        SendPackets(packet, other);
    }

    public void NotifyReshuffle(GameCard toReshuffle)
    {
        Packet outPacketInverted = null;
        //and let everyone know
        Packet outPacket = new Packet(Packet.Command.Reshuffle, toReshuffle);
        if (toReshuffle.Location == CardLocation.Hand || toReshuffle.Location == CardLocation.Deck)
            outPacketInverted = new Packet(Packet.Command.Delete, toReshuffle);
        SendPackets(outPacket, outPacketInverted);
    }

    public void NotifyDraw(GameCard toDraw)
    {
        //I think it's equivalent?
        NotifyRehand(toDraw);
    }

    public void NotifyAddToDeck(GameCard added)
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

    public void NotifySetNESW(ServerGameCard card, int n, int e, int s, int w)
    {
        if (card == null) throw new System.ArgumentNullException($"Char must not be null to notify about setting stats");
        //let everyone know to set NESW
        Packet p = new Packet(Packet.Command.SetNESW, card, n, e, s, w);
        SendToBoth(p);
    }

    public void NotifySetN(GameCard card)
    {
        Packet p = new Packet(Packet.Command.SetN, card, card.N);
        SendToBoth(p);
    }

    public void NotifySetE(GameCard card)
    {
        Packet p = new Packet(Packet.Command.SetE, card, card.E);
        SendToBoth(p);
    }

    public void NotifySetS(GameCard card)
    {
        Packet p = new Packet(Packet.Command.SetS, card, card.S);
        SendToBoth(p);
    }

    public void NotifySetW(GameCard card)
    {
        Packet p = new Packet(Packet.Command.SetW, card, card.W);
        SendToBoth(p);
    }

    public void NotifySetC(GameCard card)
    {
        Packet p = new Packet(Packet.Command.SetC, card, card.C);
        SendToBoth(p);
    }

    public void NotifySetA(GameCard card)
    {
        Packet p = new Packet(Packet.Command.SetA, card, card.A);
        SendToBoth(p);
    }

    public void NotifySetSpellStats(ServerGameCard spell, int c)
    {
        if (spell == null) throw new System.ArgumentNullException($"Spell must not be null to notify about setting stats");
        Packet p = new Packet(Packet.Command.SetSpellStats, spell, c);
        SendToBoth(p);
    }

    public void NotifySetNegated(GameCard card, bool negated)
    {
        if (card == null) throw new System.ArgumentNullException($"Card must not be null to notify about negating");
        Packet packet = new Packet(Packet.Command.Negate, card, negated);
        SendToBoth(packet);
    }

    public void NotifyActivate(GameCard card, bool activated)
    {
        if (card == null) throw new System.ArgumentNullException($"Card must not be null to notify about activating");
        Packet packet = new Packet(Packet.Command.Activate, card, activated);
        SendToBoth(packet);
    }

    public void NotifyChangeController(GameCard card, Player controller)
    {
        if (card == null) throw new System.ArgumentNullException($"Card must not be null to notify about changing controller");
        if (controller == null) throw new System.ArgumentNullException($"Player must not be null to notify about changing controller");
        Packet p = new Packet(Packet.Command.ChangeControl, card, controller == Player ? 0 : 1);
        Packet q = new Packet(Packet.Command.ChangeControl, card, controller == Player ? 1 : 0);
        SendPackets(p, q);
    }

    public void NotifySetTurn(ServerGame sGame, int indexToSet)
    {
        Packet outPacket = new Packet(Packet.Command.EndTurn, indexToSet);
        Packet outPacketInverted = new Packet(Packet.Command.EndTurn, indexToSet);
        SendToBoth(outPacket);
    }
    #endregion notify

    #region request targets
    public void GetBoardTarget(GameCard source, BoardTargetSubeffect boardTargetSubeffect)
    {
        Packet outPacket = new Packet(Packet.Command.RequestBoardTarget, source, boardTargetSubeffect);
        SendPacket(outPacket);
        Debug.Log("Asking for board target");
    }

    public void GetDeckTarget(GameCard source, CardTargetSubeffect cardTargetSubeffect)
    {
        Packet outPacket = new Packet(Packet.Command.RequestDeckTarget, source, cardTargetSubeffect);
        SendPacket(outPacket);
        Debug.Log("Asking for deck target");
    }

    public void GetDiscardTarget(GameCard source, CardTargetSubeffect cardTargetSubeffect)
    {
        Packet outPacket = new Packet(Packet.Command.RequestDiscardTarget, source, cardTargetSubeffect);
        SendPacket(outPacket);
        Debug.Log("Asking for discard target");
    }

    public void GetHandTarget(GameCard source, CardTargetSubeffect cardTargetSubeffect)
    {
        Packet outPacket = new Packet(Packet.Command.RequestHandTarget, source, cardTargetSubeffect);
        SendPacket(outPacket);
        Debug.Log("Asking for hand target");
    }

    public void GetSpaceTarget(GameCard effSrc, SpaceTargetSubeffect spaceTargetSubeffect)
    {
        Packet outPacket = new Packet(Packet.Command.SpaceTarget, effSrc, spaceTargetSubeffect);
        SendPacket(outPacket);
        Debug.Log("Asking for space target");
    }

    public void GetChoicesFromList(IEnumerable<GameCard> potentialTargets, int maxNum, ChooseFromListSubeffect src)
    {
        int[] cardIDs = new int[potentialTargets.Count()];
        int i = 0;
        foreach(GameCard c in potentialTargets) cardIDs[i++] = c.ID;
        Packet packet = new Packet(Packet.Command.GetChoicesFromList, src.ThisCard, cardIDs, src.ServerEffect.EffectIndex, src.SubeffIndex, maxNum);
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
        Packet p = new Packet(Packet.Command.EffectResolving, eff.Source, eff.EffectIndex, 
            eff.Controller == Player ? 0 : 1, 0, 0);
        Packet q = new Packet(Packet.Command.EffectResolving, eff.Source, eff.EffectIndex,
            eff.Controller == Player ? 1 : 0, 0, 0);
        SendPackets(p, q);
    }

    public void EffectImpossible()
    {
        Packet p = new Packet(Packet.Command.EffectImpossible);
        SendToBoth(p);
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
        Debug.Log($"Accepting target of {Player.index}");
        Packet p = new Packet(Packet.Command.TargetAccepted);
        SendPacket(p);
    }

    public void SetTarget(GameCard card, int effIndex, GameCard target)
    {
        Packet p = new Packet(Packet.Command.Target, card, effIndex, target.ID);
        SendPacket(p);
    }

    public void GetXForEffect(GameCard effSource, int effIndex, int subeffIndex)
    {
        Packet outPacket = new Packet(Packet.Command.PlayerSetX, effSource, effIndex, subeffIndex);
        SendPacket(outPacket);
    }

    public void NotifyEffectX(GameCard effSrc, int effIndex, int x)
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

    public void AskForTrigger(ServerTrigger t, int? x, GameCard cardTriggerer, IStackable stackTriggerer, Player triggerer)
    {
        GameCard cardWhoseTrigger = t.effToTrigger.Source;
        int effIndex = t.effToTrigger.EffectIndex;
        //TODO send info about triggerer to display on client
        Packet packet = new Packet(Packet.Command.OptionalTrigger, cardWhoseTrigger, effIndex, 0, x ?? 0, 0);
        SendPacket(packet);
    }
    #endregion other effect stuff
}
