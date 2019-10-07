using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
	{
		private const string _secondInstanceWarning = "(USBSI) A second instance of SingletonBehaviour type {0} was created";

		public static T Instance { get; private set; }

		public SingletonBehaviour()
		{
			// this is set in the constructor as opposed to Awake to avoid load order issues - other objects in the
			// same scene may want to access the singleton during enable but if they happen to be loaded before the
			// singleton, Instance otherwise wouldn't yet be available

			Instance = this as T;
		}

		protected virtual void Awake()
		{
			if (Instance != this)
			{
				Debug.LogWarningFormat(_secondInstanceWarning, GetType().Name);
				Destroy(this);
			}
		}

		protected virtual void OnDestroy()
		{
			if (Instance == this)
				Instance = null;
		}
	}
}
