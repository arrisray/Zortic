/*

GUIStyle blinkStyle = new GUIStyle();
String blink;
int level;

private GameObject entireprize;
private bool triggerSelection = false;
private GameObject levelTransition;

public void Start()
{
	levelTransition = GameObject.Find("Camera/StageNodes");
	entireprize = GameObject.Find("entirePrize");
	if(entireprize)
		entireprize.animation.Play("idle");
}

public void OnGUI()
{		
	if(Time.timeScale != 0.0)
	{
		if (GUI.Button(Rect(0, 0, Screen.width, Screen.height),"",blinkStyle))
			triggerSelection = true;
				//Application.LoadLevel(level);
				
		if (Time.time % 2 < 1)
		{
			GUI.Label(Rect(0, 0, Screen.width, Screen.height),blink,blinkStyle);
		}
	}
}

public void Update()
{
	if(triggerSelection == true)
		LoadSelection();
}

public void LoadSelection()
{
	if(entireprize)
	{
		entireprize.animation.Play("flyby");
		yield WaitForSeconds (entireprize.animation["flyby"].clip.length);
	}
		
	levelTransition.GetComponent(Navigation).LevelNext(level);
}

*/
