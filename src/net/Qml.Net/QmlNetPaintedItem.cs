using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using Qml.Net.Internal;

namespace Qml.Net
{
    public class QmlNetPaintedItem
    {
        private IntPtr _qmlNetPaintedItemRef;
        private INetQPainter _qPainter;

        public QmlNetPaintedItem(IntPtr qmlNetPaintedItemRef, IntPtr inetQPainterRef)
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
    }

    internal class QmlNetPaintedItemInterop
    {
        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_beginRecordPaintActions")]
        public BeginRecordPaintActionsDel BeginRecordPaintActions { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void BeginRecordPaintActionsDel(IntPtr paintedItem);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_endRecordPaintActions")]
        public EndRecordPaintActionsDel EndRecordPaintActions { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void EndRecordPaintActionsDel(IntPtr paintedItem);
    }
}
