using KompasCore.Cards;
using System.Collections.Generic;

namespace KompasCore.Effects
{
    public class ActivationContext
    {
        // Information about the relevant triggering situation
        public readonly GameCardInfo mainCardInfoBefore;
        public readonly GameCardInfo secondaryCardInfoBefore;
        public readonly IStackable stackable;
        public readonly Player player;
        public readonly int? x;
        public readonly Space space;

        // Used for resuming delayed effects
        public int StartIndex { get; private set; }
        public List<GameCard> Targets { get; private set; }
        public List<Space> Spaces { get; private set; }

        public ActivationContext Copy 
            => new ActivationContext(mainCardInfoBefore, MainCardInfoAfter, stackable, player, x, space);

        /// <summary>
        /// The information for the main triggering card immediately after the triggering event occurred.
        /// </summary>
        public GameCardInfo MainCardInfoAfter { get; private set; }

        /// <summary>
        /// The information for the secondary triggering card immediately after the triggering event occurred.
        /// The secondary card could be the defender in an attacks trigger, etc.
        /// </summary>
        public GameCardInfo SecondaryCardInfoAfter { get; private set; }

        private ActivationContext(GameCardInfo mainCardInfoBefore,
                                  GameCardInfo secondaryCardInfoBefore,
                                  IStackable stackable,
                                  Player player,
                                  int? x,
                                  Space space)
        {
            this.mainCardInfoBefore = mainCardInfoBefore;
            this.secondaryCardInfoBefore = secondaryCardInfoBefore;
            this.stackable = stackable;
            this.player = player;
            this.x = x;
            this.space = space;
        }

        public ActivationContext(GameCard mainCardBefore = null,
                                 GameCard secondaryCardBefore = null,
                                 IStackable stackable = null,
                                 Player player = null,
                                 int? x = null,
                                 Space space = null)
            : this(GameCardInfo.CardInfoOf(mainCardBefore),
                   GameCardInfo.CardInfoOf(secondaryCardBefore),
                   stackable,
                   player,
                   x,
                   space?.Copy)
        { }

        ~ActivationContext()
        {
            if (mainCardInfoBefore != null) UnityEngine.Object.Destroy(mainCardInfoBefore);
            if (MainCardInfoAfter != null) UnityEngine.Object.Destroy(mainCardInfoBefore);

            if (secondaryCardInfoBefore != null) UnityEngine.Object.Destroy(mainCardInfoBefore);
            if (SecondaryCardInfoAfter != null) UnityEngine.Object.Destroy(mainCardInfoBefore);
        }

        /// <summary>
        /// Set any information relevant to resuming an effect's resolution
        /// </summary>
        /// <param name="startIndex">The index at which to start resolving the effect (again)</param>
        /// <param name="targets">The targets to resume with, if any</param>
        /// <param name="spaces">The spaces to resume with, if any</param>
        public void SetResumeInfo(IEnumerable<GameCard> targets, IEnumerable<Space> spaces, int? startIndex = null)
        {
            if (startIndex.HasValue) StartIndex = startIndex.Value;
            Targets = new List<GameCard>(targets);
            Spaces = new List<Space>(spaces);
        }

        /// <summary>
        /// Caches the state of the card(s) relevant to the effect immediately after the triggering event occurred
        /// </summary>
        /// <param name="mainCard">The main card whose information to cache</param>
        /// <param name="secondaryCard">The secondary card whose information to cache, if any 
        /// (like the attacker on a defends proc)</param>
        private void CacheCardInfoAfter(GameCard mainCard, GameCard secondaryCard = null)
        {
            if (MainCardInfoAfter != null) 
                throw new System.ArgumentException("Already initialized MainCardInfoAfter on this context");
            else if (mainCardInfoBefore == null) 
                throw new System.ArgumentNullException("Never stored mainCardInfoBefore, why are you creating MainCardInfoAfter?");
            else 
                MainCardInfoAfter = GameCardInfo.CardInfoOf(mainCard);

            if (secondaryCard != null)
            {
                if (SecondaryCardInfoAfter != null)
                    throw new System.ArgumentException("Already initialized SecondaryCardInfoAfter on this context");
                else if (secondaryCardInfoBefore == null)
                    throw new System.ArgumentNullException("Never stored secondaryCardInfoBefore, why are you creating SecondaryCardInfoAfter?");
                else
                    SecondaryCardInfoAfter = GameCardInfo.CardInfoOf(mainCard);
            }
        }

        public void CacheCardInfoAfter() => CacheCardInfoAfter(mainCardInfoBefore.Card, secondaryCardInfoBefore?.Card);

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();

            sb.Append(mainCardInfoBefore == null ? "No triggering card, " : $"Card: {mainCardInfoBefore.CardName}, ");
            sb.Append(stackable == null ? "No triggering stackable, " : $"Stackable {stackable}, ");
            sb.Append(player == null ? "No triggering player, " : $"Triggerer: {player.index}, ");
            sb.Append(x == null ? "No X, " : $"X: {x}, ");
            sb.Append(space == null ? "No space, " : $"Space: {space}, ");
            sb.Append(Targets == null ? "No targets, " : $"Targets: {string.Join(", ", Targets)}, ");
            sb.Append($"Starting at {StartIndex}");

            return sb.ToString();
        }
    }
}