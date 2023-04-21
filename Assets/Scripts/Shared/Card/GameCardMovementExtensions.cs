using KompasCore.Effects;

namespace KompasCore.Cards.Movement
{
	public static class GameCardMovementExtensions
	{
		public static void Discard(this GameCard card, IStackable stackSrc = null) => card.Controller.discardCtrl.Discard(card, stackSrc);

		public static void Hand(this GameCard card, Player controller, IStackable stackSrc = null) => controller.handCtrl.Hand(card, stackSrc);
		public static void Rehand(this GameCard card, IStackable stackSrc = null) => card.Hand(card.Owner, stackSrc);

		public static void Reshuffle(this GameCard card, Player controller, IStackable stackSrc = null) => controller.deckCtrl.ShuffleIn(card, stackSrc);
		public static void Reshuffle(this GameCard card, IStackable stackSrc = null) => card.Reshuffle(card.Owner, stackSrc);

		public static void Topdeck(this GameCard card, Player controller, IStackable stackSrc = null) => controller.deckCtrl.PushTopdeck(card, stackSrc);
		public static void Topdeck(this GameCard card, IStackable stackSrc = null) => card.Topdeck(card.Owner, stackSrc);

		public static void Bottomdeck(this GameCard card, Player controller, IStackable stackSrc = null) => controller.deckCtrl.PushBottomdeck(card, stackSrc);
		public static void Bottomdeck(this GameCard card, IStackable stackSrc = null) => card.Bottomdeck(card.Owner, stackSrc);

		public static void Annihilate(this GameCard card, Player controller, IStackable stackSrc = null) => controller.annihilationCtrl.Annihilate(card, stackSrc);
		public static void Annihilate(this GameCard card, IStackable stackSrc = null) => card.Annihilate(card.Owner, stackSrc);

		public static void Play(this GameCard card, Space to, Player controller, IStackable stackSrc = null, bool payCost = false)
		{
			var costToPay = card.Cost;
			card.Game.BoardController.Play(card, to, controller, stackSrc);

			if (payCost) controller.Pips -= costToPay;
		}

		public static void Move(this GameCard card, Space to, bool normalMove, IStackable stackSrc = null)
			=> card.Game.BoardController.Move(card, to, normalMove, stackSrc);

		public static void Dispel(this GameCard card, IStackable stackSrc = null)
		{
			card.SetNegated(true, stackSrc);
			card.Discard(stackSrc);
		}

	}
}