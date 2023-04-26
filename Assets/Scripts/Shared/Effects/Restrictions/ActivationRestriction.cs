using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
	public class ActivationRestriction : Restrictions.PlayerRestrictionElements.AllOf
	{
		public Effect Effect => InitializationContext.effect;
		public GameCard Card => InitializationContext.source;

		public const string Never = "Never";

		public const string ControllerActivates = "Controller Activates";
		public const string NotNegated = "Not Negated";
		public const string InPlay = "In Play";
		public const string NotCurrentlyActivated = "Not Currently Activated";
		public const string FriendlyTurn = "Friendly Turn";
		public const string NothingHappening = "Nothing Happening";

		public const string Default = "Default";
		public static readonly string[] DefaultRestrictions = { ControllerActivates, NotNegated, InPlay, NotCurrentlyActivated, FriendlyTurn, NothingHappening };

		public static readonly string[] AtAllRestrictions =
			{ FriendlyTurn, NotNegated, InPlay,  NotCurrentlyActivated };

		public int maxTimes = 1;
		private readonly List<string> ActivationRestrictions = new List<string>();
		public string[] activationRestrictionArray = null;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);

			if (activationRestrictionArray == null) ActivationRestrictions.Add(Never);
			else
			{
				ActivationRestrictions.AddRange(activationRestrictionArray);
				if (activationRestrictionArray.Contains("Default"))
					ActivationRestrictions.AddRange(DefaultRestrictions);

				Debug.Log($"Initializing activation restriction for {Card.CardName} " +
					$"with restrictions: {string.Join(", ", ActivationRestrictions)}");
			}
		}

		private bool IsRestrictionValid(string r, Player activator) => r switch
		{
			Never => false,
			Default => true,

			FriendlyTurn => Effect.Game.TurnPlayer == activator,

			InPlay => Effect.Source.Location == CardLocation.Board,

			ControllerActivates => activator == Card.Controller,

			NotNegated => !Effect.Negated,

			NotCurrentlyActivated => !Effect.Game.StackEntries.Any(e => e == Effect),
			NothingHappening => Effect.Game.NothingHappening,

			_ => throw new System.ArgumentException($"Invalid activation restriction {r}")
		};

		/* This exists to debug a card's activation restriction,
		 * but should not be usually used because it prints a ton whenever
		 * a card's effect buttons are considered, or when the game checks to see if a person has a response.
		public bool RestrictionValidWithDebug(string restriction, Player activator)
		{
			bool valid = RestrictionValid(restriction, activator);
			if (!valid) Debug.Log($"Card {Card.CardName} effect # {Effect.EffectIndex} activation restriction " +
				$"flouts restriction {restriction} for activator {activator.index}");
			return valid;
		} */

		private bool IsGameSetUp() => Card != null && Card.Game != null;

		public bool IsValidActivation(Player activator)
			=> IsGameSetUp() && ActivationRestrictions.All(r => IsRestrictionValid(r, activator));

		protected override bool IsValidLogic(Player item, IResolutionContext context)
		{
			return base.IsValidLogic(item, context) && IsValidActivation(item);
		}

		public bool IsPotentiallyValidActivation(Player activator)
			=> IsGameSetUp() && ActivationRestrictions.Intersect(AtAllRestrictions).All(r => IsRestrictionValid(r, activator));
	}
}