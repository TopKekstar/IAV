using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unidad : MonoBehaviour {
    Mapa mapa;
    Stack<Vector2Int> camino;
	Vector2Int from;
    GameObject _cross;
    Vector2Int pos;
    // Use this for initialization
    void Start () {
        mapa = transform.parent.gameObject.GetComponent<Mapa>();
        Color color = new Color(Random.Range(.0f, 1.0f), Random.Range(.0f, 1.0f), Random.Range(.0f, 1.0f));
        GetComponent<SpriteRenderer>().color = color;
		updatePos ();
    }
	
	// Update is called once per frame
	void Update () {

    }

    private void OnMouseDown()
    {
        GameManager.instance.setCurrentUnit(gameObject);
    }
    void updatePos()
    {
        pos.x = (int)transform.localPosition.x;
        pos.y = (int)transform.localPosition.y;
        mapa.setOccupied(pos.y, pos.x, true);
    }
    public void setPath(Stack<Vector2Int> c)
    {
        camino = c;
		from = camino.Peek ();
        followPath();

    }
    void moveTo(Vector3 v)
    {

        mapa.setOccupied((int)transform.localPosition.y, (int)transform.localPosition.x, false);
        transform.Translate(v - transform.localPosition, transform);
        updatePos();
    }
    public void followPath()
    {
        if (camino.Count > 0)
        {
            Vector2Int vector = camino.Pop();
			if (vector!= from&&mapa.getOccupied (vector.y, vector.x)) {
				while (camino.Count > 0) {
					vector = camino.Pop();
				}
				Destroy (_cross);
				GameManager.instance.setCurrentUnit (gameObject);
				GameManager.instance.mueveUnidad(mapa.getTile(vector.y,vector.x).gameObject);
			} 
			else
			{
				
				moveTo (new Vector3 (vector.x, vector.y, 0));
				if (camino.Count == 0) {
					Destroy (_cross);
					GameManager.instance.camara.ResetTarget ();
				} else {
					Invoke ("followPath", mapa.getCostOfTile (vector.y, vector.x));
				}
			}
        }
    }
    public void setCross(ref GameObject cross)
    {
        _cross = cross;
        _cross.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
    }

    
}
