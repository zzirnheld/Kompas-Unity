using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerEffect : Effect, IStackable
{
    public ServerSubeffect[] Subeffects { get; }
    public ServerTrigger Trigger { get; }
    
    public ServerGame serverGame;

    //get the currently resolving subeffect
    public ServerSubeffect CurrSubeffect { get { return Subeffects[subeffectIndex]; } }
    public ServerPlayer EffectController { get { return serverGame.ServerPlayers[effectControllerIndex]; } }

    public override Player Controller => EffectController;

    public ServerEffect(SerializableEffect se, Card thisCard, int controller) : base(se.maxTimesCanUsePerTurn)
    {
        this.thisCard = thisCard ?? throw new System.ArgumentNullException("Effect cannot be attached to null card");
        this.serverGame = thisCard.game as ServerGame;
        Subeffects = new ServerSubeffect[se.subeffects.Length];
        targets = new List<Card>();
        coords = new List<Vector2Int>();

        if (!string.IsNullOrEmpty(se.trigger))
        {
            try
            {
                Trigger = ServerTrigger.FromJson(se.triggerCondition, se.trigger, this);
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
                Subeffects[i] = ServerSubeffect.FromJson(se.subeffectTypes[i], se.subeffects[i], this, i);
            }
            catch (System.ArgumentException)
            {
                Debug.LogError($"Failed to load subeffect of type {se.subeffectTypes[i]} from json {se.subeffects[i]}");
            }
        }
    }

    public void PushToStack(int controller)
    {
        serverGame.PushToStack(this, controller);
    }

    public void StartResolution()
    {
        thisCard.game.CurrEffect = this;
        EffectController.ServerNotifier.NotifyEffectX(thisCard, EffectIndex, X);
        if (Negated) EffectImpossible();
        else ResolveSubeffect(0);
    }

    public void ResolveNextSubeffect()
    {
        ResolveSubeffect(subeffectIndex + 1);
    }

    public void ResolveSubeffect(int index)
    {
        if (index >= Subeffects.Length)
        {
            FinishResolution();
            return;
        }

        Debug.Log($"Resolving subeffect of type {Subeffects[index].GetType()}");
        subeffectIndex = index;
        Subeffects[index].Resolve();
    }

    /// <summary>
    /// If the effect finishes resolving, this method is called.
    /// Any function can also call this effect to finish resolution early.
    /// </summary>
    private void FinishResolution()
    {
        doneResolving = true;
        subeffectIndex = 0;
        X = 0;
        targets.Clear();
        OnImpossible = null;
        EffectController.ServerNotifier.AcceptTarget();
        EffectController.ServerNotifier.NotifyBothPutBack();
        serverGame.FinishStackEntryResolution();
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
