using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *https://picoledelimao.github.io/blog/2015/12/06/solving-the-sliding-puzzle/ 
 * */

public class Puzzle : MonoBehaviour {
    public enum Direcciones
    {
        IZQ,DER,ARR,ABA,NULL
    }
    public int dimensiones = 3;
    public int lastMov = 0;
    public List<int> Camino;
    public int [,] Tablero;
	// Use this for initialization
	void Start () {
		
	}
    Puzzle( Puzzle a)
    {
        Tablero = a.Tablero;
        dimensiones = a.dimensiones;
        Camino = a.Camino;
        lastMov = a.lastMov;
    }

 
	
	// Update is called once per frame
	void Update () {
		
	}
    public void initMap()
    {
        Tablero = new int[dimensiones, dimensiones];
        for (int i = 0; i < dimensiones; i++)
        {
            for (int j = 0; j < dimensiones; j++)
            {
                if ((i * dimensiones + j) < dimensiones * dimensiones - 1)
                    Tablero[i, j] = i * dimensiones + j + 1;
                else
                    Tablero[i, j] = 0;
                Debug.Log(Tablero[i,j]);
            }
        }

    }

    public void desordenar()
    {
        for (int i = 0; i < 1; i++)
        {
            List<int> list = movimientosPermitidos();
            int kek = list[Random.Range(0, list.Count)];
            Debug.Log(kek);
            move(kek);
        }
    }

    public void imprimir()
    {
        for (int i = 0; i < dimensiones; i++)
        {
            for (int j = 0; j < dimensiones; j++)
            {

                Debug.Log(Tablero[i, j]);
            }
        }
    }

    public Vector2Int obtenerVacio()
    {
        Vector2Int v = new Vector2Int(-1, -1);
        for (int i = 0; i < dimensiones; i++)
        {
            for (int j = 0; j < dimensiones; j++)
            {
                if (Tablero[i, j] == 0)
                {
                    v.Set(i, j);
                }

            }
        }
        return v;

    }

    public void swap(int i1,int j1,int i2,int j2)
    {
        int temp = Tablero[i1,j1];
        Tablero[i1,j1] = Tablero[i2,j2];
        Tablero[i2,j2] = temp;

    }

    public Direcciones getMove(ref int pieza) {
        Vector2Int vacio = obtenerVacio();
        int line = vacio.y;
        int column = vacio.x;
        if (line > 0 && pieza == Tablero[ line - 1,column])
        {
            return Direcciones.ABA;
        }
        else if (line < 3 - 1 && pieza == Tablero[line + 1,column])
        {
            return Direcciones.ARR;
        }
        else if (column > 0 && pieza == Tablero[ line,column-1])
        {
            return Direcciones.DER;
        }
        else if (column < 3 - 1 && pieza == Tablero[line, column + 1])
        {
            return Direcciones.IZQ;
        }
        Debug.Log(vacio);
        return Direcciones.NULL;
    }
    
    public Direcciones move(int pieza)
    {
        Direcciones m = getMove(ref pieza);
        if(m == Direcciones.NULL)
        {
            Debug.Log("suu");
            Vector2Int vacio = obtenerVacio();
            int line = vacio.y;
            int column = vacio.x;
            switch (m)
            {
                case Direcciones.IZQ:
                    this.swap( column,line,  column + 1,line);
                    break;
                case Direcciones.DER:
                    this.swap(column,line, column - 1, line);
                    break;
                case Direcciones.ARR:
                    this.swap(column, line,column, line + 1);
                    break;
                case Direcciones.ABA:
                    this.swap( column,line, column, line - 1);
                    break;
            }
            if (m != Direcciones.NULL)
            {
                lastMov = pieza;
            }
        }
        return m;
    }

    public Puzzle copia()
    {
        return new Puzzle(this);
    }

    public List<int> movimientosPermitidos()
    {
        List<int> listaMovs = new List<int>();
        for (int i = 0; i < dimensiones; i++)
        {
            for (int j = 0; j < dimensiones; j++)
            {
                if ( getMove(ref Tablero[i, j]) != Direcciones.NULL)
                {
                    listaMovs.Add(Tablero[i, j]);
                }

            }
        }
        return listaMovs;
    }

    public List<Puzzle> visita()
    {
        List<Puzzle> hijos = new List<Puzzle>();
        List<int> movsPermitidos = movimientosPermitidos();
        for (int i = 0; i < movsPermitidos.Count; i++)
        {
            int m = movsPermitidos[i];
            if(m != lastMov)
            {
                Puzzle iteracion = copia();

                iteracion.move(m);
                iteracion.Camino.Add(m);
                hijos.Add(iteracion);
                

            }
        }
        
        return hijos;
    }

    public bool solucion()
    {
        for (int i = 0; i < dimensiones; i++)
        {
            for (int j = 0; j < dimensiones; j++)
            {
                int piece = Tablero[i,j];
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
    

    public void BFS()
    {
        Puzzle inicial = copia();
        Stack<Puzzle> stack = new Stack<Puzzle>();
        stack.Push(inicial);
        while (stack.Count>0)
        {
            Puzzle top = stack.Pop();
            if (top.solucion())
            {
                Debug.Log("suuu");
            }

            List<Puzzle> list = visita();
            for (int i = 0; i < list.Count; i++)
            {
                if (!stack.Contains(list[i]))
                {
                    stack.Push(list[i]);
                }
            }

        }


    }
}


