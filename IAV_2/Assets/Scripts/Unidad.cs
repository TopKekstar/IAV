using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unidad : MonoBehaviour {
    Mapa mapa;
    Stack<Vector2Int> camino;
    GameObject _cross;
    Vector2Int pos;
    // Use this for initialization
    void Start () {
        mapa = transform.parent.gameObject.GetComponent<Mapa>();
        updatePos();
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
        mapa.setOccupied(pos.y, pos.y, true);
    }
    public void setPath(Stack<Vector2Int> c)
    {
        camino = c;
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
            moveTo(new Vector3(vector.x, vector.y, 0));
            if (camino.Count == 0)
            {
                Destroy(_cross);
            }
            else
            {
                Invoke("followPath", mapa.getCostOfTile(vector.y, vector.x));
            } 
        }
    }
    public void setCross(ref GameObject cross)
    {
        _cross = cross;
        _cross.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
    }

    
}
