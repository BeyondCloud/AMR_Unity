using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class SimpleHttpServer : MonoBehaviour
{
    public PlayerFunctionCortroller controller;
    private HttpListener listener;
    private bool isRunning = false;

    // public string url = "http://localhost:8000/";

    // This will work on Windows, you need to turn off the firewall (local network only)
    // curl -X POST http://<your_IPv4>:8000/goto -d "kitchen"
    public string ip="192.168.0.225";
    public string port="8000";
    void Start()
    {
        string url = $"http://{ip}:{port}/";
        StartServer(url);
    }

    void OnApplicationQuit()
    {
        StopServer();
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

        // Determine action based on the URL and method
        if (request.HttpMethod == "POST")
        {
            switch (request.Url.AbsolutePath)
            {
                case "/go_forward":
                    controller.GoForward();
                    break;
                case "/go_back":
                    controller.GoBack();
                    break;
                case "/go_right":
                    controller.GoRight();
                    break;
                case "/go_left":
                    controller.GoLeft();
                    break;
                case "/go_crowded":
                    controller.GoCrowded();
                    break;
                case "/go_charge":
                    controller.Goto("charge");
                    break;
                case "/spin_right":
                    controller.SpinRight();
                    break;
                case "/spin_left":
                    controller.SpinLeft();
                    break;
                case "/follow":
                    controller.Follow();
                    break;
                case "/echo_seen_object":
                    controller.EchoSeenObjects();
                    break;
                case "/get_battery_percentage":
                    controller.GetBatteryPercentage();
                    break;
                case "/get_speed":
                    controller.GetSpeedLevel();
                    break;
                case "/dance":
                    controller.Dance();
                    break;
                case "/find":
                    Find(request, response);
                    break;
                case "/goto":
                    Goto(request, response);
                    break;
                case "/set_speed":
                    SetSpeed(request, response);
                    break;
                case "/print":
                case "/error":
                    Debug.Log(GetPostData(request));
                    break;
                default:
                    SendResponse(response, "404 Not Found", 404);
                    break;
            }
        }
        else
        {
            SendResponse(response, " Only POST /goto is supported.</BODY></HTML>", 200);
        }

        // Continue listening for incoming requests
        Listen();
    }

    private void Goto(HttpListenerRequest request, HttpListenerResponse response)
    {
        string place = GetPostData(request).ToLower();
        controller.Goto(place);
        string responseString = $"/goto: {place}";
        SendResponse(response, responseString, 200);
    }
    private void Find(HttpListenerRequest request, HttpListenerResponse response)
    {
        string obj = GetPostData(request).ToLower();
        controller.Find(obj);
        string responseString = $"/find: {obj}";
        SendResponse(response, responseString, 200);
    }
    private void SetSpeed(HttpListenerRequest request, HttpListenerResponse response)
    {
        int speedLevel = Int32.Parse(GetPostData(request));
        controller.SetSpeedLevel(speedLevel);
        string responseString = $"/setSpeedLevel: {speedLevel}";
        SendResponse(response, responseString, 200);
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