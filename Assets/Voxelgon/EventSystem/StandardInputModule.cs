using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Voxelgon;

namespace Voxelgon.EventSystems {
    public class StandardInputModule : StandaloneInputModule {
        public override void Process(){
            base.Process();
            ProcessModeChangeEvent();
        }

        private void ProcessModeChangeEvent() {
            if(Input.GetButtonDown("ChangeMode")) {
                ToggleGameMode(ModeChangeEventSource.playerKey);
            }
        }

        private void ToggleGameMode(ModeChangeEventSource source) {
            GameMode.Mode newMode;
            GameMode.Mode oldMode = GameMode.Current;

            switch (oldMode) {
                case GameMode.Mode.Flight:
                    newMode = GameMode.Mode.Edit;
                    break;
                case GameMode.Mode.Edit:
                default:
                    newMode = GameMode.Mode.Flight;
                    break;
            }

            ModeChangeEventData eventData = new ModeChangeEventData(EventSystem.current);
            eventData.source = source;
            eventData.oldMode = oldMode;
            eventData.newMode = newMode;

            GameObject editorController = GameObject.Find("EditorController");

            ExecuteEvents.Execute<IModeChangeHandler>(editorController, null, (x,y)=>x.OnModeChange(eventData));

        }
    }
}
