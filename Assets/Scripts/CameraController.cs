using UnityEngine;

public class CameraController : MonoBehaviour {
    #region fields and properties
    public static CameraController Instance {get; set;}
    private Transform cam;
    private Transform parent;

    private Vector3 localRotation;
    private float camDistance = 11f;

    public float mouseSensitivity = 4f;
    public float scrollSensitivity = 2f;
    public float orbitDampening = 10f;
    public float scrollDampening = 6f;
    public float speed = 0.1f;

    public bool camDisabled = false;
    #endregion
    void Start() {
        Instance = this;
        cam = this.transform;
        parent = this.transform.parent;
    }

    // LateUpdate(), damit Kamera nach allem anderen gerendered wird
    void LateUpdate() {
        if (Input.GetKeyDown(KeyCode.LeftShift)) camDisabled = !camDisabled;

        if (!camDisabled) {
            // Rotation basierend auf Mauskoordinaten
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) {
                localRotation.x += Input.GetAxis("Mouse X") * mouseSensitivity;
                localRotation.y -= Input.GetAxis("Mouse Y") * mouseSensitivity;
                // Y-Rotation beschränken
                localRotation.y = Mathf.Clamp(localRotation.y, -40f, 40f);
            }

            // Zoom basierend auf Mausrad
            if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
                float scrollAmount = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
                // je weiter weg, desto schneller ist der Zoom
                scrollAmount *= camDistance * 0.3f;
                camDistance += scrollAmount * -1f;
                // beschränkt den Zoom auf 1.5m und 25m
                camDistance = Mathf.Clamp(camDistance, 1.5f, 25f);
            }

            // Quaternion (um Gimbal Lock zu vermeiden), das Rotation auf z-Achse verhindert
            Quaternion q = Quaternion.Euler(localRotation.y, localRotation.x, 0);
            // Lineare Interpolation, die die Bewegung zwischen der aktuellen Rotation und der Ziel-Rotation animiert
            parent.rotation = Quaternion.Lerp(parent.rotation, q, Time.deltaTime * orbitDampening);

            if (cam.localPosition.z != camDistance * -1f) {
                // noch eine Interpolation, um zwischen aktueller z-Position und Ziel-z-Position animiert
                cam.localPosition = new Vector3(0f, 0f, Mathf.Lerp(cam.localPosition.z, camDistance * -1f, Time.deltaTime * scrollDampening));
            }
        }

        if (GameState.Instance.changePerspective) {
            camDisabled = true;
            parent.Rotate(0, 180 * speed * Time.deltaTime, 0);
            if (GameState.Instance.isWhiteTurn && parent.eulerAngles.y > 0 && parent.eulerAngles.y < 180) {
                parent.rotation = Quaternion.Euler(0,0,0);
                GameState.Instance.changePerspective = false; 
            }
            
            else if (!GameState.Instance.isWhiteTurn && parent.eulerAngles.y > 180) {
                parent.rotation = Quaternion.Euler(0,180,0);
                GameState.Instance.changePerspective = false; 
            }
        }
    }
}
