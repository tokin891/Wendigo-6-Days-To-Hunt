using UnityEngine;
using System.Collections.Generic;

public class AdultPoints : MonoBehaviour
{
    public List<Transform> _points;

    public Transform getNextTransform(int current)
    {
        if (current >= _points.Count - 1)
        {
            return _points[0];
        }
        else
            return _points[current + 1];
    }
    public int getNextTransformInt(int current)
    {
        int nextTransf = current;

        if (nextTransf >= _points.Count - 1)
        {
            return 0;
        }
        else
            return nextTransf + 1;
    }

    public Transform getRandomTransform()
    {
        int nextTransf = Random.Range(0, _points.Count - 1);

        return _points[nextTransf];
    }
}
