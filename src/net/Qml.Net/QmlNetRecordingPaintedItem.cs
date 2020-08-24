using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using Qml.Net.Internal;

namespace Qml.Net
{
    public class QmlNetRecordingPaintedItem
    {
        private IntPtr _qmlNetPaintedItemRef;
        private INetQPainter _qPainter;

        public QmlNetRecordingPaintedItem(IntPtr qmlNetPaintedItemRef, IntPtr inetQPainterRef)
        {
            _qmlNetPaintedItemRef = qmlNetPaintedItemRef;
            _qPainter = new INetQPainter(inetQPainterRef);
        }

        public void BeginRecordPaintActions()
        {
            Interop.QmlNetPaintedItem.BeginRecordPaintActions(_qmlNetPaintedItemRef);
        }

        public void EndRecordPaintActions()
        {
            Interop.QmlNetPaintedItem.EndRecordPaintActions(_qmlNetPaintedItemRef);
        }

        public void SetPen(string colorString)
        {
            _qPainter.SetPen(colorString);
        }

        public void ResetPen()
        {
            _qPainter.ResetPen();
        }

        public void SetBrush(string colorString)
        {
            _qPainter.SetBrush(colorString);
        }

        public void ResetBrush()
        {
            _qPainter.ResetBrush();
        }

        public void SetFont(string fontFamilyName, bool isBold, bool isItalic, bool isUnderline, int pxSize)
        {
            _qPainter.SetFont(fontFamilyName, isBold, isItalic, isUnderline, pxSize);
        }

        public void SetFontFamily(string fontFamilyName)
        {
            _qPainter.SetFontFamily(fontFamilyName);
        }

        public void SetFontBold(bool isBold)
        {
            _qPainter.SetFontBold(isBold);
        }

        public void SetFontItalic(bool isItalic)
        {
            _qPainter.SetFontItalic(isItalic);
        }

        public void SetFontUnderline(bool isUnderline)
        {
            _qPainter.SetFontUnderline(isUnderline);
        }

        public void SetFontSize(int pxSize)
        {
            _qPainter.SetFontSize(pxSize);
        }

        public void DrawText(int x, int y, string text)
        {
            _qPainter.DrawText(x, y, text);
        }
        
        public void DrawText(int x, int y, int width, int height, INetQPainter.DrawTextFlags flags, string text)
        {
            _qPainter.DrawText(x, y, width, height, flags, text);
        }

        public void DrawRect(int x, int y, int width, int height)
        {
            _qPainter.DrawRect(x, y, width, height);
        }

        public void FillRect(int x, int y, int width, int height, string color)
        {
            _qPainter.FillRect(x, y, width, height, color);
        }

        public void FillRect(int x, int y, int width, int height)
        {
            _qPainter.FillRect(x, y, width, height);
        }
        
        public Size GetStringSize(string fontFamily, int fontSizePx, string text)
        {
            return _qPainter.GetStringSize(fontFamily, fontSizePx, text);
        }
        
        public void DrawArc(int x, int y, int width, int height, int startAngle, int spanAngle)
        {
            _qPainter.DrawArc(x, y, width, height, startAngle, spanAngle);
        }

        public void DrawChord(int x, int y, int width, int height, int startAngle, int spanAngle)
        {
            _qPainter.DrawChord(x, y, width, height, startAngle, spanAngle);
        }
        
        public void DrawConvexPolygon(Point[] points)
        {
            _qPainter.DrawConvexPolygon(points);
        }

        public void DrawEllipse(int x, int y, int width, int height)
        {
            _qPainter.DrawEllipse(x, y, width, height);
        }
        
        public void DrawImage(Point start, byte[] imageData, Rectangle source, INetQPainter.ImageConversionFlag flags)
        {
            _qPainter.DrawImage(start, imageData, source, flags);
        }

        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            _qPainter.DrawLine(x1, y1, x2, y2);
        }

        public void DrawPie(int x, int y, int width, int height, int startAngle, int spanAngle)
        {
            _qPainter.DrawPie(x, y, width, height, startAngle, spanAngle);
        }

        public void DrawPoint(int x, int y)
        {
            _qPainter.DrawPoint(x, y);
        }
       
        public void DrawPolygon(Point[] points, bool oddFill)
        {
            _qPainter.DrawPolygon(points, oddFill);
        }
        
        public void DrawPolyline(Point[] points)
        {
            _qPainter.DrawPolyline(points);
        }

        public void DrawRoundedRect(int x, int y, int w, int h, double xRadius, double yRadius, bool absoluteSize)
        {
            _qPainter.DrawRoundedRect(x, y, w, h, xRadius, yRadius, absoluteSize);
        }

        public void EraseRect(int x, int y, int width, int height)
        {
            _qPainter.EraseRect(x, y, width, height);
        }

        public void SetBackground(string color)
        {
            _qPainter.SetBackground(color);
        }

        public void SetBackgroundMode(bool opaque)
        {
            _qPainter.SetBackgroundMode(opaque);
        }
        
        public void SetClipRect(int x, int y, int width, int height, INetQPainter.ClipOperation operation)
        {
            _qPainter.SetClipRect(x, y, width, height, operation);
        }

        public void SetClipping(bool enable)
        {
            _qPainter.SetClipping(enable);
        }

        public void SetCompositionMode(INetQPainter.CompositionMode mode)
        {
            _qPainter.SetCompositionMode(mode);
        }

        public void SetLayoutDirection(INetQPainter.LayoutDirection direction)
        {
            _qPainter.SetLayoutDirection(direction);
        }

        public void SetOpacity(double opacity)
        {
            _qPainter.SetOpacity(opacity);
        }

        public void SetRenderHint(INetQPainter.RenderHint hint, bool on)
        {
            _qPainter.SetRenderHint(hint, on);
        }

        public void SetTransform(double h11, double h12, double h13, double h21, double h22, double h23, double h31, double h32, double h33, bool combine)
        {
            _qPainter.SetTransform(h11, h12, h13, h21, h22, h23, h31, h32, h33, combine);
        }

        public void SetViewTransformEnabled(bool enable)
        {
            _qPainter.SetViewTransformEnabled(enable);
        }

        public void SetWorldTransform(double h11, double h12, double h13, double h21, double h22, double h23, double h31, double h32, double h33, bool combine)
        {
            _qPainter.SetWorldTransform(h11, h12, h13, h21, h22, h23, h31, h32, h33, combine);
        }

        public void SetWorldMatrixEnabled(bool enable)
        {
            _qPainter.SetWorldMatrixEnabled(enable);
        }

        public void Shear(double sh, double sv)
        {
            _qPainter.Shear(sh, sv);
        }
        
        public void Translate(double dx, double dy)
        {
            _qPainter.Translate(dx, dy);
        }
    }

    internal class QmlNetPaintedItemInterop
    {
        [NativeSymbol(Entrypoint = "qqmlnetrecordingpainteditem_beginRecordPaintActions")]
        public BeginRecordPaintActionsDel BeginRecordPaintActions { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void BeginRecordPaintActionsDel(IntPtr paintedItem);

        [NativeSymbol(Entrypoint = "qqmlnetrecordingpainteditem_endRecordPaintActions")]
        public EndRecordPaintActionsDel EndRecordPaintActions { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void EndRecordPaintActionsDel(IntPtr paintedItem);
    }
}
