using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Zehong.CSharp.Solution.HelperLib;

namespace Zehong.CSharp.Solution.PdfPrinter
{
  public static class GeometryDataParser
  {
    public static List<IGeometryDataFigure> GetDataFigures(String strGeometryData)
    {
      if (String.IsNullOrWhiteSpace(strGeometryData))
        return null;

      var commandChars = strGeometryData.Where(p => (p >= 'A' && p <= 'Z') || (p >= 'a' && p <= 'z')).ToList();
      if (!strGeometryData.StartsWith(commandChars[0].ToString()))
        return null;

      var leftString = strGeometryData.ToString();
      var figures = new List<IGeometryDataFigure>();
      for (int i = 0; i < commandChars.Count; i++)
      {
        int iTo = leftString.Length;
        if (i + 1 < commandChars.Count)
          iTo = leftString.IndexOf(commandChars[i + 1]);

        IGeometryDataFigure figure = null;
        var strCommand = leftString.Substring(0, iTo);
        if (strCommand.StartsWith("M", StringComparison.InvariantCultureIgnoreCase))
          figure = MoveTo.Parse(strCommand);
        else if (strCommand.StartsWith("L", StringComparison.InvariantCultureIgnoreCase))
          figure = LineTo.Parse(strCommand);
        else if (strCommand.StartsWith("A", StringComparison.InvariantCultureIgnoreCase))
          figure = ArcTo.Parse(strCommand);
        else if (strCommand.StartsWith("Z", StringComparison.InvariantCultureIgnoreCase))
          figure = CloseTo.Parse(strCommand);
        if (figure != null)
          figures.Add(figure);

        leftString = leftString.Substring(iTo);
      }
      return figures;
    }
  }

  public interface IGeometryDataFigure
  {
    Point? Draw(float left, float top, PdfContentByte dc, Transform transform, Point shapeStartPoint, Point lastEndPoint);
  }
  public class MoveTo : IGeometryDataFigure
  {
    private MoveTo(Point startPoint)
    {
      this.StartPoint = startPoint;
    }
    public Point? Draw(float left, float top, PdfContentByte dc, Transform transform, Point shapeStartPoint, Point lastEndPoint)
    {
      var startPoint = transform.Transform(StartPoint);
      dc.MoveTo(left + startPoint.X, top - startPoint.Y);
      return StartPoint;
    }

    public static MoveTo Parse(String strCommand)
    {
      if (String.IsNullOrWhiteSpace(strCommand))
        return null;
      if (strCommand.StartsWith("M"))
        strCommand = strCommand.Substring(1);

      var datas = Helper.GetSplitStrings(strCommand, true, ',');
      if (datas.Count != 2)
        return null;

      double x, y;
      if (!Double.TryParse(datas[0], out x))
        return null;
      if (!Double.TryParse(datas[1], out y))
        return null;
      return new MoveTo(new Point(x, y));
    }

    public Point StartPoint { get; private set; }
  }
  public class LineTo : IGeometryDataFigure
  {
    private LineTo(List<Point> points)
    {
      this.Points = points;
    }

    public Point? Draw(float left, float top, PdfContentByte dc, Transform transform, Point shapeStartPoint, Point lastEndPoint)
    {
      var points = this.Points.Select(p => transform.Transform(p)).ToList();
      points.ForEach(point => dc.LineTo(left + point.X, top - point.Y));
      return this.Points.Last();
    }
    public static LineTo Parse(String strCommand)
    {
      if (String.IsNullOrWhiteSpace(strCommand))
        return null;
      if (strCommand.StartsWith("L"))
        strCommand = strCommand.Substring(1);

      var pointStrings = Helper.GetSplitStrings(strCommand, true, ' ');
      if (!pointStrings.Any())
        return null;

      List<Point> points = new List<Point>();
      foreach (var pointString in pointStrings)
      {
        var datas = Helper.GetSplitStrings(pointString, true, ',');
        if (datas.Count != 2)
          return null;
        double x, y;
        if (!Double.TryParse(datas[0], out x))
          return null;
        if (!Double.TryParse(datas[1], out y))
          return null;
        points.Add(new Point(x, y));
      }
      return new LineTo(points);
    }

    public List<Point> Points { get; private set; }
  }
  public class ArcTo : IGeometryDataFigure
  {
    private ArcTo(Double radiusX, Double radiusY, Double rotationAngle, int isLargeArc, int sweepDirection, Point endPoint)
    {
      this.RadiusX = radiusX;
      this.RadiusY = radiusY;
      this.RotationAngle = rotationAngle;
      this.IsLargeArc = isLargeArc == 1;
      this.SweepDirection = sweepDirection == 1;
      this.EndPoint = endPoint;
    }

    public Point? Draw(float left, float top, PdfContentByte dc, Transform transform, Point shapeStartPoint, Point lastEndPoint)
    {
      if (this.RotationAngle != 0)
        return null;

      Double x1 = lastEndPoint.X, x2 = EndPoint.X;
      Double y1 = lastEndPoint.Y, y2 = EndPoint.Y;
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
        var startPoint = transform.Transform(lastEndPoint);
        var endPoint = transform.Transform(EndPoint);
        if (IsMatched(new Point(left + startPoint.X, top - startPoint.Y), new Point(left + endPoint.X, top - endPoint.Y), centerPoint, out startAngle, out entendAngle))
        {
          dc.Arc(centerPoint.X - radius.X, centerPoint.Y - radius.Y, centerPoint.X + radius.X, centerPoint.Y + radius.Y, startAngle, entendAngle);
          break;
        }
      }

      return EndPoint;
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
    public static ArcTo Parse(String strCommand)
    {
      if (String.IsNullOrWhiteSpace(strCommand))
        return null;
      if (strCommand.StartsWith("A"))
        strCommand = strCommand.Substring(1);

      var datas = Helper.GetSplitStrings(strCommand, true, ',');
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

      return new ArcTo(radiusX, radiusY, rotationAngle, isLargeArc, sweepDirection, new Point(endPointX, endPointY));
    }


    public Double RadiusX { get; private set; }
    public Double RadiusY { get; private set; }
    public Double RotationAngle { get; private set; }
    public Boolean IsLargeArc { get; private set; }
    public Boolean SweepDirection { get; private set; }
    public Point EndPoint { get; private set; }
  }
  public class CloseTo : IGeometryDataFigure
  {
    public static CloseTo Parse(String strCommand)
    {
      return new CloseTo();
    }
    public Point? Draw(float left, float top, PdfContentByte dc, Transform transform, Point shapeStartPoint, Point lastEndPoint)
    {
      var startPoint = transform.Transform(shapeStartPoint);
      dc.LineTo(left + startPoint.X, top - startPoint.Y);

      return null;
    }
  }
}
