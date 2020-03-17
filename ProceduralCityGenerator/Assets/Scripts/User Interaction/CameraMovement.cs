using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float _cameraSpeed = 30.0f;
    private Vector3 _movementDirection = Vector3.zero;

    private bool _cameraMoving = false;

    // Update is called once per frame
    private void Update()
    {
        CalculateMovementDir();

        if (_cameraMoving)
            transform.position = Vector3.MoveTowards(transform.position, transform.position + _movementDirection,
                _cameraSpeed * Time.deltaTime);

        ResetDir();
    }

    private void CalculateMovementDir()
    {
        if (Input.GetKey(KeyCode.W))
            _movementDirection += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            _movementDirection += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            _movementDirection += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            _movementDirection += Vector3.right;

        _movementDirection = _movementDirection.normalized;

        if (_movementDirection != Vector3.zero)
            _cameraMoving = true;
    }

    private void ResetDir()
    {
        _cameraMoving = false;
        _movementDirection = Vector3.zero;
    }
}
