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
    public bool authenticated { get; private set; }

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

    protected IEnumerator Start()
    {
        if (!Directory.Exists(WORKING_DIRECTORY))
            Directory.CreateDirectory(WORKING_DIRECTORY);

        transcoder = gameObject.AddComponent<SoundCloudTranscoder>();
        web = gameObject.AddComponent<SoundCloudWWW>();

        SoundCloudTrack track = null;
        string redirect = "";
        string tempFile = "";
        AudioClip clip = null;
        bool transcoded = false;
        yield return StartCoroutine(WebRequest("http://api.soundcloud.com/resolve?url=" + "https://soundcloud.com/snakedrocks/paul-stanley" + "&client_id=" + SoundCloudConfig.CLIENT_ID, (retVal) => redirect = retVal.responseHeaders["LOCATION"]));
        yield return StartCoroutine(WebRequestObject<SoundCloudTrack>(redirect,
            (retVal) => track = retVal));
        //yield return StartCoroutine(WebRequestObject<SoundCloudTrack>("http://api.soundcloud.com/tracks/13158665?client_id=" + SoundCloudConfig.CLIENT_ID,
        //    (retVal) => track = retVal));
        yield return StartCoroutine(WebRequest(track.stream_url + "?client_id=" + SoundCloudConfig.CLIENT_ID, (retVal) => redirect = retVal.responseHeaders["LOCATION"]));
        //yield return StartCoroutine(WebRequestAudioClip(redirect, true, (retVal) => clip = retVal));
        yield return StartCoroutine(WebRequestFile(redirect, (retVal) => tempFile = retVal));

        string convertedFile = Application.temporaryCachePath + "/output.ogg";

        System.Diagnostics.Process ffmpeg = new System.Diagnostics.Process();
        ffmpeg.StartInfo.FileName = Application.dataPath + "/SoundCloud-Unity/FFMPEG/ffmpeg.exe";
        ffmpeg.StartInfo.Arguments = "-i \"" + tempFile + "\" -y -c:a libvorbis -q:a 4 \"" + convertedFile + "\"";
        ffmpeg.StartInfo.CreateNoWindow = true;
        ffmpeg.StartInfo.UseShellExecute = false;

        ffmpeg.EnableRaisingEvents = true;
        ffmpeg.Exited += (sender, args) => transcoded = true;
        ffmpeg.Start();

        while (!transcoded)
            yield return 0;

        yield return StartCoroutine(WebRequestAudioClip("file:///" + convertedFile, (retVal) => clip = retVal));

        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();

        initialized = true;
    }

    #endregion

    #region Public Methods

    public void Connect()
    {
    }

    public void GetUser()
    {
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

    

    #endregion
}

}