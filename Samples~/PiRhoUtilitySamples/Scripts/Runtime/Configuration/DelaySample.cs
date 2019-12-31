using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Utilities/Delay")]
	public class DelaySample : MonoBehaviour
	{
		[MessageBox("The [Delay] attribute tells text and value input fields to wait to set the value until enter is pressed", MessageBoxType.Info, Location = TraitLocation.Above)]
		
		[Delay] public int Integer;
		[Delay] public float Float;
		[Delay] public string String;
		[Delay] public Vector2 Vector2;
		[Delay] public Vector2Int Vector2Int;
		[Delay] public Vector3 Vector3;
		[Delay] public Vector3Int Vector3Int;
		[Delay] [Euler] public Quaternion Quaternion;
		[Delay] public Rect Rect;
		[Delay] public RectInt RectInt;
		[Delay] public Bounds Bounds;
		[Delay] public BoundsInt BoundsInt;
		[Delay] public char Character;
	}
}