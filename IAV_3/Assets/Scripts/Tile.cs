
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    public enum T_Terreno:int
    {
        T_CESPED = 0, T_GRAVA = 2, T_PRECIPICIO = 1, T_DESCONOCIDO = 3,
    }

    public enum T_Contenido : int
    {
        C_NADA = 0, C_SANGRE = 1, C_CUCHILLO = 2,C_CUERPO = 3, C_CASA = 4, C_DESCONOCIDO = 5,
    }

    T_Terreno _terreno;
    T_Contenido _contenido;
	
	int coste;
	public int posX;
	public int posY;
    public bool ocupada;
    bool firstIni = false;

	// Use this for initialization
	void Start () {
        

    }


    public void ini(T_Terreno terreno, int x, int y)
    {
        _contenido = T_Contenido.C_NADA;
        posX = x;
        posY = y;
        _terreno = terreno;
        updateTile();
        if(!firstIni)
            GetComponent<Transform>().transform.Translate(x, y, 0);
        firstIni = true;

    }

    public bool getOccupied()
    {
        return ocupada;
    }

    public void setOccupied(bool value)
    {
        ocupada = value;
    }

    void updateTile()
    {
        switch (_terreno)
        {
            case T_Terreno.T_CESPED:
                GetComponent<SpriteRenderer>().color = Color.green;
                coste = 1;
                ocupada = false;
                break;
            case T_Terreno.T_GRAVA:
                GetComponent<SpriteRenderer>().color = Color.grey;
                coste = 2;
                ocupada = false;
                break;
            case T_Terreno.T_PRECIPICIO:
                GetComponent<SpriteRenderer>().color = Color.black;
                ocupada = true;
                break;
            case T_Terreno.T_DESCONOCIDO:
                break;
            default:
                break;
        }

    }
	// Update is called once per frame
	void Update ()
    {
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)&&GameManager.instance.editMode)
            CambiaTerreno();
        if (Input.GetMouseButtonDown(1))
            GameManager.instance.mueveUnidad(gameObject);            
    }

    private void CambiaTerreno()
	{
       
    }

    public void SetTerreno(T_Terreno terreno)
    {
        if (_terreno != T_Terreno.T_PRECIPICIO)
        {
            _terreno = terreno;
            if (_terreno == T_Terreno.T_PRECIPICIO)
            {

                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        
                        GameManager.instance.mapa.getTile(posY + GameManager.instance.directions[i].y, posX + GameManager.instance.directions[i].x).SetTerreno(T_Terreno.T_GRAVA) ;
                    }
                    catch (System.NullReferenceException )
                    {

                    }
                }
            }
        }
    }

    public void SetContenido(T_Contenido contenido)
    {
        _contenido = contenido;
        if (_contenido == T_Contenido.C_CUERPO)
        {

            for (int i = 0; i < 4; i++)
            {
                try
                { 
                    GameManager.instance.mapa.getTile(posY + GameManager.instance.directions[i].y, posX + GameManager.instance.directions[i].x).SetContenido(T_Contenido.C_SANGRE);
                }
                catch (System.NullReferenceException)
                {

                }
            }
        }
        
    }

    public T_Terreno GetTerreno()
    {
        return _terreno;
    }

    public T_Contenido GetContenido()
    {
        return _contenido;
    }
    public bool isOccupied()
    {
        return ocupada;
    }

    public int getCost() {
        return coste;
    }
}
