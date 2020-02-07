using UnityEngine;

namespace Survival.Blocks
{
	[System.Serializable]
	public class TorchBlock : Block
	{
		public override ID id
		{
			get { return ID.Torch; }
		}

		public override Rect textureCoords
		{
			get { return new Rect(32, 48, 16, 16); }
		}

		public override Type type
		{
			get { return Type.Plane; }
		}

		public override string name
		{
			get { return "torch"; }
		}

		public override bool canOcclude
		{
			get { return false; }
		}

		public TorchBlock() : base() { }
	}
}
