
namespace SoundCloud
{

public class SoundCloudGroup : DataObject<SoundCloudGroup>
{
    public const string API_CALL = "http://api.soundcloud.com/groups/{0}?client_id={1}";

    public int id { get; protected set; }
}

}