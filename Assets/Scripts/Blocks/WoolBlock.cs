using UnityEngine;

using System.Xml.Serialization;

namespace Survival.Blocks
{
	[System.Serializable]
	public class WoolBlock : Block
	{
		public enum Color
		{
			White, Yellow, Green, Orange, Red, Pink, Purple, Blue, Black
		}

		public override ID id
		{
			get { return ID.Wool; }
		}

		public override Rect textureCoords
		{
			get
			{
				switch (color)
				{
					case Color.White:
						return new Rect(0, 16, 16, 16);

					case Color.Yellow:
						return new Rect(16, 16, 16, 16);

					case Color.Green:
						return new Rect(112, 32, 16, 16);

					case Color.Orange:
						return new Rect(32, 16, 16, 16);

					case Color.Red:
						return new Rect(48, 16, 16, 16);

					case Color.Pink:
						return new Rect(64, 16, 16, 16);

					case Color.Purple:
						return new Rect(80, 16, 16, 16);

					case Color.Blue:
						return new Rect(96, 16, 16, 16);

					case Color.Black:
						return new Rect(112, 16, 16, 16);
				}

				return base.textureCoords;
			}
		}

		public override string name
		{
			get { return color.ToString() + " Wool"; }
		}

		public override bool canOcclude
		{
			get { return true; }
		}

		public override bool hasCollider
		{
			get { return true; }
		}

		[XmlAttribute]
		public Color color = Color.White;

		public WoolBlock() : base() { }

		public WoolBlock(Color color) : this()
		{
			this.color = color;
		}
	}
}
