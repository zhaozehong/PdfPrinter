using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Zehong.CSharp.Solution.HelperLib;

namespace Zehong.CSharp.Solution.PdfPrinter
{
  public static class PdfPrinter
  {
    public static Boolean Print(List<Canvas> pages, String fileName, String passward = null)
    {
      if (pages == null || String.IsNullOrWhiteSpace(fileName))
        return false;

      pages = pages.Where(p => p != null).ToList();
      if (!pages.Any())
        return false;

      fileName = System.IO.Path.ChangeExtension(fileName, ".pdf");
      if (String.IsNullOrWhiteSpace(Helper.ForceCreateDirectoryEx(fileName)))
        return false;

      try
      {
        ExceptionHandler.ClearAllExceptions();
        using (var document = new Document())
        {
          var writer = PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.Create));
          if (passward != null)
            writer.SetEncryption(PdfWriter.STRENGTH40BITS, passward, DateTime.Now.ToString("yyyyMMdd") + (DateTime.Now.Day % 2 == 0 ? "0" : "1"), PdfWriter.ALLOW_COPY);

          document.Open();
          foreach (var page in pages)
          {
            document.SetPageSize(new iTextSharp.text.Rectangle(0, 0, (float)page.Width, (float)page.Height)); // Zehong: Don't use page.ActualWidth & page.ActualHeight, since it always returns 0.0 here.
            document.NewPage();

            PrintPage(page, writer.DirectContent);
          }
        }

        // Prevent memory leak
        FontFactory.FontImp = new FontFactoryImp();
        GC.Collect(GC.MaxGeneration);
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
      if (ExceptionHandler.HasError)
      {
        File.Delete(fileName);
        return false;
      }
      return true;
    }

    private static void PrintPage(Canvas page, PdfContentByte dc)
    {
      if (page == null || page.Visibility != Visibility.Visible)
        return;

      page.SafeForceUpdateLayout();
      var pageElements = page.Children.OfType<FrameworkElement>().Where(p => p.Visibility == Visibility.Visible).OrderBy(p => p.SafeGetZIndex()).ToList();
      foreach (var pageElement in pageElements)
      {
        PrintElement(pageElement, page, dc);
      }
    }
    private static void PrintElement(FrameworkElement element, FrameworkElement relativeTo, PdfContentByte dc)
    {
      if (element == null || element.Visibility != Visibility.Visible || element.RenderSize.IsEmpty || element.RenderSize.IsZero() || element.RenderSize.IsNaN())
        return;

      try
      {
        if (element is ContentPresenter)
          PrintContentPresenter((ContentPresenter)element, relativeTo, dc);
        else if (element is Control)
          PrintControl((Control)element, relativeTo, dc);
        else if (element is Decorator)
          PrintDecorator((Decorator)element, relativeTo, dc);
        else if (element is System.Windows.Controls.Image)
          PrintImage((System.Windows.Controls.Image)element, null, relativeTo, dc);
        else if (element is Panel)
          PrintPanel((Panel)element, relativeTo, dc);
        else if (element is TextBlock)
          PrintText((TextBlock)element, relativeTo, dc);
        else if (element is Viewport3D)
          PrintImage(GetFrameworkElementImage(element), element, relativeTo, dc);
        else if (element is Shape)
          PrintShape((Shape)element, relativeTo, dc);
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
    }

    #region ContentPresenter
    private static void PrintContentPresenter(ContentPresenter element, FrameworkElement relativeTo, PdfContentByte dc)
    {
      try
      {
        var count = VisualTreeHelper.GetChildrenCount(element);
        if (count != 1)
          return;

        var content = VisualTreeHelper.GetChild(element, 0) as FrameworkElement;
        if (content == null)
          return;

        PrintElement(content, relativeTo, dc);
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
    }
    #endregion

    #region Control
    private static void PrintControl(Control control, FrameworkElement relativeTo, PdfContentByte dc)
    {
      try
      {
        if (control is ContentControl)
          PrintContentControl((ContentControl)control, relativeTo, dc);
        else if (control is ItemsControl)
          PrintItemsControl((ItemsControl)control, relativeTo, dc);
        else if (control is TextBox)
          PrintText((TextBox)control, relativeTo, dc);
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
    }
    private static void PrintContentControl(ContentControl contentControl, FrameworkElement relativeTo, PdfContentByte dc)
    {
      try
      {
        PrintBackground(contentControl, relativeTo, dc);
        var uiElement = contentControl.Content as FrameworkElement;
        if (uiElement == null)
        {
          uiElement = new TextBlock()
          {
            Text = contentControl.Content != null ? contentControl.Content.ToString() : String.Empty,
            Padding = new Thickness(0)
          };
        }
        PrintElement(uiElement, relativeTo, dc);
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
    }
    private static void PrintItemsControl(ItemsControl element, FrameworkElement relativeTo, PdfContentByte dc)
    {
      if (element.ItemContainerGenerator == null)
        return;

      try
      {
        PrintBackground(element, relativeTo, dc);
        for (int i = 0; i < element.Items.Count; i++)
        {
          var itemUI = element.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
          if (itemUI == null)
            continue;

          var count = VisualTreeHelper.GetChildrenCount(itemUI);
          if (count != 1)
            continue;

          var content = VisualTreeHelper.GetChild(itemUI, 0) as FrameworkElement;
          if (content == null)
            continue;

          PrintElement(content, relativeTo, dc);
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
    }
    #endregion

    #region Decorator
    private static void PrintDecorator(Decorator decorator, FrameworkElement relativeTo, PdfContentByte dc)
    {
      try
      {
        if (decorator is Border)
          PrintBorder((Border)decorator, relativeTo, dc);
        else if (decorator is Viewbox)
          PrintViewbox((Viewbox)decorator, relativeTo, dc);
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
    }
    private static void PrintBorder(Border border, FrameworkElement relativeTo, PdfContentByte dc)
    {
      try
      {
        PrintBackground(border, relativeTo, dc);
        PrintElement(border.Child as FrameworkElement, relativeTo, dc);
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
    }
    private static void PrintViewbox(Viewbox viewbox, FrameworkElement relativeTo, PdfContentByte dc)
    {
      try
      {
        PrintImage(GetFrameworkElementImage(viewbox), viewbox, relativeTo, dc);

        //float left, top;
        //if (!GetElementLocation(viewbox, relativeTo, out left, out top))
        //  return;

        //var template = dc.CreateTemplate((float)viewbox.ActualWidth, (float)viewbox.ActualHeight);

        //PrintElement(viewbox.Child as FrameworkElement, viewbox, dc);

        //dc.AddTemplate(template,
        //  (float)viewbox.RenderTransform.Value.M11,
        //  (float)viewbox.RenderTransform.Value.M12,
        //  (float)viewbox.RenderTransform.Value.M21,
        //  (float)viewbox.RenderTransform.Value.M22,
        //  (float)viewbox.RenderTransform.Value.OffsetX + left,
        //  (float)viewbox.RenderTransform.Value.OffsetY + top);
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
    }

    #endregion

    #region Image
    private static void PrintImage(System.Windows.Controls.Image image, FrameworkElement imageSource, FrameworkElement relativeTo, PdfContentByte dc)
    {
      try
      {
        var control = imageSource ?? (FrameworkElement)image;

        float left, top;
        if (!GetElementLocation(control, relativeTo, out left, out top))
          return;

        IElement pdfImage = iTextSharp.text.Image.GetInstance(ImageToDrawingImage(image), System.Drawing.Imaging.ImageFormat.Png);
        //IElement pdfImage = iTextSharp.text.Image.GetInstance(@"C:\Users\zehong.zhao\Pictures\IMG_0091 (002).jpg");

        WritePdfElement(dc, ref pdfImage, control, left, top);
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
    }
    #endregion

    #region Panel
    private static void PrintPanel(Panel panel, FrameworkElement relativeTo, PdfContentByte dc)
    {
      try
      {
        PrintBackground(panel, relativeTo, dc);
        var children = panel.Children.OfType<FrameworkElement>().Where(p => p.Visibility == Visibility.Visible).OrderBy(p => p.SafeGetZIndex()).ToList();
        foreach (var child in children)
        {
          PrintElement(child, relativeTo, dc);
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
    }
    #endregion

    #region Shape
    private static Rect? GetClipRectange(FrameworkElement element, FrameworkElement relativeTo)
    {
      var parent = element;
      while (parent != null && !parent.ClipToBounds)
      {
        parent = parent.Parent as FrameworkElement;
      }
      if (parent != null && !object.ReferenceEquals(parent, relativeTo) && parent.ClipToBounds)
      {
        float left, top;
        if (GetElementLocation(parent, relativeTo, out left, out top))
          return new Rect(left, top, parent.ActualWidth, parent.ActualHeight);
      }
      return null;
    }
    private static Boolean PrintShape(Shape shape, FrameworkElement relativeTo, PdfContentByte dc)
    {
      var noBorder = Helper.IsNullOrZero(shape.StrokeThickness) || Helper.IsTransparent(shape.Stroke);
      var noBackground = Helper.IsTransparent(shape.Fill);
      if (noBorder && noBackground)
        return true;

      dc.SaveState();

      try
      {
        var clipRect = GetClipRectange(shape, relativeTo);
        if (clipRect != null)
        {
          dc.Rectangle(clipRect.Value.Left, clipRect.Value.Top - clipRect.Value.Height, clipRect.Value.Width, clipRect.Value.Height);
          dc.Clip();
          dc.NewPath();
        }

        dc.SetColorFill(GetBaseColor(shape.Fill));
        dc.SetColorStroke(GetBaseColor(shape.Stroke));
        dc.SetLineWidth(shape.StrokeThickness);
        if (shape.StrokeDashArray != null && shape.StrokeDashArray.Any())
          dc.SetLineDash(shape.StrokeDashArray.Select(p => p * shape.StrokeThickness).ToArray(), 0);

        float left, top;
        if (!GetElementLocation(shape, relativeTo, out left, out top))
          return false;

        var lineGeometry = shape.RenderedGeometry as LineGeometry;
        if (lineGeometry != null)
        {
          var startPoint = shape.RenderedGeometry.Transform.Transform(lineGeometry.StartPoint);
          var endPoint = shape.RenderedGeometry.Transform.Transform(lineGeometry.EndPoint);
          dc.MoveTo(left + startPoint.X, top - startPoint.Y);
          dc.LineTo(left + endPoint.X, top - endPoint.Y);
        }
        var rectangleGeometry = shape.RenderedGeometry as RectangleGeometry;
        if (rectangleGeometry != null)
        {
          if (!rectangleGeometry.Rect.IsNaNOrZeroOrEmpty())
          {
            var bottomLeft = shape.RenderedGeometry.Transform.Transform(rectangleGeometry.Rect.BottomLeft);
            var widthHeight = shape.RenderedGeometry.Transform.Transform(new Point(rectangleGeometry.Rect.Width, rectangleGeometry.Rect.Height));
            dc.Rectangle(left + bottomLeft.X, top - bottomLeft.Y, widthHeight.X, widthHeight.Y);
          }
        }
        var ellipseGeometry = shape.RenderedGeometry as EllipseGeometry;
        if (ellipseGeometry != null)
        {
          var center = shape.RenderedGeometry.Transform.Transform(ellipseGeometry.Center);
          center = new Point(left + center.X, top - center.Y);
          if (Helper.IsNullOrZero(ellipseGeometry.RadiusX) && Helper.IsNullOrZero(ellipseGeometry.RadiusY))
            dc.Circle(center.X, center.Y, 0.01);
          else
          {
            var radius = shape.RenderedGeometry.Transform.Transform(new Point(ellipseGeometry.RadiusX, ellipseGeometry.RadiusY));
            var x1 = (Int32)Math.Max(1, Math.Round(center.X - radius.X));
            var y1 = (Int32)Math.Max(1, Math.Round(center.Y - radius.Y));
            var x2 = (Int32)Math.Max(1, Math.Round(center.X + radius.X));
            var y2 = (Int32)Math.Max(1, Math.Round(center.Y + radius.Y));
            dc.Ellipse(x1, y1, x2, y2);
          }
        }
        var streamGeometry = shape.RenderedGeometry as StreamGeometry;
        if (streamGeometry != null)
        {
          var figures = GeometryDataParser.GetDataFigures(streamGeometry.ToString());
          foreach (var figure in figures)
          {
            if (!figure.Draw(left, top, dc, shape.LayoutTransform))
              break;
          }
        }
        var pathGrometry = shape.RenderedGeometry as PathGeometry;
        if (pathGrometry != null)
        {
          var pathFigure = pathGrometry.Figures.First(p => p != null);
          if (pathFigure != null)
          {
            var points = new List<Point>();
            points.Add(shape.RenderedGeometry.Transform.Transform(pathFigure.StartPoint));

            var lineSegments = pathFigure.Segments.OfType<LineSegment>();
            if (lineSegments != null)
              points.AddRange(lineSegments.Select(p => shape.RenderedGeometry.Transform.Transform(p.Point)));

            var polyLineSegment = pathFigure.Segments.OfType<PolyLineSegment>().FirstOrDefault();
            if (polyLineSegment != null)
              points.AddRange(polyLineSegment.Points.Select(p => shape.RenderedGeometry.Transform.Transform(p)));

            points = points.Select(p => new Point(left + p.X, top - p.Y)).ToList();
            foreach (var point in points)
            {
              if (points.IndexOf(point) == 0)
                dc.MoveTo(point.X, point.Y);
              else
                dc.LineTo(point.X, point.Y);
            }
          }
        }

        if (!noBorder && !noBackground)
          dc.FillStroke();
        else
        {
          if (!noBorder)
            dc.Stroke();
          if (!noBackground)
            dc.Fill();
        }

        return true;
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        return false;
      }
      finally
      {
        dc.RestoreState();
      }
    }
    #endregion

    #region Helper
    private static void WritePdfElement(PdfContentByte dc, ref IElement pdfElement, FrameworkElement relatedElement, float left, float top)
    {
      if (dc == null || pdfElement == null || relatedElement == null)
        return;

      var pdfTable = pdfElement as PdfPTable;
      try
      {
        if (pdfTable == null)
        {
          var pdfCell = Pack(relatedElement, ref pdfElement);
          var isVertical = pdfCell.Rotation == 90 || pdfCell.Rotation == 270;
          var width = isVertical ? (float)relatedElement.ActualHeight : (float)relatedElement.ActualWidth;
          if (Helper.IsNullOrZero(width))
            return;

          if (isVertical)
            top += (float)relatedElement.ActualWidth;

          pdfTable = CreatePdfTable(new float[] { width }, relatedElement.HorizontalAlignment);
          pdfTable.AddCell(pdfCell);
        }
        pdfTable.WriteSelectedRows(0, -1, left, top, dc);
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
      finally
      {
        ReleasePdfPTable(pdfTable);
        pdfTable = null;
      }
    }

    private static PdfPCell Pack(Double fixedHeight, ref IElement pdfElement)
    {
      var pdfCell = pdfElement as PdfPCell;
      if (pdfCell == null)
      {
        pdfCell = CreatePdfPCell(fixedHeight);
        AddElementToCell(pdfCell, ref pdfElement);
      }
      return pdfCell;
    }
    private static PdfPCell Pack(FrameworkElement uiContainer, ref IElement pdfElement)
    {
      var pdfCell = CreatePdfPCell(uiContainer);
      AddElementToCell(pdfCell, ref pdfElement);
      return pdfCell;
    }

    private static PdfPCell CreatePdfPCell(Double fixedHeight = 0)
    {
      return new PdfPCell() { BorderWidth = 0, BorderColor = null, BackgroundColor = null, Padding = 0, FixedHeight = (float)fixedHeight };
    }
    private static PdfPCell CreatePdfPCell(FrameworkElement uiContainer)
    {
      if (uiContainer == null)
        return CreatePdfPCell();

      try
      {
        var padding = new Thickness(0);
        var background = (Brush)Brushes.Transparent;
        var borderBrush = (Brush)Brushes.Transparent;
        var borderThickness = new Thickness(0);

        var rotateTransform = uiContainer.RenderTransform as RotateTransform;
        if (uiContainer.LayoutTransform is RotateTransform)
          rotateTransform = uiContainer.LayoutTransform as RotateTransform;
        var rotationAngle = NormalizeRotationAngle(rotateTransform);

        var actualHeight = uiContainer.ActualHeight;
        if (rotationAngle == 90 || rotationAngle == 180)
          actualHeight = uiContainer.ActualWidth;
        var horizontalAlignment = uiContainer.HorizontalAlignment;
        var verticalAlignment = uiContainer.VerticalAlignment;

        var control = uiContainer as Control;
        if (control != null)
        {
          padding = control.Padding;
          background = control.Background;
          borderBrush = control.BorderBrush;
          borderThickness = control.BorderThickness;
          if (horizontalAlignment == HorizontalAlignment.Stretch)
            horizontalAlignment = control.HorizontalContentAlignment;
          if (verticalAlignment == VerticalAlignment.Stretch)
            verticalAlignment = control.VerticalContentAlignment;

          var textBox = control as TextBox;
          if (textBox != null)
          {
            // Zehong: TextBox in WPF has a inside margin (2,0,2,0)
            padding.Left += 2;
            padding.Right += 2;

            var textBoxTextAlignmentHasSet = true;
            if (textBoxTextAlignmentHasSet)
            {
              // Zehong: TextAlignment property has the highest priority to set alignment for TextBox
              var hAlign = HorizontalAlignment.Left;
              var strTextAlignment = textBox.TextAlignment.ToString();
              if (Enum.IsDefined(typeof(HorizontalAlignment), strTextAlignment))
                hAlign = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), strTextAlignment);
              if (hAlign != HorizontalAlignment.Left)
                horizontalAlignment = hAlign;
            }
          }
        }
        else
        {
          var textBlock = uiContainer as TextBlock;
          if (textBlock != null)
          {
            padding = textBlock.Padding;
            background = textBlock.Background;
            var strTextAlignment = textBlock.TextAlignment.ToString();
            if (horizontalAlignment == HorizontalAlignment.Stretch && Enum.IsDefined(typeof(HorizontalAlignment), strTextAlignment))
              horizontalAlignment = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), strTextAlignment);
          }
          var border = uiContainer as Border;
          if (border != null)
          {
            padding = border.Padding;
            background = border.Background;
            borderBrush = border.BorderBrush;
            borderThickness = border.BorderThickness;
          }
          var panel = uiContainer as Panel;
          if (panel != null)
          {
            background = panel.Background;
          }
          var image = uiContainer as System.Windows.Controls.Image;
          if (image != null)
          {
            horizontalAlignment = StretchToHorizontalAlignment(image.Stretch);
          }
        }

        var pdfCell = CreatePdfPCell(actualHeight);
        pdfCell.PaddingLeft = (float)padding.Left;
        pdfCell.PaddingRight = (float)padding.Right;
        pdfCell.PaddingTop = (float)padding.Top;
        pdfCell.PaddingBottom = (float)padding.Bottom;
        pdfCell.BackgroundColor = GetBaseColor(Helper.BrushToColor(background));
        pdfCell.BorderColor = GetBaseColor(Helper.BrushToColor(borderBrush));
        pdfCell.HorizontalAlignment = GetPdfElementAlignment(horizontalAlignment);
        pdfCell.VerticalAlignment = GetPdfElementAlignment(verticalAlignment);
        pdfCell.Rotation = rotationAngle;
        if (pdfCell.BorderColor != null && pdfCell.BorderColor != TransparentBaseColor)
        {
          pdfCell.BorderWidthTop = (float)borderThickness.Top;
          pdfCell.BorderWidthBottom = (float)borderThickness.Bottom;
          pdfCell.BorderWidthLeft = (float)borderThickness.Left;
          pdfCell.BorderWidthRight = (float)borderThickness.Right;
        }
        return pdfCell;
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        return null;
      }
    }

    private static PdfPTable CreatePdfTable(float[] columnWidths, HorizontalAlignment horizontalAlignment)
    {
      var pdfTable = new PdfPTable(columnWidths.Length);
      pdfTable.SetTotalWidth(columnWidths);
      pdfTable.LockedWidth = true;
      pdfTable.HorizontalAlignment = GetPdfElementAlignment(horizontalAlignment);
      return pdfTable;
    }
    private static Phrase GetPdfPhrase(String text, Double fontSize, FontFamily fontFamily, FontWeight fontWeight, Brush foreground, Double maxTextWidth, Double renderHeight, Double lineHeight = Double.NaN)
    {
      try
      {
        var font = GetPdfFont(text, fontSize, fontFamily, fontWeight, foreground);
        var chunk = new Chunk(text, font);
        var lines = Helper.GetSplitStrings(text, false, '\n');
        if (!Double.IsNaN(lineHeight) || lines.Count > 1)
          chunk.setLineHeight((float)((renderHeight - 1) / lines.Count));
        return new Paragraph(chunk);
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        return null;
      }
    }

    private static void PrintText(TextBlock textBlock, FrameworkElement relativeTo, PdfContentByte dc)
    {
      try
      {
        float left, top;
        if (!GetElementLocation(textBlock, relativeTo, out left, out top))
          return;

        IElement pdfPhrase = GetPdfPhrase(
          textBlock.Text,
          textBlock.FontSize,
          textBlock.FontFamily,
          textBlock.FontWeight,
          textBlock.Foreground,
          textBlock.ActualWidth - textBlock.Padding.Left - textBlock.Padding.Right,
          textBlock.ActualHeight,
          textBlock.LineHeight);
        if (pdfPhrase != null)
          WritePdfElement(dc, ref pdfPhrase, textBlock, left, top);
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
    }
    private static void PrintText(TextBox textBox, FrameworkElement relativeTo, PdfContentByte dc)
    {
      try
      {
        float left, top;
        if (!GetElementLocation(textBox, relativeTo, out left, out top))
          return;

        IElement pdfPhrase = GetPdfPhrase(
          textBox.Text,
          textBox.FontSize,
          textBox.FontFamily,
          textBox.FontWeight,
          textBox.Foreground,
          textBox.ActualWidth - textBox.Padding.Left - textBox.Padding.Right - 4,
          textBox.ActualHeight);
        if (pdfPhrase != null)
          WritePdfElement(dc, ref pdfPhrase, textBox, left, top);
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
    }

    private static System.Windows.Controls.Image GetFrameworkElementImage(FrameworkElement frameworkElement)
    {
      if (!frameworkElement.CheckAccess())
      {
        System.Windows.Controls.Image ret = null;
        frameworkElement.Dispatcher.Invoke(new Action(delegate { ret = GetFrameworkElementImage(frameworkElement); }));
        return ret;
      }

      RenderTargetBitmap targetBitmap = null;
      try
      {
        if (frameworkElement != null)
        {
          targetBitmap = new RenderTargetBitmap((int)frameworkElement.ActualWidth, (int)frameworkElement.ActualHeight, 96d, 96d, PixelFormats.Default);
          targetBitmap.Render(frameworkElement);
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
      return new System.Windows.Controls.Image() { Source = targetBitmap };

    }
    private static void AddElementToCell(PdfPCell pdfCell, ref IElement pdfElement)
    {
      if (pdfCell == null || pdfElement == null)
        return;

      try
      {
        var tempCell = pdfElement as PdfPCell;
        if (tempCell != null)
        {
          pdfCell.PaddingLeft += tempCell.PaddingLeft;
          pdfCell.PaddingTop += tempCell.PaddingTop;
          pdfCell.PaddingRight += tempCell.PaddingRight;
          pdfCell.PaddingBottom += tempCell.PaddingBottom;

          pdfCell.BorderWidthLeft += tempCell.BorderWidthLeft;
          pdfCell.BorderWidthRight += tempCell.BorderWidthRight;
          pdfCell.BorderWidthTop += tempCell.BorderWidthTop;
          pdfCell.BorderWidthBottom += tempCell.BorderWidthBottom;

          if (tempCell.CellEvent != null)
            pdfCell.CellEvent = tempCell.CellEvent;
          if (tempCell.BorderColor != null && tempCell.BorderColor.A != 0)
            pdfCell.BorderColor = tempCell.BorderColor;
          if (tempCell.BackgroundColor != null && tempCell.BackgroundColor.A != 0)
            pdfCell.BackgroundColor = tempCell.BackgroundColor;
          pdfCell.HorizontalAlignment = tempCell.HorizontalAlignment;
          pdfCell.VerticalAlignment = tempCell.VerticalAlignment;
          pdfCell.Rotation = tempCell.Rotation;
          if (tempCell.FixedHeight > pdfCell.FixedHeight)
            pdfCell.FixedHeight = tempCell.FixedHeight;

          if (tempCell.Phrase != null)
            pdfCell.Phrase = tempCell.Phrase;
          if (tempCell.Image != null)
            pdfCell.Image = tempCell.Image;
          if (tempCell.Table != null)
            pdfCell.Table = tempCell.Table;
          return;
        }

        var pdfImage = pdfElement as iTextSharp.text.Image;
        if (pdfImage != null)
        {
          pdfCell.Image = pdfImage;
          return;
        }
        var pdfPhrase = pdfElement as Phrase;
        if (pdfPhrase != null)
        {
          pdfCell.Phrase = pdfPhrase;
          return;
        }
        var pdfTable = pdfElement as PdfPTable;
        if (pdfTable != null)
        {
          pdfCell.Table = pdfTable;
          return;
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
      finally
      {
        pdfElement = null;
      }
    }
    public static iTextSharp.text.Color GetBaseColor(System.Windows.Media.Color color)
    {
      if (Helper.IsTransparent(color))
        return TransparentBaseColor;
      return new iTextSharp.text.Color(color.R, color.G, color.B, color.A);
    }
    public static iTextSharp.text.Color GetBaseColor(Brush brush)
    {
      if (Helper.IsTransparent(brush))
        return TransparentBaseColor;

      var linearGradientBrush = brush as LinearGradientBrush;
      if (linearGradientBrush != null)
        return GetBaseColor(Helper.BrushToColor(linearGradientBrush));

      var radialGradientBrush = brush as RadialGradientBrush;
      if (radialGradientBrush != null)
        return GetBaseColor(Helper.BrushToColor(radialGradientBrush));

      return GetBaseColor(Helper.BrushToColor(brush));
    }

    private static int GetPdfElementAlignment(HorizontalAlignment alignment)
    {
      if (alignment == HorizontalAlignment.Left)
        return Element.ALIGN_LEFT;
      if (alignment == HorizontalAlignment.Center)
        return Element.ALIGN_CENTER;
      if (alignment == HorizontalAlignment.Right)
        return Element.ALIGN_RIGHT;
      return Element.ALIGN_JUSTIFIED;
    }
    private static int GetPdfElementAlignment(VerticalAlignment alignment)
    {
      if (alignment == VerticalAlignment.Top)
        return Element.ALIGN_TOP;
      if (alignment == VerticalAlignment.Center)
        return Element.ALIGN_MIDDLE;
      if (alignment == VerticalAlignment.Bottom)
        return Element.ALIGN_BOTTOM;
      return Element.ALIGN_JUSTIFIED;
    }
    private static HorizontalAlignment StretchToHorizontalAlignment(Stretch imageStretch)
    {
      switch (imageStretch)
      {
        case Stretch.Fill:
          return HorizontalAlignment.Center;
        default:
          return HorizontalAlignment.Center;
      }
    }
    private static System.Drawing.Bitmap ImageToDrawingImage(System.Windows.Controls.Image image)
    {
      if (image == null)
        return null;

      using (var ms = new MemoryStream())
      {
        BitmapEncoder encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(image.Source as BitmapSource));
        encoder.Save(ms);
        ms.Flush();
        return new System.Drawing.Bitmap(ms);
      }
    }

    private static Font GetPdfFont(String text, Double fontSize, FontFamily fontFamily, FontWeight fontWeight, Brush foregroundBrush)
    {
      var fontFileName = String.Empty;
      var fontName = fontFamily.ToString();
      if (Helper.FontPathDictionary.ContainsKey(fontName))
        fontFileName = Helper.FontPathDictionary[fontName];
      var fontPath = String.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.Fonts), fontFileName);
      var foregroundColor = Helper.BrushToColor(foregroundBrush);
      return GetPdfFontFromFile(text, fontPath, (float)fontSize, (fontWeight == FontWeights.Bold || fontWeight == FontWeights.Black), foregroundColor);
    }
    private static Font GetPdfFont(String fontName, float fontSize, Boolean isBold, System.Windows.Media.Color color)
    {
      if (!Helper.SystemFontNames.Contains(fontName))
        fontName = Helper.SystemFontNames.FirstOrDefault();
      return FontFactory.GetFont(fontName, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, fontSize, isBold ? 1 : 0, PdfPrinter.GetBaseColor(color));
    }
    private static Font GetPdfFontFromFile(String text, String fontPath, float fontSize, Boolean isBold, System.Windows.Media.Color color)
    {
      var path = Helper.GetSplitStrings(fontPath, true, ',');
      if (path.Count == 0 || !System.IO.File.Exists(path[0]))
        return GetPdfFont(null, fontSize, isBold, color);

      var fontDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
      var isEmbedded = true;
      if (System.IO.Path.GetDirectoryName(fontPath) == fontDirectory)
        isEmbedded = false;
      var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, isEmbedded ? BaseFont.EMBEDDED : BaseFont.NOT_EMBEDDED);
      if (!CharExists(text, baseFont))
      {
        var chineseFontPath = String.Format("{0}\\{1}", fontDirectory, "simsun.ttc,0");
        baseFont = BaseFont.CreateFont(chineseFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        //if (!CharExists(text, baseFont))
        //{
        //  var europeanFontPath = String.Format("{0}\\{1}", fontDirectory, "verdana.ttf");
        //  baseFont = BaseFont.CreateFont(europeanFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        //}
      }
      return new Font(baseFont, fontSize, isBold ? 1 : 0, PdfPrinter.GetBaseColor(color));
    }

    private static Boolean CharExists(String text, BaseFont baseFont)
    {
      if (String.IsNullOrWhiteSpace(text))
        return true;
      foreach (int c in text)
      {
        if (c == 0x00 || c == 0x0d || c == 0x0a)
          continue;
        if (!baseFont.CharExists(c))
          return false;
      }
      return true;
    }
    private static void ReleasePdfPTable(PdfPTable pdfTable)
    {
      if (pdfTable == null)
        return;

      foreach (PdfPRow row in pdfTable.Rows)
      {
        var cells = row.GetCells().Where(p => p != null && p.Table != null).ToList();
        foreach (var cell in cells)
        {
          ReleasePdfPTable(cell.Table);
          cell.Table.DeleteBodyRows();
        }
      }
    }

    private static void PrintBackground(FrameworkElement uiContainer, FrameworkElement relativeTo, PdfContentByte dc)
    {
      if (uiContainer == null || dc == null)
        return;

      PdfPTable pdfTable = null;
      try
      {
        var pdfCell = CreatePdfPCell(uiContainer);
        if (pdfCell == null)
          return;

        if (pdfCell.BackgroundColor.A == 0 && pdfCell.BorderColor.A == 0)
          return;

        pdfTable = CreatePdfTable(new float[] { (float)uiContainer.ActualWidth }, uiContainer != null ? uiContainer.HorizontalAlignment : HorizontalAlignment.Left);
        pdfTable.AddCell(pdfCell);
        pdfCell = null;

        float left, top;
        if (GetElementLocation(uiContainer, relativeTo, out left, out top))
          pdfTable.WriteSelectedRows(0, -1, left, top, dc);
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
      finally
      {
        ReleasePdfPTable(pdfTable);
        pdfTable = null;
      }
    }
    private static Boolean GetElementLocation(UIElement uiElement, FrameworkElement relativeTo, out float left, out float top)
    {
      left = 0;
      top = 0;
      if (uiElement == null || relativeTo == null)
        return false;

      try
      {
        var location = uiElement.TranslatePoint(OrginalPoint, relativeTo);
        left = (float)location.X;
        top = (float)(relativeTo.ActualHeight - location.Y);
        return true;
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        return false;
      }
    }
    private static Int32 NormalizeRotationAngle(RotateTransform rotateTransform)
    {
      if (rotateTransform == null)
        return 0;

      // Zehong: iTextSharp & WPF have opposite rotation direction
      var rotationAngle = ((Int32)rotateTransform.Angle) % 360;
      if (rotationAngle < 0)
        rotationAngle *= -1;
      else if (rotationAngle > 0)
        rotationAngle = 360 - rotationAngle;

      if (rotationAngle > 45 && rotationAngle <= 135)
        return 90;
      if (rotationAngle > 135 && rotationAngle <= 225)
        return 180;
      if (rotationAngle > 225 && rotationAngle <= 315)
        return 270;
      return 0;
    }

    #endregion

    private static readonly iTextSharp.text.Color TransparentBaseColor = new iTextSharp.text.Color(0, 0, 0, 0);
    private static readonly Point OrginalPoint = new Point(0, 0);
  }
  public class GradientBackgroundEvent : IPdfPCellEvent
  {
    public GradientBackgroundEvent(PdfWriter writer, Brush backgroundBrush)
    {
      this._writer = writer;
      this._backgroundBrush = backgroundBrush;
    }
    public void CellLayout(PdfPCell cell, iTextSharp.text.Rectangle position, PdfContentByte[] canvases)
    {
      if (Helper.IsTransparent(_backgroundBrush))
        position.BackgroundColor = null;

      var gradientBrush = _backgroundBrush as GradientBrush;
      if (gradientBrush != null)
      {
        PdfShading shading = null;

        var fromColor = PdfPrinter.GetBaseColor(gradientBrush.GradientStops.FirstOrDefault().Color);
        var toColor = PdfPrinter.GetBaseColor(gradientBrush.GradientStops.LastOrDefault().Color);

        var linearGradientBrush = _backgroundBrush as LinearGradientBrush;
        if (linearGradientBrush != null)
        {
          float x0, y0, x1, y1;
          var brushSlopeAbs = Math.Abs(linearGradientBrush.EndPoint.Y - linearGradientBrush.StartPoint.Y) / Math.Abs(linearGradientBrush.EndPoint.X - linearGradientBrush.StartPoint.X);
          var cellSlopeAbs = position.Height / position.Width;
          if (linearGradientBrush.EndPoint.X > linearGradientBrush.StartPoint.X) // from left to right
          {
            x0 = position.Left;
            if (linearGradientBrush.EndPoint.Y > linearGradientBrush.StartPoint.Y) // from top to bottom
            {
              y0 = position.Top;
              if (cellSlopeAbs >= brushSlopeAbs)
              {
                x1 = position.Left + (float)(position.Height / brushSlopeAbs);
                y1 = position.Bottom;
              }
              else
              {
                x1 = position.Right;
                y1 = position.Top - (float)(position.Width * brushSlopeAbs);
              }
            }
            else // from bottom to top
            {
              y0 = position.Bottom;
              if (cellSlopeAbs >= brushSlopeAbs)
              {
                x1 = position.Left + (float)(position.Height / brushSlopeAbs);
                y1 = position.Top;
              }
              else
              {
                x1 = position.Right;
                y1 = position.Bottom + (float)(position.Width * brushSlopeAbs);
              }
            }
          }
          else // from right to left
          {
            x0 = position.Right;
            if (linearGradientBrush.EndPoint.Y > linearGradientBrush.StartPoint.Y) // from top to bottom
            {
              y0 = position.Top;
              if (cellSlopeAbs >= brushSlopeAbs)
              {
                x1 = position.Right - (float)(position.Height / brushSlopeAbs);
                y1 = position.Bottom;
              }
              else
              {
                x1 = position.Right;
                y1 = position.Top - (float)(position.Width * brushSlopeAbs);
              }
            }
            else // from bottom to top
            {
              y0 = position.Bottom;
              if (cellSlopeAbs >= brushSlopeAbs)
              {
                x1 = position.Right - (float)(position.Height / brushSlopeAbs);
                y1 = position.Top;
              }
              else
              {
                x1 = position.Right;
                y1 = position.Bottom + (float)(position.Width * brushSlopeAbs);
              }
            }
          }

          shading = PdfShading.SimpleAxial(_writer, x0, y0, x1, y1, fromColor, toColor, false, false);
        }

        var radialGradientBrush = _backgroundBrush as RadialGradientBrush;
        if (radialGradientBrush != null)
        {
          float xCenter = 0, yCenter = 0, radius = 0;
          shading = PdfShading.SimpleRadial(_writer, xCenter, yCenter, 0, xCenter, yCenter, radius, fromColor, toColor, true, true);
        }

        if (shading != null)
          position.BackgroundColor = new ShadingColor(new PdfShadingPattern(shading));
      }

      //Fill the rectangle
      canvases[PdfPTable.BACKGROUNDCANVAS].Rectangle(position);
    }

    private PdfWriter _writer;
    private Brush _backgroundBrush;
  }
}