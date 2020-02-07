using Survival.Blocks;

namespace Survival.Items
{
	public class FistTool : ToolItem
	{
		public override bool autoSwing
		{
			get { return false; }
		}

		public override SwingType swingType
		{
			get { return SwingType.Stab; }
		}

		public override Block[] effectiveAgainst
		{
			get { return new Block[] { Block.flower, Block.torch }; }
		}

		public override float breakSpeed
		{
			get { return 0.2f; }
		}
	}
}
