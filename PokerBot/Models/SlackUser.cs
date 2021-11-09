using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokerBot.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public partial class SlackUser
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }

        [JsonPropertyName("profile")]
        public Profile Profile { get; set; }
    }

    public partial class Profile
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("skype")]
        public string Skype { get; set; }

        [JsonPropertyName("real_name")]
        public string RealName { get; set; }

        [JsonPropertyName("real_name_normalized")]
        public string RealNameNormalized { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("display_name_normalized")]
        public string DisplayNameNormalized { get; set; }

        [JsonPropertyName("fields")]
        public object Fields { get; set; }

        [JsonPropertyName("status_text")]
        public string StatusText { get; set; }

        [JsonPropertyName("status_emoji")]
        public string StatusEmoji { get; set; }

        [JsonPropertyName("status_expiration")]
        public long StatusExpiration { get; set; }

        [JsonPropertyName("avatar_hash")]
        public string AvatarHash { get; set; }

        [JsonPropertyName("image_original")]
        public string ImageOriginal { get; set; }

        [JsonPropertyName("is_custom_image")]
        public bool IsCustomImage { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("image_24")]
        public string Image24 { get; set; }

        [JsonPropertyName("image_32")]
        public string Image32 { get; set; }

        [JsonPropertyName("image_48")]
        public string Image48 { get; set; }

        [JsonPropertyName("image_72")]
        public string Image72 { get; set; }

        [JsonPropertyName("image_192")]
        public string Image192 { get; set; }

        [JsonPropertyName("image_512")]
        public string Image512 { get; set; }

        [JsonPropertyName("image_1024")]
        public string Image1024 { get; set; }

        [JsonPropertyName("status_text_canonical")]
        public string StatusTextCanonical { get; set; }
    }

    public partial class SlackUser
    {
        public static SlackUser FromJson(string json) => JsonSerializer.Deserialize<SlackUser>(json);
    }

    public static class Serialize
    {
        public static string ToJson(this SlackUser self) => JsonSerializer.Serialize(self);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerOptions Settings = new JsonSerializerOptions
        {
            
            //MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            //DateParseHandling = DateParseHandling.None,
            //Converters = {
            //    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            //},
        };
    }
}
