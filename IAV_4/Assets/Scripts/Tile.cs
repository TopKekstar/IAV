
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    


    public enum T_Contenido : int
    {
        C_NADA = 0, C_SANGRE = 1, C_CUCHILLO = 2,C_CUERPO = 3, C_CASA = 4, C_DESCONOCIDO = 5,
    }


    T_Contenido _contenido;
    Vector2Int position;
    public bool descubierta;
    bool firstIni;

	// Use this for initialization
	void Start () {
        

    }
    
    public void SetDescubierta(bool desc)
    {
    }

    public bool GetOcupada()
    {
        return descubierta;
    }

    public void ini(int x, int y)
    {
        descubierta = false;
        if(_contenido != T_Contenido.C_NADA)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
        _contenido = T_Contenido.C_NADA;
        position = new Vector2Int(x, y);
        if(!firstIni)
            GetComponent<Transform>().transform.Translate(x, y, 0);
        firstIni = true;

    }
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (GameManager.instance.editMode)
            {
                GameManager.instance.ColocarObjeto(position);
            }
        }
    }

    public Vector2Int GetPosition()
    {
        return position;
    }
}
