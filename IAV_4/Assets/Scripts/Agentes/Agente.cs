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


    
    protected Agent_Status status;
    protected List<Vector2Int> frontera;
    protected TILE_INFO[,] infoMapa;
    protected Vector2Int posCasa;
    protected Mapa mapa;
    protected Stack<Vector2Int> camino;
    protected Vector2Int from;
    protected GameObject _cross;
    protected Vector2Int pos;
    protected bool cuchilloFound;
    protected bool fiambreFound;
    protected bool casoResuelto;
    int pasos;

    // Use this for initialization
    protected void Start()
    {
        pasos = 0;
        status = Agent_Status.SLEEPING;
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
    protected void Update()
    {
        if (status == Agent_Status.SLEEPING)
        {
            status = Agent_Status.EXPLORING;
            GameManager.instance.ActualizarInfo("Explorando");
            Invoke("Explore", 0.5f);
        }
    }

    protected int distanciaJugador(Vector2Int vector2Int)
    {
        return System.Math.Abs(vector2Int.x - pos.x) + System.Math.Abs(vector2Int.y - pos.y);
    }

    protected virtual void Explore()
    {
        if (!(cuchilloFound && fiambreFound))
        {
            Vector2Int bestOption = new Vector2Int(-1, -1);
            for (int i = 0; i < frontera.Count; i++)
            {
                if (bestOption.x == -1)
                {
                    bestOption = frontera[i];
                }
                else if (infoMapa[bestOption.y, bestOption.x].getRisk() == infoMapa[frontera[i].y, frontera[i].x].getRisk())
                {
                    if ((bestOption.x == 0 && frontera[i].x != 0) || (bestOption.y == 0 && frontera[i].y != 0))
                        bestOption = frontera[i];
                    if ((bestOption.x == mapa.anchoMapa - 1 && frontera[i].x != mapa.anchoMapa - 1) || (bestOption.y == mapa.altoMapa - 1 && frontera[i].y != mapa.altoMapa - 1))
                        bestOption = frontera[i];
                    if (distanciaJugador(bestOption) > distanciaJugador(frontera[i]))
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
            GetComponent<PathFinder>().CalculatePath(posCasa);
            casoResuelto = true;
        }
    }


    protected void OnDestroy()
    {
        Destroy(_cross);
    }
    protected void updatePos()
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
        if(cuchilloFound && fiambreFound && casoResuelto)
        {
            GameManager.instance.ActualizarInfo("Volviendo a casa");
        }
        else if (cuchilloFound)
        {
            GameManager.instance.ActualizarInfo("Encontrado Cuchillo");
        }
        else if (fiambreFound)
        {
            GameManager.instance.ActualizarInfo("Cuerpo Encontrado");
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
                    if (infoMapa[vecino.y, vecino.x]._Contenido == Tile.T_Contenido.C_DESCONOCIDO)
                        infoMapa[vecino.y, vecino.x].probCuerpo -= 500 / nVecinos;
                    if (fiambreFound)
                    {
                        infoMapa[vecino.y, vecino.x].probCuerpo = 0;

                    }
                }
                else if (infoMapa[pos.y, pos.x]._Contenido == Tile.T_Contenido.C_CUCHILLO)
                {
                    if (infoMapa[vecino.y, vecino.x]._Contenido == Tile.T_Contenido.C_DESCONOCIDO )
                        infoMapa[vecino.y, vecino.x].probCuerpo -= 250 / nVecinos;
                    if (fiambreFound)
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
                    

                }
            }
        }
        if (infoMapa[pos.y, pos.x]._Terreno == Tile.T_Terreno.T_PRECIPICIO)
        {
            GameManager.instance.derrota();
            casoResuelto = cuchilloFound= fiambreFound= true;
        }
        
    }
    public void setPath(Stack<Vector2Int> c)
    {
		
        camino = c;
        from = camino.Peek();
        followPath();

    }
    protected void moveTo(Vector3 v)
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
                    pasos++;
                    GameManager.instance.updateDiagnostico(pasos);
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
