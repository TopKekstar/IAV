using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour  {
	public int nTile;
	public GameManager _gm;
	public int index;
	uint x, y;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void init(int n,int ind, ref GameManager gm){
		nTile = n;
		_gm = gm;
		index = ind;
		if (n == 0) {
			gameObject.SetActive (false);
		}
		GetComponentInChildren<UnityEngine.UI.Text>().text = nTile.ToString ();
	}

	void OnMouseDown(){
		Debug.Log ("suu");
	}
}
