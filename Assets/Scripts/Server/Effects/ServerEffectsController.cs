using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerEffectsController : MonoBehaviour
{
    public readonly object TriggerStackLock = new object();

    public ServerGame ServerGame;

    protected ServerEffectStack stack;

    public Stack<(ServerTrigger, GameCard, IServerStackable, ServerPlayer, int?, ServerPlayer)> OptionalTriggersToAsk;

    //trigger map
    protected Dictionary<TriggerCondition, List<ServerTrigger>> triggerMap;
    protected Dictionary<TriggerCondition, List<HangingEffect>> hangingEffectMap;

    public IServerStackable CurrStackEntry { get; private set; }

    public void Start()
    {
        stack = new ServerEffectStack();

        OptionalTriggersToAsk = new Stack<(ServerTrigger, GameCard, IServerStackable, ServerPlayer, int?, ServerPlayer)>();

        triggerMap = new Dictionary<TriggerCondition, List<ServerTrigger>>();
        foreach (TriggerCondition c in System.Enum.GetValues(typeof(TriggerCondition)))
        {
            triggerMap.Add(c, new List<ServerTrigger>());
        }

        hangingEffectMap = new Dictionary<TriggerCondition, List<HangingEffect>>();
        foreach (TriggerCondition c in System.Enum.GetValues(typeof(TriggerCondition)))
        {
            hangingEffectMap.Add(c, new List<HangingEffect>());
        }
    }

    #region the stack
    public void PushToStack(IServerStackable eff)
    {
        stack.Push(eff);
    }

    public void PushToStack(ServerEffect eff, ServerPlayer controller)
    {
        eff.serverGame = ServerGame;
        eff.ServerController = controller;
        PushToStack(eff);
    }

    public IServerStackable PopFromStack()
    {
        return stack.Pop();
    }

    public IServerStackable CancelStackEntry(int index)
    {
        return stack.Cancel(index);
    }

    public void ResolveNextStackEntry()
    {
        var eff = stack.Pop();
        if (eff == null)
        {
            ServerGame.TurnServerPlayer.ServerNotifier.DiscardSimples();
            ServerGame.boardCtrl.DiscardSimples();
        }
        else
        {
            CurrStackEntry = eff;
            eff.StartResolution();
        }
    }

    /// <summary>
    /// The last effect that resolved is now done.
    /// </summary>
    public void FinishStackEntryResolution()
    {
        CurrStackEntry = null;
        CheckForResponse();
    }

    public void ResetPassingPriority()
    {
        foreach (var player in ServerGame.ServerPlayers)
        {
            player.passedPriority = false;
        }
    }

    public void OptionalTriggerAnswered(bool answer)
    {
        //TODO: in theory, this would allow anyone to just send a packet that had true or false in it and answer for the player
        //but they can kinda cheat like that with everything here...

        lock (TriggerStackLock)
        {
            if (OptionalTriggersToAsk.Count == 0)
            {
                Debug.LogError($"Tried to answer about a trigger when there weren't any triggers to answer about.");
                return;
            }

            var (t, cardTrigger, stackTrigger, triggeringPlayer, x, controller) = OptionalTriggersToAsk.Pop();
            if (answer) t.OverrideTrigger(x, controller);
            CheckForResponse();
        }
    }

    public void CheckForResponse()
    {
        if (CurrStackEntry != null)
        {
            Debug.Log("Tried to check for response while something is resolving");
            return;
        }

        //since a new thing is being put on the stack, mark both players as having not passed priority
        ResetPassingPriority();

        if (OptionalTriggersToAsk.Count > 0)
        {
            //then ask the respective player about that trigger.
            lock (TriggerStackLock)
            {
                var (t, cardTriggerer, stackTriggerer, triggeringPlayer, x, controller) = OptionalTriggersToAsk.Peek();
                t.effToTrigger.ServerController?.ServerNotifier.AskForTrigger(t, x, cardTriggerer, stackTriggerer, triggeringPlayer);
            }
            //if the player chooses to trigger it, it will be removed from the list
        }
        //check if responses exist. if not, resolve
        else if (ServerGame.TurnServerPlayer.HoldsPriority())
        {
            //then send them a request to do something or pass priority
            //TODO: send the stack entry encoded somehow?
        }
        else if (ServerGame.TurnServerPlayer.ServerEnemy.HoldsPriority())
        {
            //then mark the turn player as having passed priority
            ServerGame.TurnServerPlayer.ServerEnemy.passedPriority = true;

            //then ask the non turn player to do something or pass priority
        }
        else
        {
            //if neither player has anything to do, resolve the stack
            ResolveNextStackEntry();
        }
    }
    #endregion the stack

    #region triggers
    public void RegisterTrigger(TriggerCondition condition, ServerTrigger trigger)
    {
        Debug.Log($"Registering a new trigger from card {trigger.effToTrigger.Source.CardName} to condition {condition}");
        List<ServerTrigger> triggers = triggerMap[condition];
        if (triggers == null)
        {
            triggers = new List<ServerTrigger>();
            triggerMap.Add(condition, triggers);
        }
        triggers.Add(trigger);
    }

    public void RegisterHangingEffect(TriggerCondition condition, HangingEffect hangingEff)
    {
        Debug.Log($"Registering a new hanging effect to condition {condition}");
        List<HangingEffect> hangingEffs = hangingEffectMap[condition];
        hangingEffs.Add(hangingEff);
    }

    public void Trigger(TriggerCondition condition, 
        GameCard cardTriggerer = null, IServerStackable stackTrigger = null, ServerPlayer triggerer = null, int? x = null, (int, int)? space = null)
    {
        List<HangingEffect> toRemove = new List<HangingEffect>();
        foreach (HangingEffect t in hangingEffectMap[condition])
        {
            if (t.EndIfApplicable(cardTriggerer, stackTrigger, triggerer, x, space))
            {
                toRemove.Add(t);
            }
        }
        foreach (var t in toRemove)
        {
            hangingEffectMap[condition].Remove(t);
        }

        Debug.Log($"Attempting to trigger {condition}, with triggerer {cardTriggerer?.CardName}, triggered by a null stacktrigger? {stackTrigger == null}, x={x}");
        foreach (ServerTrigger t in triggerMap[condition])
        {
            t.TriggerIfValid(cardTriggerer, stackTrigger, triggerer, x, space);
        }
    }

    /// <summary>
    /// Adds this trigger to the list that, once a stack entry resolves,
    /// asks the player whose trigger it is if they actually want to trigger that effect
    /// </summary>
    /// <param name="trigger"></param>
    /// <param name="x"></param>
    public void AskForTrigger(ServerTrigger trigger, int? x, GameCard cardTriggerer, IServerStackable stackTriggerer, ServerPlayer triggerer, ServerPlayer controller)
    {
        Debug.Log($"Asking about trigger for effect of card {trigger.effToTrigger.Source.CardName}");
        lock (TriggerStackLock)
        {
            OptionalTriggersToAsk.Push((trigger, cardTriggerer, stackTriggerer, triggerer, x, controller));
        }
    }
    #endregion triggers
}
