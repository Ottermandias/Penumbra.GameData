using ImSharp;
using Penumbra.GameData.Enums;

namespace Penumbra.GameData.Files;

/// <summary> The character make parameters is a straight-up data blob only consisting of colors and some scaling floats. </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CmpData
{
    public ColorParameters       Parameters;
    public ColorParameters       Interface;
    public RacialColorParameters Races;
    public RacialScaling         Scales;

    [InlineArray(128)]
    public struct TonedColors
    {
        private Rgba32 _color0;
    }

    public ref struct ColorsPair(ref TonedColors dark, ref TonedColors light)
    {
        public ref TonedColors Dark  = ref dark;
        public ref TonedColors Light = ref light;

        public ref Rgba32 this[int index]
        {
            get
            {
                if (index < 128)
                    return ref Dark[index];

                return ref Light[index - 128];
            }
        }
    }

    [InlineArray(256)]
    public struct FullColors
    {
        private Rgba32 _color0;
    }

    public struct ColorParameters
    {
        public FullColors  Eyes;
        public FullColors  HairHighlights;
        public TonedColors LipsDark;
        public TonedColors FacePaintDark;
        public FullColors  Features;
        public TonedColors LipsLight;
        public TonedColors FacePaintLight;

        public FullColors UnusedEyes1;
        public FullColors UnusedEyes2;
        public FullColors UnusedEyes3;
        public FullColors UnusedFeatures;
    }

    public struct HairColor
    {
        public Rgba32 Main;
        public Rgba32 UnusedSheen;
    }

    [InlineArray(256)]
    public struct HairColors
    {
        private HairColor _color0;
    }

    public struct GenderClanColorParameters
    {
        public FullColors Skin;
        public HairColors Hair;
        public FullColors SkinInterface;
        public FullColors HairInterface;
    }

    [InlineArray(32)]
    public struct RacialColorParameters
    {
        private GenderClanColorParameters _parameters0;
    }

    public struct MinMax
    {
        public float Minimum;
        public float Maximum;
    }

    public struct MinMax3
    {
        public float MinimumX;
        public float MinimumY;
        public float MinimumZ;
        public float MaximumX;
        public float MaximumY;
        public float MaximumZ;
    }

    public struct Scale
    {
        public MinMax  MaleHeight;
        public MinMax  MaleTail;
        public MinMax  FemaleHeight;
        public MinMax  FemaleTail;
        public MinMax3 BreastSize;
    }

    [InlineArray(10)]
    public struct BodyTypeScales
    {
        private Scale _scale0;
    }

    [InlineArray(8)]
    public struct RacialScaling
    {
        private BodyTypeScales _scale0;
    }
}

public static class CmpFileExtensions
{
    extension(ref CmpData @this)
    {
        public ref CmpData.Scale GetScale(SubRace race)
        {
            var idx = (int)race - 1;
            return ref @this.Scales[idx >> 1][idx & 1];
        }

        public ref CmpData.FullColors GetSkin(SubRace race, Gender gender)
        {
            var index = gender is Gender.Female or Gender.FemaleNpc ? ((int)race - 1) * 2 + 1 : ((int)race - 1) * 2;
            return ref @this.Races[index].SkinInterface;
        }

        public ref CmpData.FullColors GetHair(SubRace race, Gender gender)
        {
            var index = gender is Gender.Female or Gender.FemaleNpc ? ((int)race - 1) * 2 + 1 : ((int)race - 1) * 2;
            return ref @this.Races[index].HairInterface;
        }

        public ref CmpData.FullColors Eyes
            => ref @this.Interface.Eyes;

        public ref CmpData.FullColors Highlights
            => ref @this.Interface.HairHighlights;

        public ref CmpData.FullColors Features
            => ref @this.Interface.Features;

        public CmpData.ColorsPair Lips
            => new(ref @this.Interface.LipsDark, ref @this.Interface.LipsLight);

        public CmpData.ColorsPair FacePaint
            => new(ref @this.Interface.FacePaintDark, ref @this.Interface.FacePaintLight);
    }

    extension(ref CmpData.Scale @this)
    {
        public ref float Get(RspAttribute attribute)
        {
            switch (attribute)
            {
                case RspAttribute.MaleMinSize:   return ref @this.MaleHeight.Minimum;
                case RspAttribute.MaleMaxSize:   return ref @this.MaleHeight.Maximum;
                case RspAttribute.MaleMinTail:   return ref @this.MaleTail.Minimum;
                case RspAttribute.MaleMaxTail:   return ref @this.MaleTail.Maximum;
                case RspAttribute.FemaleMinSize: return ref @this.FemaleHeight.Minimum;
                case RspAttribute.FemaleMaxSize: return ref @this.FemaleHeight.Maximum;
                case RspAttribute.FemaleMinTail: return ref @this.FemaleTail.Minimum;
                case RspAttribute.FemaleMaxTail: return ref @this.FemaleTail.Maximum;
                case RspAttribute.BustMinX:      return ref @this.BreastSize.MinimumX;
                case RspAttribute.BustMinY:      return ref @this.BreastSize.MinimumY;
                case RspAttribute.BustMinZ:      return ref @this.BreastSize.MinimumZ;
                case RspAttribute.BustMaxX:      return ref @this.BreastSize.MaximumX;
                case RspAttribute.BustMaxY:      return ref @this.BreastSize.MaximumY;
                case RspAttribute.BustMaxZ:      return ref @this.BreastSize.MaximumZ;
                default:                         throw new ArgumentOutOfRangeException(nameof(attribute), attribute, null);
            }
        }
    }
}
