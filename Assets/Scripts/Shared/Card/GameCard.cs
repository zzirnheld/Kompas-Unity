﻿using KompasCore.Effects;
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

        protected SerializableCard InitialCardValues { get; private set; }

        public bool CurrentlyVisible => gameObject.activeSelf;

        #region stats
        public int BaseN => InitialCardValues.n;
        public override int BaseE => InitialCardValues?.e ?? default;
        public int BaseS => InitialCardValues.s;
        public int BaseW => InitialCardValues.w;
        public int BaseC => InitialCardValues.c;
        public int BaseA => InitialCardValues.a;

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
        #endregion stats

        #region positioning
        private Space position;
        public override Space Position
        {
            get => position;
            set
            {
                if (null != value) Debug.Log($"Position of {CardName} set to {value}");

                position = value;
                //card controller will be null on server. not using null ? because of monobehavior
                if (cardCtrl != null) cardCtrl.SetPhysicalLocation(Location);
                foreach (var aug in augmentsList) aug.Position = value;
            }
        }

        public override int IndexInList => GameLocation?.IndexOf(this) ?? -1;
        public bool InHiddenLocation => Game.IsHiddenLocation(Location);

        public override IEnumerable<GameCard> AdjacentCards
            => Game?.boardCtrl.CardsAdjacentTo(Position) ?? new List<GameCard>();

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
                    GameLocation = augmentedCard.GameLocation;
                    Position = augmentedCard.Position;
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
        public override MovementRestriction MovementRestriction { get; protected set; }
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
                //Debug.Log($"Card {ID} named {CardName} location set to {Location}");
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

        public GameCardCardLinkHandler CardLinkHandler { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append($"id {ID} controlled by {ControllerIndex}, owned by {OwnerIndex}, in location {location}, position {Position}, ");
            if (Attached) sb.Append($"augmented card is {AugmentedCard.CardName} id {AugmentedCard.ID}, ");
            if (Augments.Count() > 0) sb.Append($"augments are {string.Join(", ", Augments.Select(c => $"{c.CardName} id {c.ID}"))}");
            return sb.ToString();
        }

        protected virtual void SetCardInfo(SerializableCard serializedCard, int id)
        {
            SetCardInformation(serializedCard);

            FileName = CardRepository.FileNameFor(CardName);

            CardLinkHandler = new GameCardCardLinkHandler(this);

            ID = id;
            InitialCardValues = serializedCard;

            EffectInitializationContext initializationContext = new EffectInitializationContext(Game, this);
            MovementRestriction = serializedCard.MovementRestriction ?? new MovementRestriction();
            MovementRestriction.Initialize(initializationContext);
            AttackRestriction = serializedCard.AttackRestriction ?? new AttackRestriction();
            AttackRestriction.Initialize(initializationContext);
            PlayRestriction = serializedCard.PlayRestriction ?? new PlayRestriction();
            PlayRestriction.Initialize(initializationContext);

            cardCtrl.ShowForCardType(CardType, false);

            Debug.Log($"Finished setting up info for card {CardName}");
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

            SetSpacesMoved(0);
            SetAttacksThisTurn(0);
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
                throw new CardNotHereException(CardLocation.Board, this, $"Can't put an augment on a card not in {Location}!");

            Debug.Log($"Attaching {augment.CardName} from {augment.Location} to {CardName} in {Location}");

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
        public override void SetN(int n, IStackable stackSrc, bool onlyStatBeingSet = true)
        {
            base.SetN(n, stackSrc, onlyStatBeingSet);
            cardCtrl.N = N;
        }

        public override void SetE(int e, IStackable stackSrc, bool onlyStatBeingSet = true)
        {
            base.SetE(e, stackSrc, onlyStatBeingSet);
            cardCtrl.E = E;
        }

        public override void SetS(int s, IStackable stackSrc, bool onlyStatBeingSet = true)
        {
            base.SetS(s, stackSrc, onlyStatBeingSet);
            cardCtrl.S = S;
        }

        public override void SetW(int w, IStackable stackSrc, bool onlyStatBeingSet = true)
        {
            base.SetW(w, stackSrc, onlyStatBeingSet);
            cardCtrl.W = W;
        }

        public override void SetC(int c, IStackable stackSrc, bool onlyStatBeingSet = true)
        {
            base.SetC(c, stackSrc, onlyStatBeingSet);
            cardCtrl.C = C;
        }

        public override void SetA(int a, IStackable stackSrc, bool onlyStatBeingSet = true)
        {
            base.SetA(a, stackSrc, onlyStatBeingSet);
            cardCtrl.A = A;
        }

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
            SetN(n, stackSrc, onlyStatBeingSet: false);
            SetE(e, stackSrc, onlyStatBeingSet: false);
            SetS(s, stackSrc, onlyStatBeingSet: false);
            SetW(w, stackSrc, onlyStatBeingSet: false);
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
        public void AddToStats(CardStats buff, IStackable stackSrc = null)
            => SetStats(Stats + buff, stackSrc);

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

        public virtual void SetSpacesMoved(int spacesMoved)
            => SpacesMoved = spacesMoved;
        public virtual void SetAttacksThisTurn(int attacksThisTurn)
            => this.attacksThisTurn = attacksThisTurn;
        public virtual void SetTurnsOnBoard(int turnsOnBoard, IStackable stackSrc = null)
            => TurnsOnBoard = turnsOnBoard;

        public void SetDuration(int duration)
            => Duration = duration;
        #endregion statfuncs

        #region moveCard
        /// <summary>
        /// Removes the card from its current location
        /// </summary>
        /// <param name="stackSrc">The stackable (if any) that caused the card's game location to change</param>
        /// <returns><see langword="true"/> if the card was successfully removed, 
        /// <see langword="false"/> if the card is an avatar that got sent back</returns>
        public virtual bool Remove(IStackable stackSrc = null)
        {
            // Debug.Log($"Removing {CardName} id {ID} from {Location}");

            if (Location == CardLocation.Nowhere) return true;

            if (Attached) Detach(stackSrc);
            else GameLocation.Remove(this);
            //If it got to either of these, it's not an avatar that failed to get removed
            return true;
        }

        public virtual void Vanish() => Discard();
        public void Discard(IStackable stackSrc = null) => Controller.discardCtrl.Discard(this, stackSrc);

        public void Rehand(Player controller, IStackable stackSrc = null) => controller.handCtrl.Hand(this, stackSrc);
        public void Rehand(IStackable stackSrc = null) => Rehand(Controller, stackSrc);

        public void Reshuffle(Player controller, IStackable stackSrc = null) => controller.deckCtrl.ShuffleIn(this, stackSrc);
        public void Reshuffle(IStackable stackSrc = null) => Reshuffle(Controller, stackSrc);

        public void Topdeck(Player controller, IStackable stackSrc = null) => controller.deckCtrl.PushTopdeck(this, stackSrc);
        public void Topdeck(IStackable stackSrc = null) => Topdeck(Controller, stackSrc);

        public void Bottomdeck(Player controller, IStackable stackSrc = null) => controller.deckCtrl.PushBottomdeck(this, stackSrc);
        public void Bottomdeck(IStackable stackSrc = null) => Bottomdeck(Controller, stackSrc);

        public void Play(Space to, Player controller, IStackable stackSrc = null, bool payCost = false)
        {
            var costToPay = Cost;
            Game.boardCtrl.Play(this, to, controller, stackSrc);

            if (payCost) controller.Pips -= costToPay;
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