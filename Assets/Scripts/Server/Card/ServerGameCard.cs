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

    public override bool IsAvatar => false;

    public virtual void SetInfo(SerializableCard serializedCard, ServerGame game, ServerPlayer owner, ServerEffect[] effects, int id)
    {
        base.SetInfo(serializedCard, id);
        ServerGame = game;
        ServerController = ServerOwner = owner;
        ServerEffects = effects;
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
        base.SetS(w);
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

    #region MoveCards
    public override void Discard(IStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.Discard, cardTriggerer: this, stackTrigger: stackSrc, triggerer: stackSrc?.Controller);
        base.Discard(stackSrc);
    }

    public override void Rehand(Player controller, IStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.Rehand, cardTriggerer: this, stackTrigger: stackSrc, triggerer: controller);
        base.Rehand(controller, stackSrc);
    }

    public override void Reshuffle(Player controller, IStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.Reshuffle, cardTriggerer: this, stackTrigger: stackSrc, triggerer: controller);
        base.Reshuffle(controller, stackSrc);
    }

    public override void Topdeck(Player controller, IStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.Topdeck, cardTriggerer: this, stackTrigger: stackSrc, triggerer: controller);
        base.Topdeck(controller, stackSrc);
    }

    public override void Bottomdeck(Player controller, IStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.Bottomdeck, cardTriggerer: this, stackTrigger: stackSrc, triggerer: controller);
        base.Bottomdeck(controller, stackSrc);
    }

    public override void Play(int toX, int toY, Player controller, IStackable stackSrc, bool payCost = false)
    {
        EffectsController.Trigger(TriggerCondition.Play,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: controller, space: (toX, toY));
        //notify from new controller because could be someone other than this controller
        ServerGame.ServerPlayers[controller.index].ServerNotifier.NotifyPlay(this, toX, toY);
        base.Play(toX, toY, controller, stackSrc, payCost);
        ServerGame.ServerPlayers[controller.index].ServerNotifier.NotifySetPips(controller.pips);

        //if we just played an augment, note that, and trigger augment
        if (CardType == 'A')
            EffectsController.Trigger(TriggerCondition.AugmentAttached,
                cardTriggerer: this, stackTrigger: stackSrc, triggerer: controller);
    }

    public override void Move(int toX, int toY, bool normalMove, IStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.Move,
            cardTriggerer: this, stackTrigger: stackSrc, triggerer: Controller, space: (toX, toY));
        ServerNotifier.NotifyMove(this, toX, toY, normalMove);
        base.Move(toX, toY, normalMove, stackSrc);
    }
    #endregion MoveCards
}
