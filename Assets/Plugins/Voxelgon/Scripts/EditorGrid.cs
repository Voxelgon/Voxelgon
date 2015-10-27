using UnityEngine;
using System.Collections;
using Voxelgon;
using Voxelgon.EventSystems;

public class EditorGrid: MonoBehaviour {


	[Space(10)]

	[SerializeField]
	protected float gridSize; //size of the build grid in worldspace units
	[SerializeField]
	protected float girdPreviewFalloff; //falloff of grid preview objects as an exponent
	[SerializeField]
	protected float gridPreviewRange;

}
