using UnityEngine;

public class NPC : MonoBehaviour
{}
/*

//SETUP
private GameObject managerPlayer;
private GameObject managerLevel;
private GameObject sceneCamera;
private float screen;
float elevation = 0.0f;
bool isBadguy = true;
bool isMiner = false;

//TYPE OF FLIGHT PATTERN
bool selfPropelled = false;		//is this a mobile entity with a purpose
bool chaotic = false;
bool balanced = false;

//MOVEMENT ATTRIBUTES
float speed = 80.0f;					//top speed
float speedAdrift = 0.25f;			//free floating drift
float bankRate = 1.5f;				//the speed at which the Entity rolls
float bankLimit = 0.0f;				//the maximum desired roll left or right
float turningSpeed = 200.0f;			//turning speed

//MOVEMENT INPUTS
private float inputHorizontal = 0.0f;	//scaleFactor from 0 - 1
private Transform objectTransform;		//node used for visual banking of vessel

//HEALTH
bool canBeTargetLocked = true;
Texture2D health[]; 
bool displayHealth = true;
private float healthCur;
private float healthMax;
private int healthElement;
private float timerGlowBad = 0.0f;
Color collisionColor = Color(0.6f, 0.6f, 0.8f);
private Color originalColor;
private int outlineIndex = 0;

//DESTROYED ATTRIBUTES
GameObject destroyedParticles;
GameObject destroyedDebris[];
bool destroyOnCollision = false;					//destroy this object if collided with by a player
bool destroyOnCollisionOnly = false;				//keeps entity from taking damage when shot
float collisionDamage = 0.0f;							//desired damage dealt to 'other' during a collision
bool collideWithAll = false;						//damage other non-player entities on collision
private float collisionDamageApplied = 0.0f;			//actual damage dealt to 'other' during a collision

//AutoDestruct
float timeOut = 0.0f;

//CALCULATIONS
bool gracePeriod = true;
private float timerProtected = 2.0f;				//display the health GUI and make entity invulnerable for this long of a time
private float timerDirection = 0.0f;				//time until it chooses to change direction
private float timerSpawned = 0.0f;				//variable used for after entity is spawned
private float timerDisplayHealth = 0.0f;			//displays health GUI timer
float timerDirectionMin = 1.0f;					//countdown before changing a new direction
float timerDirectionMax = 3.0f;					//countdown before changing a new direction

//GAME VARIABLES
int pointsRewarded = 100;
bool pointsTowardsCondition = true;

bool pointsRewardedDamage = true;
bool pointsRewardedCollision = false;

bool goalRewardedDamage = false;
bool goalRewardedCollision = true;

bool rechargeEnergy = false;

GameObject payLoad[];
float payLoadPercentChance = 50.0f;		//for a single random payload drop

//ADRIFT
private Vector3 direction;
private float randomizer = 0.0f;
private int coinFlip;

//SHOOTING	
Transform projectile;
float fireRate = 0.01f;		//seconds between shots
private float fireRateCnt;
int volley = 0;
private int volleyCnt = 0;
float angle = 0.0f;			//turret's max range of motion
private Transform turret;				
private GameObject target;	//gameObject selected to be fired at

public void Start ()
{
	managerPlayer = GameObject.Find("Managers/Player");
	managerLevel = GameObject.Find("Managers/Level");
	sceneCamera = GameObject.Find("Camera");
	
	objectTransform = transform.Find("Entity");
	if(!objectTransform)
		objectTransform = this.transform;
		
	turret = transform.Find("Entity/gun/turret");
	if(!turret)
		turret = transform.Find("Entity/turret");
	if(!turret)
		turret = transform.Find("turret");
	if(!turret)
		turret = this.transform;
	
	if(health.length >= 1)
	{
		healthCur = health.length;  //healthCur = number of slots in GUI array
		healthMax = health.length;	//healthMax = number of slots in GUI array
	}
	else
		displayHealth = false;	
	
	timerSpawned = timerProtected;
	timerDisplayHealth = timerProtected;
	fireRateCnt = fireRate;
	timerGlowBad = 0.0f;
	
	for(int i =0; i < objectTransform.renderer.materials.length; i++)
	{
		if(objectTransform.renderer.materials[i].name.IndexOf("outline") >= 0)
		{	
			outlineIndex = i;
			break;
		}
	}
	
	originalColor = objectTransform.renderer.materials[outlineIndex].color;
	
	UpdateDirection();
	
	if(timeOut != 0.0f)
		Invoke ("Destroy", timeOut);
		
	if(selfPropelled == false && chaotic == false && rigidbody)
		Adrift();
}

public void UpdateDirection()
{
	//simulated controller input
	inputHorizontal = Random.Range(-0.5f, 0.5f);
	
	//freefloating values
	var angle = Random.value * Mathf.PI * 2.0f;	
	direction.x = Mathf.Cos(angle) * speedAdrift;
	direction.y = 0;
	direction.z = Mathf.Sin(angle) * speedAdrift;
	
	//rotate left or right
	randomizer = Random.value;
	if(randomizer < 0.5f)
		coinFlip = -1;
	else
		coinFlip = 1;
}

public void Update ()
{
	screen = sceneCamera.GetComponent(CameraView).ScreenFormat();

	//update current health against the length of the health GUI array
	healthElement = healthCur - 1;  //makes zero equals first slot in the array

	if(selfPropelled == false && chaotic == false && !rigidbody)
	{
		transform.Translate(direction, Space.World);
		transform.Rotate(Vector3.one * Time.deltaTime * coinFlip * 20 * randomizer, Space.Self);
	}
	
	if(selfPropelled == false && chaotic == true)
		Floaty();
		
	if(selfPropelled == true && chaotic == false)
		ControlledFlight();
	//print (this.transform.gameObject.name);
	
	if(selfPropelled == true && chaotic == true)
		ChaoticFlight();
	
	if(balanced == true)
	{
		transform.localRotation.x = 0.0f;
		transform.localRotation.z = 0.0f;
	}
		
	transform.position.y = elevation;
	
	//TIMERS AND TRIGGERS FOR HEALTH
	if(timerSpawned > 0.0f && gracePeriod == true)
	{
		collisionDamageApplied = 0.0f;
		timerSpawned -= Time.deltaTime; //at start counter
	}
	else
	{
		collisionDamageApplied = collisionDamage;
		if(projectile)
			Shoot();
	}
		
	if(timerDisplayHealth > 0.0f)
		timerDisplayHealth -= Time.deltaTime; //display health GUI after every hit received
	
	if(timerGlowBad > 0.0f)  //make it check for a material[1]
	{
		objectTransform.renderer.materials[outlineIndex].color = Color.Lerp(originalColor, collisionColor, timerGlowBad);
		timerGlowBad -= Time.deltaTime;
	}
}

public void ControlledFlight()
{
	transform.Translate(Vector3(0,0,1) * speed * Time.deltaTime, Space.Self);
	
	float angle;
	
	//the harder the angle the faster the turn
	angle = (this.turningSpeed * inputHorizontal) * Time.deltaTime;
	this.transform.rotation *= Quaternion.AngleAxis(angle, Vector3(0,1,0));
	this.transform.eulerAngles.x = 0.0f;
	
	//fly current direction until time reaches zero
	timerDirection -= Time.deltaTime;
	
	//have I flown this direction long enough? 
	if(timerDirection <= 0.0f)
	{
		//choose a new direction at random
		inputHorizontal = Random.Range(-0.4f, 0.4f);
		
		//fly this new direction for this long
		if(inputHorizontal < -2.0f || inputHorizontal > 2.0f)
			timerDirection = timerDirectionMin;
		else
			timerDirection = timerDirectionMax;
	}
	
	//bank the entity while turning if bank node exists
	Bank();
	
}

public void ChaoticFlight()
{
	//countdown
	timerDirection -= Time.deltaTime;
	
	//when timer reaches zero, randomly select a new direction around the y-axis only and reset countdown
	if(timerDirection <= 0.0f)
	{
		transform.rotation = Random.rotation;
		transform.rotation.x = 0.0f;
		transform.rotation.z = 0.0f;
		timerDirection = 2.0f * Random.value;
	}
	
	//always propel forward and never up or down
	transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
}

public void Adrift()
{
	float randomX;
	randomX = Random.Range(-1000.0, 1000.0);
	float randomZ;
	randomZ = Random.Range(-1000.0, 1000.0);
		
	rigidbody.AddForce (randomX, 0, randomZ);
}

public void Floaty()
{
	transform.Translate(direction, Space.Self);
    transform.Rotate(Vector3.one * Time.deltaTime * coinFlip * 20 * randomizer, Space.Self);
}

public void Bank()
{	
	float bankSpeed = -1.0f * bankRate * inputHorizontal;
	
	//am I flying a direction other than straight?  if not, change my direction over time
	if(inputHorizontal != 0.0f)
		objectTransform.localRotation *= Quaternion.AngleAxis(bankSpeed, Vector3(0,0,1));
	else
		objectTransform.localRotation = Quaternion.Lerp(objectTransform.localRotation, Quaternion.identity, Time.deltaTime * 2);
	
	//do I stop banking when I reach a certain angle?
	if(bankLimit > 0.0f)
	{
		//am I banking to the right?
		if(objectTransform.localRotation.eulerAngles.z > bankLimit && objectTransform.localRotation.eulerAngles.z < 180.0f)
			//objectTransform.localRotation.eulerAngles = Vector3(0, 0, bankLimit);
			objectTransform.localRotation.eulerAngles.z = bankLimit;
		
		//am I banking to the left?
		if(objectTransform.localRotation.eulerAngles.z > 180.0f && objectTransform.localRotation.eulerAngles.z < (360.0f - bankLimit))
			//objectTransform.localRotation.eulerAngles = Vector3(0, 360.0f - bankLimit, 0);
			objectTransform.localRotation.eulerAngles.z = 360.0f - bankLimit;
	}
}

//when I receive damage
public void ApplyDamage(float damage)
{
	//make sure I'm something that can be damaged
	if(destroyOnCollisionOnly == true)
	{
		return;
	}
	else
	{
		//print(this.gameobject.name + " has been hit for " + damage + " points of damage");
		//am I still alive?
		if(healthCur > 0.0f)
		{
			//reduce health by damage done
			healthCur -= damage;
		
			//start GUI
			timerDisplayHealth = timerProtected;
		
			//start outline glow
			timerGlowBad = damage;
		}
	
		//have I been destroyed?
		if(healthCur <= 0.0f)
		{
			//mark the location I was destroyed
			Vector3 destroyedLocation = this.gameObject.transform.position;
			
			//I'm dead so destroy this entity
			Destroy();
		
			if(goalRewardedDamage == true)
				RewardGoal();
		
			//GameManager.Instance.UnityInstantiate every gameobject listed in the debris array with a direction and speed
			for (var i = 0; i <= destroyedDebris.length - 1; i++)
			{
				GameManager.Instance.UnityInstantiate(destroyedDebris[i], this.transform.position, this.transform.rotation);
				
				if(destroyedDebris[i].rigidbody)
				{
					destroyedDebris[i].rigidbody.velocity = this.rigidbody.velocity;
					destroyedDebris[i].rigidbody.angularVelocity = this.rigidbody.angularVelocity;
				}
			}
	
			//if there are points to reward, reward them now
			/*
			if(pointsRewardedDamage == true && pointsRewarded != 0)
				RewardPoints();
			* /
		
			//if there is a payload, randomly roll a d10 and dump whatever cooresponds to that number
			//roll a number that contains nothing will drop nothing
			if(payLoad.length > 0)
			{
				//chooses a random number from the available array
				int dieRoll = Mathf.FloorToInt(Random.value * payLoad.length * (100/payLoadPercentChance));
			
				//slot rolled will reward whatever is in it
				if(dieRoll < payLoad.length)
					GameManager.Instance.UnityInstantiate(payLoad[dieRoll], this.transform.position, this.transform.rotation);
			}
		}
	}
}

//when I collide with something
public void OnCollisionEnter(Collision other)
{
	//if colliding with a player
	if (other.gameObject.CompareTag("Player"))
	{
		//if I servive the collision make my outline glow for a second
		timerGlowBad = 1.0f;
		
		//and collision is not zero
		if(collisionDamageApplied != 0.0f)
			other.gameObject.SendMessageUpwards("ApplyDamage", collisionDamageApplied, SendMessageOptions.DontRequireReceiver);
		
		if(goalRewardedCollision == true)
			RewardGoal();
				
				/*
		if(pointsRewardedCollision == true && pointsRewarded != 0)
			RewardPoints();
		* /
		
		if(rechargeEnergy != false)
    		other.gameObject.SendMessageUpwards("RechargeEnergy", null, SendMessageOptions.DontRequireReceiver);
			
		//is this object set to be destroyed when collided with
		if(destroyOnCollision == true)
			Destroy();
	}
	else if (collideWithAll)
	{
		//if I servive the collision make my outline glow for a second
		timerGlowBad = 1.0f;
		
		//and collision is not zero
		if(collisionDamageApplied != 0.0f)
			other.gameObject.SendMessageUpwards("ApplyDamage", collisionDamageApplied, SendMessageOptions.DontRequireReceiver);
	}
	
	UpdateDirection();	
}


public void OnGUI()
{	
	//did the timerDisplayHealth start?
	if(timerDisplayHealth > 0.0f)
	{
		//size of GUI displayed around entity
		float healthSize = screen*0.2f;
		
		//location of Entity in GUI space
		Vector2 thisPos = sceneCamera.camera.WorldToScreenPoint(objectTransform.position);
		
		//inverted GUI screen coordinates
		thisPos.y = Screen.height - thisPos.y;
		
		//display current health around entity
		if(displayHealth == true)
			GUI.Label(Rect(thisPos.x-(healthSize/2), thisPos.y-(healthSize/2), healthSize, healthSize), health[healthElement]);
	}
}

public void Shoot()
{
	fireRateCnt -= Time.deltaTime;
	
	RaycastHit hitInfo;
	
	if (Physics.Raycast(turret.position, turret.forward, hitInfo))
	{
		GameObject hitObject = hitInfo.transform.gameObject;
		
		if(hitObject.tag == "Player" && isBadguy)
		{
			target = hitObject;
		}
		
		if(hitObject.tag == "Enemy" && !isBadguy)
		{
			target = hitObject;
		}
		
		if(hitObject.tag == "Asteroid" && isMiner)
		{
			target = hitObject;
		}
	}
		
	if(target != null && fireRateCnt <= 0.0f)
	{
		var bulletInst = GameManager.Instance.UnityInstantiate(projectile, turret.position, turret.rotation);
		fireRateCnt = fireRate;
		volleyCnt += 1;
	}

	if(volleyCnt >= volley)
	{
		target = null;
		volleyCnt = 0;
	}
}

/*
public void RewardPoints()
{
	managerPlayer.GetComponent(PlayerManager).UpdateScore(pointsRewarded);
	managerLevel.GetComponent(DisplayPoints).DisplayPoints(this.gameObject, pointsRewarded);
	if(pointsTowardsCondition == true)
		managerLevel.GetComponent(ConditionPoints).UpdatePoints(pointsRewarded);
}
* /

public void RewardGoal()
{
	managerLevel.GetComponent(ConditionGoals).UpdateGoals(true);
}

public void Destroy()
{
	//did I have damage particles?  if so, use them now.
	if(destroyedParticles != null) 
		GameManager.Instance.UnityInstantiate (destroyedParticles, this.gameObject.transform.position, Quaternion.identity);
  	
  	//destroy this entity
	DestroyObject(gameObject);
}

*/
