﻿using KompasCore.Cards;
using KompasCore.Networking;
using System.Collections.Generic;
using UnityEngine;

namespace KompasClient.Networking
{
    public class ClientNotifier : MonoBehaviour
    {
        public ClientNetworkController clientNetworkCtrl;

        private void Send(Packet packet)
        {
            if (packet != null) Debug.Log($"Sending packet with command {packet?.command}, normal args {string.Join(",", packet?.normalArgs)}, " +
                 $"special args {string.Join(",", packet?.specialArgs)}, string arg {packet?.stringArg}");
            clientNetworkCtrl.SendPacket(packet);
        }

        #region Normal Request Actions
        public void RequestPlay(GameCard card, int toX, int toY)
        {
            Debug.Log($"Requesting {card.CardName} to be played to {toX} {toY}");

            Packet packet;
            if (card.CardType == 'A') packet = new Packet(Packet.Command.Augment, card, toX, toY);
            else packet = new Packet(Packet.Command.Play, card, toX, toY);
            Send(packet);
        }

        public void RequestMove(GameCard card, int toX, int toY)
        {
            Debug.Log($"Requesting {card.CardName} to be moved to {toX} {toY}");
            Packet packet = new Packet(Packet.Command.Move, card, toX, toY);
            Send(packet);
        }

        public void RequestAttack(GameCard card, int toX, int toY)
        {
            Packet packet = new Packet(Packet.Command.Attack, card, toX, toY);
            Send(packet);
        }

        public void RequestDecklistImport(string decklist)
        {
            Debug.Log("Requesting Deck import of \"" + decklist + "\"");
            string[] cardNames = decklist.Split('\n');
            Packet packet = new Packet(Packet.Command.SetDeck)
            {
                stringArg = decklist
            };
            Send(packet);
        }

        public void RequestEndTurn()
        {
            Packet packet = new Packet(Packet.Command.EndTurn);
            Send(packet);
        }

        public void RequestTarget(GameCard card)
        {
            Debug.Log("Requesting target " + card.CardName);
            Packet packet = new Packet(Packet.Command.Target, card);
            Send(packet);
        }

        public void RequestResolveEffect(GameCard card, int index)
        {
            if (card == null) return;
            Debug.Log("Requesting effect of " + card.CardName + " number" + index);
            Packet packet = new Packet(Packet.Command.ActivateEffect, card, index);
            Send(packet);
        }

        public void RequestSetX(int x)
        {
            Debug.Log("Requesting to set X to " + x);
            Packet packet = new Packet(Packet.Command.PlayerSetX);
            packet.normalArgs[2] = x;
            Send(packet);
        }

        public void DeclineAnotherTarget()
        {
            Debug.Log("Declining to select another target");
            Packet packet = new Packet(Packet.Command.DeclineAnotherTarget);
            Send(packet);
        }

        public void RequestSpaceTarget(int x, int y)
        {
            Debug.Log("Requesting a space target of " + x + ", " + y);
            Packet packet = new Packet(Packet.Command.SpaceTarget, x, y);
            Send(packet);
        }

        public void RequestListChoices(List<GameCard> choices)
        {
            int[] cardIDs = new int[choices.Count];
            for (int i = 0; i < choices.Count; i++)
            {
                cardIDs[i] = choices[i].ID;
            }
            Packet packet = new Packet(Packet.Command.GetChoicesFromList, cardIDs);
            Send(packet);
        }

        public void RequestCancelSearch()
        {
            Packet packet = new Packet(Packet.Command.CancelSearch);
            Send(packet);
        }

        public void RequestTriggerReponse(bool answer)
        {
            Debug.Log($"Requesting trigger response for {answer}");
            Packet packet = new Packet(Packet.Command.OptionalTrigger, answer ? 1 : 0);
            Send(packet);
        }

        public void RequestChooseEffectOption(int option)
        {
            Packet packet = new Packet(Packet.Command.ChooseEffectOption, option);
            Send(packet);
        }

        public void DeclineResponse()
        {
            var p = new Packet(Packet.Command.Response);
            Send(p);
        }
        #endregion

        #region Debug Request Actions
        public void RequestTopdeck(GameCard card)
        {
            Packet packet = new Packet(Packet.Command.Topdeck, card);
            Send(packet);
        }

        public void RequestDiscard(GameCard card)
        {
            Packet packet = new Packet(Packet.Command.Discard, card);
            Send(packet);
        }

        public void RequestRehand(GameCard card)
        {
            Packet packet = new Packet(Packet.Command.Rehand, card);
            Send(packet);
        }

        public void RequestDraw()
        {
            Packet packet = new Packet(Packet.Command.Draw);
            Send(packet);
        }

        public void RequestSetNESW(GameCard card, int n, int e, int s, int w)
        {
            Packet packet = new Packet(Packet.Command.SetNESW, card, n, e, s, w);
            Send(packet);
        }

        public void RequestUpdatePips(int num)
        {
            Packet packet = new Packet(Packet.Command.SetPips, num);
            Send(packet);
            Debug.Log("requesting updating pips to " + num);
        }
        #endregion
    }
}