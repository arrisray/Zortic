/*

//gameObject = managerPlayer

private GameObject sceneCamera;

bool spawnFromStart = false;
GameObject NPC[];
int NPCStart = 3;
int NPCCap = 8;

float spawnTime = 3.0f;
[HideInInspector] bool beginSpawning = false;
private float spawnCountDown = 0.0f;

public void Start()
{
	sceneCamera = GameObject.Find("Camera");
	
	if(spawnFromStart)
	{
		for (int i = 0; i < NPCStart; i++)
			sceneCamera.GetComponent(CameraView).SpawnNPC(NPC, transform);
		
		beginSpawning = true;
	}
}

public void Update()
{
	if(beginSpawning == true)
		RequestSpawnNPC();
}

public void RequestSpawnNPC()
{
	if (transform.childCount < NPCCap)
	{
		spawnCountDown -= Time.deltaTime;
		
		if (spawnCountDown <= 0.0f)
		{
			sceneCamera.GetComponent(CameraView).SpawnNPC(NPC, transform);
			spawnCountDown = spawnTime;
		}
	}	
}

*/
