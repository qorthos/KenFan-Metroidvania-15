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
    [SerializeField]
    LayerMask _groundMask;
    int _jumpCheckLayerMask;

    bool jump = false;
    bool _isGrounded;
    Vector2 _currentDirection = Vector2.zero;
    Vector2 _characterSize;
    Rigidbody _rigidbody;
    CapsuleCollider _collider;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _characterSize = new Vector2(_collider.radius, _collider.height);
        _jumpCheckLayerMask = ~LayerMask.NameToLayer("Ground");
    }

    void FixedUpdate()
    {
        (bool isGrounded, bool onSlope, Vector3 slopeAngle, GameObject standingOn) = CheckIsGrounded();
        this._isGrounded = isGrounded;

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

        if (onSlope) newVelocity = Vector3.ProjectOnPlane(newVelocityGoal, slopeAngle);

        _rigidbody.velocity = newVelocity;
    }

    public void Move(Vector2 direction)
    {
        this._currentDirection = direction.normalized;

        if (Vector2.Distance(direction, Vector2.zero) <= 0f)
        {
            return;
        }

        transform.rotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, new Vector3(direction.x, 0, direction.y), Vector3.up), 0);
    }

    public void Jump()
    {
        if (this._isGrounded && _jumpTimer <= 0)
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

    private (bool, bool, Vector3, GameObject) CheckIsGrounded()
    {
        if (Physics.Raycast(groundCheck.position + new Vector3(0, 0.1f, 0), Vector3.down, out RaycastHit hit, 1f, _groundMask))
            return CalculateHitPosition(hit);

        //Walking forward
        if (Physics.Raycast(groundCheck.position + new Vector3(0, 0.1f, 0) + (transform.forward * (_characterSize.x * 0.5f)), Vector3.down, out hit, 1f, _groundMask))
            return CalculateHitPosition(hit);

        //Leaving the ground
        if (Physics.Raycast(groundCheck.position + new Vector3(0, 0.1f, 0) + (transform.forward * (_characterSize.x * -0.5f)), Vector3.down, out hit, 1f, _groundMask))
            return CalculateHitPosition(hit);

        return (false, false, Vector3.zero, null);
    }

    private (bool, bool, Vector3, GameObject) CalculateHitPosition(RaycastHit hit)
    {
        if (groundCheck.position.y - hit.point.y > 0.001f)
            return (false, false, Vector3.zero, null);

        var onSlope = Vector3.Distance(hit.normal, Vector3.up) > 0f;
        var slopeAngle = hit.normal;
        var groundTarget = hit.transform.gameObject;      //Footstep sounds, that kind of thing

        return (true, onSlope, slopeAngle, groundTarget);
    }

}
