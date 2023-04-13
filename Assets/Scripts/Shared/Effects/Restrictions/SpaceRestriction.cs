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

        public string blurb = "";

        //TODO correct any places still using mustBeEmpty

        public Func<Space, bool> AsThroughPredicate(IResolutionContext context)
            => s => IsValid(s, context);

        public Func<Space, bool> IsValidFor(IResolutionContext context) => s => IsValid(s, context);
    }
}