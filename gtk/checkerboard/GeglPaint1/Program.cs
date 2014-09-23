using System;
using Gtk;
using Gegl;

namespace GeglPaint1
{
	class MainClass
	{
		static MainWindow win;

		public static void Main (string[] args)
		{
			Application.Init ();
			Gegl.Global.Init ();

			win = new MainWindow ();
			win.Show ();
		
			Application.Run ();
			Gegl.Global.Exit ();
		}
	}
}
