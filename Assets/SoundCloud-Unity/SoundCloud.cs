using System;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine;

namespace SoundCloud
{

[Persistent]
public class SoundCloud : SingletonBehaviour<SoundCloud>
{
    #region Constants & Enums

    public static readonly string WORKING_DIRECTORY =
        Application.temporaryCachePath + Path.DirectorySeparatorChar + "SoundCloud";

    private const string TEMP_FILENAME = "temp.mp3";

    #endregion

    #region Public Variables & Auto-Properties

    public bool initialized { get; private set; }

    #endregion

    #region Private Variables

    private SoundCloudWWW web;
    private SoundCloudTranscoder transcoder;

    #endregion

    #region Unity Events

    protected override void AwakeSingleton()
    {
        base.AwakeSingleton();
        initialized = false;
    }

    protected void Start()
    {
        if (!Directory.Exists(WORKING_DIRECTORY))
            Directory.CreateDirectory(WORKING_DIRECTORY);

        transcoder = gameObject.AddComponent<SoundCloudTranscoder>();
        web = gameObject.AddComponent<SoundCloudWWW>();

        initialized = true;
    }

    #endregion

    #region Public Methods

    public void Connect(Action<bool> callback)
    {
        web.AuthenticateUser(callback);
    }

    public void GetUser(string url, Action<SoundCloudUser> callback)
    {
        StartCoroutine(ProcessDataCall(url, callback));
    }

    public void GetUser(int id, Action<SoundCloudUser> callback)
    {
        GetUser(string.Format(SoundCloudUser.API_CALL, id, SoundCloudConfig.CLIENT_ID), callback);
    }

    public void GetTrack()
    {
    }

    public void GetPlaylist()
    {
    }

    public void GetGroup()
    {
    }

    public void GetComments()
    {
    }

    public void GetMe()
    {
    }

    public void GetMeConnections()
    {
    }

    public void GetMeActivities()
    {
    }

    #endregion

    #region Private Methods

    private bool IsAPI(string url)
    {
        return url.Contains("api.soundcloud.com");
    }

    private IEnumerator ProcessDataCall<T>(string url, Action<T> callback) where T : DataObject<T>, new()
    {
        T data = null;

        if (!IsAPI(url))
            yield return StartCoroutine(web.ProcessResolveURL(url, (success, resolvedURL) => { if (success) url = resolvedURL; }));

        if (IsAPI(url))
            yield return StartCoroutine(web.WebRequestObject<T>(url, (retVal) => data = retVal));

        if (callback != null)
            callback(data);
    }

    #endregion

    #region Properties

    public bool authenticated
    {
        get
        {
            return web.authenticated;
        }
    }

    #endregion
}

}