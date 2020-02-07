using System.Linq;

using UnityEngine;

using Survival.Blocks;

namespace Survival.Items
{
	public class PickaxeTool : ToolItem
	{
		public override Sprite sprite
		{
			get { return Game.itemSprites[1]; }
		}

		public override Vector2 spriteOffset
		{
			get { return new Vector2(4, 4); }
		}

		public override string name
		{
			get { return "Pickaxe"; }
		}

		public override float swingSpeed
		{
			get { return 0.9f; }
		}

		public override Block[] effectiveAgainst
		{
			get { return fist.effectiveAgainst.Concat(new Block[] { Block.cactus, Block.dirt, Block.grass, Block.sand, Block.wool }).ToArray(); }
		}
	}
}
