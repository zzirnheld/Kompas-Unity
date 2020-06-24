using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ServerGameCard : GameCard
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

    public void SetInfo(SerializableCard serializedCard, ServerGame game, ServerPlayer owner, ServerEffect[] effects, int id)
    {
        base.SetInfo(serializedCard, id);
        ServerGame = game;
        ServerController = ServerOwner = owner;
        ServerEffects = effects;
    }

    #region stats
    public void SetN(int n, IServerStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.NChange, 
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController, x: n - N);
        EffectsController.Trigger(TriggerCondition.NESWChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        base.SetN(n);
        ServerNotifier.NotifySetN(this);
    }

    public void SetE(int e, IServerStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.EChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController, x: e - E);
        EffectsController.Trigger(TriggerCondition.NESWChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        base.SetE(e);
        ServerNotifier.NotifySetE(this);
    }

    public void SetS(int s, IServerStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.SChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController, x: s - S);
        EffectsController.Trigger(TriggerCondition.NESWChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        base.SetS(s);
        ServerNotifier.NotifySetS(this);
    }

    public void SetW(int w, IServerStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.WChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController, x: w - W);
        EffectsController.Trigger(TriggerCondition.NESWChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        base.SetS(w);
        ServerNotifier.NotifySetW(this);
    }

    public void SetC(int c, IServerStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.CChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController, x: c - C);
        EffectsController.Trigger(TriggerCondition.NESWChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        base.SetC(c);
        ServerNotifier.NotifySetC(this);
    }

    public void SetA(int a, IServerStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.AChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController, x: a - A);
        EffectsController.Trigger(TriggerCondition.NESWChange,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        base.SetA(a);
        ServerNotifier.NotifySetA(this);
    }

    public override void SetN(int n) => this.SetN(n, null);
    public override void SetE(int e) => this.SetE(e, null);
    public override void SetS(int s) => this.SetS(s, null);
    public override void SetW(int w) => this.SetW(w, null);
    public override void SetC(int c) => this.SetC(c, null);
    public override void SetA(int a) => this.SetA(a, null);

    public override bool Negated
    {
        get => base.Negated;
        set
        {
            if (Negated != value) ServerNotifier.NotifySetNegated(this, value);
            base.Negated = value;
        }
    }

    public override bool Activated
    {
        get => base.Activated;
        set
        {
            if (Activated != value) ServerNotifier.NotifyActivate(this, value);
            base.Activated = value;
        }
    }
    #endregion stats

    #region MoveCards
    public void Discard(IServerStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.Discard, cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        base.Discard();
    }
    public new void Discard() => Discard(null);

    public void Rehand(ServerPlayer controller, IServerStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.Rehand, cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        base.Rehand(controller);
    }
    public void Rehand(ServerPlayer controller) => Rehand(controller, null);

    public void Reshuffle(ServerPlayer controller, IServerStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.Reshuffle, cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        base.Reshuffle(controller);
    }
    public void Reshuffle(ServerPlayer controller) => Reshuffle(controller, null);

    public void Topdeck(ServerPlayer controller, IServerStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.Topdeck, cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        base.Topdeck(controller);
    }
    public void Topdeck(ServerPlayer controller) => Topdeck(controller, null);

    public void Bottomdeck(ServerPlayer controller, IServerStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.Bottomdeck, cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        base.Bottomdeck(controller);
    }
    public void Bottomdeck(ServerPlayer controller) => Bottomdeck(controller, null);

    public void Play(int toX, int toY, ServerPlayer controller, IServerStackable stackSrc, bool payCost = false)
    {
        EffectsController.Trigger(TriggerCondition.Play,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController, space: (toX, toY));
        //notify from new controller because could be someone other than this controller
        controller.ServerNotifier.NotifyPlay(this, toX, toY);
        base.Play(toX, toY, controller, payCost);
        controller.ServerNotifier.NotifySetPips(controller.pips);

        //if we just played an augment, note that, and trigger augment
        if (CardType == 'A')
            EffectsController.Trigger(TriggerCondition.AugmentAttached,
                cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
    }
    public void Play(int toX, int toY, ServerPlayer controller, bool payCost = false) => Play(toX, toY, controller, null, payCost);

    public void Move(int toX, int toY, bool normalMove, IServerStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.Move,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController, space: (toX, toY));
        ServerNotifier.NotifyMove(this, toX, toY, normalMove);
        base.Move(toX, toY, normalMove);
    }
    #endregion MoveCards
}
