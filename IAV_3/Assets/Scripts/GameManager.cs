using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour {

    public Vector2Int[] directions = { new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1) };
    public static GameManager instance;


    public RTS_Cam.RTS_Camera camara;
    public GameObject prefabUnidad;

    public GameObject agenteNormal;
    public GameObject agenteAleatorio;
    public GameObject agenteDespistado;
    public GameObject agenteCentrista;
    public GameObject agenteMediana;
  



    public GameObject prefabCross;

    public GameObject unidadActual;


    public bool editMode;
    public Mapa mapa;
    public UnityEngine.UI.Text modoActual;
    public UnityEngine.UI.Text Informacion;
    public GameObject instrucciones;
    public UnityEngine.UI.Text diagnostico;
    public List<GameObject> listaUnidades;

    public GameObject prefabCuchillo;
    public GameObject prefabCuerpo;
    public GameObject prefabCasa;
    public GameObject prefabSangre;


    private void Awake()
    {
        instance = this;
        editMode = false;
    }
    // Use this for initialization
    void Start() {
        

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
    public void derrota()
    {
        ActualizarInfo( "Se ha caido por el borde");
        Invoke("reiniciaMapa",2.5f);
    }
    public void reiniciaMapa()
    {
        borraUnidad(unidadActual);
        mapa.iniciarMapa();
    }

    public void ActualizarInfo(string estado)
    {
        Informacion.text = estado;
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
        unidad.GetComponent<Agente>().setCross(ref cross);
    }
	public void updateDiagnostico(int numeroPasos)
    {
        diagnostico.text = "Lleva " + numeroPasos + " Pasos";
    }
    public void updateDiagnostico(bool conseguido)
    {
        diagnostico.text = (conseguido)?"Ruta posible\n": "Ruta imposible\n";
        
    }

    public void SetAgenteAleatorio()
    {
        modoActual.text = "Agente Aleatorio";
        prefabUnidad = agenteAleatorio;
        reiniciaMapa();
    }
    public void SetAgenteNormal()
    {
        modoActual.text = "Agente Normal";
        prefabUnidad = agenteNormal;
        reiniciaMapa();

    }
    public void SetAgenteDespistado()
    {
        modoActual.text = "Agente Despistado";
        prefabUnidad = agenteDespistado;
        reiniciaMapa();

    }
    public void SetAgenteCentrista()
    {
        modoActual.text = "Agente Centrista";
        prefabUnidad = agenteCentrista;
        reiniciaMapa();

    }
    public void SetAgenteMediana()
    {
        modoActual.text = "Agente Mediana";
        prefabUnidad = agenteMediana;
        reiniciaMapa();

    }
}
