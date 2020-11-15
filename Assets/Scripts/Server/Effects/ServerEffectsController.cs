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

            public TriggersTriggered(IEnumerable<ServerTrigger> triggers, ActivationContext context)
            {
                this.triggers = triggers;
                this.context = context;
            }
        }

        private readonly object triggerStackLock = new object();
        private readonly object responseLock = new object();

        public ServerGame ServerGame;

        private readonly ServerEffectStack stack = new ServerEffectStack();

        //queue of triggers triggered throughout the resolution of the effect, to be ordered after the effect resolves
        private Queue<TriggersTriggered> triggeredTriggers = new Queue<TriggersTriggered>();
        //current optional trigger considered, if any
        private ServerTrigger currentOptionalTrigger;

        //trigger maps
        private readonly Dictionary<string, List<ServerTrigger>> triggerMap
            = new Dictionary<string, List<ServerTrigger>>();
        private readonly Dictionary<string, List<HangingEffect>> hangingEffectMap
            = new Dictionary<string, List<HangingEffect>>();
        private readonly Dictionary<string, List<HangingEffect>> hangingEffectFallOffMap
            = new Dictionary<string, List<HangingEffect>>();

        private bool priorityHeld = false;
        private int currStackIndex;
        private IServerStackable currStackEntry;
        public IServerStackable CurrStackEntry 
        {
            get => currStackEntry;
            private set
            {
                currStackEntry = value;
                currStackIndex = stack.Count;
            } 
        }
        
        //nothing is happening if nothing is in the stack, nothing is currently resolving, and no one is waiting to add something to the stack.
        public bool NothingHappening => stack.Empty && CurrStackEntry == null && !priorityHeld;

        #region the stack
        public void PushToStack(IServerStackable eff, ActivationContext context)
        {
            ResetPassingPriority();
            stack.Push((eff, context));
        }

        public void PushToStack(ServerEffect eff, ServerPlayer controller, ActivationContext context)
        {
            eff.PushedToStack(ServerGame, controller);
            PushToStack(eff as IServerStackable, context);
        }

        public void PushToStack(ServerEffect eff, ActivationContext context) => PushToStack(eff, eff.ServerController, context);

        private void StackEmptied()
        {
            //stack ends
            foreach (var c in ServerGame.Cards) c.ResetForStack();
            ServerGame.boardCtrl.ClearSpells();
            ServerGame.ServerPlayers.First().ServerNotifier.StackEmpty();
        }

        public void ResolveNextStackEntry()
        {
            var (stackable, context) = stack.Pop();
            Debug.Log($"Resolving next stack entry: {stackable}, {context}");
            if (stackable == null) StackEmptied();
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
            ServerGame.ServerPlayers.First().ServerNotifier.RemoveStackEntry(currStackIndex);
            CheckForResponse();
        }

        public void ResetPassingPriority()
        {
            foreach (var player in ServerGame.ServerPlayers) player.passedPriority = false;
        }
        #endregion the stack

        /// <summary>
        /// Checks to see if anything needs to be done with triggers before checking for other responses
        /// </summary>
        /// <param name="turnPlayer"></param>
        /// <returns><see langword="true"/> if all triggers have been addressed, <see langword="false"/> otherwise</returns>
        private bool CheckTriggers(ServerPlayer turnPlayer)
        {
            //then ask the respective player about that trigger.
            lock (triggerStackLock)
            {
                //get the list of triggers, and see if they're all still valid
                var triggered = triggeredTriggers.Peek();
                var list = triggered.triggers.Where(t => t.StillValidForContext(triggered.context));

                //if there's no triggers, skip all this logic
                if (!list.Any())
                {
                    Debug.Log($"Triggers that were valid from {string.Join(",", triggered.triggers)} aren't anymore");
                    triggeredTriggers.Dequeue();
                    return true;
                }

                //if any triggers have not been responded to, make them get responded to.
                //this is saved so that we know what trigger to okay or not if it's responded
                currentOptionalTrigger = list.FirstOrDefault(t => !t.Responded);
                if (currentOptionalTrigger != default)
                {
                    currentOptionalTrigger.Ask();
                    return false;
                }

                //now that all effects have been addressed, see if there's any
                foreach(var p in ServerGame.Players)
                {
                    var thisPlayers = list.Where(t => t.serverEffect.Controller == p && t.Confirmed);
                    if (thisPlayers.Count() == 1) thisPlayers.First().Order = 1;
                }

                //if all triggers have been responded to
                var confirmed = list.Where(t => t.Confirmed);
                if (confirmed.All(t => t.Ordered))
                {
                    foreach (var t in confirmed.Where(t => t.serverEffect.Controller == turnPlayer).OrderBy(t => t.Order))
                        PushToStack(t.serverEffect, triggered.context);
                    foreach (var t in confirmed.Where(t => t.serverEffect.Controller == turnPlayer.Enemy).OrderBy(t => t.Order))
                        PushToStack(t.serverEffect, triggered.context);
                    triggeredTriggers.Dequeue();
                    return true;
                }
                else
                {
                    foreach (var p in ServerGame.ServerPlayers)
                    {
                        var thisPlayers = confirmed.Where(t => t.serverEffect.Controller == p);
                        if (thisPlayers.Any(t => !t.Ordered)) p.ServerNotifier.GetTriggerOrder(thisPlayers);
                    }
                    return false;
                }
            }
        }
        
        /// <summary>
        /// Checks all triggers to see if any need to be addressed before stack resolution can continue.
        /// </summary>
        /// <param name="turnPlayer">The current turn player, who gets the first chance to accept or decline their triggers.</param>
        /// <returns></returns>
        private bool CheckAllTriggers(ServerPlayer turnPlayer)
        {
            //note: you cannot use .Any(t => CheckTriggers(t)) because the collection would be modified while iterating
            //instead, just .Any() checks the queue after each time it's modified
            while (triggeredTriggers.Any())
            {
                if (!CheckTriggers(turnPlayer: turnPlayer)) return false;
                foreach(var tList in triggerMap.Values)
                {
                    foreach (var t in tList) t.ResetConfirmation();
                }
            }
            return true;
        }

        public void CheckForResponse(bool reset = true)
        {
            if (CurrStackEntry != null)
            {
                Debug.LogWarning($"Tried to check for response while {CurrStackEntry} is resolving");
                return;
            }

            if (reset) ResetPassingPriority();

            //if there's any triggers triggered that haven't been dealt with, mark priority being held and return
            if (!CheckAllTriggers(ServerGame.TurnServerPlayer))
            {
                priorityHeld = true;
                return;
            }

            //check if responses exist. if not, resolve
            lock (responseLock)
            {
                var players = ServerGame.ServerPlayers
                    .Where(player => !player.passedPriority &&
                        ServerGame.Cards.Any(c => c.Effects.Any(e => e.ActivationRestriction.Evaluate(player))))
                    .ToArray(); //call toArray so that we don't create the collection twice.
                //remove the .ToArray() later if it turns out Linq is smart enough to only execute once, but I'm pretty sure it can't know.

                //if there's any players, that means some are holding priority
                priorityHeld = players.Any();
                //for any player that is holding priority, request a response from them. Does nothing if no one holds priority.
                foreach (var p in players) p.ServerNotifier.RequestResponse();
            }

            //if no one held priority, great! resolve the next stack entry.
            if (!priorityHeld) ResolveNextStackEntry();
        }

        public void OptionalTriggerAnswered(bool answered, Player answerer)
        {
            if (currentOptionalTrigger != default)
            {
                currentOptionalTrigger.Answered(answered, answerer);
                currentOptionalTrigger = default;
                CheckForResponse();
            }
        }

        private void ResolveHangingEffects(string condition, ActivationContext context)
        {
            if (hangingEffectMap.ContainsKey(condition))
            {
                var endedEffects = hangingEffectMap[condition]
                    .Where(he => he.EndIfApplicable(context))
                    .ToArray(); //must toArray because the map will be modified while iterating.
                foreach (var t in endedEffects)
                {
                    hangingEffectMap[condition].Remove(t);
                    if (!string.IsNullOrEmpty(t.FallOffCondition))
                        hangingEffectFallOffMap[t.FallOffCondition].Remove(t);
                }
            }

            if (hangingEffectFallOffMap.ContainsKey(condition))
            {
                var fallOffToRemove = hangingEffectFallOffMap[condition]
                    .Where(he => he.FallOffRestriction.Evaluate(context))
                    .ToArray(); //must toArray because the map will be modified while iterating.
                foreach (var toRemove in fallOffToRemove)
                {
                    hangingEffectMap[toRemove.EndCondition].Remove(toRemove);
                    hangingEffectFallOffMap[condition].Remove(toRemove);
                }
            }
        }

        public void TriggerForCondition(string condition, params ActivationContext[] contexts)
        {
            foreach (var c in contexts) TriggerForCondition(condition, c);
        }

        public void TriggerForCondition(string condition, ActivationContext context)
        {
            //first resolve any hanging effects
            ResolveHangingEffects(condition, context);

            if (triggerMap.ContainsKey(condition))
            {
                /* Needs to be toArray()ed because cards might move out of correct state after this moment.
                 * Later, when triggers are being ordered, stuff like 1/turn will be rechecked. */
                var validTriggers = triggerMap[condition]
                    .Where(t => t.ValidForContext(context))
                    .ToArray();
                if (!validTriggers.Any()) return;
                var triggers = new TriggersTriggered(triggers: validTriggers, context: context);
                Debug.Log($"Triggers triggered for condition {condition}, context {context}: " +
                    $"{string.Join(", ", triggers.triggers.Select(t => t.blurb))}");
                lock (triggerStackLock)
                {
                    triggeredTriggers.Enqueue(triggers);
                }
            }
        }

        #region register to trigger condition
        public void RegisterTrigger(string condition, ServerTrigger trigger)
        {
            Debug.Log($"Registering a new trigger from card {trigger.serverEffect.Source.CardName} to condition {condition}");
            if (!triggerMap.ContainsKey(condition)) 
                triggerMap.Add(condition, new List<ServerTrigger>());

            triggerMap[condition].Add(trigger);
        }

        public void RegisterHangingEffect(string condition, HangingEffect hangingEff)
        {
            Debug.Log($"Registering a new hanging effect to condition {condition}");
            if (!hangingEffectMap.ContainsKey(condition))
                hangingEffectMap.Add(condition, new List<HangingEffect>());

            hangingEffectMap[condition].Add(hangingEff);
        }

        public void RegisterHangingEffectFallOff(string condition, TriggerRestriction restriction, HangingEffect hangingEff)
        {
            Debug.Log($"Registering a new hanging effect to condition {condition}");
            if (!hangingEffectFallOffMap.ContainsKey(condition))
                hangingEffectFallOffMap.Add(condition, new List<HangingEffect>());

            hangingEffectFallOffMap[condition].Add(hangingEff);
        }
        #endregion register to trigger condition
    }
}