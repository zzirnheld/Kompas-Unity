﻿using System.Collections.Generic;
using UnityEngine;
using KompasCore.Effects;
using KompasServer.GameCore;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

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

        public ServerGame ServerGame;

        private readonly EffectStack<IServerStackable> stack = new EffectStack<IServerStackable>();
        public IEnumerable<IServerStackable> StackEntries => stack.StackEntries;

        //queue of triggers triggered throughout the resolution of the effect, to be ordered after the effect resolves
        private readonly Queue<TriggersTriggered> triggeredTriggers = new Queue<TriggersTriggered>();

        //trigger maps
        private readonly Dictionary<string, List<ServerTrigger>> triggerMap
            = new Dictionary<string, List<ServerTrigger>>();
        private readonly Dictionary<string, List<HangingEffect>> hangingEffectMap
            = new Dictionary<string, List<HangingEffect>>();
        private readonly Dictionary<string, List<HangingEffect>> hangingEffectFallOffMap
            = new Dictionary<string, List<HangingEffect>>();

        private bool currentlyCheckingResponses = false;
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
        public bool NothingHappening => stack.Empty && CurrStackEntry == null;

        public override string ToString()
        {
            if (CurrStackEntry == null) return string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Stack:");
            foreach (var s in stack.StackEntries)
            {
                sb.AppendLine(s.ToString());
            }

            sb.AppendLine("Currently Resolving:");
            sb.AppendLine(CurrStackEntry.ToString());
            if (CurrStackEntry is ServerEffect se)
            {
                if (se.CardTargets.Any())
                {
                    sb.Append("Targets: ");
                    sb.AppendLine(string.Join(", ", se.CardTargets.Select(c => c.ToString())));
                }
                if (se.SpaceTargets.Any())
                {
                    sb.Append("Coords: ");
                    sb.AppendLine(string.Join(", ", se.SpaceTargets.Select(c => c.ToString())));
                }
                sb.AppendLine($"X: {se.X}");
                sb.AppendLine($"Controller: {se.Controller.index}");
            }
            return sb.ToString();
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
            PushToStack(eff as IServerStackable, context);
        }

        public void PushToStack(ServerEffect eff, ActivationContext context) => PushToStack(eff, eff.ServerController, context);

        private async Task StackEmptied()
        {
            //Debug.Log($"Stack is emptied");
            //stack ends
            foreach (var c in ServerGame.Cards) c.ResetForStack();
            ServerGame.boardCtrl.ClearSpells();
            ServerGame.ServerPlayers.First().ServerNotifier.StackEmpty();
            TriggerForCondition(Trigger.StackEnd, new ActivationContext());
            //Must check whether I *should* check for response to avoid an infinite loop
            if (!stack.Empty || triggeredTriggers.Any()) await CheckForResponse();
        }

        public async Task ResolveNextStackEntry()
        {
            var (stackable, context) = stack.Pop();
            if (stackable == null) await StackEmptied();
            else
            {
                //Debug.Log($"Resolving next stack entry: {stackable}, {context}");
                //inform the players that they no longer can respond, in case they were somehow still thinking they could
                foreach (var p in ServerGame.ServerPlayers) p.ServerNotifier.RequestNoResponse();

                //set the current stack entry to the appropriate value. this is used to check if something is currently resolving.
                CurrStackEntry = stackable;

                //actually resolve the thing
                await stackable.StartResolution(context);

                //after it resolves, tell the clients it's done resolving
                ServerGame.ServerPlayers.First().ServerNotifier.RemoveStackEntry(currStackIndex);
                //take note that nothing is resolving
                CurrStackEntry = null;
                //and see if there's antyhing to resolve next.
                await CheckForResponse();
            }
        }

        private void RemoveHangingEffect(HangingEffect hangingEff)
        {
            hangingEffectMap[hangingEff.endCondition].Remove(hangingEff);
            //Not all hanging effects can fall off
            if (!string.IsNullOrEmpty(hangingEff.fallOffCondition))
                hangingEffectFallOffMap[hangingEff.fallOffCondition].Remove(hangingEff);
        }

        /// <summary>
        /// Cancel any stack entries <paramref name="eff"/> had on the stack, and any hanging effects caused by <paramref name="eff"/>
        /// </summary>
        /// <param name="eff"></param>
        public void Cancel(Effect eff)
        {
            //Remove effect from the stack, going top to bottom
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                if (stack.StackEntries.ElementAt(i) == eff)
                {
                    stack.Cancel(i);
                    ServerGame.ServerPlayers.First().ServerNotifier.RemoveStackEntry(i - 1);
                }
            }
            //Remove effect from hanging/delayed
            foreach (var triggerCondition in Trigger.TriggerConditions.Where(hangingEffectMap.ContainsKey))
            {
                foreach (var hangingEff in hangingEffectMap[triggerCondition].ToArray())
                {
                    if (hangingEff.sourceEff == eff) RemoveHangingEffect(hangingEff);
                }
            }
        }

        /// <summary>
        /// Clears the passed priority flag for all players
        /// NOTE: DOESN'T DO ANYTHING RIGHT NOW, since fast effects have been removed from the game,
        /// so to save time, we don't ask players if they have a response.
        /// IF I ever add back fast effects, have it be smarter and allow players
        /// to tell the server in advance if they want to respond,
        /// to avoid having to go back and forth constantly in big stacks
        /// </summary>
        public void ResetPassingPriority()
        {
            foreach (var player in ServerGame.ServerPlayers) player.ResetPassedPriority();
        }
        #endregion the stack

        /// <summary>
        /// Accounts for optional triggers and ordering triggers, then pushes appropriate triggers to the stack.
        /// </summary>
        /// <param name="turnPlayer">The turn player, whose effects get pushed to the stack first.</param>
        private async Task CheckTriggers(ServerPlayer turnPlayer)
        {
            //get the list of triggers, and see if they're all still valid
            var triggered = triggeredTriggers.Dequeue();
            var stillValid = triggered.triggers.Where(t => t.StillValidForContext(triggered.context));

            //if there's no triggers, skip all this logic
            if (!stillValid.Any())
            {
                Debug.Log($"All the triggers that were valid from {string.Join(",", triggered.triggers)} aren't anymore");
                return;
            }

            //if any triggers have not been responded to, make them get responded to.
            //this is saved so that we know what trigger to okay or not if it's responded
            foreach (var t in stillValid)
            {
                if (!t.Responded) await t.Ask(triggered.context.x ?? 0);
            }

            //now that all optional triggers have been answered, time to deal with ordering.
            //if a player only has one trigger, don't bother asking them for an order.
            foreach (var p in ServerGame.Players)
            {
                var thisPlayers = stillValid.Where(t => t.serverEffect.Controller == p && t.Confirmed);
                if (thisPlayers.Count() == 1) thisPlayers.First().Order = 1;
            }

            //now, if there's any triggers that have been confirmed but not ordered (that is, more than one confirmed trigger),
            //then get an ordering from the player in question.
            var confirmed = stillValid.Where(t => t.Confirmed);
            if (!confirmed.All(t => t.Ordered))
            {
                //create a list to hold the tasks, so you can get trigger orderings from both players at once.
                List<Task> triggerOrderings = new List<Task>();
                foreach (var p in ServerGame.ServerPlayers)
                {
                    var thisPlayers = confirmed.Where(t => t.serverEffect.Controller == p);
                    if (thisPlayers.Any(t => !t.Ordered)) triggerOrderings.Add(p.serverAwaiter.GetTriggerOrder(thisPlayers));
                }
                await Task.WhenAll(triggerOrderings);
            }

            //finally, push the triggers to the stack, in the proscribed order, starting with the turn player's
            foreach (var t in confirmed.Where(t => t.serverEffect.Controller == turnPlayer).OrderBy(t => t.Order))
                PushToStack(t.serverEffect, triggered.context);
            foreach (var t in confirmed.Where(t => t.serverEffect.Controller == turnPlayer.Enemy).OrderBy(t => t.Order))
                PushToStack(t.serverEffect, triggered.context);
        }

        /// <summary>
        /// Checks all triggers to see if any need to be addressed before stack resolution can continue.
        /// </summary>
        /// <param name="turnPlayer">The current turn player, who gets the first chance to accept or decline their triggers.</param>
        /// <returns></returns>
        private async Task CheckAllTriggers(ServerPlayer turnPlayer)
        {
            //note: you cannot use .Any(t => CheckTriggers(t)) because the collection would be modified while iterating
            //instead, just .Any() checks the queue after each time it's modified
            while (triggeredTriggers.Any())
            {
                await CheckTriggers(turnPlayer: turnPlayer);
                foreach (var tList in triggerMap.Values)
                {
                    foreach (var t in tList) t.ResetConfirmation();
                }
            }
        }

        public async Task CheckForResponse(bool reset = true)
        {
            //if we're already checking for response, don't check again.
            //checking again could cause us to consider the same set of triggers twice,
            //then dequeue twice, which would not consider that set of triggers.
            if (currentlyCheckingResponses || CurrStackEntry != null)
            {
                Debug.Log($"Checked response while currently checking for response {currentlyCheckingResponses} " +
                    $"or curr stack entry not null {CurrStackEntry != null}");
                return;
            }
            currentlyCheckingResponses = true;

            if (reset) ResetPassingPriority();

            await CheckAllTriggers(ServerGame.TurnServerPlayer);

            var playersHoldingPriority = ServerGame.ServerPlayers
                .Where(player => !player.PassedPriority)
                .ToArray(); //call toArray so that we don't create the collection twice.

            //for any player that is holding priority, request a response from them
            if (playersHoldingPriority.Any())
            {
                foreach (var p in playersHoldingPriority) p.ServerNotifier.RequestResponse();
                //after asking for responses, then we're done with this CheckForResponse call. 
                //now, if any more response-inducing events come in, we won't ask for responses an extra time,
                //nor will we check for triggers an extra time.
                currentlyCheckingResponses = false;
            }
            //if no one holds priority, move on to the next effect
            else
            {
                //ResolveNextStackEntry eventually calls CheckForResponse again.
                //turn the flag off so that we can reenter CheckForResponse by the time that happens.
                currentlyCheckingResponses = false;
                await ResolveNextStackEntry();
            }
        }

        private void ResolveHangingEffects(string condition, ActivationContext context)
        {
            if (hangingEffectMap.ContainsKey(condition))
            {
                foreach (var toEnd in hangingEffectMap[condition].ToArray())
                {
                    if (toEnd.EndIfApplicable(context)) RemoveHangingEffect(toEnd);
                }
            }

            if (hangingEffectFallOffMap.ContainsKey(condition))
            {
                foreach (var toRemove in hangingEffectFallOffMap[condition].ToArray())
                {
                    if (toRemove.ShouldBeCanceled(context)) RemoveHangingEffect(toRemove);
                }
            }
        }

        public void TriggerForCondition(string condition, params ActivationContext[] contexts)
        {
            foreach (var c in contexts) TriggerForCondition(condition, c);
        }

        public void TriggerForCondition(string condition, ActivationContext context)
        {
            if (!ServerGame.GameHasStarted) return;

            Debug.Log($"Triggering for condition {condition}, context {context}");
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
                Debug.Log($"Triggers triggered: {string.Join(", ", triggers.triggers.Select(t => t.Source.ID + t.Blurb))}");
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

        public void RegisterHangingEffect(string condition, HangingEffect hangingEff, string fallOffCondition = default)
        {
            Debug.Log($"Registering a new hanging effect to condition {condition}");
            if (!hangingEffectMap.ContainsKey(condition))
                hangingEffectMap.Add(condition, new List<HangingEffect>());

            hangingEffectMap[condition].Add(hangingEff);

            if (fallOffCondition != default) RegisterHangingEffectFallOff(fallOffCondition, hangingEff);
        }

        private void RegisterHangingEffectFallOff(string condition, HangingEffect hangingEff)
        {
            Debug.Log($"Registering a new hanging effect to condition {condition}");
            if (!hangingEffectFallOffMap.ContainsKey(condition))
                hangingEffectFallOffMap.Add(condition, new List<HangingEffect>());

            hangingEffectFallOffMap[condition].Add(hangingEff);
        }
        #endregion register to trigger condition
    }
}