using KompasCore.Cards;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
	public class NumberRestriction : AllOfBase<int>
	{
		public const string LessThanEffectX = "<X";
		public const string GreaterThanEffectX = ">X";
		public const string EqualsEffectX = "=X";
		public const string LessThanEqualEffectX = "<=X";

		public const string LessThanEffectArg = "<Arg";

		public const string LessThanAvatarCardValue = "<Avatar";
		public const string GreaterThanAvatarCardValue = ">Avatar";

		public const string GreaterThanSourceCardValue = ">Source";

		public string[] numberRestrictions = { };

		public int constant;

		public CardValue cardValue;

		public EffectInitializationContext initializationContext { get; private set; }
		public GameCard Source => InitializationContext.source;
		public Subeffect Subeffect => InitializationContext.subeffect;

		private bool IsRestrictionValid(string r, int x) => r switch
		{
			LessThanEffectX => x < Subeffect.Count,
			GreaterThanEffectX => x > Subeffect.Count,
			EqualsEffectX => x == Subeffect.Count,
			LessThanEqualEffectX => x <= Subeffect.Count,

			LessThanEffectArg => x < Subeffect.Effect.arg,

			LessThanAvatarCardValue => x < cardValue.GetValueOf(Source.Controller.Avatar),
			GreaterThanAvatarCardValue => x > cardValue.GetValueOf(Source.Controller.Avatar),

			_ => throw new System.ArgumentException($"Invalid X restriction {r} in X Restriction."),
		};

		private bool IsRestrictionValidDebug(string r, int x)
		{
			bool answer = IsRestrictionValid(r, x);
			if (!answer) Debug.Log($"{x} flouts {r} when effect count is {Subeffect?.Count}");
			return answer;
		}

		protected override bool IsValidLogic(int item, IResolutionContext context)
			=> base.IsValidLogic(item, context)
			&& numberRestrictions.All(r => IsRestrictionValidDebug(r, item));
	}
}