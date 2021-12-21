using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class ServerHandSizeStackable : HandSizeStackable, IServerStackable
    {
        public ServerPlayer ServerController { get; }
        public override Player Controller => ServerController;

        private readonly ServerGame serverGame;

        private bool awaitingChoices;

        public ServerHandSizeStackable(ServerGame serverGame, ServerPlayer controller)
        {
            this.ServerController = controller;
            this.serverGame = serverGame;

            //tell the players this is here now
            ServerController.ServerNotifier.NotifyHandSizeToStack();
        }
        public async Task StartResolution(ActivationContext context) => await RequestTargets();

        private async Task RequestTargets()
        {
            awaitingChoices = true;

            int[] cardIds = serverGame.Cards
                .Where(c => HandSizeCardRestriction.Evaluate(c, new ActivationContext()))
                .Select(c => c.ID)
                .ToArray();

            int overHandSize = cardIds.Count() - Controller.HandSizeLimit;
            if (overHandSize <= 0)
            {
                awaitingChoices = false;
                return;
            }

            var listRestriction = HandSizeListRestriction;
            listRestriction.minCanChoose = overHandSize;
            listRestriction.maxCanChoose = overHandSize;
            string listRestrictionJson = JsonUtility.ToJson(listRestriction);

            int[] choices = null;
            while (!TryAnswer(choices))
            {
                choices = await ServerController.serverAwaiter.GetHandSizeChoices(cardIds, listRestrictionJson);
            }
        }

        public bool TryAnswer(int[] cardIds)
        {
            if (!awaitingChoices) return false;
            if (cardIds == null) return false;

            GameCard[] cards = cardIds
                .Distinct()
                .Select(i => serverGame.GetCardWithID(i))
                .Where(c => c != null)
                .ToArray();

            int count = cards.Count();
            int correctCount = serverGame.Cards.Count(c => HandSizeCardRestriction.Evaluate(c, new ActivationContext())) - Controller.HandSizeLimit;

            if (count != correctCount || cards.Any(c => !HandSizeCardRestriction.Evaluate(c, new ActivationContext()))) return false;

            foreach (var card in cards) card.Reshuffle();
            awaitingChoices = false;
            return true;
        }
    }
}