using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public Canvas _canvas;
	public GameObject prefabTile;
    int ultimoMov;
	public int dimensiones;

	// Use this for initialization
	void Start () {
		
		for (int i = 0; i < dimensiones; i++) {
			for(int j = 0; j < dimensiones; j++)
            {
				GameObject t = Instantiate(prefabTile);
				t.transform.SetParent (_canvas.transform);
				int n = i * dimensiones + j + 1;
				if(n==dimensiones*dimensiones){
					t.GetComponent<Tile> ().init (0, n-1,ref this);
				}else{
					t.GetComponent<Tile> ().init (n, n-1,ref this);
										
				}
            }
		}


        /*for (int i = 0; i < 9; i++) {
			GameObject g = Instantiate (prefabTile);
			g.name = (i + 1).ToString ();
			g.transform.SetParent(_canvas.transform);
		}*/
    }


    // Update is called once per frame
    void Update () {
		
	}
	void updateScene(){

	
	}
    void BFS()
    {

    }
    
    
}
