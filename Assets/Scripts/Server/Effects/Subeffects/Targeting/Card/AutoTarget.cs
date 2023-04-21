﻿using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.ManyCards;
using KompasCore.Effects.Restrictions.CardRestrictionElements;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects.Subeffects
{
    public class AutoTarget : ServerSubeffect
    {
        public const string Maximum = "Maximum";
        public const string Any = "Any";
        public const string RandomCard = "Random";

        public IIdentity<IReadOnlyCollection<GameCardBase>> toSearch = new All();
        public IRestriction<GameCardBase> cardRestriction;
        public CardValue tiebreakerValue;
        public string tiebreakerDirection;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            toSearch.Initialize(DefaultInitializationContext);
            cardRestriction ??= new AlwaysValid();
            cardRestriction.Initialize(DefaultInitializationContext);
            tiebreakerValue?.Initialize(DefaultInitializationContext);
        }

        public override bool IsImpossible() => !Game.Cards.Any(c => cardRestriction.IsValid(c, ResolutionContext));

        private GameCard GetRandomCard(GameCard[] cards)
        {
            var random = new System.Random();
            return cards[random.Next(cards.Length)];
        }

        public override Task<ResolutionInfo> Resolve()
        {
            GameCard potentialTarget = null;
            IEnumerable<GameCard> potentialTargets = null;
            try
            {
                potentialTargets = toSearch.From(ResolutionContext, default)
                    .Where(c => cardRestriction.IsValid(c, ResolutionContext))
                    .Select(c => c.Card);
                potentialTarget = tiebreakerDirection switch
                {
                    Maximum => potentialTargets.OrderByDescending(tiebreakerValue.GetValueOf).First(),
                    Any => potentialTargets.First(),
                    RandomCard => GetRandomCard(potentialTargets.ToArray()),
                    _ => potentialTargets.Single(),
                };
            }
            catch (System.InvalidOperationException)
            {
                Debug.LogError($"More than one card fit the card restriction {cardRestriction} " +
                    $"for the effect {Effect.blurb} of {Source.CardName}. Those cards were {potentialTargets}");
                return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
            }

            ServerEffect.AddTarget(potentialTarget);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}