using UnityEngine;

using Survival.Items;

namespace Survival.Blocks
{
	[System.Serializable]
	public class GrassBlock : Block
	{
		public override ID id
		{
			get { return ID.Grass; }
		}

		public override Rect textureCoords
		{
			get { return new Rect(16, 48, 16, 16); }
		}

		public override string name
		{
			get { return "Grass"; }
		}

		public override bool canOcclude
		{
			get { return true; }
		}

		public override bool hasCollider
		{
			get { return true; }
		}

		public override Item dropItem
		{
			get { return new BlockItem(dirt); }
		}

		public GrassBlock() : base() { }

		public override void FixedUpdate(World world, int x, int y, int z)
		{
			if (Random.Range(0, 50) == 1)
				if (world.block[x, y + 1, z].sunLight < 0.1f) world.SetBlock<DirtBlock>(x, y, z);
		}
	}
}
