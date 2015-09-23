
namespace SoundCloud
{

public class SoundCloudUser : DataObject<SoundCloudUser>
{
    public const string API_CALL = "http://api.soundcloud.com/users/{0}?client_id={1}";

    public int id { get; protected set; }
    public string permalink { get; protected set; }
    public string username { get; protected set; }
    public string uri { get; protected set; }
    public string permalink_url { get; protected set; }
}

}