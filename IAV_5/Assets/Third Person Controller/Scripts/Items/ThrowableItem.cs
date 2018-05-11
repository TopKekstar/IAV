using UnityEngine;

namespace Opsive.ThirdPersonController
{
    /// <summary>
    /// Any Item that can be thrown, such as a grenade or baseball. The GameObject that the ThrowableItem attaches to is not the actual object that is thrown - the ThrownObject field
    /// specifies this instead. This GameObject is used by the Inventory to know that a ThrowableItem exists, and by the ItemHandler to actually use the Item.
    /// </summary>
    public class ThrowableItem : Item, IUseableItem
    {
        [Tooltip("The input name mapped to use the item")]
        [SerializeField] protected string m_UseInputName = "Fire1";
        [Tooltip("Can the item be used in the air?")]
        [SerializeField] protected bool m_CanUseInAir = true;
        [Tooltip("The object that can be thrown. Must have a component that implements IThrownObject")]
        [SerializeField] protected GameObject m_ThrownObject;
        [Tooltip("The number of objects that can be thrown per second")]
        [SerializeField] protected float m_ThrowRate = 1;
        [Tooltip("The force applied to the object thrown")]
        [SerializeField] protected Vector3 m_ThrowForce;
        [Tooltip("The torque applied to the object thrown")]
        [SerializeField] protected Vector3 m_ThrowTorque;
        [Tooltip("A random spread to allow some inconsistency in each throw")]
        [SerializeField] protected float m_Spread;
        [Tooltip("The state while using the item")]
        [SerializeField] protected AnimatorItemCollectionData m_UseStates = new AnimatorItemCollectionData("Use", "Use", 0.1f, true);
        
        // Exposed properties for the Item Builder
        public AnimatorItemCollectionData UseStates { get { return m_UseStates; } }

        // SharedFields
        private SharedMethod<Vector3, bool, Vector3> m_TargetLookDirectionLookPoint = null;

        // Internal variables
        private float m_ThrowDelay;
        private float m_LastThrowTime;
        private bool m_Throwing;
        private bool m_Initialized;

        // Component references
        private Transform m_CharacterTransform;

        /// <summary>
        /// Cache the component references and initialize the default values.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            m_ThrowDelay = 1.0f / m_ThrowRate;
            m_LastThrowTime = -m_ThrowDelay;
        }

        /// <summary>
        /// Initializes the item after it has been added to the Inventory.
        /// </summary>
        /// <param name="inventory">The parent character's inventory.</param>
        public override void Init(Inventory inventory)
        {
            base.Init(inventory);

            // Initialize the animation states.
            m_UseStates.Initialize(m_ItemType);

            m_CharacterTransform = m_Character.transform;
        }

        /// <summary>
        /// The item is no longer equipped.
        /// </summary>
        protected override void ItemDeactivated()
        {
            TryStopUse();

            base.ItemDeactivated();

            // The animation states should begin fresh.
            m_UseStates.ResetNextState();
        }

        /// <summary>
        /// Returns the input name for the item to be used.
        /// </summary>
        /// <param name="dualWield">Is the dual wield mapping being retrieved?</returns>
        /// <returns>The input name for the item to be used.</returns>
        public string GetUseInputName(bool dualWield)
        {
            return m_UseInputName;
        }

        /// <summary>
        /// Returns the destination state for the given layer.
        /// </summary>
        /// <param name="priority">Specifies the item animation priority to retrieve. High priority animations get tested before lower priority animations.</param>
        /// <param name="layer">The Animator layer index.</param>
        /// <returns>The state that the Animator should be in for the given layer. A null value indicates no change.</returns>
        public override AnimatorItemStateData GetDestinationState(ItemAnimationPriority priority, int layer)
        {
            var state = base.GetDestinationState(priority, layer);
            if (state != null) {
                return state;
            }

            // Item use is a high priority item.
            if (priority == ItemAnimationPriority.High) {
                if (InUse()) {
                    state = m_UseStates.GetState(layer, m_Controller.Moving);
                    if (state != null) {
                        return state;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Try to throw the object. An object may not be able to be thrown if another object was thrown too recently, or if there are no more thrown objects remaining (out of ammo).
        /// <returns>True if the item was used.</returns>
        /// </summary>
        public bool TryUse()
        {
            // Throwable Items aren't always visible (such a Secondary item) so Start() isn't always called. Initialize the SharedFields when the item is trying to be used.
            if (!m_Initialized) {
                SharedManager.InitializeSharedFields(m_Character, this);
                // Independent look characters do not need to communicate with the camera. Do not initialze the SharedFields on the network to prevent non-local characters from
                // using the main camera to determine their look direction. The SharedFields have been implemented by the NetworkMonitor component.
#if ENABLE_MULTIPLAYER
                if (!m_IndependentLook.Invoke() && m_IsLocalPlayer.Invoke()) {
#else
                if (!m_IndependentLook.Invoke()) {
#endif
                    SharedManager.InitializeSharedFields(Utility.FindCamera(m_Character).gameObject, this);
                }
                m_Initialized = true;
            }
            if (m_LastThrowTime + m_ThrowDelay < Time.time && m_Inventory.GetItemCount(m_ItemType) > 0) {
                // Returns true to tell the ItemHandler that the item was used. The Used callback will be registered and the object will actually be thrown within that method.
                m_Throwing = true;
                EventHandler.ExecuteEvent(m_Character, "OnItemUse");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Can the item be thrown?
        /// </summary>
        /// <returns>True if the item can be thrown.</returns>
        public bool CanUse()
        {
            if (!m_CanUseInAir && !m_Controller.Grounded) {
                return false;
            }
            return !m_Throwing;
        }

        /// <summary>
        /// Is the object currently being thrown?
        /// </summary>
        /// <returns>True if the object is currently being thrown.</returns>
        public bool InUse()
        {
            return m_Throwing; 
        }

        /// <summary>
        /// The thrown object cannot be stopped because it is atomic.
        /// </summary>
        public void TryStopUse()
        {

        }

        /// <summary>
        /// Throw the object.
        /// </summary>
        public virtual void Used()
        {
#if ENABLE_MULTIPLAYER
            // The server will spawn the GameObject and it will be sent to the clients.
            if (!m_IsServer.Invoke()) {
                return;
            }
#endif

            var thrownGameObject = ObjectPool.Spawn(m_ThrownObject, m_Transform.position, Quaternion.LookRotation(ThrowDirection()) * m_ThrownObject.transform.rotation);
            var thrownObject = (IThrownObject)(thrownGameObject.GetComponent(typeof(IThrownObject)));
            thrownObject.ApplyThrowForce(m_Character, m_ThrowForce, m_ThrowTorque);

            m_LastThrowTime = Time.time;
            m_Throwing = false;
            m_Inventory.UseItem(m_ItemType, 1);

            EventHandler.ExecuteEvent(m_Character, "OnItemStopUse");
        }

        /// <summary>
        /// Determines the direction to throw based on the camera's look position and a random spread.
        /// </summary>
        /// <returns>The direction to throw.</returns>
        private Vector3 ThrowDirection()
        {
            Vector3 direction;
            // If m_TargetLookDirectionLookPoint is null then use the forward direction. It may be null if the AI agent doesn't have the AIAgent component attached.
            if (m_TargetLookDirectionLookPoint == null) {
                direction = m_CharacterTransform.forward;
            } else {
                direction = m_TargetLookDirectionLookPoint.Invoke(m_Transform.position, true).normalized;
                // Don't let the character throw in the opposite direction of the weapon.
                if (Vector3.Dot(m_CharacterTransform.forward, direction) < 0) {
                    direction = m_CharacterTransform.forward;
                }
            }

            // Add a random spread.
            if (m_Spread > 0) {
                var variance = Quaternion.AngleAxis(Random.Range(0, 360), direction) * Vector3.up * Random.Range(0, m_Spread);
                direction += variance;
            }

            return direction;
        }
    }
}