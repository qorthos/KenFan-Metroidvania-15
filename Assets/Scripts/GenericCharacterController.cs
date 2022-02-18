using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GenericCharacterController : MonoBehaviour
{
    [SerializeField]
    float maxVelocity = 10f;
    [SerializeField]
    float accelerationPercentagePerSecond = 5f;
    [SerializeField]
    float jumpVelocity = 10f;
    [SerializeField]
    float jumpCooldown = 0.25f;
    float _jumpTimer = 0f;

    [SerializeField]
    Transform groundCheck;
    int _jumpCheckLayerMask;

    bool jump = false;
    Vector2 _currentDirection = Vector2.zero;
    Rigidbody _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _jumpCheckLayerMask = ~LayerMask.NameToLayer("Ground");
    }

    void FixedUpdate()
    {
        Vector3 newVelocityGoal = direction2DToDirection3D(_currentDirection) * maxVelocity;
        // Maintain vertical velocity from jumping/falling
        newVelocityGoal.y = this._rigidbody.velocity.y;
        // Transition toward new velocity at a fixed percentage of remaining difference per tick
        Vector3 newVelocity = Vector3.Lerp(this._rigidbody.velocity, newVelocityGoal, accelerationPercentagePerSecond * Time.deltaTime);
        // Get rid of weird tiny float differences and infinite lerping problems
        if(newVelocity.Approximately(newVelocityGoal))
        {
            newVelocity = newVelocityGoal;
        }

        if (jump)
        {
            jump = false;
            newVelocity.y = jumpVelocity;
            _jumpTimer = jumpCooldown;
        }
        else if (_jumpTimer > 0)
        {
            _jumpTimer -= Time.deltaTime;
        }

        _rigidbody.velocity = newVelocity;
    }

    public void Move(Vector2 direction)
    {
        this._currentDirection = direction.normalized;
    }

    public void Jump()
    {
        if (IsGrounded() && _jumpTimer <= 0)
        {
            jump = true;
        }
    }

    Vector3 direction2DToDirection3D(Vector2 direction)
    {
        return new Vector3(direction.x, 0, direction.y);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, 0.05f, _jumpCheckLayerMask);
    }
}
