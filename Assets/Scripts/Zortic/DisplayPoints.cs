/*

private GameObject sceneCamera; //camera for world to screen conversion
private Vector3 location; //location points should display on screen
private float pointsRewarded = 0.0f; //number of points rewarded from destroyed object

float timer = 1.0f; //length of time the points will be visible
private float timerCurrent = 0.0f; //variable counter

GUIStyle scoreStyle = new GUIStyle(); //sytle of displayed points

float scaleTime = 0.0f;

private Color origColor;
Color transColor = Color.black;

public void Start()
{
	sceneCamera = GameObject.Find("Camera");
	origColor = scoreStyle.normal.textColor;
}

public void DisplayPoints(GameObject object, float amount)
{
	location = object.transform.position;
	pointsRewarded = amount;
	timerCurrent = timer;
}

public void Update()
{
	if(timerCurrent > 0.0f)
	{
		timerCurrent = timerCurrent - Time.deltaTime;
		scaleTime += Time.deltaTime * 1.5f;
	}
	
}

public void OnGUI()
{
	if(pointsRewarded > 0.0f && location != null)
	{
		float screen;
		if(Screen.height < Screen.width)
			screen = Screen.height;
		else
			screen = Screen.width;
	
		Vector2 screenPos = sceneCamera.camera.WorldToScreenPoint(location);
		screenPos.y = Screen.height - screenPos.y;
		
		scoreStyle.fontSize = screen * 0.1f;
		
		if(timerCurrent > 0.0f)
		{
			String pointsText = pointsRewarded + "";
			//scoreSizeCur = Mathf.Lerp(scoreSizeStart, scoreSizeEnd, timerCurrent);
			scoreStyle.normal.textColor = Color.Lerp(origColor, transColor, scaleTime);
			GUI.Label (Rect(screenPos.x, screenPos.y, screen*0.2f, screen*0.2f), pointsText, scoreStyle);
		}
		else
		{
			scaleTime = 0.0f;
		}
	}
}

*/
