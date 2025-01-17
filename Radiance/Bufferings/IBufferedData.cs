/* Author:  Leonardo Trevisan Silio
 * Date:    27/12/2024
 */
namespace Radiance.Bufferings;

/// <summary>
/// Represent a object data that can be sended to a GPU Buffer.
/// </summary>
public interface IBufferedData
{
    /// <summary>
    /// Get the count of rows.
    /// </summary>
    int Rows { get; }
    
    /// <summary>
    /// Get the size of float value for each row.
    /// </summary>
    int Columns { get; }

    /// <summary>
    /// Get the number of instances this data represents.
    /// </summary>
    int Instances { get; }

    /// <summary>
    /// The number of rows per instance.
    /// If InstanceLength * Instance is different of Instance,
    /// so Radiance know that data repeats automatically.
    /// </summary>
    int InstanceLength { get; }

    /// <summary>
    /// Get if this data represents a geometry.
    /// </summary>
    bool IsGeometry { get; }

    /// <summary>
    /// Get the associated buffer.
    /// </summary>
    Buffer Buffer { get; }

    /// <summary>
    /// The object of changes of buffered data.
    /// </summary>
    Changes Changes { get; }

    /// <summary>
    /// Generate the data of this buffer.
    /// Prefer to not modify this vector to avoid incorrect behaviours.
    /// </summary>
    float[] GetBufferData();

    /// <summary>
    /// Get or Set the raw data.
    /// </summary>
    float this[int index] { get; set; }
        
    public static VirtualBufferData operator *(int times, IBufferedData stream)
        => new(stream, times);
    
    public static VirtualBufferData operator *(IBufferedData stream, int times)
        => new(stream, times);
}