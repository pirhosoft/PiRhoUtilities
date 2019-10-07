using System;

namespace PiRhoSoft.Utilities
{
	public class WeakEvent
	{
		private event Action _event;

		public void Subscribe<T>(T target, Action<T> callback) where T : class
		{
			var reference = new WeakReference(target, false);
			var handler = (Action)null;

			handler = () =>
			{
				if (reference.Target is T t)
					callback(t);
				else
					_event -= handler;
			};

			_event += handler;
		}

		public void Trigger()
		{
			_event?.Invoke();
		}
	}

	public class WeakEvent<Args>
	{
		private event Action<Args> _event;

		public void Subscribe<T>(T target, Action<T, Args> callback) where T : class
		{
			var reference = new WeakReference(target, false);
			var handler = (Action<Args>)null;

			handler = args =>
			{
				if (reference.Target is T t)
					callback(t, args);
				else
					_event -= handler;
			};

			_event += handler;
		}

		public void Trigger(Args args)
		{
			_event?.Invoke(args);
		}
	}
}
