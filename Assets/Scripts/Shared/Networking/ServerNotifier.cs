using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasCore.Networking;
using KompasServer.Cards;
using KompasServer.Effects;
using KompasServer.GameCore;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace KompasServer.Networking
{
    public class ServerNotifier : MonoBehaviour
    {
        public ServerPlayer Player;
        public ServerNetworkController ServerNetworkCtrl;
        public ServerNotifier OtherNotifier;

        public void SendPacket(Packet packet)
        {
            if (packet != null) Debug.Log($"Sending packet to {Player.index} with info {packet}");
            ServerNetworkCtrl.SendPacket(packet);
        }

        private void SendPackets(Packet a, Packet b)
        {
            SendPacket(a);
            OtherNotifier.SendPacket(b);
        }

        private void SendToBoth(Packet p)
        {
            SendPackets(p, p.Copy());
        }

        #region game start
        public void GetDecklist()
        {
            SendPacket(new GetDeckPacket());
        }

        public void DeckAccepted()
        {
            SendPacket(new DeckAcceptedPacket());
        }

        public void SetFriendlyAvatar(string cardName, int cardID)
        {
            var p = new SetAvatarPacket(0, cardName, cardID);
            var q = new SetAvatarPacket(1, cardName, cardID);
            SendPackets(p, q);
        }

        public void SetFirstTurnPlayer(int firstPlayer)
        {
            var p = new SetFirstPlayerPacket((firstPlayer + Player.index) % Player.serverGame.Players.Length);
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

        public void NotifyLeyload(int leyload)
        {
            SendToBoth(new SetLeyloadPacket(leyload));
        }

        public void NotifyAttach(GameCard toAttach, int x, int y)
        {
            //tell everyone to do it
            var p = new AttachCardPacket(toAttach.ID, toAttach.CardName, toAttach.ControllerIndex, x, y);
            var q = p.GetInversion(toAttach.KnownToEnemy);
            SendPackets(p, q);
        }

        /// <summary>
        /// Notifies that the Player corresponding to this notifier played a given card
        /// </summary>
        public void NotifyPlay(GameCard toPlay, int x, int y)
        {
            //if this card is an augment, don't bother notifying about it. attach will take care of it.
            if (toPlay.CardType == 'A') return;

            //tell everyone to do it
            var p = new PlayCardPacket(toPlay.ID, toPlay.CardName, toPlay.ControllerIndex, x, y);
            var q = p.GetInversion(toPlay.KnownToEnemy);
            SendPackets(p, q);
        }

        public void NotifyMove(GameCard toMove, int x, int y, bool playerInitiated)
        {
            var p = new MoveCardPacket(toMove.ID, x, y, playerInitiated, invert: toMove.ControllerIndex == 0);
            var q = p.GetInversion(known: true);
            SendPackets(p, q);
        }

        public void NotifyDiscard(GameCard toDiscard)
        {
            var p = new DiscardCardPacket(toDiscard.ID, toDiscard.CardName, toDiscard.ControllerIndex, invert: toDiscard.ControllerIndex == 0);
            var q = p.GetInversion(toDiscard.KnownToEnemy);
            SendPackets(p, q);
        }

        public void NotifyRehand(GameCard toRehand)
        {
            var p = new RehandCardPacket(toRehand.ID);
            var q = p.GetInversion(toRehand.KnownToEnemy);
            SendPackets(p, q);
        }

        public void NotifyDecrementHand()
        {
            var p = new ChangeEnemyHandCountPacket(-1);
            SendPacket(p);
        }

        public void NotifyAnnhilate(GameCard toAnnhilate)
        {
            var p = new AnnihilateCardPacket(toAnnhilate.ID, toAnnhilate.CardName, toAnnhilate.ControllerIndex);
            var q = p.GetInversion(toAnnhilate.KnownToEnemy);
            SendPackets(p, q);
        }

        public void NotifyTopdeck(GameCard card)
        {
            var p = new TopdeckCardPacket(card.ID, card.OwnerIndex);
            var q = p.GetInversion(card.KnownToEnemy);
            SendPackets(p, q);
        }

        public void NotifyBottomdeck(GameCard card)
        {
            var p = new BottomdeckCardPacket(card.ID, card.OwnerIndex);
            var q = p.GetInversion(card.KnownToEnemy);
            SendPackets(p, q);
        }

        public void NotifyReshuffle(GameCard toReshuffle)
        {
            var p = new ReshuffleCardPacket(toReshuffle.ID, toReshuffle.OwnerIndex);
            var q = p.GetInversion(toReshuffle.KnownToEnemy);
            SendPackets(p, q);
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

        public void NotifyStats(GameCard card)
        {
            var p = new ChangeCardNumericStatsPacket(card.ID, card.Stats);
            var q = p.GetInversion(card.KnownToEnemy);
            SendPackets(p, q);
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
            foreach (GameCard c in potentialTargets) cardIDs[i++] = c.ID;
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
        
        public void RequestNoResponse()
        {
            var p = new Packet(Packet.Command.NoMoreResponse);
            SendPacket(p);
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

        public void StackEmpty()
        {
            var p = new Packet(Packet.Command.StackEmpty);
            SendToBoth(p);
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
}