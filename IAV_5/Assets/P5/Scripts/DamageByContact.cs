using UnityEngine;
using Opsive.ThirdPersonController;

/// <summary>
/// Allows an object with the Invetory component to pickup items when that object enters the trigger.
/// </summary>
public class DamageByContact: MonoBehaviour
{
    [Tooltip("The amount of damage to deal")]
    [SerializeField] protected float damageAmount;

    
    public virtual void OnTriggerEnter(Collider other)
    {
#if ENABLE_MULTIPLAYER
           
            if (!isServer) {
                return;
            }
#endif
        
		Health health= Utility.GetComponentForType<Health>(other.gameObject);
		
        if (health != null)
        {
            health.Damage(damageAmount, transform.position,Vector3.zero);

     
        }
    }
}