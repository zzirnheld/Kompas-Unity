using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.Effects;
using KompasServer.GameCore;
using System.Collections.Generic;
using System.Linq;
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
            //if (packet != null) Debug.Log($"Sending packet to {Player.index} with info {packet}");
            ServerNetworkCtrl.SendPacket(packet);
        }

        private void SendPackets(Packet a, Packet b)
        {
            SendPacket(a);
            OtherNotifier.SendPacket(b);
        }

        private void SendToBoth(Packet p) => SendPackets(p, p.Copy());

        private void SendToBothInverting(Packet p, bool known = true) => SendPackets(p, p.GetInversion(known));

        #region game start
        public void GetDecklist() => SendPacket(new GetDeckPacket());

        public void DeckAccepted() => SendPacket(new DeckAcceptedPacket());

        public void SetFriendlyAvatar(string json, int cardID)
            => SendToBothInverting(new SetAvatarPacket(0, json, cardID));

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

        public void NotifyPutBack() => SendPacket(new PutCardsBackPacket());

        public void NotifyBothPutBack() => SendToBoth(new PutCardsBackPacket());

        #region game stats
        public void NotifyLeyload(int leyload) => SendToBoth(new SetLeyloadPacket(leyload));

        public void NotifySetPips(int pipsToSet)
            => SendToBothInverting(new SetPipsPacket(pipsToSet, Player.index, invert: Player.index != 0));
        public void NotifyYourTurn() => SendToBothInverting(new SetTurnPlayerPacket(0));

        public void NotifyDeckCount(int count) => SendToBothInverting(new SetDeckCountPacket(0, count));
        #endregion game stats

        #region card location
        public void NotifyAttach(GameCard toAttach, int x, int y, bool wasKnown)
            => SendToBothInverting(new AttachCardPacket(toAttach, x, y, invert: Player.index != 0), wasKnown);

        /// <summary>
        /// Notifies that the Player corresponding to this notifier played a given card
        /// </summary>
        public void NotifyPlay(GameCard toPlay, int x, int y, bool wasKnown)
        {
            //if this card is an augment, don't bother notifying about it. attach will take care of it.
            if (toPlay.CardType == 'A') return;

            //tell everyone to do it
            var p = new PlayCardPacket(toPlay.ID, toPlay.BaseJson, toPlay.ControllerIndex, x, y, invert: Player.index != 0);
            var q = p.GetInversion(wasKnown);
            SendPackets(p, q);
        }

        public void NotifyMove(GameCard toMove, int x, int y)
            => SendToBothInverting(new MoveCardPacket(toMove.ID, x, y, invert: Player.index != 0));

        public void NotifyDiscard(GameCard toDiscard, bool wasKnown)
            => SendToBothInverting(new DiscardCardPacket(toDiscard, invert: Player.index != 0), wasKnown);

        public void NotifyRehand(GameCard toRehand, bool wasKnown)
            => SendToBothInverting(new RehandCardPacket(toRehand.ID), wasKnown);

        public void NotifyDecrementHand() => SendPacket(new ChangeEnemyHandCountPacket(-1));

        public void NotifyAnnhilate(GameCard toAnnhilate, bool wasKnown)
            => SendToBothInverting(new AnnihilateCardPacket(toAnnhilate.ID, toAnnhilate.BaseJson, toAnnhilate.ControllerIndex, invert: Player.index != 0), 
                known: wasKnown);

        public void NotifyTopdeck(GameCard card, bool wasKnown)
            => SendToBothInverting(new TopdeckCardPacket(card.ID, card.OwnerIndex, invert: Player.index != 0), 
                known: wasKnown);

        public void NotifyBottomdeck(GameCard card, bool wasKnown)
            => SendToBothInverting(new BottomdeckCardPacket(card.ID, card.OwnerIndex, invert: Player.index != 0),
                known: wasKnown);

        public void NotifyReshuffle(GameCard toReshuffle, bool wasKnown)
            => SendToBothInverting(new ReshuffleCardPacket(toReshuffle.ID, toReshuffle.OwnerIndex, invert: Player.index != 0),
                known: wasKnown);

        public void NotifyCreateCard(GameCard added, bool wasKnown)
            => SendToBothInverting(new AddCardPacket(added, invert: Player.index != 0), known: wasKnown);

        public void NotifyRevealCard(GameCard revealed)
        {
            //NotifyDecrementHand();
            SendPacket(new AddCardPacket(revealed, invert: Player.index != 0));
        }

        public void GetHandSizeChoices(int[] cardIds, string listRestrictionJson)
            => SendPacket(new GetHandSizeChoicesOrderPacket(cardIds, listRestrictionJson));

        public void NotifyHandSizeToStack() => SendToBothInverting(new HandSizeToStackPacket(0));
        #endregion card location

        #region card stats
        public void NotifyStats(GameCard card)
            => SendToBothInverting(new ChangeCardNumericStatsPacket(card.ID, card.Stats, card.SpacesMoved), card.KnownToEnemy);

        public void NotifySpacesMoved(GameCard card)
            => SendToBothInverting(new SpacesMovedPacket(card.ID, card.SpacesMoved), card.KnownToEnemy);

        public void NotifyAttacksThisTurn(GameCard card) 
            => SendToBothInverting(new AttacksThisTurnPacket(card.ID, card.AttacksThisTurn), card.KnownToEnemy);

        public void NotifySetNegated(GameCard card, bool negated)
            => SendToBothInverting(new NegateCardPacket(card.ID, negated), card.KnownToEnemy);

        public void NotifyActivate(GameCard card, bool activated)
            => SendToBothInverting(new ActivateCardPacket(card.ID, activated), card.KnownToEnemy);

        public void NotifyResetCard(GameCard card)
            => SendToBothInverting(new ResetCardPacket(card.ID), card.KnownToEnemy);

        public void NotifyChangeController(GameCard card, Player controller)
            => SendToBothInverting(new ChangeCardControllerPacket(card.ID, controller.index, invert: Player.index != 0), card.KnownToEnemy);
        #endregion card stats

        #region request targets
        public void GetCardTarget(string cardName, string targetBlurb, int[] ids, string listRestrictionJson, bool list)
            => SendPacket(new GetCardTargetPacket(cardName, targetBlurb, ids, listRestrictionJson, list));

        public void GetSpaceTarget(string cardName, string targetBlurb, (int, int)[] spaces)
            => SendPacket(new GetSpaceTargetPacket(cardName, targetBlurb, spaces));
        #endregion request targets

        public void NotifyAttackStarted(GameCard atk, GameCard def, Player initiator) 
            => SendToBoth(new AttackStartedPacket(atk.ID, def.ID, initiator.index));

        #region other effect stuff
        public void ChooseEffectOption(string cardName, string choiceBlurb, string[] optionBlurbs, bool hasDefault, bool showX, int x) 
            => SendPacket(new GetEffectOptionPacket(cardName, choiceBlurb, optionBlurbs, hasDefault, showX, x));

        public void EffectResolving(ServerEffect eff)
            => SendToBothInverting(new EffectResolvingPacket(eff.Source.ID, eff.EffectIndex, eff.Controller.index, invert: Player.index != 0));

        public void NotifyEffectActivated(ServerEffect eff) 
            => SendToBoth(new EffectActivatedPacket(eff.Source.ID, eff.EffectIndex));

        public void RemoveStackEntry(int i) => SendToBoth(new RemoveStackEntryPacket(i));

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

        public void AskForTrigger(ServerTrigger t, int x, bool showX) 
            => SendToBothInverting(new OptionalTriggerPacket(t.serverEffect.Source.ID, t.serverEffect.EffectIndex, x, showX));

        public void GetTriggerOrder(IEnumerable<ServerTrigger> triggers)
        {
            int[] cardIds = triggers.Select(t => t.serverEffect.Source.ID).ToArray();
            int[] effIndices = triggers.Select(t => t.serverEffect.EffectIndex).ToArray();
            SendPacket(new GetTriggerOrderPacket(cardIds, effIndices));
        }
        #endregion other effect stuff
    }
}