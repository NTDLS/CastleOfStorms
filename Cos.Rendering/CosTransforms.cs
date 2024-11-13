using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;

namespace Cos.Rendering
{
    internal static class CosTransforms
    {
        private static readonly Dictionary<RenderTarget, Stack<Matrix3x2>> _transformStack = new();

        public static void RegisterRenderTarget(RenderTarget renderTarget)
        {
            //I add the render target to the dictonary here because I don't wan't to checkeach time I render a frame.
            _transformStack.Add(renderTarget, new Stack<Matrix3x2>());
        }


        public static Matrix3x2 CreateOffsetTransform(float x, float y)
        {
            // Create a translation transform to move the drawing to the desired position;
            return new(1.0f, 0.0f, 0.0f, 1.0f, x, y);
        }

        public static RawMatrix3x2 CreateAngleTransform(RawRectangleF rect, float angleRadians)
        {
            float centerX = rect.Left + (rect.Right - rect.Left) / 2.0f;
            float centerY = rect.Top + (rect.Bottom - rect.Top) / 2.0f;

            // Calculate the rotation matrix
            float cosAngle = (float)Math.Cos(angleRadians);
            float sinAngle = (float)Math.Sin(angleRadians);

            return new RawMatrix3x2(
                cosAngle, sinAngle,
                -sinAngle, cosAngle,
                centerX - cosAngle * centerX + sinAngle * centerY,
                centerY - sinAngle * centerX - cosAngle * centerY
            );
        }

        public static RawMatrix3x2 CreateScaleTransform(float scale, Vector2 centerPoint)
        {
            // Create a scale matrix at the specified center point
            return Matrix3x2.Scaling(scale, scale, centerPoint);
        }

        public static RawMatrix3x2 CreatePanAndZoomTransform(float scaleX, float scaleY, Vector2 panOffset, Vector2 zoomCenter)
        {
            // Create a scaling matrix around the zoom center
            var scalingMatrix = Matrix3x2.Scaling(scaleX, scaleY, zoomCenter);

            // Create a translation matrix for panning
            var translationMatrix = Matrix3x2.Translation(panOffset);

            // Combine the scaling and translation matrices
            var combinedMatrix = Matrix3x2.Multiply(scalingMatrix, translationMatrix);

            return combinedMatrix;
        }

        public static void PushTransform(RenderTarget renderTarget, Matrix3x2 transform)
        {
            _transformStack[renderTarget].Push(renderTarget.Transform);
            renderTarget.Transform = transform;
        }

        public static void PopTransform(RenderTarget renderTarget)
            => renderTarget.Transform = _transformStack[renderTarget].Pop();

    }
}
