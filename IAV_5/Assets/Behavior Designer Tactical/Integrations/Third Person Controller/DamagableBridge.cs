using UnityEngine;
using Opsive.ThirdPersonController;

namespace BehaviorDesigner.Runtime.Tactical.ThirdPersonController
{
    public class DamagableBridge : MonoBehaviour, IDamageable
    {
        // Internal variables
        private bool alive = true;

        /// <summary>
        /// Registers for the OnDeath event.
        /// </summary>
        private void Awake()
        {
            EventHandler.RegisterEvent(gameObject, "OnDeath", OnDeath);
        }

        /// <summary>
        /// Take damage by the specified amount.
        /// </summary>
        /// <param name="amout">The amount of damage to take.</param>
        public void Damage(float amount)
        {
            // Intentionally left blank. The Third Person Controller handles damage on its own.
        }

        /// <summary>
        /// Is the object currently alive?
        /// </summary>
        /// <returns>True if the object is alive.</returns>
        public bool IsAlive()
        {
            return alive;
        }

        /// <summary>
        /// The agent has died and is no longer alive.
        /// </summary>
        private void OnDeath()
        {
            alive = false;
            EventHandler.RegisterEvent(gameObject, "OnRespawn", OnRespawn);
            EventHandler.UnregisterEvent(gameObject, "OnDeath", OnDeath);
        }

        /// <summary>
        /// The agent has respawned so is alive again.
        /// </summary>
        private void OnRespawn()
        {
            alive = true;
            EventHandler.RegisterEvent(gameObject, "OnDeath", OnDeath);
            EventHandler.UnregisterEvent(gameObject, "OnRespawn", OnRespawn);
        }
    }
}