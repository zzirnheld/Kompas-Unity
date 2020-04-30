using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientEffect : Effect
{
    public override Player Controller => ClientController;

    public ClientPlayer ClientController;
    public ClientGame ClientGame { get; }
    public DummySubeffect[] Subeffects { get; }
    public ClientTrigger Trigger { get; }

    public ClientEffect(SerializableEffect se, Card thisCard, int controller) : base(se.maxTimesCanUsePerTurn)
    {
        this.thisCard = thisCard ?? throw new System.ArgumentNullException("Effect cannot be attached to null card");
        this.ClientGame = thisCard.game as ClientGame;
        Subeffects = new DummySubeffect[se.subeffects.Length];
        targets = new List<Card>();
        coords = new List<Vector2Int>();

        if (!string.IsNullOrEmpty(se.trigger))
        {
            try
            {
                Trigger = ClientTrigger.FromJson(se.triggerCondition, se.trigger, this);
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
                Subeffects[i] = DummySubeffect.FromJson(se.subeffectTypes[i], se.subeffects[i], this, i);
            }
            catch (System.ArgumentException)
            {
                Debug.LogError($"Failed to load subeffect of type {se.subeffectTypes[i]} from json {se.subeffects[i]}");
            }
        }
    }
}
