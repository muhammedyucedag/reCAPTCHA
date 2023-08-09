using API.Interfaces;
using API.Request;
using API.Response;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace API.Services;

public class SignupService : ISignupService
{
    private readonly AppSettings _appSettings;

    public SignupService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public async Task<SignupResponse> Signup(SignupRequest signupRequest)
    {
        
            var dictionary = new Dictionary<string, string>
                    {
                        { "secret", _appSettings.RecaptchaSecretKey },
                        { "response", signupRequest.ReCaptchaToken }
                    };

            var postContent = new FormUrlEncodedContent(dictionary);

            HttpResponseMessage recaptchaResponse = null;
            string stringContent = "";
            
            using (var http = new HttpClient())
            {
                recaptchaResponse = await http.PostAsync("https://www.google.com/recaptcha/api/siteverify", postContent);
                stringContent = await recaptchaResponse.Content.ReadAsStringAsync();
            }

            if (!recaptchaResponse.IsSuccessStatusCode)
            {
                return new SignupResponse() { Success = false, Error = "Unable to verify recaptcha token", ErrorCode = "S03" };
            }

            if (string.IsNullOrEmpty(stringContent))
            {
                return new SignupResponse() { Success = false, Error = "Invalid reCAPTCHA verification response", ErrorCode = "S04" };
            }

            var googleReCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(stringContent);

            if (!googleReCaptchaResponse.Success)
            {
                var errors = string.Join(",", googleReCaptchaResponse.ErrorCodes);

                return new SignupResponse() { Success = false, Error = errors, ErrorCode = "S05" };
            }

            if (!googleReCaptchaResponse.Action.Equals("signup", StringComparison.OrdinalIgnoreCase))
            {
                return new SignupResponse() { Success = false, Error = "Invalid action", ErrorCode = "S06" };
            }
        

            if (googleReCaptchaResponse.Score < 0.5)
            {
                return new SignupResponse() { Success = false, Error = "This is a potential bot. Signup request rejected", ErrorCode = "S07" };
            }
            
            return new SignupResponse() { Success = true };
        }
    }
