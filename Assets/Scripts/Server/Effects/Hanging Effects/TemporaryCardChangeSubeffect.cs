using System.Collections.Generic;

[System.Serializable]
public abstract class TemporaryCardChangeSubeffect : ServerSubeffect
{
    public TriggerRestriction TriggerRestriction = new TriggerRestriction();
    public string EndCondition;

    public string FallOffCondition = Trigger.Remove;
    public string[] FallOffRestrictions =
    {
        TriggerRestriction.ThisCardTriggered,
        TriggerRestriction.ThisCardInPlay,
    };

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        TriggerRestriction.Initialize(this, ThisCard, null);
    }

    public override void Resolve()
    {
        //create the hanging effects, of which there can be multiple
        var effectsApplied = CreateHangingEffects();

        //each of the effects needs to be registered, and registered for how it could fall off
        foreach(var (eff, card) in effectsApplied)
        {
            ServerGame.EffectsController.RegisterHangingEffect(EndCondition, eff);

            //conditions for falling off
            var triggerRest = new TriggerRestriction() { restrictions = FallOffRestrictions };
            triggerRest.Initialize(this, card, null);
            ServerGame.EffectsController.RegisterHangingEffectFallOff(FallOffCondition, triggerRest, eff);
        }

        //after all that's done, make it do the next subeffect
        ServerEffect.ResolveNextSubeffect();
    }

    protected abstract IEnumerable<(HangingEffect, GameCard)> CreateHangingEffects();
}
