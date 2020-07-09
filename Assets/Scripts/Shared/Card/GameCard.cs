using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public abstract class GameCard : CardBase {
    public CardController cardCtrl;

    private SerializableCard serializedCard;

    #region stats
    public int BaseN => serializedCard.n;
    public int BaseE => serializedCard.e;
    public int BaseS => serializedCard.s;
    public int BaseW => serializedCard.w;
    public int BaseC => serializedCard.c;
    public int BaseA => serializedCard.a;

    public int N { get; private set; }
    public int E { get; protected set; }
    public int S { get; private set; }
    public int W { get; private set; }
    public int C { get; private set; }
    public int A { get; private set; }

    public bool Fast { get; private set; }

    public string Subtext { get; private set; }
    public string SpellSubtype { get; private set; }
    public int Arg { get; private set; }

    public int Negations { get; private set; } = 0;
    public virtual bool Negated {
        get => Negations > 0;
        private set {
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
    public string StatsString
    {
        get
        {
            switch (CardType)
            {
                case 'C': return $"N: {N} / E: {E} / S: {S} / W: {W}";
                case 'S': return $"C {C} {(Fast ? "Fast" : "")} {SpellSubtype}";
                case 'A': return $"A {A}";
                default: throw new System.NotImplementedException($"Stats string not implemented for card type {CardType}");
            }
        }
    }
    public virtual bool Summoned => CardType == 'C' && Location == CardLocation.Field;
    public int CombatDamage => W;
    public (int n, int e, int s, int w) CharStats => (N, E, S, W);
    #endregion stats

    #region positioning
    public int BoardX { get; protected set; }
    public int BoardY { get; protected set; }
    public (int, int) Position
    {
        get => (BoardX, BoardY);
        set
        {
            (BoardX, BoardY) = value;
            cardCtrl.SetPhysicalLocation(Location);
            foreach (var aug in Augments) aug.Position = value;
        }
    }

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
    public CardLocation Location
    {
        get => location;
        set
        {
            location = value;
            cardCtrl.SetPhysicalLocation(location);
        }
    }
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
                case CardLocation.Annihilation: return Game.AnnihilationCtrl.Cards.IndexOf(this);
                default:
                    Debug.LogError($"Tried to ask for card index when in location {Location}");
                    return -1;
            }
        }
    }

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

        SpacesMoved = 0;
        MovementRestriction = serializedCard.MovementRestriction ?? new MovementRestriction();
        MovementRestriction.SetInfo(this);
        AttackRestriction = serializedCard.AttackRestriction ?? new AttackRestriction();
        AttackRestriction.SetInfo(this);
        PlayRestriction = serializedCard.PlayRestriction ?? new PlayRestriction();
        PlayRestriction.SetInfo(this);
    }

    #region distance/adjacency
    public int DistanceTo(int x, int y)
    {
        if (Location != CardLocation.Field) return -1;
        return Mathf.Abs(x - BoardX) > Mathf.Abs(y - BoardY) ? Mathf.Abs(x - BoardX) : Mathf.Abs(y - BoardY);
        /* equivalent to
         * if (Mathf.Abs(card.X - X) > Mathf.Abs(card.Y - Y)) return Mathf.Abs(card.X - X);
         * else return Mathf.Abs(card.Y - Y);
         * is card.X - X > card.Y - Y? If so, return card.X -X, otherwise return card.Y - Y
        */
    }
    public int DistanceTo(GameCard card) => DistanceTo(card.BoardX, card.BoardY);
    public bool WithinSlots(int numSlots, GameCard card)
    {
        if (card == null) return false;
        return DistanceTo(card) <= numSlots;
    }
    public bool WithinSlots(int numSlots, int x, int y) => DistanceTo(x, y) <= numSlots;
    public bool IsAdjacentTo(GameCard card) => card != null && DistanceTo(card) == 1;
    public bool IsAdjacentTo(int x, int y) => DistanceTo(x, y) == 1;
    public virtual bool CardInAOE(GameCard c) => false;
    public virtual bool SpaceInAOE(int x, int y) => false;
    #endregion distance/adjacency

    public void PutBack() => cardCtrl.SetPhysicalLocation(Location);

    public void CountSpacesMovedTo((int x, int y) to)
    {
        SpacesMoved += DistanceTo(to.x, to.y);
    }

    /// <summary>
    /// Resets any of the card's values that might be different from their originals.
    /// Should be called when cards move out the discard, or into the hand, deck, or annihilation
    /// </summary>
    public virtual void ResetCard() { if (serializedCard != null) SetInfo(serializedCard, ID); }

    /// <summary>
    /// Resets anything that needs to be reset for the start of the turn.
    /// </summary>
    public virtual void ResetForTurn(Player turnPlayer)
    {
        foreach(Effect eff in Effects)
        {
            eff.ResetForTurn(turnPlayer);
        }

        SpacesMoved = 0;
        TurnsOnBoard++;
    }

    #region augments
    public virtual void AddAugment(GameCard augment, IStackable stackSrc = null)
    {
        if (augment == null) return;
        augment.Remove(stackSrc);
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

    public virtual void SetNegated(bool negated, IStackable stackSrc = null) => Negated = negated;
    public virtual void SetActivated(bool activated, IStackable stackSrc = null) => Activated = activated;
    #endregion statfuncs

    #region moveCard
    //so that notify stuff can be sent in the server
    public virtual void Remove(IStackable stackSrc = null)
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
                Game.AnnihilationCtrl.Remove(this);
                break;
            default:
                Debug.LogWarning($"Tried to remove card {CardName} from invalid location {Location}");
                break;
        }
    }

    public void Discard(IStackable stackSrc = null) => Controller.discardCtrl.AddToDiscard(this, stackSrc);

    public void Rehand(Player controller, IStackable stackSrc = null) => controller.handCtrl.AddToHand(this, stackSrc);
    public void Rehand(IStackable stackSrc = null) => Rehand(Controller, stackSrc);

    public void Reshuffle(Player controller, IStackable stackSrc = null) => controller.deckCtrl.ShuffleIn(this, stackSrc);
    public void Reshuffle(IStackable stackSrc = null) => Reshuffle(Controller, stackSrc);

    public void Topdeck(Player controller, IStackable stackSrc = null) => controller.deckCtrl.PushTopdeck(this, stackSrc);
    public void Topdeck(IStackable stackSrc = null) => Topdeck(Controller, stackSrc);

    public void Bottomdeck(Player controller, IStackable stackSrc = null) => controller.deckCtrl.PushBottomdeck(this, stackSrc);
    public void Bottomdeck(IStackable stackSrc = null) => Bottomdeck(Controller, stackSrc);

    public void Play(int toX, int toY, Player controller, IStackable stackSrc = null, bool payCost = false)
    {
        Game.boardCtrl.Play(this, toX, toY, controller);
        if (payCost) controller.Pips -= Cost;
    }

    public void Move(int toX, int toY, bool normalMove, IStackable stackSrc = null)
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
