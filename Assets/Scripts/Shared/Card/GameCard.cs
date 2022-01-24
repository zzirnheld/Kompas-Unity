using KompasCore.Effects;
using KompasCore.Exceptions;
using KompasCore.GameCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KompasCore.Cards
{
    public abstract class GameCard : GameCardBase
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
        public override GameCard Card
        {
            get => this;
            protected set
            {
                throw new NotImplementedException("What are you smoking?");
            }
        }

        private SerializableCard serializedCard;

        public bool CurrentlyVisible => gameObject.activeSelf;

        #region stats
        public int BaseN => serializedCard.n;
        public override int BaseE
        {
            get => serializedCard.e;
            protected set
            {
                throw new NotImplementedException($"Tried to set base e of actual GameCard {this}");
            }
        }
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
        public override bool Negated
        {
            get => Negations > 0;
            protected set
            {
                if (value) Negations++;
                else Negations--;

                foreach (var e in Effects) e.Negated = Negated;
            }
        }
        public int Activations { get; private set; } = 0;
        public override bool Activated
        {
            get => Activations > 0;
            protected set
            {
                if (value) Activations++;
                else Activations--;
            }
        }

        public override bool Summoned
        {
            get => CardType != 'C' || Location == CardLocation.Board;
            protected set
            {
                throw new NotImplementedException($"Tried to set summoned on list of actual GameCard {this}");
            }
        }
        public virtual bool CanRemove => true;
        public virtual int CombatDamage => W;
        public (int n, int e, int s, int w) CharStats => (N, E, S, W);
        public (int n, int e, int s, int w, int c, int a) Stats => (N, E, S, W, C, A);
        #endregion stats

        #region positioning
        private Space position;
        public override Space Position
        {
            get => position;
            set
            {
                Debug.Log($"Position of {CardName} set to {value}");
                position = value;
                //card controller will be null on server. not using null ? because of monobehavior
                if (cardCtrl != null) cardCtrl.SetPhysicalLocation(Location);
                foreach (var aug in augmentsList) aug.Position = value;
            }
        }

        public override int IndexInList
        {
            get => GameLocation?.IndexOf(this) ?? -1;
            protected set
            {
                throw new NotImplementedException($"Tried to set index in list of actual GameCard {this}");
            }
        }
        public bool InHiddenLocation => Game.IsHiddenLocation(Location);

        public override IEnumerable<GameCard> AdjacentCards
        {
            get => Game.boardCtrl.CardsAdjacentTo(Position);
            protected set
            {
                throw new NotImplementedException($"Tried to set adjacent cards of actual GameCard {this}");
            }
        }

        public bool AlreadyCopyOnBoard => Game.BoardHasCopyOf(this);

        public bool IsFriendlyCopyOf(GameCard c) => c != this && c.Controller == Controller && c.CardName == CardName;
        #endregion positioning

        #region Augments
        private readonly List<GameCard> augmentsList = new List<GameCard>();
        public override IEnumerable<GameCard> Augments
        {
            get => augmentsList;
            protected set
            {
                augmentsList.Clear();
                augmentsList.AddRange(value);
            }
        }

        private GameCard augmentedCard;
        public override GameCard AugmentedCard
        {
            get => augmentedCard;
            protected set
            {
                augmentedCard = value;
                if (augmentedCard != null)
                {
                    Position = augmentedCard.Position;
                    Location = augmentedCard.Location;
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
        public override int SpacesMoved { get; protected set; } = 0;
        public override int SpacesCanMove
        {
            get => N - SpacesMoved;
            protected set => SpacesMoved = N - value;
        }

        public int attacksThisTurn = 0;
        public int AttacksThisTurn => attacksThisTurn;

        //restrictions
        public MovementRestriction MovementRestriction { get; private set; }
        public AttackRestriction AttackRestriction { get; private set; }
        public override PlayRestriction PlayRestriction { get; protected set; }

        //controller/owners
        public int ControllerIndex => Controller?.index ?? 0;
        public int OwnerIndex => Owner?.index ?? -1;

        //misc
        private CardLocation location;
        public override CardLocation Location 
        {
            get => location;
            protected set
            {
                location = value;
                Debug.Log($"Card {ID} named {CardName} location set to {Location}");
                if (cardCtrl != null) cardCtrl.SetPhysicalLocation(Location);
                //else Debug.LogWarning($"Missing a card control. Is this a debug card?");
            }
        }

        private IGameLocation gameLocation;
        public IGameLocation GameLocation 
        {
            get => gameLocation;
            set
            {
                gameLocation = value;
                Location = value.CardLocation;
            }
        }

        public string BaseJson => Game.cardRepo.GetJsonFromName(CardName);

        public int TurnsOnBoard { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append($"id {ID} controlled by {ControllerIndex}, owned by {OwnerIndex}, in location {location}, position {Position}, ");
            if (Attached) sb.Append($"augmented card is {AugmentedCard.CardName} id {AugmentedCard.ID}, ");
            if (Augments.Count() > 0) sb.Append($"augments are {string.Join(", ", Augments.Select(c => $"{c.CardName} id {c.ID}"))}");
            return sb.ToString();
        }

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
            if (serializedCard == null)
            {
                Debug.Log("Tried to reset card whose info was never set! This should only happen at game start");
                return;
            }

            // Set info in CardBase
            base.SetInfo(serializedCard);

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
            if (Location == CardLocation.Board) TurnsOnBoard++;
        }

        public void ResetForStack()
        {
            foreach (var e in Effects) e.TimesUsedThisStack = 0;
        }

        public void PutBack()
        {
            if (cardCtrl != null) cardCtrl.SetPhysicalLocation(Location);
        }

        /// <summary>
        /// Accumulates the distance to <paramref name="to"/> into the number of spaces this card moved this turn.
        /// </summary>
        /// <param name="to">The space being moved to</param>
        public void CountSpacesMovedTo((int x, int y) to) => SetSpacesMoved(SpacesMoved + Game.boardCtrl.ShortestEmptyPath(this, to));

        #region augments

        public virtual void AddAugment(GameCard augment, IStackable stackSrc = null)
        {
            //can't add a null augment
            if (augment == null) 
                throw new NullAugmentException(stackSrc, this, "Can't add a null augment");
            if (Location != CardLocation.Board) 
                throw new CardNotHereException(CardLocation.Board, this, "Can't put an augment on a card not in play!");

            augment.Remove(stackSrc);

            augmentsList.Add(augment);
            //and update the augment's augmented card, to reflect its new status
            augment.AugmentedCard = this;
        }

        protected virtual void Detach(IStackable stackSrc = null)
        {
            if (!Attached) throw new NotAugmentingException(this);

            AugmentedCard.augmentsList.Remove(this);
            AugmentedCard = null;
        }
        #endregion augments

        #region statfuncs
        /* This must happen through setters, not properties, so that notifications and stack sending
         * can be managed as intended. */
        public virtual void SetN(int n, IStackable stackSrc = null, bool notify = true) => N = n;
        public virtual void SetE(int e, IStackable stackSrc = null, bool notify = true) => E = e;
        public virtual void SetS(int s, IStackable stackSrc = null, bool notify = true) => S = s;
        public virtual void SetW(int w, IStackable stackSrc = null, bool notify = true) => W = w;
        public virtual void SetC(int c, IStackable stackSrc = null, bool notify = true) => C = c;
        public virtual void SetA(int a, IStackable stackSrc = null, bool notify = true) => A = a;
        /// <summary>
        /// Inflicts the given amount of damage, which can affect both shield and E. Used by attacks and (rarely) by effects.
        /// </summary>
        public virtual void TakeDamage(int dmg, IStackable stackSrc = null) => SetE(E - dmg, stackSrc: stackSrc);

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

        public virtual void SetSpacesMoved(int spacesMoved, bool fromReset = false)
            => SpacesMoved = spacesMoved;
        public virtual void SetAttacksThisTurn(int attacksThisTurn, bool fromReset = false)
            => this.attacksThisTurn = attacksThisTurn;
        public virtual void SetTurnsOnBoard(int turnsOnBoard, IStackable stackSrc = null, bool fromReset = false)
            => TurnsOnBoard = turnsOnBoard;
        #endregion statfuncs

        #region moveCard
        //so that notify stuff can be sent in the server
        public virtual void Remove(IStackable stackSrc = null)
        {
            // Debug.Log($"Removing {CardName} id {ID} from {Location}");

            if (Location == CardLocation.Nowhere) return;

            if (Attached) Detach(stackSrc);
            else GameLocation.Remove(this);
        }

        public virtual void Vanish() => Discard();
        public void Discard(IStackable stackSrc = null) => Controller.discardCtrl.Add(this, stackSrc);

        public void Rehand(Player controller, IStackable stackSrc = null) => controller.handCtrl.Add(this, stackSrc);
        public void Rehand(IStackable stackSrc = null) => Rehand(Controller, stackSrc);

        public void Reshuffle(Player controller, IStackable stackSrc = null) => controller.deckCtrl.ShuffleIn(this, stackSrc);
        public void Reshuffle(IStackable stackSrc = null) => Reshuffle(Controller, stackSrc);

        public void Topdeck(Player controller, IStackable stackSrc = null) => controller.deckCtrl.PushTopdeck(this, stackSrc);
        public void Topdeck(IStackable stackSrc = null) => Topdeck(Controller, stackSrc);

        public void Bottomdeck(Player controller, IStackable stackSrc = null) => controller.deckCtrl.PushBottomdeck(this, stackSrc);
        public void Bottomdeck(IStackable stackSrc = null) => Bottomdeck(Controller, stackSrc);

        public void Play(Space to, Player controller, IStackable stackSrc = null, bool payCost = false)
        {
            Game.boardCtrl.Play(this, to, controller, stackSrc);

            if (payCost) controller.Pips -= Cost;
        }

        public void Move(Space to, bool normalMove, IStackable stackSrc = null)
            => Game.boardCtrl.Move(this, to, normalMove, stackSrc);

        public void Dispel(IStackable stackSrc = null)
        {
            SetNegated(true, stackSrc);
            Discard(stackSrc);
        }

        public virtual void Reveal(IStackable stackSrc = null) 
        {
            //Reveal should only succeed if the card is not known to the enemy
            if (KnownToEnemy) throw new AlreadyKnownException(this);
        }
        #endregion moveCard
    }
}