namespace Survival.Utility
{
	public class BlockPosition
	{
		public int x = 0, y = 0, z = 0;

		public BlockPosition(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public override int GetHashCode()
		{
			int hash = 47;

			hash = hash * 227 + x.GetHashCode();
			hash = hash * 227 + y.GetHashCode();
			hash = hash * 227 + z.GetHashCode();

			return hash;
		}

		public override bool Equals(object obj)
		{
			if (GetHashCode() == obj.GetHashCode())
				return true;

			return false;
		}

		public static BlockPosition operator -(BlockPosition current, BlockPosition after)
		{
			return new BlockPosition(current.x - after.x, current.y - after.y, current.z - after.z);
		}

		public static BlockPosition operator +(BlockPosition current, BlockPosition after)
		{
			return new BlockPosition(current.x + after.x, current.y + after.y, current.z + after.z);
		}

		public static bool operator ==(BlockPosition position1, BlockPosition position2)
		{
			return Equals(position1, position2);
		}

		public static bool operator !=(BlockPosition position1, BlockPosition position2)
		{
			return !Equals(position1, position2);
		}

		public override string ToString()
		{
			return string.Format("({0}, {1}, {2})", x, y, z);
		}
	}
}
