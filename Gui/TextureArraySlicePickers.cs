using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Client.System.Resource.Handle;
using ImSharp;
using Luna;
using Luna.DirectX;

namespace Penumbra.GameData.Gui;

public sealed unsafe class TextureArraySlicePickers : IUiService
{
    public const int TileOrbArrayTexIndex  = 81;
    public const int TileNormArrayTexIndex = 82;
    public const int SphereDArrayTexIndex  = 97;

    private const float MaximumTextureSize = 64.0f;

    private readonly TextureArraySlicer _textureArraySlicer;

    public readonly IEditor<byte> TileIndexPicker;
    public readonly IEditor<byte> SphereMapIndexPicker;

    public TextureArraySlicePickers(TextureArraySlicer textureArraySlicer)
    {
        _textureArraySlicer  = textureArraySlicer;
        TileIndexPicker      = ((IEditor<float>)new Editor(DrawTileIndexPicker)).Reinterpreting<byte>();
        SphereMapIndexPicker = ((IEditor<float>)new Editor(DrawSphereMapIndexPicker)).Reinterpreting<byte>();
    }

    public bool DrawTileIndexPicker(ReadOnlySpan<byte> label, ReadOnlySpan<byte> description, ref ushort value, bool compact)
    {
        var characterUtility = CharacterUtility.Instance();
        if (characterUtility is null)
            return false;

        return DrawTextureArrayIndexPicker(label, description, ref value, compact, [
            characterUtility->ResourceHandles[TileOrbArrayTexIndex].Cast<TextureResourceHandle>(),
            characterUtility->ResourceHandles[TileNormArrayTexIndex].Cast<TextureResourceHandle>(),
        ]);
    }

    public bool DrawSphereMapIndexPicker(ReadOnlySpan<byte> label, ReadOnlySpan<byte> description, ref ushort value, bool compact)
    {
        var characterUtility = CharacterUtility.Instance();
        if (characterUtility is null)
            return false;

        return DrawTextureArrayIndexPicker(label, description, ref value, compact, [
            characterUtility->ResourceHandles[SphereDArrayTexIndex].Cast<TextureResourceHandle>(),
        ]);
    }

    public bool DrawTextureArrayIndexPicker(ReadOnlySpan<byte> label, ReadOnlySpan<byte> description, ref ushort value, bool compact,
        ReadOnlySpan<FFXIVClientStructs.Interop.Pointer<TextureResourceHandle>> textureRHs)
    {
        const ComboFlags flags = ComboFlags.NoArrowButton | ComboFlags.HeightLarge;

        var firstNonNullTextureRh = textureRHs.FindFirst(t => !t.IsNull && t.Value->Texture is not null, out var p) ? p.Value : null;
        var firstNonNullTexture   = firstNonNullTextureRh is not null ? firstNonNullTextureRh->Texture : null;

        var textureSize = firstNonNullTexture is not null
            ? new Vector2(firstNonNullTexture->ActualWidth, firstNonNullTexture->ActualHeight).Contain(new Vector2(MaximumTextureSize))
            : Vector2.Zero;
        var count = firstNonNullTexture is not null ? firstNonNullTexture->ArraySize : 0;

        var framePadding = Im.Style.FramePadding;
        var itemSpacing  = Im.Style.ItemSpacing;

        var ret = false;
        using (Im.Font.PushMono())
        {
            var       spaceSize  = Im.Font.Mono.GetCharacterAdvance(' ');
            var       compactX   = compact ? 0.0f : (textureSize.X + itemSpacing.X) * textureRHs.Length;
            var       spaces     = (int)((Im.Item.CalculateWidth() - framePadding.X * 2.0f - compactX) / spaceSize);
            var       newPadding = framePadding.AddX(Math.Max(textureSize.Y - Im.Style.FrameHeight + itemSpacing.Y, 0.0f) * 0.5f);
            using var padding    = ImStyleDouble.FramePadding.Push(newPadding, !compact);
            using var combo      = Im.Combo.Begin(label, (value is ushort.MaxValue ? "\u2014" : value.ToString()).PadLeft(spaces), flags);
            if (combo.Success && firstNonNullTextureRh is not null)
            {
                var lineHeight = Math.Max(Im.Style.TextHeightWithSpacing, framePadding.Y * 2.0f + textureSize.Y);
                var itemWidth = Math.Max(Im.ContentRegion.Available.X,
                    Im.Font.CalculateSize("MMM"u8).X + (itemSpacing.X + textureSize.X) * textureRHs.Length + framePadding.X * 2.0f);
                if (Im.Window.Appearing && value < count)
                    Im.Scroll.SetFromPositionY((value + 0.5f) * lineHeight);
                using var center  = ImStyleDouble.SelectableTextAlign.Push(new Vector2(0, 0.5f));
                using var clipper = new Im.ListClipper(count, lineHeight);
                foreach (var index in clipper)
                {
                    if (Im.Selectable($"{index,3}", index == value, size: new Vector2(itemWidth, lineHeight)))
                    {
                        ret   = value != index;
                        value = (ushort)index;
                    }

                    var rectMin = Im.Item.UpperLeftCorner;
                    var rectMax = Im.Item.LowerRightCorner;
                    var startX = rectMax.X - framePadding.X - textureSize.X * textureRHs.Length - itemSpacing.X * (textureRHs.Length - 1);
                    var textureRegionStart = new Vector2(startX, rectMin.Y + framePadding.Y);
                    var maxSize = textureSize with { Y = rectMax.Y - framePadding.Y - textureRegionStart.Y };
                    DrawTextureSlices(textureRegionStart, maxSize, itemSpacing.X, textureRHs, (byte)index);
                }
            }
        }

        if (!compact && value is not ushort.MaxValue)
        {
            var cbRectMin = Im.Item.UpperLeftCorner;
            var cbRectMax = Im.Item.LowerRightCorner;
            var startX = cbRectMax.X - framePadding.X - textureSize.X * textureRHs.Length - itemSpacing.X * (textureRHs.Length - 1);
            var cbTextureRegionStart = new Vector2(startX, cbRectMin.Y + framePadding.Y);
            var cbMaxSize = textureSize with { Y = cbRectMax.Y - framePadding.Y - cbTextureRegionStart.Y };
            DrawTextureSlices(cbTextureRegionStart, cbMaxSize, itemSpacing.X, textureRHs, (byte)value);
        }

        if (Im.Item.Hovered() && Im.Io.KeyControl)
        {
            Im.Item.SetUsingMouseWheel();
            var delta = (int)Im.Io.MouseWheel;
            if (delta is not 0)
            {
                var newValue = (ushort)ImUtility.ApplyMouseWheelDelta(delta, value, count);
                ret   |= newValue != value;
                value =  newValue;
            }
        }

        if (Im.Item.Hovered(HoveredFlags.AllowWhenDisabled) && (description.Length > 0 || compact && value is not ushort.MaxValue))
        {
            using var disabled = Im.Enabled();
            using var tt       = Im.Tooltip.Begin();
            if (description.Length > 0)
                Im.Text(description);
            if (compact && value is not ushort.MaxValue)
            {
                Im.Dummy(textureSize with { X = textureSize.X * textureRHs.Length + itemSpacing.X * (textureRHs.Length - 1) });
                var rectMin = Im.Item.UpperLeftCorner;
                DrawTextureSlices(rectMin, textureSize, itemSpacing.X, textureRHs, (byte)value);
            }
        }

        return ret;
    }

    public void DrawTextureSlices(Vector2 regionStart, Vector2 itemSize, float itemSpacing,
        ReadOnlySpan<FFXIVClientStructs.Interop.Pointer<TextureResourceHandle>> textureRHs, byte sliceIndex)
    {
        for (var j = 0; j < textureRHs.Length; ++j)
        {
            if (textureRHs[j].Value is null)
                continue;

            var texture = textureRHs[j].Value->Texture;
            if (texture is null || sliceIndex >= texture->ArraySize)
                continue;

            var handle = _textureArraySlicer.GetImGuiHandle(texture, sliceIndex);
            if (handle.IsNull)
                continue;

            var position = regionStart with { X = regionStart.X + (itemSize.X + itemSpacing) * j };
            var size     = new Vector2(texture->ActualWidth, texture->ActualHeight).Contain(itemSize);
            position += (itemSize - size) * 0.5f;
            var uvSize = Rectangle.FromSize(texture->ActualWidth / (float)texture->AllocatedWidth,
                texture->ActualHeight / (float)texture->AllocatedHeight);
            Im.Window.DrawList.Image(handle, Rectangle.FromSize(position, size), uvSize);
        }
    }

    private delegate bool DrawEditor(ReadOnlySpan<byte> label, ReadOnlySpan<byte> description, ref ushort value, bool compact);

    private sealed class Editor(DrawEditor draw) : IEditor<float>
    {
        public bool Draw(Span<float> values, bool disabled)
        {
            var helper = Editors.PrepareMultiComponent(values.Length);
            var ret    = false;

            for (var valueIdx = 0; valueIdx < values.Length; ++valueIdx)
            {
                helper.SetupComponent(valueIdx);

                var value = ushort.CreateSaturating(MathF.Round(values[valueIdx]));
                if (disabled)
                {
                    using var _ = Im.Disabled();
                    draw(helper.Id, default, ref value, true);
                }
                else
                {
                    if (draw(helper.Id, default, ref value, true))
                    {
                        values[valueIdx] = value;
                        ret              = true;
                    }
                }
            }

            return ret;
        }
    }
}
