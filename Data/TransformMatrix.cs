namespace Penumbra.GameData.Data;

/// <summary>
/// Represents a transform matrix with the following values, stored in row-major order:
/// [ xx xy xz x0 ]
/// [ yx yy yz y0 ]
/// [ zx zy zz z0 ]
/// [  0  0  0  1 ]
/// Where x0, y0 and z0 are the translation vector, and the 9 other numbers are the scaling/rotation values.
/// </summary>
/// <remarks> This type is, notably, part of the PBD file format. </remarks>
[StructLayout(LayoutKind.Sequential)]
public readonly struct TransformMatrix : IEquatable<TransformMatrix>
{
    public static readonly TransformMatrix Zero     = new(Vector4.Zero,  Vector4.Zero,  Vector4.Zero);
    public static readonly TransformMatrix Identity = new(Vector4.UnitX, Vector4.UnitY, Vector4.UnitZ);

    private static readonly TransformMatrix NaN = new(new Vector4(float.NaN), new Vector4(float.NaN), new Vector4(float.NaN));

    public readonly Vector4 XRow;
    public readonly Vector4 YRow;
    public readonly Vector4 ZRow;

    public readonly Vector3 XColumn
        => new(XRow.X, YRow.X, ZRow.X);

    public readonly Vector3 YColumn
        => new(XRow.Y, YRow.Y, ZRow.Y);

    public readonly Vector3 ZColumn
        => new(XRow.Z, YRow.Z, ZRow.Z);

    public readonly Vector3 Translation
        => new(XRow.W, YRow.W, ZRow.W);

    public TransformMatrix(Vector4 xRow, Vector4 yRow, Vector4 zRow)
    {
        XRow = xRow;
        YRow = yRow;
        ZRow = zRow;
    }

    public TransformMatrix(Vector3 xColumn, Vector3 yColumn, Vector3 zColumn, Vector3 translation)
    {
        XRow = new(xColumn.X, yColumn.X, zColumn.X, translation.X);
        YRow = new(xColumn.Y, yColumn.Y, zColumn.Y, translation.Y);
        ZRow = new(xColumn.Z, yColumn.Z, zColumn.Z, translation.Z);
    }

    public static TransformMatrix CreateTranslation(Vector3 translation)
        => new(
            new Vector4(1.0f, 0.0f, 0.0f, translation.X),
            new Vector4(0.0f, 1.0f, 0.0f, translation.Y),
            new Vector4(0.0f, 0.0f, 1.0f, translation.Z));

    public static TransformMatrix CreateScale(Vector3 scale)
        => new(
            new Vector4(scale.X, 0.0f, 0.0f, 0.0f),
            new Vector4(0.0f, scale.Y, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, scale.Z, 0.0f));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TransformMatrix CreateRotation(Quaternion rotation)
        => (TransformMatrix)Matrix4x4.CreateFromQuaternion(rotation);

    public static TransformMatrix Compose(Vector3 scale, Quaternion rotation, Vector3 translation)
        => CreateTranslation(translation) * CreateRotation(rotation) * CreateScale(scale);

    public bool TryDecompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation)
        => Matrix4x4.Decompose(this, out scale, out rotation, out translation);

    public bool Equals(TransformMatrix other)
        => XRow == other.XRow && YRow == other.YRow && ZRow == other.ZRow;

    public override bool Equals(object? obj)
        => obj is TransformMatrix other && Equals(other);

    public override int GetHashCode()
        => unchecked(XRow.GetHashCode() + YRow.GetHashCode() * 31 + ZRow.GetHashCode() * 961);

    public override string ToString()
        => $"TransformMatrix {{ X = {XRow}, Y = {YRow}, Z = {ZRow} }}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TransformMatrix Append(in TransformMatrix mat)
        => mat * this;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TransformMatrix Prepend(in TransformMatrix mat)
        => this * mat;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Apply(Vector3 vec)
        => this * vec;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Apply(Vector4 vec)
        => this * vec;

    public float Determinant()
    {
        var xColumn = XColumn;
        var yColumn = YColumn;
        var zColumn = ZColumn;

        var coXColumn = Vector3.Cross(yColumn, zColumn);
        return Vector3.Dot(xColumn, coXColumn);
    }

    public TransformMatrix Invert()
        => TryInvert(out var inv) ? inv : throw new DivideByZeroException($"{this} is not invertible");

    public bool TryInvert(out TransformMatrix inv)
    {
        var xColumn = XColumn;
        var yColumn = YColumn;
        var zColumn = ZColumn;

        var coXColumn = Vector3.Cross(yColumn, zColumn);
        var det = Vector3.Dot(xColumn, coXColumn);

        if (det == 0.0f)
        {
            inv = NaN;
            return false;
        }

        var invDet = 1.0f / det;

        var coYColumn = Vector3.Cross(zColumn, xColumn);
        var coZColumn = Vector3.Cross(xColumn, yColumn);

        var negTranslation = -Translation;

        inv = new(
            new Vector4(coXColumn, Vector3.Dot(coXColumn, negTranslation)) * invDet,
            new Vector4(coYColumn, Vector3.Dot(coYColumn, negTranslation)) * invDet,
            new Vector4(coZColumn, Vector3.Dot(coZColumn, negTranslation)) * invDet);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in TransformMatrix lhs, in TransformMatrix rhs)
        => lhs.Equals(rhs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in TransformMatrix lhs, in TransformMatrix rhs)
        => !lhs.Equals(rhs);

    public static TransformMatrix operator +(in TransformMatrix lhs, in TransformMatrix rhs)
        => new(lhs.XRow + rhs.XRow, lhs.YRow + rhs.YRow, lhs.ZRow + rhs.ZRow);

    public static TransformMatrix operator *(float factor, in TransformMatrix mat)
        => new(factor * mat.XRow, factor * mat.YRow, factor * mat.ZRow);

    public static TransformMatrix operator *(in TransformMatrix mat, float factor)
        => new(mat.XRow * factor, mat.YRow * factor, mat.ZRow * factor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator *(in TransformMatrix mat, Vector3 vec)
        => mat * new Vector4(vec, 1.0f);

    public static Vector3 operator *(in TransformMatrix mat, Vector4 vec)
        => new(Vector4.Dot(mat.XRow, vec), Vector4.Dot(mat.YRow, vec), Vector4.Dot(mat.ZRow, vec));

    public static TransformMatrix operator *(in TransformMatrix lhs, in TransformMatrix rhs)
        => new(
            lhs * new Vector4(rhs.XColumn, 0.0f),
            lhs * new Vector4(rhs.YColumn, 0.0f),
            lhs * new Vector4(rhs.ZColumn, 0.0f),
            lhs * new Vector4(rhs.Translation, 1.0f));

    /// <summary> Converts the given transform matrix into an equivalent <see cref="Matrix4x4"/>. </summary>
    /// <remarks> The resulting matrix will be transposed, to fit how transforms are represented as <see cref="Matrix4x4"/>. </remarks>
    public static implicit operator Matrix4x4(in TransformMatrix mat)
        => new(
            mat.XRow.X, mat.YRow.X, mat.ZRow.X, 0.0f,
            mat.XRow.Y, mat.YRow.Y, mat.ZRow.Y, 0.0f,
            mat.XRow.Z, mat.YRow.Z, mat.ZRow.Z, 0.0f,
            mat.XRow.W, mat.YRow.W, mat.ZRow.W, 1.0f);

    /// <summary> Converts the given <see cref="Matrix4x4"/> into an equivalent transform matrix, if possible. </summary>
    /// <remarks> The resulting matrix will be transposed, due to how transforms are represented as <see cref="Matrix4x4"/>. </remarks>
    public static explicit operator TransformMatrix(in Matrix4x4 mat)
    {
        if (mat.M14 != 0.0f || mat.M24 != 0.0f || mat.M34 != 0.0f || mat.M44 != 1.0f)
            throw new InvalidCastException($"Invalid transform matrix, expected the fourth column to be <0 0 0 1>, found <{mat.M14} {mat.M24} {mat.M34} {mat.M44}>");

        return new(
            new Vector4(mat.M11, mat.M21, mat.M31, mat.M41),
            new Vector4(mat.M12, mat.M22, mat.M32, mat.M42),
            new Vector4(mat.M13, mat.M23, mat.M33, mat.M43));
    }
}
