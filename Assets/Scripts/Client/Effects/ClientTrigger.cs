using KompasCore.Effects;
using UnityEngine;

namespace KompasClient.Effects
{
    public class ClientTrigger : Trigger
    {
        public ClientEffect ClientEffect { get; private set; }

        public static ClientTrigger FromJson(string condition, string json, ClientEffect parent)
        {
            var toReturn = JsonUtility.FromJson<ClientTrigger>(json);

            if (toReturn != null)
            {
                //set all values shared by all triggers
                toReturn.triggerCondition = condition;
                toReturn.ClientEffect = parent;
            }

            return toReturn;
        }
    }
}