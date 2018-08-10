﻿using System;
using System.Windows;
using System.Windows.Interop;

namespace QuickCopy
{
	public class ClipboardManager
	{
		public event EventHandler ClipboardChanged;

		public ClipboardManager(Window windowSource)
		{
			if (!(PresentationSource.FromVisual(windowSource) is HwndSource source))
			{
				throw new ArgumentException(
					"Window source MUST be initialized first, such as in the Window's OnSourceInitialized handler."
					, nameof(windowSource));
			}

			source.AddHook(WndProc);

			// get window handle for interop
			var windowHandle = new WindowInteropHelper(windowSource).Handle;

			// register for clipboard events
			NativeMethods.AddClipboardFormatListener(windowHandle);
		}

		private void OnClipboardChanged()
		{
			ClipboardChanged?.Invoke(this, EventArgs.Empty);
		}

		private static readonly IntPtr WndProcSuccess = IntPtr.Zero;

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == NativeMethods.WM_CLIPBOARDUPDATE)
			{
				OnClipboardChanged();
				handled = true;
			}

			return WndProcSuccess;
		}
	}
}
