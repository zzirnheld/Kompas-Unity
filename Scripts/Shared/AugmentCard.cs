using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentCard : Card {

    private CharacterCard thisCharacter;

    public CharacterCard ThisCharacter
    {
        get { return thisCharacter; }
        set { thisCharacter = value; }
    }
}
