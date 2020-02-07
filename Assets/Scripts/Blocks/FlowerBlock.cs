using System.Xml.Serialization;
using UnityEngine;

namespace Survival.Blocks
{
	[System.Serializable]
	public class FlowerBlock : Block
	{
		public enum Color
		{
			Red, White
		}

		public override ID id
		{
			get { return ID.Flower; }
		}

		public override Type type
		{
			get { return Type.Plane; }
		}

		public override Rect textureCoords
		{
			get
			{
				switch (color)
				{
					case Color.Red:
						return new Rect(48, 32, 16, 16);

					case Color.White:
						return new Rect(64, 32, 16, 16);
				}

				return base.textureCoords;
			}
		}

		public override string name
		{
			get
			{
				switch (color)
				{
					case Color.Red:
						return "Rose";

					case Color.White:
						return "Daisy";
				}

				return base.name;
			}
		}

		public override bool canOcclude
		{
			get { return false; }
		}

		public override float durability
		{
			get { return 0.1f; }
		}

		[XmlAttribute]
		public Color color = Color.Red;

		public FlowerBlock() : base() { }

		public FlowerBlock(Color color) : base()
		{
			this.color = color;
		}

		public override bool CanStay(World world, int x, int y, int z)
		{
			return y > 0 && world.block[x, y - 1, z].id == ID.Grass;
		}
	}
}
