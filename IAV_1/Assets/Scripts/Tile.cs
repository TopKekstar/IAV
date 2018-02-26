using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour  {
	public int nTile;
	public int index;
	uint x, y;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void init(int n,int ind){
		nTile = n;
		index = ind;
		if (n == 0) {
            GetComponent<UnityEngine.UI.Image>().color = new Color(0,0,0,0);
		}
		GetComponentInChildren<UnityEngine.UI.Text>().text = nTile.ToString ();
	}

   public void OnClick()
    {
        int cambio;
        if( GameManager.instance.canMove(index, out cambio))
        {
            GameManager.instance.move(index, cambio);
        }
    }

    
}
