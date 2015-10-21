using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Voxelgon;

namespace Voxelgon.EventSystems {
	public interface IModeChangeHandler : IEventSystemHandler {
		void onModeChange(modeChangeData data);
	} 

	public class StandardInputModule : StandaloneInputModule {
		public override void Process(){
			base.Process();

		}
	}

	public class modeChangeData : BaseEventData {

		public GameMode newMode;
		public GameMode oldMode;

		public modeChangeData(EventSystem eSystem) : base(eSystem) {}

	}
}
