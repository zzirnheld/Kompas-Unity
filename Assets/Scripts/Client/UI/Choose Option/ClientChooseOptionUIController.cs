using KompasClient.GameCore;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace KompasClient.UI
{
	public class ClientChooseOptionUIController : MonoBehaviour
	{
		public GameObject optionPrefab;
		public ClientGame clientGame;

		public Transform optionsGridParent;
		public TMP_Text choiceBlurbText;

		public List<ClientEffectOptionUIController> options = new List<ClientEffectOptionUIController>();

		public void ShowEffectOptions(string choiceBlurb, string[] optionBlurbs, bool showX, int x)
		{
			//remove old options
			foreach (var o in options) Destroy(o.gameObject);
			options.Clear();

			//set new information
			choiceBlurbText.text = showX ? $"{choiceBlurb} (X = {x})" : choiceBlurb;

			int i = 0;
			foreach (var blurb in optionBlurbs)
			{
				var obj = Instantiate(optionPrefab, optionsGridParent);
				obj.transform.SetSiblingIndex(0);
				var ctrl = obj.GetComponent<ClientEffectOptionUIController>();
				ctrl.Initialize(this, blurb, i++);
				options.Add(ctrl);
			}

			//make visible
			gameObject.SetActive(true);
		}

		public void ChooseOption(int index)
		{
			clientGame.clientNotifier.RequestChooseEffectOption(index);
			gameObject.SetActive(false);
		}
	}
}