using KompasCore.Cards;
using KompasCore.GameCore;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects
{
    /// <summary>
    /// Effects will only be resolved on server. Clients will just get to know what effects they can use
    /// </summary>
    [System.Serializable]
    public abstract class Effect : IStackable
    {
        public Game Game => Source.Game;

        public int EffectIndex { get; private set; }
        public GameCard Source { get; private set; }
        public abstract Player Controller { get; set; }

        //subeffects
        public abstract Subeffect[] Subeffects { get; }
        //current subeffect that's resolving
        public int SubeffectIndex { get; protected set; }
        public Subeffect CurrSubeffect => Subeffects[SubeffectIndex];

        //Targets
        protected List<GameCard> TargetsList { get; } = new List<GameCard>();
        public IEnumerable<GameCard> Targets => TargetsList;
        public List<(int x, int y)> Coords { get; private set; } = new List<(int x, int y)>();
        public List<GameCard> Rest { get; private set; } = new List<GameCard>();
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

        private int negations = 0;
        public bool Negated
        {
            get => negations > 0;
            set
            {
                if (value) negations++;
                else negations--;
            }
        }

        protected void SetInfo(GameCard source, int effIndex, Player owner)
        {
            Source = source != null ? source : throw new System.ArgumentNullException("source", "Effect cannot be attached to null card");
            Controller = owner;
            activationRestriction?.Initialize(this);
            EffectIndex = effIndex;
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

        public virtual void Negate() => Negated = true;

        public virtual void AddTarget(GameCard card) => TargetsList.Add(card);
        public virtual void RemoveTarget(GameCard card) => TargetsList.Remove(card);

        public virtual bool CanBeActivatedBy(Player controller)
            => Trigger == null && activationRestriction.Evaluate(controller);

        public virtual bool CanBeActivatedAtAllBy(Player activator)
            => Trigger == null && activationRestriction.EvaluateAtAll(activator);

        public abstract void StartResolution(ActivationContext context);

        public GameCard GetTarget(int num)
        {
            int trueIndex = num < 0 ? num + TargetsList.Count : num;
            return trueIndex < 0 ? null : TargetsList[trueIndex];
        }

        public (int x, int y) GetSpace(int num)
        {
            var trueIndex = num < 0 ? num + Coords.Count : num;
            return trueIndex < 0 ? (0, 0) : Coords[trueIndex];
        }
    }
}