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
                mapaTiles[i, j].ini(Tile.T_Terreno.T_CESPED, j, i);
            }
        }
        mapaTiles[0,0].SetContenido(Tile.T_Contenido.C_CASA);
        int y = Random.Range(2, altoMapa);
        int x = Random.Range(2, anchoMapa);
        mapaTiles[y, x].SetContenido(Tile.T_Contenido.C_CUERPO);

        int xCuchillo = Random.Range((x-2<0)?0:x-2, (x + 2 >= anchoMapa) ?anchoMapa:x+2);
        int yCuchillo = Random.Range((y - 2 < 0) ? 0 : y - 2, (y + 2 >= altoMapa) ? altoMapa : x + 2);
        mapaTiles[yCuchillo, xCuchillo].SetContenido(Tile.T_Contenido.C_CUCHILLO);
        int nPrecipicios = 3;
        for (int i = 0; i < nPrecipicios; i++)
        {
            y = Random.Range(1, altoMapa);
            x = Random.Range(1, anchoMapa);
            if (mapaTiles[y, x].GetContenido()!= Tile.T_Contenido.C_CUERPO && mapaTiles[y, x].GetContenido() != Tile.T_Contenido.C_CUCHILLO)
                mapaTiles[y, x].SetTerreno(Tile.T_Terreno.T_PRECIPICIO);
        }
        Instantiate(GameManager.instance.prefabUnidad, mapaTiles[0, 0].transform.position+Vector3.back*2, mapaTiles[0, 0].transform.rotation, transform);

        


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
