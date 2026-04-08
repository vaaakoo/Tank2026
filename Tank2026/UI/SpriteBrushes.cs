using System.Windows;
using System.Windows.Media;

namespace Tank2026.UI;

public static class SpriteBrushes
{
    public static DrawingBrush BrickBrush { get; }
    public static DrawingBrush SteelBrush { get; }
    public static DrawingBrush WaterBrush { get; }
    public static DrawingBrush GrassBrush { get; }
    public static DrawingBrush BaseBrush { get; }

    static SpriteBrushes()
    {
        BrickBrush = CreateBrickBrush();
        SteelBrush = CreateSteelBrush();
        WaterBrush = CreateWaterBrush();
        GrassBrush = CreateGrassBrush();
        BaseBrush = CreateBaseBrush();
    }

    private static DrawingBrush CreateBrickBrush()
    {
        var group = new DrawingGroup();
        group.Children.Add(new GeometryDrawing(new SolidColorBrush(Color.FromRgb(156, 74, 36)), null, Geometry.Parse("M0,0 h32 v32 h-32 Z")));
        group.Children.Add(new GeometryDrawing(new SolidColorBrush(Color.FromRgb(200, 200, 200)) { Opacity = 0.5 }, null, Geometry.Parse("M0,15 h32 v2 h-32 Z M15,0 h2 v15 h-2 Z M31,0 h2 v15 h-2 Z M7,17 h2 v15 h-2 Z M23,17 h2 v15 h-2 Z")));
        return new DrawingBrush(group) { Stretch = Stretch.None };
    }

    private static DrawingBrush CreateSteelBrush()
    {
        var group = new DrawingGroup();
        group.Children.Add(new GeometryDrawing(new SolidColorBrush(Color.FromRgb(150, 150, 150)), null, Geometry.Parse("M0,0 h32 v32 h-32 Z")));
        group.Children.Add(new GeometryDrawing(Brushes.White, null, Geometry.Parse("M2,2 h28 v2 h-28 Z M2,2 h2 v28 h-2 Z")));
        group.Children.Add(new GeometryDrawing(Brushes.Black, null, Geometry.Parse("M2,30 h30 v2 h-30 Z M30,2 h2 v30 h-2 Z")));
        group.Children.Add(new GeometryDrawing(Brushes.LightGray, null, Geometry.Parse("M8,8 h16 v16 h-16 Z")));
        return new DrawingBrush(group) { Stretch = Stretch.None };
    }

    private static DrawingBrush CreateWaterBrush()
    {
        var group = new DrawingGroup();
        group.Children.Add(new GeometryDrawing(new SolidColorBrush(Color.FromRgb(30, 60, 200)), null, Geometry.Parse("M0,0 h32 v32 h-32 Z")));
        group.Children.Add(new GeometryDrawing(null, new Pen(Brushes.LightBlue, 2), Geometry.Parse("M0,8 Q 8,0 16,8 T 32,8 M0,24 Q 8,16 16,24 T 32,24")));
        return new DrawingBrush(group) { Stretch = Stretch.None };
    }

    private static DrawingBrush CreateGrassBrush()
    {
        var group = new DrawingGroup();
        group.Children.Add(new GeometryDrawing(new SolidColorBrush(Color.FromRgb(40, 120, 40)), null, Geometry.Parse("M0,0 h32 v32 h-32 Z")));
        group.Children.Add(new GeometryDrawing(null, new Pen(Brushes.LightGreen, 1), Geometry.Parse("M4,16 l 4,-8 l 4,8 M20,28 l 4,-8 l 4,8 M8,32 l 4,-8 l 4,8")));
        return new DrawingBrush(group) { Stretch = Stretch.None };
    }

    private static DrawingBrush CreateBaseBrush()
    {
        var group = new DrawingGroup();
        group.Children.Add(new GeometryDrawing(Brushes.Black, null, Geometry.Parse("M0,0 h32 v32 h-32 Z")));
        group.Children.Add(new GeometryDrawing(Brushes.Yellow, null, Geometry.Parse("M16,4 L20,12 L28,12 L22,18 L24,28 L16,22 L8,28 L10,18 L4,12 L12,12 Z")));
        return new DrawingBrush(group) { Stretch = Stretch.None };
    }

    public static DrawingBrush GetTankBrush(Brush bodyColor)
    {
        var group = new DrawingGroup();
        
        var gray = Brushes.Gray;
        var blackPen = new Pen(Brushes.Black, 1);
        
        // Tracks
        group.Children.Add(new GeometryDrawing(gray, null, Geometry.Parse("M2,0 h6 v32 h-6 Z M0,4 h10 v2 h-10 Z M0,10 h10 v2 h-10 Z M0,16 h10 v2 h-10 Z M0,22 h10 v2 h-10 Z M0,28 h10 v2 h-10 Z")));
        group.Children.Add(new GeometryDrawing(gray, null, Geometry.Parse("M24,0 h6 v32 h-6 Z M22,4 h10 v2 h-10 Z M22,10 h10 v2 h-10 Z M22,16 h10 v2 h-10 Z M22,22 h10 v2 h-10 Z M22,28 h10 v2 h-10 Z")));
        
        // Body
        group.Children.Add(new GeometryDrawing(bodyColor, null, Geometry.Parse("M8,4 h16 v24 h-16 Z")));
        group.Children.Add(new GeometryDrawing(null, blackPen, Geometry.Parse("M10,8 h12 v16 h-12 Z")));
        
        // Gun Base (Ellipse is complicated in path data, using a path curve or just a square base)
        group.Children.Add(new GeometryDrawing(bodyColor, blackPen, Geometry.Parse("M12,12 h8 v8 h-8 Z")));
        
        // Barrel
        group.Children.Add(new GeometryDrawing(gray, blackPen, Geometry.Parse("M14,12 h4 v-12 h-4 Z")));
        
        return new DrawingBrush(group) { Stretch = Stretch.None };
    }
}
