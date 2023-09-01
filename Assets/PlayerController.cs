using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private Vector2 moveInput;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void physicsUpdate()
    {

    }

    void onMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}
