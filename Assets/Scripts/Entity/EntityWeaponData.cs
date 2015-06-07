using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntityWeaponData : EntityAttachmentData
{
    // --------------------------------------------------------------------------
    // CLASSES

    [Serializable]
    public class VisualData
    {
        public GameObject projectile = null;
        public Texture2D targetLock;         //GUI image on targeted object
        public bool showReticle = true;
    }

    [Serializable]
    public class ProjectileData
    {
		[Serializable]
		public class VisualInfo
		{
			public float trailLength = 1.0f;
			public Vector2 trailWidth = Vector2.one;
			public float headSize = 1.0f;
			
			public VisualInfo()
			{
			}
			
			public VisualInfo( VisualInfo info )
			{
				this.trailLength = info.trailLength;
				this.trailWidth = info.trailWidth;
				this.headSize = info.headSize;
			}
		}
		
        public bool isHoming = false;
        public float fireRate = 0.1f; //! @brief Cool down period after each shot (in seconds).
        public float energyUse = 0.25f; //per shot energy pool use for primary fire
        public int volleyCount = 1;
        public float volleyArc = 22.5f; //! @brief Half-angle in degrees about the weapon system's forward vector.
		public float trailLengthDelta = 1.5f;
		public Vector2 trailWidthDelta = new Vector2( 1.0f, 0.0f );
		public float headSizeDelta = 1.5f;
		public VisualInfo initVisualInfo = new VisualInfo();
		[HideInInspector] public VisualInfo currentVisualInfo = new VisualInfo();
    }   

    [Serializable]
    public class TargetingData
    {
        public bool isAutomated = false;
        public bool performRangeCheck = false;
        public float trackingArc = 22.5f; //! @brief Half-angle in degrees about the weapon system's forward vector.
        public List<EntityTag> targetFilter = new List<EntityTag>();
    }

    [Serializable]
    public class GameData
    {
        public String id = String.Empty;
        public EntityWeapon.ActionType actionType = EntityWeapon.ActionType.Unknown;
        public bool showInsufficientEnergyWarning = false;
        public int damage = 1;
		public int maxDamage = 25;
        [HideInInspector] public EntityManager proxyForEntity = null;
    }

    // --------------------------------------------------------------------------
    // DATA MEMBERS

    public VisualData visual = new VisualData();
    public ProjectileData projectile = new ProjectileData();
    public TargetingData targeting = new TargetingData();
    public GameData game = new GameData();

    // --------------------------------------------------------------------------
    // METHODS

    public void OnEnable()
    {
        base.attachmentType = EntityAttachmentData.Type.WeaponSystem;
    }
} // end public class EntityWeaponSystem
