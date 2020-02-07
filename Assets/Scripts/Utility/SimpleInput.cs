using System;

using UnityEngine;

namespace Survival.Utility
{
	public static class SimpleInput
	{
		public static float GetKeyCodeAxis(KeyCode negative, KeyCode positive)
		{
			return -Convert.ToInt32(Input.GetKey(negative)) + Convert.ToInt32(Input.GetKey(positive));
		}
	}
}
