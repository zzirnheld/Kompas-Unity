using KompasClient.GameCore;
using KompasClient.UI;
using KompasCore.Cards;
using System.Linq;
using TMPro;
using UnityEngine;
using static KompasCore.UI.UIController;

namespace KompasCore.UI
{
	[RequireComponent(typeof(CardController))]
	public class GameCardViewController : GameCardlikeViewController
	{
		public const float LargeUnzoomedTextFontSize = 32f;
		public const float SmallUnzoomedTextFontSize = 22f;
		private static float UnzoomedFontSizeForValue(int value) => value < 10 ? LargeUnzoomedTextFontSize : SmallUnzoomedTextFontSize;

		private ZoomLevel ZoomLevel => ClientCameraController.MainZoomLevel;

		public CardAOEController aoeController;

		//Zoomed-in text is handled by base class
		[Header("Zoomed-in card numeric stats")]
		[EnumNamedArray(typeof(ZoomLevel))]
		public TMP_Text[] zoomedInNTexts;
		[EnumNamedArray(typeof(ZoomLevel))]
		public TMP_Text[] zoomedInETexts;
		[EnumNamedArray(typeof(ZoomLevel))]
		public TMP_Text[] zoomedInCostTexts;
		[EnumNamedArray(typeof(ZoomLevel))]
		public TMP_Text[] zoomedInWTexts;
		[EnumNamedArray(typeof(ZoomLevel))]
		public TMP_Text[] costLabels;

		public TMP_Text zoomedInWithTextName;
		public TMP_Text zoomedInWithTextSubtype;

		//TODO give these to dummy cards, as empties probably
		[Header("Card highlighting")]
		public GameObject uniqueCopyObject;
		public GameObject linkedCardObject;
		public GameObject primaryStackableObject;
		public GameObject secondaryStackableObject;

		[Header("Can attack/effect indicators")]
		public OscillatingController attackOscillator;
		public OscillatingController effectOscillator;

		public GameObject zoomedInNoEffTextUI;
		public GameObject unzoomedUI;
		public GameObject zoomedInWithTextUI;

		public BoxCollider[] outsideCardBoxColliders;

		/// <summary>
		/// Used to make sure we don't regenerate the texture unnecessarily
		/// </summary>
		private string oldFileName;

		protected override void Display()
		{
			base.Display();

			HandleZoom();
		}

		protected override bool ShowingInfo
		{
			set => base.ShowingInfo = value
				&& (ShownGameCard.CardController.ShownInSearch
					|| (ShownGameCard?.Location != CardLocation.Deck && ShownGameCard?.Location != CardLocation.Nowhere));
		}

		protected override void DisplayCardRulesText()
		{
			base.DisplayCardRulesText();
			zoomedInWithTextName.text = shownCard.CardName;
			zoomedInWithTextSubtype.text = shownCard.QualifiedSubtypeText;
		}

		protected void HandleZoom()
		{
			handleStatColors(nText, eText, costText, wText);
			for (int i = 0; i < zoomedInNTexts.Length; i++)
				handleStatColors(zoomedInNTexts[i], zoomedInETexts[i], zoomedInCostTexts[i], zoomedInWTexts[i]);

			bool isChar = ShownCard.CardType == 'C';
			cardModelController.ShowZoom(isChar: isChar, zoomLevel: ZoomLevel);

			unzoomedUI.SetActive(ZoomLevel == ZoomLevel.ZoomedOut);
			zoomedInNoEffTextUI.SetActive(ZoomLevel == ZoomLevel.ZoomedInNoEffectText);
			zoomedInWithTextUI.SetActive(ZoomLevel == ZoomLevel.ZoomedInWithEffectText);
		}

		private static bool HasCurrentlyActivateableEffect(GameCard card)
			=> card.Effects != null && card.Effects.Count(e => e.CanBeActivatedBy(card.Controller)) > 0;

		private static bool HasAtAllActivateableEffect(GameCard card)
			=> card.Effects != null && card.Effects.Count(e => e.CanBeActivatedAtAllBy(card.Controller)) > 0;

		protected override void DisplaySpecialEffects()
		{
			base.DisplaySpecialEffects();
			ShowFrameColor();

			if (ShownGameCard.Location == CardLocation.Board)
			{
				//if you can attack at all, enable the attack indicator
				if (ShownGameCard.AttackingDefenderRestriction.CouldAttackValidTarget(stackSrc: null))
					//oscillate the attack indicator if can attack a card right now
					attackOscillator.Enable(ShownGameCard.AttackingDefenderRestriction.CanAttackAnyCard(stackSrc: null));
				else attackOscillator.Disable();

				//if you can activate any effect, enable the attack indicator
				if (HasAtAllActivateableEffect(ShownGameCard))
					//oscillate the effect indicator if you can activate an effect right now
					effectOscillator.Enable(HasCurrentlyActivateableEffect(ShownGameCard));
				else effectOscillator.Disable();

				if (shownCard.SpellSubtypes.Any(CardBase.RadialSubtype.Equals)) aoeController.Show(shownCard.Radius);
			}
			else
			{
				attackOscillator.Disable();
				effectOscillator.Disable();
				aoeController.Hide();
			}
		}

		protected override void DisplayCardNumericStats()
		{
			base.DisplayCardNumericStats();

			for (int i = 0; i < zoomedInNTexts.Length; i++)
			{
				zoomedInNTexts[i].text = $"{shownCard.N}";
				zoomedInETexts[i].text = $"{shownCard.E}";
				zoomedInWTexts[i].text = $"{shownCard.W}";

				zoomedInCostTexts[i].text = $"{shownCard.Cost}";
				costLabels[i].text = ShownCard.CostCardValue.DisplayName;
			}
		}

		protected override string DisplayN(int n) => $"{n}";
		protected override string DisplayE(int e) => $"{e}";
		protected override string DisplayS(int s) => $"{s}";
		protected override string DisplayW(int w) => $"{w}";
		protected override string DisplayC(int c) => $"{c}";
		protected override string DisplayA(int a) => $"{a}";

		public virtual void ShowUniqueCopy(bool copy = true) => uniqueCopyObject.SetActive(copy);

		public virtual void ShowLinkedCard(bool show = true) => linkedCardObject.SetActive(show);

		public virtual void ShowPrimaryOfStackable(bool show = true) => primaryStackableObject.SetActive(show);
		public virtual void ShowSecondaryOfStackable(bool show = true) => secondaryStackableObject.SetActive(show);

		private void ShowFrameColor()
		{
			if (ShownGameCard.Controller == null)
			{
				//no controller yet, don't bother showing color
				return;
			}
			/*
			Material material = ShownGameCard.Controller.Friendly ? friendlyCardFrameMaterial : enemyCardFrameMaterial;
			if (material == null) return; //Could be an error (TODO handle) but could be just a server card

			foreach (var obj in frameObjects)
			{
				obj.material = material;
			}*/
		}
	}
}