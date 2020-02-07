using UnityEngine;

using Survival.Blocks;
using Survival.Mobs;

namespace Survival.Items
{
	[System.Serializable]
	public class BlockItem : Item
	{
		public override Sprite sprite
		{
			get { return block.sprite; }
		}

		public override string name
		{
			get { return block.name; }
		}

		public new Block block;

		public BlockItem() : base() { }

		public BlockItem(Block block) : base()
		{
			this.block = block.Clone();
		}

		public override void UseAlt(World world, Mob mob, Inventory.Slot slot)
		{
			if (Game.mouseHit.collider)
			{
				int x = (int)Game.mouseHit.transform.position.x, y = (int)Game.mouseHit.transform.position.y, z = (int)Game.mouseHit.transform.position.z;

				if (world.block[x, y, z].canActivate)
				{
					world.block[x, y, z].OnActivate(mob, this);
					return;
				}

				int xNormal = Mathf.Clamp(x + (int)Game.mouseHit.normal.x, 0, world.sizeX - 1);
				int yNormal = Mathf.Clamp(y + (int)Game.mouseHit.normal.y, 0, world.sizeY - 1);
				int zNormal = Mathf.Clamp(z + (int)Game.mouseHit.normal.z, 0, world.sizeZ - 1);

				if ((!world.IsPlayerLocatedAt(xNormal, yNormal, zNormal) || !block.hasCollider) && world.block[xNormal, yNormal, zNormal].id == Block.ID.Air && block.CanStay(world, xNormal, yNormal, zNormal))
				{
					Swing(world, mob);
					world.SetBlock(block.Clone(), xNormal, yNormal, zNormal);
					slot.Remove(1);
				}
			}
		}
	}
}
