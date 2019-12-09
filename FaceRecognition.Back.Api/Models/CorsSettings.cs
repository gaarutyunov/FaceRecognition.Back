using System.Collections.Generic;

namespace FaceRecognition.Back.Api.Models
{
    public class CorsSettings
    {
        public string DefaultCors { get; set; }
        public Dictionary<string, CorsPolicy> Policies { get; set; }

        public CorsSettings()
        {
            DefaultCors = string.Empty;
            Policies = new Dictionary<string, CorsPolicy>();
        }
    }

    public class CorsPolicy
    {
        public string[] Headers { get; set; }
        public string[] Methods { get; set; }
        public string[] Origins { get; set; }
        public bool AllowCredentials { get; set; }

        public CorsPolicy()
        {
            Headers = new string[0];
            Methods = new string[0];
            Origins = new string[0];
        }
    }
}