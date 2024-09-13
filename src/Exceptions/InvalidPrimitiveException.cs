/* Author:  Leonardo Trevisan Silio
 * Date:    13/09/2024
 */
using System;

namespace Radiance.Exceptions;

public class InvalidPrimitiveException(object value) : RadianceException
{
    public override string ErrorMessage =>
        $"""
        A render was called with a invalid primitive type {value}.
        Use only int, float, double, Vec2, Vec3, Vec4, float[] or Texture types.
        """;
}