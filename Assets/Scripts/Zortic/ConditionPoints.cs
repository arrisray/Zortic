/*

private GameObject sceneCamera;
private GameObject levelTransition;

int pointsRequired = 0;
int pointsCurrent = 0;
int pointsFailure = 0;
GUIStyle pointsStyle = new GUIStyle();
private String pointsDisplayed = null;

bool displayPointsCnt = false;

GameObject NPC[];

public void Start()
{
	sceneCamera = GameObject.Find("Camera");
	levelTransition = GameObject.Find("Camera/StageNodes");
}

public void Update()
{
	pointsDisplayed = pointsCurrent + ""; //converts int to string
}

public void UpdatePoints(int points)
{		
	pointsCurrent += points;
		
	if(pointsFailure < 0 && pointsCurrent <= pointsFailure)
		levelTransition.GetComponent(Navigation).GameOver();
	
	if(pointsCurrent >= pointsRequired)
	{
		if(NPC.length > 0)
			sceneCamera.GetComponent(CameraView).SpawnNPC(NPC, null);
			
		pointsCurrent = 0;
	}
}

public void OnGUI()
{
	if(displayPointsCnt == true)
		GUI.Label(Rect(20,20,200,200), pointsDisplayed, pointsStyle);
}

*/