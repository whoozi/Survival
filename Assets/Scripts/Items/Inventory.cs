using System;

using UnityEngine;

namespace Survival.Items
{
	[Serializable]
	public class Inventory
	{
		[Serializable]
		public class Slot
		{
			public Item item = null;
			public int quantity = 0;
			public float damage = 0f;

			public Slot(Item item, int quantity)
			{
				this.item = item;
				this.quantity = quantity;
			}

			public Slot(Item item, int quantity, float damage) : this(item, quantity)
			{
				this.damage = damage;
			}

			public void Add(Item item, int quantity)
			{
				if (this.item != null)
				{
					if (this.item.name == item.name)
						this.quantity += quantity;

					this.quantity = Mathf.Clamp(this.quantity, 0, this.item.maxStack);
				}
				else
				{
					this.item = item;
					this.quantity = quantity;
				}
			}

			public void Remove(int quantity)
			{
				if (item != null)
					this.quantity -= quantity;

				if (this.quantity < 1)
				{
					item = null;
					quantity = 0;
				}
			}

			public static void Transfer(ref Slot from, ref Slot to, int quantity)
			{
				to.Add(from.item, quantity);
				from.Remove(quantity);
			}
		}

		public Slot[] slots;

		public virtual Slot HasRoom(Item item)
		{
			for (int i = 0; i < slots.Length; i++)
				if (slots[i].item != null && slots[i].item.name == item.name && slots[i].quantity < item.maxStack) return slots[i];
				else if (slots[i].item == null) return slots[i];

			return null;
		}
	}
}
