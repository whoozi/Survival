#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Survival.Utility
{
	public class CanvasGrid : MonoBehaviour
	{
		private Canvas canvas;
		private RectTransform rectTransform;
		public Color gridColor = new Color(1, 1, 1, 0.25f);

		public void OnDrawGizmos()
		{
			if (!canvas) canvas = GetComponent<Canvas>();
			if (!rectTransform) rectTransform = GetComponent<RectTransform>();

			if (!Selection.activeGameObject || Selection.activeGameObject.transform.root != transform) return;

			Rect canvasRect = new Rect(transform.position - (Vector3)rectTransform.sizeDelta / 2 * rectTransform.localScale.x, rectTransform.sizeDelta * rectTransform.localScale.x);

			Gizmos.color = gridColor;

			for (int x = 0; x < rectTransform.sizeDelta.x; x++)
			{
				Gizmos.DrawLine(new Vector3(canvasRect.xMin + (x * rectTransform.localScale.x), canvasRect.yMin, transform.position.z), new Vector3(canvasRect.xMin + (x * rectTransform.localScale.x), canvasRect.yMax, transform.position.z));
			}

			for (int y = 0; y < rectTransform.sizeDelta.y; y++)
			{
				Gizmos.DrawLine(new Vector3(canvasRect.xMin, canvasRect.yMin + (y * rectTransform.localScale.y), transform.position.z), new Vector3(canvasRect.xMax, canvasRect.yMin + (y * rectTransform.localScale.y), transform.position.z));
			}
		}
	}
}
#endif
