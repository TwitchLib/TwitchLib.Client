namespace TwitchLib.Client.Models
{
    /// <summary>Class representing a detection return object.</summary>
    public class DetectionReturn
    {
        /// <summary>Property representing whether detection was successful.</summary>
        public bool Successful { get; }
        /// <summary>Property representing the detected channel, could be null.</summary>
        public string Channel { get; }
        /// <summary>Property representing optional data that can be passed out of chat parsing class.</summary>
        public string OptionalData { get; }

        /// <summary>DetectionReturn object constructor.</summary>
        public DetectionReturn(bool successful, string channel = null, string optionalData = null)
        {
            Successful = successful;
            Channel = channel;
            OptionalData = optionalData;
        }
    }
}
