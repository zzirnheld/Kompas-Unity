﻿using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Restrictions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
	public class ConditionalEnd : ServerSubeffect
	{
		public const string NoneFitRestriction = "None Fit Restriction";
		public const string AnyFitRestriction = "Any Fit Restriction";
		public const string NumberOfCardsFittingRestrictionFitsNumberRestriction =
			"Number of Cards Fitting CardRestriction Fits NumberRestriction";

		public const string NoSpaceFitsRestriction = "No Space Fits Restriction";

		public const string MustBeFriendlyTurn = "Must be Friendly Turn";
		public const string MustBeEnemyTurn = "Must be Enemy Turn";
		public const string TargetViolatesRestriction = "Target Violates Restriction";
		public const string TargetFitsRestriction = "Target Fits Restriction";

		public const string SpaceTargetViolatesRestriction = "Space Target Violates Restriction";
		public const string SpaceTargetFitsRestriction = "Space Target Fits Restriction";

		public const string SourceViolatesRestriction = "Source Violates Restriction";
		public const string NumTargetsLTEConstant = "Number Targets <= Constant";
		public const string HandFull = "Hand Full";
		public const string PlayerValueFitsNumberRestriction = "Player Value Fits Number Restriction";
		public const string PlayerValueFloutsNumberRestriction = "Player Value Flouts Number Restriction";

		public int constant = 0;
		public IRestriction<GameCardBase> cardRestriction;
		public IRestriction<Space> spaceRestriction;
		public IRestriction<int> numberRestriction;

		public PlayerValue playerValue;
		public IRestriction<int> playerValueNumberRestriction;

		public string condition;

		public IGamestateRestriction endIfTrue;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);

			cardRestriction?.Initialize(DefaultInitializationContext);
			spaceRestriction?.Initialize(DefaultInitializationContext);
			numberRestriction?.Initialize(DefaultInitializationContext);
			playerValueNumberRestriction?.Initialize(DefaultInitializationContext);
		}

		private bool doesNumberOfCardsFittingRestrictionFitNumberRestriction()
		{
			int number = ServerGame.Cards.Where(c => cardRestriction.IsValid(c, ResolutionContext)).Count();
			return numberRestriction.IsValid(number, ResolutionContext);
		}

		private bool ShouldEnd
		{
			get
			{
				if (endIfTrue != null) return endIfTrue.IsValid(ResolutionContext);

				if (condition == null) throw new ArgumentNullException(nameof(condition));
				return condition switch
				{
					NoneFitRestriction => !ServerGame.Cards.Any(c => cardRestriction.IsValid(c, ResolutionContext)),
					AnyFitRestriction => ServerGame.Cards.Any(c => cardRestriction.IsValid(c, ResolutionContext)),
					NumberOfCardsFittingRestrictionFitsNumberRestriction => doesNumberOfCardsFittingRestrictionFitNumberRestriction(),

					NoSpaceFitsRestriction => !Space.Spaces.Any(s => spaceRestriction.IsValid(s, ResolutionContext)),

					MustBeFriendlyTurn => ServerGame.TurnPlayer != Effect.Controller,
					MustBeEnemyTurn => ServerGame.TurnPlayer == Effect.Controller,

					TargetViolatesRestriction => !cardRestriction.IsValid(CardTarget, ResolutionContext),
					TargetFitsRestriction => cardRestriction.IsValid(CardTarget, ResolutionContext),

					SpaceTargetViolatesRestriction => !spaceRestriction.IsValid(SpaceTarget, ResolutionContext),
					SpaceTargetFitsRestriction => spaceRestriction.IsValid(SpaceTarget, ResolutionContext),

					SourceViolatesRestriction => !cardRestriction.IsValid(Source, ResolutionContext),
					NumTargetsLTEConstant => Effect.CardTargets.Count() <= constant,
					HandFull => PlayerTarget.HandFull,
					PlayerValueFitsNumberRestriction => playerValueNumberRestriction.IsValid(playerValue.GetValueOf(PlayerTarget), ResolutionContext),
					PlayerValueFloutsNumberRestriction => !playerValueNumberRestriction.IsValid(playerValue.GetValueOf(PlayerTarget), ResolutionContext),
					_ => throw new System.ArgumentException($"Condition {condition} invalid for conditional end subeffect"),
				};
			}
		}

		public override Task<ResolutionInfo> Resolve()
		{
			if (ShouldEnd) return Task.FromResult(ResolutionInfo.End(condition));
			else return Task.FromResult(ResolutionInfo.Next);
		}
	}
}