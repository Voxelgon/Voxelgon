#pragma strict
//var mainCam : GameObject;
var mainTransform : Transform;
//var selfTransform : Transform;

function Start () {
	yield WaitForSeconds(1);

	var starSystems : Component[];
	starSystems = GetComponentsInChildren(ParticleSystem);
	for (var i : Component in starSystems){
		i.particleSystem.Pause();
	}

}

function Update () {
	var Transform = GetComponent(Transform);
	Transform.position = mainTransform.position;
}