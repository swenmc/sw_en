using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Configuration;
using System.Globalization;
using System.Windows.Media;
using System.Collections.ObjectModel;
using MATH;
using BaseClasses;
using BaseClasses.GraphObj;
using CRSC;
using DATABASE;
using DATABASE.DTO;
using System.Windows;
using System.Windows.Controls;
using EXPIMP;

namespace PFD
{
    public class SCVDisplayOptionsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------        
        bool bDrawPoints;
        bool bDrawOutLine;
        bool bDrawPointNumbers;
        bool bDrawHoles;
        bool bDrawHoleCentreSymbol;
        bool bDrawDrillingRoute;
        bool bDrawDimensions;
        bool bDrawMemberOutline;
        bool bDrawBendLines;

        bool bExportRequired;

        //-------------------------------------------------------------------------------------------------------------        
        public bool DrawPoints2D
        {
            get
            {
                return bDrawPoints;
            }

            set
            {
                bDrawPoints = value;
                NotifyPropertyChanged("DrawPoints2D");
            }
        }

        public bool DrawOutLine2D
        {
            get
            {
                return bDrawOutLine;
            }

            set
            {
                bDrawOutLine = value;
                NotifyPropertyChanged("DrawOutLine2D");
            }
        }

        public bool DrawPointNumbers2D
        {
            get
            {
                return bDrawPointNumbers;
            }

            set
            {
                bDrawPointNumbers = value;
                NotifyPropertyChanged("DrawPointNumbers2D");
            }
        }

        public bool DrawHoles2D
        {
            get
            {
                return bDrawHoles;
            }

            set
            {
                bDrawHoles = value;
                NotifyPropertyChanged("DrawHoles2D");
            }
        }

        public bool DrawHoleCentreSymbol2D
        {
            get
            {
                return bDrawHoleCentreSymbol;
            }

            set
            {
                bDrawHoleCentreSymbol = value;
                NotifyPropertyChanged("DrawHoleCentreSymbol2D");
            }
        }

        public bool DrawDrillingRoute2D
        {
            get
            {
                return bDrawDrillingRoute;
            }

            set
            {
                bDrawDrillingRoute = value;
                NotifyPropertyChanged("DrawDrillingRoute2D");
            }
        }

        public bool DrawDimensions2D
        {
            get
            {
                return bDrawDimensions;
            }

            set
            {
                bDrawDimensions = value;
                NotifyPropertyChanged("DrawDimensions2D");
            }
        }

        public bool DrawMemberOutline2D
        {
            get
            {
                return bDrawMemberOutline;
            }

            set
            {
                bDrawMemberOutline = value;
                NotifyPropertyChanged("DrawMemberOutline2D");
            }
        }

        public bool DrawBendLines2D
        {
            get
            {
                return bDrawBendLines;
            }

            set
            {
                bDrawBendLines = value;
                NotifyPropertyChanged("DrawBendLines2D");
            }
        }

        public bool ExportRequired
        {
            get
            {
                return bExportRequired;
            }

            set
            {
                bExportRequired = value;
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public SCVDisplayOptionsViewModel()
        {
            DrawPoints2D = false;
            DrawOutLine2D= true; // Default
            DrawPointNumbers2D = false;
            DrawHoles2D = true; // Default
            DrawHoleCentreSymbol2D = false;
            DrawDrillingRoute2D = false;
            DrawDimensions2D = true;
            DrawMemberOutline2D = true;
            DrawBendLines2D = true;

            ExportRequired = false;
        }
        public SCVDisplayOptionsViewModel(bool drawPoints, bool drawOutLine, bool drawPointNumbers, bool drawHoles, bool drawHoleCentreSymbol, bool drawDrillingRoute,
                                bool drawDimensions, bool drawMemberOutline, bool drawBendLines)
        {
            DrawPoints2D = drawPoints;
            DrawOutLine2D = drawOutLine;
            DrawPointNumbers2D = drawPointNumbers;
            DrawHoles2D = drawHoles;
            DrawHoleCentreSymbol2D = drawHoleCentreSymbol;
            DrawDrillingRoute2D = drawDrillingRoute;
            DrawDimensions2D = drawDimensions;
            DrawMemberOutline2D = drawMemberOutline;
            DrawBendLines2D = drawBendLines;

            ExportRequired = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
