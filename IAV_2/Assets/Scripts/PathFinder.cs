using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PathFinder : MonoBehaviour
{

    Mapa mapa;
    Vector2Int[] directions = { new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1) };
	public Vector3 to;
    int[,] DistTo;
    Vector2Int[,] EdgeTo;
    Vector2Int UltimaCasilla;
    bool caminoPosible;
    private Priority_Queue.SimplePriorityQueue<Vector2Int, int> PQ = new Priority_Queue.SimplePriorityQueue<Vector2Int, int>();
    // Use this for initialization
    void Start()
    {
        mapa = transform.parent.gameObject.GetComponent<Mapa>();
        
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

            if (!mapa.getOccupied(destino.y, destino.x))
            {
                if (DistTo[destino.y, destino.x] > DistTo[origen.y, origen.x] + mapa.getCostOfTile(destino.y, destino.x))
                {
                    DistTo[destino.y, destino.x] = DistTo[origen.y, origen.x] + mapa.getCostOfTile(destino.y, destino.x);
                    EdgeTo[destino.y, destino.x] = origen;
                    PQ.Enqueue(destino, DistTo[destino.y, destino.x] + heuristic(origen, UltimaCasilla));
                }
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
            to = EdgeTo[to.y, to.x];

        }



        return queue;
    }

    public bool CalculatePath(GameObject g)
    {
        to = g.transform.localPosition;
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        UltimaCasilla = new Vector2Int((int)to.x, (int)to.y);
        PQ = new Priority_Queue.SimplePriorityQueue<Vector2Int, int>();
        DistTo = mapa.getDistTo((int)transform.localPosition.y, (int)transform.localPosition.x);
        EdgeTo = new Vector2Int[mapa.altoMapa, mapa.anchoMapa];
        for (int i = 0; i < mapa.altoMapa; i++)
        {
            for (int j = 0; j < mapa.anchoMapa; j++)
            {
                EdgeTo[i, j] = new Vector2Int(-1, -1);
            }
        }
        Vector2Int from = new Vector2Int((int)transform.localPosition.x, (int)transform.localPosition.y);
        mapa.setOccupied(from.y, from.x, true);
        PQ.EnqueueWithoutDuplicates(from, 0);
        caminoPosible = false;


        int k = 0;

        while (PQ.Count > 0 && !caminoPosible)
        {
            k++;
            Vector2Int top = PQ.Dequeue();
            if (top != UltimaCasilla)
            {
                for (int i = 0; i < directions.Length; i++)
                {
                    relax(top, directions[i]);
                }
            }
            else
            {
                caminoPosible = true;
            }

        }
        stopwatch.Stop();
        if (caminoPosible)
        {
            GetComponent<Unidad>().setPath(GetPath(ref UltimaCasilla, ref from));
        }
        GameManager.instance.updateDiagnostico(caminoPosible);
		GameManager.instance.updateDiagnostico(k,stopwatch.Elapsed.TotalMilliseconds,stopwatch.ElapsedTicks);


        return (caminoPosible);

    }

}
