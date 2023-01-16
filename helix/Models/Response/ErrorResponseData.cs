using Newtonsoft.Json;
namespace helix.Models.Response;

public class ErrorResponseData
{
    public string Error { get; set; }
    public int StatusCode { get; set; }
    public string Path { get; set; }
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}

