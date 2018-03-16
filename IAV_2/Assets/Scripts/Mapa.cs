using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapa : MonoBehaviour {
    public TextAsset fichero;
	public GameObject prefabTile;
	public int anchoMapa;
	public int altoMapa;
    public int nRocasMax;
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
		iniciarAleatorio();
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

                mapaTiles[i, j].ini(Tile.E_Tile.CESPED, j, i);

            }
        }

    }

    public void iniciarAleatorio()
    {
        int nMuros = 0;
        for (int i = 0; i < altoMapa; i++)
        {
            for (int j = 0; j < anchoMapa; j++)
            {
				int aletario =  Random.Range(-3, 3);
				if (aletario <= 0)
					aletario = 0;
				Tile.E_Tile tipoTile = (Tile.E_Tile)aletario;
                if(tipoTile == Tile.E_Tile.MURO)
                {
                    if (nMuros < nRocasMax)
                    {
                        nMuros++;

                    }
                    else
                    {
                        tipoTile -= Random.Range(1, 2);
                    }
                }
                mapaTiles[i, j].ini(tipoTile, j, i);

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
		return mapaTiles [i, j];
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
