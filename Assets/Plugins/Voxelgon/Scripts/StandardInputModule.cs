using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace Voxelgon.EventSystems {
	public class StandardInputModule : StandaloneInputModule {
		public override void Process(){
			base.Process();
			print("hello!");
		}

	}
}
