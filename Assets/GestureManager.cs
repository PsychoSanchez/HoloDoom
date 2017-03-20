using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GestureManager : MonoBehaviour
{
    public static RaycastHit HitInfo;
    public static Vector3 deltaVector;

    public static GestureManager Instance { get; private set; }

    public GameObject FocusedObject { get; private set; }

    GestureRecognizer recognizer;

    void Start()
    {
        Instance = this;

        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.ManipulationTranslate | GestureSettings.Tap);
        recognizer.TappedEvent += (source, tapCount, ray) =>
        {
            if (FocusedObject != null)
            {
                FocusedObject.SendMessageUpwards("OnSelect");
            }
        };

        recognizer.ManipulationStartedEvent += (s, v, r) =>
        {
            deltaVector = v;
            if (FocusedObject != null)
            {
                FocusedObject.SendMessageUpwards("StartManipulation");
            }
        };
        recognizer.StartCapturingGestures();
    }

    void Update()
    {
        GameObject oldFocusObject = FocusedObject;

        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        if (Physics.Raycast(headPosition, gazeDirection, out HitInfo))
        {
            FocusedObject = HitInfo.collider.gameObject;
        }
        else
        {
            FocusedObject = null;
        }

        if (FocusedObject != oldFocusObject)
        {
            recognizer.CancelGestures();
            recognizer.StartCapturingGestures();
        }
    }
}
