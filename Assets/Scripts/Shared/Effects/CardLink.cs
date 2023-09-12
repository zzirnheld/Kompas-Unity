using KompasCore.Cards;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Effects
{
	/// <summary>
	/// NOTE: Never edit a link after creating it. Delete it and create a new one.
	/// Otherwise, the client won't know the difference between two links of the same cards, from the same effects,
	/// which is a scenario I want to allow. (Two activations of the same card's effect linking the same cards a second time over)
	/// </summary>
	public class CardLink
	{
		public static readonly Color32 DefaultColor = new (195, 0, 195, 195);

		public HashSet<int> CardIDs { get; }
		public Effect LinkingEffect { get; }
		public Color32 LinkColor { get; }

		public CardLink(HashSet<int> cardIDs, Effect linkingEffect, Color32 linkColor)
		{
			CardIDs = cardIDs;
			LinkingEffect = linkingEffect;
			LinkColor = linkColor;
		}

		public bool Matches(IEnumerable<int> cardIDs, Effect linkingEffect)
		{
			return LinkingEffect == linkingEffect && CardIDs.SetEquals(cardIDs);
		}
	}
}
