/*

GUIStyle distributionStyle;
GUIStyle productionStyle;

private int cnt = 0;
private float startTime1 = 0.0f;
private float pauseTime1 = 2.0f;
private float outTime1 = 3.0f;
private float deadTime1 = 5.0f;

private float startTime2 = 5.5f;
private float pauseTime2 = 7.5f;
private float outTime2 = 8.5f;
private float deadTime2 = 9.5f;

private float startTime3 = 10.0f;
private float pauseTime3 = 12.0f;
private float outTime3 = 13.0f;

private float lerpTime;
private float fadeTime = 2.0f;

public void Start()
{
	distributionStyle.fontSize = (Screen.width + Screen.height)*0.04f;
	productionStyle.fontSize = (Screen.width + Screen.height)*0.16f;
	lerpTime = fadeTime;
}

public void OnGUI()
{	
	var distributionRect = Rect(0, 0, Screen.width, Screen.height);
	String distributionName = "entireprizeenterprises.com/zortic/";
	var productionRect = Rect(0, 0, Screen.width, Screen.height);
	String productionName = "5CT";
	
	//DISTRIBUTION
	if(Time.timeSinceLevelLoad > startTime1 && Time.timeSinceLevelLoad <= pauseTime1)
	{
		//print("startTime1");
		GUI.Label(distributionRect, distributionName, distributionStyle);
		distributionStyle.normal.textColor.a = Mathf.Lerp(1, 0, lerpTime);
		lerpTime = lerpTime - Time.deltaTime;
	}
	
	if(Time.timeSinceLevelLoad > pauseTime1 && Time.timeSinceLevelLoad <= outTime1)
	{
		//print("pauseTime1");
		GUI.Label(distributionRect, distributionName, distributionStyle);
		distributionStyle.normal.textColor.a = 1.0f;
		lerpTime = fadeTime;
	}
	
	if(Time.timeSinceLevelLoad > outTime1 && Time.timeSinceLevelLoad <= deadTime1)
	{
		//print("outTime1");
		GUI.Label(distributionRect, distributionName, distributionStyle);
		distributionStyle.normal.textColor.a = Mathf.Lerp(0, 1, lerpTime);
		lerpTime = lerpTime - Time.deltaTime;
	}

	if(Time.timeSinceLevelLoad > deadTime1 && Time.timeSinceLevelLoad <= startTime2)
	{
		//print("deadTime1");
		lerpTime = fadeTime;
	}
	
	//PRODUCTION HOUSE 5CT
	if(Time.timeSinceLevelLoad > startTime2 && Time.timeSinceLevelLoad <= pauseTime2)
	{
		//print("startTime2");
		GUI.Label(productionRect, productionName, productionStyle);
		productionStyle.normal.textColor.a = Mathf.Lerp(1, 0, lerpTime);
		lerpTime = lerpTime - Time.deltaTime;
	}
	
	if(Time.timeSinceLevelLoad > pauseTime2 && Time.timeSinceLevelLoad <= outTime2)
	{
		//print("pauseTime2");
		GUI.Label(productionRect, productionName, productionStyle);
		productionStyle.normal.textColor.a = 1.0f;
		lerpTime = fadeTime;
	}
	
	if(Time.timeSinceLevelLoad > outTime2 && Time.timeSinceLevelLoad <= deadTime2)
	{
		//print("outTime2");
		GUI.Label(productionRect, productionName, productionStyle);
		productionStyle.normal.textColor.a = Mathf.Lerp(0, 1, lerpTime);
		lerpTime = lerpTime - Time.deltaTime;
	}

	if(Time.timeSinceLevelLoad > deadTime2 && Time.timeSinceLevelLoad <= startTime3)
	{
		//print("deadTime2");
		lerpTime = fadeTime;
		Application.LoadLevel("MainMenu");
	}
	
}

*/
