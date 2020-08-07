using System.Collections.Generic;
using UnityEngine;
using KompasCore.Effects;
using KompasServer.GameCore;
using System.Linq;

namespace KompasServer.Effects
{
    public class ServerEffectsController : MonoBehaviour
    {
        private struct TriggersTriggered
        {
            public IEnumerable<ServerTrigger> triggers;
            public ActivationContext context;
        }

        private readonly object triggerStackLock = new object();
        private readonly object responseLock = new object();

        public ServerGame ServerGame;

        private readonly ServerEffectStack stack = new ServerEffectStack();

        //queue of triggers triggered throughout the resolution of the effect, to be ordered after the effect resolves
        private readonly Queue<TriggersTriggered> triggeredTriggers = new Queue<TriggersTriggered>();
        //current optional trigger considered, if any
        private ServerTrigger currentOptionalTrigger;

        //trigger maps
        private readonly Dictionary<string, List<ServerTrigger>> triggerMap 
            = new Dictionary<string, List<ServerTrigger>>();
        private readonly Dictionary<string, List<HangingEffect>> hangingEffectMap 
            = new Dictionary<string, List<HangingEffect>>();
        private readonly Dictionary<string, List<(HangingEffect he, TriggerRestriction tr)>> hangingEffectFallOffMap
            = new Dictionary<string, List<(HangingEffect, TriggerRestriction)>>();

        public IServerStackable CurrStackEntry { get; private set; }

        public void Start()
        {
            foreach (var c in Trigger.TriggerConditions)
            {
                triggerMap.Add(c, new List<ServerTrigger>());
                hangingEffectMap.Add(c, new List<HangingEffect>());
                hangingEffectFallOffMap.Add(c, new List<(HangingEffect, TriggerRestriction)>());
            }
        }

        #region the stack
        public void PushToStack(IServerStackable eff, ActivationContext context)
        {
            ResetPassingPriority();
            stack.Push((eff, context));
        }

        public void PushToStack(ServerEffect eff, ServerPlayer controller, ActivationContext context)
        {
            eff.PushedToStack(ServerGame, controller);
            PushToStack(eff, context);
        }

        public void PushToStack(ServerEffect eff, ActivationContext context) => PushToStack(eff, eff.ServerController, context);

        public IServerStackable CancelStackEntry(int index)
        {
            return stack.Cancel(index);
        }

        public void ResolveNextStackEntry()
        {
            var (stackable, context) = stack.Pop();
            Debug.Log($"Resolving next stack entry: {stackable}, {context}");
            if (stackable == null)
            {
                ServerGame.TurnServerPlayer.ServerNotifier.DiscardSimples();
                ServerGame.boardCtrl.DiscardSimples();
                ServerGame.ServerPlayers.First().ServerNotifier.StackEmpty();
            }
            else
            {
                foreach (var p in ServerGame.ServerPlayers) p.ServerNotifier.RequestNoResponse();
                CurrStackEntry = stackable;
                stackable.StartResolution(context);
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

        public void CheckForResponse(bool reset = true)
        {
            if (CurrStackEntry != null)
            {
                Debug.Log($"Tried to check for response while {CurrStackEntry?.Source?.CardName} is resolving");
                return;
            }

            if (reset) ResetPassingPriority();

            //if there's any triggers triggered
            if (triggeredTriggers.Count > 0)
            {
                //then ask the respective player about that trigger.
                lock (triggerStackLock)
                {
                    var triggered = triggeredTriggers.Peek();
                    var list = triggered.triggers;
                    //if any triggers have not been responded to, make them get responded to
                    currentOptionalTrigger = list.FirstOrDefault(t => !t.Responded);
                    if(currentOptionalTrigger != default)
                    {
                        currentOptionalTrigger.effToTrigger.ServerController.ServerNotifier.AskForTrigger(currentOptionalTrigger);
                        return;
                    }
                    //if all triggers have been responded to
                    else
                    {
                        //push the ones that are confirmed (i.e. mandatory or accepted) TODO here's where I need to put the ordering
                        foreach (var t in list.Where(t => t.Confirmed)) PushToStack(t.effToTrigger, triggered.context);
                        //and reset them all, for the next time triggering might happen
                        foreach (var t in list) t.ResetConfirmation();
                    }
                }
                //if the player chooses to trigger it, it will be removed from the list
            }

            //check if responses exist. if not, resolve
            lock (responseLock)
            {
                //TODO if any player can activate effects, do ServerGame.Players.Any() the entire expression
                var players = ServerGame.Cards.Where(c => c.Effects.Any(e => e.ActivationRestriction.Evaluate(c.Controller)))
                    .Select(c => c.Controller).Distinct().Where(p => !p.passedPriority);

                //TODO figure out a way to not resolve the deferred execution of Linq twice
                if (players.Any()) foreach (var p in players) ServerGame.ServerPlayers[p.index].ServerNotifier.RequestResponse();
                //if neither player has anything to do, resolve the stack
                else ResolveNextStackEntry();
            }
        }
        #endregion the stack

        #region triggers
        public void RegisterTrigger(string condition, ServerTrigger trigger)
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

        public void RegisterHangingEffect(string condition, HangingEffect hangingEff)
        {
            Debug.Log($"Registering a new hanging effect to condition {condition}");
            List<HangingEffect> hangingEffs = hangingEffectMap[condition];
            hangingEffs.Add(hangingEff);
        }

        public void RegisterHangingEffectFallOff(string condition, TriggerRestriction restriction, HangingEffect hangingEff)
        {
            Debug.Log($"Registering a new hanging effect to condition {condition}");
            var hangingEffs = hangingEffectFallOffMap[condition];
            hangingEffs.Add((hangingEff, restriction));
        }

        public void TriggerForCondition(string condition, params ActivationContext[] contexts)
        {
            foreach (var c in contexts) TriggerForCondition(condition, c);
        }

        public void TriggerForCondition(string condition, ActivationContext context)
        {
            Debug.Log($"Attempting to trigger {condition}, with context {context}");
            var endedEffects = hangingEffectMap[condition].Where(he => he.EndIfApplicable(context));
            foreach (var t in endedEffects) hangingEffectMap[condition].Remove(t);

            var fallOffToRemove = hangingEffectFallOffMap[condition].Where((he) => he.tr.Evaluate(context));
            foreach (var toRemove in fallOffToRemove)
            {
                hangingEffectMap[toRemove.he.EndCondition].Remove(toRemove.he);
                hangingEffectFallOffMap[condition].Remove(toRemove);
            }

            /* 
             * this might need to be .toArray()ed, but it might not
             * since linq builds a query, this might be executed only when the enumerable is iterated through 
             * which would be exactly what we need: 
             * for it to only check the triggers to be ordered only after the others have been put on the stack. 
             */
            var triggers = new TriggersTriggered 
            { 
                triggers = triggerMap[condition].Where(t => t.ValidForContext(context)), 
                context = context 
            };  
            triggeredTriggers.Enqueue(triggers);
        }

        public void OptionalTriggerAnswered(bool answered)
        {
            if(currentOptionalTrigger != default)
            {
                currentOptionalTrigger.Confirmed = answered;
                currentOptionalTrigger.Responded = true;
            }
        }
        #endregion triggers
    }
}