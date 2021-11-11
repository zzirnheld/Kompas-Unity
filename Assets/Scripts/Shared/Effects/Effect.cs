using KompasCore.Cards;
using KompasCore.GameCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace KompasCore.Effects
{
    /// <summary>
    /// Effects will only be resolved on server. Clients will just get to know what effects they can use
    /// </summary>
    public abstract class Effect : IStackable
    {
        public abstract Game Game { get; }

        public int EffectIndex { get; private set; }
        public GameCard Source { get; private set; }
        public abstract Player Controller { get; set; }

        //subeffects
        public abstract Subeffect[] Subeffects { get; }
        //current subeffect that's resolving
        public int SubeffectIndex { get; protected set; }
        public Subeffect CurrSubeffect => Subeffects[SubeffectIndex];

        //Targets
        protected readonly List<GameCard> targetsList = new List<GameCard>();
        public IEnumerable<GameCard> Targets => targetsList;
        protected readonly List<Space> coords = new List<Space>();
        public readonly List<Player> players = new List<Player>();
        public readonly List<GameCard> rest = new List<GameCard>();
        /// <summary>
        /// X value for card effect text (not coordinates)
        /// </summary>
        public int X = 0;

        //Triggering and Activating
        public TriggerData triggerData;
        public abstract Trigger Trigger { get; }
        public ActivationRestriction activationRestriction;
        public string blurb;

        public ActivationContext CurrActivationContext { get; protected set; }
        public int TimesUsedThisTurn { get; protected set; }
        public int TimesUsedThisRound { get; protected set; }
        public int TimesUsedThisStack { get; set; }

        public virtual bool Negated { get; set; }

        protected void SetInfo(GameCard source, int effIndex, Player owner)
        {
            Source = source != null ? source : throw new ArgumentNullException("source", "Effect cannot be attached to null card");
            EffectIndex = effIndex;
            Controller = owner;

            blurb = string.IsNullOrEmpty(blurb) ? $"Effect of {source.CardName}" : blurb;
            activationRestriction?.Initialize(this);
            TimesUsedThisTurn = 0;
        }

        public void ResetForTurn(Player turnPlayer)
        {
            TimesUsedThisTurn = 0;
            if (turnPlayer == Source.Controller) TimesUsedThisRound = 0;
        }

        public void Reset()
        {
            TimesUsedThisRound = 0;
            TimesUsedThisTurn = 0;
        }

        public virtual void AddTarget(GameCard card) => targetsList.Add(card);
        public virtual void RemoveTarget(GameCard card) => targetsList.Remove(card);

        public virtual bool CanBeActivatedBy(Player controller)
            => Trigger == null && activationRestriction != null && activationRestriction.Evaluate(controller);

        public virtual bool CanBeActivatedAtAllBy(Player activator)
            => Trigger == null && activationRestriction != null && activationRestriction.EvaluateAtAll(activator);

        public GameCard GetTarget(int num)
        {
            int trueIndex = num < 0 ? num + targetsList.Count() : num;
            return trueIndex < 0 ? null : targetsList[trueIndex];
        }

        public Space GetSpace(int num)
        {
            var trueIndex = num < 0 ? num + coords.Count() : num;
            return trueIndex < 0 ? null : coords[trueIndex];
        }

        public Player GetPlayer(int num)
        {
            int trueIndex = num < 0 ? num + players.Count() : num;
            return trueIndex < 0 ? null : players[trueIndex];
        }

        public void AddSpace(Space space)
        {
            coords.Add(space.Copy);
        }

        public bool AnyCoords() => coords.Any();

        public IEnumerable<T> SelectCoords<T>(Func<Space, T> lambda) => coords.Select(lambda);
    }
}