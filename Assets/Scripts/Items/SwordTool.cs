using UnityEngine;

namespace Survival.Items
{
	public class SwordTool : ToolItem
	{
		public override Sprite sprite
		{
			get { return Game.itemSprites[3]; }
		}

		public override Vector2 spriteOffset
		{
			get { return new Vector2(3, 3); }
		}

		public override string name
		{
			get { return "Sword"; }
		}

		public override bool autoSwing
		{
			get { return false; }
		}

		public override float swingSpeed
		{
			get { return 0.6f; }
		}

		public override SwingType swingType
		{
			get { return SwingType.Stab; }
		}
	}
}
