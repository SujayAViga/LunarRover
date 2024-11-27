using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

[System.Serializable]
public class ControllerData
{
    public float r_x;
    public float r_y;
    public float l_x;
    public float l_y;
    public float command;
}

public class ChairController : MonoBehaviour
{
    UdpClient socket;
    IPEndPoint target;

    // The object to monitor for rotation
    public Transform monitoredObject;

    private float vib = 0; // Vibration or command placeholder

    void Start()
    {
        // Initialize UDP socket and target
        socket = new UdpClient(8051);
        target = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2000);

        // Check if monitoredObject is assigned
        if (monitoredObject == null)
        {
            Debug.LogError("ObjectRotationSender: No object assigned for monitoring.");
        }
    }

    void FixedUpdate()
    {
        // Get the monitored object's rotation in local space
        Vector3 rotation = monitoredObject.localEulerAngles;

        // Map rotation to the data structure
        float r_x = NormalizeAngle(rotation.x); // Map rotation x to r_x
        float r_y = NormalizeAngle(rotation.y); // Map rotation y to r_y
        float l_x = NormalizeAngle(rotation.z); // Map rotation z to l_x
        float l_y = 0f; // Placeholder for unused data

        // Send the rotation as UDP
        SendUDPMessage(r_x, r_y, l_x, l_y);
    }

    void SendUDPMessage(float r_x, float r_y, float l_x, float l_y)
    {
        // Populate the data object
        ControllerData data = new ControllerData
        {
            r_x = r_x,
            r_y = r_y,
            l_x = l_x,
            l_y = l_y,
            command = vib // Add vibration/command value
        };

        // Convert to JSON and send
        string jsonMessage = JsonUtility.ToJson(data);
        byte[] message = Encoding.UTF8.GetBytes(jsonMessage);
        socket.Send(message, message.Length, target);
    }

    // Normalize angles to stay within -180 to 180 degrees
    float NormalizeAngle(float angle)
    {
        return angle > 180 ? angle - 360 : angle;
    }

    // Public methods to control vibration value
    public void SetVibration(float value)
    {
        vib = value;
        Debug.Log("Vibration value set to: " + vib);
    }
}
