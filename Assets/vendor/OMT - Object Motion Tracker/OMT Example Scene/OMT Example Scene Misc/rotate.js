#pragma strict

var rotateAmount : Vector3;
var rotateSpeed : float = 20;
var movespeed : float;

function Update () {
		transform.Rotate(rotateAmount * Time.deltaTime * rotateSpeed);
		transform.Translate(Vector3.up * Time.deltaTime * movespeed);
}