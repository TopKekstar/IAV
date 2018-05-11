using UnityEngine;
#if ENABLE_MULTIPLAYER
using UnityEngine.Networking;
#endif

namespace Opsive.ThirdPersonController
{
    /// <summary>
    /// An item is the base class of anything that can be picked up. 
    /// Items can be used by subscribing to the IUseableItem interface and reloaded by subscribing to the IReloadableItem interface.
    /// </summary>
    public abstract class Item : MonoBehaviour
    {
        [Tooltip("A reference to the item type")]
        [SerializeField] protected ItemType m_ItemType;
        [Tooltip("The name of the Animator substate")]
        [SerializeField] protected string m_ItemName;
        [Tooltip("The states while no other action is being performed.")]
        [SerializeField] protected AnimatorItemCollectionData m_DefaultStates = new AnimatorItemCollectionData("Movement", "Movement", 0.2f, true);
        [Tooltip("The state while aiming")]
        [SerializeField] protected AnimatorItemCollectionData m_AimStates = new AnimatorItemCollectionData("Aim", "Aim", 0.2f, true);
        [Tooltip("The state while equipping the item")]
        [SerializeField] protected AnimatorItemCollectionData m_EquipStates = new AnimatorItemCollectionData("Equip", "Equip", 0.2f, false);
        [Tooltip("The state while unequipping the item")]
        [SerializeField] protected AnimatorItemCollectionData m_UnequipStates = new AnimatorItemCollectionData("Unequip", "Unequip", 0.2f, false);
        [Tooltip("Can the item aim?")]
        [SerializeField] protected bool m_CanAim = true;
        [Tooltip("Is the aim state required for the use state to be called?")]
        [SerializeField] protected bool m_RequireAim = true;
        [Tooltip("The Item sprite used by the UI")]
        [SerializeField] protected Sprite m_ItemSprite;
        [Tooltip("Should the item sprite appear on the right side? Only used when the ItemMonitor.MonitorType is set to left or right hand")]
        [SerializeField] protected bool m_RightItemSprite = true;
        [Tooltip("The Item crosshairs used by the UI")]
        [SerializeField] protected CrosshairsType m_CrosshairsSprite;
        [Tooltip("Does the character hold the item with both hands?")]
        [SerializeField] protected bool m_TwoHandedItem;
        [Tooltip("The IK position of the non-dominant hand")]
        [SerializeField] protected Transform m_NonDominantHandPosition;
        [Tooltip("A reference to the Transform to holster the item to when unequipped. The item will be disabled if no Transform is specified")]
        [SerializeField] protected Transform m_HolsterTarget;
        [Tooltip("A reference to the ItemPickup prefab which will spawn after the character has died")]
        [SerializeField] protected GameObject m_ItemPickup;
        [Tooltip("The name of the camera state to transition to when aiming")]
        [SerializeField] protected string m_AimCameraState;

        /// <summary>
        /// Specifies which item animation priority should play.
        /// </summary>
        public enum ItemAnimationPriority { Low, Medium, High }

        // Exposed properties
        public ItemType ItemType { set { m_ItemType = value; } get { return m_ItemType; } }
        public string ItemName
        {
            set { m_ItemName = value; }
            get
            {
                // If a dual wield item exists then the item name is a concatination between the two items.
                var dualWieldItem = m_CurrentDualWieldItem.Get();
                if (dualWieldItem != null && dualWieldItem.ItemType != m_ItemType) {
                    var primaryName = true;
                    // Determine which name comes first. While this step isn't necessarily necessary, it does make things easier on the animator
                    // because there only needs to be one substate machine per set of dual wielded items. This will normalize the names so one name always comes first.
                    // For example, if the primary item is a Pistol and the dual wielded item is a Shield, the name will be "Pistol Shield". The name will still be
                    // "Pistol Shield" even if the primary item is a Shield and the dual wielded item is a Pistol.
                    if (dualWieldItem.ItemType is PrimaryItemType) {
                        var dualWieldItemType = dualWieldItem.ItemType as PrimaryItemType;
                        for (int i = 0; i < dualWieldItemType.DualWieldItems.Length; ++i) {
                            if (dualWieldItemType.DualWieldItems[i].ItemType.Equals(m_ItemType)) {
                                primaryName = dualWieldItemType.DualWieldItems[i].PrimaryName;
                                break;
                            }
                        }
                    }

                    if (primaryName) {
                        return string.Format("{0} {1}", m_ItemName, dualWieldItem.ItemName);
                    }
                    return string.Format("{0} {1}", dualWieldItem.ItemName, m_ItemName);
                }
                return m_ItemName;
            }
        }
        public AnimatorItemCollectionData DefaultStates { get { return m_DefaultStates; } }
        public AnimatorItemCollectionData AimStates { get { return m_AimStates; } }
        public AnimatorItemCollectionData EquipStates { get { return m_EquipStates; } }
        public AnimatorItemCollectionData UnequipStates { get { return m_UnequipStates; } }
        public bool RequireAim { get { return m_CanAim && m_RequireAim; } }
        public Transform HandTransform { get { return m_HandTransform; } }
        public Sprite ItemSprite { get { return m_ItemSprite; } }
        public bool RightItemSprite { get { return m_RightItemSprite; } }
        public CrosshairsType CrosshairsSprite { get { return m_CrosshairsSprite; } }
        public bool TwoHandedItem { get { return m_TwoHandedItem; } }
        public Transform NonDominantHandPosition { get { return m_NonDominantHandPosition; } }
        public GameObject ItemPickup { get { return m_ItemPickup; } }
        public ItemExtension[] ItemExtensions { get { return m_ItemExtensions; } }

        // SharedFields
        protected SharedProperty<Item> m_CurrentDualWieldItem = null;
#if ENABLE_MULTIPLAYER
        protected SharedMethod<bool> m_IsServer = null;
        protected SharedMethod<bool> m_IsLocalPlayer = null;
#endif
        protected SharedMethod<bool> m_IndependentLook = null;
        private SharedMethod<string, bool, bool> m_ChangeCameraState = null;

        // Internal fields
        private bool m_IsEquipping;
        private bool m_IsUnequipping;
        private Vector3 m_StartLocalPosition;
        private Quaternion m_StartLocalRotation;

        // Component references
        private Transform m_HandTransform;
        private Transform m_ParentTransform;
        private Rigidbody m_Rigidbody;
        protected Collider[] m_Colliders;
        [System.NonSerialized] protected GameObject m_GameObject;
        protected Transform m_Transform;
        protected AnimatorMonitor m_AnimatorMonitor;
        protected GameObject m_Character;
        protected RigidbodyCharacterController m_Controller;
        protected Inventory m_Inventory;
        protected ItemExtension[] m_ItemExtensions;
#if ENABLE_MULTIPLAYER
        protected NetworkMonitor m_NetworkMonitor;
#endif

        /// <summary>
        /// Cache the component references and initialize the default values.
        /// </summary>
        public virtual void Awake()
        {
            // Awake may have already been called if not on the network.
            if (m_HandTransform != null) {
                return;
            }

            // The parent of ItemPlacement will be a bone transform.
            m_HandTransform = GetComponentInParent<ItemPlacement>().transform.parent;

            m_GameObject = gameObject;
            m_Transform = transform;
            m_Rigidbody = GetComponent<Rigidbody>();
            if (m_Rigidbody != null) {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
            m_Colliders = GetComponents<Collider>();
            m_ParentTransform = transform.parent;
            m_StartLocalPosition = m_Transform.localPosition;
            m_StartLocalRotation = m_Transform.localRotation;

            // There can be multiple items attached to the main item.
            m_ItemExtensions = GetComponents<ItemExtension>();
        }

        /// <summary>
        /// The item has been disabled.
        /// </summary>
        private void OnDisable()
        {
            // Stop the item from equipping or unequipping when disabled. This prevents the character from trying to unequip after a respawn if the item was switched just
            // before death.
            m_IsEquipping = false;
            m_IsUnequipping = false;

            if (m_Controller != null) {
                ItemDeactivated();
            }
        }

        /// <summary>
        /// Initializes the item after it has been added to the Inventory.
        /// </summary>
        /// <param name="inventory">The parent character's inventory.</param>
        public virtual void Init(Inventory inventory)
        {
            m_Inventory = inventory;
            m_Character = inventory.gameObject;
#if ENABLE_MULTIPLAYER
            m_NetworkMonitor = m_Character.GetComponent<NetworkMonitor>();
#endif
            m_AnimatorMonitor = inventory.GetComponent<AnimatorMonitor>();
            m_Controller = inventory.GetComponent<RigidbodyCharacterController>();

            // Initialize the animation states.
            m_DefaultStates.Initialize(m_ItemType);
            if (m_CanAim) {
                m_AimStates.Initialize(m_ItemType);
            }
            m_EquipStates.Initialize(m_ItemType);
            m_UnequipStates.Initialize(m_ItemType);

            EventHandler.RegisterEvent(gameObject, "OnInventoryItemEquipping", OnItemEquipping);
            EventHandler.RegisterEvent(gameObject, "OnInventoryItemUnequipping", OnItemUnequipping);
            EventHandler.RegisterEvent(gameObject, "OnInventoryItemEquipped", OnItemEquipped);
            EventHandler.RegisterEvent(gameObject, "OnInventoryItemUnequipped", OnItemUnequipped);

            for (int i = 0; i < m_ItemExtensions.Length; ++i) {
                m_ItemExtensions[i].Init(this, inventory);
            }

            for (int i = 0; i < m_Colliders.Length; ++i) {
                m_Colliders[i].enabled = false;
            }

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
        }
        
        /// <summary>
        /// Returns the destination state for the given layer.
        /// </summary>
        /// <param name="priority">Specifies the item animation priority to retrieve. High priority animations get tested before lower priority animations.</param>
        /// <param name="layer">The Animator layer index.</param>
        /// <returns>The state that the Animator should be in for the given layer. A null value indicates no change.</returns>
        public virtual AnimatorItemStateData GetDestinationState(ItemAnimationPriority priority, int layer)
        {
            // Secondary Items are not visible so cannot aim or play any idle/movement states.
            if (m_ItemType is SecondaryItemType) {
                return null;
            }
            
            if (priority == ItemAnimationPriority.High) {
                // Equip/unequip has the highest priority.
                if (m_IsEquipping) {
                    var equipState = m_EquipStates.GetState(layer, m_Controller.Moving);
                    if (equipState != null) {
                        return equipState;
                    }
                }
                if (m_IsUnequipping) {
                    var unequipState = m_UnequipStates.GetState(layer, m_Controller.Moving);
                    if (unequipState != null) {
                        return unequipState;
                    }
                }

                // Extension item animations have a higher priority than aiming.
                for (int i = 0; i < m_ItemExtensions.Length; ++i) {
                    var extensionState = m_ItemExtensions[i].GetDestinationState(priority, layer);
                    if (extensionState != null) {
                        return extensionState;
                    }
                }

                // No high priority animations need to play. Return null.
                return null;
            } else if (priority == ItemAnimationPriority.Medium) {
                // Aiming is a middle priority animations.
                if (m_CanAim) {
                    if (m_Controller.Aiming) {
                        var aimstate = m_AimStates.GetState(layer, m_Controller.Moving);
                        if (aimstate != null) {
                            return aimstate;
                        }
                    }
                }

                return null;
            }

            // A lower priority animation can play. Play the ability, movement, or idle state.
            var state = m_DefaultStates.GetState(layer, m_Controller.Moving);
            if (state != null) {
                return state;
            }
            return null;
        }

        /// <summary>
        /// The item is being equipped. Play the corresponding animation.
        /// </summary>
        private void OnItemEquipping()
        {
            m_IsUnequipping = false;
            // Issue the equipped event immediately if there is no equip state.
            var state = m_EquipStates.GetState(m_AnimatorMonitor.UpperLayerIndex, m_Controller.Moving);
            if (state == null || string.IsNullOrEmpty(state.Name)) {
                EventHandler.ExecuteEvent(m_Character, "OnAnimatorItemEquipped");
            } else {
                m_IsEquipping = true;
            }
        }

        /// <summary>
        /// The item has been equipped.
        /// </summary>
        private void OnItemEquipped()
        {
            // m_IsEquipping may be false if the equip state name is empty.
            if (m_IsEquipping) {
                m_IsEquipping = false;
            }

            ItemActivated();
        }

        /// <summary>
        /// The item is being unequipped. Play the corresponding animation.
        /// </summary>
        private void OnItemUnequipping()
        {
            m_IsEquipping = false;
            // Issue the equipped event immediately if there is no equip state.
            var state = m_UnequipStates.GetState(m_AnimatorMonitor.UpperLayerIndex, m_Controller.Moving);
            if (state == null || string.IsNullOrEmpty(state.Name)) {
                EventHandler.ExecuteEvent(m_Character, "OnAnimatorItemUnequipped");
            } else {
                m_IsUnequipping = true;
            }

            ItemDeactivated();
        }

        /// <summary>
        /// The item has been unequipped.
        /// </summary>
        private void OnItemUnequipped()
        {
            // m_IsUnequipping may be false if the unequip state name is empty.
            if (m_IsUnequipping) {
                m_IsUnequipping = false;
            }
        }

        /// <summary>
        /// Prepare the item for use.
        /// </summary>
        protected virtual void ItemActivated()
        {
            // Register for any events that the item is interested in while equipped.
            EventHandler.RegisterEvent<bool>(m_Character, "OnControllerAim", OnAim);
            EventHandler.RegisterEvent(m_Character, "OnControllerStartAim", OnStartAim);

            // The collider should be activated.
            for (int i = 0; i < m_Colliders.Length; ++i) {
                m_Colliders[i].enabled = true;
                Physics.IgnoreCollision(m_Controller.CapsuleCollider, m_Colliders[i]);
            }

            // Activate the flashlight/laser sight/aim state if necessary.
            if (m_Controller.Aiming) {
                if (this is IFlashlightUseable) {
                    (this as IFlashlightUseable).ActivateFlashlightOnAim(true);
                }
                if (this is ILaserSightUseable) {
                    (this as ILaserSightUseable).ActivateLaserSightOnAim(true);
                }
                if (!string.IsNullOrEmpty(m_AimCameraState) && m_ChangeCameraState != null) {
                    m_ChangeCameraState.Invoke(m_AimCameraState, true);
                }
            }

            // Notify the extension items.
            for (int i = 0; i < m_ItemExtensions.Length; ++i) {
                m_ItemExtensions[i].ItemExtensionActivated();
            }
        }

        /// <summary>
        /// The item is no longer equipped.
        /// </summary>
        protected virtual void ItemDeactivated()
        {
            // The aim states should begin fresh.
            m_AimStates.ResetNextState();

            // Unregister for any events that the item is only interested in while equipped.
            EventHandler.UnregisterEvent<bool>(m_Character, "OnControllerAim", OnAim);
            EventHandler.UnregisterEvent(m_Character, "OnControllerStartAim", OnStartAim);

            // The collider shouldn't affect the item.
            for (int i = 0; i < m_Colliders.Length; ++i) {
                m_Colliders[i].enabled = false;
            }

            // Disable the flashlight/laser sight.
            if (this is IFlashlightUseable) {
                (this as IFlashlightUseable).ActivateFlashlightOnAim(false);
            }
            if (this is ILaserSightUseable) {
                (this as ILaserSightUseable).ActivateLaserSightOnAim(false);
            }

            // The camera's aim state should no longer play when the item is deactivated.
            if (!string.IsNullOrEmpty(m_AimCameraState) && m_ChangeCameraState != null) {
                m_ChangeCameraState.Invoke(m_AimCameraState, false);
            }

            // Notify the extension items.
            for (int i = 0; i < m_ItemExtensions.Length; ++i) {
                m_ItemExtensions[i].ItemExtensionDeactivated();
            }
        }

        /// <summary>
        /// Callback from the controller when the item starts to aim.
        /// </summary>
        protected virtual void OnStartAim()
        {
            if (!string.IsNullOrEmpty(m_AimCameraState) && m_ChangeCameraState != null) {
                m_ChangeCameraState.Invoke(m_AimCameraState, true);
            }
        }

        /// <summary>
        /// Callback from the controller when the item is aimed or no longer aimed.
        /// <param name="aim">Is the controller aiming?</param>
        /// </summary>
        protected virtual void OnAim(bool aim)
        {
            if (!aim) {
                // Revert the camera state back when the item is no longer aiming.
                if (!string.IsNullOrEmpty(m_AimCameraState) && m_ChangeCameraState != null) {
                    m_ChangeCameraState.Invoke(m_AimCameraState, false);
                }
                // When the character is no longer aiming reset the animation states so they will begin fresh.
                m_AimStates.ResetNextState();
            }
        }

        /// <summary>
        /// Activates or deactivates the item. If there is a holster target then deactivating will mean setting the parent transform to that holster target.
        /// </summary>
        /// <param name="active">Should the item be activated?</param>
        /// <param name="count">The number of items that the character is carrying.</param>
        public virtual void SetActive(bool active, int count)
        {
            if (m_HolsterTarget != null && count > 0) {
                if (active) {
                    m_Transform.parent = m_ParentTransform;
                    m_Transform.localPosition = m_StartLocalPosition;
                    m_Transform.localRotation = m_StartLocalRotation;
                } else {
                    m_Transform.parent = m_HolsterTarget;
                    m_Transform.localPosition = Vector3.zero;
                    m_Transform.localRotation = Quaternion.identity;
                }
                m_GameObject.SetActive(true);
            } else {
                m_GameObject.SetActive(active);
            }
        }
    }
}