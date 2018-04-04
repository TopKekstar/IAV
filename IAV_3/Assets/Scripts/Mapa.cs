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
        nulo = Instantiate(prefabTile, new Vector3(0,0,-20),Quaternion.identity,gameObject.transform).GetComponent<Tile>();
        
        iniciarMapa();
	}
	
	// Update is called once per frame
	void Update () {
    }
    public void setOccupied(int i,int j,bool value)
    {
        mapaTiles[i, j].setOccupied(value);
    }
    public void iniciarMapa()
    {
        for (int i = 0; i < altoMapa; i++)
        {
            for (int j = 0; j < anchoMapa; j++)
            {

                mapaTiles[i, j].ini(Tile.T_Terreno.T_CESPED, j, i);

            }
        }   

    }

    public void iniciarAleatorio()
    {
        for (int i = 0; i < altoMapa; i++)
        {
            for (int j = 0; j < anchoMapa; j++)
            {
				
            }
        }

    }
    public bool getOccupied(int i, int j)
    {
        return mapaTiles[i, j].getOccupied();
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

	public Tile getTile(int i,int j){
        try
        {
		    return mapaTiles [i, j];

        }
        catch (System.Exception)
        {
            return nulo;
            throw;
        }
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
