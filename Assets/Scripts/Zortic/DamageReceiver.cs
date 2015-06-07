/*

//COMMUNICATIONS
private GameObject managerPlayer;
private GameObject managerLevel;

//GENERAL ATTRIBUTES
Texture2D gizmoImage;
bool canBeTargetLocked = true;
float health = 1.0f;
GameObject damageParticles;

//EXPLOSION ATTRIBUTES
GameObject explosionDebris[];
GameObject explosionParticles;

//GAME VARIABLES
bool pointsReward = true;
int pointsAmount = 100;
bool pointsDisplay = true;

GameObject payLoad[];
int = 0; // 0.0 = 0%, 1.0 payLoadIncrement = 100%

public void OnDrawGizmos()
{
	Gizmos.DrawIcon(transform.position, gizmoImage.name, true);
}

public void Start()
{
	managerPlayer = GameObject.Find("ManagerPlayer");
	managerLevel = GameObject.Find("ManagerLevel");
}

public void ApplyDamage(float damage)
{	
	if(health > 0.0f)
		health -= damage;

	if(health <= 0.0f)
	{
		Vector3 destroyedLocation = this.gameObject.transform.position;
		
		GameManager.Instance.UnityDestroythis.gameObject);

		for (var i = 0; i <= explosionDebris.length - 1; i++)
		{
			GameManager.Instance.UnityInstantiate(explosionDebris[i], this.transform.position, this.transform.rotation);
			explosionDebris[i].rigidbody.velocity = this.rigidbody.velocity;
			explosionDebris[i].rigidbody.angularVelocity = this.rigidbody.angularVelocity;
		}

		if(explosionParticles)
			GameManager.Instance.UnityInstantiate(explosionParticles, this.transform.position, this.transform.rotation);	
		
			/*
		if(pointsReward == true)
		{
			managerPlayer.GetComponent(PlayerManager).UpdateScore(pointsAmount);
			if(pointsDisplay == true)
				managerLevel.GetComponent(DisplayPoints).DisplayPoints(this.gameObject, pointsAmount);
		}
		* /
		
		if(payLoad.length > 0)
		{
			int d10 = Mathf.FloorToInt(Random.value * 10.0f);
			
			//print(d10);
			if(payLoadIncrement == d10)
			{
				print("dropping payload");
				GameManager.Instance.UnityInstantiate(payLoad[payLoadIncrement], this.transform.position, this.transform.rotation);
			}
		}	
	}
}

*/