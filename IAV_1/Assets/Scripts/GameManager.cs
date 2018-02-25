using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public Canvas _canvas;
	public GameObject prefabTile;
    public struct mapTile {
	    int _x; 
		int _y;
		public int _ID;
		public mapTile(int x, int y, int ID){
			_x = x;
			_y = y;
			_ID = ID;
		}
			
	}
	mapTile [,] map;

	// Use this for initialization
	void Start () {
		map = new mapTile[3, 3];
		initDefaultMap ();
		for (int i = 0; i < 3; i++) {
			for(int j = 0; j < 3; j++)
				Debug.Log(map[i,j]._ID);
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
    bool canMoveTile(int x_ini, int y_ini, int x_dest, int y_dest)
    {
        if (map[x_dest, y_dest]._ID != -1) return false;
        else return true;
    }


	mapTile [,] getMap(){
		return map;
	}
	void initDefaultMap(){
		for(int i = 0; i< 3; i++){
			for(int j = 0; j< 3 ; j++){
				if ((i * 3 + j) < 8)
					map [i, j] = new  mapTile ( i, j, i * 3 + j +1);
				else
					map [i, j] = new mapTile( i, j, -1 );
					}
			}
	}

    bool checkFinished()
    {
        bool ordered = true;
        for(int i = 0; i < 3 && ordered; i++)
        {
            for(int j = 0; j < 3&& ordered; j++)
            {
                if (i * 3 + j != 9)
                    ordered = (map[i, j]._ID != i * 3 + j);
                else ordered = (map[i, j]._ID == -1);
            }
        }
        
        return ordered;

    }
}
