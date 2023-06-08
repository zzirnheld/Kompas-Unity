using KompasCore.Cards;
using KompasCore.Effects;
using UnityEngine;

namespace KompasClient.Effects
{
	public interface IClientStackable : IStackable
	{
		/// <summary>
		/// The primary card for this stackable. The source of an effect, or an attacker
		/// </summary>
		Sprite PrimarySprite { get; }

		/// <summary>
		/// The primary card for this stackable. The source of an effect, or an attacker
		/// </summary>
		CardController PrimaryCardController { get; }

		/// <summary>
		/// The secondary card for this stackable. The defender of an attack
		/// </summary>
		Sprite SecondarySprite { get; }

		/// <summary>
		/// The secondary card for this stackable. The defender of an attack
		/// </summary>
		CardController SecondaryCardController { get; }

		/// <summary>
		/// The blurb for this stackable
		/// </summary>
		string StackableBlurb { get; }
	}
}