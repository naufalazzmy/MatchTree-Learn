using UnityEngine;

public class PointOfInterest : Subject {

    [SerializeField]
    private string _poiName;

    
    private void OnDisable() {
        Notify(_poiName);
    }
}