/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2023
 */
namespace Radiance.Data;

using ShaderSupport.Objects;

/// <summary>
/// Represents a ARGB Color
/// </summary>
/// <param name="A">Transparence channel</param>
/// <param name="R">Red value</param>
/// <param name="G">Green value</param>
/// <param name="B">Blue value</param>
public record Color(byte A, byte R, byte G, byte B)
{
    public static readonly Color White = new Color(255, 255, 255, 255);
    public static readonly Color Black = new Color(255, 0, 0, 0);
    public static readonly Color Red = new Color(255, 255, 0, 0);
    public static readonly Color Green = new Color(255, 0, 255, 0);
    public static readonly Color Blue = new Color(255, 0, 0, 255);
    public static readonly Color Yellow = new Color(255, 255, 255, 0);
    public static readonly Color Magenta = new Color(255, 255, 0, 255);
    public static readonly Color Cyan = new Color(255, 0, 255, 255);

    public static implicit operator Vec4ShaderObject(Color color)
        => new Vec4ShaderObject(
            $"vec4({color.R / 255f}, {color.G / 255f}, {color.B / 255f}, {color.A / 255f})"
        );
}