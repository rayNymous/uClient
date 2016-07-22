using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;


using UnityEngine.UI;

public class NetworkController : MonoBehaviour
{
    public static string serverName = "127.0.0.1";
    public static int serverPort = 3333;

    static TcpClient _client;
    static BinaryReader _reader;
    static BinaryWriter _writer;
    static Thread networkThread;

    private static Queue<Message> _messageQueue = new Queue<Message>();


    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start ()
    {
        StartServer();
    }
	
	// Update is called once per frame
	void Update ()
	{
	    ProcessMessage();
	}

    static void AddItemToQueue(Message item)
    {
        lock (_messageQueue)
        {
            _messageQueue.Enqueue(item);
        }
    }

    static Message GetItemFromQueue()
    {
        lock (_messageQueue)
        {
            if (_messageQueue.Count > 0)
            {
                return _messageQueue.Dequeue();
            }
            else
            {
                return null;
            }
        }
    }

    static void ProcessMessage()
    {
        Message msg = GetItemFromQueue();
        if (msg != null)
        {
            Debug.LogWarning("message received: " + Message.GetString(msg.Content).ToString());
            //process current message is here
        }
    }

    public static void SendMessage(Message msg)
    {
        msg.WriteToStream(_writer);
        _writer.Flush();
    }

    static void StartServer()
    {
        if (networkThread == null)
        {
            Connect();
            networkThread = new Thread(() => {
                                                 while (_reader != null)
                                                 {
                                                     Message msg = Message.ReadFromStream(_reader);
                                                     AddItemToQueue(msg);
                                                 }
                                                 lock (networkThread)
                                                 {
                                                     networkThread = null;
                                                 }
            });
            networkThread.Start();
        }
    }

    static void Connect()
    {
        if (_client == null)
        {
            _client = new TcpClient(serverName, serverPort);
            Stream stream = _client.GetStream();
            _reader = new BinaryReader(stream);
            _writer = new BinaryWriter(stream);

            Debug.Log("Connected <3");
        }
    }

    public void SendMessageString()
    {
        GameObject go = GameObject.FindGameObjectWithTag("INPUT");
        string s = (go.transform.GetComponent<Text>()).text;


        Debug.Log("trying to send message with text: "+ s);
        byte[] bytes = Message.GetBytes(s);
        Message m = new Message(bytes);
        SendMessage(m);
        Debug.Log("message: " + s + " send. love chu <3");
    }
}
