using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    protected struct State
    {
        public void ini(List<int> l, int[,] t)
        {
            c = new List<int>(l);
            _t = t;
            cantidadDesorden = 0;
        }
        public List<int> c;
        public int[,] _t;
        public int cantidadDesorden;
    }
    List<int> movimientos;
    float tiempoNecesitado;

    public Canvas _canvas;
	public GameObject prefabTile;
    public UnityEngine.UI.Text HUDTiempo;
    public UnityEngine.UI.Text HUDPasos;
    int ultimoMov;
	public int dimensiones;
    public static GameManager instance;
    int[,] tablero;
    int[,] tSol;

   
    int lastMov;
	// Use this for initialization
	void Start () {
        instance= this;
        setIni();


        
    }

    protected int[,] copiarTablero(ref int [,] t)
    {
        int[,] tc = new int[dimensiones, dimensiones];
        for (int i = 0; i < dimensiones; i++)
        {
            for (int j = 0; j < dimensiones; j++)
            {
                tc[i, j] = t[i, j];

            }
        }
        return tc;

    }

    public bool canMove(int index, out int cambio)
    {
        int linea = index / dimensiones;
        int columna = index % dimensiones;
        cambio = linea * dimensiones + columna;
        if (linea > 0 && 0 == tablero[linea - 1, columna])
        {
            cambio = (linea - 1) * dimensiones + columna;
            return true;
        }
        else if (linea < dimensiones - 1 && 0 == tablero[linea + 1, columna])
        {
            cambio = (linea + 1)* dimensiones + columna;
            return true;
        }
        else if (columna > 0 && 0 == tablero[linea, columna - 1])
        {
            cambio = linea * dimensiones + columna-1;
            return true;
        }
        else if (columna < dimensiones - 1 && 0== tablero[linea, columna + 1])
        {
            cambio = linea * dimensiones + columna+1;
            return true;
        }
        return false;
    }

    public bool canMove(int i,int j, out int cambio,ref int [,] t)
    {
        int linea = i;
        int columna = j;
        cambio = linea * dimensiones + columna;
        if (linea > 0 && 0 == t[linea - 1, columna])
        {
            cambio = (linea - 1) * dimensiones + columna;
            return true;
        }
        else if (linea < dimensiones - 1 && 0 == t[linea + 1, columna])
        {
            cambio = (linea + 1) * dimensiones + columna;
            return true;
        }
        else if (columna > 0 && 0 == t[linea, columna - 1])
        {
            cambio = linea * dimensiones + columna - 1;
            return true;
        }
        else if (columna < dimensiones - 1 && 0 == t[linea, columna + 1])
        {
            cambio = linea * dimensiones + columna + 1;
            return true;
        }
        return false;
    }

    public void setIni() {
        lastMov = -1;
        
        tablero = new int[dimensiones, dimensiones];
        for (int i = 0; i < dimensiones; i++)
        {
            for (int j = 0; j < dimensiones; j++)
            {
                int n = i * dimensiones + j + 1;
                tablero[i, j] = n;
                if (n == dimensiones * dimensiones)
                {
                    tablero[i, j] = 0;
                }
                
            }
        }
        tSol = copiarTablero(ref tablero);
        construir();

    }

    void construir()
    {
        if (_canvas.transform.childCount > 0)
        {
            for (int i = 0; i < dimensiones * dimensiones; i++)
            {
                Destroy(_canvas.transform.GetChild(i).gameObject);

            }
        }
        for (int i = 0; i < dimensiones; i++)
        {
            for (int j = 0; j < dimensiones; j++)
            {
                GameObject t = Instantiate(prefabTile);
                t.transform.SetParent(_canvas.transform);
                int n = i * dimensiones + j + 1;
                if (tablero[i,j]==0)
                {
                    t.GetComponent<Tile>().init(0, n - 1);
                }
                else
                {
                    t.GetComponent<Tile>().init(tablero[i, j], n-1);
                }
            }
        }

    }
    
    public List<int> movsDisponibles(ref int[,] t)
    {
        List<int> listaMovs = new List<int>();
        for (int i = 0; i < dimensiones; i++)
        {
            for (int j = 0; j < dimensiones; j++)
            {
                int c;
                if (canMove(i,j,out c, ref t) )
                {
                    listaMovs.Add(t[i, j]);
                }

            }
        }
        return listaMovs;
    }

    public void barajar()
    {
        lastMov = -1;
        for (int i = 0; i < 10; i++)
        {
            List<int> list = movsDisponibles(ref tablero);
            int kek = list[Random.Range(0, list.Count)];
            int i1, j1, i2, j2;
            getIJ(0, out i1, out j1, ref tablero);
            getIJ(kek, out i2, out j2, ref tablero);
            swap(i1, j1, i2, j2,ref tablero);
        }
        construir();
    }

    void getIJ(int id,out int ic, out int jc, ref int [,] t)
    {
        bool flag=true;
        ic = 0;
        jc = 0;
        for (int i = 0; i < dimensiones&&flag; i++)
        {
            for (int j = 0; j < dimensiones&&flag; j++)
            {
                if (t[i, j] == id)
                {
                    flag = false;
                    ic = i;
                    jc = j;
                }

            }

        }
    }

        public void move(int i1,int i2)
    {
        int l1 = i1 / dimensiones;
        int c1 = i1 % dimensiones;
        int l2 = i2 / dimensiones;
        int c2 = i2 % dimensiones;

        Transform t1 = _canvas.transform.GetChild(i1);
        Transform t2 = _canvas.transform.GetChild(i2);

        t1.GetComponent<Tile>().index = i2;
        t2.GetComponent<Tile>().index = i1;

        swap(l1, c1, l2, c2,ref tablero);
        t1.SetSiblingIndex(i2);
        t2.SetSiblingIndex(i1);

    }

    void swap(int i1, int j1,int i2, int j2, ref int [,] t)
    {
        int temp = t[i1, j1];
        t[i1, j1] = t[ i2, j2];
        t[i2, j2] = temp;
    }

    


    // Update is called once per frame
    void Update () {
		
	}
	void updateScene(){

	
	}

    void mostrarSolucion()
    {
        if (movimientos.Count > 0)
        {
            int ir, jr, ih, jh;
            getIJ(movimientos[0], out ir, out jr, ref tablero);
            getIJ(0, out ih, out jh, ref tablero);
            movimientos.RemoveAt(0);
            swap(ir, jr, ih, jh, ref tablero);
            construir();
            Invoke("mostrarSolucion", 0.5f);

        }
    }

    

     bool comparaTableros( int[,] t1,  int[,] t2)
    {
        bool flag = true;
        for (int i = 0; i < dimensiones && flag; i++)
        {
            for (int j = 0; j < dimensiones && flag; j++)
            {
                if (t1[i, j] != t2[i, j])
                    flag = false;
            }
        }
        return flag;
    }

    bool solucion( int[,] t)
    {
        
        return comparaTableros( t, tSol);
    }

    int calcularDesorden(ref int[,] t)
    {
        int desorden = 0;
        for (int i = 0; i < dimensiones ; i++)
        {
            for (int j = 0; j < dimensiones; j++)
            {
                if (t[i, j] != 0 && t[i, j] != tSol[i, j])
                    desorden++;
            }
        }
        return desorden;

    }
    int calcularDistancia(ref int[,] t)
    {
        int distancia = 0;
        for (int i = 0; i < dimensiones; i++)
        {
            for (int j = 0; j < dimensiones; j++)
            {
                if (t[i, j] != 0)
                {
                    int lineaOriginal = (t[i, j]-1) / dimensiones;
                    int columnaOriginal = (t[i, j] - 1) % dimensiones;
                    distancia += System.Math.Abs(i - lineaOriginal) + System.Math.Abs(j - columnaOriginal);
                }
                    
            }
        }
        return distancia;

    }

    List<State> visita(ref State t)
    {
        List<State> iter = new List<State>();
        List<int> movs = movsDisponibles(ref t._t);
        for (int i = 0; i < movs.Count; i++)
        {
            int m = movs[i];
            if (m != lastMov)
            {
                int[,] ite = copiarTablero(ref t._t);
                int ir, jr, ih, jh;
                getIJ(m, out ir, out jr, ref ite);
                getIJ(0, out ih, out jh, ref ite);
                swap(ir, jr, ih, jh, ref ite);
                State st = new State();
                st.ini(t.c, ite);
                st.c.Add(m);
                iter.Add(st);
            }
        }
        return iter;

    }
    
    public void BFS()
    {
        float tini = Time.deltaTime;
        int[,] inicial = copiarTablero(ref tablero);
        List<State> stack = new List<State>();
        State iniS = new State();
        iniS.ini(new List<int>(),inicial);
        stack.Add(iniS);
        
        bool flag = true;
        while (stack.Count > 0&&flag)
        {
           
            State top = (State)stack[0];
            stack.RemoveAt(0);
            if (solucion( top._t))
            {

                movimientos = top.c;
                flag = false;
            }
            else
            {
                List<State> list = visita(ref top);
                int limite = stack.Count;
                for (int i = 0; i < list.Count; i++)
                {
                    bool meter = true;
                    for (int j = 0; j < limite; j++)
                    {
                        if (comparaTableros(stack[j]._t, list[i]._t))
                        {
                            if (stack[j].c.Count > list[i].c.Count)
                                stack[j] = list[i];
                            else
                                meter = false;
                        }
                    }
                    if (meter)
                    {
                        stack.Add(list[i]);
                    }

                }
        }
            
        }
        tiempoNecesitado = Time.deltaTime- tini;
        HUDTiempo.text = tiempoNecesitado.ToString() + " ms";
        mostrarSolucion();
    }

    List<State> visitaH(ref State t)
    {
        List<State> iter = new List<State>();
        List<int> movs = movsDisponibles(ref t._t);
        for (int i = 0; i < movs.Count; i++)
        {
            int m = movs[i];
            if (m != lastMov)
            {
                int[,] ite = copiarTablero(ref t._t);
                int ir, jr, ih, jh;
                getIJ(m, out ir, out jr, ref ite);
                getIJ(0, out ih, out jh, ref ite);
                swap(ir, jr, ih, jh, ref ite);
                State st = new State();
                st.ini(t.c, ite);
                st.cantidadDesorden = calcularDesorden(ref st._t);
                if (st.cantidadDesorden <= t.cantidadDesorden)
                {
                    st.c.Add(m);
                    iter.Add(st);
                }
            }
        }
        return iter;

    }

    public void hamill()
    {
        float tini = Time.deltaTime;
        int[,] inicial = copiarTablero(ref tablero);
        List<State> stack = new List<State>();
        State iniS = new State();
        iniS.ini(new List<int>(), inicial);
        iniS.cantidadDesorden = calcularDesorden(ref iniS._t);
        stack.Add(iniS);
        bool flag = true;
        while (stack.Count > 0 && flag)
        {

            State top = (State)stack[0];
            stack.RemoveAt(0);
            if (solucion(top._t))
            {

                movimientos = top.c;
                flag = false;
            }
            else
            {



                List<State> list = visitaH(ref top);
                int limite = stack.Count;
                for (int i = 0; i < list.Count; i++)
                {
                    bool meter = true;
                    for (int j = 0; j < limite; j++)
                    {
                        if (comparaTableros(stack[j]._t, list[i]._t))
                        {
                            if (stack[j].c.Count > list[i].c.Count)
                                stack[j] = list[i];
                            else
                                meter = false;
                        }
                    }
                    if (meter)
                    {
                        stack.Add(list[i]);
                    }

                }
                Debug.Log(stack.Count);
            }

        }
        tiempoNecesitado = Time.deltaTime - tini;
        HUDTiempo.text = tiempoNecesitado.ToString() + " ms";
        mostrarSolucion();
    }

    List<State> visitaM(ref State t)
    {
        float tini = Time.time;
        List<State> iter = new List<State>();
        List<int> movs = movsDisponibles(ref t._t);
        for (int i = 0; i < movs.Count; i++)
        {
            int m = movs[i];
            if (m != lastMov)
            {
                int[,] ite = copiarTablero(ref t._t);
                int ir, jr, ih, jh;
                getIJ(m, out ir, out jr, ref ite);
                getIJ(0, out ih, out jh, ref ite);
                swap(ir, jr, ih, jh, ref ite);
                State st = new State();
                st.ini(t.c, ite);
                st.cantidadDesorden = calcularDistancia(ref st._t);
                if (st.cantidadDesorden <= t.cantidadDesorden)
                {
                    st.c.Add(m);
                    iter.Add(st);
                }
            }
        }
        return iter;

    }

    public void manhattan() {
        float tini = Time.deltaTime;
        int[,] inicial = copiarTablero(ref tablero);
        List<State> stack = new List<State>();
        State iniS = new State();
        iniS.ini(new List<int>(), inicial);
        iniS.cantidadDesorden = calcularDistancia(ref iniS._t);
        stack.Add(iniS);
        bool flag = true;
        while (stack.Count > 0 && flag)
        {

            State top = (State)stack[0];
            stack.RemoveAt(0);
            if (solucion(top._t))
            {

                movimientos = top.c;
                flag = false;
            }
            else
            {



                List<State> list = visitaM(ref top);
                int limite = stack.Count;
                for (int i = 0; i < list.Count; i++)
                {
                    bool meter = true;
                    for (int j = 0; j < limite; j++)
                    {
                        if (comparaTableros(stack[j]._t, list[i]._t))
                        {
                            if (stack[j].c.Count > list[i].c.Count)
                                stack[j] = list[i];
                            else
                                meter = false;
                        }
                    }
                    if (meter)
                    {
                        stack.Add(list[i]);
                    }

                }
                Debug.Log(stack.Count);
            }

        }
        tiempoNecesitado = Time.time - tini;
        HUDTiempo.text = tiempoNecesitado.ToString() + " ms";
        mostrarSolucion();

    }


}


    


    
    



