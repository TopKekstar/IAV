
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    public enum E_Tile
    {
        CESPED = 0, RIO = 1, MURO = 2
    }

    E_Tile tipo;
	
	int coste;
	public int posX;
	public int posY;
    bool ocupada;
	// Use this for initialization
	void Start () {
        ocupada = false;
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("suu");
        ocupada = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ocupada = false;
    }
    public void ini(E_Tile tipo_, int x, int y){



        tipo = tipo_;
        updateTile();
       Transform t =  GetComponent<Transform>();
        t.transform.Translate(x, y, 0);

    }

    public void setOccupied(bool value)
    {
        ocupada = value;
    }

    void updateTile()
    {
        switch (tipo)
        {
            case E_Tile.CESPED:
                GetComponent<SpriteRenderer>().color = Color.green;
                coste = 1;
                ocupada = false;
                break;
            case E_Tile.RIO:
                GetComponent<SpriteRenderer>().color = Color.blue;
                coste = 2;
                ocupada = false;
                break;
            case E_Tile.MURO:
                GetComponent<SpriteRenderer>().color = Color.gray;
                ocupada = true;
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
        if (Input.GetMouseButtonDown(0))
            changeTile();
        if (Input.GetMouseButtonDown(1))
            GameManager.instance.mueveUnidad(gameObject);            
    }

    private void changeTile()
    {
        if (!ocupada || tipo != E_Tile.MURO)
        {
            switch (tipo)
            {
                case E_Tile.CESPED:
                    tipo = E_Tile.RIO;
                    break;
                case E_Tile.RIO:
                    tipo = E_Tile.MURO;
                    break;
                case E_Tile.MURO:
                    tipo = E_Tile.CESPED;
                    break;
                default:
                    break;
            }
            updateTile();
        }
    }
    public bool isOccupied()
    {
        return ocupada;
    }

    public int getCost() {
        return coste;
    }
}
