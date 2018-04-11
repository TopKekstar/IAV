using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agente : MonoBehaviour {
    public struct TILE_INFO
    {
        public int probPrecipicio;
        public int probCuerpo;
        public Tile.T_Contenido _Contenido;
        public Tile.T_Terreno _Terreno;
        public bool frontera;
        public bool noPrecipicio;
        public bool noCuerpo;
        public int getRisk()
        {
            return probCuerpo + probPrecipicio;
        }

    }

    public enum Agent_Status
    {
        SLEEPING, EXPLORING, GOING_HOME
    }


    List<Vector2Int> frontera;
    TILE_INFO[,] infoMapa;
    Vector2Int posCasa;
    Mapa mapa;
    Stack<Vector2Int> camino;
    Vector2Int from;
    GameObject _cross;
    Vector2Int pos;
	bool cuchilloFound;
	bool fiambreFound;
    bool casoResuelto;

    // Use this for initialization
    private void Start()
    {
		cuchilloFound = false;
		fiambreFound = false;
        casoResuelto = false;
        mapa = GameManager.instance.mapa;
        infoMapa = new TILE_INFO[mapa.altoMapa, mapa.anchoMapa];
        frontera = new List<Vector2Int>();
		posCasa = new Vector2Int (0, 0);
        for (int i = 0; i < mapa.altoMapa; i++)
        {
            for (int j = 0; j < mapa.anchoMapa; j++)
            {
				TILE_INFO aux;
                aux.probCuerpo = aux.probPrecipicio = 0;
                aux._Contenido = Tile.T_Contenido.C_DESCONOCIDO;
                aux._Terreno = Tile.T_Terreno.T_DESCONOCIDO;
                aux.frontera = false;
                aux.noPrecipicio = false;
                aux.noCuerpo = false;
				infoMapa [i, j] = aux;
			}
        }
        Color color = new Color(Random.Range(.0f, 1.0f), Random.Range(.0f, 1.0f), Random.Range(.0f, 1.0f));
        GetComponent<SpriteRenderer>().color = color;
        updatePos();
        mapa.getTile(pos.y, pos.x).SetDescubierta(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void Explore()
    {
        if (!(cuchilloFound && fiambreFound))
        {
            Vector2Int bestOption = new Vector2Int(-1, -1);
            for (int i = 0; i < frontera.Count; i++)
            {
                if (bestOption.x == -1 )
                {
                    bestOption = frontera[i];
                }
                else if ( infoMapa[bestOption.y, bestOption.x].getRisk() == infoMapa[frontera[i].y, frontera[i].x].getRisk())
                {
                    if(Random.Range(0,10)>6)
                        bestOption = frontera[i];

                }
                else if (infoMapa[bestOption.y, bestOption.x].getRisk() > infoMapa[frontera[i].y, frontera[i].x].getRisk())
                {
                    bestOption = frontera[i];

                }
            }
            GetComponent<PathFinder>().CalculatePath(bestOption);
        }
        else if (!casoResuelto)
        {
            Debug.Log("vuelta a casa");
            GetComponent<PathFinder>().CalculatePath(posCasa);
            casoResuelto = true;
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !GameManager.instance.editMode)
            Explore();
        if (Input.GetMouseButtonDown(1) && GameManager.instance.editMode)
        {

            GameManager.instance.borraUnidad(gameObject);

        }
    }
    private void OnDestroy()
    {
        Destroy(_cross);
    }
    void updatePos()
    {
        pos.x = (int)transform.localPosition.x;
        pos.y = (int)transform.localPosition.y;
        infoMapa[pos.y, pos.x]._Terreno = mapa.getTile(pos.y, pos.x).GetTerreno();
        infoMapa[pos.y, pos.x]._Contenido = mapa.getTile(pos.y, pos.x).GetContenido();
        infoMapa[pos.y, pos.x].frontera = false;
        infoMapa[pos.y, pos.x].noPrecipicio = true;
        infoMapa[pos.y, pos.x].probPrecipicio = 0;
        infoMapa[pos.y, pos.x].probCuerpo = 0;
        frontera.Remove(pos);


        switch (infoMapa[pos.y, pos.x]._Contenido)
        {
            case Tile.T_Contenido.C_CUCHILLO:
                cuchilloFound = true;
                break;
            case Tile.T_Contenido.C_CUERPO:
                fiambreFound = true;
                break;
            case Tile.T_Contenido.C_CASA:

                break;
            default:
                break;
        }

        int nVecinos =0;
		for (int i = 0; i < 4; i++) {
			Vector2Int vecino = pos + GameManager.instance.directions [i];
			if (vecino.x >= 0 && vecino.x < mapa.anchoMapa && vecino.y >= 0 && vecino.y < mapa.altoMapa) {
				if (infoMapa [vecino.y, vecino.x]._Terreno == Tile.T_Terreno.T_DESCONOCIDO) {
					infoMapa [vecino.y, vecino.x].frontera = true;
					nVecinos++;
                    if (!frontera.Contains(vecino))
                    {
                        Debug.Log("metio");
                        frontera.Add(vecino);
                    }
				}
			}            
		}

        for (int i = 0; i < 4; i++)
        {
            Vector2Int vecino = pos + GameManager.instance.directions[i];
            if (vecino.x >= 0 && vecino.x < mapa.anchoMapa && vecino.y >= 0 && vecino.y < mapa.altoMapa)
            {
                if (infoMapa[pos.y, pos.x]._Terreno == Tile.T_Terreno.T_GRAVA)
                {
                    if (infoMapa[vecino.y, vecino.x]._Terreno == Tile.T_Terreno.T_DESCONOCIDO && !infoMapa[vecino.y, vecino.x].noPrecipicio)
                    {
                        infoMapa[vecino.y, vecino.x].probPrecipicio += 1000 / nVecinos;
                    }
                }
                else if (infoMapa[pos.y, pos.x]._Terreno == Tile.T_Terreno.T_CESPED)
                {
                    infoMapa[vecino.y, vecino.x].noPrecipicio = true;
                    infoMapa[vecino.y, vecino.x].probPrecipicio = 0;

                }

                if (infoMapa[pos.y, pos.x]._Contenido == Tile.T_Contenido.C_SANGRE)
                {
                    if (infoMapa[vecino.y, vecino.x]._Contenido == Tile.T_Contenido.C_DESCONOCIDO&&!fiambreFound)
                        infoMapa[vecino.y, vecino.x].probCuerpo -= 500 / nVecinos;
                    else if (fiambreFound)
                    {
                        infoMapa[vecino.y, vecino.x].probCuerpo = 0;

                    }
                }
                else  if (infoMapa[pos.y, pos.x]._Contenido == Tile.T_Contenido.C_CUERPO)
                {
                    if (fiambreFound)
                    {
                        infoMapa[vecino.y, vecino.x].probCuerpo = 0;

                    }
                }
                else if(infoMapa[pos.y, pos.x]._Contenido == Tile.T_Contenido.C_NADA)
                {
                    infoMapa[vecino.y, vecino.x].probCuerpo = 0;
                    infoMapa[vecino.y, vecino.x].noCuerpo = true;

                }
            }
        }
        
    }
    public void setPath(Stack<Vector2Int> c)
    {
		
        camino = c;
        from = camino.Peek();
        followPath();

    }
    void moveTo(Vector3 v)
    {
        transform.Translate(v - transform.localPosition, transform);
        updatePos();
    }
    public void followPath()
    {
        if (camino.Count > 0)
        {
            Vector2Int vector = camino.Pop();


            {

                moveTo(new Vector3(vector.x, vector.y, 0));
                if (camino.Count == 0)
                {
                    Destroy(_cross);
                    GameManager.instance.camara.ResetTarget();
                    Explore();
                    mapa.getTile(pos.y, pos.x).SetDescubierta(true);
                }
                else
                {
                    Invoke("followPath", 0.5f);
                }
            }

        }
    }
    public void setCross(ref GameObject cross)
    {
        _cross = cross;
        _cross.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
    }

    public TILE_INFO GetTILE_INFO(int i, int j)
    {
        return infoMapa[i, j];
    }
}
