using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mousePointer : MonoBehaviour
{
    public GameObject cursor;
    Camera Camera;

    // Start is called before the first frame update
    void Start()
    {
        Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Update_MousePosition();
    }

    private void Update_MousePosition()
    {
        Vector2 mousePos = Input.mousePosition;
        mousePos = Camera.ScreenToWorldPoint(mousePos);
        Vector3 pos = new Vector3 (mousePos.x, mousePos.y, -2f);
        cursor.transform.position = pos;
        cursor.transform.position = pos;
    }
}
