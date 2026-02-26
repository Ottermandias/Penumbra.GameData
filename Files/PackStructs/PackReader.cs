using Luna;
using Penumbra.GameData.Files.PackStructs;

/// <summary> A helper utility to read all pack structure data for files. </summary>
public unsafe ref struct PackReader
{
    private ReadOnlySpan<byte> _baseData;
    private PackFooter         _packFooter;

    /// <summary> The total length of the pack section, if it exists. </summary>
    public ulong PackLength
        => _packFooter.TotalSize;

    /// <summary> Whether there is data remaining. </summary>
    public bool HasData { get; private set; }

    /// <summary> Read the <see cref="PackFooter"/> of the data if it exists and note if we have further data. </summary>
    /// <param name="baseData"> The full file data. </param>
    public PackReader(ReadOnlySpan<byte> baseData)
    {
        _baseData = baseData;
        HasData   = PackFooter.TryRead(_baseData, out _packFooter);
        if (!HasData)
        {
            _packFooter.TotalSize = 0;
            return;
        }

        _baseData = _baseData[..^sizeof(PackFooter)];
        if (_packFooter.Header.PriorOffset + _baseData.Length >= _baseData.Length)
            HasData = false;
        if (_packFooter.Header.PackCount < 1)
            HasData = false;
    }

    /// <summary> Try to obtain the prior pack structure of a given type if there is any. </summary>
    /// <param name="type"> The type to expect. Returns false if the structure does not correspond to this type. </param>
    /// <param name="prior"> The returned pack. </param>
    /// <returns> True on success. </returns>
    public bool TryGetPrior(uint type, out Pack prior)
    {
        if (!HasData)
        {
            prior = default;
            return false;
        }

        var start     = (int)(_baseData.Length + _packFooter.Header.PriorOffset);
        var reader    = new SpanBinaryReader(_baseData[start..]);
        var newFooter = reader.Read<PackHeader>();
        if (newFooter.Type != type)
        {
            prior = default;
            return false;
        }

        prior.Header       = newFooter;
        prior.Data         = _baseData[(start + sizeof(PackHeader))..];
        _packFooter.Header = newFooter;
        _baseData          = _baseData[..start];
        if (_packFooter.Header.PriorOffset + _baseData.Length >= _baseData.Length)
            HasData = false;
        if (_packFooter.Header.PackCount < 1)
            HasData = false;
        return true;
    }
}
