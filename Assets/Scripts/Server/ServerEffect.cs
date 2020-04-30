using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerEffect : Effect
{
    public ServerEffect(SerializableEffect se, Card thisCard, int controller)
    {
        this.thisCard = thisCard ?? throw new System.ArgumentNullException("Effect cannot be attached to null card");
        this.serverGame = thisCard.game as ServerGame;
        Subeffects = new Subeffect[se.subeffects.Length];
        targets = new List<Card>();
        coords = new List<Vector2Int>();

        if (!string.IsNullOrEmpty(se.trigger))
        {
            try
            {
                Trigger = Trigger.FromJson(se.triggerCondition, se.trigger, this);
            }
            catch (System.ArgumentException)
            {
                Debug.LogError($"Failed to load trigger of type {se.triggerCondition} from json {se.trigger}");
                throw;
            }
        }

        for (int i = 0; i < se.subeffectTypes.Length; i++)
        {
            try
            {
                Subeffects[i] = thisCard.game.SubeffectFactory.FromJson(se.subeffectTypes[i], se.subeffects[i], this, i);
            }
            catch (System.ArgumentException)
            {
                Debug.LogError($"Failed to load subeffect of type {se.subeffectTypes[i]} from json {se.subeffects[i]}");
                throw;
            }
        }

        MaxTimesCanUsePerTurn = se.maxTimesCanUsePerTurn;
        TimesUsedThisTurn = 0;
    }
}
