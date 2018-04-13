using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgenteMedio : Agente {

    private int DistanciaDelCentro(Vector2Int vector2Int)
    {
        return System.Math.Abs(vector2Int.x - mapa.anchoMapa / 2) + System.Math.Abs(vector2Int.y - mapa.altoMapa / 2);
    }
    // Use this for initialization
    protected override void Explore()
    {
        if (!(cuchilloFound && fiambreFound))
        {
            Vector2Int bestOption = new Vector2Int(-1, -1);
            for (int i = 0; i < frontera.Count; i++)
            {
                if (bestOption.x == -1)
                {
                    bestOption = frontera[i];
                }
                else if(infoMapa[bestOption.y, bestOption.x].getRisk() == infoMapa[frontera[i].y, frontera[i].x].getRisk())
                {
                    if(DistanciaDelCentro(bestOption)> DistanciaDelCentro(frontera[i]))
                        bestOption = frontera[i];
                    else if (DistanciaDelCentro(bestOption) == DistanciaDelCentro(frontera[i]))
                    {
                        if((bestOption.x == 0 && frontera[i].x != 0)||(bestOption.y == 0 && frontera[i].y != 0))
                            bestOption = frontera[i];
                        if ((bestOption.x == mapa.anchoMapa-1 && frontera[i].x != mapa.anchoMapa-1) || (bestOption.y == mapa.altoMapa-1 && frontera[i].y != mapa.altoMapa-1))
                            bestOption = frontera[i];
                        if (distanciaJugador(bestOption) > distanciaJugador(frontera[i]))
                            bestOption = frontera[i];

                    }

                }
                else if (infoMapa[bestOption.y, bestOption.x].getRisk() > infoMapa[frontera[i].y, frontera[i].x].getRisk())
                {
                    bestOption = frontera[i];

                }
            }
            GetComponent<PathFinder>().CalculatePath(bestOption);
        }
        else if (!casoResuelto)
        {
            GetComponent<PathFinder>().CalculatePath(posCasa);
            casoResuelto = true;
        }
    }
}
