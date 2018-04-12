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
        List<Tile> list = GetVecinas(y, x);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].SetContenido(Tile.T_Contenido.C_SANGRE);

        }
        Tile tile = list[Random.Range(0, list.Count)];
        list = GetVecinas(tile.posY, tile.posX);
        list.Remove(getTile(y, x));
        tile = list[Random.Range(0, list.Count)];
        tile.SetContenido(Tile.T_Contenido.C_CUCHILLO);
        
        
        int nPrecipicios = 3;
        for (int i = 0; i < nPrecipicios; i++)
        {
            y = Random.Range(1, altoMapa);
            x = Random.Range(1, anchoMapa);
            if (mapaTiles[y, x].GetContenido() != Tile.T_Contenido.C_CUERPO && mapaTiles[y, x].GetContenido() != Tile.T_Contenido.C_CUCHILLO
                && mapaTiles[y, x].GetContenido() != Tile.T_Contenido.C_SANGRE)
            {
                mapaTiles[y, x].SetTerreno(Tile.T_Terreno.T_PRECIPICIO);
                list = GetVecinas(y, x);
                for (int j = 0; j < list.Count; j++)
                {
                    list[j].SetTerreno(Tile.T_Terreno.T_GRAVA);

                }
            }
        }
        GameObject agente = Instantiate(GameManager.instance.prefabUnidad, mapaTiles[0, 0].transform.position+Vector3.back*2, mapaTiles[0, 0].transform.rotation, transform);
        GameManager.instance.setCurrentUnit(agente);
        


    }

    List<Tile> GetVecinas(int iY,int jX)
    {
        List<Tile> vecinas = new List<Tile>();
        for (int i = 0; i < 4; i++)
        {
            int y = iY + GameManager.instance.directions[i].y;
            int x = jX + GameManager.instance.directions[i].x;
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
