using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Voxelgon;

namespace Voxelgon.EventSystems{

	public interface IModeChangeHandler : IEventSystemHandler {
		void OnModeChange(ModeChangeEventData data);
	} 
}