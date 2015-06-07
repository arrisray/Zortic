/*

public void ScreenFormat()
{
	float screenFormat;
	
	if(Screen.width > Screen.height)
		screenFormat = Screen.width;
	else
		screenFormat = Screen.height;
	
	return screenFormat;
}

public void ScreenAverage()
{
	float avg;
	if(Screen.width > Screen.height)
		avg = (Screen.height * Screen.width) / 2;
	else
		avg = Screen.width;
	return avg;
}

public void RandomPoint()
{
	Vector3 screenPos;
	float coinFlip = Random.value;

	if(Time.time < 2.0f)
	{
		if(coinFlip <= 0.5f)
			screenPos = this.camera.ViewportToWorldPoint (Vector3(Random.value, Random.value, camera.farClipPlane));
		else
			screenPos = this.camera.ViewportToWorldPoint (Vector3(Random.value, Random.value, camera.farClipPlane));
	}
	else
	{
		if(coinFlip <= 0.5f)
			screenPos = this.camera.ViewportToWorldPoint (Vector3(1.0f, Random.value, camera.farClipPlane));
		else
			screenPos = this.camera.ViewportToWorldPoint (Vector3(Random.value, 1.0f, camera.farClipPlane));
	}
		
	return screenPos;
}

public void SpawnNPC(GameObject arr[], Transform parentNode)
{	
	Vector3 worldPos = RandomPoint();
	int dieRoll = Mathf.FloorToInt(Random.value * arr.length);
	var startOrientation = Random.rotation;
	startOrientation.eulerAngles.z = 0.0f;
	var newNPC = GameManager.Instance.UnityInstantiate(arr[dieRoll], worldPos, startOrientation);
	newNPC.transform.parent = parentNode;
}

//screenPosition = sceneCamera.GetComponent(CameraView).ReturnScreenPosition(); //return on screen position of the sent transform
public void ReturnScreenPosition(GameObject point)
{
	Vector2 screenPosition; 
	screenPosition = this.camera.WorldToScreenPoint(point.transform.position);
	screenPosition.y = Screen.height - screenPosition.y;
	
	return screenPosition;
}

*/
