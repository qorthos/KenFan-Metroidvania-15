using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [SerializeField] internal Transform player;
    [SerializeField] internal NavMeshAgent navMeshAgent;
    [SerializeField] internal Vector2 roamingDistance = new Vector2(5f, 20f);       //min-max
    [SerializeField] internal float roamingSpeed = 2f;
    [SerializeField] internal bool isAgressive;
    [SerializeField] internal float viewDistance = 20f;

    [Header("AI Debug")]
    [SerializeField] bool debugAi;
    //Debug data, just to see what the ai sees. Don't use these for anything other than seeing what it's seeing
    [SerializeField] internal string currentState;     
    [SerializeField] internal Vector2 playerData;       

    internal Vector3 _homePosition;

    IBehaviour _currentState;

    private void Awake()
    {
        if (navMeshAgent == null) navMeshAgent = GetComponent<NavMeshAgent>();

        _currentState = new AIPatrolling(this);

        _homePosition = transform.position;
    }

    void Start()
    {
        //if (player == null) player = GameObject.FindObjectOfType<CharacterInputManager>().transform;
        if (player == null) player = GameObject.Find("Test Player").transform;
    }

    void Update()
    {
        _currentState.UpdateStatus();
        _currentState.CanSeePlayer(IsPlayerVisible());
    }

    bool IsPlayerVisible()
    {
        if (debugAi)
        {
            var b = 1;
        }
        var distance = Vector3.Distance(transform.position, player.position);
        var angle = Vector3.Angle(transform.forward, (transform.position - player.position));
        playerData = new Vector2(distance, angle);

        if (distance > viewDistance) return false;
        if (angle < 90f) return false;

        //Raycast this maybe
        return true;
    }

    internal void SetState<T>() where T : IBehaviour
    {
        _currentState = (T)System.Activator.CreateInstance(typeof(T), new object[] { this });
    }
}
