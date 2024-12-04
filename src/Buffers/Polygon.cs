/* Author:  Leonardo Trevisan Silio
 * Date:    02/12/2024
 */
namespace Radiance.Buffers;

using System.Buffers;
using Internal;

/// <summary>
/// A buffer with a Polygon points.
/// </summary>
public class Polygon(float[] data) : IPolygon
{
    Buffer? buffer = null;
    BufferData? pointsPair = null;
    BufferData? boundPair = null;
    BufferData? triangulationPair = null;

    public int Rows => data.Length / 3;

    public int Columns => 3;
    
    public int Instances => 1;

    public int InstanceLength => Rows;

    public bool IsGeometry => true;

    public Buffer Buffer => buffer ??= Buffer.From(this);

    public IBufferedData Triangules
        => triangulationPair ??= FindTriangules();
    
    public IBufferedData Lines
        => boundPair ??= FindBounds();
    
    public IBufferedData Points
        => pointsPair ??= FindPoints();

    public float[] GetBufferData()
        => data[..];

    BufferData FindPoints()
    {
        var points = data[..];
        return CreateBuffer(points);
    }

    BufferData FindBounds()
    {
        var lines = Bounds
            .GetBounds(data[..]);
        
        return CreateBuffer(lines);
    }

    BufferData FindTriangules()
    {   
        var triangules = Triangulations
            .PlanarPolygonTriangulation(data[..]);
        
        return CreateBuffer(triangules);
    }

    static BufferData CreateBuffer(float[] points)
    {
        var bufferData = new BufferData(3, points.Length / 3, true);
        bufferData.AddRange(points);
        
        return bufferData;
    }

    public static implicit operator Polygon(float[] data) => new(data);

    public static RepeatPolygon operator *(Polygon polygon, int times)
        => new (polygon, times);

    public static RepeatPolygon operator *(int times, Polygon polygon)
        => new (polygon, times);
}