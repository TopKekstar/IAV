using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PathFinder : MonoBehaviour {
    Mapa mapa;
    Vector3[] directions = { new Vector3(-1, 0), new Vector3(0, 1), new Vector3(1, 0), new Vector3(0, -1) };
	PriorityQueue pq;
	bool[,] marcados;
	int[,] DistTo;
	Vector3[,] EdgeTo;
	class PriorityQueue
    {
		private SortedDictionary<int, Queue<Vector3>> list = new SortedDictionary<int, Queue<Vector3>>();
		private bool[] contains = new bool[int.MaxValue];
		public void Enqueue(int priority, Vector3 value)
        {
            bool meter = true;
            if (meter)
            {
				Queue<Vector3> q;
                if (!list.TryGetValue(priority, out q))
                {
					contains [priority] = true;
					q = new Queue<Vector3>();
                    list.Add(priority, q);
					Debug.Log (value.ToString ());
                }
                q.Enqueue(value);
            }
        }
		public Vector3 Dequeue()
        {
            // will throw if there isn’t any first element!
			Vector3 v = Vector3.zero;
			if (!IsEmpty) {
				int Key = list.GetEnumerator ().Current.Key;
				while (!contains [Key]) {
					Key = list.GetEnumerator ().Current.Key;
				}
				Debug.Log (Key);
				Debug.Log (list [Key].Count);
				v = list [Key].Dequeue () * 1;
				if (list [Key].Count == 0) { // nothing left of the top priority.
					list.Remove (Key);
					contains [Key] = false;
				}
			}
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

	void relax(Vector3 origen, Vector3 direccion)
    {
		Vector3 destino = origen + direccion;
		if (!(destino.x < 0 || destino.y < 0 || destino.x >= mapa.getAncho () || destino.x >= mapa.getAncho ())) {
			if (!marcados [(int)destino.y, (int)destino.x]) {
				if (DistTo [(int)destino.y, (int)destino.x] > DistTo [(int)origen.y, (int)origen.x]+ mapa.getCostOfTile((int)destino.y, (int)destino.x)) {
					DistTo [(int)destino.y, (int)destino.x] = DistTo [(int)origen.y, (int)origen.x] + mapa.getCostOfTile ((int)destino.y, (int)destino.x);
					EdgeTo [(int)destino.y, (int)destino.x] = origen;
					pq.Enqueue (DistTo [(int)destino.y, (int)destino.x], destino*1);
				}
			}
		}
    }

   public void CalculatePath(Vector3 to)
    {
        
		pq = new PriorityQueue();
        marcados = mapa.getMarcados();
        DistTo = mapa.getDistTo((int)transform.localPosition.y, (int)transform.localPosition.x);
		EdgeTo = new Vector3[mapa.altoMapa, mapa.anchoMapa];
		pq.Enqueue (0, new Vector3(transform.localPosition.x,transform.localPosition.y));



		bool encontrado = false;

		while (!pq.IsEmpty)
        {
			Vector3 top = pq.Dequeue()*1;
            for (int i = 0; i < directions.Length; i++)
            {
                relax(top, directions[i]);
            }

        }
		Debug.Log (DistTo [(int)to.y, (int)to.x]);
    }

}
