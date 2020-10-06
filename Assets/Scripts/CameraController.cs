using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour {
    #region fields and properties
    public static CameraController Instance {get; set;}
    private Transform cam;
    private Transform parent;

    private Vector3 localRotation;
    private float camDistance = 16f;

    public float mouseSensitivity = 4f;
    public float scrollSensitivity = 2f;
    public float orbitDampening = 10f;
    public float scrollDampening = 6f;
    public float speed = 0.1f;

    public bool camDisabled = false;
    public bool xAligned = false;
    public bool yAligned = false;
    #endregion
    void Start() {
        Instance = this;
        cam = this.transform;
        parent = this.transform.parent;
        localRotation.y = 30;
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
                localRotation.y = Mathf.Clamp(localRotation.y, 10f, 90f);
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
            /*
            if (xAligned && yAligned) GameState.Instance.changePerspective = false;

            #region x-Achse
            if (!xAligned) {
                if (Mathf.Floor(parent.eulerAngles.x) == 30) {
                    parent.rotation = Quaternion.Euler(30,parent.rotation.y,0);
                    localRotation = new Vector3(localRotation.y,30,0);
                    xAligned = true;
                }
                else {
                    // Space.World immens wichtig
                    parent.Rotate(180 * speed * Time.deltaTime, 0, 0, Space.World);
                }
            }
            #endregion
            */

            #region y-Achse
            //else if (!yAligned) {
                // Space.World immens wichtig
                parent.Rotate(0, 180 * speed * Time.deltaTime, 0, Space.World);
                StartCoroutine("StopCamera");
            //}
            #endregion
        }
    }

    IEnumerator StopCamera() {
        yield return new WaitForSeconds(1);
        // von schwarz nach weiß
        if (GameState.Instance.isWhiteTurn && Mathf.Floor(parent.eulerAngles.y) == 0 || Mathf.Ceil(parent.eulerAngles.y) == 0) {
            parent.rotation = Quaternion.Euler(30,0,0);
            localRotation = new Vector3(0,30,0);
            //yAligned = true;
            GameState.Instance.changePerspective = false;
        }
        // von weiß nach schwarz
        else if (!GameState.Instance.isWhiteTurn && Mathf.Floor(parent.eulerAngles.y) == 180 || Mathf.Ceil(parent.eulerAngles.y) == 180) {
            parent.rotation = Quaternion.Euler(30,180,0);
            localRotation = new Vector3(180,30,0);
            //yAligned = true;
            GameState.Instance.changePerspective = false;
        }
    }
}
