using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerEffect : Effect, IServerStackable
{
    public ServerSubeffect[] ServerSubeffects { get; }
    public ServerTrigger ServerTrigger { get; }
    
    public ServerGame serverGame;
    public ServerEffectsController EffectsController => serverGame.EffectsController;

    public ServerSubeffect OnImpossible = null;
    
    public ServerPlayer ServerController { get;  set; }
    public override Player Controller
    {
        get { return ServerController; }
        set { ServerController = value as ServerPlayer; }
    }
    public override Subeffect[] Subeffects => ServerSubeffects;
    public override Trigger Trigger => ServerTrigger;

    public ServerEffect(SerializableEffect se, Card thisCard, ServerGame serverGame, ServerPlayer controller) 
        : base(se.activationRestriction ?? new ActivationRestriction(), thisCard, se.blurb)
    {
        this.serverGame = serverGame;
        this.ServerController = controller;
        ServerSubeffects = new ServerSubeffect[se.subeffects.Length];

        if (!string.IsNullOrEmpty(se.trigger))
        {
            try
            {
                ServerTrigger = ServerTrigger.FromJson(se.triggerCondition, se.trigger, this);
                EffectsController.RegisterTrigger(se.triggerCondition, ServerTrigger);
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
                ServerSubeffects[i] = ServerSubeffect.FromJson(se.subeffectTypes[i], se.subeffects[i], this, i);
            }
            catch (System.ArgumentException)
            {
                Debug.LogError($"Failed to load subeffect of type {se.subeffectTypes[i]} from json {se.subeffects[i]}");
                throw;
            }
        }
    }

    public bool CanBeActivatedBy(ServerPlayer controller)
    {
        if (serverGame.uiCtrl.DebugMode) return true;
        return Trigger == null
            && controller.index == Source.ControllerIndex
            && ActivationRestriction.Evaluate(controller);
    }

    public void PushToStack(ServerPlayer controller)
    {
        EffectsController.PushToStack(this, controller);
    }

    public void StartResolution()
    {
        Source.game.CurrEffect = this;
        TimesUsedThisTurn++;
        ServerController.ServerNotifier.NotifyEffectX(Source, EffectIndex, X);
        ServerController.ServerNotifier.EffectResolving(this);
        if (Negated) EffectImpossible();
        else ResolveSubeffect(0);
    }

    public void ResolveNextSubeffect()
    {
        ResolveSubeffect(SubeffectIndex + 1);
    }

    public void ResolveSubeffect(int index)
    {
        if (index >= ServerSubeffects.Length)
        {
            FinishResolution();
            return;
        }

        Debug.Log($"Resolving subeffect of type {ServerSubeffects[index].GetType()}");
        SubeffectIndex = index;
        ServerController.ServerNotifier.NotifyEffectX(Source, EffectIndex, X);
        ServerSubeffects[index].Resolve();
    }

    /// <summary>
    /// If the effect finishes resolving, this method is called.
    /// </summary>
    private void FinishResolution()
    {
        SubeffectIndex = 0;
        X = 0;
        Targets.Clear();
        Rest.Clear();
        OnImpossible = null;
        ServerController.ServerNotifier.NotifyBothPutBack();
        EffectsController.FinishStackEntryResolution();
    }

    /// <summary>
    /// Cancels resolution of the effect, 
    /// or, if there is something pending if the effect becomes impossible, resolves that
    /// </summary>
    public void EffectImpossible()
    {
        Debug.Log($"Effect of {Source.CardName} is being declared impossible");
        if (OnImpossible == null) FinishResolution();
        else
        {
            SubeffectIndex = OnImpossible.SubeffIndex;
            OnImpossible.OnImpossible();
        }
    }

    public void DeclineAnotherTarget()
    {
        OnImpossible?.OnImpossible();
    }
}
