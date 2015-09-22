using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;

namespace SoundCloud
{
	
public class SoundCloud : SingletonBehaviour<SoundCloud>
{
    private const string CONNECT_URL = "https://soundcloud.com/connect/";
    private const int LISTEN_PORT = 8080;

    public bool connected { get; private set; }

    protected IEnumerator Start()
    {
        //WWW www = new WWW();
        //yield return www;
        //SoundCloudTrack track = new SoundCloudTrack();
        //track.Deserialize(www.text);

        SoundCloudTrack track = null;
        string redirect = "";
        AudioClip clip = null;
        yield return StartCoroutine(WebRequest("http://api.soundcloud.com/resolve?url=" + "https://soundcloud.com/snakedrocks/paul-stanley" + "&client_id=" + SoundCloudConfig.CLIENT_ID, (retVal) => redirect = retVal.responseHeaders["LOCATION"]));
        yield return StartCoroutine(WebRequestObject<SoundCloudTrack>(redirect,
            (retVal) => track = retVal));
        //yield return StartCoroutine(WebRequestObject<SoundCloudTrack>("http://api.soundcloud.com/tracks/13158665?client_id=" + SoundCloudConfig.CLIENT_ID,
        //    (retVal) => track = retVal));
        yield return StartCoroutine(WebRequest(track.stream_url + "?client_id=" + SoundCloudConfig.CLIENT_ID, (retVal) => redirect = retVal.responseHeaders["LOCATION"]));
        yield return StartCoroutine(WebRequestAudioClip(redirect, true, (retVal) => clip = retVal));

        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();
    }

    public void Connect()
    {
    }

    private IEnumerator WebRequest(string uri, Action<WWW> callback)
    {
        WWW www = new WWW(uri);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
            yield break;
        }

        if (callback != null)
            callback(www);
    }

    private IEnumerator WebRequestObject<T>(string uri, Action<T> callback) where T : DataObject<T>, new()
    {
        using (WWW www = new WWW(uri))
        {
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.Log(www.error);
                yield break;
            }

            T target = new T();
            target.Deserialize(www.text);

            if (callback != null)
                callback(target);
        }
    }

    private IEnumerator WebRequestAudioClip(string uri, bool stream, Action<AudioClip> callback)
    {
        WWW www = new WWW(uri);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
            yield break;
        }

        if (callback != null)
            callback(www.GetAudioClip(false, false, AudioType.MPEG));
        
    }

    private IEnumerator AuthenticateUser() {
        string uriPrefix ="http://localhost:" + LISTEN_PORT + "/";
        string connectUrl = CONNECT_URL + "?";
        connectUrl += "client_id=" + SoundCloudConfig.CLIENT_ID;
        connectUrl += "&redirect_uri=" +  WWW.EscapeURL(uriPrefix + "unity-game-authentication");
        connectUrl += "&response_type=code";

        HttpListener listener = new HttpListener();
        listener.Prefixes.Add(uriPrefix);
        listener.Start();

        Thread authListener = new Thread(
            () =>
            {
                HttpListenerContext context = listener.GetContext();
                ProcessAuthRequest(context);
                listener.Stop();
            }
        );

        authListener.Start();
        Application.OpenURL(connectUrl);

        yield return StartCoroutine(WaitForAuthentication());

        yield break;
    }

    private void ProcessAuthRequest(HttpListenerContext context)
    {
        HttpListenerRequest req = context.Request;
        HttpListenerResponse res = context.Response;

        Debug.Log(req.Url);

        using (Stream outputStream = res.OutputStream)
        {
            string responseString = "<HTML><BODY>Authenticated! You can now return to your game.</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            outputStream.Write(buffer, 0, buffer.Length);
        }
    }

    private IEnumerator WaitForAuthentication()
    {
        // TODO
        yield break;
    }
}

}