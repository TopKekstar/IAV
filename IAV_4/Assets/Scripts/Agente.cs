using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agente : MonoBehaviour {

   
    public enum Agent_Status
    {
        SLEEPING, EXPLORING, GOING_HOME
    }


    
    protected Agent_Status status;


    protected Mapa mapa;
    protected Stack<Vector2Int> camino;
    protected Vector2Int from;
    protected Vector2Int pos;
    protected bool cuchilloFound;
    protected bool fiambreFound;
    protected bool casoResuelto;

    // Use this for initialization
    protected void Start()
    {
        status = Agent_Status.SLEEPING;
		cuchilloFound = false;
		fiambreFound = false;
        casoResuelto = false;
        mapa = GetComponentInParent<Mapa>();
        
        updatePos();
        mapa.getTile(pos.y, pos.x).SetDescubierta(true);
    }

    // Update is called once per frame
    protected void Update()
    {
       
    }

    protected int distanciaJugador(Vector2Int vector2Int)
    {
        return System.Math.Abs(vector2Int.x - pos.x) + System.Math.Abs(vector2Int.y - pos.y);
    }

    protected virtual void Explore()
    {
        
    }


    protected void OnDestroy()
    {
    }
    protected void updatePos()
    {
        pos.x = (int)transform.localPosition.x;
        pos.y = (int)transform.localPosition.y;        
    }
    public void setPath(Stack<Vector2Int> c)
    {
		
        camino = c;
        from = camino.Peek();
        followPath();

    }
    protected void moveTo(Vector3 v)
    {
        transform.Translate(v - transform.localPosition, transform);
        
        updatePos();
    }
    public void followPath()
    {
        if (camino.Count > 0)
        {
            Vector2Int vector = camino.Pop();


            {

                moveTo(new Vector3(vector.x, vector.y, 0));
                if (camino.Count == 0)
                {
                    Explore();
                    mapa.getTile(pos.y, pos.x).SetDescubierta(true);
                }
                else
                {
                    Invoke("followPath", 0.5f);
                }
                
            }

        }
    }
    public void setCross(ref GameObject cross)
    {
      
    }

   
}
