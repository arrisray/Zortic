using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! @note Banking targets a dummy node unrelated to physics movement.
public class EntityBanking : MonoBehaviour
{
	struct RigidbodyParams
	{
		public float mass;
		public float drag;
		public float angularDrag;
		public bool useGravity;
		public bool isKinematic;
		public RigidbodyInterpolation interpolation;
		public CollisionDetectionMode collisionDetectionMode;
		public RigidbodyConstraints constraints;

		public RigidbodyParams( Rigidbody rigidbody )
		{
			this.mass = 0.0f;
			this.drag = 0.0f;
			this.angularDrag = 0.0f;
			this.useGravity = false;
			this.isKinematic = false;
			this.interpolation = RigidbodyInterpolation.None;
			this.collisionDetectionMode = CollisionDetectionMode.Discrete;
			this.constraints = RigidbodyConstraints.None;

			this.Store( rigidbody );
		}

		public void Store( Rigidbody rigidbody )
		{
			this.mass = rigidbody.mass;
			this.drag = rigidbody.drag;
			this.angularDrag = rigidbody.angularDrag;
			this.useGravity = rigidbody.useGravity;
			this.isKinematic = rigidbody.isKinematic;
			this.interpolation = rigidbody.interpolation;
			this.collisionDetectionMode = rigidbody.collisionDetectionMode;
			this.constraints = rigidbody.constraints;
		}

		public void Load( Rigidbody rigidbody )
		{
			rigidbody.mass = this.mass;
			rigidbody.drag = this.drag;
			rigidbody.angularDrag = this.angularDrag;
			rigidbody.useGravity = this.useGravity;
			rigidbody.isKinematic = this.isKinematic;
			rigidbody.interpolation = this.interpolation;
			rigidbody.collisionDetectionMode = this.collisionDetectionMode;
			rigidbody.constraints = this.constraints;
		}
	}

    // --------------------------------------------------------------------------
    // DATA MEMBERS

	private EntityManager m_entity = null;
	private float m_currentBankAngle = 0.0f;

    // --------------------------------------------------------------------------
    // PROPERTIES

	public EntityData data { get { return this.m_entity.data; } }
	public EntityInputHandler inputHandler { get { return this.m_entity.inputHandler; } }
	public EntityNavigation navigation { get { return this.m_entity.navigation; } }
	public Transform movementRoot { get { return this.m_entity && this.m_entity.data.movement.rootNode ? this.m_entity.data.movement.rootNode : null; } }
    public Transform rotationRoot { get { return this.m_entity && this.m_entity.data.movement.rotationNode ? this.m_entity.data.movement.rotationNode.transform : null; } }

    // --------------------------------------------------------------------------
    // METHODS

	public void Setup()
	{
	    this.m_entity = this.gameObject.GetComponent<EntityManager>();

		this.CreateRotationNode();
	}

	public void Teardown()
	{
	}
	
	public void Reset()
	{
		this.m_currentBankAngle = 0.0f;
	}

	// --------------------------------------------------------------------------
	//! @brief Update calculation of PC's current banking angle.
    public void ManagedFixedUpdate()
    {
        if (this.movementRoot == null || this.m_entity.inputHandler == null)
        {
            return;
        }
		
		// Update the target banking angle...
		float horizontalDirection = -this.m_entity.inputHandler.direction.x;
        float deltaBankAngle = this.data.movement.bankRate * horizontalDirection * Time.fixedDeltaTime;

		if( this.m_entity.inputHandler.isThrusting && (this.navigation.currentTurnAngle != 0.0f) )
        {
            this.m_currentBankAngle += deltaBankAngle;
			if( this.data.movement.bankLimit != 0.0f )
			{
				this.m_currentBankAngle = Mathf.Clamp( this.m_currentBankAngle, -this.data.movement.bankLimit, this.data.movement.bankLimit );
			}

			// Apply the target banking angle
			Quaternion rotation = this.rotationRoot.rotation;
			Vector3 eulers = this.rotationRoot.rotation.eulerAngles;
			eulers.z = this.m_currentBankAngle;
			rotation.eulerAngles = eulers;
			this.rotationRoot.rotation = rotation;
        }
		// ...or zero it out!
		else
		{
			this.m_currentBankAngle = Mathf.LerpAngle( this.m_currentBankAngle, 0.0f, Time.fixedDeltaTime );

			// Lerp to the neutral banking angle
			Quaternion rotation = this.rotationRoot.rotation;
			Vector3 eulers = rotation.eulerAngles;
			eulers.z = this.m_currentBankAngle;
			rotation.eulerAngles = eulers;
			this.rotationRoot.rotation = rotation;
		}

		// /// Debug.Log( "current=" + this.m_currentBankAngle + ", delta=" + deltaBankAngle );
    }

	protected void CreateRotationNode()
    {
        if( !this.data.movement.createRotationNode )
        {
            this.data.movement.rotationNode = this.gameObject;      
			return;
        }

        if( !this.data.movement.rotationNode )
        {
            // Get root movement node
            Transform rootNode = this.data.movement.rootNode ? this.data.movement.rootNode : this.transform;
   
            // Create rotation node
            string name = "_rotation";
            this.data.movement.rotationNode = new GameObject( name );

            // Get movement node children
            List<Transform> children = new List<Transform>();
            foreach( Transform child in rootNode )
            {
                children.Add( child );
            }

            // Re-parent movement node children to rotation node
            foreach( Transform child in children )
            {
                child.parent = this.data.movement.rotationNode.transform;
            }

            // Attach rotation node to movement root node
            this.data.movement.rotationNode.transform.parent = rootNode;
        }

		// Re-parent movement node children to rotation node
		this.data.movement.rotationNode.transform.localPosition = Vector3.zero;
		this.data.movement.rotationNode.transform.localRotation = Quaternion.identity;
        foreach( Transform child in this.data.movement.rotationNode.transform )
        {
			child.localPosition = Vector3.zero;
			child.localRotation = Quaternion.identity;
        }

		// /// Debug.Log( "rotation-node: pos=" + this.data.movement.rotationNode.transform.localPosition + ", rot=" + this.data.movement.rotationNode.transform.localRotation );
		
		// @note Changing a GameObject hierarchy that contains a Rigidbody at the root and any colliders will screw up physics calcs...
		// @ref http://answers.unity3d.com/questions/16224/dynamically-parenting-a-collider-object-to-a-rigid.html
		this.RefreshRigidbody( ref this.data.movement.rigidbody );
    }

	protected void RefreshRigidbody( ref Rigidbody rigidbody )
	{
		RigidbodyParams rigidbodyParams = new RigidbodyParams( rigidbody );
		GameObject gameObject = rigidbody.gameObject;
		DestroyImmediate( rigidbody );

		rigidbody = gameObject.AddComponent<Rigidbody>();
		rigidbodyParams.Load( rigidbody );
	}
}