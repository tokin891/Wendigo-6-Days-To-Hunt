using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothArm : MonoBehaviour
{
    public float amount;
    public float smootAmount;

    private float smootAmountAwake;
    private Vector3 initialPos;

    [SerializeField] private float xRot;
    [SerializeField] private float yRot;

    void Start()
    {
        initialPos = transform.localPosition;
        smootAmountAwake = smootAmount;
    }

    void Update()
    {
        Game.Player.CharacterMove controller = GetComponentInParent<Game.Player.CharacterMove>();
        if (controller.SomeWindowIsOpen())
            return;

        float MX = -Input.GetAxis("Mouse X") * amount;
        float MY = -Input.GetAxis("Mouse Y") * amount;

        if(controller.IsMoving)
        {
            xRot = MX;
            if(controller.IsRun)
            {
                yRot = -0.085f;
            }
            else
                yRot = -0.075f;

            smootAmount = smootAmountAwake * 1.5f;
            xRot = Mathf.Clamp(xRot, -0.06f, 0.06f);
        }
        else
        {
            xRot = MX;
            yRot = MY;

            xRot = Mathf.Clamp(xRot, -0.06f, 0.06f);
            yRot = Mathf.Clamp(yRot, -0.06f, 0.06f);

            smootAmount = smootAmountAwake;
        }

        Vector3 finalPos = new Vector3(xRot, yRot, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPos + initialPos, Time.deltaTime * smootAmount);
    }
}
