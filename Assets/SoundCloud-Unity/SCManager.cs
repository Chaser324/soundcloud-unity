using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace SoundCloud
{

[Persistent]
public class SCManager : SingletonBehaviour<SCManager>
{
    #region Constants & Enums

    public static readonly string WORKING_DIRECTORY =
        Application.temporaryCachePath + Path.DirectorySeparatorChar + "SoundCloud";

    private const string TEMP_FILENAME = "temp.mp3";

    #endregion

    #region Public Variables & Auto-Properties

    

    #endregion

    #region Private Variables

    private bool _initialized = false;

    private SoundCloudWWW web;
    private SoundCloudTranscoder transcoder;
    private SoundCloudCache cache;

    #endregion

    #region Unity Events

    protected void Start()
    {
        if (!Directory.Exists(WORKING_DIRECTORY))
            Directory.CreateDirectory(WORKING_DIRECTORY);

        transcoder = gameObject.AddComponent<SoundCloudTranscoder>();
        web = gameObject.AddComponent<SoundCloudWWW>();
        cache = new SoundCloudCache();

        _initialized = true;
    }

    protected void OnDestroy()
    {
        if (Directory.Exists(SCManager.WORKING_DIRECTORY))
            Directory.Delete(SCManager.WORKING_DIRECTORY, true);
    }

    #endregion

    #region Public Methods

    public static void Connect(Action<bool> callback)
    {
        Instance.StartCoroutine(Instance.web.AuthenticateUser(callback));
    }

    public static void GetImage(string url, Action<Texture2D> callback)
    {
        Instance.StartCoroutine(Instance.web.WebRequestTexture(url, callback));
    }

    public static void GetDataType(string url, Action<string, Type> callback)
    {
        Instance.StartCoroutine(Instance.ProcessGenericDataCall(url, callback));
    }

    public static void GetUser(string url, Action<SoundCloudUser> callback)
    {
        Instance.StartCoroutine(Instance.ProcessDataCall(url, callback));
    }

    public static void GetUser(int id, Action<SoundCloudUser> callback)
    {
        GetUser(string.Format(SoundCloudUser.API_CALL, id, SoundCloudConfig.CLIENT_ID), callback);
    }

    public static void GetTrack(string url, Action<SoundCloudTrack> callback)
    {
        Instance.StartCoroutine(Instance.ProcessDataCall(url, callback));
    }

    public static void GetTrack(int id, Action<SoundCloudTrack> callback)
    {
        GetTrack(string.Format(SoundCloudTrack.API_CALL, id, SoundCloudConfig.CLIENT_ID), callback);
    }

    public static void GetTrackAudioClip(string url, Action<AudioClip> callback)
    {
        Instance.StartCoroutine(Instance.ProcessGetAudioTrack(url, callback));
    }

    public static void GetTrackAudioClip(int id, Action<AudioClip> callback)
    {
        GetTrackAudioClip(string.Format(SoundCloudTrack.API_CALL, id, SoundCloudConfig.CLIENT_ID), callback);
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

    private IEnumerator ProcessGetAudioTrack(string url, Action<AudioClip> callback)
    {
        AudioClip clip = null;

        // Get the track data.
        SoundCloudTrack track = null;

        if (!IsAPI(url))
            yield return StartCoroutine(web.ProcessResolveURL(url, (success, resolvedURL) => { if (success) url = resolvedURL; }));

        if (IsAPI(url))
            yield return StartCoroutine(ProcessDataCall<SoundCloudTrack>(url, (retVal) => track = retVal));

        // Get the MP3 stream.
        string mp3FilePath = string.Empty;
        yield return StartCoroutine(web.WebRequestFile(track.stream_url + "?client_id=" + SoundCloudConfig.CLIENT_ID, TEMP_FILENAME, (retVal) => mp3FilePath = retVal));

        // Transcode to OGG.
        bool transcodeComplete = false;
        bool transcodeSuccess = false;
        string outputFilePath = WORKING_DIRECTORY + "/" + track.id + ".ogg";
        transcoder.Transcode(mp3FilePath, outputFilePath, (retVal) => { transcodeSuccess = retVal; transcodeComplete = true; });

        while (!transcodeComplete)
            yield return 0;

        if (transcodeSuccess)
        {
            // Load the transcoded audio file as an AudioClip.
            yield return StartCoroutine(web.WebRequestAudioClip("file:///" + outputFilePath, (retVal) => clip = retVal));
        }

        if (callback != null)
            callback(clip);
    }

    private IEnumerator ProcessGenericDataCall(string url, Action<string, Type> callback)
    {
        // TODO: Refactor the way unknown generic data calls are handled based on automatically following
        //       redirect and getting the "kind" and "uri" from the resulting SoundCloudGeneric.
        Type returnType = null;

        if (!IsAPI(url))
            yield return StartCoroutine(web.ProcessResolveURL(url, (success, resolvedURL) => { if (success) url = resolvedURL; }));

        if (IsAPI(url))
        {
            if (url.Contains("users"))
                returnType = typeof(SoundCloudUser);
            else if (url.Contains("tracks"))
                returnType = typeof(SoundCloudTrack);
        }

        if (callback != null)
            callback(url, returnType);
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

    public static bool initialized 
    {
        get
        {
            return Instance._initialized;
        }
    }

    #endregion
}

}