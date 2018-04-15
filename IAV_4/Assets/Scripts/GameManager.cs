using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour {

    public Vector2Int[] directions = { new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1) };
    public static GameManager instance;


    public RTS_Cam.RTS_Camera camara;

    public GameObject prefabUnidad;
    public GameObject prefabZombi;
    public GameObject prefabAliado;
    public GameObject prefabRefugio;



    public GameObject unidadActual;

    public GameObject colocar;

    public GameObject Heroe;
    public GameObject Refugio;

    public bool Dia { get; set; }

    public bool editMode;
    public Mapa mapa;
    public UnityEngine.UI.Text modoActual;
    public UnityEngine.UI.Text Informacion;
    public GameObject instrucciones;
    public UnityEngine.UI.Text diagnostico;
    public List<GameObject> listaUnidades;
    public List<GameObject> zombies;




    private void Awake()
    {
        
        Dia = true;
        colocar = null;
        Refugio = null;
        Heroe = null;
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

    
    public void setCross(GameObject unidad,GameObject casilla) {
       
    }
	public void updateDiagnostico(int numeroPasos)
    {
        diagnostico.text = "Lleva " + numeroPasos + " Pasos";
    }
    public void updateDiagnostico(bool conseguido)
    {
        diagnostico.text = (conseguido)?"Ruta posible\n": "Ruta imposible\n";
        
    }

    public void ColocarObjeto(Vector2Int posicion)
    {
        if (colocar)
        {
            GameObject gObj = Instantiate(colocar, mapa.transform);
            gObj.transform.Translate(posicion.x, posicion.y, 0);
            if(colocar == prefabZombi)
            {
                zombies.Add(gObj);
            }
            if (colocar == prefabUnidad)
            {
                if (Heroe)
                    Destroy(gObj);
                else
                    Heroe = gObj;
            }
            if (colocar == prefabRefugio)
            {
                if (Refugio)
                    Destroy(gObj);
                else
                    Refugio = gObj;
            }
            if (colocar == prefabAliado)
            {
                listaUnidades.Add(gObj);
                
            }

        }
    }

    public void EscogerHeroe()
    {
        colocar = prefabUnidad;

    }

    public void EscogerZombie()
    {
        colocar = prefabZombi;

    }
    public void EscogerAliado()
    {
        colocar = prefabAliado;

    }

    public void EscogerRefugio()
    {
        colocar = prefabRefugio;


    }
}
