using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGameCard : GameCard
{
    public ServerGame ServerGame { get; private set; }
    public override Game Game => ServerGame;

    public ServerEffectsController EffectsController => ServerGame.EffectsController;
    public ServerNotifier ServerNotifier => ServerController.ServerNotifier;

    private ServerPlayer serverController;
    public ServerPlayer ServerController
    {
        get => serverController;
        set
        {
            serverController = value;
            cardCtrl.SetRotation();
            ServerNotifier.NotifyChangeController(this, ServerController);
        }
    }
    public override Player Controller
    {
        get => ServerController;
        set => ServerController = value as ServerPlayer;
    }
    
    public ServerPlayer ServerOwner { get; private set; }
    public override Player Owner => ServerController;

    public ServerEffect[] ServerEffects { get; private set; }
    public override IEnumerable<Effect> Effects => ServerEffects;

    public override bool IsAvatar => false;

    public virtual void SetInfo(SerializableCard serializedCard, ServerGame game, ServerPlayer owner, ServerEffect[] effects, int id)
    {
        base.SetInfo(serializedCard, id);
        ServerGame = game;
        ServerController = ServerOwner = owner;
        ServerEffects = effects;
    }

    public override void AddAugment(GameCard augment, IStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.AugmentAttached,
            cardTriggerer: augment, stackTrigger: stackSrc, triggerer: stackSrc?.Controller ?? Controller);
        ServerNotifier.NotifyAttach(augment, BoardX, BoardY);
        base.AddAugment(augment, stackSrc);
    }

    protected override void Detach(IStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.AugmentDetached,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.Controller ?? Controller);
        base.Detach(stackSrc);
    }

    public override void Remove(IStackable stackSrc = null)
    {
        base.Remove(stackSrc);
        foreach (var aug in Augments) aug.Discard();
    }

    #region stats
    public override void SetN(int n, IStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.NChange, 
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.Controller, x: n - N);
        EffectsController.Trigger(TriggerCondition.NESWChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.Controller);
        base.SetN(n);
        ServerNotifier.NotifySetN(this);
    }

    public override void SetE(int e, IStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.EChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.Controller, x: e - E);
        EffectsController.Trigger(TriggerCondition.NESWChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.Controller);
        base.SetE(e);
        ServerNotifier.NotifySetE(this);
    }

    public override void SetS(int s, IStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.SChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.Controller, x: s - S);
        EffectsController.Trigger(TriggerCondition.NESWChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.Controller);
        base.SetS(s);
        ServerNotifier.NotifySetS(this);
    }

    public override void SetW(int w, IStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.WChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.Controller, x: w - W);
        EffectsController.Trigger(TriggerCondition.NESWChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.Controller);
        base.SetW(w);
        ServerNotifier.NotifySetW(this);
    }

    public override void SetC(int c, IStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.CChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.Controller, x: c - C);
        EffectsController.Trigger(TriggerCondition.NESWChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.Controller);
        base.SetC(c);
        ServerNotifier.NotifySetC(this);
    }

    public override void SetA(int a, IStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.AChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.Controller, x: a - A);
        EffectsController.Trigger(TriggerCondition.NESWChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.Controller);
        base.SetA(a);
        ServerNotifier.NotifySetA(this);
    }

    public override void SetNegated(bool negated, IStackable stackSrc = null)
    {
        if (Negated != negated) ServerNotifier.NotifySetNegated(this, negated);
        base.SetNegated(negated, stackSrc);
    }

    public override void SetActivated(bool activated, IStackable stackSrc = null)
    {
        if (Activated != activated)
        {
            ServerNotifier.NotifyActivate(this, activated);
            if (activated) EffectsController.Trigger(TriggerCondition.Activate,
                 cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.Controller);
            else EffectsController.Trigger(TriggerCondition.Deactivate,
                 cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.Controller);
        }
        base.SetActivated(activated, stackSrc);
    }
    #endregion stats
}
