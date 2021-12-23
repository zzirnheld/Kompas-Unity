using KompasCore.Cards;
using System.Collections.Generic;

namespace KompasCore.Effects
{
    public class ActivationContext
    {
        // Information about the relevant triggering situation
        public readonly IGameCardInfo mainCardInfoBefore;
        public readonly IGameCardInfo secondaryCardInfoBefore;
        public readonly IStackable stackable;
        public readonly Player player;
        public readonly int? x;
        public readonly Space space;

        // Used for resuming delayed effects
        public readonly int startIndex;
        public readonly List<GameCard> targets;
        public readonly List<Space> spaces;

        /// <summary>
        /// The information for the main triggering card immediately after the triggering event occurred.
        /// </summary>
        public IGameCardInfo MainCardInfoAfter { get; private set; }

        /// <summary>
        /// The information for the secondary triggering card immediately after the triggering event occurred.
        /// The secondary card could be the defender in an attacks trigger, etc.
        /// </summary>
        public IGameCardInfo SecondaryCardInfoAfter { get; private set; }

        public ActivationContext(GameCard mainCardBefore = null,
                                 GameCard secondaryCardBefore = null,
                                 IStackable stackable = null,
                                 Player player = null,
                                 int? x = null,
                                 Space space = null,
                                 //Used for resuming delayed effects
                                 int startIndex = 0,
                                 List<GameCard> targets = null,
                                 List<Space> spaces = null)
        {
            mainCardInfoBefore = GameCardInfo.CardInfoOf(mainCardBefore);
            secondaryCardInfoBefore = GameCardInfo.CardInfoOf(secondaryCardBefore);
            this.stackable = stackable;
            this.player = player;
            this.x = x;
            this.space = space?.Copy;

            this.startIndex = startIndex;
            this.targets = targets ?? new List<GameCard>();
            this.spaces = spaces ?? new List<Space>();
        }

        /// <summary>
        /// Caches the state of the card(s) relevant to the effect immediately after the triggering event occurred
        /// </summary>
        /// <param name="mainCard">The main card whose information to cache</param>
        /// <param name="secondaryCard">The secondary card whose information to cache, if any 
        /// (like the attacker on a defends proc)</param>
        public void CacheCardInfoAfter(GameCard mainCard, GameCard secondaryCard = null)
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

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();

            sb.Append(mainCardInfoBefore == null ? "No triggering card, " : $"Card: {mainCardInfoBefore.CardName}, ");
            sb.Append(stackable == null ? "No triggering stackable, " : $"Stackable {stackable}, ");
            sb.Append(player == null ? "No triggering player, " : $"Triggerer: {player.index}, ");
            sb.Append(x == null ? "No X, " : $"X: {x}, ");
            sb.Append(space == null ? "No space, " : $"Space: {space}, ");
            sb.Append(targets == null ? "No targets, " : $"Targets: {string.Join(", ", targets)}, ");
            sb.Append($"Starting at {startIndex}");

            return sb.ToString();
        }
    }
}