
namespace SoundCloud
{

public class SCGroup : DataObject<SCGroup>
{
    public const string API_CALL = "http://api.soundcloud.com/groups/{0}?client_id={1}";

    public int id { get; protected set; }
}

}