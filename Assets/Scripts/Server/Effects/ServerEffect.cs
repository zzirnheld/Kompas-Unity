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

    public ServerPlayer ServerController { get; set; }
    public Player Controller => ServerController;
    public override Subeffect[] Subeffects => ServerSubeffects;
    public override Trigger Trigger => ServerTrigger;

    public ServerEffect(SerializableEffect se, Card thisCard, ServerGame serverGame) : base(se.maxTimesCanUsePerTurn)
    {
        this.thisCard = thisCard ?? throw new System.ArgumentNullException("Effect cannot be attached to null card");
        this.serverGame = serverGame;
        ServerSubeffects = new ServerSubeffect[se.subeffects.Length];
        targets = new List<Card>();
        coords = new List<Vector2Int>();

        if (!string.IsNullOrEmpty(se.trigger))
        {
            try
            {
                ServerTrigger = ServerTrigger.FromJson(se.triggerCondition, se.trigger, this);
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
            }
        }
    }

    public bool CanBeActivatedBy(ServerPlayer controller)
    {
        return Trigger == null
            && controller.index == thisCard.ControllerIndex;
    }

    public void PushToStack(ServerPlayer controller)
    {
        EffectsController.PushToStack(this, controller);
    }

    public void StartResolution()
    {
        thisCard.game.CurrEffect = this;
        ServerController.ServerNotifier.NotifyEffectX(thisCard, EffectIndex, X);
        if (Negated) EffectImpossible();
        else ResolveSubeffect(0);
    }

    public void ResolveNextSubeffect()
    {
        ResolveSubeffect(subeffectIndex + 1);
    }

    public void ResolveSubeffect(int index)
    {
        if (index >= ServerSubeffects.Length)
        {
            FinishResolution();
            return;
        }

        Debug.Log($"Resolving subeffect of type {ServerSubeffects[index].GetType()}");
        subeffectIndex = index;
        ServerSubeffects[index].Resolve();
    }

    /// <summary>
    /// If the effect finishes resolving, this method is called.
    /// Any function can also call this effect to finish resolution early.
    /// </summary>
    private void FinishResolution()
    {
        subeffectIndex = 0;
        X = 0;
        targets.Clear();
        OnImpossible = null;
        ServerController.ServerNotifier.AcceptTarget();
        ServerController.ServerNotifier.NotifyBothPutBack();
        EffectsController.FinishStackEntryResolution();
    }

    /// <summary>
    /// Cancels resolution of the effect, 
    /// or, if there is something pending if the effect becomes impossible, resolves that
    /// </summary>
    public void EffectImpossible()
    {
        Debug.Log($"Effect of {thisCard.CardName} is being declared impossible");
        if (OnImpossible == null) FinishResolution();
        else
        {
            subeffectIndex = OnImpossible.SubeffIndex;
            OnImpossible.OnImpossible();
        }
    }

    public void DeclineAnotherTarget()
    {
        OnImpossible?.OnImpossible();
    }
}
