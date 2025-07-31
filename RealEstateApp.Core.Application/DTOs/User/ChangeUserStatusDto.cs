using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;


namespace RealEstateApp.Core.Application.DTOs.User
{
    public class ChangeUserStatusDto
    {
        [JsonProperty("status")]
        [Required]
        public bool Status { get; set; }
    }

}
