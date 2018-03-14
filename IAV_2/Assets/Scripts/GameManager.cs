using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public GameObject unidadActual;
    public GameObject prefabCross;
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

    public void mueveUnidad(GameObject casilla)
    {
        if (unidadActual != null)
        {
            if (unidadActual.GetComponent<PathFinder>().CalculatePath(casilla))
            {
                setCross(unidadActual, casilla);

            }
           unidadActual = null;
        }
    }
    public void setCross(GameObject unidad,GameObject casilla) {
        GameObject cross = Instantiate(prefabCross, casilla.transform.position, casilla.transform.localRotation, mapa.transform);
        unidad.GetComponent<Unidad>().setCross(ref cross);
    }
}
