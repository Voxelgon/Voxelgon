using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Voxelgon;

namespace Voxelgon.EventSystems{

	public interface IModeChangeHandler : IEventSystemHandler {
		void onModeChange(modeChangeData data);
	} 

	public class modeChangeData : BaseEventData {

		public Gamemode.GameMode newMode;
		public Gamemode.GameMode oldMode;
		public bool fromPlayer;

		public modeChangeData(EventSystem eSystem) : base(eSystem) {}

	}
}