using UnityEngine;


using Opsive.ThirdPersonController;


public class GhostRespawner : Respawner
{
    
    public override void Spawn()
    {
        var location = GhostSpawnSelection.GetSpawnLocation();

        SpawnLocal(location.position, location.rotation);
    }
    
    private void SpawnLocal(Vector3 position, Quaternion rotation)
    {
        m_RespawnEvent = null;

        transform.SetPositionAndRotation(position, rotation);
        m_GameObject.SetActive(true);

        EventHandler.ExecuteEvent(m_GameObject, "OnRespawn");
    }
}