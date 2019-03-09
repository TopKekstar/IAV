using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unidad : MonoBehaviour {
    Mapa mapa;
    Stack<Vector2Int> camino;
	Vector2Int from;
    GameObject _cross;
    Vector2Int pos;
    TextMesh forceHeuristic;
    PathFinder pathFinder;
    LineRendererController line;
    public GameObject linePrefab;
    Color unitColor;


    void Start () {
        mapa = transform.parent.gameObject.GetComponent<Mapa>();
        unitColor = new Color(Random.Range(.0f, 1.0f), Random.Range(.0f, 1.0f), Random.Range(.0f, 1.0f));
        GetComponent<SpriteRenderer>().color = unitColor;
        pathFinder = GetComponent<PathFinder>();
        forceHeuristic = GetComponentInChildren<TextMesh>();
        changeHeuristic();
		UpdatePos ();
        
        
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !GameManager.instance.editMode)
        {
            GameManager.instance.setCurrentUnit(gameObject);
        }
        if (Input.GetMouseButtonDown(1) && GameManager.instance.editMode)
        {
            GameManager.instance.borraUnidad(gameObject);
        }
        if (Input.GetMouseButtonDown(0) && GameManager.instance.editMode)
        {
            changeHeuristic();
        }
    }

    /// <summary>
    /// Changes the heuristic.
    /// </summary>
    private void changeHeuristic()
    {
        pathFinder.hard++;
        pathFinder.hard %= 11;
        forceHeuristic.text = pathFinder.hard.ToString();

    }
    /// <summary>
    /// Called when [destroy].
    /// </summary>
    private void OnDestroy()
    {
        Destroy(_cross);
        mapa.setOccupied(pos.y, pos.x, false);
    }

    /// <summary>
    /// Updates the position.
    /// </summary>
    void UpdatePos()
    {
        pos.x = (int)transform.localPosition.x;
        pos.y = (int)transform.localPosition.y;
        mapa.setOccupied(pos.y, pos.x, true);
    }

    /// <summary>
    /// Sets the path.
    /// </summary>
    /// <param name="c">The stack of the path.</param>
    public void SetPath(Stack<Vector2Int> c)
    { 
        line =  Instantiate(linePrefab, transform.parent).GetComponent<LineRendererController>();
        line.SetPath(new Stack<Vector2Int>(c));
        line.SetColorLine(unitColor);
        camino = c;
		from = camino.Peek ();
        followPath();
    }

    /// <summary>
    /// Moves to v.
    /// </summary>
    /// <param name="v">The positiont.</param>
    /// <param name="moveVel">The move vel.</param>
    /// <returns></returns>
    IEnumerator MoveTo(Vector3 v,float moveVel)
    {
        mapa.setOccupied((int)transform.localPosition.y, (int)transform.localPosition.x, false);
        while (Vector3.Distance(transform.position, v) > 0.001f) {
            transform.position = Vector3.MoveTowards(transform.position, v,0.05f/moveVel);
            yield return new WaitForEndOfFrame();
        }
        transform.position = Vector3.MoveTowards(transform.position, v, moveVel);

        UpdatePos();
        followPath();
    }

    /// <summary>
    /// Follows the path.
    /// </summary>
    public void followPath()
    {
        if (camino.Count > 0)
        {
            Vector2Int vector = camino.Pop();
			if (vector!= from&&mapa.getOccupied (vector.y, vector.x)) {
				while (camino.Count > 0) {
					vector = camino.Pop();
				}
				Destroy (_cross);
                Destroy(line.gameObject);
				GameManager.instance.setCurrentUnit (gameObject);
				GameManager.instance.mueveUnidad(mapa.getTile(vector.y,vector.x).gameObject);
            }
            else
			{
                IEnumerator coroutine = MoveTo(new Vector3(vector.x, vector.y, 0), mapa.getCostOfTile(vector.y, vector.x));

                StartCoroutine(coroutine);
				if (camino.Count == 0) {
					GameManager.instance.camara.ResetTarget ();
				}
			}
        }
        else{
            Destroy(_cross);
            Destroy(line.gameObject);
        }
    }
    public void setCross(ref GameObject cross)
    {
        _cross = cross;
        _cross.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
    }    
}
