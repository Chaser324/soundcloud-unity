using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SoundCloud
{

public class SoundCloudCache
{
    private const int APIDATA_CAPACITY = 20;
    private const int URLRESOLVE_CAPCITY = 20;
    private const int AUDIO_CAPACITY = 20;

    private Dictionary<string, Object> apiDataCache = new Dictionary<string,Object>(APIDATA_CAPACITY);
    private Dictionary<string, string> urlResolveCache = new Dictionary<string,string>(URLRESOLVE_CAPCITY);
    private Dictionary<int, string> audioCache = new Dictionary<int,string>(AUDIO_CAPACITY);

    private Queue<string> apiDataQueue = new Queue<string>(APIDATA_CAPACITY);
    private Queue<string> urlResolveQueue = new Queue<string>(URLRESOLVE_CAPCITY);
    private Queue<int> audioQueue = new Queue<int>(AUDIO_CAPACITY);

    public ~SoundCloudCache()
    {
        if (Directory.Exists(SoundCloud.WORKING_DIRECTORY))
            Directory.Delete(SoundCloud.WORKING_DIRECTORY);
    }



    public void AddApiData(string apiCall, Object data)
    {
        while (apiDataQueue.Count > APIDATA_CAPACITY - 1)
            apiDataCache.Remove(apiDataQueue.Dequeue());

        apiDataQueue.Enqueue(apiCall);
        apiDataCache.Add(apiCall, data);
    }

    public bool HasApiData(string apiCall)
    {
        return apiDataCache.ContainsKey(apiCall);
    }

    public T GetApiData<T>(string apiCall) where T : Object
    {
        return apiDataCache[apiCall] as T;
    }




    public void AddUrlResolve(string url, string resolveUrl)
    {
        while (urlResolveQueue.Count > URLRESOLVE_CAPCITY - 1)
            urlResolveCache.Remove(urlResolveQueue.Dequeue());

        urlResolveQueue.Enqueue(url);
        urlResolveCache.Add(url, resolveUrl);
    }

    public bool HasUrlResolve(string url)
    {
        return urlResolveCache.ContainsKey(url);
    }

    public string GetUrlResolve(string url)
    {
        return urlResolveCache[url];
    }




    public void AddAudio(int trackID, string filepath)
    {
        while (audioQueue.Count > AUDIO_CAPACITY - 1)
            audioCache.Remove(audioQueue.Dequeue());

        audioQueue.Enqueue(trackID);
        audioCache.Add(trackID, filepath);
    }

    public bool HasAudio(int trackID)
    {
        return audioCache.ContainsKey(trackID);
    }

    public string GetAudio(int trackID)
    {
        return audioCache[trackID];
    }
}

}