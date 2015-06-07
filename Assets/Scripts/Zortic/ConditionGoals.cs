/*

private GameObject sceneCamera;
private GameObject levelTransition;

int goalsRequired = 0;
int goalsCurrent = 0;
GUIStyle goalsStyle = new GUIStyle();
int goalsToEnd = 3;
private int goalsToEndCnt = 0;
private String goalsDisplayed = null;
private String goalsRequiredDisplayed = null;

bool displayGoalCnt = false;

GameObject NPC[];

public void Start()
{
	sceneCamera = GameObject.Find("Camera");
	levelTransition = GameObject.Find("Camera/StageNodes");
	goalsRequiredDisplayed = goalsRequired + ""; //converts int to string
}

public void Update()
{
	goalsDisplayed = goalsCurrent + ""; //converts int to string
	
	if(goalsCurrent >= goalsRequired)
	{
		if(NPC.length > 0)
		{
			sceneCamera.GetComponent(CameraView).SpawnNPC(NPC, null);
			goalsCurrent = 0;
		}
		
		goalsToEndCnt++;
		
	}
	//if(goalsToEndCnt >= goalsToEnd)
	//	levelTransition.GetComponent(LevelTransition).LevelNext(0);
}

public void UpdateGoals(bool condition)
{
	if(condition == true)
		goalsCurrent++;
}

public void OnGUI()
{	
	//screen depth
	GUI.depth = 1;
	
	if(displayGoalCnt == true)
	{
		if(goalsCurrent < goalsRequired)
			GUI.Label(Rect(Screen.width/2,0,200,200), goalsDisplayed + "/" + goalsRequiredDisplayed, goalsStyle);
		
		if(goalsToEndCnt == goalsToEnd)
			GUI.Label(Rect(Screen.width/2,0,200,200), "Success", goalsStyle);
	}
}

*/
