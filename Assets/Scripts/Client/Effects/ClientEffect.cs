using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientEffect : Effect
{
    public ClientPlayer ClientController;
    public override Player Controller
    {
        get { return ClientController; }
        set { ClientController = value as ClientPlayer; }
    }
    public ClientGame ClientGame { get; }
    public DummySubeffect[] DummySubeffects { get; }
    public ClientTrigger ClientTrigger { get; }

    public override Subeffect[] Subeffects => DummySubeffects;
    public override Trigger Trigger => ClientTrigger;

    public List<ClientGameCard> ClientTargets { get; } = new List<ClientGameCard>();
    public override IEnumerable<GameCard> Targets => ClientTargets;

    public ClientEffect(SerializableEffect se, GameCard thisCard, ClientGame clientGame)
        : base(se.activationRestriction ?? new ActivationRestriction(), thisCard, se.blurb)
    {
        this.ClientGame = clientGame;
        DummySubeffects = new DummySubeffect[se.subeffects.Length];

        if (!string.IsNullOrEmpty(se.trigger))
        {
            try
            {
                ClientTrigger = ClientTrigger.FromJson(se.triggerCondition, se.trigger, this);
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
                DummySubeffects[i] = DummySubeffect.FromJson(se.subeffectTypes[i], se.subeffects[i], this, i);
            }
            catch (System.ArgumentException)
            {
                Debug.LogError($"Failed to load subeffect of type {se.subeffectTypes[i]} from json {se.subeffects[i]}");
            }
        }
    }
}
