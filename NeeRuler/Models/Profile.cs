using System.Runtime.Serialization;

namespace NeeRuler.Models
{
    [DataContract]
    public class Profile
    {
        [DataMember(Order = 1)]
        public double? StorageLocationX { get; set; }

        [DataMember(Order = 2)]
        public double? StorageLocationY { get; set; }

        [DataMember(Order = 3)]
        public double? Stride { get; set; }

        [DataMember(Order = 4)]
        public double? AdjustmentStride { get; set; }

        [DataMember(Order = 5)]
        public bool? IsAutoStride { get; set; }

        [DataMember(Order = 10)]
        public AutoStrideProfile? AutoStride { get; set; }

        [DataMember(Order = 11)]
        public LayoutProfile? Layout { get; set; }
    }
}