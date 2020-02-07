using UnityEngine;

using Survival.Mobs;

namespace Survival.Blocks
{
	[System.Serializable]
	public class CactusBlock : Block
	{
		public override ID id
		{
			get { return ID.Cactus; }
		}

		public override Type type
		{
			get { return Type.Plane; }
		}

		public override Rect textureCoords
		{
			get { return new Rect(32, 32, 16, 16); }
		}

		public override string name
		{
			get { return "Cactus"; }
		}

		public override bool canOcclude
		{
			get { return false; }
		}

		public override bool hasCollider
		{
			get { return true; }
		}

		public CactusBlock() : base() { }

		public override bool CanStay(World world, int x, int y, int z)
		{
			if (x > 0 && world.block[x - 1, y, z].id != ID.Air)
				return false;
			else if (x < world.sizeX - 1 && world.block[x + 1, y, z].id != ID.Air)
				return false;
			else if (z > 0 && world.block[x, y, z - 1].id != ID.Air)
				return false;
			else if (z < world.sizeZ - 1 && world.block[x, y, z + 1].id != ID.Air)
				return false;
			else
			{
				ID under = world.block[x, y - 1, z].id;
				return y > 0 && (under == ID.Cactus || under == ID.Sand);
			}
		}

		public override void OnTouch(Mob mob, Vector3 fromDirection)
		{
			mob.Damage(1, new Vector3(-fromDirection.x * 0.2f, 0.5f, -fromDirection.z * 0.2f));
		}
	}
}
