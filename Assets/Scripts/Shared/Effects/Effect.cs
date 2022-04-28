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
        /// <summary>
        /// Current subeffect that's resolving
        /// </summary>
        public int SubeffectIndex { get; protected set; }

        //Targets
        protected readonly List<GameCard> cardTargets = new List<GameCard>();
        protected readonly List<Space> spaceTargets = new List<Space>();
        //we don't care about informing players of the contents of these. yet. but we might later
        public readonly List<GameCardInfo> cardInfoTargets = new List<GameCardInfo>();
        public readonly List<Player> playerTargets = new List<Player>();
        public readonly List<IStackable> stackableTargets = new List<IStackable>();
        public readonly List<GameCard> rest = new List<GameCard>();

        public IEnumerable<GameCard> CardTargets => cardTargets;
        public IEnumerable<Space> SpaceTargets => spaceTargets;

        /// <summary>
        /// X value for card effect text (not coordinates)
        /// </summary>
        public int X = 0;

        //Triggering and Activating
        public abstract Trigger Trigger { get; }
        public TriggerData triggerData;
        public ActivationRestriction activationRestriction;

        //Misc effect info
        public string blurb;
        public int arg; //used for keyword arguments, and such

        public ActivationContext CurrActivationContext { get; protected set; }
        public int TimesUsedThisTurn { get; protected set; }
        public int TimesUsedThisRound { get; protected set; }
        public int TimesUsedThisStack { get; set; }

        public virtual bool Negated { get; set; }

        protected void SetInfo(GameCard source, int effIndex, Player owner)
        {
            Source = source ?? throw new ArgumentNullException("source", "Effect cannot be attached to null card");
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

        public virtual void AddTarget(GameCard card) => cardTargets.Add(card);
        public virtual void RemoveTarget(GameCard card) => cardTargets.Remove(card);

        public virtual bool CanBeActivatedBy(Player controller)
            => Trigger == null && activationRestriction != null && activationRestriction.IsValidActivation(controller);

        public virtual bool CanBeActivatedAtAllBy(Player activator)
            => Trigger == null && activationRestriction != null && activationRestriction.IsPotentiallyValidActivation(activator);

        public static T GetItem<T>(IEnumerable<T> enumerable, int index)
        {
            int trueIndex = index < 0 ? index + enumerable.Count() : index;
            return enumerable.ElementAtOrDefault(trueIndex);
        }

        public GameCard GetTarget(int num) => GetItem(cardTargets, num);

        public Space GetSpace(int num) => GetItem(spaceTargets, num);

        public Player GetPlayer(int num) => GetItem(playerTargets, num);

        public void AddSpace(Space space) => spaceTargets.Add(space.Copy);

        public override string ToString() => $"Effect of {(Source == null ? "Nothing???" : Source.CardName)}";
    }
}