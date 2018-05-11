﻿using UnityEngine;
#if !(UNITY_5_3 || UNITY_5_4)
using UnityEngine.AI;
#endif

namespace Opsive.ThirdPersonController
{
    /// <summary>
    /// Acts as a bridge between the NavMeshAgent and Third Person Controller. This link will move the character according to the NavMeshAgent velocity.
    /// </summary>
    public class NavMeshAgentBridge : MonoBehaviour
    {
        [Tooltip("Specifies how quickly the agent should slow down when arriving at the destination")]
        [SerializeField] protected AnimationCurve m_ArriveRampDownCurve = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 1), new Keyframe(1, 0.25f) });

        // Component references
        [System.NonSerialized] private GameObject m_GameObject;
        protected RigidbodyCharacterController m_Controller;
        protected Transform m_Transform;
        protected NavMeshAgent m_NavMeshAgent;
        private Abilities.Jump m_JumpAbility;
        private Abilities.Fall m_FallAbility;

        /// <summary>
        /// Cache the component references.
        /// </summary>
        protected virtual void Awake()
        {
            m_GameObject = gameObject;
            m_Transform = transform;
            m_Controller = GetComponent<RigidbodyCharacterController>();
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_JumpAbility = GetComponent<Abilities.Jump>();
            m_FallAbility = GetComponent<Abilities.Fall>();

            m_NavMeshAgent.autoTraverseOffMeshLink = false;

            EventHandler.RegisterEvent(m_GameObject, "OnDeath", OnDeath);
            EventHandler.RegisterEvent<bool>(m_GameObject, "OnControllerGrounded", OnGrounded);
        }

        /// <summary>
        /// Move according to the NavMeshAgent velocity.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            var velocity = Vector3.zero;
            var lookRotation = Quaternion.LookRotation(m_Transform.forward);
            if (m_NavMeshAgent.isOnOffMeshLink) {
                UpdateOffMeshLink(ref velocity, ref lookRotation);
            } else {
                // Only move if a path exists.
                if (m_NavMeshAgent.desiredVelocity.sqrMagnitude > 0.01f) {
                    if (m_NavMeshAgent.updateRotation) {
                        lookRotation = Quaternion.LookRotation(m_NavMeshAgent.desiredVelocity);
                    } else {
                        lookRotation = Quaternion.LookRotation(m_Transform.forward);
                    }
                    // The normalized velocity should be relative to the look direction.
                    velocity = Quaternion.Inverse(lookRotation) * m_NavMeshAgent.desiredVelocity;
                    // Only normalize if the magnitude is greater than 1. This will allow the character to walk.
                    if (velocity.sqrMagnitude > 1) {
                        velocity.Normalize();
                        // Smoothly come to a stop at the destination.
                        if (m_NavMeshAgent.remainingDistance < 1f) {
                            velocity *= m_ArriveRampDownCurve.Evaluate(1 - m_NavMeshAgent.remainingDistance);
                        }
                    }
                }
            }

            // Don't let the NavMeshAgent move the character - the controller can move it.
            m_NavMeshAgent.updatePosition = false;
            m_NavMeshAgent.velocity = Vector3.zero;
            m_Controller.Move(velocity.x, velocity.z, lookRotation);
            m_NavMeshAgent.nextPosition = m_Transform.position;
        }

        /// <summary>
        /// Updates the velocity and look rotation using the off mesh link.
        /// </summary>
        /// <param name="velocity">The desired velocity.</param>
        /// <param name="lookRotation">The desired look rotation.</param>
        protected virtual void UpdateOffMeshLink(ref Vector3 velocity, ref Quaternion lookRotation)
        {
            if (m_NavMeshAgent.currentOffMeshLinkData.linkType == OffMeshLinkType.LinkTypeDropDown || m_NavMeshAgent.currentOffMeshLinkData.linkType == OffMeshLinkType.LinkTypeJumpAcross) {
                // Ignore the y difference when determining a look direction and velocity.
                // This will give XZ distances a greater impact when normalized.
                var direction = m_NavMeshAgent.currentOffMeshLinkData.endPos - m_Transform.position;
                direction.y = 0;
                if (direction.sqrMagnitude > 0.1f || m_Controller.Grounded) {
                    velocity = m_Transform.InverseTransformPoint(m_NavMeshAgent.currentOffMeshLinkData.endPos);
                    velocity.y = 0;
                    velocity.Normalize();
                }

                // Jump if the agent hasn't jumped yet.
                if (m_JumpAbility != null && m_NavMeshAgent.currentOffMeshLinkData.linkType == OffMeshLinkType.LinkTypeJumpAcross) {
                    if (!m_JumpAbility.IsActive && (m_FallAbility == null || !m_FallAbility.IsActive)) {
                        m_Controller.TryStartAbility(m_JumpAbility);

                        // Only update the LookRotation when the character starts jumping to prevent the agent from rotating while in the air.
                        lookRotation = Quaternion.LookRotation(direction);
                    }
                }
            }
        }

        /// <summary>
        /// The character has changed grounded state. 
        /// </summary>
        /// <param name="grounded">Is the character on the ground?</param>
        protected virtual void OnGrounded(bool grounded)
        {
            if (grounded) {
                // The agent is no longer on an off mesh link if they just landed.
                if (m_NavMeshAgent.isOnOffMeshLink && (m_NavMeshAgent.currentOffMeshLinkData.linkType == OffMeshLinkType.LinkTypeDropDown ||
                                                       m_NavMeshAgent.currentOffMeshLinkData.linkType == OffMeshLinkType.LinkTypeJumpAcross)) {
                    m_NavMeshAgent.CompleteOffMeshLink();
                }
                // Warp the NavMeshAgent just in case the navmesh position doesn't match the transform position.
                var destination = m_NavMeshAgent.destination;
                m_NavMeshAgent.Warp(m_Transform.position);
                // Warp can change the destination so make sure that doesn't happen.
                if (m_NavMeshAgent.destination != destination) {
                    m_NavMeshAgent.SetDestination(destination);
                }
            }
        }

        /// <summary>
        /// The character has died. Don't move anymore.
        /// </summary>
        private void OnDeath()
        {
            enabled = false;
            m_NavMeshAgent.updateRotation = false;
            EventHandler.UnregisterEvent(m_GameObject, "OnDeath", OnDeath);
            EventHandler.RegisterEvent(m_GameObject, "OnRespawn", OnRespawn);
        }

        /// <summary>
        /// The character has respawned. Start moving again.
        /// </summary>
        private void OnRespawn()
        {
            // Reset the NavMeshAgent to the new position.
            m_NavMeshAgent.Warp(transform.position);
            if (m_NavMeshAgent.isOnOffMeshLink) {
                m_NavMeshAgent.ActivateCurrentOffMeshLink(false);
            }
            m_NavMeshAgent.updateRotation = true;

            enabled = true;
            EventHandler.RegisterEvent(m_GameObject, "OnDeath", OnDeath);
            EventHandler.UnregisterEvent(m_GameObject, "OnRespawn", OnRespawn);
        }
    }
}