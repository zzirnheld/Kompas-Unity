using KompasCore.Effects;
using KompasCore.GameCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Cards
{
    public abstract class GameCard : CardBase
    {
        public const string Nimbleness = "N";
        public const string Endurance = "E";
        public const string SummoningCost = "S";
        public const string Wounding = "W";
        public const string CastingCost = "C";
        public const string AugmentCost = "A";
        public const string CostStat = "Cost";

        public abstract Game Game { get; }
        public int ID { get; private set; }
        public CardController cardCtrl;

        private SerializableCard serializedCard;

        public bool CurrentlyVisible => gameObject.activeSelf;

        #region stats
        public int BaseN => serializedCard.n;
        public int BaseE => serializedCard.e;
        public int BaseS => serializedCard.s;
        public int BaseW => serializedCard.w;
        public int BaseC => serializedCard.c;
        public int BaseA => serializedCard.a;

        public override int N 
        { 
            get => base.N; 
            protected set => cardCtrl.N = base.N = value;
        }
        public override int E
        {
            get => base.E;
            protected set => cardCtrl.E = base.E = value;
        }
        public override int S
        {
            get => base.S;
            protected set => cardCtrl.S = base.S = value;
        }
        public override int W
        {
            get => base.W;
            protected set => cardCtrl.W = base.W = value;
        }
        public override int C
        {
            get => base.C;
            protected set => cardCtrl.C = base.C = value;
        }
        public override int A
        {
            get => base.A;
            protected set => cardCtrl.A = base.A = value;
        }

        public int Negations { get; private set; } = 0;
        public virtual bool Negated
        {
            get => Negations > 0;
            private set
            {
                if (value) Negations++;
                else Negations--;

                foreach (var e in Effects) e.Negated = value;
            }
        }
        public int Activations { get; private set; } = 0;
        public virtual bool Activated
        {
            get => Activations > 0;
            private set
            {
                if (value) Activations++;
                else Activations--;
            }
        }

        public virtual bool Summoned => CardType == 'C' && Location == CardLocation.Field;
        public virtual bool CanRemove => true;
        public virtual int CombatDamage => W;
        public (int n, int e, int s, int w) CharStats => (N, E, S, W);
        public (int n, int e, int s, int w, int c, int a) Stats => (N, E, S, W, C, A);
        #endregion stats

        #region positioning
        public int BoardX { get; protected set; }
        public int BoardY { get; protected set; }
        public (int x, int y) Position
        {
            get => (BoardX, BoardY);
            set
            {
                (BoardX, BoardY) = value;
                if(cardCtrl != null) cardCtrl.SetPhysicalLocation(Location);
                foreach (var aug in Augments) aug.Position = value;
            }
        }

        public (int x, int y) SubjectivePosition => Controller.SubjectiveCoords(Position);

        public int IndexInList
        {
            get
            {
                switch (Location)
                {
                    case CardLocation.Deck: return Controller.deckCtrl.IndexOf(this);
                    case CardLocation.Discard: return Controller.discardCtrl.IndexOf(this);
                    case CardLocation.Field: return BoardX * 7 + BoardY;
                    case CardLocation.Hand: return Controller.handCtrl.IndexOf(this);
                    case CardLocation.Annihilation: return Game.annihilationCtrl.Cards.IndexOf(this);
                    default:
                        Debug.LogError($"Tried to ask for card index when in location {Location}");
                        return -1;
                }
            }
        }
        public List<GameCard> AdjacentCards => Game.boardCtrl.CardsAdjacentTo(BoardX, BoardY);
        #endregion positioning

        #region Augments
        public List<GameCard> Augments { get; private set; } = new List<GameCard>();
        private GameCard augmentedCard;
        public GameCard AugmentedCard
        {
            get { return augmentedCard; }
            private set
            {
                augmentedCard = value;
                if (value != null)
                {
                    Position = value.Position;
                    Location = value.Location;
                }
            }
        }

        public bool Attached => AugmentedCard != null;
        #endregion

        #region effects
        public abstract IEnumerable<Effect> Effects { get; }
        /// <summary>
        /// Whether there is an effect that is ready to be activated right now
        /// </summary>
        public bool HasCurrentlyActivateableEffect => Effects != null && Effects.Count(e => e.CanBeActivatedBy(Controller)) > 0;
        /// <summary>
        /// Whether there is any effect that can still be activated (this turn, this round, etc.) 
        /// even if it can't be activated at this exact moment.
        /// </summary>
        public bool HasAtAllActivateableEffect => Effects != null && Effects.Count(e => e.CanBeActivatedAtAllBy(Controller)) > 0;
        #endregion effects

        //movement
        private int spacesMoved = 0;
        public int SpacesMoved => spacesMoved;
        public int SpacesCanMove => N - SpacesMoved;

        public int attacksThisTurn = 0;
        public int AttacksThisTurn => attacksThisTurn;

        //restrictions
        public MovementRestriction MovementRestriction { get; private set; }
        public AttackRestriction AttackRestriction { get; private set; }
        public PlayRestriction PlayRestriction { get; private set; }

        //controller/owners
        public abstract Player Controller { get; set; }
        public int ControllerIndex => Controller?.index ?? 0;
        public abstract Player Owner { get; }
        public int OwnerIndex => Owner.index;

        //misc
        private CardLocation location;
        public virtual CardLocation Location
        {
            get => location;
            set
            {
                Debug.Log($"Card {ID} location set to {value}");
                location = value;
                if (cardCtrl == null) Debug.LogWarning($"Missing a card control. Is this a debug card?");
                else cardCtrl.SetPhysicalLocation(location);
            }
        }

        public string BaseJson => Game.cardRepo.GetJsonFromName(CardName);

        /// <summary>
        /// Represents whether this card is currently known to the enemy of this player.
        /// TODO: make this also be accurate on client, remembering what thigns have been revealed
        /// </summary>
        public virtual bool KnownToEnemy => !Game.HiddenLocations.Contains(Location);
        public int TurnsOnBoard { get; private set; }
        public abstract bool IsAvatar { get; }

        public void SetInfo(SerializableCard serializedCard, int id)
        {
            base.SetInfo(serializedCard); //base is redundant but adds clarity

            this.ID = id;
            this.serializedCard = serializedCard;

            MovementRestriction = serializedCard.MovementRestriction ?? new MovementRestriction();
            MovementRestriction.SetInfo(this);
            AttackRestriction = serializedCard.AttackRestriction ?? new AttackRestriction();
            AttackRestriction.SetInfo(this);
            PlayRestriction = serializedCard.PlayRestriction ?? new PlayRestriction();
            PlayRestriction.SetInfo(this);

            cardCtrl.ShowForCardType(CardType, false);

            Debug.Log($"Finished setting up info for card {CardName}");
        }

        /// <summary>
        /// Resets any of the card's values that might be different from their originals.
        /// Should be called when cards move out the discard, or into the hand, deck, or annihilation
        /// </summary>
        public virtual void ResetCard()
        {
            if(serializedCard == null)
            {
                Debug.Log("Tried to reset card whose info was never set! This should only happen at game start");
                return;
            }

            base.SetInfo(serializedCard); //base is redundant but adds clarity

            TurnsOnBoard = 0;
            SetSpacesMoved(0, true);
            SetAttacksThisTurn(0, true);

            if (Effects != null) foreach (var eff in Effects) eff.Reset();
            //instead of setting negations or activations to 0, so that it updates the client correctly
            while (Negated) Negated = false;
            while (Activated) Activated = false;
        }

        /// <summary>
        /// Resets anything that needs to be reset for the start of the turn.
        /// </summary>
        public virtual void ResetForTurn(Player turnPlayer)
        {
            foreach (Effect eff in Effects)
            {
                eff.ResetForTurn(turnPlayer);
            }

            SetSpacesMoved(0, true);
            SetAttacksThisTurn(0, true);
            if (Location == CardLocation.Field) TurnsOnBoard++;
        }

        public void ResetForStack()
        {
            foreach (var e in Effects) e.TimesUsedThisStack = 0;
        }

        #region distance/adjacency
        public int DistanceTo(int x, int y)
        {
            if (Location != CardLocation.Field) return int.MaxValue;
            return Mathf.Abs(x - BoardX) > Mathf.Abs(y - BoardY) ? Mathf.Abs(x - BoardX) : Mathf.Abs(y - BoardY);
            /* equivalent to
             * if (Mathf.Abs(card.X - X) > Mathf.Abs(card.Y - Y)) return Mathf.Abs(card.X - X);
             * else return Mathf.Abs(card.Y - Y);
             * is card.X - X > card.Y - Y? If so, return card.X -X, otherwise return card.Y - Y
            */
        }
        public int DistanceTo((int x, int y) space) => DistanceTo(space.x, space.y);
        public int DistanceTo(GameCard card) => DistanceTo(card.BoardX, card.BoardY);
        public bool WithinSpaces(int numSpaces, GameCard card)
            => card != null && card.Location == CardLocation.Field && Location == CardLocation.Field && DistanceTo(card) <= numSpaces;
        public bool WithinSpaces(int numSpaces, int x, int y) => DistanceTo(x, y) <= numSpaces;
        public bool IsAdjacentTo(GameCard card) => Location == CardLocation.Field && card != null 
            && card.Location == CardLocation.Field && DistanceTo(card) == 1;
        public bool IsAdjacentTo(int x, int y) => Location == CardLocation.Field && DistanceTo(x, y) == 1;
        public bool CardInAOE(GameCard c) => SpaceInAOE(c.Position);
        public bool SpaceInAOE((int x, int y) space) => SpaceInAOE(space.x, space.y);
        public bool SpaceInAOE(int x, int y)
            => CardType == 'S' && SpellSubtype == RadialSubtype && DistanceTo(x, y) <= Arg;
        public bool SameColumn(int x, int y) => BoardX - BoardY == x - y;
        public bool SameColumn(GameCard c) => c.Location == CardLocation.Field && SameColumn(c.BoardX, c.BoardY);

        /// <summary>
        /// Returns whether the <paramref name="space"/> passed in is in front of this card
        /// </summary>
        /// <param name="space">The space to check if it's in front of this card</param>
        /// <returns><see langword="true"/> if <paramref name="space"/> is in front of this, <see langword="false"/> otherwise.</returns>
        public bool SpaceInFront((int x, int y) space) => Controller.SubjectiveCoord(space.x) > SubjectivePosition.x;

        /// <summary>
        /// Returns whether the card passed in is in front of this card
        /// </summary>
        /// <param name="card">The card to check if it's in front of this one</param>
        /// <returns><see langword="true"/> if <paramref name="card"/> is in front of this, <see langword="false"/> otherwise.</returns>
        public bool CardInFront(GameCard card) => SpaceInFront(card.Position);

        /// <summary>
        /// Returns whether the <paramref name="space"/> passed in is behind this card
        /// </summary>
        /// <param name="space">The space to check if it's behind this card</param>
        /// <returns><see langword="true"/> if <paramref name="space"/> is behind this, <see langword="false"/> otherwise.</returns>
        public bool SpaceBehind((int x, int y) space) => Controller.SubjectiveCoord(space.x) < SubjectivePosition.x;

        /// <summary>
        /// Returns whether the card passed in is behind this card
        /// </summary>
        /// <param name="card">The card to check if it's behind this one</param>
        /// <returns><see langword="true"/> if <paramref name="card"/> is behind this, <see langword="false"/> otherwise.</returns>
        public bool CardBehind(GameCard card) => SpaceBehind(card.Position);

        public bool SpaceDirectlyInFront((int x, int y) space)
            => Controller.SubjectiveCoords(space) == (SubjectivePosition.x + 1, SubjectivePosition.y + 1);

        public bool CardDirectlyInFront(GameCard card) 
            => Location == CardLocation.Field && card.Location == CardLocation.Field && SpaceDirectlyInFront(card.Position);

        public bool OnMyDiagonal((int x, int y) space) => Location == CardLocation.Field && (BoardX == space.x || BoardY == space.y);

        /// <summary>
        /// Refers to this situation: <br></br>
        /// | <paramref name="space"/> | <br></br>
        /// | this card | <br></br>
        /// | <paramref name="card"/> param | <br></br>
        /// </summary>
        /// <param name="space">The space in the same axis as this card and <paramref name="card"/> param</param>
        /// <param name="card">The card in the same axis as this card and the <paramref name="space"/> param.</param>
        /// <returns></returns>
        public bool SpaceDirectlyAwayFrom((int x, int y) space, GameCard card)
        {
            if (card.Location != CardLocation.Field || Location != CardLocation.Field) return false;
            int xDiffCard = card.BoardX - BoardX;
            int yDiffCard = card.BoardY - BoardY;
            int xDiffSpace = space.x - BoardX;
            int yDiffSpace = space.y - BoardY;

            return (xDiffCard == 0 && xDiffSpace == 0)
                || (yDiffCard == 0 && yDiffSpace == 0)
                || (xDiffCard == yDiffCard && xDiffSpace == yDiffSpace);
        }
        #endregion distance/adjacency

        public void PutBack()
        {
            if(cardCtrl != null) cardCtrl.SetPhysicalLocation(Location);
        }

        public void CountSpacesMovedTo((int x, int y) to) => SetSpacesMoved(spacesMoved + DistanceTo(to.x, to.y));

        #region augments
        public virtual bool AddAugment(GameCard augment, IStackable stackSrc = null)
        {
            //can't add a null augment
            if (augment == null) return false;

            //if this and the other are in the same place, it doesn't leave play
            bool canHappen = false;
            if (augment.Location != Location) canHappen = augment.Remove(stackSrc);
            else if (augment.AugmentedCard != null) canHappen = augment.Detach(stackSrc);

            if (!canHappen) return false;

            //regardless, add the augment
            Augments.Add(augment);

            //and update the augment's augmented card, location, and position to reflect its new status
            augment.AugmentedCard = this;
            augment.Location = Location;
            augment.Position = Position;

            return true;
        }

        protected virtual bool Detach(IStackable stackSrc = null)
        {
            if (AugmentedCard == null) return false;

            AugmentedCard.Augments.Remove(this);
            AugmentedCard = null;
            return true;
        }
        #endregion augments

        #region statfuncs
        public int GetStat(string stat)
        {
            switch (stat)
            {
                case Nimbleness: return N;
                case Endurance: return E;
                case SummoningCost: return S;
                case Wounding: return W;
                case CastingCost: return C;
                case AugmentCost: return A;
                case CostStat: return Cost;
                default: throw new System.ArgumentException($"I'm sorry, but {stat} is not a valid stat you stunted mongoose!", stat);
            }
        }

        /* This must happen through setters, not properties, so that notifications and stack sending
         * can be managed as intended. */
        public virtual void SetN(int n, IStackable stackSrc = null, bool notify = true) => N = n;
        public virtual void SetE(int e, IStackable stackSrc = null, bool notify = true) => E = e;
        public virtual void SetS(int s, IStackable stackSrc = null, bool notify = true) => S = s;
        public virtual void SetW(int w, IStackable stackSrc = null, bool notify = true) => W = w;
        public virtual void SetC(int c, IStackable stackSrc = null, bool notify = true) => C = c;
        public virtual void SetA(int a, IStackable stackSrc = null, bool notify = true) => A = a;

        /// <summary>
        /// Shorthand for modifying a card's NESW all at once.
        /// On the server, this only notifies the clients of stat changes once.
        /// </summary>
        public virtual void SetCharStats(int n, int e, int s, int w, IStackable stackSrc = null)
        {
            SetN(n, stackSrc, notify: false);
            SetE(e, stackSrc, notify: false);
            SetS(s, stackSrc, notify: false);
            SetW(w, stackSrc, notify: false);
        }

        /// <summary>
        /// Shorthand for modifying a card's NESW all at once.
        /// On the server, this only notifies the clients of stat changes once.
        /// </summary>
        public void AddToCharStats(int n, int e, int s, int w, IStackable stackSrc = null)
            => SetCharStats(N + n, E + e, S + s, W + w, stackSrc: stackSrc);

        /// <summary>
        /// Shorthand for modifying a card's stats all at once.
        /// On the server, this only notifies the clients of stat changes once.
        /// </summary>
        public virtual void SetStats((int n, int e, int s, int w, int c, int a) stats, IStackable stackSrc = null)
        {
            SetN(stats.n, stackSrc, notify: false);
            SetE(stats.e, stackSrc, notify: false);
            SetS(stats.s, stackSrc, notify: false);
            SetW(stats.w, stackSrc, notify: false);
            SetC(stats.c, stackSrc, notify: false);
            SetA(stats.a, stackSrc, notify: false);
        }

        /// <summary>
        /// Shorthand for modifying a card's stats all at once.
        /// On the server, this only notifies the clients of stat changes once.
        /// </summary>
        public void AddToStats((int n, int e, int s, int w, int c, int a) stats, IStackable stackSrc = null)
            => SetStats((N + stats.n, E + stats.e, S + stats.s, W + stats.w, C + stats.c, A + stats.a), stackSrc);

        public void SwapCharStats(GameCard other, bool swapN = true, bool swapE = true, bool swapS = true, bool swapW = true)
        {
            int[] aNewStats = new int[4];
            int[] bNewStats = new int[4];

            (aNewStats[0], bNewStats[0]) = swapN ? (other.N, N) : (N, other.N);
            (aNewStats[1], bNewStats[1]) = swapE ? (other.E, E) : (E, other.E);
            (aNewStats[2], bNewStats[2]) = swapS ? (other.S, S) : (S, other.S);
            (aNewStats[3], bNewStats[3]) = swapW ? (other.W, W) : (W, other.W);

            SetCharStats(aNewStats[0], aNewStats[1], aNewStats[2], aNewStats[3]);
            other.SetCharStats(bNewStats[0], bNewStats[1], bNewStats[2], bNewStats[3]);
        }

        public virtual void SetNegated(bool negated, IStackable stackSrc = null) => Negated = negated;
        public virtual void SetActivated(bool activated, IStackable stackSrc = null) => Activated = activated;

        public virtual void SetSpacesMoved(int spacesMoved, bool fromReset = false) => this.spacesMoved = spacesMoved;
        public virtual void SetAttacksThisTurn(int attacksThisTurn, bool fromReset = false) => this.attacksThisTurn = attacksThisTurn;
        #endregion statfuncs

        #region moveCard
        //so that notify stuff can be sent in the server
        public virtual bool Remove(IStackable stackSrc = null)
        {
            // Debug.Log($"Removing {CardName} id {ID} from {Location}");

            switch (Location)
            {
                case CardLocation.Nowhere: return true;
                case CardLocation.Field:
                    if (AugmentedCard != null) return Detach(stackSrc);
                    else return Game.boardCtrl.RemoveFromBoard(this);
                case CardLocation.Discard:
                    return Controller.discardCtrl.RemoveFromDiscard(this);
                case CardLocation.Hand:
                    return Controller.handCtrl.RemoveFromHand(this);
                case CardLocation.Deck:
                    return Controller.deckCtrl.RemoveFromDeck(this);
                case CardLocation.Annihilation:
                    return Game.annihilationCtrl.Remove(this);
                default:
                    Debug.LogWarning($"Tried to remove card {CardName} from invalid location {Location}");
                    return false;
            }
        }

        public bool Discard(IStackable stackSrc = null) => Controller.discardCtrl.AddToDiscard(this, stackSrc);

        public bool Rehand(Player controller, IStackable stackSrc = null) => controller.handCtrl.AddToHand(this, stackSrc);
        public bool Rehand(IStackable stackSrc = null) => Rehand(Controller, stackSrc);

        public bool Reshuffle(Player controller, IStackable stackSrc = null) => controller.deckCtrl.ShuffleIn(this, stackSrc);
        public bool Reshuffle(IStackable stackSrc = null) => Reshuffle(Controller, stackSrc);

        public bool Topdeck(Player controller, IStackable stackSrc = null) => controller.deckCtrl.PushTopdeck(this, stackSrc);
        public bool Topdeck(IStackable stackSrc = null) => Topdeck(Controller, stackSrc);

        public bool Bottomdeck(Player controller, IStackable stackSrc = null) => controller.deckCtrl.PushBottomdeck(this, stackSrc);
        public bool Bottomdeck(IStackable stackSrc = null) => Bottomdeck(Controller, stackSrc);

        public bool Play(int toX, int toY, Player controller, IStackable stackSrc = null, bool payCost = false)
        {
            if (Game.boardCtrl.Play(this, toX, toY, controller))
            {
                if (payCost) controller.Pips -= Cost;
                return true;
            }
            return false;
        }

        public bool Move(int toX, int toY, bool normalMove, IStackable stackSrc = null)
            => Game.boardCtrl.Move(this, toX, toY, normalMove, stackSrc);

        public void Dispel(IStackable stackSrc = null)
        {
            SetNegated(true, stackSrc);
            Discard(stackSrc);
        }
        #endregion moveCard
    }
}