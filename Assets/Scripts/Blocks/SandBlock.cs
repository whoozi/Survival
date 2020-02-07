using UnityEngine;

namespace Survival.Blocks
{
	[System.Serializable]
	public class SandBlock : Block
	{
		public override ID id
		{
			get { return ID.Sand; }
		}

		public override Rect textureCoords
		{
			get { return new Rect(16, 32, 16, 16); }
		}

		public override string name
		{
			get { return "Sand"; }
		}

		public override bool canOcclude
		{
			get { return true; }
		}

		public override bool hasCollider
		{
			get { return true; }
		}

		public SandBlock() : base() { }
	}
}
