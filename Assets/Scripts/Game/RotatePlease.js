#pragma strict

var varX : int = 0;
var varY : int = 0;
var varZ : int = 0;

function Update() {
        transform.Rotate(Time.deltaTime*varX, Time.deltaTime*varY, Time.deltaTime*varZ);
}