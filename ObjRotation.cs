using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppKit;
using System.Text;

public class ObjRotation : MonoBehaviour
{
    SerialHandler serial;
    Vector3 rotation;
    public string portNum;
    public Transform rotationObj;
    public string massage;
//
    private Vector3 _dir;

    public float angle = 0.1f;

    public float speed = 0.1f;
    private GameObject player;
//

    enum MOVE_STATE
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }

    void Start()
    {
        serial = SerialHandler.Instance;
        bool success = serial.Open(portNum, SerialHandler.Baudrate.B_115200);
        if (!success)
        {
            return;
        }
        serial.OnDataReceived += SerialCallBack;
//
        player = GameObject.Find("Player") as GameObject;
//
//

    }
    void OnDisable()
    {
        serial.Close();
        serial.OnDataReceived -= SerialCallBack;
    }
    // Update is called once per frame
    void Update()
    {
        // ジャイロから重力の下向きのベクトルを取得。水平に置いた場合は、gravityV.zが-9.8になる.
        Vector3 gravityV = Input.gyro.gravity;

        // 外力のベクトルを計算.
        float scale = 0.01f;
        Vector3 forceV = new Vector3(rotation.y, 0.0f, -rotation.x) * scale;

        // m_sphereに外力を加える.
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(forceV);
    }
    void SerialCallBack(string m)
    {
        objMove(m);
        objRotation(m);
        massage = m;
    }

    void objRotation(string message)
    {
        string[] a;

        a = message.Split("="[0]);
        if (a.Length != 2) return;
        int v = int.Parse(a[1]);
        switch (a[0])
        {
            case "pitch":
                rotation = new Vector2(v, rotation.y);
                break;
            case "roll":
                rotation = new Vector2(rotation.x, v);
                break;
        }
        Quaternion AddRot = Quaternion.identity;
        AddRot.eulerAngles = new Vector3(-rotation.x, 0, -rotation.y);

        transform.rotation = AddRot;

    }

    void objMove(string message)
    {
        string[] a;

        a = message.Split("="[0]);
        if (a.Length != 2) return;
        int v = int.Parse(a[1]);
        string m = a[0];
        
        if (m == "button")
        {

            string mc = a[1];

            //a_button
            if (mc[0] == '1')
            {
                print("a_button");
            }
            //b_button
            if (mc[1] == '1')
            {
                print("b_button");
            }
            //          //y_button
            if (mc[2] == '1')
            {
                print("y_button");
                rotationObj.GetComponent<Renderer>().material.color = Color.blue;
            }
            //x_button
            if (mc[3] == '1')
            {
                print("x_button");
                rotationObj.GetComponent<Renderer>().material.color = Color.red;
            }
            //up
            if (mc[4] == '1')
            {
                print("up");
                transform.SetPositionAndRotation(transform.position + (Vector3.forward * 1), transform.rotation);
//                rotationObj.GetComponent<Renderer>().material.color = Color.green;
            }
            //dawn
            if (mc[5] == '1')
            {
                print("dawn");
                transform.SetPositionAndRotation(transform.position + (Vector3.back * 1), transform.rotation);
//                rotationObj.GetComponent<Renderer>().material.color = Color.magenta;
            }
            //left
            if (mc[6] == '1')
            {
                print("left");
                transform.SetPositionAndRotation(transform.position + (Vector3.left * 1), transform.rotation);
//                rotationObj.GetComponent<Renderer>().material.color = Color.yellow;
            }
            //right
            if (mc[7] == '1')
            {
                print("right");
                transform.SetPositionAndRotation(transform.position + (Vector3.right * 1), transform.rotation);
//                rotationObj.GetComponent<Renderer>().material.color = Color.cyan;
            }

        }
    }

}