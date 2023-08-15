using System.Text.Json.Serialization;

namespace API.Response;

public class SignupResponse
{
    //Kayıt geri dönüşü
    public bool Success { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Error { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string ErrorCode { get; set; } 
}