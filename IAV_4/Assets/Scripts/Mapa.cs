using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapa : MonoBehaviour {
    public TextAsset fichero;
	public GameObject prefabTile;
	public int anchoMapa;
	public int altoMapa;
    public int nRocasMax;
    bool firstIni;
	Tile[,] mapaTiles;
    Tile nulo;
	// Use this for initialization
	void Start () {
		mapaTiles = new Tile[altoMapa,anchoMapa];
		for (int i = 0; i < altoMapa; i++) {
			for (int j = 0; j < anchoMapa; j++) {
				GameObject auxT = Instantiate (prefabTile, gameObject.transform);
				mapaTiles [i, j] = auxT.GetComponent<Tile> ();
            }
		}
        nulo = Instantiate(prefabTile, new Vector3(0,-100,-20),Quaternion.identity,gameObject.transform).GetComponent<Tile>();
        
        iniciarMapa();
	}
	
	// Update is called once per frame
	void Update () {
    }
   
    public void iniciarMapa()
    {
        for (int i = 0; i < altoMapa; i++)
        {
            for (int j = 0; j < anchoMapa; j++)
            {
                mapaTiles[i, j].ini(j, i);
            }
        }
        
        
        


    }

    List<Tile> GetVecinas(int iY,int jX)
    {
        List<Tile> vecinas = new List<Tile>();
        for (int i = 0; i < 4; i++)
        {
            int y = iY + PathFinder.directions[i].y;
            int x = jX + PathFinder.directions[i].x;
            if(x >= anchoMapa || x < 0||y >= altoMapa || y < 0)
            {

            }
            else
                vecinas.Add(getTile(y, x));
            

        }
        return vecinas;
    }





    public int[,] getDistTo(int origenI, int origenJ)
    {
        int[,] distTo = new int[altoMapa, anchoMapa];
        for (int i = 0; i < altoMapa; i++)
        {
            for (int j = 0; j < anchoMapa; j++)
            {
                distTo[i, j] = (i==origenI&&j==origenJ)? 0 :int.MaxValue  ;
            }
        }
        return distTo;
    }

	public Tile getTile(int i,int j){
        try
        {
		    return mapaTiles [i, j];

        }
        catch (System.Exception)
        {
            return nulo;
        }
	}

   
    public int getAlto()
    {
        return altoMapa;
    }
    public int getAncho()
    {
        return anchoMapa;
    }
}
