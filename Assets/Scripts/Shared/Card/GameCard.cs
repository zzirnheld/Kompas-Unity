using KompasCore.Effects;
using KompasCore.GameCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Cards
{
    public abstract class GameCard : CardBase
    {
        public CardController cardCtrl;

        private SerializableCard serializedCard;

        #region stats
        public int BaseN => serializedCard.n;
        public int BaseE => serializedCard.e;
        public int BaseS => serializedCard.s;
        public int BaseW => serializedCard.w;
        public int BaseC => serializedCard.c;
        public int BaseA => serializedCard.a;

        private int n;
        private int e;
        private int s;
        private int w;
        private int c;
        private int a;
        public int N 
        { 
            get => n < 0 ? 0 : n;
            private set
            {
                n = value;
                cardCtrl.N = value;
            }
        }
        public int E
        {
            get => e < 0 ? 0 : e;
            protected set
            {
                e = value;
                cardCtrl.E = value;
            }
        }
        public int S
        {
            get => s < 0 ? 0 : s;
            private set
            {
                s = value;
                cardCtrl.S = value;
            }
        }
        public int W
        {
            get => w < 0 ? 0 : w;
            private set
            {
                w = value;
                cardCtrl.W = value;
            }
        }
        public int C
        {
            get => c < 0 ? 0 : c;
            private set
            {
                c = value;
                cardCtrl.C = value;
            }
        }
        public int A
        {
            get => a < 0 ? 0 : a;
            private set
            {
                a = value;
                cardCtrl.A = value;
            }
        }

        public bool Fast { get; private set; }

        public string Subtext { get; private set; }
        public string SpellSubtype { get; private set; }
        public int Arg { get; private set; }

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

        public int Cost
        {
            get
            {
                switch (CardType)
                {
                    case 'C': return S;
                    case 'S': return C;
                    case 'A': return A;
                    default: throw new System.NotImplementedException($"Cost not implemented for card type {CardType}");
                }
            }
        }
        private string SpellSubtypeString
        {
            get
            {
                if (CardType != 'S') return "";
                switch (SpellSubtype)
                {
                    case TerraformSubtype: return $" Radius {Arg}";
                    case DelayedSubtype: return $" {Arg} turns";
                    default: return "";
                }
            }
        }
        public string StatsString
        {
            get
            {
                switch (CardType)
                {
                    case 'C': return $"N: {N} / E: {E} / S: {S} / W: {W}";
                    case 'S': return $"C {C}{(Fast ? " Fast" : "")} {SpellSubtype}{SpellSubtypeString}";
                    case 'A': return $"A {A}";
                    default: throw new System.NotImplementedException($"Stats string not implemented for card type {CardType}");
                }
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
                cardCtrl?.SetPhysicalLocation(Location);
                foreach (var aug in Augments) aug.Position = value;
            }
        }
        public (int x, int y) SubjectivePosition => SubjectiveCoords(Position);

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
        #endregion positioning

        //movement
        public int SpacesMoved { get; set; }
        public int SpacesCanMove => N - SpacesMoved;

        public int AttacksThisTurn { get; set; } = 0;

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
                location = value;
                if (cardCtrl == null) Debug.LogWarning($"Missing a card control. Is this a debug card?");
                cardCtrl?.SetPhysicalLocation(location);
            }
        }
        public virtual bool KnownToEnemy => !Game.HiddenLocations.Contains(Location);
        public int ID { get; private set; }
        public abstract IEnumerable<Effect> Effects { get; }
        public List<GameCard> Augments { get; private set; } = new List<GameCard>();
        public int TurnsOnBoard { get; private set; }
        public abstract bool IsAvatar { get; }

        public abstract Game Game { get; }

        public List<GameCard> AdjacentCards => Game.boardCtrl.CardsAdjacentTo(BoardX, BoardY);

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


        public int SubjectiveCoord(int coord) => ControllerIndex == 0 ? coord : 6 - coord;
        public (int, int) SubjectiveCoords((int x, int y) space) => (SubjectiveCoord(space.x), SubjectiveCoord(space.y));

        public void SetInfo(SerializableCard serializedCard, int id)
        {
            base.SetInfo(serializedCard);

            this.ID = id;
            this.serializedCard = serializedCard;

            N = serializedCard.n;
            E = serializedCard.e;
            S = serializedCard.s;
            W = serializedCard.w;
            C = serializedCard.c;
            A = serializedCard.a;

            Subtext = serializedCard.subtext;
            SpellSubtype = serializedCard.spellType;
            Fast = serializedCard.fast;
            Arg = serializedCard.arg;

            TurnsOnBoard = 0;
            SpacesMoved = 0;
            MovementRestriction = serializedCard.MovementRestriction ?? new MovementRestriction();
            MovementRestriction.SetInfo(this);
            AttackRestriction = serializedCard.AttackRestriction ?? new AttackRestriction();
            AttackRestriction.SetInfo(this);
            PlayRestriction = serializedCard.PlayRestriction ?? new PlayRestriction();
            PlayRestriction.SetInfo(this);

            if (Effects != null) foreach (var eff in Effects) eff?.Reset();
            //instead of setting negations or activations to 0, so that it updates the client correctly
            while (Negated) Negated = false;
            while (Activated) Activated = false;

            cardCtrl.ShowForCardType(CardType, false);
        }

        #region distance/adjacency
        public int DistanceTo(int x, int y)
        {
            if (Location != CardLocation.Field) return int.MaxValue;
            Debug.Log($"Distance from {CardName} to {x}, {y} is" +
                $" {(Mathf.Abs(x - BoardX) > Mathf.Abs(y - BoardY) ? Mathf.Abs(x - BoardX) : Mathf.Abs(y - BoardY))}");
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
        {
            if (card == null || card.Location != CardLocation.Field || Location != CardLocation.Field) return false;
            return DistanceTo(card) <= numSpaces;
        }
        public bool WithinSlots(int numSpaces, int x, int y) => DistanceTo(x, y) <= numSpaces;
        public bool IsAdjacentTo(GameCard card) => card != null && DistanceTo(card) == 1;
        public bool IsAdjacentTo(int x, int y) => DistanceTo(x, y) == 1;
        public bool CardInAOE(GameCard c) => SpaceInAOE(c.Position);
        public bool SpaceInAOE((int x, int y) space) => SpaceInAOE(space.x, space.y);
        public bool SpaceInAOE(int x, int y)
        {
            if (CardType != 'S' || SpellSubtype != TerraformSubtype) return false;
            return DistanceTo(x, y) <= Arg;
        }
        public bool SameColumn(int x, int y) => BoardX + BoardY == x + y;
        public bool SameColumn(GameCard c) => c.Location == CardLocation.Field && SameColumn(c.BoardX, c.BoardY);

        /// <summary>
        /// Returns whether the <paramref name="space"/> passed in is in front of this card
        /// </summary>
        /// <param name="space">The space to check if it's in front of this card</param>
        /// <returns><see langword="true"/> if <paramref name="space"/> is in front of this, <see langword="false"/> otherwise.</returns>
        public bool SpaceInFront((int x, int y) space)
        {
            return SameColumn(space.x, space.y) && SubjectiveCoord(space.x) > SubjectivePosition.x;
        }

        /// <summary>
        /// Returns whether the card passed in is in front of this card
        /// </summary>
        /// <param name="card">The card to check if it's in front of this one</param>
        /// <returns><see langword="true"/> if <paramref name="card"/> is in front of this, <see langword="false"/> otherwise.</returns>
        public bool CardInFront(GameCard card) => SpaceInFront(card.Position);
        #endregion distance/adjacency

        public void PutBack() => cardCtrl?.SetPhysicalLocation(Location);

        public void CountSpacesMovedTo((int x, int y) to)
        {
            SpacesMoved += DistanceTo(to.x, to.y);
        }

        /// <summary>
        /// Resets any of the card's values that might be different from their originals.
        /// Should be called when cards move out the discard, or into the hand, deck, or annihilation
        /// </summary>
        public virtual void ResetCard() 
        { 
            if (serializedCard != null) SetInfo(serializedCard, ID); 
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

            SpacesMoved = 0;
            AttacksThisTurn = 0;
            if(Location == CardLocation.Field) TurnsOnBoard++;
        }

        public void ResetForStack()
        {
            foreach (var e in Effects) e.TimesUsedThisStack = 0;
        }

        #region augments
        public virtual void AddAugment(GameCard augment, IStackable stackSrc = null)
        {
            if (augment == null) return;
            augment.Remove(stackSrc);
            augment.Location = CardLocation.Field;
            Augments.Add(augment);
            augment.AugmentedCard = this;
        }

        protected virtual void Detach(IStackable stackSrc = null)
        {
            AugmentedCard.Augments.Remove(this);
            AugmentedCard = null;
        }
        #endregion augments

        #region statfuncs
        public virtual void SetN(int n, IStackable stackSrc = null) => N = n;
        public virtual void SetE(int e, IStackable stackSrc = null)
        {
            E = e;
            if (E <= 0 && CardType == 'C') Discard(stackSrc);
        }
        public virtual void SetS(int s, IStackable stackSrc = null) => S = s;
        public virtual void SetW(int w, IStackable stackSrc = null) => W = w;
        public virtual void SetC(int c, IStackable stackSrc = null) => C = c;
        public virtual void SetA(int a, IStackable stackSrc = null) => A = a;

        public void SetCharStats(int n, int e, int s, int w, IStackable stackSrc = null)
        {
            SetN(n, stackSrc);
            SetE(e, stackSrc);
            SetS(s, stackSrc);
            SetW(w, stackSrc);
        }

        public void AddToCharStats(int n, int e, int s, int w, IStackable stackSrc = null)
        {
            SetN(N + n, stackSrc);
            SetE(E + e, stackSrc);
            SetS(S + s, stackSrc);
            SetW(W + w, stackSrc);
        }

        public void SetStats((int n, int e, int s, int w, int c, int a) stats, IStackable stackSrc = null)
        {
            SetN(stats.n, stackSrc);
            SetE(stats.e, stackSrc);
            SetS(stats.s, stackSrc);
            SetW(stats.w, stackSrc);
            SetC(stats.c, stackSrc);
            SetA(stats.a, stackSrc);
        }

        public virtual void SetNegated(bool negated, IStackable stackSrc = null) => Negated = negated;
        public virtual void SetActivated(bool activated, IStackable stackSrc = null) => Activated = activated;
        #endregion statfuncs

        #region moveCard
        //so that notify stuff can be sent in the server
        public virtual bool Remove(IStackable stackSrc = null)
        {
            Debug.Log($"Removing {CardName} id {ID} from {Location}");

            switch (Location)
            {
                case CardLocation.Field:
                    if (AugmentedCard != null) Detach(stackSrc);
                    else Game.boardCtrl.RemoveFromBoard(this);
                    break;
                case CardLocation.Discard:
                    Controller.discardCtrl.RemoveFromDiscard(this);
                    break;
                case CardLocation.Hand:
                    Controller.handCtrl.RemoveFromHand(this);
                    break;
                case CardLocation.Deck:
                    Controller.deckCtrl.RemoveFromDeck(this);
                    break;
                case CardLocation.Annihilation:
                    Game.annihilationCtrl.Remove(this);
                    break;
                default:
                    Debug.LogWarning($"Tried to remove card {CardName} from invalid location {Location}");
                    break;
            }

            return true;
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

        public void Dispel(IStackable stackSrc = null)
        {
            SetNegated(true, stackSrc);
            Discard(stackSrc);
        }
        #endregion moveCard
    }
}