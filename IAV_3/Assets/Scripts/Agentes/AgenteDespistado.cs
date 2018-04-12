using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgenteDespistado : Agente {

    protected override void Explore()
    {
        if (!(cuchilloFound && fiambreFound))
        {
            List<Vector2Int> opciones = new List<Vector2Int>();
            for (int i = 0; i < frontera.Count; i++)
            {
                if (opciones.Count == 0)
                {
                    opciones.Add(frontera[i]);
                }
                else if (infoMapa[opciones[0].y, opciones[0].x].getRisk() == infoMapa[frontera[i].y, frontera[i].x].getRisk())
                {
                    opciones.Add(frontera[i]);

                }
                else if (infoMapa[opciones[0].y, opciones[0].x].getRisk() > infoMapa[frontera[i].y, frontera[i].x].getRisk())
                {
                    opciones.Clear();
                    opciones.Add(frontera[i]);

                }
            }
            GetComponent<PathFinder>().CalculatePath(opciones[Random.Range(0, opciones.Count )]);
        }
        else if (!casoResuelto)
        {

            GetComponent<PathFinder>().CalculatePath(posCasa);
            casoResuelto = true;
        }
    }
}
