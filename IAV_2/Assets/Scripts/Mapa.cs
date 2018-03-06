using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapa : MonoBehaviour {
	public GameObject prefabTile;
	int anchoCasilla;
	int altoCasilla;
	Tile[,] mapaTiles;
	// Use this for initialization
	void Start () {
		mapaTiles = new Tile[10,10];
		anchoCasilla = 32;
		altoCasilla = 32;
		for (int i = 0; i < 10; i++) {
			for (int j = 0; j < 10; j++) {
				GameObject auxT = Instantiate (prefabTile, gameObject.transform);
				mapaTiles [i, j] = auxT.GetComponent<Tile> ();
				mapaTiles [i, j].ini (0, 1);
				mapaTiles [i, j].setPos (anchoCasilla * j, altoCasilla * i);
				Debug.Log ("kek");
				int k = 0;
				k++;
			}
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
