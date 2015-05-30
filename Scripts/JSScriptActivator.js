#pragma strict

public var toActivate : MonoBehaviour;

function OnEnable () {
	
	toActivate.enabled = true;
}

function OnDisable () {

	toActivate.enabled = false;
}