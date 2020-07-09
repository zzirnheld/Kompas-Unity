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

    public ServerEffect(SerializableEffect se, GameCard thisCard, ServerGame serverGame, ServerPlayer controller, int effectIndex) 
        : base(se.activationRestriction ?? new ActivationRestriction(), thisCard, se.blurb, effectIndex)
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
            && !Negated
            && ActivationRestriction.Evaluate(controller);
    }

    public void PushToStack(ServerPlayer controller, ActivationContext context)
    {
        EffectsController.PushToStack(this, controller, context);
    }

    public void StartResolution(ActivationContext context)
    {
        Debug.Log($"Resolving effect {EffectIndex} of {Source.CardName} in context {context}");
        serverGame.CurrEffect = this;

        //set context parameters
        CurrActivationContext = context;
        X = context.X ?? 0;
        if (context.Card != null) AddTarget(context.Card);
        if (context.Space.HasValue) Coords.Add(context.Space.Value);
        TimesUsedThisTurn++;

        //notify relevant to this effect starting
        ServerController.ServerNotifier.NotifyEffectX(Source, EffectIndex, X);
        ServerController.ServerNotifier.EffectResolving(this);

        //resolve the effect if possible
        if (Negated) EffectImpossible();
        else ResolveSubeffect(context.StartIndex);
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
        if (OnImpossible == null)
        {
            FinishResolution();
            ServerController.ServerNotifier.EffectImpossible();
        }
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

    public override void AddTarget(GameCard card)
    {
        base.AddTarget(card);
        serverGame.ServerPlayers[card.ControllerIndex].ServerNotifier.SetTarget(Source, EffectIndex, card);
    }
}
