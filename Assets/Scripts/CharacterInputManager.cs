using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GenericCharacterController))]
public class CharacterInputManager : MonoBehaviour
{
    [SerializeField]
    readonly float inputRotation = -45f;

    InputManager _inputManager;
    GenericCharacterController _characterController;

    void Awake()
    {
        _inputManager = new InputManager();
        _characterController = GetComponent<GenericCharacterController>();
    }

    void OnEnable()
    {
        _inputManager.Player.Enable();
    }

    void OnDisable()
    {
        _inputManager.Player.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        _characterController.Move(_inputManager.Player.Move.ReadValue<Vector2>().Rotate(inputRotation));
        if(_inputManager.Player.Jump.IsPressed())
        {
            _characterController.Jump();
        }
    }
}
