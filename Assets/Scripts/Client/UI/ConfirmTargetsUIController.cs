using KompasClient.GameCore;
using KompasCore.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KompasClient.UI
{
    public class ConfirmTargetsUIController : MonoBehaviour
    {
        public ClientSearchController searchCtrl;
        public Transform targetsObjParent;
        public GameObject cardImagePrefab;

        private readonly List<GameObject> images = new List<GameObject>();

        public void Show(IEnumerable<GameCard> targets)
        {
            foreach (var obj in images) Destroy(obj);
            images.Clear();

            foreach(var c in targets)
            {
                var obj = Instantiate(cardImagePrefab, parent: targetsObjParent);
                images.Add(obj);
                Image img = obj.GetComponent<Image>();
                img.sprite = c.simpleSprite;
            }

            gameObject.SetActive(true);
        }

        public void Confirm()
        {
            searchCtrl.SendTargets(confirmed: true);
            gameObject.SetActive(false);
        }

        public void Cancel()
        {
            gameObject.SetActive(false);
        }
    }
}