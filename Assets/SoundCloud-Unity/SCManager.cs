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

    private SCWeb web;
    private SCTranscoder transcoder;
    private SCCache cache;

    #endregion

    #region Unity Events

    protected void Start()
    {
        if (!Directory.Exists(WORKING_DIRECTORY))
            Directory.CreateDirectory(WORKING_DIRECTORY);

        transcoder = gameObject.AddComponent<SCTranscoder>();
        web = gameObject.AddComponent<SCWeb>();
        cache = new SCCache();

        _initialized = true;
    }

    protected void OnDestroy()
    {
        // SoundCloud Terms of Use:
        // "Your app may employ session-based caching, but only to the extent reasonably necessary for the operation of your app."
        // "Except for session-based caching referred to above, your app must not offer offline access to any User Content including, 
        // in the case of audio User Content, as permanent downloads or temporary downloads for offline listening"
        // More Info: https://developers.soundcloud.com/docs/api/terms-of-use#caching

        // We must make sure the entire cache is deleted at the end of a session.
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

    public static void GetUser(string url, Action<SCUser> callback)
    {
        Instance.StartCoroutine(Instance.ProcessDataCall(url, callback));
    }

    public static void GetUser(int id, Action<SCUser> callback)
    {
        GetUser(string.Format(SCUser.API_CALL, id, SCConfig.CLIENT_ID), callback);
    }

    public static void GetTrack(string url, Action<SCTrack> callback)
    {
        Instance.StartCoroutine(Instance.ProcessDataCall(url, callback));
    }

    public static void GetTrack(int id, Action<SCTrack> callback)
    {
        GetTrack(string.Format(SCTrack.API_CALL, id, SCConfig.CLIENT_ID), callback);
    }

    public static void GetTrackAudioClip(string url, Action<AudioClip> callback)
    {
        Instance.StartCoroutine(Instance.ProcessGetAudioTrack(url, callback));
    }

    public static void GetTrackAudioClip(int id, Action<AudioClip> callback)
    {
        GetTrackAudioClip(string.Format(SCTrack.API_CALL, id, SCConfig.CLIENT_ID), callback);
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
        SCTrack track = null;

        if (!IsAPI(url))
            yield return StartCoroutine(web.ProcessResolveURL(url, (success, resolvedURL) => { if (success) url = resolvedURL; }));

        if (IsAPI(url))
            yield return StartCoroutine(ProcessDataCall<SCTrack>(url, (retVal) => track = retVal));

        // Get the MP3 stream.
        string mp3FilePath = string.Empty;
        yield return StartCoroutine(web.WebRequestFile(track.stream_url + "?client_id=" + SCConfig.CLIENT_ID, TEMP_FILENAME, (retVal) => mp3FilePath = retVal));

        // Transcode to OGG.
        bool transcodeComplete = false;
        bool transcodeSuccess = false;
        string outputFilePath = WORKING_DIRECTORY + "/" + track.id + ".ogg";
        transcoder.Transcode(mp3FilePath, outputFilePath, (retVal) => { transcodeSuccess = retVal; transcodeComplete = true; });

        while (!transcodeComplete)
            yield return 0;

        if (transcodeSuccess)
        {
            
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            // On windows, we need to prepend "file:///" when accessing a local file through WWW.
            outputFilePath = "file:///" + outputFilePath;
#endif
            // Load the transcoded audio file as an AudioClip.
            yield return StartCoroutine(web.WebRequestAudioClip(outputFilePath, (retVal) => clip = retVal));
        }

        if (callback != null)
            callback(clip);
    }

    private IEnumerator ProcessGenericDataCall(string url, Action<string, Type> callback)
    {
        // TODO: Refactor the way unknown generic data calls are handled based on automatically following
        //       redirect and getting the "kind" and "uri" from the resulting SCGeneric.
        Type returnType = null;

        if (!IsAPI(url))
            yield return StartCoroutine(web.ProcessResolveURL(url, (success, resolvedURL) => { if (success) url = resolvedURL; }));

        if (IsAPI(url))
        {
            if (url.Contains("users"))
                returnType = typeof(SCUser);
            else if (url.Contains("tracks"))
                returnType = typeof(SCTrack);
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