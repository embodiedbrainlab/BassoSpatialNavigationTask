#pragma strict

var amount : float = 2;
var speed : float = 0.5;

//****************************************************************************************************

function Update ()
{
	this.transform.localScale = Vector3(PingPongTool(Time.time * speed, 1, amount), PingPongTool(Time.time * speed, 1, amount), PingPongTool(Time.time * speed, 1, amount));
}

//****************************************************************************************************

function PingPongTool(duration : float, min : float, max : float) : float
{
	return Mathf.PingPong(duration, max-min) + min;
}