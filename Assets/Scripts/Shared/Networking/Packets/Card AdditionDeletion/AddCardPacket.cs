using KompasCore.Networking;
using KompasClient.GameCore;
using KompasCore.GameCore;
using KompasCore.Cards;
using KompasCore.Cards.Movement;

namespace KompasCore.Networking
{
	public class AddCardPacket : Packet
	{
		public int cardId;
		public string json;
		public int location;
		public int controllerIndex;
		public int x;
		public int y;
		public bool attached;
		public bool known;

		protected CardLocation Location => (CardLocation)location;

		public AddCardPacket() : base(AddCard) { }

		public AddCardPacket(int cardId, string json, CardLocation location, int controllerIndex, bool nowKnown = false, bool invert = false) : this()
		{
			this.cardId = cardId;
			this.json = json;
			this.location = (int)location;
			this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
			this.known = nowKnown;
		}

		public AddCardPacket(int cardId, string json, CardLocation location, int controllerIndex,
			int x, int y, bool attached, bool known, bool invert = false)
			: this(cardId, json, location, controllerIndex, invert: invert)
		{
			this.x = invert ? 6 - x : x;
			this.y = invert ? 6 - y : y;
			this.attached = attached;
			this.known = known;
		}

		//TODO allow for card to be added with stats not as defaults.
		//this will require using a json library that allows for polymorphism-ish stuff
		public AddCardPacket(GameCard card, bool invert = false)
			: this(card, card.KnownToEnemy, invert)
		{ }

		public AddCardPacket(GameCard card, bool known, bool invert = false)
			: this(cardId: card.ID, json: card.BaseJson, location: card.Location, controllerIndex: card.ControllerIndex,
				  x: card.Position?.x ?? 0, y: card.Position?.y ?? 0, attached: card.Attached, known: known, invert: invert)
		{ }

		public override Packet Copy() => new AddCardPacket(cardId, json, Location, controllerIndex, x, y, attached, known);

		public override Packet GetInversion(bool known)
		{
			if (Game.IsHiddenLocation(Location))
			{
				return Location switch
				{
					CardLocation.Hand => new ChangeEnemyHandCountPacket(1),
					CardLocation.Deck => null,
					_ => throw new System.ArgumentException($"What should add card packet do when a card is added to the hidden location {location}"),
				};
			}
			else return new AddCardPacket(cardId, json, Location, controllerIndex, x, y, attached, known, invert: true);
		}
	}
}

namespace KompasClient.Networking
{
	public class AddCardClientPacket : AddCardPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var controller = clientGame.clientPlayers[controllerIndex];
			var card = clientGame.cardRepo.InstantiateClientNonAvatar(json, controller, cardId);
			card.KnownToEnemy = known;
			switch (Location)
			{
				case CardLocation.Nowhere: break;
				case CardLocation.Board:
					if (attached) clientGame.BoardController.GetCardAt((x, y)).AddAugment(card);
					else card.Play((x, y), controller);
					break;
				case CardLocation.Discard:
					card.Discard();
					break;
				case CardLocation.Hand:
					card.Rehand();
					break;
				case CardLocation.Deck:
					card.Topdeck();
					break;
				case CardLocation.Annihilation:
					card.Annihilate();
					break;
				default:
					throw new System.ArgumentException($"Invalid location {location} for Add Card Client Packet to put card");
			}
		}
	}
}