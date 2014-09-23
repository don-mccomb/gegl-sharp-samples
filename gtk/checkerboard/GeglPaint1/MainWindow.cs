using System;
using Gtk;
using Gegl;
using Cairo;

public partial class MainWindow: Gtk.Window
{	
	Node graphNode;
	Node checkerBoardNode;
	DrawingArea da;

	ImageSurface s = null;

	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();

		// Create a drawing area and add it to the window
		da = new DrawingArea ();
		Box box = new HBox (true, 0);
		box.Add (da);
		Add (box);

		// Create our GEGL graph node
		graphNode = new Node ();

		// Create a checkboard node and set the 2 colors of the checkerboard
		checkerBoardNode = graphNode.CreateChild ("gegl:checkerboard");
		checkerBoardNode.SetProperty("color1", new Gegl.Color("rgb(0.4, 0.4, 0.4)"));
		checkerBoardNode.SetProperty("color2", new Gegl.Color("rgb(0.6, 0.6, 0.6)"));

		// Add event handlers for handling resize and redraw events on the DrawingArea widget
		da.SizeAllocated += DrawingAreaHandleSizeAllocated;
		da.ExposeEvent += DrawingAreaHandleExposeEvent;

		// Show all controls and request a redraw (aka Invalidate)
		ShowAll ();
		QueueDraw ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	void DrawingAreaHandleSizeAllocated (object o, SizeAllocatedArgs args)
	{
		// Create a pixel format
		Babl.Format b = Babl.Format.FromString ("cairo-ARGB32");

		// Create an ImageSurface onto which we will paint the checkerboard pattern
		s = new Cairo.ImageSurface (Cairo.Format.ARGB32, da.Allocation.Width, da.Allocation.Height);

		if (s != null) {
			if (checkerBoardNode != null) {
				// Paint the checkerboard pattern
				checkerBoardNode.Blit (1, Gegl.Rectangle.New(0,0,Convert.ToUInt32(da.Allocation.Width),
				                                             Convert.ToUInt32(da.Allocation.Height)), b, s.DataPtr, 0, Gegl.BlitFlags.Default);
				// Mark the ImageSurface as being dirty.
				s.MarkDirty ();
			}
		}

		// Queue a redraw (aka Invalidate)
		QueueDraw ();
	}

	void DrawingAreaHandleExposeEvent (object o, ExposeEventArgs args)
	{
		// Paint the ImageSurface onto the DrawingArea
		DrawingArea area = (DrawingArea) o;
		using (Cairo.Context context = Gdk.CairoHelper.Create (area.GdkWindow)) {
			if (s != null) {
				context.SetSource(s);
				context.Paint ();
			}
		}
	}
}