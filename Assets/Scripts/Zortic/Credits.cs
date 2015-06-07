/*

GUIStyle nameStyle;
private var lerpTime = 0.0f;

private float startTime1 = 1.0f;
private float startTime2 = 53.0f;
private float startTime3 = 77.0f;
private float startTime4 = 108.0f;
private float startTime5 = 127.0f;
private float onScreenDuration = 18.0f;
private float fadeLength = 4.0f;
private float screenAvg;
private GameObject sceneCamera;

public void Start()
{
	sceneCamera = GameObject.Find("Camera");
	screenAvg = sceneCamera.GetComponent(CameraView).ScreenFormat();
	nameStyle.fontSize = screenAvg * 0.03f;
}

public void OnGUI()
{
	//print(Time.timeSinceLevelLoad);

	if(Time.timeSinceLevelLoad > startTime1 && Time.timeSinceLevelLoad < startTime1 + onScreenDuration)
	{		
		GUI.Label(Rect(Screen.width*0.1f, Screen.height*0.8f, Screen.width*0.8f, Screen.height*0.05f), "Completely original gameplay not at all like the game that rhymes with 'QUASTEROIDS'", nameStyle);
		
		if(Time.timeSinceLevelLoad < startTime1 + fadeLength)
		{
			nameStyle.normal.textColor.a = Mathf.Lerp(0,1,lerpTime);
			lerpTime = lerpTime + Time.deltaTime*0.2f;
		}
		
		if(Time.timeSinceLevelLoad > startTime1 + fadeLength && Time.timeSinceLevelLoad < startTime1 + onScreenDuration - fadeLength)
			lerpTime = 0.0f;
			
		if(Time.timeSinceLevelLoad > startTime1 + onScreenDuration - fadeLength)
		{
			nameStyle.normal.textColor.a = Mathf.Lerp(1,0,lerpTime);
			lerpTime = lerpTime + Time.deltaTime*0.2f;
		}
	}
	// 1 to 2
	if(Time.timeSinceLevelLoad > startTime1 + onScreenDuration && Time.timeSinceLevelLoad < startTime2)
	{
		//print("lerpTime RESET");
		lerpTime = 0.0f;
	}

	if(Time.timeSinceLevelLoad > startTime2 && Time.timeSinceLevelLoad < startTime2 + onScreenDuration)
	{
		//print("Second Credit");
		GUI.Label(Rect(Screen.width*0.1f, Screen.height*0.4f, Screen.width*0.2f, Screen.height*0.05f), "Art by DJ CASSEL", nameStyle);
		
		if(Time.timeSinceLevelLoad < startTime2 + fadeLength)
		{
			nameStyle.normal.textColor.a = Mathf.Lerp(0,1,lerpTime);
			lerpTime = lerpTime + Time.deltaTime*0.2f;
		}
		
		if(Time.timeSinceLevelLoad > startTime2 + fadeLength && Time.timeSinceLevelLoad < startTime2 + onScreenDuration - fadeLength)
			lerpTime = 0.0f;
			
		if(Time.timeSinceLevelLoad > startTime2 + onScreenDuration - fadeLength)
		{
			nameStyle.normal.textColor.a = Mathf.Lerp(1,0,lerpTime);
			lerpTime = lerpTime + Time.deltaTime*0.2f;
		}
	}

	// 2 to 3
	if(Time.timeSinceLevelLoad > startTime2 + onScreenDuration && Time.timeSinceLevelLoad < startTime3)
	{
		//print("lerpTime RESET");
		lerpTime = 0.0f;
	}
	
	if(Time.timeSinceLevelLoad > startTime3 && Time.timeSinceLevelLoad < startTime3 + onScreenDuration)
	{
		//print("Third Credit");
		GUI.Label(Rect(Screen.width*0.1f, Screen.height*0.5f, Screen.width*0.4f, Screen.height*0.05f), "Scripted by DJ CASSEL and ARRIS RAY", nameStyle);
		
		if(Time.timeSinceLevelLoad < startTime3 + fadeLength)
		{
			nameStyle.normal.textColor.a = Mathf.Lerp(0,1,lerpTime);
			lerpTime = lerpTime + Time.deltaTime*0.2f;
		}
		
		if(Time.timeSinceLevelLoad > startTime3 + fadeLength && Time.timeSinceLevelLoad < startTime3 + onScreenDuration - fadeLength)
			lerpTime = 0.0f;
			
		if(Time.timeSinceLevelLoad > startTime3 + onScreenDuration - fadeLength)
		{
			nameStyle.normal.textColor.a = Mathf.Lerp(1,0,lerpTime);
			lerpTime = lerpTime + Time.deltaTime*0.2f;
		}
	}	
	
	// 3 to 4
	if(Time.timeSinceLevelLoad > startTime3 + onScreenDuration && Time.timeSinceLevelLoad < startTime4)
	{
		//print("lerpTime RESET");
		lerpTime = 0.0f;
	}
	
	if(Time.timeSinceLevelLoad > startTime4 && Time.timeSinceLevelLoad < startTime4 + onScreenDuration)
	{
		//print("Fourth Credit");
		GUI.Label(Rect(Screen.width*0.1f, Screen.height*0.7f, Screen.width*0.8f, Screen.height*0.05f), "Based on the online comic strip 'ZORTIC' by MARK MEKKES...", nameStyle);
		
		if(Time.timeSinceLevelLoad < startTime4 + fadeLength)
		{
			nameStyle.normal.textColor.a = Mathf.Lerp(0,1,lerpTime);
			lerpTime = lerpTime + Time.deltaTime*0.2f;
		}
		
		if(Time.timeSinceLevelLoad > startTime4 + fadeLength && Time.timeSinceLevelLoad < startTime4 + onScreenDuration - fadeLength)
			lerpTime = 0.0f;
			
		if(Time.timeSinceLevelLoad > startTime4 + onScreenDuration - fadeLength)
		{
			nameStyle.normal.textColor.a = Mathf.Lerp(1,0,lerpTime);
			lerpTime = lerpTime + Time.deltaTime*0.2f;
		}
	}
	
	// 4 to 5
	if(Time.timeSinceLevelLoad > startTime4 + onScreenDuration && Time.timeSinceLevelLoad < startTime5)
	{
		//print("lerpTime RESET");
		lerpTime = 0.0f;
	}
	
	if(Time.timeSinceLevelLoad > startTime5 && Time.timeSinceLevelLoad < startTime5 + onScreenDuration)
	{
		//print("Fifth Credit");
		onScreenDuration = 10.0f;
		GUI.Label(Rect(Screen.width*0.1f, Screen.height*0.7f, Screen.width*0.8f, Screen.height*0.05f), "...but not any of the actual storylines", nameStyle);
		
		if(Time.timeSinceLevelLoad < startTime5 + fadeLength)
		{
			nameStyle.normal.textColor.a = Mathf.Lerp(0,1,lerpTime);
			lerpTime = lerpTime + Time.deltaTime;
		}
		
		if(Time.timeSinceLevelLoad > startTime5 + fadeLength && Time.timeSinceLevelLoad < startTime5 + onScreenDuration - fadeLength)
			lerpTime = 0.0f;
			
		if(Time.timeSinceLevelLoad > startTime5 + onScreenDuration - fadeLength)
		{
			nameStyle.normal.textColor.a = Mathf.Lerp(1,0,lerpTime);
			lerpTime = lerpTime + Time.deltaTime*0.2f;
		}
	}	
}

*/
