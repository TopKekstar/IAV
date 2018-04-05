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

    }

    

    TILE_INFO[,] infoMapa;
    Vector2Int posCasa;
    Mapa mapa;
    Stack<Vector2Int> camino;
    Vector2Int from;
    GameObject _cross;
    Vector2Int pos;
    // Use this for initialization
    private void Start()
    {
        mapa = GameManager.instance.mapa;
        infoMapa = new TILE_INFO[mapa.altoMapa, mapa.anchoMapa];
        for (int i = 0; i < mapa.altoMapa; i++)
        {
            for (int j = 0; j < mapa.anchoMapa; j++)
            {
                TILE_INFO aux = infoMapa[i, j];
                aux.probCuerpo = aux.probPrecipicio = 0;
                aux._Contenido = Tile.T_Contenido.C_DESCONOCIDO;
                aux._Terreno = Tile.T_Terreno.T_DESCONOCIDO;
                aux.frontera = false;
            }
        }
        Color color = new Color(Random.Range(.0f, 1.0f), Random.Range(.0f, 1.0f), Random.Range(.0f, 1.0f));
        GetComponent<SpriteRenderer>().color = color;
        updatePos();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !GameManager.instance.editMode)
            GameManager.instance.setCurrentUnit(gameObject);
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
        infoMapa[pos.y, pos.x].frontera = false;

        for (int i = 0; i < 4; i++)
        {
            try
            {
                if(infoMapa[pos.y + GameManager.instance.directions[i].y, pos.x + GameManager.instance.directions[i].x]._Terreno == Tile.T_Terreno.T_DESCONOCIDO)
                    infoMapa[pos.y + GameManager.instance.directions[i].y, pos.x + GameManager.instance.directions[i].x].frontera = true;

            }
            catch (System.NullReferenceException)
            {


            }
        }
        if (infoMapa[pos.y, pos.x]._Terreno == Tile.T_Terreno.T_GRAVA)
        {
            int nVecinos=0;
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    if (infoMapa[pos.y + GameManager.instance.directions[i].y, pos.x + GameManager.instance.directions[i].x]._Terreno != Tile.T_Terreno.T_DESCONOCIDO) {
                        nVecinos++;
                    }
                    
                }
                catch (System.NullReferenceException)
                {

                    
                }
            }
            for (int i = 0; i < nVecinos; i++)
            {
                try
                {
                    infoMapa[pos.y + GameManager.instance.directions[i].y, pos.x + GameManager.instance.directions[i].x].probPrecipicio += 100/nVecinos;

                }
                catch (System.NullReferenceException)
                {


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

        mapa.setOccupied((int)transform.localPosition.y, (int)transform.localPosition.x, false);
        transform.Translate(v - transform.localPosition, transform);
        updatePos();
    }
    public void followPath()
    {
        if (camino.Count > 0)
        {
            Vector2Int vector = camino.Pop();
            if (vector != from && mapa.getOccupied(vector.y, vector.x))
            {
                while (camino.Count > 0)
                {
                    vector = camino.Pop();
                }
                Destroy(_cross);
                GameManager.instance.setCurrentUnit(gameObject);
                GameManager.instance.mueveUnidad(mapa.getTile(vector.y, vector.x).gameObject);
            }
            else
            {

                moveTo(new Vector3(vector.x, vector.y, 0));
                if (camino.Count == 0)
                {
                    Destroy(_cross);
                    GameManager.instance.camara.ResetTarget();
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
