using KompasCore.Cards;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
	public class Location : CardRestrictionElement
	{
		[JsonProperty(Required = Required.Always)]
		public string[] locations;

		public Location() { }
		public Location(CardLocation location)
		{
			locations = new string[] { CardLocationHelpers.StringVersion(location) };
		}

		protected IReadOnlyCollection<CardLocation> Locations => locations.Select(CardLocationHelpers.FromString).ToArray();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			if (locations == null) throw new System.ArgumentNullException("locations");
		}

		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
			=> Locations.Any(loc => card.Location == loc);

		public override string ToString() => $"must be in {string.Join(", ", Locations)}";
	}
}