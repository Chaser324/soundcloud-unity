using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SoundCloud
{

public class SCTranscoder : MonoBehaviour
{
    private const string FFMPEG_ARGUMENTS = "-i \"{0}\" -y -c:a libvorbis -q:a 4 \"{1}\"";
    private const float TRANSCODE_TIMEOUT = 30.0f;

    // TODO: Figure out path structure for different platforms/builds.
#if UNITY_EDITOR_WIN
    private static readonly string FFMPEG_PATH = Application.dataPath + "/SoundCloud-Unity/FFMPEG/ffmpeg.exe";
#elif UNITY_EDITOR_OSX
    private static readonly string FFMPEG_PATH = "";
#elif UNITY_STANDALONE_WIN
    private static readonly string FFMPEG_PATH = "";
#elif UNITY_STANDALONE_OSX
    private static readonly string FFMPEG_PATH = "";
#elif UNITY_STANDALONE_LINUX
    private static readonly string FFMPEG_PATH = "";
#endif

    public void Transcode(string mp3FilePath, string outputFilePath, Action<SCError> callback)
    {
        StartCoroutine(ProcessTranscode(mp3FilePath, outputFilePath, callback));
    }

    private IEnumerator ProcessTranscode(string mp3FilePath, string outputFilePath, Action<SCError> callback)
    {
        bool processComplete = false;
        bool transcoded = false;
        SCError error = SCError.OK;
        float timer = 0;

        // Setup FFMPEG process.
        System.Diagnostics.Process ffmpeg = new System.Diagnostics.Process();
        ffmpeg.StartInfo.FileName = FFMPEG_PATH;
        ffmpeg.StartInfo.Arguments = string.Format(FFMPEG_ARGUMENTS, mp3FilePath, outputFilePath);
        ffmpeg.StartInfo.CreateNoWindow = true;
        ffmpeg.StartInfo.UseShellExecute = false;
        ffmpeg.EnableRaisingEvents = true;

        // Create process exit callback.
        ffmpeg.Exited += (sender, args) =>
            {
                if(ffmpeg.ExitCode == 0 && File.Exists(outputFilePath))
                    transcoded = true;

                processComplete = true;
            };

        // Start process and wait for completion.
        ffmpeg.Start();
        while (!processComplete && timer < TRANSCODE_TIMEOUT)
        {
            timer += Time.deltaTime;
            yield return 0;
        }

        // Handle any error.
        if (transcoded)
        {
            Debug.Log("Transcode Complete (" + timer + " seconds)");
        }
        else if (processComplete && !transcoded)
        {
            Debug.LogError("Transcode Failed. Error code: " + ffmpeg.ExitCode);
            error = SCError.TranscodeFailed;
        }
        else if (!processComplete)
        {
            Debug.LogError("Transcode Failed. Timed Out.");
            ffmpeg.Kill();

            error = SCError.TranscodeTimeout;
        }

        if (callback != null)
            callback(error);
    }
}

}