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
        public IServerStackable CurrStackEntry { get; private set; }
        public bool StackEmpty => stack.Empty && !priorityHeld;

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
                //stack ends
                foreach (var c in ServerGame.Cards) c.ResetForStack();
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
                    var thisPlayers = list.Where(t => t.serverEffect.Controller == p);
                    if (thisPlayers.Count() == 1) thisPlayers.First().order = 1;
                }

                //if all triggers have been responded to
                var confirmed = list.Where(t => t.Confirmed);
                if (confirmed.All(t => t.Ordered))
                {
                    foreach (var t in confirmed.Where(t => t.serverEffect.Controller == turnPlayer).OrderBy(t => t.order))
                        PushToStack(t.serverEffect, triggered.context);
                    foreach (var t in confirmed.Where(t => t.serverEffect.Controller == turnPlayer.Enemy).OrderBy(t => t.order))
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
        
        private bool CheckAllTriggers(ServerPlayer turnPlayer)
        {
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
                Debug.Log($"Tried to check for response while {CurrStackEntry?.Source?.CardName} is resolving");
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
                    .ToArray();

                if (players.Any())
                {
                    foreach (var p in players) p.ServerNotifier.RequestResponse();
                    priorityHeld = true;
                }
                //if neither player has anything to do, resolve the stack
                else
                {
                    priorityHeld = false;
                    ResolveNextStackEntry();
                }
            }
        }

        public void OptionalTriggerAnswered(bool answered, Player answerer)
        {
            if (currentOptionalTrigger != default)
            {
                currentOptionalTrigger.Answered(answered, answerer);
                currentOptionalTrigger = default;
            }

            CheckForResponse();
        }

        public void TriggerForCondition(string condition, params ActivationContext[] contexts)
        {
            foreach (var c in contexts) TriggerForCondition(condition, c);
        }

        public void TriggerForCondition(string condition, ActivationContext context)
        {
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
                Debug.Log($"Triggers triggered: {string.Join(", ", triggers.triggers.Select(t => t.blurb))}");
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

        private void ResolveHangingEffects(string condition, ActivationContext context)
        {
            Debug.Log($"Attempting to trigger {condition}, with context {context}");
            if (hangingEffectMap.ContainsKey(condition))
            {
                var endedEffects = hangingEffectMap[condition]
                    .Where(he => he.EndIfApplicable(context))
                    .ToArray();
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
                    .ToArray();
                foreach (var toRemove in fallOffToRemove)
                {
                    hangingEffectMap[toRemove.EndCondition].Remove(toRemove);
                    hangingEffectFallOffMap[condition].Remove(toRemove);
                }
            }
        }
        #endregion register to trigger condition
    }
}