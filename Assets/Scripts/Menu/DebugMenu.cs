using UnityEngine;
using UnityEngine.UI;

using Survival.Blocks;

namespace Survival.Menu
{
	public class DebugMenu : MonoBehaviour
	{
		public Text performanceText;

		private float timeToUpdate;
		private const float updateTime = 0.2f;

		private void Update()
		{
			timeToUpdate += Time.deltaTime;

			if (timeToUpdate > updateTime)
			{
				timeToUpdate = 0;
				if (performanceText) performanceText.text = string.Format("{0:0.00} ms, {1:0} fps", Time.deltaTime * 1000.0, 1.0 / Time.deltaTime);
			}
		}
	}
}
