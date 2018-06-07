using UnityEngine;
using Opsive.ThirdPersonController;
using BehaviorDesigner.Runtime.Tasks.ThirdPersonController;

namespace BehaviorDesigner.Runtime.Tactical.ThirdPersonController
{
    public class AttackAgentBridge : MonoBehaviour, IAttackAgent
    {
        // The maximum distance that the agent can attack from.
        public float attackDistance;
        // The maximum angle that the agent can attack from.
        public float attackAngle = 100;

        private ItemHandler itemHandler;
        private AIAgent aiAgent;
        private Transform target;

        /// <summary>
        /// Cache the component references.
        /// </summary>
        private void Awake()
        {
            itemHandler = GetComponent<ItemHandler>();
            aiAgent = GetComponent<AIAgent>();
        }

        /// <summary>
        /// Returns the furthest distance that the agent is able to attack from.
        /// </summary>
        /// <returns>The distance that the agent can attack from.</returns>
        public float AttackDistance()
        {
            return attackDistance;
        }

        /// <summary>
        /// Returns the maximum angle that the agent can attack from.
        /// </summary>
        /// <returns>The maximum angle that the agent can attack from.</returns>
        public float AttackAngle()
        {
            return attackAngle;
        }

        /// <summary>
        /// Can the agent attack?
        /// </summary>
        /// <returns>Returns true if the agent can attack.</returns>
        public bool CanAttack()
        {
            // The agent can always try to attack. TryUseItem will fail if the agent cannot attack.
            return true;
        }

        /// <summary>
        /// Does the actual attack.
        /// </summary>
        /// <param name="targetPosition">The position to attack.</param>
        public void Attack(Vector3 targetPosition)
        {
            // The AIAgent component needs a target.
            if (aiAgent != null) {
                if (aiAgent.Target == null) {
                    var targetGameObject = new GameObject(name + " Target");
                    aiAgent.Target = targetGameObject;
                    target = targetGameObject.transform;
                }
                target.transform.position = targetPosition;
            }
            itemHandler.TryUseItem(typeof(PrimaryItemType));
        }
    }
}