using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttack : IBehaviour
{
    internal AIController _aiController;

    public AIAttack(AIController aiController)
    {
        _aiController = aiController;
        _aiController.currentState = "Attack";
    }

    public void UpdateStatus()
    {
        //This should probably be a CanSeePlayer thing
        var distance = Vector3.Distance(_aiController.transform.position, _aiController.player.position);
        if (distance > _aiController.viewDistance)
        {
            _aiController.SetState<AIPatrolling>();
            return;
        }

        _aiController.navMeshAgent.isStopped = true;
    }

    public void CanSeePlayer(bool set)
    {
        //If cant see, look around for them a bit, then give up. For now, just go back to what they were doing

        if (!set) _aiController.SetState<AIPatrolling>();
    }
}
