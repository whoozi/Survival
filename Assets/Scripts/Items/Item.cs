using System.Xml.Serialization;

using UnityEngine;

using Survival.Mobs;

namespace Survival.Items
{
	[XmlInclude(typeof(BlockItem)), XmlInclude(typeof(ToolItem))]
	public class Item
	{
		public enum State
		{
			Idle, Use, UseAlt
		}

		public enum SwingType
		{
			Stab, Pick
		}

		public virtual Sprite sprite
		{
			get { return null; }
		}

		public virtual Vector2 spriteOffset
		{
			get { return Vector2.zero; }
		}

		public virtual string name
		{
			get { return null; }
		}

		public virtual int maxStack
		{
			get { return 64; }
		}

		public virtual bool autoSwing
		{
			get { return true; }
		}

		public virtual float swingSpeed
		{
			get { return 1f; }
		}

		public virtual SwingType swingType
		{
			get { return SwingType.Pick; }
		}

		public Item() { }

		public virtual void Use(World world, Mob mob, Inventory.Slot slot)
		{
			Swing(world, mob);
		}

		public virtual void UseAlt(World world, Mob mob, Inventory.Slot slot)
		{
			if (Game.mouseHit.collider)
			{
				int x = (int)Game.mouseHit.transform.position.x, y = (int)Game.mouseHit.transform.position.y, z = (int)Game.mouseHit.transform.position.z;

				if (world.block[x, y, z].canActivate) world.block[x, y, z].OnActivate(mob, this);
			}
		}

		public virtual void Swing(World world, Mob mob)
		{
			GameObject temp = new GameObject(name, typeof(SpriteRenderer));
			temp.transform.SetParent(mob.transform.GetChild(0), false);

			if (sprite != null)
			{
				SpriteRenderer tempRenderer = temp.GetComponent<SpriteRenderer>();
				tempRenderer.material = Game.spriteMaterial;
				tempRenderer.material.color = new Color(world.block[mob.blockPosition.x, mob.blockPosition.y, mob.blockPosition.z].sunLight, world.block[mob.blockPosition.x, mob.blockPosition.y, mob.blockPosition.z].sunLight, world.block[mob.blockPosition.x, mob.blockPosition.y, mob.blockPosition.z].sunLight, 1);
				tempRenderer.sprite = sprite;
				tempRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
				tempRenderer.receiveShadows = true;
			}

			mob.animator.SetTrigger("Attack");
			mob.animator.SetInteger("Attack Type", (int)swingType);
			mob.state = Mob.State.Attack;
		}

		public static readonly FistTool fist = new FistTool();
		public static readonly BlockItem block = new BlockItem();
		public static readonly SwordTool sword = new SwordTool();
		public static readonly SwordTool spear = new SwordTool();
		public static readonly PickaxeTool pickaxe = new PickaxeTool();
	}
}
