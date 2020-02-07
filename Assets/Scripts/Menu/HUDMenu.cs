using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Survival.Menu
{
	public class HUDMenu : MonoBehaviour
	{
		private const int healthPerHeart = 2;

		public Text lifeText;
		public Sprite[] heartSprites = new Sprite[3];
		public RectTransform heartParent;
		public Text itemText;
		private Image[] quickSlots = new Image[9];
		private List<Image> hearts = new List<Image>();

		private void Start()
		{
			quickSlots = transform.GetChild(1).GetComponentsInChildren<Image>();
		}

		private void FixedUpdate()
		{
			if (!Game.localPlayer) return;

			// Life
			lifeText.text = string.Format("Life: {0}/{1}", Game.localPlayer.health, Game.localPlayer.maxHealth);
			if (hearts.Count != Game.localPlayer.maxHealth / healthPerHeart) UpdateHearts();

			// Quick Slots
			itemText.text = Game.localPlayer.inventory.slots[Game.localPlayer.currentSlot].item != null
				? Game.localPlayer.inventory.slots[Game.localPlayer.currentSlot].item.name + (Game.localPlayer.inventory.slots[Game.localPlayer.currentSlot].item.maxStack > 1
				? " x" + Game.localPlayer.inventory.slots[Game.localPlayer.currentSlot].quantity : "") : "Items";

			for (int i = 0; i < quickSlots.Length; i++)
			{
				if (Game.localPlayer.currentSlot == i) quickSlots[i].color = Color.white;
				else quickSlots[i].color = new Color(0.1f, 0.1f, 0.1f, 1f);

				if (Game.localPlayer.inventory.slots[i].item != null && Game.localPlayer.inventory.slots[i].item.sprite != null)
				{
					if (quickSlots[i].transform.childCount < 1)
					{
						GameObject temp = new GameObject(Game.localPlayer.inventory.slots[i].item.sprite.name, typeof(Image));
						temp.transform.SetParent(quickSlots[i].transform, false);

						{
							RectTransform tempRectTransform = temp.GetComponent<RectTransform>();

							tempRectTransform.localPosition = Game.localPlayer.inventory.slots[i].item.spriteOffset;
							tempRectTransform.sizeDelta = new Vector2(16, 16);
						}

						temp.GetComponent<Image>().sprite = Game.localPlayer.inventory.slots[i].item.sprite;
					}
				}
				else if (quickSlots[i].transform.childCount > 0) Destroy(quickSlots[i].transform.GetChild(0).gameObject);
			}
		}

		public void UpdateHearts()
		{
			if (hearts.Count > 0)
			{
				for (int i = 0; i < hearts.Count; i++) Destroy(hearts[i].gameObject);
				hearts.Clear();
			}

			for (int i = 0; i < Game.localPlayer.maxHealth / healthPerHeart; i++)
			{
				hearts.Add(new GameObject("Heart", typeof(Shadow)).AddComponent<Image>());
				hearts[i].transform.SetParent(heartParent, false);
				hearts[i].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(9, 9);
				hearts[i].transform.localPosition = new Vector2(i * (9 + 1) + 4.5f, 0f);
			}

			{
				bool restAreEmpty = false;
				int i = 0;

				foreach (Image heart in hearts)
				{
					if (restAreEmpty) heart.sprite = heartSprites[0];
					else
					{
						i += 1;

						if (Game.localPlayer.health >= i * healthPerHeart) heart.sprite = heartSprites[heartSprites.Length - 1];
						else
						{
							int currentHeartHealth = healthPerHeart - (healthPerHeart * i - Game.localPlayer.health);
							int healthPerImage = healthPerHeart / (heartSprites.Length - 1);
							int spriteIndex = currentHeartHealth / healthPerImage;

							if (spriteIndex == 0 && currentHeartHealth > 0) spriteIndex = 1;

							heart.sprite = heartSprites[spriteIndex];
							restAreEmpty = true;
						}
					}
				}
			}
		}
	}
}
