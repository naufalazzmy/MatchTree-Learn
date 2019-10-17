using System;
using UnityEngine;

public class PointOfInterestWithEvents : MonoBehaviour {
    public static event Action<PointOfInterestWithEvents> OnPointOfInterestEntered;

    [SerializeField]
    private string _poiname;

    public string Poiname { get => _poiname; }

    void OnDisable() {
        if (OnPointOfInterestEntered != null)
            OnPointOfInterestEntered(this);
    }
}