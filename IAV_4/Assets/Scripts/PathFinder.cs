using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PathFinder : MonoBehaviour
{
    Mapa mapa;
    Agente agent;
    public static Vector2Int[] directions = { new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1) };
	public Vector3 to;
    Vector2Int bestOption;
    int[,] DistTo;
    Vector2Int[,] EdgeTo;
    bool[,] marcados;

    Vector2Int UltimaCasilla;
    bool caminoPosible;
    private Priority_Queue.SimplePriorityQueue<Vector2Int, int> PQ = new Priority_Queue.SimplePriorityQueue<Vector2Int, int>();
    // Use this for initialization
    void Start()
    {
        mapa = transform.parent.gameObject.GetComponent<Mapa>();
        agent = GetComponent<Agente>();
		
    }
    // Update is called once per frame
    void Update()
    {

    }

    int heuristic(Vector2Int a, Vector2Int b)
    {
        return 1*(System.Math.Abs(a.x - b.x) + System.Math.Abs(a.y - b.y));
    }

    void relax(Vector2Int origen, Vector2Int direccion)
    {
        Vector2Int destino = origen + direccion;
        if (!(destino.x < 0 || destino.y < 0 || destino.x >= mapa.getAncho() || destino.y >= mapa.getAlto()))
        {
            
                if (DistTo[destino.y, destino.x] > DistTo[origen.y, origen.x] + 1)
                {
                    DistTo[destino.y, destino.x] = DistTo[origen.y, origen.x] + 1;
                    EdgeTo[destino.y, destino.x] = origen;
                    int h = heuristic(destino, UltimaCasilla);
                    if (h != 0)
                        h += DistTo[destino.y, destino.x];
                    else
                        h = 0;

                    PQ.Enqueue(destino, h);

                }
            
        }
    }

    Stack<Vector2Int> GetPath(ref Vector2Int to, ref Vector2Int from)
    {
        Stack<Vector2Int> queue = new Stack<Vector2Int>();

        queue.Push(to);
        while (to != from)
        {
			
            queue.Push(EdgeTo[to.y, to.x]);
			to = EdgeTo [to.y, to.x];


        }



        return queue;
    }

    public bool CalculatePath(Vector2Int casilla)
    {

        UltimaCasilla = new Vector2Int((int)casilla.x, (int)casilla.y);
        PQ = new Priority_Queue.SimplePriorityQueue<Vector2Int, int>();
        Vector2Int from = new Vector2Int((int)transform.localPosition.x, (int)transform.localPosition.y);
        DistTo = mapa.getDistTo(from.y, from.x);
        EdgeTo = new Vector2Int[mapa.altoMapa, mapa.anchoMapa];
        for (int i = 0; i < mapa.altoMapa; i++)
        {
            for (int j = 0; j < mapa.anchoMapa; j++)
            {
                EdgeTo[i, j] = new Vector2Int(-1, -1);
            }
        }
        PQ.EnqueueWithoutDuplicates(from, 0);
        caminoPosible = false;


        int k = 0;

        while (PQ.Count > 0)
        {
            k++;
            Vector2Int top = PQ.Dequeue();
            if (top != UltimaCasilla)
            {
                for (int i = 0; i < directions.Length; i++)
                {
                    relax(top, directions[i]);
                }
            }else
            {
                caminoPosible = true;
            }

        }

        if (caminoPosible)
        {
            GetComponent<Agente>().setPath(GetPath(ref UltimaCasilla, ref from));
        }
        


        return (caminoPosible);

    }

    
	

}
