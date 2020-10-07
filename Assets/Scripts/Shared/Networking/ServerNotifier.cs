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

        /// <summary>
        /// Takes care of inverting first turn player
        /// </summary>
        /// <param name="firstPlayer">First turn player, from the server's perspective</param>
        public void SetFirstTurnPlayer(int firstPlayer)
        {
            var p = new SetFirstPlayerPacket((firstPlayer + Player.index) % Player.serverGame.Players.Length);
            SendPacket(p);
        }
        #endregion game start

        #region notify
        public void NotifyPutBack() => SendPacket(new PutCardsBackPacket());

        public void NotifyBothPutBack() => SendToBoth(new PutCardsBackPacket());

        public void NotifyLeyload(int leyload) => SendToBoth(new SetLeyloadPacket(leyload));

        public void NotifyAttach(GameCard toAttach, int x, int y)
        {
            //tell everyone to do it
            var p = new AttachCardPacket(toAttach.ID, toAttach.CardName, toAttach.ControllerIndex, x, y, invert: Player.index != 0);
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
            var p = new PlayCardPacket(toPlay.ID, toPlay.CardName, toPlay.ControllerIndex, x, y, invert: Player.index != 0);
            var q = p.GetInversion(toPlay.KnownToEnemy);
            SendPackets(p, q);
        }

        public void NotifyMove(GameCard toMove, int x, int y, bool playerInitiated)
        {
            var p = new MoveCardPacket(toMove.ID, x, y, playerInitiated, invert: Player.index != 0);
            var q = p.GetInversion(known: true);
            SendPackets(p, q);
        }

        public void NotifyDiscard(GameCard toDiscard)
        {
            var p = new DiscardCardPacket(toDiscard.ID, toDiscard.CardName, toDiscard.ControllerIndex, invert: Player.index != 0);
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
            var p = new AddCardPacket(added.ID, added.CardName, added.Location, added.ControllerIndex,
                added.BoardX, added.BoardY, attached: false, invert: Player.index != 0);
            var q = p.GetInversion(added.KnownToEnemy);
            SendPackets(p, q);
        }

        public void NotifySetPips(int pipsToSet)
        {
            var p = new SetPipsPacket(pipsToSet, Player.index, invert: Player.index != 0);
            var q = p.GetInversion(true);
            SendPackets(p, q);
        }

        public void NotifyStats(GameCard card)
        {
            var p = new ChangeCardNumericStatsPacket(card.ID, card.Stats);
            var q = p.GetInversion(card.KnownToEnemy);
            SendPackets(p, q);
        }

        public void NotifySetNegated(GameCard card, bool negated)
        {
            var p = new NegateCardPacket(card.ID, negated);
            var q = p.GetInversion(card.KnownToEnemy);
            SendPackets(p, q);
        }

        public void NotifyActivate(GameCard card, bool activated)
        {
            var p = new ActivateCardPacket(card.ID, activated);
            var q = p.GetInversion(card.KnownToEnemy);
            SendPackets(p, q);
        }

        public void NotifyResetCard(GameCard card)
        {
            var p = new ResetCardPacket(card.ID);
            var q = p.GetInversion(card.KnownToEnemy);
            SendPackets(p, q);
        }

        public void NotifyChangeController(GameCard card, Player controller)
        {
            var p = new ChangeCardControllerPacket(card.ID, controller.index, invert: Player.index != 0);
            var q = p.GetInversion(card.KnownToEnemy);
            SendPackets(p, q);
        }

        public void NotifySetTurn(ServerGame sGame, int indexToSet)
        {
            var p = new SetTurnPlayerPacket(indexToSet, invert: Player.index != 0);
            var q = p.GetInversion();
            SendPackets(p, q);
        }
        #endregion notify

        #region request targets
        public void GetBoardTarget(BoardTargetSubeffect boardTargetSubeffect)
            => SendPacket(new GetBoardTargetPacket(boardTargetSubeffect.cardRestriction));

        public void GetDeckTarget(CardTargetSubeffect cardTargetSubeffect)
            => SendPacket(new GetDeckTargetPacket(cardTargetSubeffect.cardRestriction));

        public void GetDiscardTarget(CardTargetSubeffect cardTargetSubeffect)
            => SendPacket(new GetDiscardTargetPacket(cardTargetSubeffect.cardRestriction));

        public void GetHandTarget(CardTargetSubeffect cardTargetSubeffect)
            => SendPacket(new GetHandTargetPacket(cardTargetSubeffect.cardRestriction));

        public void GetSpaceTarget(SpaceTargetSubeffect spaceTargetSubeffect)
            => SendPacket(new GetSpaceTargetPacket(spaceTargetSubeffect.spaceRestriction));

        public void GetChoicesFromList(IEnumerable<GameCard> potentialTargets, int maxNum, ChooseFromListSubeffect src)
        {
            var p = new GetListChoicesPacket(potentialTargets.Select(c => c.ID).ToArray(), maxNum,
                src.Source.ID, src.Effect.EffectIndex, src.SubeffIndex);
            SendPacket(p);
        }
        #endregion request targets

        #region other effect stuff
        public void ChooseEffectOption(ChooseOptionSubeffect src) => SendPacket(new GetEffectOptionPacket(src));

        public void EffectResolving(ServerEffect eff)
        {
            var p = new EffectResolvingPacket(eff.Source.ID, eff.EffectIndex, eff.Controller.index, invert: Player.index != 0);
            var q = p.GetInversion();
            SendPackets(p, q);
        }

        public void NotifyEffectActivated(ServerEffect eff) => SendToBoth(new EffectActivatedPacket(eff.Source.ID, eff.EffectIndex));

        public void EffectImpossible() => SendToBoth(new EffectImpossiblePacket());

        public void RequestResponse() => SendPacket(new ToggleAllowResponsesPacket(true));
        
        public void RequestNoResponse() => SendPacket(new ToggleAllowResponsesPacket(false));

        /// <summary>
        /// Lets that player know their target has been accepted. called if the Target method returns True
        /// </summary>
        public void AcceptTarget() => SendPacket(new TargetAcceptedPacket());

        public void StackEmpty() => SendToBoth(new StackEmptyPacket());

        public void SetTarget(GameCard card, int effIndex, GameCard target)
            => SendToBoth(new AddTargetPacket(card.ID, effIndex, target.ID));

        public void RemoveTarget(GameCard card, int effIndex, GameCard target)
            => SendToBoth(new RemoveTargetPacket(card.ID, effIndex, target.ID));

        public void GetXForEffect() => SendPacket(new GetPlayerChooseXPacket());

        public void NotifyEffectX(GameCard effSrc, int effIndex, int x)
        {
            var p = new SetEffectsXPacket(effSrc.ID, effIndex, x);
            SendToBoth(p);
        }

        public void EnableDecliningTarget() => SendPacket(new ToggleDecliningTargetPacket(true));

        public void DisableDecliningTarget() => SendPacket(new ToggleDecliningTargetPacket(false));

        public void DiscardSimples() => SendToBoth(new DiscardSimplesPacket());

        public void AskForTrigger(ServerTrigger t) 
            => SendPacket(new OptionalTriggerPacket(t.serverEffect.Source.ID, t.serverEffect.EffectIndex));

        public void GetTriggerOrder(IEnumerable<ServerTrigger> triggers)
        {
            int[] cardIds = triggers.Select(t => t.serverEffect.Source.ID).ToArray();
            int[] effIndices = triggers.Select(t => t.serverEffect.EffectIndex).ToArray();
            SendPacket(new GetTriggerOrderPacket(cardIds, effIndices));
        }
        #endregion other effect stuff
    }
}