﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
	
	int nTile;
	int coste;
	public int posX;
	public int posY;
    bool ocupada;
	// Use this for initialization
	void Start () {
        ocupada = false;
	}
	public void ini(int nTile_,int coste_ , int x, int y){
		nTile = nTile_;
		coste = coste_;
        posX = x;
        posY = y;
		switch (nTile) {
		case 0:
			GetComponent<SpriteRenderer> ().color = Color.green;
			break;
		case 1:
			GetComponent<SpriteRenderer> ().color = Color.blue;
			break;
		case 2:
			GetComponent<SpriteRenderer> ().color = Color.gray;
                ocupada = true;
			break;
		default:
			break;
		}

       Transform t =  GetComponent<Transform>();
        t.transform.Translate(x, y, 0);

    }
	// Update is called once per frame
	void Update ()
    {
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
            Debug.Log("Pressed left click.");
        if (Input.GetMouseButtonDown(1))
            GameManager.instance.mueveUnidad(transform.localPosition);            
    }
    public bool isOccupied()
    {
        return ocupada;
    }

    public int getCost() {
        return coste;
    }
}