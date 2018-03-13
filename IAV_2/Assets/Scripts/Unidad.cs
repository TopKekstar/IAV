using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unidad : MonoBehaviour {
    Mapa mapa;
    Stack<Vector2Int> camino;
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

    public void setPath(Stack<Vector2Int> c)
    {
        camino = c;
        followPath();

    }

    public void followPath()
    {
        if (camino.Count > 0)
        {
            Vector2Int vector = camino.Pop();
            
            Vector3 v = new Vector3(vector.x, vector.y, 0);

            
            

            transform.Translate(v-transform.localPosition,transform);
            Invoke("followPath", mapa.getCostOfTile(vector.y, vector.x));
        }
    }

    
}
