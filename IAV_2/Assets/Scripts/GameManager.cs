﻿using System.Collections;
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
        Debug.Log(casilla.ToString());
        if (unidadActual != null)
        {
            Queue<Vector3>camino =  unidadActual.GetComponent<PathFinder>().CalculatePath(casilla);
            while (camino.Count>0)
            {
                Debug.Log(camino.Peek());

            }
        }
    }
}
