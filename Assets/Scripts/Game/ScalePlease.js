#pragma strict

var scaleFactor : float = 0.005f;


function Update()
{
	if(transform.localScale.x >= 0.0)
	{
		transform.localScale.x = transform.localScale.x - scaleFactor;
        transform.localScale = Vector3(transform.localScale.x, transform.localScale.x, transform.localScale.x);
	}

}