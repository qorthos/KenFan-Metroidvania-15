using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WeightedGravity : MonoBehaviour
{
    [SerializeField]
    [Min(1)]
    float _normalMultiplier = 2f;
    [SerializeField]
    [Min(1)]
    float _fallMultiplier = 2.5f;

    Rigidbody _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        _rigidbody.velocity += Vector3.up * Physics.gravity.y * ((_rigidbody.velocity.y < 0 ? _fallMultiplier : _normalMultiplier) - 1) * Time.deltaTime;
    }
}
