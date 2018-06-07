using UnityEngine;

public class Flock : MonoBehaviour
{

    public float Speed;

    public float RotationSpeed;

    public float NeighbourDistance;

    void Start()
    {
        Speed = Random.Range(0.5f, 1.0f);
    }

    void Update()
    {
        bool turning = Vector3.Distance(transform.position, Vector3.zero) >= GlobalFlock.SceneSize;

        if (turning)
        {
            Vector3 direction = Vector3.zero - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), RotationSpeed * Time.deltaTime);

            Speed = Random.Range(0.5f, 1);
        }
        else
        {
            if (Random.Range(0, 5) < 1)
                ApplyRules();
        }

        //Mueve el pez hacia delante
        transform.Translate(0, 0, Time.deltaTime * Speed);
    }

    private void ApplyRules()
    {
        //Cada pez necesita información de todos los demás fantasmas
        GameObject[] allGhosts = GlobalFlock.AllGhosts;

        Vector3 center = Vector3.zero;  //Centro del grupo
        Vector3 avoid = Vector3.zero;   //Vector para evitar colisionar a otros fantasmas

        float groupSpeed = 0.1f;    //Velocidad del grupo

        Vector3 goalPos = GlobalFlock.GoalPos; //Posición a la que se dirigen

        float distance; //Aux

        int groupSize = 0; //Cuantos fantasmas están juntos a este

        foreach (GameObject ghost in allGhosts)
        {
            if (ghost != this.gameObject)
            {
                distance = Vector3.Distance(ghost.transform.position, transform.position);

                if (distance <= NeighbourDistance)
                {
                    center += ghost.transform.position;

                    if (distance < 0.2f)
                        avoid += (transform.position - ghost.transform.position);

                    //Calculamos la velocidad del grupo
                    groupSpeed += ghost.GetComponent<Flock>().Speed;

                    //Aumentamos el número de fantasmas en el grupo
                    groupSize++;
                }
            }

            //Si el fantasma está en un grupo
            if (groupSize > 0)
            {
                //Obtenemos el centro total del grupo y la velocidad total del grupo
                center = center / groupSize + (goalPos - transform.position);
                Speed = groupSpeed / groupSize;

                //Obtenemos la dirección del grupo y giramos al pez gradualmente
                Vector3 direction = (center + avoid) - transform.position;

                if (direction != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), RotationSpeed * Time.deltaTime);
            }

        }
    }
}
