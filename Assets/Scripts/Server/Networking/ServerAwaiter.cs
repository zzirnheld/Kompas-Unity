using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.Effects;
using KompasServer.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Networking {
    public class ServerAwaiter : MonoBehaviour
    {
        private const int DefaultDelay = 100;
        private const int TargetCheckDelay = 100;

        public ServerNotifier serverNotifier;
        public ServerNetworkController serverNetCtrl;

        #region awaited values
        /*the following are properties so that later i can override the setter to 
         * also signal a semaphore to awaken the relevant awaiting request. */
        public bool? OptionalTriggerAnswer { get; set; }
        public (int[] cardIds, int[] effIndices, int[] orders)? TriggerOrders { get; set; }
        public GameCard CardTarget { get; set; }
        #endregion awaited values

        #region locks
        private readonly object OptionalTriggerLock = new object();
        private readonly object TriggerOrderLock = new object();
        private readonly object CardTargetLock = new object();
        #endregion locks

        public async Task<bool> GetOptionalTriggerChoice(ServerTrigger trigger)
        {
            serverNotifier.AskForTrigger(trigger);
            //TODO later use a semaphore or something to signal once packet is available
            while (true)
            {
                lock (OptionalTriggerLock)
                {
                    if (OptionalTriggerAnswer.HasValue)
                    {
                        bool answer = OptionalTriggerAnswer.Value;
                        OptionalTriggerAnswer = null;
                        return answer;
                    }
                }
                
                await Task.Delay(DefaultDelay); 
            }
        }

        public async Task GetTriggerOrder(IEnumerable<ServerTrigger> triggers)
        {
            serverNotifier.GetTriggerOrder(triggers);
            while (true)
            {
                lock (TriggerOrderLock)
                {
                    if (TriggerOrders.HasValue)
                    {
                        (int[] cardIds, int[] effIndices, int[] orders) = TriggerOrders.Value;
                        for (int i = 0; i < effIndices.Length; i++)
                        {
                            //TODO deal with garbage values here
                            var card = serverNetCtrl.sGame.GetCardWithID(cardIds[i]);
                            if (card == null) continue;
                            if (card.Effects.ElementAt(effIndices[i]).Trigger is ServerTrigger trigger)
                                trigger.Order = orders[i];
                        }

                        TriggerOrders = null;
                        return;
                    }
                }
                
                await Task.Delay(DefaultDelay);
            }
        }

        public async Task<GameCard> GetCardTarget(string sourceCardName, string blurb, int[] ids, string listRestrictionJson)
        {
            serverNotifier.GetCardTarget(sourceCardName, blurb, ids, listRestrictionJson);
            while (true)
            {
                lock (CardTargetLock)
                {
                    if (CardTarget != null)
                    {
                        var target = CardTarget;
                        CardTarget = null;
                        return target;
                    }
                }

                await Task.Delay(TargetCheckDelay);
            }
        }
    }
}