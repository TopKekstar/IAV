
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
    public bool descubierta;
    bool firstIni = false;

	// Use this for initialization
	void Start () {
        

    }
    
    public void SetDescubierta(bool desc)
    {
        descubierta = desc;
        updateTile();
    }

    public bool GetOcupada()
    {
        return descubierta;
    }

    public void ini(T_Terreno terreno, int x, int y)
    {
        descubierta = false;
        if(_contenido != T_Contenido.C_NADA)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
        _contenido = T_Contenido.C_NADA;
        posX = x;
        posY = y;
        _terreno = terreno;
        updateTile();
        if(!firstIni)
            GetComponent<Transform>().transform.Translate(x, y, 0);
        firstIni = true;

    }




    void updateTile()
    {
        switch (_terreno)
        {
            case T_Terreno.T_CESPED:
                GetComponent<SpriteRenderer>().color = Color.green;
                coste = 1;
                break;
            case T_Terreno.T_GRAVA:
                GetComponent<SpriteRenderer>().color = Color.grey;
                coste = 2;
                break;
            case T_Terreno.T_PRECIPICIO:
                break;
            case T_Terreno.T_DESCONOCIDO:
                break;
            default:
                break;
        }
        if (!descubierta)
            GetComponent<SpriteRenderer>().color = Color.black;

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
        }
        updateTile();
    }
    
    public void SetContenido(T_Contenido contenido)
    {
        _contenido = contenido;
        switch (_contenido)
        {
            case T_Contenido.C_NADA:
                break;
            case T_Contenido.C_SANGRE:
                Instantiate(GameManager.instance.prefabSangre, transform);
                break;
            case T_Contenido.C_CUCHILLO:
                Instantiate(GameManager.instance.prefabCuchillo, transform);
                break;
            case T_Contenido.C_CUERPO:
                Instantiate(GameManager.instance.prefabCuerpo, transform);
                break;
            case T_Contenido.C_CASA:
                Instantiate(GameManager.instance.prefabCasa, transform);
                break;
            case T_Contenido.C_DESCONOCIDO:
                break;
            default:
                break;
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
    public int getCost() {
        return coste;
    }
}
