using System.Drawing;
using System.Windows.Forms;

public class CustomSplitContainer : SplitContainer
{
    public Color SplitterColor { get; set; } = Color.Gray;

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Rectangle splitterRect;

        if (Orientation == Orientation.Vertical)
        {
            splitterRect = new Rectangle(
                SplitterDistance,
                0,
                SplitterWidth,
                Height);
        }
        else
        {
            splitterRect = new Rectangle(
                0,
                SplitterDistance,
                Width,
                SplitterWidth);
        }

        using (var brush = new SolidBrush(SplitterColor))
        {
            e.Graphics.FillRectangle(brush, splitterRect);
        }
    }
}
