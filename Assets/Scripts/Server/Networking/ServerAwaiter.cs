using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Networking {
    public class ServerAwaiter : MonoBehaviour
    {
        private const int TargetCheckDelay = 100;

        public ServerNotifier serverNotifier;
        public ServerNetworkController serverNetCtrl;

        private Dictionary<string, Queue<Packet>> packets;

        #region locks
        private readonly object packetDictLock = new object();
        #endregion locks

        private Packet PacketForCommand(string command)
        {
            if (packets.ContainsKey(command))
            {
                var q = packets[command];
                if (q == null || q.Count == 0) return null;
                else return q.Dequeue();
            }
            else return null;
        }

        public void EnqueuePacket(Packet p)
        {
            lock (packetDictLock)
            {
                if (packets[p.command] == null) packets[p.command] = new Queue<Packet>();
                packets[p.command].Enqueue(p);
            }
        }

        public async Task<GameCard> GetCardTarget(string sourceCardName, string blurb, int[] ids, string listRestrictionJson)
        {
            serverNotifier.GetCardTarget(sourceCardName, blurb, ids, listRestrictionJson);
            while (true)
            {
                var packet = PacketForCommand(Packet.CardTargetChosen);
                if (packet is CardTargetServerPacket p) return p.Target;
                else await Task.Delay(TargetCheckDelay);
            }
        }
    }
}