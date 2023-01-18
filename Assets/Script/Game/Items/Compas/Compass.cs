using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : CompassBehivour
{
    [SerializeField] Transform _targetCompass;
    public bool isOn;

    void Update()
    {
        if (!isOn)
            return;
    }
}
