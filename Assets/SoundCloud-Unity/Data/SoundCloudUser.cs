
namespace SoundCloud
{

public class SoundCloudUser : DataObject<SoundCloudUser>
{
    public const string API_CALL = "http://api.soundcloud.com/users/{0}?client_id={1}";

    public int id { get; protected set; }
}

}