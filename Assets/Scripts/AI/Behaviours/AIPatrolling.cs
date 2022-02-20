using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AIPatrolling : IBehaviour
{
    private AIController _aiController;

    public AIPatrolling(AIController aiController)
    {
        _aiController = aiController;
        _aiController.navMeshAgent.isStopped = false;
        _aiController.currentState = "Patrol";
    }

    public void UpdateStatus()
    {
        if (IsMovingToPoint()) return;

        SelectNewPosition();
    }

    bool IsMovingToPoint()
    {
        return _aiController.navMeshAgent.remainingDistance > 1f || 
            _aiController.navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid;
    }

    void SelectNewPosition()
    {
        /*
        //Snap to four directions. Doesnt look great
        var dir = Random.Range(0, 3);
        var direction = targetDirections[Random.Range(0, targetDirections.Length)];
        */

        var direction = Random.Range(0f, 360f);
        var distance = Random.Range(_aiController.roamingDistance.x, _aiController.roamingDistance.y);

        var targetPosition = _aiController._homePosition + (Quaternion.AngleAxis(direction, Vector3.up) * Vector3.forward * distance);

        if (!NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;

        _aiController.navMeshAgent.SetDestination(hit.position);
        _aiController.navMeshAgent.speed = _aiController.roamingSpeed;
        _aiController.navMeshAgent.isStopped = false;
    }

    public void CanSeePlayer(bool set)
    {
        if (!set) return;
        if (_aiController.isAgressive) _aiController.SetState<AIAttack>();
    }
}
