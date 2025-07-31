using NeeLaboratory.Windows.Media;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace NeeRuler.Models
{
    [DataContract]
    public class LayoutProfile
    {
        [DataMember(Order = 1)]
        public bool? IsVertical { get; set; }

        [DataMember(Order = 2)]
        public bool? IsFlatPanel { get; set; }

        [DataMember(Order = 3)]
        public double? Width { get; set; }

        [DataMember(Order = 4)]
        public double? Height { get; set; }

        [DataMember(Order = 5)]
        public double? TextLineHeight { get; set; }

        [DataMember(Order = 6)]
        public double? TextLineTopMargin { get; set; }

        [DataMember(Order = 7)]
        public double? TextLineBottomMargin { get; set; }

        [DataMember(Order = 8)]
        public double? BaseLine { get; set; }

        [DataMember(Order = 9)]
        public double? BaseLineHeight { get; set; }

        public Color? BackgroundColor { get; set; }

        [DataMember(Order = 10, Name = nameof(BackgroundColor))]
        public string? BackgroundColorString { get; set; }

        public Color? TextLineColor { get; set; }

        [DataMember(Order = 11, Name = nameof(TextLineColor))]
        public string? TextLineColorString { get; set; }

        public Color? BaseLineColor { get; set; }

        [DataMember(Order = 12, Name = nameof(BaseLineColor))]
        public string? BaseLineColorString { get; set; }

        [DataMember(Order = 13)]
        public double? InactiveWindowOpacity { get; set; }

        [DataMember(Order = 14)]
        public bool? IsFollowMouse { get; set; }


        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            BackgroundColorString = BackgroundColor?.ToString();
            TextLineColorString = TextLineColor?.ToString();
            BaseLineColorString = BaseLineColor?.ToString();
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            BackgroundColor = ColorUtils.FromString(BackgroundColorString);
            TextLineColor = ColorUtils.FromString(TextLineColorString);
            BaseLineColor = ColorUtils.FromString(BaseLineColorString);
        }
    }

}