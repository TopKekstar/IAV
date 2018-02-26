using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public Canvas _canvas;
	public GameObject prefabTile;
    int ultimoMov;
	public int dimensiones;
    public static GameManager instance;
    int[,] tablero;
    int lastMov;
	// Use this for initialization
	void Start () {
        instance= this;
        setIni();


        /*for (int i = 0; i < 9; i++) {
			GameObject g = Instantiate (prefabTile);
			g.name = (i + 1).ToString ();
			g.transform.SetParent(_canvas.transform);
		}*/
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
            getIJ(0, out i1, out j1);
            getIJ(kek, out i2, out j2);
            swap(i1, j1, i2, j2);
        }
        construir();
    }

    void getIJ(int id,out int ic, out int jc)
    {
        bool flag=true;
        ic = 0;
        jc = 0;
        for (int i = 0; i < dimensiones&&flag; i++)
        {
            for (int j = 0; j < dimensiones&&flag; j++)
            {
                if (tablero[i, j] == id)
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

        swap(l1, c1, l2, c2);
        t1.SetSiblingIndex(i2);
        t2.SetSiblingIndex(i1);

    }

    void swap(int i1, int j1,int i2, int j2)
    {
        int temp = tablero[i1, j1];
        tablero[i1, j1] = tablero[ i2, j2];
        tablero[i2, j2] = temp;
    }


    // Update is called once per frame
    void Update () {
		
	}
	void updateScene(){

	
	}
    void BFS()
    {

    }

    List<int[,]> visita(ref int[,] t)
    {
        List<int[,]> iter = new List<int[,]>();
        List<int> movs = movsDisponibles(ref t);
        for (int i = 0; i < movs.Count; i++)
        {
            int m = movs[i];
            if(m != lastMov)
            {
                int[,] ite = t;

            }
        }
        return iter;

    }
    bool solucion()
    {
        for (int i = 0; i < dimensiones; i++)
        {
            for (int j = 0; j < dimensiones; j++)
            {
                int piece = tablero[i, j];
                
                if (piece != 0)
                {

                    int originalLine = piece - 1 / dimensiones;
                    int originalColumn = (piece - 1) % dimensiones;
                    if (i != originalLine || j != originalColumn) return false;
                }
            }
        }
        return true;
    }


}
