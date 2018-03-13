using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PathFinder : MonoBehaviour {
    Mapa mapa;
    Vector2Int[] directions = { new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1) };
	
	bool[,] marcados;
	int[,] DistTo;
	Vector2Int[,] EdgeTo;
    Vector2Int UltimaCasilla;
    private Priority_Queue.SimplePriorityQueue<Vector2Int,int> PQ = new Priority_Queue.SimplePriorityQueue<Vector2Int,int>();	
    // Use this for initialization
    void Start () {
        mapa = transform.parent.gameObject.GetComponent<Mapa>();		
	}
	// Update is called once per frame
	void Update () {
		
	}

	void relax(Vector2Int origen, Vector2Int direccion)
    {
        Vector2Int destino = origen + direccion;
		if (!(destino.x < 0 || destino.y < 0 || destino.x >= mapa.getAncho () || destino.y >= mapa.getAlto ())) {
            
			if (!marcados [destino.y, destino.x]) {
				if (DistTo [destino.y, destino.x] > DistTo [origen.y, origen.x]+ mapa.getCostOfTile(destino.y, destino.x)) {
					DistTo [destino.y, destino.x] = DistTo [origen.y, origen.x] + mapa.getCostOfTile (destino.y, destino.x);
					EdgeTo [destino.y, destino.x] = origen;
                    PQ.Enqueue(destino, DistTo[destino.y, destino.x]);                   
				}
			}
		}
    }

    Stack<Vector2Int> GetPath(ref Vector2Int to,ref Vector2Int from)
    {
        Stack<Vector2Int> queue = new Stack<Vector2Int>();

        queue.Push(to);
        while (to != from) 
        {
            queue.Push(EdgeTo[to.y, to.x]);
            Debug.Log(queue.Peek().ToString());
            to = EdgeTo[to.y, to.x];
            
        } 



        return queue;
    }

   public void CalculatePath(Vector3 to)
    {
        UltimaCasilla = new Vector2Int((int)to.x, (int)to.y);
        Vector2Int from = new Vector2Int((int)transform.localPosition.x, (int)transform.localPosition.y);


        PQ = new Priority_Queue.SimplePriorityQueue<Vector2Int, int>();
        marcados = mapa.getMarcados();
        DistTo = mapa.getDistTo((int)transform.localPosition.y, (int)transform.localPosition.x);
		EdgeTo = new Vector2Int[mapa.altoMapa, mapa.anchoMapa];
		PQ.EnqueueWithoutDuplicates(from,0);
        int k = 0;

		while (PQ.Count>0 && k <100)
        {
			Vector2Int top = PQ.Dequeue();
            for (int i = 0; i < directions.Length; i++)
            {
                relax(top, directions[i]);
            }
            k++;
        }
        GetComponent<Unidad>().setPath( GetPath(ref UltimaCasilla, ref from));
		
    }

}
