using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

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

		private BoardUIController boardUIController;
		private Space position;

		public void Init(BoardUIController boardUIController, Space position)
		{
			this.boardUIController = boardUIController;
			this.position = position;
		}

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

		public void OnMouseUp()
		{
			//don't do anything if we're over an event system object, 
			//because that would let us click on cards underneath prompts
			if (EventSystem.current.IsPointerOverGameObject()) return;

			//select cards if the player releases the mouse button while over one
			boardUIController.Clicked(position);
		}
	}
}