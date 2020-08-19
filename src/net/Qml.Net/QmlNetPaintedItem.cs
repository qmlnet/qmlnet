using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using Qml.Net.Internal;

namespace Qml.Net
{
    public class QmlNetPaintedItem
    {
        private IntPtr _qmlNetPaintedItemRef;
        private Dictionary<string, int> _registeredColors = new Dictionary<string, int>();

        public QmlNetPaintedItem(IntPtr qmlNetPaintedItemRef)
        {
            _qmlNetPaintedItemRef = qmlNetPaintedItemRef;
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
            var colorId = GetColorId(colorString);
            Interop.QmlNetPaintedItem.SetPen(_qmlNetPaintedItemRef, colorId);
        }

        public void ResetPen()
        {
            Interop.QmlNetPaintedItem.ResetPen(_qmlNetPaintedItemRef);
        }

        public void SetBrush(string colorString)
        {
            var colorId = GetColorId(colorString);
            Interop.QmlNetPaintedItem.SetBrush(_qmlNetPaintedItemRef, colorId);
        }

        public void ResetBrush()
        {
            Interop.QmlNetPaintedItem.ResetBrush(_qmlNetPaintedItemRef);
        }

        public void SetFont(string fontFamilyName, bool isBold, bool isItalic, bool isUnderline, int pxSize)
        {
            Interop.QmlNetPaintedItem.SetFont(_qmlNetPaintedItemRef, fontFamilyName, isBold, isItalic, isUnderline, pxSize);
        }

        public void SetFontFamily(string fontFamilyName)
        {
            Interop.QmlNetPaintedItem.SetFontFamily(_qmlNetPaintedItemRef, fontFamilyName);
        }

        public void SetFontBold(bool isBold)
        {
            Interop.QmlNetPaintedItem.SetFontBold(_qmlNetPaintedItemRef, isBold);
        }

        public void SetFontItalic(bool isItalic)
        {
            Interop.QmlNetPaintedItem.SetFontItalic(_qmlNetPaintedItemRef, isItalic);
        }

        public void SetFontUnderline(bool isUnderline)
        {
            Interop.QmlNetPaintedItem.SetFontUnderline(_qmlNetPaintedItemRef, isUnderline);
        }

        public void SetFontSize(int pxSize)
        {
            Interop.QmlNetPaintedItem.SetFontSize(_qmlNetPaintedItemRef, pxSize);
        }

        public void DrawText(int x, int y, string text)
        {
            Interop.QmlNetPaintedItem.DrawText(_qmlNetPaintedItemRef, x, y, text);
        }

        public void DrawRect(int x, int y, int width, int height)
        {
            Interop.QmlNetPaintedItem.DrawRect(_qmlNetPaintedItemRef, x, y, width, height);
        }

        public void FillRect(int x, int y, int width, int height, int colorId)
        {
            Interop.QmlNetPaintedItem.FillRectColor(_qmlNetPaintedItemRef, x, y, width, height, colorId);
        }

        public void FillRect(int x, int y, int width, int height)
        {
            Interop.QmlNetPaintedItem.FillRect(_qmlNetPaintedItemRef, x, y, width, height);
        }

        public int CreateColor(string colorString)
        {
            return Interop.QmlNetPaintedItem.CreateColor(_qmlNetPaintedItemRef, colorString);
        }

        public void FreeColor(int colorId)
        {
            Interop.QmlNetPaintedItem.FreeColor(_qmlNetPaintedItemRef, colorId);
        }

        private int GetColorId(string colorString)
        {
            if (_registeredColors.ContainsKey(colorString))
            {
                return _registeredColors[colorString];
            }
            var colorId = CreateColor(colorString);
            _registeredColors.Add(colorString, colorId);
            return colorId;
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

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_setPen")]
        public SetPenDel SetPen { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetPenDel(IntPtr paintedItem, int colorId);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_resetPen")]
        public ResetPenDel ResetPen { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ResetPenDel(IntPtr paintedItem);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_setBrush")]
        public SetBrushDel SetBrush { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetBrushDel(IntPtr paintedItem, int colorId);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_resetBrush")]
        public ResetBrushDel ResetBrush { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ResetBrushDel(IntPtr paintedItem);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_setFont")]
        public SetFontDel SetFont { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontDel(IntPtr paintedItem, [MarshalAs(UnmanagedType.LPWStr)] string fontFamilyName, bool isBold, bool isItalic, bool isUnderline, int pxSize);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_setFontFamily")]
        public SetFontFamilyDel SetFontFamily { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontFamilyDel(IntPtr paintedItem, [MarshalAs(UnmanagedType.LPWStr)] string fontFamilyName);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_setFontBold")]
        public SetFontBoldDel SetFontBold { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontBoldDel(IntPtr paintedItem, bool isBold);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_setFontItalic")]
        public SetFontItalicDel SetFontItalic { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontItalicDel(IntPtr paintedItem, bool isItalic);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_setFontUnderline")]
        public SetFontUnderlineDel SetFontUnderline { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontUnderlineDel(IntPtr paintedItem, bool isUnderline);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_setFontSize")]
        public SetFontSizeDel SetFontSize { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontSizeDel(IntPtr paintedItem, int pxSize);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_drawText")]
        public DrawTextDel DrawText { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawTextDel(IntPtr paintedItem, int x, int y, [MarshalAs(UnmanagedType.LPWStr)] string text);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_drawRect")]
        public DrawRectDel DrawRect { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawRectDel(IntPtr paintedItem, int x, int y, int width, int height);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_fillRectColor")]
        public FillRectColorDel FillRectColor { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FillRectColorDel(IntPtr paintedItem, int x, int y, int width, int height, int colorId);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_fillRect")]
        public FillRectDel FillRect { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FillRectDel(IntPtr paintedItem, int x, int y, int width, int height);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_createColor")]
        public CreateColorDel CreateColor { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int CreateColorDel(IntPtr paintedItem, [MarshalAs(UnmanagedType.LPWStr)] string colorString);

        [NativeSymbol(Entrypoint = "qqmlnetpainteditem_freeColor")]
        public FreeColorDel FreeColor { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FreeColorDel(IntPtr paintedItem, int colorId);
    }
}
