using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public enum DragState
	{
		Idle,
		Ready,
		Dragging
	}

	public interface IDraggable
	{
		DragState DragState { get; set; }
		string DragText { get; }
		Object[] DragObjects { get; }
		object DragData { get; }
	}

	public interface IDragReceiver
	{
		bool IsDragValid(Object[] objects, object data);
		void AcceptDrag(Object[] objects, object data);
	}

	public static class DragHelper
	{
		private const string _dragData = "DragData";

		public static void MakeDraggable<Draggable>(Draggable draggable) where Draggable : VisualElement, IDraggable
		{
			draggable.DragState = DragState.Idle;
			draggable.RegisterCallback<MouseDownEvent>(OnMouseDown);
			draggable.RegisterCallback<MouseMoveEvent>(OnMouseMove);
			draggable.RegisterCallback<MouseUpEvent>(OnMouseUp);
		}

		public static void MakeDragReceiver<Receiver>(Receiver receiver) where Receiver : VisualElement, IDragReceiver
		{
			receiver.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
			receiver.RegisterCallback<DragPerformEvent>(OnDragPerform);
		}

		private static void OnMouseDown(MouseDownEvent evt)
		{
			if (evt.target is IDraggable draggable && evt.button == (int)MouseButton.LeftMouse)
				draggable.DragState = DragState.Ready;
		}

		private static void OnMouseMove(MouseMoveEvent evt)
		{
			if (evt.target is IDraggable draggable && draggable.DragState == DragState.Ready)
			{
				DragAndDrop.PrepareStartDrag();
				DragAndDrop.objectReferences = draggable.DragObjects;
				DragAndDrop.SetGenericData(_dragData, draggable.DragData);
				DragAndDrop.StartDrag(draggable.DragText);

				draggable.DragState = DragState.Dragging;
			}
		}

		private static void OnMouseUp(MouseUpEvent evt)
		{
			if (evt.target is IDraggable draggable && draggable.DragState == DragState.Dragging && evt.button == (int)MouseButton.LeftMouse)
				draggable.DragState = DragState.Idle;
		}

		private static void OnDragUpdated(DragUpdatedEvent evt)
		{
			if (evt.target is IDragReceiver receiver)
			{
				var objects = DragAndDrop.objectReferences;
				var data = DragAndDrop.GetGenericData(_dragData);

				DragAndDrop.visualMode = receiver.IsDragValid(objects, data) ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;
			}
		}

		private static void OnDragPerform(DragPerformEvent evt)
		{
			if (evt.target is IDragReceiver receiver)
			{
				var objects = DragAndDrop.objectReferences;
				var data = DragAndDrop.GetGenericData(_dragData);

				if (receiver.IsDragValid(objects, data))
				{
					DragAndDrop.AcceptDrag();
					receiver.AcceptDrag(objects, data);
				}
			}
		}
	}
}
