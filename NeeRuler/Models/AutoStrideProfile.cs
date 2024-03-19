using System.Runtime.Serialization;

namespace NeeRuler.Models
{
    [DataContract]
    public class AutoStrideProfile
    {
        [DataMember]
        public double? TextLineComplexityThreshold { get; set; }

        [DataMember]
        public double? MinTextHeight { get; set; }
    }

}