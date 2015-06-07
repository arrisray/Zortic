/*

Transform target;
var speed = 1.0;

public void Update () 
{
	if (target == null && GameObject.FindWithTag("Player"))
		target = GameObject.FindWithTag("Player").transform;
		
	if(target)
	{
		//TARGETING
		var rotate = Quaternion.LookRotation(target.position - transform.position);
		//var rotatexy = rotate;
		
		//rotatexy.eulerAngles.x = 0;
		//rotatexy.eulerAngles.y = 0;
		
		transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * speed);
	}
}

*/
