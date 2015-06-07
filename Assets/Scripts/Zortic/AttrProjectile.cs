/*

GameObject parentEntity = null;

//MOVEMENT
float travelSpeed = 100.0f;
float homingRate = 0.0f; 			
public GameObject homingTarget; 	//object to be tracked by projectile - determined by AttributeWeapon script

//HIT TARGET RESOLUTION - OnCollisionEnter variables
private Collision hitObject;
bool onlyDamagePlayer = false;

float impactDamage = 1.0f;
float impactForce = 0.0f;
GameObject impactParticle;
bool impactExplosion = false;
float impactExplosionRadius = 0.0f;
float impactExplosionDamage = 0.0f;
float impactExplosionForce = 0.0f;

//TIMED OUT RESOLUTION
float selfDestructTimer = 3.0f;
GameObject selfDestructParticle;
bool selfDestructExplosion = false;
float selfDestructExplosionRadius;
float selfDestructExplosionDamage = 1.0f;
float selfDestructExplosionForce = 0.0f;

//DEBUG HELPERS
bool visualizeExplosionRadius = false;

public void Start()
{
	Invoke("SelfDestruct", selfDestructTimer);
}

public void Update()
{
	if(homingTarget)
	{
		transform.Translate(0.0f, 0.0f, travelSpeed * Time.deltaTime);
		var trackSpeed = Quaternion.LookRotation(homingTarget.transform.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, trackSpeed, Time.deltaTime * homingRate);
	}
	else
		transform.Translate(0.0f, 0.0f, travelSpeed * Time.deltaTime);
		
	transform.position.y = 0.0f;
}

public void Homing(GameObject target)
{
	homingTarget = target;
}

public void OnCollisionEnter(Collision other)
{
	if ( other.gameObject && (other.gameObject == this.parentEntity) )
	{
		/// Debug.Log( "Ignoring collision between " + other.gameObject.name + " and " + this.parentEntity.name );
		return;
	}

	/// Debug.Log ("Projectile hit: " + other.gameObject.name);

	hitObject = other;
	
	if(onlyDamagePlayer == true)
	{
		if(other.gameObject.tag == "Player")
			DamageHitObject();
		else
		{
			DestroyProjectile();
		}
	}
	else
	{
		DamageHitObject();
	}
}
public void DamageHitObject()
{
	if(visualizeExplosionRadius == true)
	{
		GameObject mySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		mySphere.transform.localScale = Vector3(impactExplosionRadius, impactExplosionRadius, impactExplosionRadius);
		mySphere.transform.position = this.transform.position;
		mySphere.collider.enabled = false;
		GameManager.Instance.UnityDestroymySphere, 2.0f);
	}
	if(impactDamage != 0.0f)
		hitObject.gameObject.SendMessageUpwards("ApplyDamage", impactDamage, SendMessageOptions.DontRequireReceiver);
	
	if(impactForce != 0.0f && hitObject.gameObject.rigidbody)
		hitObject.gameObject.rigidbody.AddExplosionForce(impactExplosionForce, this.gameObject.transform.position, 0.5f, 0.0f);
	
	if(impactParticle != null)
    	GameManager.Instance.UnityInstantiate (impactParticle, this.gameObject.transform.position, Quaternion.identity);
	
    if(impactExplosion == true)
    {
		Collider[] bystanders = Physics.OverlapSphere (this.gameObject.transform.position, impactExplosionRadius);

		foreach (Collider bystander in bystanders)
    	{    		
			if (!bystander || bystander.name == hitObject.gameObject.name)
				continue;
				
        	//print(bystander.name);
        	
			// Calculate distance from the explosion position to the closest point on the collider
			var closestPoint = bystander.ClosestPointOnBounds(this.gameObject.transform.position);
			var distance = Vector3.Distance(closestPoint, this.gameObject.transform.position);
		
			if(impactExplosionDamage != 0.0f && bystanders)
			{
				// The hit points we apply fall decrease with distance from the explosion point
				var explosionDamage = 1.0f - Mathf.Clamp01(distance/impactExplosionRadius);
				explosionDamage *= impactExplosionDamage;
				bystander.SendMessageUpwards("ApplyDamage", explosionDamage, SendMessageOptions.DontRequireReceiver);
        	}
        		
			if (impactExplosionForce != 0.0f && bystander.rigidbody)
			{
				var explosionForce = 1.0f - Mathf.Clamp01(distance/impactExplosionRadius);
				explosionForce *= impactExplosionForce;
				bystander.rigidbody.AddExplosionForce(explosionForce, this.gameObject.transform.position, impactExplosionRadius, 0.0f);
			}
    	}
    }
	
	DestroyProjectile();
}

public void SelfDestruct()
{
	if(visualizeExplosionRadius == true)
	{
		GameObject mySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		mySphere.transform.localScale = Vector3(selfDestructExplosionRadius, selfDestructExplosionRadius, selfDestructExplosionRadius);
		mySphere.transform.position = this.transform.position;
		mySphere.collider.enabled = false;
		GameManager.Instance.UnityDestroymySphere, 2.0f);
	}
	
	if(selfDestructParticle != null)
    	GameManager.Instance.UnityInstantiate (selfDestructParticle, this.gameObject.transform.position, Quaternion.identity);
    	
	if(selfDestructExplosion == true)
    {
		Collider[] bystanders = Physics.OverlapSphere (this.gameObject.transform.position, selfDestructExplosionRadius);
    
		foreach (Collider bystander in bystanders)
    	{
			if (!bystander)
				continue;
        	
        	//print(bystander.name); //print out the name of the bystander
        	
        	// Calculate distance from the explosion position to the closest point on the collider
			var closestPoint = bystander.ClosestPointOnBounds(this.gameObject.transform.position);
			var distance = Vector3.Distance(closestPoint, this.gameObject.transform.position);
		
			if(selfDestructExplosionDamage != 0.0f)
			{
				// The hit points we apply fall decrease with distance from the explosion point
				var explosionDamage = 1.0 - Mathf.Clamp01(distance/selfDestructExplosionRadius);
				explosionDamage *= selfDestructExplosionDamage;
				bystander.SendMessageUpwards("ApplyDamage", explosionDamage, SendMessageOptions.DontRequireReceiver);
        	}
        		
			if (selfDestructExplosionForce != 0.0f && bystander.rigidbody)
			{
				var explosionForce = 1.0f - Mathf.Clamp01(distance/selfDestructExplosionRadius);
				explosionForce *= selfDestructExplosionForce;
				bystander.rigidbody.AddExplosionForce(explosionForce, this.gameObject.transform.position, selfDestructExplosionRadius, 0.0f);
			}
    	}
    }
    
	DestroyProjectile();
}

public void DestroyProjectile()
{
	GameManager.Instance.UnityDestroythis.gameObject);
}

*/
