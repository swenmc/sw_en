using _3DTools;
using netDxf;
using netDxf.Entities;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using WindowsShapes = System.Windows.Shapes;

namespace EXPIMP
{
    public static class CExportToDXF
    {
        //--------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------
        public static void ExportViewPort_DXF(Viewport3D viewPort)
        {
            DxfDocument doc = new DxfDocument();
            foreach (Visual3D objVisual3D in viewPort.Children)
            {
                if (objVisual3D is ScreenSpaceLines3D)
                {
                    ScreenSpaceLines3D lines3D = objVisual3D as ScreenSpaceLines3D;
                    if (lines3D == null) continue;

                    AddLinesToDXF(lines3D, doc);
                }
            }

            DateTime d = DateTime.Now;
            string fileName = string.Format("3DExportDXF_{0}{1}{2}T{3}{4}{5}.dxf",
                d.Year, d.Month.ToString("D2"), d.Day.ToString("D2"), d.Hour.ToString("D2"), d.Minute.ToString("D2"), d.Second.ToString("D2"));

            doc.Save(fileName);
        }
        //public static void ExportCanvas_DXF(Canvas canvas, double xx, double yy)
        //{
        //    //---------------------------------------------------------------------------------------
        //    // TODO 305
        //    // TO Ondrej - pokus nascalovat canvas tak, aby pri exporte sedeli jednotky, canvas sa scaluje, ale realne ostanu hodnoty rovnake, takze tade cesta nevedie :)
        //    // POtrebujeme funckie upravit tak aby sme boli schopni vyrobit Canvas nastavit mu velkost nasobenu faktorom ktorym bol realny plech zmenseny, vsetko co sa bude vykreslovat bude potom 1:1, do tohoto canvas vykreslime plech
        //    Canvas cloneCanvas = new Canvas();
        //    cloneCanvas = canvas;

        //    float ScaleRate = 10f;
        //    ScaleTransform scale = new ScaleTransform(cloneCanvas.LayoutTransform.Value.M11 * ScaleRate, cloneCanvas.LayoutTransform.Value.M22 * ScaleRate);
        //    cloneCanvas.LayoutTransform = scale;
        //    cloneCanvas.UpdateLayout();
        //    //---------------------------------------------------------------------------------------

        //    DxfDocument doc = new DxfDocument();
        //    double Z = 0; //is is 2D so Z axis is always 0
        //    double fontSize = 10;

        //    foreach (object o in cloneCanvas.Children)
        //    {
        //        System.Diagnostics.Trace.WriteLine(o.GetType());

        //        if (o is WindowsShapes.Rectangle)
        //        {
        //            WindowsShapes.Rectangle winRect = o as WindowsShapes.Rectangle;
        //            double x = Canvas.GetLeft(winRect);
        //            double y = Canvas.GetTop(winRect);
        //            //double y = Canvas.GetTop(winRect) * -1; //pretocenim podla osi y dostanem body tak ako v canvase

        //            System.Windows.Media.Color c = ((SolidColorBrush)winRect.Fill).Color;
        //            System.Drawing.Color drawingcolor = System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);

        //            System.Windows.Point pTL = winRect.RenderedGeometry.Bounds.TopLeft;
        //            System.Windows.Point pTR = winRect.RenderedGeometry.Bounds.TopRight;
        //            System.Windows.Point pBL = winRect.RenderedGeometry.Bounds.BottomLeft;
        //            System.Windows.Point pBR = winRect.RenderedGeometry.Bounds.BottomRight;
        //            //Wipeout wip = new Wipeout(new Vector2(p1.X + x, p1.Y + y), new Vector2(p2.X + x, p2.Y + y));

        //            if(winRect.RenderTransform != null)
        //            {
        //                pTL = winRect.RenderTransform.Transform(pTL);
        //                pTR = winRect.RenderTransform.Transform(pTR);
        //                pBL = winRect.RenderTransform.Transform(pBL);
        //                pBR = winRect.RenderTransform.Transform(pBR);
        //            }

        //            Solid solid = new Solid();
        //            solid.Color = new AciColor(drawingcolor);
        //            solid.Transparency = new Transparency(60); // from 0 - 90

        //            solid.FirstVertex = new Vector2(pTL.X + x, -(pTL.Y + y));
        //            solid.SecondVertex = new Vector2(pTR.X + x, -(pTR.Y + y));
        //            solid.ThirdVertex = new Vector2(pBL.X + x, -(pBL.Y + y));
        //            solid.FourthVertex = new Vector2(pBR.X + x, -(pBR.Y + y));

        //            doc.AddEntity(solid);
        //        }
        //        else if (o is WindowsShapes.Polyline)
        //        {
        //            WindowsShapes.Polyline winPol = o as WindowsShapes.Polyline;
        //            Polyline poly = new Polyline();

        //            //vznika tam posun...ani srnka netusi preco
        //            //double x = Canvas.GetLeft(winPol);
        //            //double y = Canvas.GetTop(winPol);

        //            foreach (System.Windows.Point p in winPol.Points)
        //            {
        //                poly.Vertexes.Add(new PolylineVertex(p.X + xx, p.Y - yy, Z));
        //            }

        //            doc.AddEntity(poly);
        //        }
        //        else if (o is WindowsShapes.Ellipse)
        //        {
        //            WindowsShapes.Ellipse winElipse = o as WindowsShapes.Ellipse;
        //            double majorAxis = winElipse.Width;
        //            double minorAxis = winElipse.Height;

        //            System.Windows.Point p1 = winElipse.RenderedGeometry.Bounds.TopLeft;
        //            System.Windows.Point p2 = winElipse.RenderedGeometry.Bounds.BottomRight;
        //            System.Windows.Point pCenter = new System.Windows.Point((p2.X - p1.X) / 2, (p2.Y - p1.Y) / 2);

        //            double x = Canvas.GetLeft(winElipse);
        //            double y = Canvas.GetTop(winElipse);
        //            Ellipse elipse = new Ellipse(new Vector2(pCenter.X + x, pCenter.Y + y), majorAxis, minorAxis);

        //            doc.AddEntity(elipse);
        //        }
        //        else if (o is WindowsShapes.Line)
        //        {
        //            WindowsShapes.Line winLine = o as WindowsShapes.Line;

        //            Vector2 startPoint = new Vector2(winLine.X1, winLine.Y1);
        //            Vector2 endPoint = new Vector2(winLine.X2, winLine.Y2);
        //            Line line = new Line(startPoint, endPoint);

        //            doc.AddEntity(line);
        //        }
        //        else if (o is System.Windows.Controls.TextBlock)
        //        {
        //            System.Windows.Controls.TextBlock winText = o as System.Windows.Controls.TextBlock;

        //            double x = Canvas.GetLeft(winText);
        //            x += winText.ActualWidth / 2;
        //            double y = Canvas.GetTop(winText);
        //            y -= winText.BaselineOffset;
        //            y += fontSize / 2;

        //            Text txt = new Text(winText.Text, new Vector2(x, y), fontSize);
        //            //Text txt = new Text(winText.Text, new Vector2(x, -y), fontSize);  //pretocenim podla osi y dostanem body tak ako v canvase
        //            txt.Color = AciColor.Yellow;
        //            doc.AddEntity(txt);

        //            //Takto sa da spravit zlozitejsi text, napr. Bold atd..
        //            /*TextStyle style = new TextStyle("Times.ttf");
        //            //TextStyle style = TextStyle.Default;
        //            MText mText = new MText(new Vector2(x, y), fontSize, 100.0f, style);
        //            mText.Layer = new Layer("Multiline Text");
        //            //mText.Layer.Color.Index = 8;
        //            mText.Rotation = 0;
        //            //mText.LineSpacingFactor = 1.0;
        //            //mText.ParagraphHeightFactor = 1.0;
        //            //mText.AttachmentPoint = MTextAttachmentPoint.TopCenter;

        //            MTextFormattingOptions options = new MTextFormattingOptions(mText.Style);
        //            options.Bold = true;
        //            options.Color = AciColor.Yellow;
        //            mText.Write(winText.Text, options);
        //            mText.EndParagraph();
        //            doc.AddEntity(mText);    */
        //        }
        //    }

        //    DateTime d = DateTime.Now;
        //    string fileName = string.Format("ExportDXF_{0}{1}{2}T{3}{4}{5}.dxf",
        //        d.Year, d.Month.ToString("D2"), d.Day.ToString("D2"), d.Hour.ToString("D2"), d.Minute.ToString("D2"), d.Second.ToString("D2"));

        //    doc.Save(fileName);
        //}

        
        public static void ExportCanvas_DXF(Canvas canvas, double xx, double yy)
        {
            //---------------------------------------------------------------------------------------
            // TODO 305
            // TO Ondrej - pokus nascalovat canvas tak, aby pri exporte sedeli jednotky, canvas sa scaluje, ale realne ostanu hodnoty rovnake, takze tade cesta nevedie :)
            // POtrebujeme funckie upravit tak aby sme boli schopni vyrobit Canvas nastavit mu velkost nasobenu faktorom ktorym bol realny plech zmenseny, vsetko co sa bude vykreslovat bude potom 1:1, do tohoto canvas vykreslime plech
            //Canvas cloneCanvas = new Canvas();
            //cloneCanvas = canvas;

            //float ScaleRate = 10f;
            //ScaleTransform scale = new ScaleTransform(cloneCanvas.LayoutTransform.Value.M11 * ScaleRate, cloneCanvas.LayoutTransform.Value.M22 * ScaleRate);
            //cloneCanvas.LayoutTransform = scale;
            //cloneCanvas.UpdateLayout();
            //---------------------------------------------------------------------------------------

            DxfDocument doc = new DxfDocument();
            double Z = 0; //is is 2D so Z axis is always 0
            double fontSize = 10;

            foreach (object o in canvas.Children)
            {
                System.Diagnostics.Trace.WriteLine(o.GetType());

                if (o is WindowsShapes.Rectangle)
                {
                    WindowsShapes.Rectangle winRect = o as WindowsShapes.Rectangle;
                    double x = Canvas.GetLeft(winRect);
                    double y = Canvas.GetTop(winRect);
                    //double y = Canvas.GetTop(winRect) * -1; //pretocenim podla osi y dostanem body tak ako v canvase

                    System.Windows.Media.Color c = ((SolidColorBrush)winRect.Fill).Color;
                    System.Drawing.Color drawingcolor = System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);

                    System.Windows.Point pTL = winRect.RenderedGeometry.Bounds.TopLeft;
                    System.Windows.Point pTR = winRect.RenderedGeometry.Bounds.TopRight;
                    System.Windows.Point pBL = winRect.RenderedGeometry.Bounds.BottomLeft;
                    System.Windows.Point pBR = winRect.RenderedGeometry.Bounds.BottomRight;
                    //Wipeout wip = new Wipeout(new Vector2(p1.X + x, p1.Y + y), new Vector2(p2.X + x, p2.Y + y));

                    if (winRect.RenderTransform != null)
                    {
                        pTL = winRect.RenderTransform.Transform(pTL);
                        pTR = winRect.RenderTransform.Transform(pTR);
                        pBL = winRect.RenderTransform.Transform(pBL);
                        pBR = winRect.RenderTransform.Transform(pBR);
                    }

                    Solid solid = new Solid();
                    solid.Color = new AciColor(drawingcolor);
                    solid.Transparency = new Transparency(60); // from 0 - 90

                    solid.FirstVertex = new Vector2(pTL.X + x, -(pTL.Y + y));
                    solid.SecondVertex = new Vector2(pTR.X + x, -(pTR.Y + y));
                    solid.ThirdVertex = new Vector2(pBL.X + x, -(pBL.Y + y));
                    solid.FourthVertex = new Vector2(pBR.X + x, -(pBR.Y + y));
                    solid.Layer = new netDxf.Tables.Layer("Rectangle");
                    doc.AddEntity(solid);
                }
                else if (o is WindowsShapes.Polyline)
                {
                    WindowsShapes.Polyline winPol = o as WindowsShapes.Polyline;
                    Polyline poly = new Polyline();

                    //vznika tam posun...ani srnka netusi preco
                    //double x = Canvas.GetLeft(winPol);
                    //double y = Canvas.GetTop(winPol);

                    foreach (System.Windows.Point p in winPol.Points)
                    {
                        poly.Vertexes.Add(new PolylineVertex(p.X + xx, p.Y - yy, Z));
                    }
                    poly.Layer = new netDxf.Tables.Layer("PolyLine");
                    doc.AddEntity(poly);
                }
                else if (o is WindowsShapes.Ellipse)
                {
                    WindowsShapes.Ellipse winElipse = o as WindowsShapes.Ellipse;
                    double majorAxis = winElipse.Width;
                    double minorAxis = winElipse.Height;

                    System.Windows.Point p1 = winElipse.RenderedGeometry.Bounds.TopLeft;
                    System.Windows.Point p2 = winElipse.RenderedGeometry.Bounds.BottomRight;
                    System.Windows.Point pCenter = new System.Windows.Point((p2.X - p1.X) / 2, (p2.Y - p1.Y) / 2);

                    double x = Canvas.GetLeft(winElipse);
                    double y = Canvas.GetTop(winElipse);
                    Ellipse elipse = new Ellipse(new Vector2(pCenter.X + x, pCenter.Y + y), majorAxis, minorAxis);
                    elipse.Layer = new netDxf.Tables.Layer("Elipse");
                    doc.AddEntity(elipse);
                }
                else if (o is WindowsShapes.Line)
                {
                    WindowsShapes.Line winLine = o as WindowsShapes.Line;

                    Vector2 startPoint = new Vector2(winLine.X1, winLine.Y1);
                    Vector2 endPoint = new Vector2(winLine.X2, winLine.Y2);
                    Line line = new Line(startPoint, endPoint);
                    line.Layer = new netDxf.Tables.Layer("Line");
                    doc.AddEntity(line);
                }
                else if (o is System.Windows.Controls.TextBlock)
                {
                    System.Windows.Controls.TextBlock winText = o as System.Windows.Controls.TextBlock;

                    double x = Canvas.GetLeft(winText);
                    x += winText.ActualWidth / 2;
                    double y = Canvas.GetTop(winText);
                    y -= winText.BaselineOffset;
                    y += fontSize / 2;

                    Text txt = new Text(winText.Text, new Vector2(x, y), fontSize);
                    //Text txt = new Text(winText.Text, new Vector2(x, -y), fontSize);  //pretocenim podla osi y dostanem body tak ako v canvase
                    txt.Color = AciColor.Yellow;
                    txt.Layer = new netDxf.Tables.Layer("Text");
                    doc.AddEntity(txt);

                    //Takto sa da spravit zlozitejsi text, napr. Bold atd..
                    /*TextStyle style = new TextStyle("Times.ttf");
                    //TextStyle style = TextStyle.Default;
                    MText mText = new MText(new Vector2(x, y), fontSize, 100.0f, style);
                    mText.Layer = new Layer("Multiline Text");
                    //mText.Layer.Color.Index = 8;
                    mText.Rotation = 0;
                    //mText.LineSpacingFactor = 1.0;
                    //mText.ParagraphHeightFactor = 1.0;
                    //mText.AttachmentPoint = MTextAttachmentPoint.TopCenter;

                    MTextFormattingOptions options = new MTextFormattingOptions(mText.Style);
                    options.Bold = true;
                    options.Color = AciColor.Yellow;
                    mText.Write(winText.Text, options);
                    mText.EndParagraph();
                    doc.AddEntity(mText);    */
                }
            }

            DateTime d = DateTime.Now;
            string fileName = string.Format("ExportDXF_{0}{1}{2}T{3}{4}{5}.dxf",
                d.Year, d.Month.ToString("D2"), d.Day.ToString("D2"), d.Hour.ToString("D2"), d.Minute.ToString("D2"), d.Second.ToString("D2"));

            doc.Save(fileName);
        }

        public static void ExportViewPort_DXF_Test(Viewport3D viewPort)
        {
            netDxf.DxfDocument doc = new netDxf.DxfDocument();

            foreach (Visual3D objVisual3D in viewPort.Children)
            {
                if (objVisual3D is ScreenSpaceLines3D)
                {
                    ScreenSpaceLines3D lines3D = objVisual3D as ScreenSpaceLines3D;
                    if (lines3D == null) continue;

                    //AddLinesToDXF(lines3D, doc);
                }
                else if (objVisual3D is ModelVisual3D)
                {
                    if ((objVisual3D as ModelVisual3D).Content is Model3DGroup)
                    {
                        foreach (Model3D m3D in ((Model3DGroup)(objVisual3D as ModelVisual3D).Content).Children)
                        {
                            if (m3D is Model3DGroup)
                            {
                                //System.Diagnostics.Trace.WriteLine(m3D.Bounds);
                                //if(((Model3DGroup)m3D).Children.Count == 0) AddRect3DToDXF(m3D.Bounds, doc);

                                foreach (Model3D childM3D in ((Model3DGroup)m3D).Children)
                                {
                                    if (childM3D is GeometryModel3D)
                                    {
                                        System.Diagnostics.Trace.WriteLine(childM3D.Bounds);
                                        //AddRect3DToDXF(m3D.Bounds, doc);
                                        AddGeometryModel3DToDXF(((GeometryModel3D)childM3D).Geometry as MeshGeometry3D, doc);
                                        //System.Diagnostics.Trace.WriteLine(((GeometryModel3D)childM3D).Geometry);
                                    }
                                }
                            }
                        } //foreach
                    } //if
                } //end else if
            }

            DateTime d = DateTime.Now;
            string fileName = string.Format("3DExportDXF_{0}{1}{2}T{3}{4}{5}.dxf",
                d.Year, d.Month.ToString("D2"), d.Day.ToString("D2"), d.Hour.ToString("D2"), d.Minute.ToString("D2"), d.Second.ToString("D2"));

            doc.Save(fileName);
        }
        //--------------------------------------------------------------------------------------------------
        private static void AddLinesToDXF(ScreenSpaceLines3D lines3D, DxfDocument doc)
        {
            Point3D startPoint;
            Point3D endPoint;
            for (int i = 0; i < lines3D.Points.Count; i = i + 2)
            {
                startPoint = lines3D.Points[i];
                endPoint = lines3D.Points[i + 1];
                Line line = new Line(new Vector3(startPoint.X, startPoint.Y, startPoint.Z), new Vector3(endPoint.X, endPoint.Y, endPoint.Z));

                doc.AddEntity(line);
            }
        }


        private static void AddRect3DToDXF(Rect3D bounds, DxfDocument doc)
        {
            bool topView = false;
            bool frontView = true;
            bool sideView = false;

            if (topView)
            {
                Polyline poly1 = new Polyline();
                poly1.Vertexes.Add(new PolylineVertex(bounds.X, bounds.Y, 0));
                poly1.Vertexes.Add(new PolylineVertex(bounds.X + bounds.SizeX, bounds.Y, 0));
                poly1.Vertexes.Add(new PolylineVertex(bounds.X + bounds.SizeX, bounds.Y + bounds.SizeY, 0));
                poly1.Vertexes.Add(new PolylineVertex(bounds.X, bounds.Y + bounds.SizeY, 0));
                poly1.Vertexes.Add(new PolylineVertex(bounds.X, bounds.Y, 0));
                doc.AddEntity(poly1);
            }
            if (frontView)
            {
                Polyline poly2 = new Polyline();
                poly2.Vertexes.Add(new PolylineVertex(bounds.X, bounds.Z, 0));
                poly2.Vertexes.Add(new PolylineVertex(bounds.X + bounds.SizeX, bounds.Z, 0));
                poly2.Vertexes.Add(new PolylineVertex(bounds.X + bounds.SizeX, bounds.Z + bounds.SizeZ, 0));
                poly2.Vertexes.Add(new PolylineVertex(bounds.X, bounds.Z + bounds.SizeZ, 0));
                poly2.Vertexes.Add(new PolylineVertex(bounds.X, bounds.Z, 0));
                doc.AddEntity(poly2);
            }
            if (sideView)
            {
                Polyline poly3 = new Polyline();
                poly3.Vertexes.Add(new PolylineVertex(bounds.Y, bounds.Z, 0));
                poly3.Vertexes.Add(new PolylineVertex(bounds.Y + bounds.SizeX, bounds.Z, 0));
                poly3.Vertexes.Add(new PolylineVertex(bounds.Y + bounds.SizeX, bounds.Z + bounds.SizeZ, 0));
                poly3.Vertexes.Add(new PolylineVertex(bounds.Y, bounds.Z + bounds.SizeZ, 0));
                poly3.Vertexes.Add(new PolylineVertex(bounds.Y, bounds.Z, 0));
                doc.AddEntity(poly3);
            }
        }


        private static void AddGeometryModel3DToDXF(MeshGeometry3D geometry, DxfDocument doc)
        {
            if (geometry == null) return;

            bool topView = false;
            bool frontView = true;
            bool sideView = false;

            if (topView)
            {
                netDxf.Entities.Polyline poly = new netDxf.Entities.Polyline();

                Point3D p1 = geometry.Positions.OrderBy(p => p.X).ThenBy(p => p.Y).FirstOrDefault();
                Point3D p2 = geometry.Positions.OrderByDescending(p => p.X).ThenByDescending(p => p.Y).FirstOrDefault();
                Point3D p3 = geometry.Positions.OrderBy(p => p.Y).ThenByDescending(p => p.X).FirstOrDefault();
                Point3D p4 = geometry.Positions.OrderByDescending(p => p.Y).ThenBy(p => p.X).FirstOrDefault();

                poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(p1.X, p1.Y, 0));
                poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(p3.X, p3.Y, 0));
                poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(p2.X, p2.Y, 0));
                poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(p4.X, p4.Y, 0));
                poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(p1.X, p1.Y, 0));

                //double minX = geometry.Positions.Min(p => p.X);
                //double minY = geometry.Positions.Min(p => p.Y);
                //double maxX = geometry.Positions.Max(p => p.X);
                //double maxY = geometry.Positions.Max(p => p.Y);
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(minX, minY, 0));
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(maxX, minY, 0));
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(maxX, maxY, 0));
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(minX, maxY, 0));
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(minX, minY, 0));
                doc.AddEntity(poly);
            }
            if (frontView)
            {
                Polyline poly = new Polyline();

                Point3D p1 = geometry.Positions.OrderBy(p => p.X).ThenBy(p => p.Z).FirstOrDefault();
                Point3D p2 = geometry.Positions.OrderByDescending(p => p.X).ThenByDescending(p => p.Z).FirstOrDefault();
                Point3D p3 = geometry.Positions.OrderBy(p => p.Z).ThenByDescending(p => p.X).FirstOrDefault();
                Point3D p4 = geometry.Positions.OrderByDescending(p => p.Z).ThenBy(p => p.X).FirstOrDefault();

                poly.Vertexes.Add(new PolylineVertex(p1.X, p1.Z, 0));
                poly.Vertexes.Add(new PolylineVertex(p3.X, p3.Z, 0));
                poly.Vertexes.Add(new PolylineVertex(p2.X, p2.Z, 0));
                poly.Vertexes.Add(new PolylineVertex(p4.X, p4.Z, 0));
                poly.Vertexes.Add(new PolylineVertex(p1.X, p1.Z, 0));

                //or                
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(p2.X, p2.Z, 0));
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(p3.X, p3.Z, 0));
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(p1.X, p1.Z, 0));
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(p4.X, p4.Z, 0));
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(p2.X, p2.Z, 0));

                //double minX = geometry.Positions.Min(p => p.X);
                //double minZ = geometry.Positions.Min(p => p.Z);
                //double maxX = geometry.Positions.Max(p => p.X);
                //double maxZ = geometry.Positions.Max(p => p.Z);
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(minX, minZ, 0));
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(maxX, minZ, 0));
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(maxX, maxZ, 0));
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(minX, maxZ, 0));
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(minX, minZ, 0));
                doc.AddEntity(poly);
            }
            if (sideView)
            {
                Polyline poly = new Polyline();
                Point3D p1 = geometry.Positions.OrderBy(p => p.Y).ThenBy(p => p.Z).FirstOrDefault();
                Point3D p2 = geometry.Positions.OrderByDescending(p => p.Y).ThenByDescending(p => p.Z).FirstOrDefault();
                Point3D p3 = geometry.Positions.OrderBy(p => p.Z).ThenByDescending(p => p.Y).FirstOrDefault();
                Point3D p4 = geometry.Positions.OrderByDescending(p => p.Z).ThenBy(p => p.Y).FirstOrDefault();

                poly.Vertexes.Add(new PolylineVertex(p1.Y, p1.Z, 0));
                poly.Vertexes.Add(new PolylineVertex(p3.Y, p3.Z, 0));
                poly.Vertexes.Add(new PolylineVertex(p2.Y, p2.Z, 0));
                poly.Vertexes.Add(new PolylineVertex(p4.Y, p4.Z, 0));
                poly.Vertexes.Add(new PolylineVertex(p1.Y, p1.Z, 0));

                //double minY = geometry.Positions.Min(p => p.Y);
                //double minZ = geometry.Positions.Min(p => p.Z);
                //double maxY = geometry.Positions.Max(p => p.Y);
                //double maxZ = geometry.Positions.Max(p => p.Z);
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(minY, minZ, 0));
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(maxY, minZ, 0));
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(maxY, maxZ, 0));
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(minY, maxZ, 0));
                //poly.Vertexes.Add(new netDxf.Entities.PolylineVertex(minY, minZ, 0));
                doc.AddEntity(poly);
            }
        }




    }

}