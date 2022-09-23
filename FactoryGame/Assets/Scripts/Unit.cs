// MoveTo.cs
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{

    public Transform goal;
    NavMeshAgent agent;
    Enemy enemy;

	void Start()
	{
        agent = GetComponent<NavMeshAgent>();
    }

	void Update()
    {
        agent.destination = goal.position;
        if (!agent.pathPending && agent.remainingDistance < 0.5f) // Arrived! (Attack)
            enemy.Attack();

        // Animation: https://docs.unity3d.com/Manual/nav-CouplingAnimationAndNavigation.html
    }
}