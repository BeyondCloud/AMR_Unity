using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class SimpleHttpServer : MonoBehaviour
{
    public GameObject player;
    private Navigation platerNavigation;
    private HttpListener listener;
    private bool isRunning = false;

    void Start()
    {
        StartServer();
        platerNavigation = player.GetComponent<Navigation>();
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
            listener.Prefixes.Add("http://localhost:8000/"); // Ensure this is correct
            listener.Start();
            isRunning = true;
            Listen();
            Debug.Log("Server started on http://localhost:8000/");
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
                    HandleMyFunction(request, response);
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

    private void HandleMyFunction(HttpListenerRequest request, HttpListenerResponse response)
    {
        // Process POST data for /goto
        using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
        {
            string postData = reader.ReadToEnd();
            Debug.Log($"Received POST data at /goto: {postData}");
            switch (postData.ToLower())
            {
                case "kitchen":
                    platerNavigation.places = Navigation.PlacesEmum.kitchen;
                    break;
                case "livingroom":
                    platerNavigation.places = Navigation.PlacesEmum.livingRoom;
                    break;
                case "bathroom":
                    platerNavigation.places = Navigation.PlacesEmum.bathroom;
                    break;
                default:
                    break;
            }
            // Process postData here as needed

            string responseString = $"/goto: {postData}";
            SendResponse(response, responseString, 200);
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