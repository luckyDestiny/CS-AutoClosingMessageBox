using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

public class AutoClosingMessageBox
{
	private System.Threading.Timer _timeoutTimer;

	private string _caption;

	private const int WM_CLOSE = 16;

	private AutoClosingMessageBox(string text, string caption, int timeout)
	{
		_caption = caption;
		_timeoutTimer = new System.Threading.Timer(OnTimerElapsed, null, timeout, -1);
		using (_timeoutTimer)
		{
			MessageBox.Show(text, caption);
		}
	}

	public static void Show(string text, string caption, int timeout)
	{
		new AutoClosingMessageBox(text, caption, timeout);
	}

	public static void Show(string text, int timeout)
	{
		new AutoClosingMessageBox(text, timeout / 1000 + "will be Closed", timeout);
	}

	public static void Show(string text)
	{
		new AutoClosingMessageBox(text, "5 secs ", 5000);
	}

	private void OnTimerElapsed(object state)
	{
		IntPtr intPtr = FindWindow("#32770", _caption);
		if (intPtr != IntPtr.Zero)
		{
			SendMessage(intPtr, 16u, IntPtr.Zero, IntPtr.Zero);
		}
		_timeoutTimer.Dispose();
	}

	[DllImport("user32.dll", SetLastError = true)]
	private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
}