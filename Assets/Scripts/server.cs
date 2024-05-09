using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class SimpleHttpServer : MonoBehaviour
{
    public GameObject player;
    public PlayerFunctionCortroller controller;
    private HttpListener listener;
    private bool isRunning = false;

    // public string url = "http://localhost:8000/";

    // This will work on Windows, you need to turn off the firewall (local network only)
    // curl -X POST http://<your_IPv4>:8000/goto -d "kitchen"
    private string url = "http://192.168.0.225:8000/"; 
    // private string url = "http://0.0.0.0:8000/";
    
    // private string url = "http://36.228.19.33:8000/";
    

    void Start()
    {
        StartServer();
    }

    void OnApplicationQuit()
    {
        StopServer();
    }

    private void StartServer()
    {
        try
        {
            listener = new HttpListener();
            listener.Prefixes.Add(url); // Ensure this is correct
            listener.Start();
            isRunning = true;
            Listen();
            Debug.Log("Server started on /" + url);
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
                case "/goto":
                    Goto(request, response);
                    break;
                case "/go_forward":
                    GoForward();
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
        // Process POST data for /goto
        using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
        {
            string postData = reader.ReadToEnd();
            Debug.Log($"Received POST data at /goto: {postData}");
            var place = postData.ToLower();
            controller.Goto(place);
            string responseString = $"/goto: {postData}";
            SendResponse(response, responseString, 200);
        }
    }
    private void GoForward()
    {
        
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