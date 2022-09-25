// MoveTo.cs
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{

    public Transform goal;
    public bool dangerous = true;
    NavMeshAgent agent;
    Enemy enemy;

	void Start()
	{
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<Enemy>();
    }

	void Update()
    {
        agent.destination = goal.position;
        if (!agent.pathPending && agent.remainingDistance < 3f) // Arrived! (Attack)
		{
            if (dangerous)
                enemy.Attack();

            // Put animation code here
        }

        // Animation: https://docs.unity3d.com/Manual/nav-CouplingAnimationAndNavigation.html
    }
}