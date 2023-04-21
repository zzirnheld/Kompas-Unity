
using KompasCore.Cards;
using KompasCore.Effects;
using UnityEngine;

namespace KompasClient.Effects
{
	public class ClientAttack : Attack, IClientStackable
	{
		public Sprite PrimarySprite => attacker.SimpleSprite;
		public CardController PrimaryCardController => attacker.CardController;

		public Sprite SecondarySprite => defender.SimpleSprite;
		public CardController SecondaryCardController => defender.CardController;

		public string StackableBlurb => $"{attacker.CardName} attacks {defender.CardName}";

		public ClientAttack(Player controller, GameCard attacker, GameCard defender) : base(controller, attacker, defender) { }
	}
}