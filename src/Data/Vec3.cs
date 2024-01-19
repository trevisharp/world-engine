/* Author:  Leonardo Trevisan Silio
 * Date:    19/01/2024
 */
namespace Radiance.Data;

/// <summary>
/// Represents a three dimension float vector.
/// </summary>
public record Vec3(float X, float Y, float Z)
{
    public void Deconstruct(out float x, out float y, out float z)
    {
        x = this.X;
        y = this.Y;
        z = this.Z;
    }
    
    public static Vec3 operator +(Vec3 u, Vec3 v)
        => new Vec3(u.X + v.X, u.Y + v.Y, u.Z + v.Z);
    public static Vec3 operator -(Vec3 u, Vec3 v)
        => new Vec3(u.X - v.X, u.Y - v.Y, u.Z - v.Z);
    public static float operator *(Vec3 u, Vec3 v)
        => u.X * v.X + u.Y * v.Y + u.Z * v.Z;
    
    public static implicit operator Vec3((float x, float y, float z) tuple)
        => new Vec3(tuple.x, tuple.y, tuple.z);
}