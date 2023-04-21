using UnityEngine;
using UnityEngine.UI;
using KompasCore.Effects;

namespace KompasClient.UI
{
	public class TriggerUIController : MonoBehaviour
	{
		public Image image;
		public TMPro.TMP_Text orderText;
		public TMPro.TMP_Text blurbText;

		private int index = -1;
		public int Index
		{
			get => index;
			set
			{
				index = value;
				orderText.text = index < 0 ? "" : index.ToString();
			}
		}
		public bool Indexed => Index != -1;

		public Trigger Trigger { get; private set; }

		private TriggerOrderUIController triggerOrderUI;

		public void SetInfo(TriggerOrderUIController triggerOrderUI, Sprite sprite, Trigger trigger)
		{
			this.triggerOrderUI = triggerOrderUI;
			image.sprite = sprite;
			orderText.text = "";
			blurbText.text = trigger.Blurb;
			Trigger = trigger;
		}

		public void OnClick()
		{
			if (Index == -1) Index = triggerOrderUI.CurrIndex++;
			else
			{
				Index = -1;
				triggerOrderUI.CurrIndex--;
			}
		}
	}
}