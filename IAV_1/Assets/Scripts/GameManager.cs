using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public Canvas _canvas;
	public GameObject prefabTile;
    int ultimoMov;
    public struct mapTile {
		public int _ID;
		public mapTile(int ID){
			_ID = ID;
		}
			
	}
	mapTile [,] map;
    void swap(int x1, int y1,int x2, int y2)
    {
        mapTile temp = map[x1, y1];
        map[x1, y1] = map[x2, y2];
        map[x2, y2] = temp;
      

    }
    Vector2Int obtenerMoviemiento(int piece, ref Vector2Int  hueco)
    {
        Vector2Int m = new Vector2Int(-1,-1);
        int line = hueco.y;
        int column = hueco.x;
        if ( line > 0 && piece == map[column, line - 1]._ID)
        {
            m.Set(column, line - 1);
        }
        else if (line < 3-1 && piece == map[column, line + 1]._ID)
        {
            m.Set(column, line + 1);
        }
        else if (column > 0 && piece == map[column - 1,line]._ID)
        {
            m.Set(column-1, line);
        }
        else if (column < 3-1  && piece == map[column + 1,line]._ID)
        {
            m.Set(column+1, line);
        }
        return m;
    }
    List<int> getMovimientosDisponibles()
    {
        List<int> Movs = new List<int>(4);
        Vector2Int hueco = encontrarHueco();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2Int m = obtenerMoviemiento(map[i, j]._ID,ref hueco);
                if (m.x != -1)
                {
                    Movs.Add(map[i, j]._ID);
                }
            }
        }
        
        return Movs;
    }

    void move(ref Vector2Int m)
    {
        
    }

	// Use this for initialization
	void Start () {
		map = new mapTile[3, 3];
        ultimoMov = -2;


        initDefaultMap ();
		for (int i = 0; i < 3; i++) {
			for(int j = 0; j < 3; j++)
            {
				Debug.Log(map[i,j]._ID);
            }
		}


        /*for (int i = 0; i < 9; i++) {
			GameObject g = Instantiate (prefabTile);
			g.name = (i + 1).ToString ();
			g.transform.SetParent(_canvas.transform);
		}*/
    }
    Vector2Int encontrarHueco()
    {
        Vector2Int suu = new Vector2Int(0,0);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {

                if (map[i, j]._ID == -1)
                    suu.Set(i, j);
                    
            }
        }
        return suu;
    }
    // Update is called once per frame
    void Update () {
		
	}
	void updateScene(){

	
	}
    void BFS()
    {

    }
    
    List<mapTile[,]> visita()
    {
        List<mapTile[,]> hijos = new List<mapTile[,]>();
        List<int> movs = getMovimientosDisponibles();
        for(int i = 0; i < movs.Count; i++)
        {
            int pieza = movs[i];
            if(pieza == ultimoMov)
            {

            }
        }

        return hijos;

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
					map [i, j] = new  mapTile ( i * 3 + j +1);
				else
					map [i, j] = new mapTile(  -1 );
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
