using System.Xml.Serialization;

using UnityEngine;

using Survival.Blocks;
using Survival.Mobs;

namespace Survival.Items
{
	[XmlInclude(typeof(FistTool)), XmlInclude(typeof(SwordTool)), XmlInclude(typeof(PickaxeTool))]
	public class ToolItem : Item
	{
		public override int maxStack
		{
			get { return 1; }
		}

		public virtual int damage
		{
			get { return 2; }
		}

		public virtual Block[] effectiveAgainst
		{
			get { return new Block[] { }; }
		}

		public virtual float breakSpeed
		{
			get { return 1f; }
		}

		public bool IsEffectiveAgainst(Block block)
		{
			for (int i = 0; i < effectiveAgainst.Length; i++)
				if (effectiveAgainst[i].id == block.id) return true;

			return false;
		}

		public override void Use(World world, Mob mob, Inventory.Slot slot)
		{
			base.Use(world, mob, slot);

			if (Game.mouseHit.collider)
			{
				int x = (int)Game.mouseHit.transform.position.x, y = (int)Game.mouseHit.transform.position.y, z = (int)Game.mouseHit.transform.position.z;

				int xNormal = Mathf.Clamp(x + (int)Game.mouseHit.normal.x, 0, world.sizeX - 1);
				int yNormal = Mathf.Clamp(y + (int)Game.mouseHit.normal.y, 0, world.sizeY - 1);
				int zNormal = Mathf.Clamp(z + (int)Game.mouseHit.normal.z, 0, world.sizeZ - 1);

				if (world.block[xNormal, yNormal, zNormal].id != Block.ID.Air && !world.block[xNormal, yNormal, zNormal].hasCollider) world.block[xNormal, yNormal, zNormal].Damage(world, xNormal, yNormal, zNormal, this);
				else world.block[x, y, z].Damage(world, x, y, z, this);
            }
		}
	}
}
