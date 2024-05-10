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
        go_forward = 1,
        go_back = 2,
        go_right = 3,
        go_left = 4,
        go_crowded = 5,
        _goto = 6,
        spin_right = 7,
        spin_left = 8,
        follow = 9,
        echo_seen_object = 10,
        get_battery_percentage = 11,
        get_speed = 12,
        dance = 13,
        find = 14,
        set_speed = 15,
        print = 16

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
    void Update()
    {
        if (flag == FuncEnum.idle)
            return;
        else
        {
            switch (flag)
            {
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
                    controller.SpinRight();
                    break;
                case FuncEnum.spin_left:
                    controller.SpinLeft();
                    break;
                case FuncEnum.follow:
                    controller.Follow();
                    break;
                case FuncEnum.echo_seen_object:
                    controller.EchoSeenObjects();
                    break;
                case FuncEnum.get_battery_percentage:
                    controller.GetBatteryPercentage();
                    break;
                case FuncEnum.get_speed:
                    controller.GetSpeedLevel();
                    break;
                case FuncEnum.dance:
                    controller.Dance();
                    break;
                case FuncEnum.find:
                    controller.Find(funcCallArg);
                    break;
                case FuncEnum.set_speed:
                    controller.SetSpeedLevel(Int32.Parse(funcCallArg));
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
        Debug.Log("Request received");
        if (!isRunning) return;

        HttpListenerContext context = listener.EndGetContext(result);
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        // Determine action based on the URL and method
        if (request.HttpMethod == "POST")
        {
            switch (request.Url.AbsolutePath)
            {
                case "/go_forward":
                    flag = FuncEnum.go_forward;
                    break;
                case "/go_back":
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
                    controller.Follow();
                    break;
                case "/echo_seen_object":
                    flag = FuncEnum.echo_seen_object;
                    break;
                case "/get_battery_percentage":
                    flag = FuncEnum.get_battery_percentage;
                    break;
                case "/get_speed":
                    flag = FuncEnum.get_speed;
                    break;
                case "/dance":
                    flag = FuncEnum.dance;
                    break;
                case "/find":
                    flag = FuncEnum.find;
                    funcCallArg = GetPostData(request).ToLower();
                    break;
                case "/goto":
                    flag = FuncEnum._goto;
                    funcCallArg = GetPostData(request).ToLower();
                    break;
                case "/set_speed":
                    flag = FuncEnum.set_speed;
                    funcCallArg = GetPostData(request).ToLower();
                    break;
                case "/print":
                case "/error":
                    flag = FuncEnum.print;
                    break;
                default:
                    SendResponse(response, "404 Not Found", 404);
                    break;
            }
        }
        string responseString = $"{request.Url.AbsolutePath}: {funcCallArg}";
        SendResponse(response, responseString, 200);
        // Continue listening for incoming requests
        Listen();
    }
    string GetPostData(HttpListenerRequest request)
    {
        using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
        {
            string postData = reader.ReadToEnd();
            Debug.Log($"Received POST data at /goto: {postData}");
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