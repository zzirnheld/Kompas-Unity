using KompasCore.Cards;
using KompasCore.Cards.Movement;
using KompasCore.Effects;
using KompasCore.Effects.Restrictions;
using KompasServer.GameCore;
using Newtonsoft.Json;
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
			ServerController.notifier.NotifyHandSizeToStack();
		}
		public async Task StartResolution(IResolutionContext context) => await RequestTargets();

		private async Task RequestTargets()
		{
			Debug.Log("Trying to request hand size targets");
			awaitingChoices = true;

			var context = new ResolutionContext(new TriggeringEventContext(game: serverGame, stackableCause: this, stackableEvent: this));
			int[] cardIds = serverGame.Cards
				.Where(c => HandSizeCardRestriction.IsValid(c, context))
				.Select(c => c.ID)
				.ToArray();

			int overHandSize = cardIds.Count() - Controller.HandSizeLimit;
			if (overHandSize <= 0)
			{
				awaitingChoices = false;
				return;
			}

			var listRestriction = IListRestriction.ConstantCount(overHandSize);
			string listRestrictionJson = JsonConvert.SerializeObject(listRestriction);

			int[] choices = null;
			while (!TryAnswer(choices))
			{
				choices = await ServerController.awaiter.GetHandSizeChoices(cardIds, listRestrictionJson);
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
			var context = new ResolutionContext(new TriggeringEventContext(game: serverGame, stackableCause: this, stackableEvent: this));
			int correctCount = serverGame.Cards.Count(c => HandSizeCardRestriction.IsValid(c, context)) - Controller.HandSizeLimit;

			if (count != correctCount || cards.Any(c => !HandSizeCardRestriction.IsValid(c, context))) return false;

			foreach (var card in cards) card.Reshuffle();
			awaitingChoices = false;
			return true;
		}
	}
}