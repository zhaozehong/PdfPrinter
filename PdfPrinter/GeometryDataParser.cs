using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using iTextSharp.text.pdf;
using System.Windows.Media;
using System.Diagnostics;
using Zehong.CSharp.Solution.HelperLib;

namespace Zehong.CSharp.Solution.PdfPrinter
{
  public static class GeometryDataParser
  {
    public static List<IGeometryDataFigure> GetDataFigures(String strGeometryData)
    {
      if (String.IsNullOrWhiteSpace(strGeometryData) || !strGeometryData.StartsWith("M"))
        return null;

      var figures = new List<IGeometryDataFigure>();
      var commandChars = strGeometryData.Where(p => (p >= 'A' && p <= 'Z') || (p >= 'a' && p <= 'z')).ToList();
      var leftString = strGeometryData.ToString();
      for (int i = 0; i < commandChars.Count; i++)
      {
        int iTo = leftString.Length;
        if (i + 1 < commandChars.Count)
          iTo = leftString.IndexOf(commandChars[i + 1]);

        IGeometryDataFigure figure = null;
        var strCommand = leftString.Substring(0, iTo);
        if (strCommand.StartsWith("M", StringComparison.InvariantCultureIgnoreCase))
          figure = MoveTo.Parse(strCommand, figures.LastOrDefault());
        else if (strCommand.StartsWith("L", StringComparison.InvariantCultureIgnoreCase))
          figure = LineTo.Parse(strCommand, figures.LastOrDefault());
        else if (strCommand.StartsWith("A", StringComparison.InvariantCultureIgnoreCase))
          figure = ArcTo.Parse(strCommand, figures.LastOrDefault());
        else if (strCommand.StartsWith("Z", StringComparison.InvariantCultureIgnoreCase))
          figure = CloseTo.Parse(strCommand, figures.LastOrDefault());
        else
          ExceptionHandler.ThrowException(String.Format("Unsupported geometry command: \"{0}\"", strCommand));
        if (figure != null)
          figures.Add(figure);

        leftString = leftString.Substring(iTo);
      }
      return figures;
    }
  }

  public interface IGeometryDataFigure
  {
    /// <summary>
    /// The previous figure.
    /// </summary>
    IGeometryDataFigure PreviousFigure { get; }
    /// <summary>
    /// The absolute start point of current figure.
    /// </summary>
    Point StartPoint { get; }
    /// <summary>
    /// The absolute end point of current figure.
    /// </summary>
    Point EndPoint { get; }

    /// <summary>
    /// Render current figure
    /// </summary>
    /// <param name="left">Left position of canvas</param>
    /// <param name="top">Top position of canvas</param>
    /// <param name="dc">Drawing dc</param>
    /// <param name="transform">Currently applied transform</param>
    /// <returns>Return true if it's successful</returns>
    Boolean Draw(float left, float top, PdfContentByte dc, Transform transform);
  }
  public class MoveTo : IGeometryDataFigure
  {
    private MoveTo(Point point, IGeometryDataFigure previousFigure, Boolean isOffset = false)
    {
      this.PreviousFigure = previousFigure;
      this.StartPoint = point;
      if (isOffset && previousFigure != null)
        this.StartPoint = new Point(previousFigure.EndPoint.X + point.X, previousFigure.EndPoint.Y + point.Y);
    }

    public static MoveTo Parse(String strCommand, IGeometryDataFigure previousFigure)
    {
      if (String.IsNullOrWhiteSpace(strCommand) || !strCommand.StartsWith("M", StringComparison.InvariantCultureIgnoreCase))
        return null;

      var datas = Helper.GetSplitStrings(strCommand.Substring(1), true, ',');
      if (datas.Count != 2)
        return null;

      Double x, y;
      if (!Double.TryParse(datas[0], out x))
        return null;
      if (!Double.TryParse(datas[1], out y))
        return null;

      return new MoveTo(new Point(x, y), previousFigure, strCommand.StartsWith("m"));
    }
    public Boolean Draw(float left, float top, PdfContentByte dc, Transform transform)
    {
      try
      {
        var startPoint = transform.Transform(StartPoint);
        dc.MoveTo(left + startPoint.X, top - startPoint.Y);
        return true;
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        return false;
      }
    }

    public IGeometryDataFigure PreviousFigure { get; private set; }
    public Point StartPoint { get; private set; }
    public Point EndPoint { get { return StartPoint; } }
  }
  public class LineTo : IGeometryDataFigure
  {
    private LineTo(List<Point> points, IGeometryDataFigure previousFigure, Boolean isOffset = false)
    {
      Debug.Assert(points != null && points.Any());

      this.Points = points;
      if (isOffset && previousFigure != null)
        this.Points = points.Select(p => new Point(previousFigure.EndPoint.X + p.X, previousFigure.EndPoint.Y + p.Y)).ToList();

      this.PreviousFigure = previousFigure;
      this.StartPoint = previousFigure != null ? previousFigure.EndPoint : new Point(0, 0);
      this.EndPoint = this.Points.LastOrDefault();
    }

    public static LineTo Parse(String strCommand, IGeometryDataFigure previousFigure)
    {
      if (String.IsNullOrWhiteSpace(strCommand) || !strCommand.StartsWith("L", StringComparison.InvariantCultureIgnoreCase))
        return null;

      var pointStrings = Helper.GetSplitStrings(strCommand.Substring(1), true, ' ');
      if (!pointStrings.Any())
        return null;

      var points = new List<Point>();
      foreach (var pointString in pointStrings)
      {
        var datas = Helper.GetSplitStrings(pointString, true, ',');
        if (datas.Count != 2)
          return null;

        Double x, y;
        if (!Double.TryParse(datas[0], out x))
          return null;
        if (!Double.TryParse(datas[1], out y))
          return null;

        points.Add(new Point(x, y));
      }
      return new LineTo(points, previousFigure, strCommand.StartsWith("l"));
    }
    public Boolean Draw(float left, float top, PdfContentByte dc, Transform transform)
    {
      try
      {
        var points = this.Points.Select(p => transform.Transform(p)).ToList();
        points.ForEach(point => dc.LineTo(left + point.X, top - point.Y));
        return true;
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        return false;
      }
    }

    public IGeometryDataFigure PreviousFigure { get; private set; }
    public Point StartPoint { get; private set; }
    public Point EndPoint { get; private set; }
    public List<Point> Points { get; private set; }
  }
  public class ArcTo : IGeometryDataFigure
  {
    private ArcTo(Double radiusX, Double radiusY, Double rotationAngle, int isLargeArc, int sweepDirection, Point endPoint, IGeometryDataFigure previousFigure, Boolean isOffset = false)
    {
      this.RadiusX = radiusX;
      this.RadiusY = radiusY;
      this.RotationAngle = rotationAngle;
      this.IsLargeArc = isLargeArc == 1;
      this.SweepDirection = sweepDirection == 1;
      this.EndPoint = endPoint;
      if (isOffset && previousFigure != null)
        this.EndPoint = new Point(previousFigure.EndPoint.X + endPoint.X, previousFigure.EndPoint.Y + endPoint.Y);

      this.PreviousFigure = previousFigure;
      this.StartPoint = previousFigure != null ? previousFigure.EndPoint : new Point(0, 0);
    }

    public static ArcTo Parse(String strCommand, IGeometryDataFigure previousFigure)
    {
      if (String.IsNullOrWhiteSpace(strCommand) || !strCommand.StartsWith("A", StringComparison.InvariantCultureIgnoreCase))
        return null;

      var datas = Helper.GetSplitStrings(strCommand.Substring(1), true, ',');
      if (datas.Count != 7)
        return null;

      double radiusX, radiusY, rotationAngle, endPointX, endPointY;
      int isLargeArc, sweepDirection;
      if (!Double.TryParse(datas[0], out radiusX))
        return null;
      if (!Double.TryParse(datas[1], out radiusY))
        return null;
      if (!Double.TryParse(datas[2], out rotationAngle))
        return null;
      if (!Int32.TryParse(datas[3], out isLargeArc))
        return null;
      if (!Int32.TryParse(datas[4], out sweepDirection))
        return null;
      if (!Double.TryParse(datas[5], out endPointX))
        return null;
      if (!Double.TryParse(datas[6], out endPointY))
        return null;

      return new ArcTo(radiusX, radiusY, rotationAngle, isLargeArc, sweepDirection, new Point(endPointX, endPointY), previousFigure, strCommand.StartsWith("a"));
    }
    public Boolean Draw(float left, float top, PdfContentByte dc, Transform transform)
    {
      if (this.RotationAngle != 0)
        return false;

      try
      {
        Double x1 = this.StartPoint.X, x2 = EndPoint.X;
        Double y1 = this.StartPoint.Y, y2 = EndPoint.Y;
        Double a = RadiusX, b = RadiusY;

        var aByb = a / b;
        var M1 = aByb * aByb;
        var M2 = M1 * M1;
        var xError = x1 - x2;
        var yError = y1 - y2;
        var xErrorSquare = xError * xError;
        var yErrorSquare = yError * yError;
        var ySquareError = y1 * y1 - y2 * y2;

        var A = 4 * M1 * (M1 * yErrorSquare / xErrorSquare + 1);
        var B = -4 * M1 * yError * (1 + M1 * ySquareError / xErrorSquare + 2 * y2 / yError);
        var C = xErrorSquare + M2 * ySquareError * ySquareError / xErrorSquare + 2 * M1 * ySquareError + 4 * M1 * y2 * y2 - 4 * a * a;

        List<Point> centers = new List<Point>();
        Point transformedPoint;

        var y0 = (-B + Math.Sqrt(B * B - 4 * A * C)) / (2 * A);
        var x0 = x2;
        var temp = 1 - (y2 - y0) * (y2 - y0) / (b * b);
        if (Helper.IsNullOrZero(temp))
        {
          if (Helper.IsNullOrZero((x1 - x0) * (x1 - x0) / a / a + (y1 - y0) * (y1 - y0) / b / b - 1))
          {
            transformedPoint = transform.Transform(new Point(x0, y0));
            centers.Add(new Point(left + transformedPoint.X, top - transformedPoint.Y));
          }
        }
        else if (temp > 0)
        {
          x0 = x2 + a * Math.Sqrt(temp);
          if (Helper.IsNullOrZero((x1 - x0) * (x1 - x0) / a / a + (y1 - y0) * (y1 - y0) / b / b - 1))
          {
            transformedPoint = transform.Transform(new Point(x0, y0));
            centers.Add(new Point(left + transformedPoint.X, top - transformedPoint.Y));
          }

          x0 = x2 - a * Math.Sqrt(temp);
          if (Helper.IsNullOrZero((x1 - x0) * (x1 - x0) / a / a + (y1 - y0) * (y1 - y0) / b / b - 1))
          {
            transformedPoint = transform.Transform(new Point(x0, y0));
            centers.Add(new Point(left + transformedPoint.X, top - transformedPoint.Y));
          }
        }

        y0 = (-B - Math.Sqrt(B * B - 4 * A * C)) / (2 * A);
        x0 = x2;
        temp = 1 - (y2 - y0) * (y2 - y0) / (b * b);
        if (Helper.IsNullOrZero(temp))
        {
          if (Helper.IsNullOrZero((x1 - x0) * (x1 - x0) / a / a + (y1 - y0) * (y1 - y0) / b / b - 1))
          {
            transformedPoint = transform.Transform(new Point(x0, y0));
            centers.Add(new Point(left + transformedPoint.X, top - transformedPoint.Y));
          }
        }
        else if (temp > 0)
        {
          x0 = x2 + a * Math.Sqrt(temp);
          if (Helper.IsNullOrZero((x1 - x0) * (x1 - x0) / a / a + (y1 - y0) * (y1 - y0) / b / b - 1))
          {
            transformedPoint = transform.Transform(new Point(x0, y0));
            centers.Add(new Point(left + transformedPoint.X, top - transformedPoint.Y));
          }

          x0 = x2 - a * Math.Sqrt(temp);
          if (Helper.IsNullOrZero((x1 - x0) * (x1 - x0) / a / a + (y1 - y0) * (y1 - y0) / b / b - 1))
          {
            transformedPoint = transform.Transform(new Point(x0, y0));
            centers.Add(new Point(left + transformedPoint.X, top - transformedPoint.Y));
          }
        }

        var radius = transform.Transform(new Point(RadiusX, RadiusY));
        foreach (var centerPoint in centers)
        {
          Double startAngle = 0, entendAngle = 0;
          var startPoint = transform.Transform(this.StartPoint);
          var endPoint = transform.Transform(EndPoint);
          if (IsMatched(new Point(left + startPoint.X, top - startPoint.Y), new Point(left + endPoint.X, top - endPoint.Y), centerPoint, out startAngle, out entendAngle))
          {
            var array = PdfContentByte.BezierArc(centerPoint.X - radius.X, centerPoint.Y - radius.Y, centerPoint.X + radius.X, centerPoint.Y + radius.Y, startAngle, entendAngle);
            if (array.Count == 0)
              continue;

            var pt = (float[])array[0];
            dc.LineTo(pt[0], pt[1]);
            for (int k = 0; k < array.Count; k++)
            {
              pt = (float[])array[k];
              dc.CurveTo(pt[2], pt[3], pt[4], pt[5], pt[6], pt[7]);
            }
            break;
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        return false;
      }
    }
    private Boolean IsMatched(Point startPoint, Point endPoint, Point centerPoint, out Double startAngle, out Double entendAngle)
    {
      Vector vecStart = startPoint - centerPoint;
      Vector vecEnd = endPoint - centerPoint;

      startAngle = Vector.AngleBetween(new Vector(1, 0), vecStart);
      if (startAngle < 0)
        startAngle += 360;
      entendAngle = Vector.AngleBetween(vecStart, vecEnd);
      if (SweepDirection && entendAngle > 0)
        entendAngle -= 360;
      if (!SweepDirection && entendAngle < 0)
        entendAngle += 360;

      return IsLargeArc ? Math.Abs(entendAngle) > 180 : Math.Abs(entendAngle) < 180;
    }

    public IGeometryDataFigure PreviousFigure { get; private set; }
    public Point StartPoint { get; private set; }
    public Double RadiusX { get; private set; }
    public Double RadiusY { get; private set; }
    public Double RotationAngle { get; private set; }
    public Boolean IsLargeArc { get; private set; }
    public Boolean SweepDirection { get; private set; }
    public Point EndPoint { get; private set; }
  }
  public class CloseTo : IGeometryDataFigure
  {
    private CloseTo(IGeometryDataFigure previousFigure)
    {
      this.PreviousFigure = previousFigure;
      this.StartPoint = previousFigure != null ? previousFigure.EndPoint : new Point(0, 0);
      var moveFigure = GetPreviousMoveFigure();
      if (moveFigure != null)
        this.EndPoint = moveFigure.StartPoint;
    }

    public static CloseTo Parse(String strCommand, IGeometryDataFigure previousFigure)
    {
      if (String.IsNullOrWhiteSpace(strCommand) || !strCommand.StartsWith("Z", StringComparison.InvariantCultureIgnoreCase))
        return null;

      return new CloseTo(previousFigure);
    }
    public Boolean Draw(float left, float top, PdfContentByte dc, Transform transform)
    {
      dc.ClosePath();
      return true;
    }

    private IGeometryDataFigure GetPreviousMoveFigure()
    {
      try
      {
        if (this.PreviousFigure == null)
          return null;

        var parent = this.PreviousFigure;
        while (parent != null && !(parent is MoveTo))
        {
          parent = parent.PreviousFigure;
        }
        return parent;
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        return null;
      }
    }

    public IGeometryDataFigure PreviousFigure { get; private set; }
    public Point StartPoint { get; private set; }
    public Point EndPoint { get; private set; }
  }
}