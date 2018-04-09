﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PathFinder : MonoBehaviour
{
    Mapa mapa;
    Agente agent;
    Vector2Int[] directions = { new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1) };
	public Vector3 to;
    Vector2Int bestOption;
    int[,] DistTo;
    Vector2Int[,] EdgeTo;
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

            if ( agent.GetTILE_INFO(destino.y, destino.x).frontera || agent.GetTILE_INFO(destino.y, destino.x)._Terreno != Tile.T_Terreno.T_DESCONOCIDO) {
               
                
                if (DistTo[destino.y, destino.x] > DistTo[origen.y, origen.x] + agent.GetTILE_INFO(destino.y, destino.x).probPrecipicio+ agent.GetTILE_INFO(destino.y, destino.x).probCuerpo + 1)
                {
                    DistTo[destino.y, destino.x] = DistTo[origen.y, origen.x] + agent.GetTILE_INFO(destino.y, destino.x).probPrecipicio+ agent.GetTILE_INFO(destino.y, destino.x).probCuerpo + 1;
                    EdgeTo[destino.y, destino.x] = origen;
                    int h = heuristic(destino, destino);
                    h += DistTo[destino.y, destino.x];
                    if (agent.GetTILE_INFO(destino.y, destino.x).frontera)
                    {
                       if(bestOption.x == -1||DistTo[destino.y, destino.x]< DistTo[bestOption.y, bestOption.x])
                        {
                             bestOption= destino;

                        }


                    }
                    PQ.Enqueue(destino, h);
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
			to = EdgeTo [to.y, to.x];


        }



        return queue;
    }

    public bool CalculatePath(GameObject g)
    {
        bestOption = new Vector2Int(-1, -1);
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
            }

        }
        stopwatch.Stop();
        if (caminoPosible)
        {
            GetComponent<Agente>().setPath(GetPath(ref UltimaCasilla, ref from));
        }
        GameManager.instance.updateDiagnostico(caminoPosible);
		GameManager.instance.updateDiagnostico(k,stopwatch.Elapsed.TotalMilliseconds,stopwatch.ElapsedTicks);


        return (caminoPosible);

    }
	public bool Explore()
	{
		bestOption = new Vector2Int(-1, -1);
		System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
		stopwatch.Start();
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

		while (PQ.Count > 0)
		{
			k++;
			Vector2Int top = PQ.Dequeue();
			for (int i = 0; i < directions.Length; i++)
				{
					relax(top, directions[i]);
				}
			
		}
		stopwatch.Stop();
		Debug.Log (DistTo [bestOption.y, bestOption.x]+" coste destino");
		GetComponent<Agente>().setPath(GetPath(ref bestOption, ref from));
		GameManager.instance.updateDiagnostico(caminoPosible);
		GameManager.instance.updateDiagnostico(k,stopwatch.Elapsed.TotalMilliseconds,stopwatch.ElapsedTicks);

		return true;

	}

}
