
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
	
	int nTile;
	int coste;
	public int posX;
	public int posY;
	bool movida;
	// Use this for initialization
	void Start () {
		movida = false;
		
	}
	public void ini(int nTile_,int coste_ ){
		nTile = nTile_;
		coste = coste_;
		switch (nTile) {
		case 0:
			GetComponent<UnityEngine.UI.Image> ().color = Color.green;
			break;
		case 1:
			GetComponent<UnityEngine.UI.Image> ().color = Color.blue;
			break;
		case 2:
			GetComponent<UnityEngine.UI.Image> ().color = Color.gray;
			break;
		default:
			break;
		}
		

		int k = 0;
		k++;
	}
	public void setPos(int pX,int pY){
		posX = pX;
		posY = pY;
		gameObject.transform.localPosition.Set ((float)pX,(float) pY, 0);
	}
	// Update is called once per frame
	void Update () {
		if(movida)
		
	}
}
