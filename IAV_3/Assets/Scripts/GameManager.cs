using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public Vector2Int[] directions = { new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1) };
    public static GameManager instance;
    public RTS_Cam.RTS_Camera camara;
    public GameObject unidadActual;
    public GameObject prefabUnidad;
    public GameObject prefabCross;
    public bool editMode;
    public Mapa mapa;
    public UnityEngine.UI.Text modoActual;
    public UnityEngine.UI.Text Informacion;
    public GameObject instrucciones;
    public UnityEngine.UI.Text diagnostico;
    public List<GameObject> listaUnidades;


    // Use this for initialization
    void Start() {
        instance = this;
        editMode = false;

    }
    public void GuardaUnidad(GameObject unidad)
    {
        listaUnidades.Add(unidad);
    }
    public void borraUnidad(GameObject gObj)
    {
        Destroy(gObj);

    }
    void borraUnidades()
    {
        for (int i = 0; i < listaUnidades.Count; i++)
        {
            Destroy(listaUnidades[i]);
        }
    }

    public void reiniciaMapa()
    {
        borraUnidades();
        mapa.iniciarAleatorio();
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.E))
        {
            editMode = !editMode;
            if (editMode)
            {
                modoActual.text = "Modo Editar";

            }
            else
            {
                modoActual.text = "Modo Mover";
            }

        }
        if (Input.GetKeyUp(KeyCode.H))
        {
			instrucciones.SetActive(!instrucciones.gameObject.active);
        }
		if (Input.GetKeyUp(KeyCode.R))
		{
			camara.ResetTarget ();
			Informacion.text = "NULL";
			Informacion.color = Color.white;
			Informacion.GetComponentInParent<UnityEngine.UI.Image>().color = Color.black;
			unidadActual = null;
            diagnostico.text = "";
		}
        if (Input.GetKeyUp(KeyCode.Q))
        {
            reiniciaMapa();
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

    }

    public void setCurrentUnit(GameObject unidad)
    {
        unidadActual = unidad;
        
        Informacion.text = "unidad █ ";
        Informacion.color = unidadActual.GetComponent<SpriteRenderer>().color;
        Informacion.GetComponentInParent<UnityEngine.UI.Image>().color = new Color(1.0f - Informacion.color.r, 1.0f - Informacion.color.g, 1.0f - Informacion.color.b);
    }

    public void mueveUnidad(GameObject casilla)
    {
        if (editMode)
        {
            GameObject gObj= Instantiate(prefabUnidad, mapa.transform);
            gObj.transform.Translate(casilla.transform.localPosition);
            GuardaUnidad(gObj);

        }
        else if (unidadActual != null)
        {
            if (unidadActual.GetComponent<PathFinder>().CalculatePath(casilla))
            {
                setCross(unidadActual, casilla);

            }
            camara.SetTarget(unidadActual.transform);
            
            unidadActual = null;
        }
    }
    public void setCross(GameObject unidad,GameObject casilla) {
        GameObject cross = Instantiate(prefabCross, casilla.transform.position, casilla.transform.localRotation, mapa.transform);
        unidad.GetComponent<Unidad>().setCross(ref cross);
    }
	public void updateDiagnostico(int numeroVisitas,double milisengundos,long ticks)
    {
        diagnostico.text += "Ha visitado " + numeroVisitas.ToString() + " casillas\nHa necesitado "+ milisengundos.ToString() + " ms";
        diagnostico.text += "\nHa necesitado \b"+ ticks.ToString() + " ticks";
    }
    public void updateDiagnostico(bool conseguido)
    {
        diagnostico.text = (conseguido)?"Ruta posible\n": "Ruta imposible\n";
        
    }
}
