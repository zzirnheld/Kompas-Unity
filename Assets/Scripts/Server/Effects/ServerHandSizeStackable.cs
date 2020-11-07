using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasServer.Effects
{
    public class ServerHandSizeStackable : IServerStackable
    {
        public const int MaxHandSize = 7;

        public ServerPlayer ServerController { get; }
        public Player Controller => ServerController;

        public GameCard Source => null;

        public ListRestriction HandSizeListRestriction => new ListRestriction()
        {
            listRestrictions = new string[]
            {
                ListRestriction.MaxCanChoose, ListRestriction.MinCanChoose
            }
        };

        public static readonly CardRestriction cardRestriction = new CardRestriction()
        {
            cardRestrictions = new string[]
            {
                CardRestriction.Friendly, CardRestriction.Hand
            }
        };

        private readonly ServerGame serverGame;
        private readonly ServerEffectsController effectsController;

        private bool awaiting;

        public ServerHandSizeStackable(ServerGame serverGame, ServerEffectsController effectsController, ServerPlayer controller)
        {
            this.ServerController = controller;
            this.effectsController = effectsController;
            this.serverGame = serverGame;

            //tell the players this is here now
            ServerController.ServerNotifier.NotifyHandSizeToStack(true);
        }

        private void RequestTargets()
        {
            awaiting = true;
            cardRestriction.Initialize(Source, Controller, null);

            int[] cardIds = serverGame.Cards
                .Where(c => cardRestriction.Evaluate(c))
                .Select(c => c.ID)
                .ToArray();

            int overHandSize = cardIds.Count() - MaxHandSize;
            if (overHandSize <= 0)
            {
                awaiting = false;
                effectsController.FinishStackEntryResolution();
                return;
            }

            var listRestriction = HandSizeListRestriction;
            listRestriction.minCanChoose = overHandSize;
            listRestriction.maxCanChoose = overHandSize;
            string listRestrictionJson = JsonUtility.ToJson(listRestriction);

            ServerController.ServerNotifier.GetHandSizeChoices(cardIds, listRestrictionJson);
        }

        public void StartResolution(ActivationContext context) => RequestTargets();

        public void TryAnswer(int[] cardIds)
        {
            if (!awaiting) return;
            GameCard[] cards = cardIds
                .Distinct()
                .Select(i => serverGame.GetCardWithID(i))
                .Where(c => c != null)
                .ToArray();

            int count = cards.Count();
            int correctCount = serverGame.Cards.Count(c => cardRestriction.Evaluate(c)) - MaxHandSize;

            if(count != correctCount || cards.Any(c => !cardRestriction.Evaluate(c)))
            {
                RequestTargets();
                return;
            }

            foreach (var card in cards) card.Reshuffle();
            awaiting = false;
            effectsController.FinishStackEntryResolution();
        }
    }
}