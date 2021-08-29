using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControllerMouseKeyboard : InputController
{
    public string positionAxisCodeX = "Horizontal";
    public string positionAxisCodeY = "Vertical";
    [Space]
    public string directionAxisCodeX = "Mouse X";
    public string directionAxisCodeY = "Mouse Y";
    [Space]
    public string[] keyAxisCode = new string[4];

    Camera cam;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        cam = Camera.main;
    }

    private void Update()
    {
        //Debug.Assert(keyAxisCode.Length >= inputHolder.keys.Length);

        

        /// position
        inputHolder.positionInput.x = Input.GetAxisRaw(positionAxisCodeX);
        inputHolder.positionInput.y = Input.GetAxisRaw(positionAxisCodeY);

        /// direction
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        var box = GetComponent<BoxCollider>();

        if (box && box.Raycast(ray, out hit, float.PositiveInfinity))
        {
            Vector3 v = hit.point;
            inputHolder.directionInput = new Vector2(v.x - inputHolder.transform.position.x, v.z - inputHolder.transform.position.z);
        }
        else
            Debug.LogWarning("player direction raycast does not hit a collider");

        /// keys
        for (int i = 0; i < inputHolder.keys.Length; ++i)
        {
            inputHolder.keys[i] = Mathf.Abs(Input.GetAxisRaw(keyAxisCode[i])) > 0.1f;
        }
    }
}
