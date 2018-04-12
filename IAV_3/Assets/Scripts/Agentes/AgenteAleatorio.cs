using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgenteAleatorio : Agente {

    // Use this for initialization
    protected override void Explore()
    {
        if (!(cuchilloFound && fiambreFound))
        {
            Vector2Int bestOption = frontera[Random.Range(0,frontera.Count)];            
            GetComponent<PathFinder>().CalculatePath(bestOption);
        }
        else if (!casoResuelto)
        {
            GetComponent<PathFinder>().CalculatePath(posCasa);
            casoResuelto = true;
        }
    }
}
