using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasClient.Networking
{
    public class ClientNotifier : MonoBehaviour
    {
        public ClientNetworkController clientNetworkCtrl;

        private void Send(Packet packet)
        {
            if (packet != null) Debug.Log($"Sending packet {packet}");
            clientNetworkCtrl.SendPacket(packet);
        }

        #region Normal Request Actions
        public void RequestPlay(GameCard card, int toX, int toY)
        {
            if (card.CardType == 'A') Send(new AugmentActionPacket(card.ID, toX, toY));
            else Send(new PlayActionPacket(card.ID, toX, toY));
        }

        public void RequestMove(GameCard card, int toX, int toY)
            => Send(new MoveActionPacket(card.ID, toX, toY));

        public void RequestAttack(GameCard attacker, GameCard defender)
            => Send(new AttackActionPacket(attacker.ID, defender.ID));

        public void RequestDecklistImport(string decklist)
            => Send(new SetDeckPacket(decklist));

        public void RequestEndTurn() => Send(new EndTurnActionPacket());

        public void RequestTarget(GameCard card) => Send(new CardTargetPacket(card.ID));

        public void RequestResolveEffect(GameCard card, int index)
            => Send(new ActivateEffectActionPacket(card.ID, index));

        public void RequestSetX(int x) => Send(new SelectXPacket(x));

        public void DeclineAnotherTarget() => Send(new DeclineAnotherTargetPacket());

        public void RequestSpaceTarget(int x, int y) => Send(new SpaceTargetPacket(x, y));

        public void RequestListChoices(List<GameCard> choices) => Send(new ListChoicesPacket(choices));

        public void RequestTriggerReponse(bool answer) => Send(new OptionalTriggerAnswerPacket(answer));

        public void RequestChooseEffectOption(int option) => Send(new EffectOptionResponsePacket(option));

        public void ChooseTriggerOrder(IEnumerable<(Trigger, int)> triggers)
        {
            int count = triggers.Count();
            int[] cardIds = new int[count];
            int[] effIndices = new int[count];
            int[] orders = new int[count];
            int i = 0;
            foreach(var (t, o) in triggers)
            {
                cardIds[i] = t.Source.ID;
                effIndices[i] = t.Effect.EffectIndex;
                orders[i] = o;
                i++;
            }
            Send(new TriggerOrderResponsePacket(cardIds, effIndices, orders));
        }

        public void DeclineResponse() => Send(new PassPriorityPacket());
        #endregion

        #region Debug Request Actions
        public void RequestTopdeck(GameCard card)
        {
            Send(new DebugTopdeckPacket(card.ID));
        }

        public void RequestDiscard(GameCard card)
        {
            Send(new DebugDiscardPacket(card.ID));
        }

        public void RequestRehand(GameCard card)
        {
            Send(new DebugRehandPacket(card.ID));
        }

        public void RequestDraw()
        {
            Send(new DebugDrawPacket());
        }

        public void RequestSetNESW(GameCard card, int n, int e, int s, int w)
        {
            Send(new DebugSetNESWPacket(card.ID, n, e, s, w));
        }

        public void RequestUpdatePips(int num)
        {
            Send(new DebugSetPipsPacket(num));
        }
        #endregion
    }
}