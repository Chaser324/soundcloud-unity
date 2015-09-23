
namespace SoundCloud
{

public class SoundCloudGeneric : DataObject<SoundCloudGeneric>
{
    public const string API_CALL = "";

    public int id { get; protected set; }
    public string kind { get; protected set; }
    public string uri { get; protected set; }
}

}