using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class SimpleHttpServer : MonoBehaviour
{
    public enum FuncEnum
    {
        idle = 0,
        stop = 1,
        go_forward = 2,
        go_back = 3,
        go_right = 4,
        go_left = 5,
        go_crowded = 6,
        _goto = 7,
        spin_right = 8,
        spin_left = 9,
        follow = 10,
        echo_seen_objects = 11,
        get_power = 12,
        get_speed = 13,
        dance = 14,
        find = 15,
        set_speed = 16,
        print = 17

    }
    public PlayerFunctionCortroller controller;
    private HttpListener listener;
    private bool isRunning = false;

    // public string url = "http://localhost:8000/";

    // This will work on Windows, you need to turn off the firewall (local network only)
    // curl -X POST http://<your_IPv4>:8000/goto -d "kitchen"
    public string ip = "192.168.0.225";
    public string port = "8000";
    void Start()
    {
        string url = $"http://{ip}:{port}/";
        StartServer(url);
    }
    private FuncEnum flag = 0;
    private string funcCallArg = "";
    private string returnJsonString = "{\"data\":null}";
    void Update()
    {
        if (flag == FuncEnum.idle)
            return;
        else
        {
            string string_holder = "";
            switch (flag)
            {
                case FuncEnum.stop:
                    controller.Reset();
                    break;
                case FuncEnum.go_forward:
                    controller.GoForward();
                    break;
                case FuncEnum.go_back:
                    controller.GoBack();
                    break;
                case FuncEnum.go_right:
                    controller.GoRight();
                    break;
                case FuncEnum.go_left:
                    controller.GoLeft();
                    break;
                case FuncEnum.go_crowded:
                    controller.GoCrowded();
                    break;
                case FuncEnum._goto:
                    controller.Goto(funcCallArg);
                    break;
                case FuncEnum.spin_right:
                    controller.Rotate(180);
                    break;
                case FuncEnum.spin_left:
                    controller.Rotate(-180);
                    break;
                case FuncEnum.follow:
                    controller.Follow();
                    break;
                case FuncEnum.echo_seen_objects:
                    controller.EchoSeenObjects();
                    break;
                case FuncEnum.get_speed:
                    string_holder = controller.GetSpeedLevel().ToString();
                    returnJsonString = "{\"data\":" + string_holder + "}";
                    break;
                case FuncEnum.get_power:
                    string_holder = controller.GetBatteryPercentage().ToString();
                    returnJsonString = "{\"data\":" + string_holder + "}";
                    break;
                case FuncEnum.dance:
                    controller.Dance();
                    break;
                case FuncEnum.find:
                    controller.Find(funcCallArg);
                    break;
                case FuncEnum.set_speed:
                    controller.SetSpeedLevel(int.Parse(funcCallArg));
                    break;
                case FuncEnum.print:
                    Debug.Log(funcCallArg);
                    break;
            }
            flag = FuncEnum.idle;
        }
    }
    void OnApplicationQuit()
    {
        StopServer();
    }
    public void Goto(string place)
    {
        controller.Goto(place);
    }
    private void StartServer(string url = "http://localhost:8000/")
    {
        try
        {
            listener = new HttpListener();
            listener.Prefixes.Add(url); // Ensure this is correct
            listener.Start();
            isRunning = true;
            Listen();
            Debug.Log("Server started: " + url);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to start server: {e.Message}");
        }
    }

    private void Listen()
    {
        listener.BeginGetContext(new AsyncCallback(OnRequestReceived), listener);
    }

    private void OnRequestReceived(IAsyncResult result)
    {
        if (!isRunning) return;

        HttpListenerContext context = listener.EndGetContext(result);
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;
        int returnCode = 200;
        // Determine action based on the URL and method
        if (request.HttpMethod == "POST")
        {
            returnJsonString = "{\"data\":null}";
            switch (request.Url.AbsolutePath)
            {
                case "/stop":
                    flag = FuncEnum.stop;
                    break;
                case "/go_forward":
                    flag = FuncEnum.go_forward;
                    break;
                case "/go_back":
                case "/go_backward":
                    flag = FuncEnum.go_back;
                    break;
                case "/go_right":
                    flag = FuncEnum.go_right;
                    break;
                case "/go_left":
                    flag = FuncEnum.go_left;
                    break;
                case "/go_crowded":
                    flag = FuncEnum.go_crowded;
                    break;
                case "/go_charge":
                    flag = FuncEnum._goto;
                    funcCallArg = "charge_station";
                    break;
                case "/spin_right":
                    flag = FuncEnum.spin_right;
                    break;
                case "/spin_left":
                    flag = FuncEnum.spin_left;
                    break;
                case "/follow":
                    flag = FuncEnum.follow;
                    break;
                case "/echo_seen_objects":
                    flag = FuncEnum.echo_seen_objects;
                    break;
                case "/get_power":
                    flag = FuncEnum.get_power;
                    while (flag != FuncEnum.idle) {}
                    break;
                case "/get_speed":
                    flag = FuncEnum.get_speed;
                    while (flag != FuncEnum.idle) {}
                    break;
                case "/dance":
                    flag = FuncEnum.dance;
                    break;
                case "/find":
                    funcCallArg = GetPostData(request).ToLower();
                    flag = FuncEnum.find; // Make sure flag is set last to avoid race condition
                    break;
                case "/goto":
                    funcCallArg = GetPostData(request).ToLower();
                    flag = FuncEnum._goto; // Make sure flag is set last to avoid race condition
                    break;
                case "/set_speed":
                    funcCallArg = GetPostData(request).ToLower();
                    flag = FuncEnum.set_speed; // Make sure flag is set last to avoid race condition
                    break;
                case "/print":
                case "/error":
                    funcCallArg = GetPostData(request);
                    flag = FuncEnum.print; // Make sure flag is set last to avoid race condition
                    break;
                default:
                    returnJsonString = "{\"error\":\"404 Not Found\"}";
                    returnCode = 404;
                    break;
            }
        }
        SendResponse(response, returnJsonString, returnCode);

        // Continue listening for incoming requests
        Listen();
    }
    string GetPostData(HttpListenerRequest request)
    {
        using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
        {
            string postData = reader.ReadToEnd();
            return postData;
        }
    }
    private void SendResponse(HttpListenerResponse response, string responseString, int statusCode)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.StatusCode = statusCode;
        response.ContentLength64 = buffer.Length;
        using (Stream output = response.OutputStream)
        {
            output.Write(buffer, 0, buffer.Length);
        }
    }

    private void StopServer()
    {
        if (listener == null)
            return;

        listener.Stop();
        listener.Close();
        isRunning = false;
        Debug.Log("Server stopped.");
    }
}