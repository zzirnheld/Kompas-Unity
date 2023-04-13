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
        public Game Game => InitializationContext.game;
        public Player Controller => InitializationContext.Controller;
        public Effect Effect => InitializationContext.effect;

        #region space restrictions
        //adjacency
        public const string AdjacentToSource = "Adjacent to Source";
        #endregion space restrictions

        public string[] spaceRestrictions = { };
        public CardRestriction adjacencyRestriction;

        public string blurb = "";

        //TODO correct any places still using mustBeEmpty

        public List<SpaceRestrictionElement> spaceRestrictionElements = new List<SpaceRestrictionElement>();

        public Func<Space, bool> AsThroughPredicate(IResolutionContext context)
            => s => IsValidSpace(s, context);

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);

            adjacencyRestriction?.Initialize(initializationContext);

            foreach (var sre in spaceRestrictionElements)
            {
                sre.Initialize(initializationContext);
            }
        }

        /// <summary>
        /// Whether the given restriction is valid for the given space.
        /// </summary>
        /// <param name="restriction">The restriction to be evaluated</param>
        /// <param name="x">The x coordinate of the space</param>
        /// <param name="y">The y coordinate of the space</param>
        /// <param name="theoreticalTarget">If this space restriction is being considered with a theoretical additional target, this is it</param>
        /// <returns></returns>
        private bool IsRestrictionValid(string restriction, Space space, GameCard theoreticalTarget, IResolutionContext context)
        {
            var target = theoreticalTarget ?? Subeffect?.CardTarget;

            if (space == null)
            {
                Debug.LogError("Tried to check a null space in SpaceRestriction!");
                return false;
            }

            return restriction != null && restriction switch
            {
                //adjacency
                AdjacentToSource => Source.IsAdjacentTo(space),

                _ => throw new ArgumentException($"Invalid space restriction {restriction}", "restriction"),
            };
        }

        private bool IsRestrictionValidWithDebug(string r, Space space, GameCard theoreticalTarget, IResolutionContext context)
        {
            try
            {
                bool success = IsRestrictionValid(r, space, theoreticalTarget, context);
                //if (!success) Debug.Log($"Space resetriction {r} was flouted by {space}");
                return success;
            }
            catch (NullReferenceException nre)
            {
                Debug.Log($"Space resetriction {r} was flouted by {space} throwing exception {nre}. Init context was {InitializationContext}");
                return false;
            }
        }

        public bool IsValidSpace(Space space, IResolutionContext context, GameCard theoreticalTarget = null)
        {
            ComplainIfNotInitialized();
            if (!space.IsValid) throw new InvalidSpaceException(space, "Invalid space to consider for restriction!");

            return spaceRestrictions.All(r => IsRestrictionValidWithDebug(r, space, theoreticalTarget, context))
                && spaceRestrictionElements.All(sre => sre.IsValid(space, context));
        }

        public Func<Space, bool> IsValidFor(IResolutionContext context) => s => IsValidSpace(s, context);

        public override string ToString()
            => $"Space restriction of card {Source} on subeff {Subeffect}, restrictions {string.Join(",", spaceRestrictions)}";
    }
}