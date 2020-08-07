using KompasCore.Cards;
using KompasCore.Networking;
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
            Debug.Log($"Requesting {card.CardName} to be played to {toX} {toY}");
            if (card.CardType == 'A') Send(new AugmentActionPacket(card.ID, toX, toY));
            else Send(new PlayActionPacket(card.ID, toX, toY));
        }

        public void RequestMove(GameCard card, int toX, int toY)
        {
            Debug.Log($"Requesting {card.CardName} to be moved to {toX} {toY}");
            Send(new MoveActionPacket(card.ID, toX, toY));
        }

        public void RequestAttack(GameCard attacker, GameCard defender)
        {
            Send(new AttackActionPacket(attacker.ID, defender.ID));
        }

        public void RequestDecklistImport(string decklist)
        {
            Debug.Log("Requesting Deck import of \"" + decklist + "\"");
            Send(new SetDeckPacket(decklist));
        }

        public void RequestEndTurn()
        {
            Send(new EndTurnActionPacket());
        }

        public void RequestTarget(GameCard card)
        {
            Send(new CardTargetPacket(card.ID));
        }

        public void RequestResolveEffect(GameCard card, int index)
        {
            Send(new ActivateEffectActionPacket(card.ID, index));
        }

        public void RequestSetX(int x)
        {
            Debug.Log("Requesting to set X to " + x);
            Send(new SelectXPacket(x));
        }

        public void DeclineAnotherTarget()
        {
            Debug.Log("Declining to select another target");
            Send(new DeclineAnotherTargetPacket());
        }

        public void RequestSpaceTarget(int x, int y)
        {
            Debug.Log("Requesting a space target of " + x + ", " + y);
            Send(new SpaceTargetPacket(x, y));
        }

        public void RequestListChoices(List<GameCard> choices)
        {
            Send(new ListChoicesPacket(choices));
        }

        public void RequestTriggerReponse(bool answer)
        {
            Debug.Log($"Requesting trigger response for {answer}");
            Send(new OptionalTriggerAnswerPacket(answer));
        }

        public void RequestChooseEffectOption(int option)
        {
            Send(new EffectOptionResponsePacket(option));
        }

        public void DeclineResponse()
        {
            Send(new PassPriorityPacket());
        }
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