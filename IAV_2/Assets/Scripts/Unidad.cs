using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unidad : MonoBehaviour {
    Mapa mapa;
    Queue<Vector2Int> camino;
    // Use this for initialization
    void Start () {
        mapa = transform.parent.gameObject.GetComponent<Mapa>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        Debug.Log("unidad Clickada");
        GameManager.instance.setCurrentUnit(gameObject);
    }

    public void setPath(Queue<Vector2Int> c)
    {
        camino = c;
        followPath();

    }

    public void followPath()
    {
        Debug.Log("suu");
        if (camino.Count > 0)
        {
            Vector2Int vector = camino.Dequeue();
            Vector3 v = new Vector3(vector.x, vector.y, -0.5f);
            v = transform.localPosition - new Vector3(vector.x, vector.y, -0.5f);
            transform.localPosition += v;
            Invoke("followPath", mapa.getCostOfTile(vector.y, vector.x));
        }
    }

    
}
