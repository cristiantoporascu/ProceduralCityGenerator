using UnityEngine;

public class RoadGenerationUI : MonoBehaviour
{
    private GameObject _mainCamera;
    private Camera _cameraComponent;

    private GameObject _manager;

    // Start is called before the first frame update
    private void Start()
    {
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        _manager = GameObject.FindGameObjectWithTag("Manager");

        if (_mainCamera != null)
        {
            _cameraComponent = _mainCamera.GetComponent<Camera>();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (CameraFound())
        {
           LeftClickEvent();
        }
    }

    private bool CameraFound()
    {
        return _cameraComponent != null;
    }

    private void LeftClickEvent()
    {
        Ray mouseRay = _cameraComponent.ScreenPointToRay(Input.mousePosition);
        Vector3 mousePos = Vector3.zero;

        if (mouseRay.direction.y != 0)
        {
            float dToXZPlane = Mathf.Abs(mouseRay.origin.y / mouseRay.direction.y);
            mousePos = mouseRay.GetPoint(dToXZPlane);
        }

        if (Input.GetMouseButtonDown(0))
        {
            _manager.GetComponent<RoadGenerator>().AddPoints(mousePos);
        }
    }
}
