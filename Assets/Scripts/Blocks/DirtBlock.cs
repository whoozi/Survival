using UnityEngine;

namespace Survival.Blocks
{
	[System.Serializable]
	public class DirtBlock : Block
	{
		public override ID id
		{
			get { return ID.Dirt; }
		}

		public override Rect textureCoords
		{
			get { return new Rect(0, 32, 16, 16); }
		}

		public override string name
		{
			get { return "Dirt"; }
		}

		public override bool canOcclude
		{
			get { return true; }
		}

		public override bool hasCollider
		{
			get { return true; }
		}

		public DirtBlock() : base() { }

		public override void FixedUpdate(World world, int x, int y, int z)
		{
			if (y < world.sizeY - 1 && world.block[x, y + 1, z].sunLight > 0.3f)
			{
				if (Random.Range(0, 500) == 1)
				{
					if ((x > 0 && world.block[x - 1, y, z].id == ID.Grass) || (x < world.sizeX - 1 && world.block[x + 1, y, z].id == ID.Grass)
						|| (z > 0 && world.block[x, y, z - 1].id == ID.Grass) || (z < world.sizeZ && world.block[x, y, z + 1].id == ID.Grass))
					{
						world.SetBlock<GrassBlock>(x, y, z);
					}
				}
			}
		}
	}
}
