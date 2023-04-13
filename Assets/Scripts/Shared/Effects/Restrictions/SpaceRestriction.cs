using KompasCore.Cards;
using KompasCore.Effects.Restrictions;
using KompasCore.Exceptions;
using KompasCore.GameCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    public class SpaceRestriction : RestrictionBase<Space>
    {
        public Subeffect Subeffect => InitializationContext.subeffect;
        public GameCard Source => InitializationContext.source;

        public string blurb = "";

        //TODO correct any places still using mustBeEmpty

        public List<SpaceRestrictionElement> spaceRestrictionElements = new List<SpaceRestrictionElement>();

        public Func<Space, bool> AsThroughPredicate(IResolutionContext context)
            => s => IsValidSpace(s, context);

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);

            foreach (var sre in spaceRestrictionElements)
            {
                sre.Initialize(initializationContext);
            }
        }

        public bool IsValidSpace(Space space, IResolutionContext context, GameCard theoreticalTarget = null)
        {
            ComplainIfNotInitialized();
            if (!space.IsValid) throw new InvalidSpaceException(space, "Invalid space to consider for restriction!");

            return spaceRestrictionElements.All(sre => sre.IsValid(space, context));
        }

        public Func<Space, bool> IsValidFor(IResolutionContext context) => s => IsValidSpace(s, context);

        public override string ToString()
            => $"Space restriction of card {Source} on subeff {Subeffect}, restrictions {string.Join(",", spaceRestrictions)}";
    }
}