using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using Qml.Net.Internal;

namespace Qml.Net
{
    public class INetQPainter
    {
        private IntPtr _qPainterRef;
        private Dictionary<string, int> _registeredColors = new Dictionary<string, int>();
        private Dictionary<string, int> _registeredFontFamilies = new Dictionary<string, int>();

        public INetQPainter(IntPtr qPainterRef)
        {
            _qPainterRef = qPainterRef;
        }
        
        public void SetPen(string colorString)
        {
            var colorId = GetColorId(colorString);
            Interop.INetQPainter.SetPen(_qPainterRef, colorId);
        }

        public void ResetPen()
        {
            Interop.INetQPainter.ResetPen(_qPainterRef);
        }

        public void SetBrush(string colorString)
        {
            var colorId = GetColorId(colorString);
            Interop.INetQPainter.SetBrush(_qPainterRef, colorId);
        }

        public void ResetBrush()
        {
            Interop.INetQPainter.ResetBrush(_qPainterRef);
        }

        public void SetFont(string fontFamilyName, bool isBold, bool isItalic, bool isUnderline, int pxSize)
        {
            int fontFamilyId = GetFontFamilyId(fontFamilyName);
            Interop.INetQPainter.SetFont(_qPainterRef, fontFamilyId, isBold, isItalic, isUnderline, pxSize);
        }

        public void SetFontFamily(string fontFamilyName)
        {
            int fontFamilyId = GetFontFamilyId(fontFamilyName);
            Interop.INetQPainter.SetFontFamily(_qPainterRef, fontFamilyId);
        }

        public void SetFontBold(bool isBold)
        {
            Interop.INetQPainter.SetFontBold(_qPainterRef, isBold);
        }

        public void SetFontItalic(bool isItalic)
        {
            Interop.INetQPainter.SetFontItalic(_qPainterRef, isItalic);
        }

        public void SetFontUnderline(bool isUnderline)
        {
            Interop.INetQPainter.SetFontUnderline(_qPainterRef, isUnderline);
        }

        public void SetFontSize(int pxSize)
        {
            Interop.INetQPainter.SetFontSize(_qPainterRef, pxSize);
        }

        public void DrawText(int x, int y, string text)
        {
            Interop.INetQPainter.DrawText(_qPainterRef, x, y, text);
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
            Interop.INetQPainter.DrawTextRect(_qPainterRef, x, y, width, height, (int)flags, text);
        }

        public void DrawRect(int x, int y, int width, int height)
        {
            Interop.INetQPainter.DrawRect(_qPainterRef, x, y, width, height);
        }

        public void FillRect(int x, int y, int width, int height, string color)
        {
            var colorId = GetColorId(color);
            Interop.INetQPainter.FillRectColor(_qPainterRef, x, y, width, height, colorId);
        }

        public void FillRect(int x, int y, int width, int height)
        {
            Interop.INetQPainter.FillRect(_qPainterRef, x, y, width, height);
        }

        private int RegisterColor(string colorString)
        {
            return Interop.INetQPainter.RegisterColor(_qPainterRef, colorString);
        }

        private void FreeColor(int colorId)
        {
            Interop.INetQPainter.FreeColor(_qPainterRef, colorId);
        }

        private int RegisterFontFamily(string fontFamily)
        {
            return Interop.INetQPainter.RegisterFontFamily(_qPainterRef, fontFamily);
        }

        private void FreeFontFamily(int fontFamilyId)
        {
            Interop.INetQPainter.FreeFontFamily(_qPainterRef, fontFamilyId);
        }

        public Size GetStringSize(string fontFamily, int fontSizePx, string text)
        {
            int fontFamilyId = GetFontFamilyId(fontFamily);
            var res = Interop.INetQPainter.GetStringSize(_qPainterRef, fontFamilyId, fontSizePx, text);
            return new Size(res.width, res.height);
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
    
    internal class INetQPainterInterop
    {
        [NativeSymbol(Entrypoint = "inetqpainter_setPen")]
        public SetPenDel SetPen { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetPenDel(IntPtr paintedItem, int colorId);

        [NativeSymbol(Entrypoint = "inetqpainter_resetPen")]
        public ResetPenDel ResetPen { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ResetPenDel(IntPtr paintedItem);

        [NativeSymbol(Entrypoint = "inetqpainter_setBrush")]
        public SetBrushDel SetBrush { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetBrushDel(IntPtr paintedItem, int colorId);

        [NativeSymbol(Entrypoint = "inetqpainter_resetBrush")]
        public ResetBrushDel ResetBrush { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ResetBrushDel(IntPtr paintedItem);

        [NativeSymbol(Entrypoint = "inetqpainter_setFont")]
        public SetFontDel SetFont { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontDel(IntPtr paintedItem, int fontFamilyId, bool isBold, bool isItalic, bool isUnderline, int pxSize);

        [NativeSymbol(Entrypoint = "inetqpainter_setFontFamily")]
        public SetFontFamilyDel SetFontFamily { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontFamilyDel(IntPtr paintedItem, int fontFamilyId);

        [NativeSymbol(Entrypoint = "inetqpainter_setFontBold")]
        public SetFontBoldDel SetFontBold { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontBoldDel(IntPtr paintedItem, bool isBold);

        [NativeSymbol(Entrypoint = "inetqpainter_setFontItalic")]
        public SetFontItalicDel SetFontItalic { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontItalicDel(IntPtr paintedItem, bool isItalic);

        [NativeSymbol(Entrypoint = "inetqpainter_setFontUnderline")]
        public SetFontUnderlineDel SetFontUnderline { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontUnderlineDel(IntPtr paintedItem, bool isUnderline);

        [NativeSymbol(Entrypoint = "inetqpainter_setFontSize")]
        public SetFontSizeDel SetFontSize { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFontSizeDel(IntPtr paintedItem, int pxSize);

        [NativeSymbol(Entrypoint = "inetqpainter_drawText")]
        public DrawTextDel DrawText { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawTextDel(IntPtr paintedItem, int x, int y, [MarshalAs(UnmanagedType.LPWStr)] string text);

        [NativeSymbol(Entrypoint = "inetqpainter_drawTextRect")]
        public DrawTextRectDel DrawTextRect { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawTextRectDel(IntPtr paintedItem, int x, int y, int width, int height, int flags, [MarshalAs(UnmanagedType.LPWStr)] string text);

        [NativeSymbol(Entrypoint = "inetqpainter_drawRect")]
        public DrawRectDel DrawRect { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DrawRectDel(IntPtr paintedItem, int x, int y, int width, int height);

        [NativeSymbol(Entrypoint = "inetqpainter_fillRectColor")]
        public FillRectColorDel FillRectColor { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FillRectColorDel(IntPtr paintedItem, int x, int y, int width, int height, int colorId);

        [NativeSymbol(Entrypoint = "inetqpainter_fillRect")]
        public FillRectDel FillRect { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FillRectDel(IntPtr paintedItem, int x, int y, int width, int height);

        [NativeSymbol(Entrypoint = "inetqpainter_registerColor")]
        public RegisterColorDel RegisterColor { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int RegisterColorDel(IntPtr paintedItem, [MarshalAs(UnmanagedType.LPWStr)] string colorString);

        [NativeSymbol(Entrypoint = "inetqpainter_freeColor")]
        public FreeColorDel FreeColor { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FreeColorDel(IntPtr paintedItem, int colorId);

        [NativeSymbol(Entrypoint = "inetqpainter_registerFontFamily")]
        public RegisterFontFamilyDel RegisterFontFamily { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int RegisterFontFamilyDel(IntPtr paintedItem, [MarshalAs(UnmanagedType.LPWStr)] string fontFamilyString);

        [NativeSymbol(Entrypoint = "inetqpainter_freeFontFamily")]
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

        [NativeSymbol(Entrypoint = "inetqpainter_getStringSize")]
        public GetStringSizeDel GetStringSize { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate StringSizeResult GetStringSizeDel(IntPtr paintedItem, int fontFamilyId, int sizePx, [MarshalAs(UnmanagedType.LPWStr)] string text);

    }
}