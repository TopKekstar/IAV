using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapa : MonoBehaviour {
    public TextAsset fichero;
	public GameObject prefabTile;
	public int anchoMapa;
	public int altoMapa;
	Tile[,] mapaTiles;
	// Use this for initialization
	void Start () {
		mapaTiles = new Tile[altoMapa,anchoMapa];
		for (int i = 0; i < altoMapa; i++) {
			for (int j = 0; j < anchoMapa; j++) {
				GameObject auxT = Instantiate (prefabTile, gameObject.transform);
				mapaTiles [i, j] = auxT.GetComponent<Tile> ();
            }
		}
        Invoke("iniciarMapa", 0.16f);
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

                mapaTiles[i, j].ini(Tile.E_Tile.CESPED, j, i);

            }
        }

    }

    public bool[,] getMarcados()
    {
        bool[,] marcados = new bool[altoMapa,anchoMapa];
        for (int i = 0; i < altoMapa; i++)
        {
            for (int j = 0; j < anchoMapa; j++)
            {
                marcados[i,j] = mapaTiles[i, j].isOccupied();
            }
        }
        return marcados;
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

    public int getCostOfTile(int i,int j)
    {
        return mapaTiles[i, j].getCost();
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
