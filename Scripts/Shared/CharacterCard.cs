using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCard : Card {

    //game mechanic data
    private int n;
    private int e;
    private int s;
    private int w;
    private int m;

    private string[] subtypes;
    private List<AugmentCard> augments = new List<AugmentCard>();

    //stat getters TODO take into account tags here
    //reminder: don't need separate setters because you don't notify because you'll only change stats when server tells you to
    //all of these will return 0 if their value is < 0
    #region stats
    public int N
    {
        get {
            if (n < 0) return 0;
            return n;
        }
        set
        {
            n = value;
            //update movement: if you gain n, gain that much m, if you lose n, lose that much n
            M += n - this.n;
        }
    }
    public int E
    {
        get
        {
            if (e < 0) return 0;
            return e;
        }
        set
        {
            e = value;
            //TODO when e goes to 0, die
        }
    }
    public int S
    {
        get
        {
            if (s < 0) return 0;
            return s;
        }
        set { s = value; }
    }
    public int W
    {
        get
        {
            if (w < 0) return 0;
            return w;
        }
        set { w = value; }
    }
    public int M
    {
        get
        {
            if (m < 0) return 0;
            return m;
        }
        set { m = value; }
    }
    #endregion stats

    public string[] Subtypes { get { return subtypes; } }
    public List<AugmentCard> Augments { get { return augments; } }

    //get other information
    public override int GetCost() { return S; }
    public string GetStatsString() { return "N: " + N + "\t\tE: " + E + "\t\tS: " + S + "\t\tW: " + W; }
    
    public SerializableCharCard GetSerializableVersion()
    {
        int index = -1;
        if (location == CardLocation.Hand) index = Game.mainGame.Players[owner].handCtrl.IndexOf(this);
        else if(location == CardLocation.Discard) index = Game.mainGame.Players[owner].discardCtrl.IndexOf(this);

        SerializableCharCard serializableChar = new SerializableCharCard
        {
            cardName = cardName,
            effText = effText,
            n = n,
            e = e,
            s = s,
            w = w,
            subtypes = subtypes,

            m = m,
            location = location,
            owner = owner,
            BoardX = boardX,
            BoardY = boardY,
            subtypeText = subtypeText,
            index = index
            
        };
        return serializableChar;
    }

    //set information
    public override void SetInfo(SerializableCard serializedCard)
    {
        if (!(serializedCard is SerializableCharCard)) return;
        SerializableCharCard serializedChar = serializedCard as SerializableCharCard;

        n = serializedChar.n;
        e = serializedChar.e;
        s = serializedChar.s;
        w = serializedChar.w;
        subtypes = serializedChar.subtypes;
        m = serializedChar.m;

        base.SetInfo(serializedCard);
    }
    /// <summary>
    /// For convenience of not having to type out four lines
    /// </summary>
    public void SetNESW(int n, int e, int s, int w)
    {
        N = n;
        E = e;
        S = s;
        W = w;
    }

    public void ResetM()
    {
        M = N;
    }

    public int GetCombatDamage()
    {
        return W;
    }

    public void DealCombatDamage(int dmg)
    {
        E -= dmg;
    }

    //game mechanics
    public override void MoveTo(int toX, int toY)
    {
        base.MoveTo(toX, toY);
        foreach (AugmentCard aug in augments) aug.MoveTo(toX, toY);
    }

    public void AddAugment(AugmentCard augment)
    {
        if (augment == null) return;
        augments.Add(augment);
        augment.ThisCharacter = this;
    }
    public bool HasAugment(AugmentCard augment) { return augments.Contains(augment); }
    public void RemoveAugment(AugmentCard augment)
    {
        augments.Remove(augment);
        augment.ThisCharacter = null;
    }
    public void RemoveAugmentAt(int index)
    {
        AugmentCard aug = augments[index];
        augments.RemoveAt(index);
        aug.ThisCharacter = null;
    }

    public void Attack(CharacterCard defender)
    {
        int attackerDmg = GetCombatDamage();
        int defenderDmg = defender.GetCombatDamage();

        defender.DealCombatDamage(attackerDmg);
        DealCombatDamage(defenderDmg);
    }


}
