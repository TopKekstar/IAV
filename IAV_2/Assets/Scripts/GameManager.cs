using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public GameObject unidadActual;
    public Mapa mapa;

    // Use this for initialization
    void Start () {
        instance = this;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setCurrentUnit(GameObject unidad)
    {
        unidadActual = unidad;
    }

    public void mueveUnidad(Vector3 casilla)
    {
       if (unidadActual != null)
        {
           unidadActual.GetComponent<PathFinder>().CalculatePath(casilla);
        }
    }
}
