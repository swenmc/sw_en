using _3DTools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    // Class CNode
    [Serializable]
    public class CNode : CEntity
    {
        public float X;
        public float Y;
        public float Z;

        // Konstruktor1 CNode
        public CNode()
        {

        }
        // Konstruktor2 CNode (2D)
        public CNode(
            int iNode_ID,
            float fCoord_X,
            float fCoord_Y,
            float fTime
            )
        {
            ID = iNode_ID;
            X = fCoord_X;
            Y = fCoord_Y;
            FTime = fTime;
        }

        // Konstruktor3 CNode (3D)
        public CNode(
            int iNode_ID,
            float fCoord_X,
            float fCoord_Y,
            float fCoord_Z,
            float fTime
            )
        {
            ID = iNode_ID;
            X = fCoord_X;
            Y = fCoord_Y;
            Z = fCoord_Z;
            FTime = fTime;
        }
        

        #region IComparer Members

        public int Compare(object x, object y)
        {
            return ((CNode)x).ID - ((CNode)y).ID;
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return this.ID - ((CNode)obj).ID;
        }

        public override bool Equals(object obj)
        {
            if (obj is CNode) return this.ID.Equals(((CNode)obj).ID);
            else return false;
        }

        #endregion
    } // End of Class CNode

    /*
    public class CCompare_NodeID : IComparer
    {
        // x<y - zaporne cislo; x=y - nula; x>y - kladne cislo
        public int Compare(object x, object y)
        {
            return ((CNode)x).ID - ((CNode)y).ID;
        }
    }*/

    public class PointVisual3D : ModelVisual3D
    {
        private readonly GeometryModel3D _model;
        private readonly MeshGeometry3D _mesh;
        private Matrix3D _visualToScreen;
        private Matrix3D _screenToVisual;

        public PointVisual3D()
        {
            _mesh = new MeshGeometry3D();
            _model = new GeometryModel3D();
            _model.Geometry = _mesh;
            SetColor(this.Color);
            this.Content = _model;
            this.Points = new Point3DCollection();
            CompositionTarget.Rendering += OnRender;
        }

        private void OnRender(object sender, EventArgs e)
        {
            if (Points.Count == 0 && _mesh.Positions.Count == 0)
            {
                return;
            }


            // spristupnit mainwindow !!!!!!!!!!
            /*
            if (UpdateTransforms() && MainWindow.mousedown == false)
            {
                RebuildGeometry();
            }*/
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(PointVisual3D), new PropertyMetadata(Colors.White, OnColorChanged));
        private static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((PointVisual3D)sender).SetColor((Color)args.NewValue);
        }

        private void SetColor(Color color)
        {
            MaterialGroup unlitMaterial = new MaterialGroup();
            unlitMaterial.Children.Add(new DiffuseMaterial(new SolidColorBrush(Colors.Black)));
            unlitMaterial.Children.Add(new EmissiveMaterial(new SolidColorBrush(color)));
            unlitMaterial.Freeze();
            _model.Material = unlitMaterial;
            _model.BackMaterial = unlitMaterial;
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        //public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(double), typeof(PointVisual3D), new PropertyMetadata(1.0, OnThicknessChanged));
        //private static void OnThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        //{
        //    ((PointVisual3D)sender).GeometryDirty();
        //}

        //public double Thickness
        //{
        //    get { return (double)GetValue(ThicknessProperty); }
        //    set { SetValue(ThicknessProperty, value); }
        //}

        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(Point3DCollection), typeof(PointVisual3D), new PropertyMetadata(null, OnPointsChanged));
        private static void OnPointsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((PointVisual3D)sender).GeometryDirty();
        }

        public Point3DCollection Points
        {
            get { return (Point3DCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        private void GeometryDirty()
        {
            _visualToScreen = MathUtils.ZeroMatrix;
        }

        private void RebuildGeometry()
        {
            //double halfThickness = Thickness / 2.0;
            //int numLines = Points.Count / 2;
            //Point3DCollection positions = new Point3DCollection(numLines * 4);
            //for (int i = 0; i < numLines; i++)
            //{
            //    int startIndex = i * 2;
            //    Point3D startPoint = Points[startIndex];
            //    Point3D endPoint = Points[startIndex + 1];
            //    AddSegment(positions, startPoint, endPoint, halfThickness);
            //}
            //positions.Freeze();
            //_mesh.Positions = positions;
            Int32Collection indices = new Int32Collection(Points.Count * 6);
            for (int i = 0; i < Points.Count; i++)
            {
                indices.Add(i * 4 + 2);
                indices.Add(i * 4 + 1);
                indices.Add(i * 4 + 0);
                indices.Add(i * 4 + 2);
                indices.Add(i * 4 + 3);
                indices.Add(i * 4 + 1);
            }

            indices.Freeze();
            _mesh.TriangleIndices = indices;
            _mesh.Positions = CreatePositions(this.Points, this.Size, 0.0);
        }

        public Point3DCollection CreatePositions(IList<Point3D> points, double size = 1.0, double depthOffset = 0.0)
        {
            double halfSize = size / 2.0;
            int numPoints = points.Count;

            var outline = new[]
            {
                new Vector(-halfSize, halfSize), new Vector(-halfSize, -halfSize), new Vector(halfSize, halfSize),
                new Vector(halfSize, -halfSize)
            };

            var positions = new Point3DCollection(numPoints * 4);

            for (int i = 0; i < numPoints; i++)
            {
                var screenPoint = (Point4D)points[i] * this._visualToScreen;

                double spx = screenPoint.X;
                double spy = screenPoint.Y;
                double spz = screenPoint.Z;
                double spw = screenPoint.W;

                if (!depthOffset.Equals(0))
                {
                    spz -= depthOffset * spw;
                }

                var p0 = new Point4D(spx, spy, spz, spw) * this._screenToVisual;
                double pwinverse = 1 / p0.W;

                foreach (var v in outline)
                {
                    var p = new Point4D(spx + v.X * spw, spy + v.Y * spw, spz, spw) * this._screenToVisual;
                    positions.Add(new Point3D(p.X * pwinverse, p.Y * pwinverse, p.Z * pwinverse));
                }
            }

            positions.Freeze();
            return positions;
        }

        /// <summary>
        /// Identifies the <see cref="Size"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
            "Size", typeof(double), typeof(PointVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

        protected static void GeometryChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((PointVisual3D)sender).GeometryDirty();
        }

        public double Size
        {
            get
            {
                return (double)this.GetValue(SizeProperty);
            }

            set
            {
                this.SetValue(SizeProperty, value);
            }
        }


        //private void AddSegment(Point3DCollection positions, Point3D startPoint, Point3D endPoint, double halfThickness)
        //{
        //    Vector3D lineDirection = endPoint * _visualToScreen - startPoint * _visualToScreen;
        //    lineDirection.Z = 0;
        //    lineDirection.Normalize();
        //    Vector delta = new Vector(-lineDirection.Y, lineDirection.X);
        //    delta *= halfThickness;
        //    Point3D pOut1, pOut2;
        //    Widen(startPoint, delta, out pOut1, out pOut2);
        //    positions.Add(pOut1);
        //    positions.Add(pOut2);
        //    Widen(endPoint, delta, out pOut1, out pOut2);
        //    positions.Add(pOut1);
        //    positions.Add(pOut2);
        //}
        //private void Widen(Point3D pIn, Vector delta, out Point3D pOut1, out Point3D pOut2)
        //{
        //    Point4D pIn4 = (Point4D)pIn;
        //    Point4D pOut41 = pIn4 * _visualToScreen;
        //    Point4D pOut42 = pOut41;
        //    pOut41.X += delta.X * pOut41.W;
        //    pOut41.Y += delta.Y * pOut41.W;
        //    pOut42.X -= delta.X * pOut42.W;
        //    pOut42.Y -= delta.Y * pOut42.W;
        //    pOut41 *= _screenToVisual;
        //    pOut42 *= _screenToVisual;
        //    pOut1 = new Point3D(pOut41.X / pOut41.W, pOut41.Y / pOut41.W, pOut41.Z / pOut41.W);
        //    pOut2 = new Point3D(pOut42.X / pOut42.W, pOut42.Y / pOut42.W, pOut42.Z / pOut42.W);
        //}
        private bool UpdateTransforms()
        {
            Viewport3DVisual viewport;
            bool success;
            Matrix3D visualToScreen = MathUtils.TryTransformTo2DAncestor(this, out viewport, out success);
            if (!success || !visualToScreen.HasInverse)
            {
                _mesh.Positions = null;
                return false;
            }
            if (visualToScreen == _visualToScreen)
            {
                return false;
            }
            _visualToScreen = _screenToVisual = visualToScreen;
            _screenToVisual.Invert();
            return true;
        }
    }

}
