using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AugmentCard : Card
{

    private Card augmentedCard;

    private int a;
    private string subtext;
    private string[] augSubtypes;
    private bool fast;

    public int A
    {
        get { return a; }
        set { a = value; }
    }

    public Card AugmentedCard
    {
        get { return augmentedCard; }
        set {
            augmentedCard = value;
            if(boardX != augmentedCard.BoardX || boardY != augmentedCard.BoardY)
                MoveTo(augmentedCard.BoardX, augmentedCard.BoardY, false);
        }
    }

    public string[] AugSubtypes
    {
        get => augSubtypes;
    }

    public override int Cost => A;
    public override string StatsString => fast ? "Fast A" : "A"  + A;

    //get data
    public SerializableAugCard GetSerializableVersion()
    {
        int index = augmentedCard.Augments.IndexOf(this);
        SerializableAugCard serializableSpell = new SerializableAugCard
        {
            cardName = CardName,
            effText = EffText,
            subtext = subtext,
            a = a,
            augSubtypes = augSubtypes,

            location = location,
            owner = ControllerIndex,
            BoardX = boardX,
            BoardY = boardY,
            subtypeText = SubtypeText,
            index = index
        };
        return serializableSpell;
    }

    //set data
    public override void SetInfo(SerializableCard serializedCard, Game game, Player owner, Effect[] effects)
    {
        if (!(serializedCard is SerializableAugCard serializedAug)) return;

        a = serializedAug.a;
        subtext = serializedAug.subtext;
        augSubtypes = serializedAug.augSubtypes;
        fast = serializedAug.fast;

        base.SetInfo(serializedCard, game, owner, effects);
    }

    //game mechanics
    //TODO prevent z fighting
    public override void MoveTo(int toX, int toY, bool playerInitiated)
    {
        base.MoveTo(toX, toY, playerInitiated);

        transform.localPosition = new Vector3(GridIndexToPos(toX), GridIndexToPos(toY), -0.05f);

    }

    public void Detach()
    {
        AugmentedCard = null;
    }
}
