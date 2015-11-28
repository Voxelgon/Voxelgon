using UnityEngine.EventSystems;

namespace Voxelgon.EventSystems {

	public interface IModeChangeHandler : IEventSystemHandler {
		void OnModeChange(ModeChangeEventData data);
	} 
}