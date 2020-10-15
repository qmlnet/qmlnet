using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using Qml.Net.Internal;
using static System.Runtime.InteropServices.UnmanagedType;

// ReSharper disable InconsistentNaming

namespace Qml.Net
{
    public class NetQPainter
    {
        private IntPtr _qPainterRef;
        private Dictionary<string, int> _registeredColors = new Dictionary<string, int>();
        private Dictionary<string, int> _registeredFontFamilies = new Dictionary<string, int>();

        public NetQPainter(IntPtr qPainterRef)
        {
            _qPainterRef = qPainterRef;
        }

        public void SetPen(string colorString, int width)
        {
            var colorId = GetColorId(colorString);
            Interop.NetQPainter.SetPen(_qPainterRef, colorId, width);
        }

        public void ResetPen()
        {
            Interop.NetQPainter.ResetPen(_qPainterRef);
        }

        public void SetBrush(string colorString)
        {
            var colorId = GetColorId(colorString);
            Interop.NetQPainter.SetBrush(_qPainterRef, colorId);
        }

        public void ResetBrush()
        {
            Interop.NetQPainter.ResetBrush(_qPainterRef);
        }

        public void SetFont(string fontFamilyName, bool isBold, bool isItalic, bool isUnderline, int pxSize)
        {
            int fontFamilyId = GetFontFamilyId(fontFamilyName);
            Interop.NetQPainter.SetFont(_qPainterRef, fontFamilyId, isBold, isItalic, isUnderline, pxSize);
        }

        public void SetFontFamily(string fontFamilyName)
        {
            int fontFamilyId = GetFontFamilyId(fontFamilyName);
            Interop.NetQPainter.SetFontFamily(_qPainterRef, fontFamilyId);
        }

        public void SetFontBold(bool isBold)
        {
            Interop.NetQPainter.SetFontBold(_qPainterRef, isBold);
        }

        public void SetFontItalic(bool isItalic)
        {
            Interop.NetQPainter.SetFontItalic(_qPainterRef, isItalic);
        }

        public void SetFontUnderline(bool isUnderline)
        {
            Interop.NetQPainter.SetFontUnderline(_qPainterRef, isUnderline);
        }

        public void SetFontSize(int pxSize)
        {
            Interop.NetQPainter.SetFontSize(_qPainterRef, pxSize);
        }

        public void DrawText(int x, int y, string text)
        {
            Interop.NetQPainter.DrawText(_qPainterRef, x, y, text);
        }

        [Flags]
        public enum DrawTextFlags
        {
            None = 0x0000,
            AlignLeft = 0x0001,
            AlignRight = 0x0002,
            AlignHCenter = 0x0004,
            AlignJustify = 0x0008,
            AlignTop = 0x0020,
            AlignBottom = 0x0040,
            AlignVCenter = 0x0080,
            AlignCenter = AlignVCenter | AlignHCenter,
            TextSingleLine = 0x0100,
            TextDontClip = 0x0200,
            TextExpandTabs = 0x0400,
            TextShowMnemonic = 0x0800,
            TextWordWrap = 0x1000,
            TextIncludeTrailingSpaces = 0x08000000
        }

        public void DrawText(int x, int y, int width, int height, DrawTextFlags flags, string text)
        {
            Interop.NetQPainter.DrawTextRect(_qPainterRef, x, y, width, height, (int) flags, text);
        }

        public void DrawRect(int x, int y, int width, int height)
        {
            Interop.NetQPainter.DrawRect(_qPainterRef, x, y, width, height);
        }

        public void FillRect(int x, int y, int width, int height, string color)
        {
            var colorId = GetColorId(color);
            Interop.NetQPainter.FillRectColor(_qPainterRef, x, y, width, height, colorId);
        }

        public void FillRect(int x, int y, int width, int height)
        {
            Interop.NetQPainter.FillRect(_qPainterRef, x, y, width, height);
        }

        private int RegisterColor(string colorString)
        {
            return Interop.NetQPainter.RegisterColor(_qPainterRef, colorString);
        }

        private void FreeColor(int colorId)
        {
            Interop.NetQPainter.FreeColor(_qPainterRef, colorId);
        }

        private int RegisterFontFamily(string fontFamily)
        {
            return Interop.NetQPainter.RegisterFontFamily(_qPainterRef, fontFamily);
        }

        private void FreeFontFamily(int fontFamilyId)
        {
            Interop.NetQPainter.FreeFontFamily(_qPainterRef, fontFamilyId);
        }

        public static Size GetStringSize(string fontFamily, int fontSizePx, string text)
        {
            var res = Interop.NetQPainter.GetStringSize(fontFamily, fontSizePx, text);
            return new Size(res.width, res.height);
        }

        public void DrawArc(int x, int y, int width, int height, int startAngle, int spanAngle)
        {
            Interop.NetQPainter.DrawArc(_qPainterRef, x, y, width, height, startAngle, spanAngle);
        }

        public void DrawChord(int x, int y, int width, int height, int startAngle, int spanAngle)
        {
            Interop.NetQPainter.DrawChord(_qPainterRef, x, y, width, height, startAngle, spanAngle);
        }

        public void DrawConvexPolygon(Point[] points)
        {
            var count = points.Length;
            var pointsArray = new NetQPainterInterop.netqpainter_Point[count];
            for (var i = 0; i < count; i++)
            {
                var p = points[i];
                pointsArray[i] = new NetQPainterInterop.netqpainter_Point()
                {
                    x = p.X,
                    y = p.Y
                };
            }

            Interop.NetQPainter.DrawConvexPolygon(_qPainterRef, pointsArray, pointsArray.Length);
        }

        public void DrawEllipse(int x, int y, int width, int height)
        {
            Interop.NetQPainter.DrawEllipse(_qPainterRef, x, y, width, height);
        }

        public enum ImageConversionFlag
        {
            None = 0x00000000,
            ColorMode_Mask = 0x00000003,
            AutoColor = 0x00000000,
            ColorOnly = 0x00000003,
            MonoOnly = 0x00000002,

            AlphaDither_Mask = 0x0000000c,
            ThresholdAlphaDither = 0x00000000,
            OrderedAlphaDither = 0x00000004,
            DiffuseAlphaDither = 0x00000008,
            NoAlpha = 0x0000000c, // Not supported

            Dither_Mask = 0x00000030,
            DiffuseDither = 0x00000000,
            OrderedDither = 0x00000010,
            ThresholdDither = 0x00000020,

            DitherMode_Mask = 0x000000c0,
            AutoDither = 0x00000000,
            PreferDither = 0x00000040,
            AvoidDither = 0x00000080,

            NoOpaqueDetection = 0x00000100,
            NoFormatConversion = 0x00000200
        }

        public void DrawImage(Point start, string imagePath, Rectangle source, ImageConversionFlag flags)
        {
            var p = new NetQPainterInterop.netqpainter_Point()
            {
                x = start.X,
                y = start.Y
            };
            var sourceRect = new NetQPainterInterop.netqpainter_Rect()
            {
                x = source.X,
                y = source.Y,
                width = source.Width,
                height = source.Height
            };
            Interop.NetQPainter.DrawImageFile(_qPainterRef, p, imagePath, sourceRect, flags);
        }
        
        public void DrawImage(Point start, byte[] imageData, Rectangle source, ImageConversionFlag flags)
        {
            var p = new NetQPainterInterop.netqpainter_Point()
            {
                x = start.X,
                y = start.Y
            };
            var sourceRect = new NetQPainterInterop.netqpainter_Rect()
            {
                x = source.X,
                y = source.Y,
                width = source.Width,
                height = source.Height
            };
            Interop.NetQPainter.DrawImage(_qPainterRef, p, imageData, imageData.Length, sourceRect, flags);
        }

        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            Interop.NetQPainter.DrawLine(_qPainterRef, x1, y1, x2, y2);
        }

        public void DrawPie(int x, int y, int width, int height, int startAngle, int spanAngle)
        {
            Interop.NetQPainter.DrawPie(_qPainterRef, x, y, width, height, startAngle, spanAngle);
        }

        public void DrawPoint(int x, int y)
        {
            Interop.NetQPainter.DrawPoint(_qPainterRef, x, y);
        }

        public void DrawPolygon(Point[] points, bool oddFill)
        {
            var count = points.Length;
            var pointsArray = new NetQPainterInterop.netqpainter_Point[count];
            for (var i = 0; i < count; i++)
            {
                var p = points[i];
                pointsArray[i] = new NetQPainterInterop.netqpainter_Point()
                {
                    x = p.X,
                    y = p.Y
                };
            }

            Interop.NetQPainter.DrawPolygon(_qPainterRef, pointsArray, pointsArray.Length, oddFill);
        }

        public void DrawPolyline(Point[] points)
        {
            var count = points.Length;
            var pointsArray = new NetQPainterInterop.netqpainter_Point[count];
            for (var i = 0; i < count; i++)
            {
                var p = points[i];
                pointsArray[i] = new NetQPainterInterop.netqpainter_Point()
                {
                    x = p.X,
                    y = p.Y
                };
            }

            Interop.NetQPainter.DrawPolyline(_qPainterRef, pointsArray, pointsArray.Length);
        }

        public void DrawRoundedRect(int x, int y, int w, int h, double xRadius, double yRadius, bool absoluteSize)
        {
            Interop.NetQPainter.DrawRoundedRect(_qPainterRef, x, y, w, h, xRadius, yRadius, absoluteSize);
        }

        public void EraseRect(int x, int y, int width, int height)
        {
            Interop.NetQPainter.EraseRect(_qPainterRef, x, y, width, height);
        }

        public void SetBackground(string color)
        {
            var colorId = GetColorId(color);
            Interop.NetQPainter.SetBackground(_qPainterRef, colorId);
        }

        public void SetBackgroundMode(bool opaque)
        {
            Interop.NetQPainter.SetBackgroundMode(_qPainterRef, opaque);
        }

        public enum ClipOperation
        {
            NoClip = 1,
            ReplaceClip,
            IntersectClip
        }

        public void SetClipRect(int x, int y, int width, int height, ClipOperation operation)
        {
            Interop.NetQPainter.SetClipRect(_qPainterRef, x, y, width, height, operation);
        }

        public void SetClipping(bool enable)
        {
            Interop.NetQPainter.SetClipping(_qPainterRef, enable);
        }

        public enum CompositionMode
        {
            CompositionMode_SourceOver = 1,
            CompositionMode_DestinationOver,
            CompositionMode_Clear,
            CompositionMode_Source,
            CompositionMode_Destination,
            CompositionMode_SourceIn,
            CompositionMode_DestinationIn,
            CompositionMode_SourceOut,
            CompositionMode_DestinationOut,
            CompositionMode_SourceAtop,
            CompositionMode_DestinationAtop,
            CompositionMode_Xor,

            // svg 1.2 blend modes
            CompositionMode_Plus,
            CompositionMode_Multiply,
            CompositionMode_Screen,
            CompositionMode_Overlay,
            CompositionMode_Darken,
            CompositionMode_Lighten,
            CompositionMode_ColorDodge,
            CompositionMode_ColorBurn,
            CompositionMode_HardLight,
            CompositionMode_SoftLight,
            CompositionMode_Difference,
            CompositionMode_Exclusion,

            // ROPs
            RasterOp_SourceOrDestination,
            RasterOp_SourceAndDestination,
            RasterOp_SourceXorDestination,
            RasterOp_NotSourceAndNotDestination,
            RasterOp_NotSourceOrNotDestination,
            RasterOp_NotSourceXorDestination,
            RasterOp_NotSource,
            RasterOp_NotSourceAndDestination,
            RasterOp_SourceAndNotDestination,
            RasterOp_NotSourceOrDestination,
            RasterOp_SourceOrNotDestination,
            RasterOp_ClearDestination,
            RasterOp_SetDestination,
            RasterOp_NotDestination
        }

        public void SetCompositionMode(CompositionMode mode)
        {
            Interop.NetQPainter.SetCompositionMode(_qPainterRef, mode);
        }

        public enum LayoutDirection
        {
            LeftToRight = 1,
            RightToLeft,
            LayoutDirectionAuto
        }

        public void SetLayoutDirection(LayoutDirection direction)
        {
            Interop.NetQPainter.SetLayoutDirection(_qPainterRef, direction);
        }

        public void SetOpacity(double opacity)
        {
            Interop.NetQPainter.SetOpacity(_qPainterRef, opacity);
        }

        public enum RenderHint
        {
            Antialiasing = 0x01,
            TextAntialiasing = 0x02,
            SmoothPixmapTransform = 0x04,
            Qt4CompatiblePainting = 0x20,
            LosslessImageRendering = 0x40,
        }

        public void SetRenderHint(RenderHint hint, bool on)
        {
            Interop.NetQPainter.SetRenderHint(_qPainterRef, hint, on);
        }

        public void Shear(double sh, double sv)
        {
            Interop.NetQPainter.Shear(_qPainterRef, sh, sv);
        }

        public void Translate(double dx, double dy)
        {
            Interop.NetQPainter.Translate(_qPainterRef, dx, dy);
        }

        private int GetColorId(string colorString)
        {
            if (_registeredColors.ContainsKey(colorString))
            {
                return _registeredColors[colorString];
            }

            var colorId = RegisterColor(colorString);
            _registeredColors.Add(colorString, colorId);
            return colorId;
        }

        private int GetFontFamilyId(string fontFamilyName)
        {
            if (_registeredFontFamilies.ContainsKey(fontFamilyName))
            {
                return _registeredFontFamilies[fontFamilyName];
            }

            var fontFamilyId = RegisterFontFamily(fontFamilyName);
            _registeredFontFamilies.Add(fontFamilyName, fontFamilyId);
            return fontFamilyId;
        }
    }

    internal class NetQPainterInterop
    {
        [NativeSymbol(Entrypoint = "netqpainter_setPen")]
        public SetPenDel SetPen { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetPenDel(IntPtr paintedItem, int colorId, int width);

        [NativeSymbol(Entrypoint = "netqpainter_resetPen")]
        public ResetPenDel ResetPen { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ResetPenDel(IntPtr paintedItem);

        [NativeSymbol(Entrypoint = "netqpainter_setBrush")]
        public SetBrushDel SetBrush { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetBrushDel(IntPtr paintedItem, int colorId);

        [NativeSymbol(Entrypoint = "netqpainter_resetBrush")]
        public ResetBrushDel ResetBrush { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ResetBrushDel(IntPtr paintedItem);

        [NativeSymbol(Entrypoint = "netqpainter_setFont")]
        public SetFontDel SetFont { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontDel(IntPtr paintedItem, int fontFamilyId, bool isBold, bool isItalic,
            bool isUnderline, int pxSize);

        [NativeSymbol(Entrypoint = "netqpainter_setFontFamily")]
        public SetFontFamilyDel SetFontFamily { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontFamilyDel(IntPtr paintedItem, int fontFamilyId);

        [NativeSymbol(Entrypoint = "netqpainter_setFontBold")]
        public SetFontBoldDel SetFontBold { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontBoldDel(IntPtr paintedItem, bool isBold);

        [NativeSymbol(Entrypoint = "netqpainter_setFontItalic")]
        public SetFontItalicDel SetFontItalic { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontItalicDel(IntPtr paintedItem, bool isItalic);

        [NativeSymbol(Entrypoint = "netqpainter_setFontUnderline")]
        public SetFontUnderlineDel SetFontUnderline { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontUnderlineDel(IntPtr paintedItem, bool isUnderline);

        [NativeSymbol(Entrypoint = "netqpainter_setFontSize")]
        public SetFontSizeDel SetFontSize { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontSizeDel(IntPtr paintedItem, int pxSize);

        [NativeSymbol(Entrypoint = "netqpainter_drawText")]
        public DrawTextDel DrawText { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawTextDel(IntPtr paintedItem, int x, int y,
            [MarshalAs(LPWStr)] string text);

        [NativeSymbol(Entrypoint = "netqpainter_drawTextRect")]
        public DrawTextRectDel DrawTextRect { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawTextRectDel(IntPtr paintedItem, int x, int y, int width, int height, int flags,
            [MarshalAs(LPWStr)] string text);

        [NativeSymbol(Entrypoint = "netqpainter_drawRect")]
        public DrawRectDel DrawRect { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawRectDel(IntPtr paintedItem, int x, int y, int width, int height);

        [NativeSymbol(Entrypoint = "netqpainter_fillRectColor")]
        public FillRectColorDel FillRectColor { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FillRectColorDel(IntPtr paintedItem, int x, int y, int width, int height, int colorId);

        [NativeSymbol(Entrypoint = "netqpainter_fillRect")]
        public FillRectDel FillRect { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FillRectDel(IntPtr paintedItem, int x, int y, int width, int height);

        [NativeSymbol(Entrypoint = "netqpainter_registerColor")]
        public RegisterColorDel RegisterColor { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int RegisterColorDel(IntPtr paintedItem, [MarshalAs(LPWStr)] string colorString);

        [NativeSymbol(Entrypoint = "netqpainter_freeColor")]
        public FreeColorDel FreeColor { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FreeColorDel(IntPtr paintedItem, int colorId);

        [NativeSymbol(Entrypoint = "netqpainter_registerFontFamily")]
        public RegisterFontFamilyDel RegisterFontFamily { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int RegisterFontFamilyDel(IntPtr paintedItem,
            [MarshalAs(LPWStr)] string fontFamilyString);

        [NativeSymbol(Entrypoint = "netqpainter_freeFontFamily")]
        public FreeFontFamilyDel FreeFontFamily { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FreeFontFamilyDel(IntPtr paintedItem, int fontFamilyId);

        [StructLayout(LayoutKind.Sequential)]
        public struct StringSizeResult
        {
            public int width;
            public int height;
        }

        [NativeSymbol(Entrypoint = "netqpainter_getStringSize")]
        public GetStringSizeDel GetStringSize { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate StringSizeResult GetStringSizeDel([MarshalAs(LPWStr)] string fontFamily, int sizePx,
            [MarshalAs(LPWStr)] string text);

        [NativeSymbol(Entrypoint = "netqpainter_drawArc")]
        public DrawArcDel DrawArc { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawArcDel(IntPtr paintedItem, int x, int y, int width, int height, int startAngle,
            int spanAngle);

        [NativeSymbol(Entrypoint = "netqpainter_drawChord")]
        public DrawChordDel DrawChord { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawChordDel(IntPtr paintedItem, int x, int y, int width, int height, int startAngle,
            int spanAngle);

        [StructLayout(LayoutKind.Sequential)]
        public struct netqpainter_Point
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct netqpainter_Rect
        {
            public int x;
            public int y;
            public int width;
            public int height;
        }

        [NativeSymbol(Entrypoint = "netqpainter_drawConvexPolygon")]
        public DrawConvexPolygonDel DrawConvexPolygon { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawConvexPolygonDel(IntPtr paintedItem, netqpainter_Point[] points, int pointCount);

        [NativeSymbol(Entrypoint = "netqpainter_drawEllipse")]
        public DrawEllipseDel DrawEllipse { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawEllipseDel(IntPtr paintedItem, int x, int y, int width, int height);

        [NativeSymbol(Entrypoint = "netqpainter_drawImage")]
        public DrawImageDel DrawImage { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawImageDel(IntPtr paintedItem, netqpainter_Point point, byte[] imgData, int imgDataSize,
            netqpainter_Rect sourceRect, NetQPainter.ImageConversionFlag flags);

        [NativeSymbol(Entrypoint = "netqpainter_drawImageFile")]
        public DrawImageFileDel DrawImageFile { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawImageFileDel(IntPtr paintedItem, netqpainter_Point point, [MarshalAs(LPWStr)] string imageFilePath,
            netqpainter_Rect sourceRect, NetQPainter.ImageConversionFlag flags);

        [NativeSymbol(Entrypoint = "netqpainter_drawLine")]
        public DrawLineDel DrawLine { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawLineDel(IntPtr paintedItem, int x1, int y1, int x2, int y2);

        [NativeSymbol(Entrypoint = "netqpainter_drawPie")]
        public DrawPieDel DrawPie { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawPieDel(IntPtr paintedItem, int x, int y, int width, int height, int startAngle,
            int spanAngle);

        [NativeSymbol(Entrypoint = "netqpainter_drawPoint")]
        public DrawPointDel DrawPoint { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawPointDel(IntPtr paintedItem, int x, int y);

        [NativeSymbol(Entrypoint = "netqpainter_drawPolygon")]
        public DrawPolygonDel DrawPolygon { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawPolygonDel(IntPtr paintedItem, netqpainter_Point[] points, int pointCount,
            bool oddFill);

        [NativeSymbol(Entrypoint = "netqpainter_drawPolyline")]
        public DrawPolylineDel DrawPolyline { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawPolylineDel(IntPtr paintedItem, netqpainter_Point[] points, int pointCount);

        [NativeSymbol(Entrypoint = "netqpainter_drawRoundedRect")]
        public DrawRoundedRectDel DrawRoundedRect { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawRoundedRectDel(IntPtr paintedItem, int x, int y, int w, int h, double xRadius,
            double yRadius, bool absoluteSize);

        [NativeSymbol(Entrypoint = "netqpainter_eraseRect")]
        public EraseRectDel EraseRect { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void EraseRectDel(IntPtr paintedItem, int x, int y, int width, int height);

        [NativeSymbol(Entrypoint = "netqpainter_setBackground")]
        public SetBackgroundDel SetBackground { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetBackgroundDel(IntPtr paintedItem, int colorId);

        [NativeSymbol(Entrypoint = "netqpainter_setBackgroundMode")]
        public SetBackgroundModeDel SetBackgroundMode { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetBackgroundModeDel(IntPtr paintedItem, bool opaque);

        [NativeSymbol(Entrypoint = "netqpainter_setClipRect")]
        public SetClipRectDel SetClipRect { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetClipRectDel(IntPtr paintedItem, int x, int y, int width, int height,
            NetQPainter.ClipOperation operation);

        [NativeSymbol(Entrypoint = "netqpainter_setClipping")]
        public SetClippingDel SetClipping { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetClippingDel(IntPtr paintedItem, bool enable);

        [NativeSymbol(Entrypoint = "netqpainter_setCompositionMode")]
        public SetCompositionModeDel SetCompositionMode { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetCompositionModeDel(IntPtr paintedItem, NetQPainter.CompositionMode mode);

        [NativeSymbol(Entrypoint = "netqpainter_setLayoutDirection")]
        public SetLayoutDirectionDel SetLayoutDirection { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetLayoutDirectionDel(IntPtr paintedItem, NetQPainter.LayoutDirection dir);

        [NativeSymbol(Entrypoint = "netqpainter_setOpacity")]
        public SetOpacityDel SetOpacity { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetOpacityDel(IntPtr paintedItem, double opacity);

        [NativeSymbol(Entrypoint = "netqpainter_setRenderHint")]
        public SetRenderHintDel SetRenderHint { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetRenderHintDel(IntPtr paintedItem, NetQPainter.RenderHint hint, bool on);
        
        [NativeSymbol(Entrypoint = "netqpainter_shear")]
        public ShearDel Shear { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ShearDel(IntPtr paintedItem, double sh, double sv);

        [NativeSymbol(Entrypoint = "netqpainter_translate")]
        public TranslateDel Translate { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void TranslateDel(IntPtr paintedItem, double dx, double dy);
    }
}