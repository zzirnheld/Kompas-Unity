using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.UI
{
    public class SpaceCueController : MonoBehaviour
    {
        public enum CueType
        {
            Move,
            MoveOpenGamestate, //Whether the card could be moved if we were in an open gamestate
            Attack,
            Play,
            Target,
        }

        private static readonly CueType[] MutuallyExclusiveCues = { CueType.Move, CueType.MoveOpenGamestate, CueType.Attack, CueType.Play };

        [EnumNamedArray(typeof(CueType))]
        public GameObject[] cueCubes; //Should be in order of the above enum

        public void Show(CueType cue, bool active = true)
        {
            if (MutuallyExclusiveCues.Contains(cue))
            {
                Clear(MutuallyExclusiveCues);
            }

            cueCubes[(int) cue].SetActive(active);
        }

        public void Clear()
        {
            Clear(MutuallyExclusiveCues);
        }

        private void Clear(params CueType[] cues)
        {
            foreach (var ct in cues) cueCubes[(int) ct].SetActive(false);
        }
    }
}