using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PathFinder : MonoBehaviour {
    Mapa mapa;
    Vector3[] directions = { new Vector3(-1, 0), new Vector3(0, 1), new Vector3(1, 0), new Vector3(0, -1) };
    struct Camino
    {
        Queue<Vector3> queue;
        Vector3 casilla;
        int coste;
        public Camino(Camino c)
        {
            coste = c.coste;
            queue = new Queue<Vector3>(c.queue);
            casilla = new Vector3(c.casilla.x,c.casilla.y);

        }
        public void init(Queue<Vector3>q , Vector3 v,int cost)
        {
            coste = cost;
            queue = new Queue<Vector3>(q);
            casilla = new Vector3(v.x,v.y);
        }
        public void encola(Vector3 v)
        {
            queue.Enqueue(v);
        }
        public Vector3 Casilla()
        {
            return casilla;
        }
        public int getCoste()
        {
            return coste;
        }
        public Queue<Vector3> GetQueue()
        {
            return queue;
        }
    }
    class PriorityQueue<P, V>
    {
        private SortedDictionary<int, Queue<V>> list = new SortedDictionary<int, Queue<V>>();
        public void Enqueue(int priority, V value)
        {
            bool meter = true;
            foreach (int prior in list.Keys)
            {
                if (list[prior].Contains(value))
                {
                    if (prior <= priority)
                    {
                        meter = false;
                    }
                }
            }
            if (meter)
            {
                Queue<V> q;
                if (!list.TryGetValue(priority, out q))
                {
                    q = new Queue<V>();
                    list.Add(priority, q);
                }
                q.Enqueue(value);
            }
        }
        public V Dequeue()
        {
            // will throw if there isn’t any first element!
            var pair = list.GetEnumerator().Current;
            var v = pair.Value.Dequeue();
            if (pair.Value.Count == 0) // nothing left of the top priority.
                list.Remove(pair.Key);
            return v;
        }
        

        public bool IsEmpty
        {
            get { return list.Count==0; }
        }
    }
    // Use this for initialization
    void Start () {
        mapa = transform.parent.gameObject.GetComponent<Mapa>();		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void relax(ref PriorityQueue<int, Camino> pq, Camino origen, Vector3 direccion, ref bool[,] marcados, ref int[,] DistTo)
    {
        Vector3 casilla = origen.Casilla() + direccion;
        if(casilla.x>=0&& casilla.y >= 0 && casilla.x < mapa.getAncho() && casilla.y < mapa.getAlto())
        {
            if (!marcados[(int)casilla.y, (int)casilla.x])
            {
                if(DistTo[(int)casilla.y, (int)casilla.x]< DistTo[(int)origen.Casilla().y, (int)origen.Casilla().x] +mapa.getCostOfTile((int)casilla.y, (int)casilla.x))
                {
                    DistTo[(int)casilla.y, (int)casilla.x] = DistTo[(int)origen.Casilla().y, (int)origen.Casilla().y] + mapa.getCostOfTile((int)casilla.y, (int)casilla.x);
                    Camino paso = new Camino(origen);
                    paso.encola(direccion);
                    pq.Enqueue(DistTo[(int)casilla.y, (int)casilla.x], paso);
                }
            }
        }
        
    }

   public Queue<Vector3> CalculatePath(Vector3 to)
    {
        
        PriorityQueue<int, Camino> pq = new PriorityQueue<int, Camino>();
        bool[,] marcados = mapa.getMarcados();
        int[,] DistTo = mapa.getDistTo((int)transform.localPosition.y, (int)transform.localPosition.x);
        Camino inicial = new Camino();
        inicial.init(new Queue<Vector3>(), new Vector3(transform.localPosition.y, transform.localPosition.y),0);
        Camino path= new Camino();
        bool encontrado = false;
        pq.Enqueue(0,inicial);
        while (!encontrado)
        {
            Camino top = pq.Dequeue();
            if(top.Casilla().x == to.x && top.Casilla().x == to.x)
            {
                if (!encontrado)
                {
                    encontrado = true;
                    path = top;
                }
                else
                {
                    if (top.getCoste() < path.getCoste())
                    {
                        path = top;
                    }


                }

            }
            for (int i = 0; i < directions.Length; i++)
            {
                relax(ref pq, top, directions[i], ref marcados, ref DistTo);
            }

        }
        return path.GetQueue();

    }

}
