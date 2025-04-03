using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escapetrigger : MonoBehaviour
{
    public int detectorIndex; // Inspector에서 설정
    public EscapeDetecter Detectors;

    private void OnTriggerEnter(Collider other)
    {
        Detectors.OnPlayerEnteredDetector(other, detectorIndex);
    }
}
