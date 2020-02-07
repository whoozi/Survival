using UnityEngine;

namespace Survival.Utility
{
	public class Singleton<T> : MonoBehaviour where T : Component
	{
		private static T _instance;

		public static T instance
		{
			get
			{
				if (!_instance)
				{
					_instance = FindObjectOfType<T>();

					if (!_instance)
					{
						_instance = new GameObject(typeof(T).Name, typeof(T)).GetComponent<T>();
					}
				}

				return _instance;
			}
		}

		public virtual void Awake()
		{
			DontDestroyOnLoad(gameObject);

			if (_instance == null) _instance = this as T;
			else Destroy(gameObject);
		}
	}
}
