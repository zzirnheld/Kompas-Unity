using System.Collections.Generic;

[System.Serializable]
public abstract class TemporaryCardChangeSubeffect : ServerSubeffect
{
    public TriggerRestriction triggerRestriction = new TriggerRestriction();
    public string endCondition;

    public string fallOffCondition = Trigger.Remove;
    public string[] fallOffRestrictions =
    {
        TriggerRestriction.ThisCardTriggered,
        TriggerRestriction.ThisCardInPlay,
    };

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        triggerRestriction.Initialize(this, ThisCard, null);
    }

    public override bool Resolve()
    {
        //create the hanging effects, of which there can be multiple
        var effectsApplied = CreateHangingEffects();

        //each of the effects needs to be registered, and registered for how it could fall off
        foreach(var (eff, card) in effectsApplied)
        {
            ServerGame.EffectsController.RegisterHangingEffect(endCondition, eff);

            //conditions for falling off
            var triggerRest = new TriggerRestriction() { triggerRestrictions = fallOffRestrictions };
            triggerRest.Initialize(this, card, null);
            ServerGame.EffectsController.RegisterHangingEffectFallOff(fallOffCondition, triggerRest, eff);
        }

        //after all that's done, make it do the next subeffect
        return ServerEffect.ResolveNextSubeffect();
    }

    protected abstract IEnumerable<(HangingEffect, GameCard)> CreateHangingEffects();
}
