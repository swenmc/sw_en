using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using BaseClasses;
using MATH;
using EXPIMP;
using System.Collections.Generic;

namespace PFD
{
    /// <summary>
    /// Interaction logic for Pokus2DView.xaml
    /// </summary>
    public partial class Pokus2DView : Window
    {
        public Canvas CanvasSection2D = null;
        public CModel_PFD Model = null;

        double modelMarginLeft_x;
        double modelMarginBottom_y;
        double fReal_Model_Zoom_Factor;

        //double fModel_Length_x_page;
        //double fModel_Length_y_page;

        double dModelDimension_Y_real;
        double dModelDimension_Z_real;

        double dPageWidth;
        double dPageHeight;

        float fTempMax_X;
        float fTempMin_X;
        float fTempMax_Y;
        float fTempMin_Y;
        float fTempMax_Z;
        float fTempMin_Z;

        public Pokus2DView()
        {
            InitializeComponent();
            
            canvasForImage.Children.Clear();
            CanvasSection2D = canvasForImage;
        }

        public Pokus2DView(CModel_PFD model)
        {
            InitializeComponent();
            Model = model;

            comboViews.SelectedIndex = 3;
        }

        public void DrawRightView_ModelToCanvas(CModel_PFD model)
        {
            canvasForImage.Children.Clear();

            dPageWidth = this.Width;
            dPageHeight = this.Height;

            if (model != null)
            {
                Drawing3D.CalculateModelLimitsWithoutCrsc(model, new DisplayOptions(), out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);
                //CalculateModelLimits(model, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);
                dModelDimension_Y_real = fTempMax_Y - fTempMin_Y;
                dModelDimension_Z_real = fTempMax_Z - fTempMin_Z;
            }

            CalculateBasicValue();

            // Set 3D environment data to generate 2D view
            // Point of View / Camera

            // View perpendicular to the global plane YZ (in "-X" direction)
            Point3D pCameraPosition = new Point3D(fTempMax_X + 1, 0, 0);
            Vector3D pCameraViewDirection = new Vector3D(-1, 0, 0);
            float fViewDepth = 2; // [m]

            float fMinCoord_X = (float)(pCameraPosition.X + pCameraViewDirection.X * fViewDepth);
            float fMaxCoord_X = (float)pCameraPosition.X;

            for (int i = 0; i < model.m_arrMembers.Length; i++)
            {
                if (model.m_arrMembers[i] == null) continue;

                // Transform Units from 3D real model to 2D view (depends on size of window)
                Point pA = new Point(model.m_arrMembers[i].PointStart.Y * fReal_Model_Zoom_Factor, model.m_arrMembers[i].PointStart.Z * fReal_Model_Zoom_Factor);
                Point pB = new Point(model.m_arrMembers[i].PointEnd.Y * fReal_Model_Zoom_Factor, model.m_arrMembers[i].PointEnd.Z * fReal_Model_Zoom_Factor);
                double b = model.m_arrMembers[i].CrScStart.b * fReal_Model_Zoom_Factor; // Todo - to ci sa ma vykreslovat sirka alebo vyska prierezu zavisi od uhla theta smeru lokalnej osy z prierezu voci smeru pohladu
                double h = model.m_arrMembers[i].CrScStart.h * fReal_Model_Zoom_Factor;

                double width = b;
                //if (model.m_arrMembers[i].DTheta_x == 0 || model.m_arrMembers[i].DTheta_x == MathF.dPI)
                //    width = h;

                if ((fMinCoord_X < model.m_arrMembers[i].PointStart.X && model.m_arrMembers[i].PointStart.X < fMaxCoord_X) &&
               (fMinCoord_X < model.m_arrMembers[i].PointEnd.X && model.m_arrMembers[i].PointEnd.X < fMaxCoord_X))
                {
                    // Both definition points of the member are within interval - draw rectangle (L - length in the plane YZ)
                    double fRotationAboutX_rad = Geom2D.GetAlpha2D_CW((float)pA.X, (float)pB.X, (float)pA.Y, (float)pB.Y); // ToDo - dopocitat podla suradnic koncovych bodov v rovine pohladu YZ
                    double dLengthProjected = Math.Sqrt(MathF.Pow2(pB.X - pA.X) + MathF.Pow2(pB.Y - pA.Y));

                    DrawMember2D(Brushes.Black, Brushes.Azure, pA, width, dLengthProjected, fRotationAboutX_rad, canvasForImage);
                }
                else if (((fMinCoord_X < model.m_arrMembers[i].PointStart.X && model.m_arrMembers[i].PointStart.X < fMaxCoord_X) &&
                   (fMinCoord_X > model.m_arrMembers[i].PointEnd.X || model.m_arrMembers[i].PointEnd.X > fMaxCoord_X)) ||
                   ((fMinCoord_X < model.m_arrMembers[i].PointEnd.X && model.m_arrMembers[i].PointEnd.X < fMaxCoord_X) &&
                   (fMinCoord_X > model.m_arrMembers[i].PointStart.X || model.m_arrMembers[i].PointStart.X > fMaxCoord_X)))
                {
                    // Only one point is within interval for view depth - draw cross-section
                    // We should check that other point is perpendicular to the plane YZ otherwise modified cross-section shape (cut of he member) should be displayed

                    // Set centroid coordinates
                    Point p = new Point();
                    if (fMinCoord_X < model.m_arrMembers[i].PointStart.X && model.m_arrMembers[i].PointStart.X < fMaxCoord_X)
                    {
                        p.X = model.m_arrMembers[i].PointStart.Y * fReal_Model_Zoom_Factor;
                        p.Y = model.m_arrMembers[i].PointStart.Z * fReal_Model_Zoom_Factor;
                    }
                    else
                    {
                        p.X = model.m_arrMembers[i].PointEnd.Y * fReal_Model_Zoom_Factor;
                        p.Y = model.m_arrMembers[i].PointEnd.Z * fReal_Model_Zoom_Factor;
                    }

                    // Transform cross-section coordinates in 2D
                    List<Point> crsccoordoutline = null;
                    List<Point> crsccoordinline = null;

                    if (model.m_arrMembers[i].CrScStart.CrScPointsOut != null)
                    {
                        //crsccoordoutline = new float[model.m_arrMembers[i].CrScStart.INoPointsOut, 2];
                        //Array.Copy(model.m_arrMembers[i].CrScStart.CrScPointsOut, crsccoordoutline, model.m_arrMembers[i].CrScStart.CrScPointsOut.Count);
                        crsccoordoutline = new List<Point>(model.m_arrMembers[i].CrScStart.CrScPointsOut);


                        // Transfom coordinates to geometry center
                        crsccoordoutline = model.m_arrMembers[i].CrScStart.GetCoordinatesInGeometryRelatedToGeometryCenterPoint(crsccoordoutline);

                        // TODO - kedze prut nemusi byt kolmy na smer pohladu, tak spravne by sa mal detekovat uhol, pod ktorym sa na prierez pozerame
                        // a mali by sa prepocitat lokalne suradnice prierezu
                        // Napr. stresny nosnik "rafter" je voci rovine pohladu sikmo v uhle roofpitch, takze prierez by sa mal zvacsit v smere zvislej lokalnej osy, zobrazena vyska prierezu je c = h / cos(roofpitch)
                        // takto by sa mali prepocitat vsetky suradnice prierezu v smere lokalnej zvislej osi [i,1]

                        for (int j = 0; j < model.m_arrMembers[i].CrScStart.INoPointsOut; j++)
                        {
                            double fx = Geom2D.GetRotatedPosition_x_CCW_rad(crsccoordoutline[j].X, crsccoordoutline[j].Y, model.m_arrMembers[i].DTheta_x);
                            double fy = Geom2D.GetRotatedPosition_y_CCW_rad(crsccoordoutline[j].X, crsccoordoutline[j].Y, model.m_arrMembers[i].DTheta_x);

                            Point point = crsccoordoutline[j];
                            point.X = (fx * fReal_Model_Zoom_Factor);
                            point.Y = (fy * fReal_Model_Zoom_Factor);

                            point.X += p.X;
                            point.Y += p.Y;
                        }
                    }

                    if (model.m_arrMembers[i].CrScStart.CrScPointsIn != null)
                    {
                        //crsccoordinline = new float[model.m_arrMembers[i].CrScStart.INoPointsIn, 2];
                        //Array.Copy(model.m_arrMembers[i].CrScStart.CrScPointsIn, crsccoordinline, model.m_arrMembers[i].CrScStart.CrScPointsIn.Count);

                        crsccoordinline = new List<Point>(model.m_arrMembers[i].CrScStart.CrScPointsIn);

                        // Transfom coordinates to geometry center
                        crsccoordinline = model.m_arrMembers[i].CrScStart.GetCoordinatesInGeometryRelatedToGeometryCenterPoint(crsccoordinline);

                        for (int j = 0; j < model.m_arrMembers[i].CrScStart.INoPointsIn; j++)
                        {
                            double fx = Geom2D.GetRotatedPosition_x_CCW_rad(crsccoordinline[j].X, crsccoordinline[j].Y, model.m_arrMembers[i].DTheta_x);
                            double fy = Geom2D.GetRotatedPosition_y_CCW_rad(crsccoordinline[j].X, crsccoordinline[j].Y, model.m_arrMembers[i].DTheta_x);

                            Point point = crsccoordinline[j];
                            point.X = (fx * fReal_Model_Zoom_Factor);
                            point.Y = (fy * fReal_Model_Zoom_Factor);

                            point.X += p.X;
                            point.Y += p.Y;
                        }
                    }

                    // Draw cross-section
                    DrawCrossSection(p, b, h, model.m_arrMembers[i].DTheta_x, crsccoordoutline, crsccoordinline);
                }
                else
                {
                    // Member is outside the box (do not draw)
                }
            }

            CanvasSection2D = canvasForImage;

        }

        public void DrawLeftView_ModelToCanvas(CModel_PFD model)
        {
            canvasForImage.Children.Clear(); return;

            dPageWidth = this.Width;
            dPageHeight = this.Height;

            if (model != null)
            {
                Drawing3D.CalculateModelLimitsWithoutCrsc(model, new DisplayOptions(), out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);
                //CalculateModelLimits(model, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);
                dModelDimension_Y_real = fTempMax_Y - fTempMin_Y;
                dModelDimension_Z_real = fTempMax_Z - fTempMin_Z;
            }

            CalculateBasicValue();

            // Set 3D environment data to generate 2D view
            // Point of View / Camera

            // View perpendicular to the global plane YZ (in "-X" direction)
            Point3D pCameraPosition = new Point3D(fTempMax_X + 1, 0, 0);
            Vector3D pCameraViewDirection = new Vector3D(-1, 0, 0);
            float fViewDepth = 2; // [m]

            float fMinCoord_X = (float)(pCameraPosition.X + pCameraViewDirection.X * fViewDepth);
            float fMaxCoord_X = (float)pCameraPosition.X;

            for (int i = 0; i < model.m_arrMembers.Length; i++)
            {
                if (model.m_arrMembers[i] == null) continue;

                // Transform Units from 3D real model to 2D view (depends on size of window)
                Point pA = new Point(model.m_arrMembers[i].PointStart.Y * fReal_Model_Zoom_Factor, model.m_arrMembers[i].PointStart.Z * fReal_Model_Zoom_Factor);
                Point pB = new Point(model.m_arrMembers[i].PointEnd.Y * fReal_Model_Zoom_Factor, model.m_arrMembers[i].PointEnd.Z * fReal_Model_Zoom_Factor);
                double b = model.m_arrMembers[i].CrScStart.b * fReal_Model_Zoom_Factor; // Todo - to ci sa ma vykreslovat sirka alebo vyska prierezu zavisi od uhla theta smeru lokalnej osy z prierezu voci smeru pohladu
                double h = model.m_arrMembers[i].CrScStart.h * fReal_Model_Zoom_Factor;

                double width = b;
                //if (model.m_arrMembers[i].DTheta_x == 0 || model.m_arrMembers[i].DTheta_x == MathF.dPI)
                //    width = h;

                if ((fMinCoord_X < model.m_arrMembers[i].PointStart.X && model.m_arrMembers[i].PointStart.X < fMaxCoord_X) &&
               (fMinCoord_X < model.m_arrMembers[i].PointEnd.X && model.m_arrMembers[i].PointEnd.X < fMaxCoord_X))
                {
                    // Both definition points of the member are within interval - draw rectangle (L - length in the plane YZ)
                    double fRotationAboutX_rad = Geom2D.GetAlpha2D_CW((float)pA.X, (float)pB.X, (float)pA.Y, (float)pB.Y); // ToDo - dopocitat podla suradnic koncovych bodov v rovine pohladu YZ
                    double dLengthProjected = Math.Sqrt(MathF.Pow2(pB.X - pA.X) + MathF.Pow2(pB.Y - pA.Y));

                    DrawMember2D(Brushes.Black, Brushes.Azure, pA, width, dLengthProjected, fRotationAboutX_rad, canvasForImage);
                }
                else if (((fMinCoord_X < model.m_arrMembers[i].PointStart.X && model.m_arrMembers[i].PointStart.X < fMaxCoord_X) &&
                   (fMinCoord_X > model.m_arrMembers[i].PointEnd.X || model.m_arrMembers[i].PointEnd.X > fMaxCoord_X)) ||
                   ((fMinCoord_X < model.m_arrMembers[i].PointEnd.X && model.m_arrMembers[i].PointEnd.X < fMaxCoord_X) &&
                   (fMinCoord_X > model.m_arrMembers[i].PointStart.X || model.m_arrMembers[i].PointStart.X > fMaxCoord_X)))
                {
                    // Only one point is within interval for view depth - draw cross-section
                    // We should check that other point is perpendicular to the plane YZ otherwise modified cross-section shape (cut of he member) should be displayed

                    // Set centroid coordinates
                    Point p = new Point();
                    if (fMinCoord_X < model.m_arrMembers[i].PointStart.X && model.m_arrMembers[i].PointStart.X < fMaxCoord_X)
                    {
                        p.X = model.m_arrMembers[i].PointStart.Y * fReal_Model_Zoom_Factor;
                        p.Y = model.m_arrMembers[i].PointStart.Z * fReal_Model_Zoom_Factor;
                    }
                    else
                    {
                        p.X = model.m_arrMembers[i].PointEnd.Y * fReal_Model_Zoom_Factor;
                        p.Y = model.m_arrMembers[i].PointEnd.Z * fReal_Model_Zoom_Factor;
                    }

                    // Transform cross-section coordinates in 2D
                    List<Point> crsccoordoutline = null;
                    List<Point> crsccoordinline = null;

                    if (model.m_arrMembers[i].CrScStart.CrScPointsOut != null)
                    {
                        //crsccoordoutline = new float[model.m_arrMembers[i].CrScStart.INoPointsOut, 2];
                        //Array.Copy(model.m_arrMembers[i].CrScStart.CrScPointsOut, crsccoordoutline, model.m_arrMembers[i].CrScStart.CrScPointsOut.Count);
                        crsccoordoutline = new List<Point>(model.m_arrMembers[i].CrScStart.CrScPointsOut);

                        // Transfom coordinates to geometry center
                        crsccoordoutline = model.m_arrMembers[i].CrScStart.GetCoordinatesInGeometryRelatedToGeometryCenterPoint(crsccoordoutline);

                        // TODO - kedze prut nemusi byt kolmy na smer pohladu, tak spravne by sa mal detekovat uhol, pod ktorym sa na prierez pozerame
                        // a mali by sa prepocitat lokalne suradnice prierezu
                        // Napr. stresny nosnik "rafter" je voci rovine pohladu sikmo v uhle roofpitch, takze prierez by sa mal zvacsit v smere zvislej lokalnej osy, zobrazena vyska prierezu je c = h / cos(roofpitch)
                        // takto by sa mali prepocitat vsetky suradnice prierezu v smere lokalnej zvislej osi [i,1]

                        for (int j = 0; j < model.m_arrMembers[i].CrScStart.INoPointsOut; j++)
                        {
                            double fx = Geom2D.GetRotatedPosition_x_CCW_rad(crsccoordoutline[j].X, crsccoordoutline[j].Y, model.m_arrMembers[i].DTheta_x);
                            double fy = Geom2D.GetRotatedPosition_y_CCW_rad(crsccoordoutline[j].X, crsccoordoutline[j].Y, model.m_arrMembers[i].DTheta_x);

                            Point point = crsccoordoutline[j];
                            point.X = (fx * fReal_Model_Zoom_Factor);
                            point.Y = (fy * fReal_Model_Zoom_Factor);

                            point.X += p.X;
                            point.Y += p.Y;
                        }
                    }

                    if (model.m_arrMembers[i].CrScStart.CrScPointsIn != null)
                    {
                        //crsccoordinline = new float[model.m_arrMembers[i].CrScStart.INoPointsIn, 2];
                        //Array.Copy(model.m_arrMembers[i].CrScStart.CrScPointsIn, crsccoordinline, model.m_arrMembers[i].CrScStart.CrScPointsIn.Count);

                        crsccoordinline = new List<Point>(model.m_arrMembers[i].CrScStart.CrScPointsIn);

                        // Transfom coordinates to geometry center
                        crsccoordinline = model.m_arrMembers[i].CrScStart.GetCoordinatesInGeometryRelatedToGeometryCenterPoint(crsccoordinline);

                        for (int j = 0; j < model.m_arrMembers[i].CrScStart.INoPointsIn; j++)
                        {
                            double fx = Geom2D.GetRotatedPosition_x_CCW_rad(crsccoordinline[j].X, crsccoordinline[j].Y, model.m_arrMembers[i].DTheta_x);
                            double fy = Geom2D.GetRotatedPosition_y_CCW_rad(crsccoordinline[j].X, crsccoordinline[j].Y, model.m_arrMembers[i].DTheta_x);

                            Point point = crsccoordinline[j];
                            point.X = (fx * fReal_Model_Zoom_Factor);
                            point.Y = (fy * fReal_Model_Zoom_Factor);

                            point.X += p.X;
                            point.Y += p.Y;
                        }
                    }

                    // Draw cross-section
                    DrawCrossSection(p, b, h, model.m_arrMembers[i].DTheta_x, crsccoordoutline, crsccoordinline);
                }
                else
                {
                    // Member is outside the box (do not draw)
                }
            }

            CanvasSection2D = canvasForImage;

        }

        public void DrawFrontView_ModelToCanvas(CModel_PFD model)
        {
            canvasForImage.Children.Clear(); return;

            dPageWidth = this.Width;
            dPageHeight = this.Height;

            if (model != null)
            {
                Drawing3D.CalculateModelLimitsWithoutCrsc(model, new DisplayOptions(), out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);
                //CalculateModelLimits(model, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);
                dModelDimension_Y_real = fTempMax_Y - fTempMin_Y;
                dModelDimension_Z_real = fTempMax_Z - fTempMin_Z;
            }

            CalculateBasicValue();

            // Set 3D environment data to generate 2D view
            // Point of View / Camera

            // View perpendicular to the global plane YZ (in "-X" direction)
            Point3D pCameraPosition = new Point3D(fTempMax_X + 1, 0, 0);
            Vector3D pCameraViewDirection = new Vector3D(-1, 0, 0);
            float fViewDepth = 2; // [m]

            float fMinCoord_X = (float)(pCameraPosition.X + pCameraViewDirection.X * fViewDepth);
            float fMaxCoord_X = (float)pCameraPosition.X;

            for (int i = 0; i < model.m_arrMembers.Length; i++)
            {
                if (model.m_arrMembers[i] == null) continue;

                // Transform Units from 3D real model to 2D view (depends on size of window)
                Point pA = new Point(model.m_arrMembers[i].PointStart.Y * fReal_Model_Zoom_Factor, model.m_arrMembers[i].PointStart.Z * fReal_Model_Zoom_Factor);
                Point pB = new Point(model.m_arrMembers[i].PointEnd.Y * fReal_Model_Zoom_Factor, model.m_arrMembers[i].PointEnd.Z * fReal_Model_Zoom_Factor);
                double b = model.m_arrMembers[i].CrScStart.b * fReal_Model_Zoom_Factor; // Todo - to ci sa ma vykreslovat sirka alebo vyska prierezu zavisi od uhla theta smeru lokalnej osy z prierezu voci smeru pohladu
                double h = model.m_arrMembers[i].CrScStart.h * fReal_Model_Zoom_Factor;

                double width = b;
                //if (model.m_arrMembers[i].DTheta_x == 0 || model.m_arrMembers[i].DTheta_x == MathF.dPI)
                //    width = h;

                if ((fMinCoord_X < model.m_arrMembers[i].PointStart.X && model.m_arrMembers[i].PointStart.X < fMaxCoord_X) &&
               (fMinCoord_X < model.m_arrMembers[i].PointEnd.X && model.m_arrMembers[i].PointEnd.X < fMaxCoord_X))
                {
                    // Both definition points of the member are within interval - draw rectangle (L - length in the plane YZ)
                    double fRotationAboutX_rad = Geom2D.GetAlpha2D_CW((float)pA.X, (float)pB.X, (float)pA.Y, (float)pB.Y); // ToDo - dopocitat podla suradnic koncovych bodov v rovine pohladu YZ
                    double dLengthProjected = Math.Sqrt(MathF.Pow2(pB.X - pA.X) + MathF.Pow2(pB.Y - pA.Y));

                    DrawMember2D(Brushes.Black, Brushes.Azure, pA, width, dLengthProjected, fRotationAboutX_rad, canvasForImage);
                }
                else if (((fMinCoord_X < model.m_arrMembers[i].PointStart.X && model.m_arrMembers[i].PointStart.X < fMaxCoord_X) &&
                   (fMinCoord_X > model.m_arrMembers[i].PointEnd.X || model.m_arrMembers[i].PointEnd.X > fMaxCoord_X)) ||
                   ((fMinCoord_X < model.m_arrMembers[i].PointEnd.X && model.m_arrMembers[i].PointEnd.X < fMaxCoord_X) &&
                   (fMinCoord_X > model.m_arrMembers[i].PointStart.X || model.m_arrMembers[i].PointStart.X > fMaxCoord_X)))
                {
                    // Only one point is within interval for view depth - draw cross-section
                    // We should check that other point is perpendicular to the plane YZ otherwise modified cross-section shape (cut of he member) should be displayed

                    // Set centroid coordinates
                    Point p = new Point();
                    if (fMinCoord_X < model.m_arrMembers[i].PointStart.X && model.m_arrMembers[i].PointStart.X < fMaxCoord_X)
                    {
                        p.X = model.m_arrMembers[i].PointStart.Y * fReal_Model_Zoom_Factor;
                        p.Y = model.m_arrMembers[i].PointStart.Z * fReal_Model_Zoom_Factor;
                    }
                    else
                    {
                        p.X = model.m_arrMembers[i].PointEnd.Y * fReal_Model_Zoom_Factor;
                        p.Y = model.m_arrMembers[i].PointEnd.Z * fReal_Model_Zoom_Factor;
                    }

                    // Transform cross-section coordinates in 2D
                    List<Point> crsccoordoutline = null;
                    List<Point> crsccoordinline = null;

                    if (model.m_arrMembers[i].CrScStart.CrScPointsOut != null)
                    {
                        //crsccoordoutline = new float[model.m_arrMembers[i].CrScStart.INoPointsOut, 2];
                        //Array.Copy(model.m_arrMembers[i].CrScStart.CrScPointsOut, crsccoordoutline, model.m_arrMembers[i].CrScStart.CrScPointsOut.Count);

                        crsccoordoutline = new List<Point>(model.m_arrMembers[i].CrScStart.CrScPointsOut);

                        // Transfom coordinates to geometry center
                        crsccoordoutline = model.m_arrMembers[i].CrScStart.GetCoordinatesInGeometryRelatedToGeometryCenterPoint(crsccoordoutline);

                        // TODO - kedze prut nemusi byt kolmy na smer pohladu, tak spravne by sa mal detekovat uhol, pod ktorym sa na prierez pozerame
                        // a mali by sa prepocitat lokalne suradnice prierezu
                        // Napr. stresny nosnik "rafter" je voci rovine pohladu sikmo v uhle roofpitch, takze prierez by sa mal zvacsit v smere zvislej lokalnej osy, zobrazena vyska prierezu je c = h / cos(roofpitch)
                        // takto by sa mali prepocitat vsetky suradnice prierezu v smere lokalnej zvislej osi [i,1]

                        for (int j = 0; j < model.m_arrMembers[i].CrScStart.INoPointsOut; j++)
                        {
                            double fx = Geom2D.GetRotatedPosition_x_CCW_rad(crsccoordoutline[j].X, crsccoordoutline[j].Y, model.m_arrMembers[i].DTheta_x);
                            double fy = Geom2D.GetRotatedPosition_y_CCW_rad(crsccoordoutline[j].X, crsccoordoutline[j].Y, model.m_arrMembers[i].DTheta_x);

                            Point point = crsccoordoutline[j];
                            point.X = (fx * fReal_Model_Zoom_Factor);
                            point.Y = (fy * fReal_Model_Zoom_Factor);

                            point.X += p.X;
                            point.Y += p.Y;
                        }
                    }

                    if (model.m_arrMembers[i].CrScStart.CrScPointsIn != null)
                    {
                        //crsccoordinline = new float[model.m_arrMembers[i].CrScStart.INoPointsIn, 2];                        
                        //Array.Copy(model.m_arrMembers[i].CrScStart.CrScPointsIn, crsccoordinline, model.m_arrMembers[i].CrScStart.CrScPointsIn.Count);

                        crsccoordinline = new List<Point>(model.m_arrMembers[i].CrScStart.CrScPointsIn);

                        // Transfom coordinates to geometry center
                        crsccoordinline = model.m_arrMembers[i].CrScStart.GetCoordinatesInGeometryRelatedToGeometryCenterPoint(crsccoordinline);

                        for (int j = 0; j < model.m_arrMembers[i].CrScStart.INoPointsIn; j++)
                        {
                            double fx = Geom2D.GetRotatedPosition_x_CCW_rad(crsccoordinline[j].X, crsccoordinline[j].Y, model.m_arrMembers[i].DTheta_x);
                            double fy = Geom2D.GetRotatedPosition_y_CCW_rad(crsccoordinline[j].X, crsccoordinline[j].Y, model.m_arrMembers[i].DTheta_x);

                            Point point = crsccoordinline[j];
                            point.X = (fx * fReal_Model_Zoom_Factor);
                            point.Y = (fy * fReal_Model_Zoom_Factor);

                            point.X += p.X;
                            point.Y += p.Y;
                        }
                    }

                    // Draw cross-section
                    DrawCrossSection(p, b, h, model.m_arrMembers[i].DTheta_x, crsccoordoutline, crsccoordinline);
                }
                else
                {
                    // Member is outside the box (do not draw)
                }
            }

            CanvasSection2D = canvasForImage;
        }

        public void DrawBackView_ModelToCanvas(CModel_PFD model)
        {
            canvasForImage.Children.Clear(); return;

            dPageWidth = this.Width;
            dPageHeight = this.Height;

            if (model != null)
            {
                Drawing3D.CalculateModelLimitsWithoutCrsc(model, new DisplayOptions(), out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);
                //CalculateModelLimits(model, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);
                dModelDimension_Y_real = fTempMax_Y - fTempMin_Y;
                dModelDimension_Z_real = fTempMax_Z - fTempMin_Z;
            }

            CalculateBasicValue();

            // Set 3D environment data to generate 2D view
            // Point of View / Camera

            // View perpendicular to the global plane YZ (in "-X" direction)
            Point3D pCameraPosition = new Point3D(fTempMax_X + 1, 0, 0);
            Vector3D pCameraViewDirection = new Vector3D(-1, 0, 0);
            float fViewDepth = 2; // [m]

            float fMinCoord_X = (float)(pCameraPosition.X + pCameraViewDirection.X * fViewDepth);
            float fMaxCoord_X = (float)pCameraPosition.X;

            for (int i = 0; i < model.m_arrMembers.Length; i++)
            {
                if (model.m_arrMembers[i] == null) continue;

                // Transform Units from 3D real model to 2D view (depends on size of window)
                Point pA = new Point(model.m_arrMembers[i].PointStart.Y * fReal_Model_Zoom_Factor, model.m_arrMembers[i].PointStart.Z * fReal_Model_Zoom_Factor);
                Point pB = new Point(model.m_arrMembers[i].PointEnd.Y * fReal_Model_Zoom_Factor, model.m_arrMembers[i].PointEnd.Z * fReal_Model_Zoom_Factor);
                double b = model.m_arrMembers[i].CrScStart.b * fReal_Model_Zoom_Factor; // Todo - to ci sa ma vykreslovat sirka alebo vyska prierezu zavisi od uhla theta smeru lokalnej osy z prierezu voci smeru pohladu
                double h = model.m_arrMembers[i].CrScStart.h * fReal_Model_Zoom_Factor;

                double width = b;
                //if (model.m_arrMembers[i].DTheta_x == 0 || model.m_arrMembers[i].DTheta_x == MathF.dPI)
                //    width = h;

                if ((fMinCoord_X < model.m_arrMembers[i].PointStart.X && model.m_arrMembers[i].PointStart.X < fMaxCoord_X) &&
               (fMinCoord_X < model.m_arrMembers[i].PointEnd.X && model.m_arrMembers[i].PointEnd.X < fMaxCoord_X))
                {
                    // Both definition points of the member are within interval - draw rectangle (L - length in the plane YZ)
                    double fRotationAboutX_rad = Geom2D.GetAlpha2D_CW((float)pA.X, (float)pB.X, (float)pA.Y, (float)pB.Y); // ToDo - dopocitat podla suradnic koncovych bodov v rovine pohladu YZ
                    double dLengthProjected = Math.Sqrt(MathF.Pow2(pB.X - pA.X) + MathF.Pow2(pB.Y - pA.Y));

                    DrawMember2D(Brushes.Black, Brushes.Azure, pA, width, dLengthProjected, fRotationAboutX_rad, canvasForImage);
                }
                else if (((fMinCoord_X < model.m_arrMembers[i].PointStart.X && model.m_arrMembers[i].PointStart.X < fMaxCoord_X) &&
                   (fMinCoord_X > model.m_arrMembers[i].PointEnd.X || model.m_arrMembers[i].PointEnd.X > fMaxCoord_X)) ||
                   ((fMinCoord_X < model.m_arrMembers[i].PointEnd.X && model.m_arrMembers[i].PointEnd.X < fMaxCoord_X) &&
                   (fMinCoord_X > model.m_arrMembers[i].PointStart.X || model.m_arrMembers[i].PointStart.X > fMaxCoord_X)))
                {
                    // Only one point is within interval for view depth - draw cross-section
                    // We should check that other point is perpendicular to the plane YZ otherwise modified cross-section shape (cut of he member) should be displayed

                    // Set centroid coordinates
                    Point p = new Point();
                    if (fMinCoord_X < model.m_arrMembers[i].PointStart.X && model.m_arrMembers[i].PointStart.X < fMaxCoord_X)
                    {
                        p.X = model.m_arrMembers[i].PointStart.Y * fReal_Model_Zoom_Factor;
                        p.Y = model.m_arrMembers[i].PointStart.Z * fReal_Model_Zoom_Factor;
                    }
                    else
                    {
                        p.X = model.m_arrMembers[i].PointEnd.Y * fReal_Model_Zoom_Factor;
                        p.Y = model.m_arrMembers[i].PointEnd.Z * fReal_Model_Zoom_Factor;
                    }

                    // Transform cross-section coordinates in 2D
                    List<Point> crsccoordoutline = null;
                    List<Point> crsccoordinline = null;

                    if (model.m_arrMembers[i].CrScStart.CrScPointsOut != null)
                    {
                        //crsccoordoutline = new float[model.m_arrMembers[i].CrScStart.INoPointsOut, 2];
                        //Array.Copy(model.m_arrMembers[i].CrScStart.CrScPointsOut, crsccoordoutline, model.m_arrMembers[i].CrScStart.CrScPointsOut.Count);

                        crsccoordoutline = new List<Point>(model.m_arrMembers[i].CrScStart.CrScPointsOut);

                        // Transfom coordinates to geometry center
                        crsccoordoutline = model.m_arrMembers[i].CrScStart.GetCoordinatesInGeometryRelatedToGeometryCenterPoint(crsccoordoutline);

                        // TODO - kedze prut nemusi byt kolmy na smer pohladu, tak spravne by sa mal detekovat uhol, pod ktorym sa na prierez pozerame
                        // a mali by sa prepocitat lokalne suradnice prierezu
                        // Napr. stresny nosnik "rafter" je voci rovine pohladu sikmo v uhle roofpitch, takze prierez by sa mal zvacsit v smere zvislej lokalnej osy, zobrazena vyska prierezu je c = h / cos(roofpitch)
                        // takto by sa mali prepocitat vsetky suradnice prierezu v smere lokalnej zvislej osi [i,1]

                        for (int j = 0; j < model.m_arrMembers[i].CrScStart.INoPointsOut; j++)
                        {
                            double fx = Geom2D.GetRotatedPosition_x_CCW_rad(crsccoordoutline[j].X, crsccoordoutline[j].Y, model.m_arrMembers[i].DTheta_x);
                            double fy = Geom2D.GetRotatedPosition_y_CCW_rad(crsccoordoutline[j].X, crsccoordoutline[j].Y, model.m_arrMembers[i].DTheta_x);

                            Point point= crsccoordoutline[j];
                            point.X = (fx * fReal_Model_Zoom_Factor);
                            point.Y = (fy * fReal_Model_Zoom_Factor);

                            point.X += p.X;
                            point.Y += p.Y;
                        }
                    }

                    if (model.m_arrMembers[i].CrScStart.CrScPointsIn != null)
                    {
                        //crsccoordinline = new float[model.m_arrMembers[i].CrScStart.INoPointsIn, 2];
                        //Array.Copy(model.m_arrMembers[i].CrScStart.CrScPointsIn, crsccoordinline, model.m_arrMembers[i].CrScStart.CrScPointsIn.Count);

                        crsccoordinline = new List<Point>(model.m_arrMembers[i].CrScStart.CrScPointsIn);

                        // Transfom coordinates to geometry center
                        crsccoordinline = model.m_arrMembers[i].CrScStart.GetCoordinatesInGeometryRelatedToGeometryCenterPoint(crsccoordinline);

                        for (int j = 0; j < model.m_arrMembers[i].CrScStart.INoPointsIn; j++)
                        {
                            double fx = Geom2D.GetRotatedPosition_x_CCW_rad(crsccoordinline[j].X, crsccoordinline[j].Y, model.m_arrMembers[i].DTheta_x);
                            double fy = Geom2D.GetRotatedPosition_y_CCW_rad(crsccoordinline[j].X, crsccoordinline[j].Y, model.m_arrMembers[i].DTheta_x);

                            Point point = crsccoordinline[j];
                            point.X = (fx * fReal_Model_Zoom_Factor);
                            point.Y = (fy * fReal_Model_Zoom_Factor);

                            point.X += p.X;
                            point.Y += p.Y;
                        }
                    }

                    // Draw cross-section
                    DrawCrossSection(p, b, h, model.m_arrMembers[i].DTheta_x, crsccoordoutline, crsccoordinline);
                }
                else
                {
                    // Member is outside the box (do not draw)
                }
            }

            CanvasSection2D = canvasForImage;

        }

        public void DrawTopView_ModelToCanvas(CModel_PFD model)
        {
            canvasForImage.Children.Clear(); return;

            dPageWidth = this.Width;
            dPageHeight = this.Height;

            if (model != null)
            {
                Drawing3D.CalculateModelLimitsWithoutCrsc(model, new DisplayOptions(), out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);
                //CalculateModelLimits(model, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);
                dModelDimension_Y_real = fTempMax_Y - fTempMin_Y;
                dModelDimension_Z_real = fTempMax_Z - fTempMin_Z;
            }

            CalculateBasicValue();

            // Set 3D environment data to generate 2D view
            // Point of View / Camera

            // View perpendicular to the global plane YZ (in "-X" direction)
            Point3D pCameraPosition = new Point3D(fTempMax_X + 1, 0, 0);
            Vector3D pCameraViewDirection = new Vector3D(-1, 0, 0);
            float fViewDepth = 2; // [m]

            float fMinCoord_X = (float)(pCameraPosition.X + pCameraViewDirection.X * fViewDepth);
            float fMaxCoord_X = (float)pCameraPosition.X;

            for (int i = 0; i < model.m_arrMembers.Length; i++)
            {
                if (model.m_arrMembers[i] == null) continue;

                // Transform Units from 3D real model to 2D view (depends on size of window)
                Point pA = new Point(model.m_arrMembers[i].PointStart.Y * fReal_Model_Zoom_Factor, model.m_arrMembers[i].PointStart.Z * fReal_Model_Zoom_Factor);
                Point pB = new Point(model.m_arrMembers[i].PointEnd.Y * fReal_Model_Zoom_Factor, model.m_arrMembers[i].PointEnd.Z * fReal_Model_Zoom_Factor);
                double b = model.m_arrMembers[i].CrScStart.b * fReal_Model_Zoom_Factor; // Todo - to ci sa ma vykreslovat sirka alebo vyska prierezu zavisi od uhla theta smeru lokalnej osy z prierezu voci smeru pohladu
                double h = model.m_arrMembers[i].CrScStart.h * fReal_Model_Zoom_Factor;

                double width = b;
                //if (model.m_arrMembers[i].DTheta_x == 0 || model.m_arrMembers[i].DTheta_x == MathF.dPI)
                //    width = h;

                if ((fMinCoord_X < model.m_arrMembers[i].PointStart.X && model.m_arrMembers[i].PointStart.X < fMaxCoord_X) &&
               (fMinCoord_X < model.m_arrMembers[i].PointEnd.X && model.m_arrMembers[i].PointEnd.X < fMaxCoord_X))
                {
                    // Both definition points of the member are within interval - draw rectangle (L - length in the plane YZ)
                    double fRotationAboutX_rad = Geom2D.GetAlpha2D_CW((float)pA.X, (float)pB.X, (float)pA.Y, (float)pB.Y); // ToDo - dopocitat podla suradnic koncovych bodov v rovine pohladu YZ
                    double dLengthProjected = Math.Sqrt(MathF.Pow2(pB.X - pA.X) + MathF.Pow2(pB.Y - pA.Y));

                    DrawMember2D(Brushes.Black, Brushes.Azure, pA, width, dLengthProjected, fRotationAboutX_rad, canvasForImage);
                }
                else if (((fMinCoord_X < model.m_arrMembers[i].PointStart.X && model.m_arrMembers[i].PointStart.X < fMaxCoord_X) &&
                   (fMinCoord_X > model.m_arrMembers[i].PointEnd.X || model.m_arrMembers[i].PointEnd.X > fMaxCoord_X)) ||
                   ((fMinCoord_X < model.m_arrMembers[i].PointEnd.X && model.m_arrMembers[i].PointEnd.X < fMaxCoord_X) &&
                   (fMinCoord_X > model.m_arrMembers[i].PointStart.X || model.m_arrMembers[i].PointStart.X > fMaxCoord_X)))
                {
                    // Only one point is within interval for view depth - draw cross-section
                    // We should check that other point is perpendicular to the plane YZ otherwise modified cross-section shape (cut of he member) should be displayed

                    // Set centroid coordinates
                    Point p = new Point();
                    if (fMinCoord_X < model.m_arrMembers[i].PointStart.X && model.m_arrMembers[i].PointStart.X < fMaxCoord_X)
                    {
                        p.X = model.m_arrMembers[i].PointStart.Y * fReal_Model_Zoom_Factor;
                        p.Y = model.m_arrMembers[i].PointStart.Z * fReal_Model_Zoom_Factor;
                    }
                    else
                    {
                        p.X = model.m_arrMembers[i].PointEnd.Y * fReal_Model_Zoom_Factor;
                        p.Y = model.m_arrMembers[i].PointEnd.Z * fReal_Model_Zoom_Factor;
                    }

                    // Transform cross-section coordinates in 2D
                    List<Point> crsccoordoutline = null;
                    List<Point> crsccoordinline = null;

                    if (model.m_arrMembers[i].CrScStart.CrScPointsOut != null)
                    {
                        //crsccoordoutline = new float[model.m_arrMembers[i].CrScStart.INoPointsOut, 2];
                        crsccoordoutline = new List<Point>(model.m_arrMembers[i].CrScStart.CrScPointsOut);
                        //Array.Copy(, crsccoordoutline, model.m_arrMembers[i].CrScStart.CrScPointsOut.Count);

                        // Transfom coordinates to geometry center
                        crsccoordoutline = model.m_arrMembers[i].CrScStart.GetCoordinatesInGeometryRelatedToGeometryCenterPoint(crsccoordoutline);

                        // TODO - kedze prut nemusi byt kolmy na smer pohladu, tak spravne by sa mal detekovat uhol, pod ktorym sa na prierez pozerame
                        // a mali by sa prepocitat lokalne suradnice prierezu
                        // Napr. stresny nosnik "rafter" je voci rovine pohladu sikmo v uhle roofpitch, takze prierez by sa mal zvacsit v smere zvislej lokalnej osy, zobrazena vyska prierezu je c = h / cos(roofpitch)
                        // takto by sa mali prepocitat vsetky suradnice prierezu v smere lokalnej zvislej osi [i,1]

                        for (int j = 0; j < model.m_arrMembers[i].CrScStart.INoPointsOut; j++)
                        {
                            double fx = Geom2D.GetRotatedPosition_x_CCW_rad(crsccoordoutline[j].X, crsccoordoutline[j].Y, model.m_arrMembers[i].DTheta_x);
                            double fy = Geom2D.GetRotatedPosition_y_CCW_rad(crsccoordoutline[j].X, crsccoordoutline[j].Y, model.m_arrMembers[i].DTheta_x);

                            Point point = crsccoordoutline[j];
                            point.X = (fx * fReal_Model_Zoom_Factor);
                            point.Y = (fy * fReal_Model_Zoom_Factor);

                            point.X += p.X;
                            point.Y += p.Y;
                        }
                    }

                    if (model.m_arrMembers[i].CrScStart.CrScPointsIn != null)
                    {
                        //crsccoordinline = new float[model.m_arrMembers[i].CrScStart.INoPointsIn, 2];
                        crsccoordinline = new List<Point>(model.m_arrMembers[i].CrScStart.CrScPointsIn);
                        //Array.Copy(model.m_arrMembers[i].CrScStart.CrScPointsIn, crsccoordinline, model.m_arrMembers[i].CrScStart.CrScPointsIn.Count);

                        // Transfom coordinates to geometry center
                        crsccoordinline = model.m_arrMembers[i].CrScStart.GetCoordinatesInGeometryRelatedToGeometryCenterPoint(crsccoordinline);

                        for (int j = 0; j < model.m_arrMembers[i].CrScStart.INoPointsIn; j++)
                        {
                            double fx = Geom2D.GetRotatedPosition_x_CCW_rad(crsccoordinline[j].X, crsccoordinline[j].Y, model.m_arrMembers[i].DTheta_x);
                            double fy = Geom2D.GetRotatedPosition_y_CCW_rad(crsccoordinline[j].X, crsccoordinline[j].Y, model.m_arrMembers[i].DTheta_x);

                            Point point = crsccoordinline[j];
                            point.X = (fx * fReal_Model_Zoom_Factor);
                            point.Y = (fy * fReal_Model_Zoom_Factor);

                            point.X += p.X;
                            point.Y += p.Y;
                        }
                    }

                    // Draw cross-section
                    DrawCrossSection(p, b, h, model.m_arrMembers[i].DTheta_x, crsccoordoutline, crsccoordinline);
                }
                else
                {
                    // Member is outside the box (do not draw)
                }
            }

            CanvasSection2D = canvasForImage;

        }

        public void DrawMember2D(SolidColorBrush strokeColor, SolidColorBrush fillColor, Point pA, double Width, double Length, double fRotationAboutX_rad, Canvas imageCanvas)
        {
            Point lt = new Point(0, - 0.5f * Width);
            Point br = new Point(Length, 0.5f * Width);
            Rectangle rect = new Rectangle();
            rect.Stretch = Stretch.Fill;
            rect.Opacity = 0.7f;
            rect.Fill = fillColor;
            rect.Stroke = strokeColor;
            rect.Width = br.X - lt.X;
            rect.Height = br.Y - lt.Y;
            Canvas.SetTop(rect, modelMarginBottom_y - pA.Y - lt.Y);
            Canvas.SetLeft(rect, modelMarginLeft_x + pA.X + lt.X);

            // Rotate about (X-axis)
            RotateTransform Rotation2D = new RotateTransform(-fRotationAboutX_rad / MathF.dPI * 180, lt.X , -lt.Y); // TODO Doriesit vypocet uhla??? 
            rect.RenderTransform = Rotation2D;
                        
            // Translate to the pA point coordinates in plane
            //TranslateTransform translate = new TranslateTransform(pA.X, pA.Y);
            //TranslateTransform translate = new TranslateTransform(200, 200);
            //rect.RenderTransform = translate;

            imageCanvas.Children.Add(rect);
        }

        public void DrawCrossSection(Point centroid, double b, double h, double theta_x, float[,] crsccoordoutline, float[,] crsccoordinline)
        {
            // Outer outline lines
            if (crsccoordoutline != null) // If is array of points not empty
            {
                double fCanvasTop = modelMarginBottom_y /*- fModel_Length_y_page*/ - centroid.Y;
                double fCanvasLeft = modelMarginLeft_x + centroid.X - 0.5f * b;

                if (MathF.d_equal(Math.Abs(theta_x), MathF.dPI / 2) || MathF.d_equal(Math.Abs(theta_x), 3 / 2 * MathF.dPI)) // Set different margin for rotated cross-section
                    fCanvasLeft = modelMarginLeft_x + centroid.X - 0.5f * h;

                DrawPolyLine(crsccoordoutline, fCanvasTop, fCanvasLeft, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
            }

            // Internal outline lines
            if (crsccoordinline != null) // If is array of points not empty
            {
                // TODO - doladit posun vonkajsieho obrysu voci vnutornemu (spravidla m_ft)
                double fCanvasTop = modelMarginBottom_y /*- fModel_Length_y_page*/ - centroid.Y;
                double fCanvasLeft = modelMarginLeft_x + centroid.X - 0.5f * b;

                if (MathF.d_equal(Math.Abs(theta_x), MathF.dPI / 2) || MathF.d_equal(Math.Abs(theta_x), 3 / 2 * MathF.dPI)) // Set different margin for rotated cross-section
                    fCanvasLeft = modelMarginLeft_x + centroid.X - 0.5 * h;

                DrawPolyLine(crsccoordinline, fCanvasTop, fCanvasLeft, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
            }
        }

        public void DrawCrossSection(Point centroid, double b, double h, double theta_x, List<Point> crsccoordoutline, List<Point> crsccoordinline)
        {
            // Outer outline lines
            if (crsccoordoutline != null) // If is array of points not empty
            {
                double fCanvasTop = modelMarginBottom_y /*- fModel_Length_y_page*/ - centroid.Y;
                double fCanvasLeft = modelMarginLeft_x + centroid.X - 0.5f * b;

                if (MathF.d_equal(Math.Abs(theta_x), MathF.dPI / 2) || MathF.d_equal(Math.Abs(theta_x), 3 / 2 * MathF.dPI)) // Set different margin for rotated cross-section
                    fCanvasLeft = modelMarginLeft_x + centroid.X - 0.5f * h;

                DrawPolyLine(crsccoordoutline, fCanvasTop, fCanvasLeft, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
            }

            // Internal outline lines
            if (crsccoordinline != null) // If is array of points not empty
            {
                // TODO - doladit posun vonkajsieho obrysu voci vnutornemu (spravidla m_ft)
                double fCanvasTop = modelMarginBottom_y /*- fModel_Length_y_page*/ - centroid.Y;
                double fCanvasLeft = modelMarginLeft_x + centroid.X - 0.5f * b;

                if (MathF.d_equal(Math.Abs(theta_x), MathF.dPI / 2) || MathF.d_equal(Math.Abs(theta_x), 3 / 2 * MathF.dPI)) // Set different margin for rotated cross-section
                    fCanvasLeft = modelMarginLeft_x + centroid.X - 0.5 * h;

                DrawPolyLine(crsccoordinline, fCanvasTop, fCanvasLeft, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, canvasForImage);
            }
        }

        public void DrawPolyLine(float[,] arrPoints, double dCanvasTopTemp, double dCanvasLeftTemp, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
        {
            PointCollection points = new PointCollection();

            for (int i = 0; i < arrPoints.Length / 2 + 1; i++)
            {
                if (i < ((arrPoints.Length / 2)))
                    points.Add(new Point(arrPoints[i, 0], arrPoints[i, 1]));
                else
                    points.Add(new Point(arrPoints[0, 0], arrPoints[0, 1])); // Last point is same as first one
            }

            Polyline myLine = new Polyline();
            myLine.Stretch = Stretch.Fill;
            myLine.Stroke = color;
            myLine.Points = points;
            myLine.StrokeThickness = thickness;
            myLine.StrokeStartLineCap = startCap;
            myLine.StrokeEndLineCap = endCap;
            Canvas.SetTop(myLine, dCanvasTopTemp);
            Canvas.SetLeft(myLine, dCanvasLeftTemp);
            imageCanvas.Children.Add(myLine);
        }
        public void DrawPolyLine(List<Point> listPoints, double dCanvasTopTemp, double dCanvasLeftTemp, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
        {
            PointCollection points = new PointCollection();

            for (int i = 0; i < listPoints.Count + 1; i++)
            {
                if (i < ((listPoints.Count)))
                    points.Add(new Point(listPoints[i].X, listPoints[i].Y));
                else
                    points.Add(new Point(listPoints[0].X, listPoints[0].Y)); // Last point is same as first one
            }

            Polyline myLine = new Polyline();
            myLine.Stretch = Stretch.Fill;
            myLine.Stroke = color;
            myLine.Points = points;
            myLine.StrokeThickness = thickness;
            myLine.StrokeStartLineCap = startCap;
            myLine.StrokeEndLineCap = endCap;
            Canvas.SetTop(myLine, dCanvasTopTemp);
            Canvas.SetLeft(myLine, dCanvasLeftTemp);
            imageCanvas.Children.Add(myLine);
        }

        private void CalculateBasicValue()
        {
            double fModel_Length_x_page = dModelDimension_Y_real;
            double fModel_Length_y_page = dModelDimension_Z_real;

            // Calculate maximum zoom factor
            // Original ratio
            double dFactor_x = fModel_Length_x_page / dPageWidth;
            double dFactor_y = fModel_Length_y_page / dPageHeight;

            // Calculate new model dimensions (zoom of model size is 90%)
            fReal_Model_Zoom_Factor = 0.9f / MathF.Max(dFactor_x, dFactor_y);

            // Set new size of model on the page
            fModel_Length_x_page = fReal_Model_Zoom_Factor * dModelDimension_Y_real;
            fModel_Length_y_page = fReal_Model_Zoom_Factor * dModelDimension_Z_real;

            modelMarginLeft_x = 0.5 * (dPageWidth - fModel_Length_x_page);
            modelMarginBottom_y = fModel_Length_y_page + 0.5 * (dPageHeight - fModel_Length_y_page);
        }

        private void comboViews_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboViews.SelectedIndex)
            {
                case 0: DrawFrontView_ModelToCanvas(Model); break;
                case 1: DrawBackView_ModelToCanvas(Model); break;
                case 2: DrawLeftView_ModelToCanvas(Model); break;
                case 3: DrawRightView_ModelToCanvas(Model); break;
                case 4: DrawTopView_ModelToCanvas(Model); break;
            }
        }

        private void saveDXF_Click(object sender, RoutedEventArgs e)
        {
            CExportToDXF.ExportCanvas_DXF(CanvasSection2D, modelMarginLeft_x + 14, modelMarginBottom_y);
        }

        //public void CalculateModelLimits(CModel cmodel, out float fTempMax_X, out float fTempMin_X, out float fTempMax_Y, out float fTempMin_Y, out float fTempMax_Z, out float fTempMin_Z)
        //{
        //    fTempMax_X = float.MinValue;
        //    fTempMin_X = float.MaxValue;
        //    fTempMax_Y = float.MinValue;
        //    fTempMin_Y = float.MaxValue;
        //    fTempMax_Z = float.MinValue;
        //    fTempMin_Z = float.MaxValue;

        //    if (cmodel.m_arrNodes != null) // Some nodes exist
        //    {   
        //        fTempMax_X = cmodel.m_arrNodes.Max(n => n.X);
        //        fTempMin_X = cmodel.m_arrNodes.Min(n => n.X);
        //        fTempMax_Y = cmodel.m_arrNodes.Max(n => n.Y);
        //        fTempMin_Y = cmodel.m_arrNodes.Min(n => n.Y);
        //        fTempMax_Z = cmodel.m_arrNodes.Max(n => n.Z);
        //        fTempMin_Z = cmodel.m_arrNodes.Min(n => n.Z);
        //    }
        //    else if (cmodel.m_arrGOPoints != null) // Some points exist
        //    {
        //        fTempMax_X = (float)cmodel.m_arrGOPoints.Max(p => p.X);
        //        fTempMin_X = (float)cmodel.m_arrGOPoints.Min(p => p.X);
        //        fTempMax_Y = (float)cmodel.m_arrGOPoints.Max(p => p.Y);
        //        fTempMin_Y = (float)cmodel.m_arrGOPoints.Min(p => p.Y);
        //        fTempMax_Z = (float)cmodel.m_arrGOPoints.Max(p => p.Z);
        //        fTempMin_Z = (float)cmodel.m_arrGOPoints.Min(p => p.Z);
        //    }
        //    else
        //    {
        //        // Exception - no definition nodes or points
        //    }
        //}
    }
}
