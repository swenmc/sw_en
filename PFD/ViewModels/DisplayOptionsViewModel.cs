using BaseClasses;
using BaseClasses.GraphObj;
using BaseClasses.Helpers;
using DATABASE;
using DATABASE.DTO;
using MATH;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace PFD
{
    public class DisplayOptionsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private Color m_WireframeColor;
        private int m_WireframeColorIndex;
        private float m_WireFrameLineThickness;

        private Color m_MemberCenterlineColor;
        private int m_MemberCenterlineColorIndex;
        private float m_MemberCenterlineThickness;

        private float m_NodeDescriptionTextFontSize;
        private float m_MemberDescriptionTextFontSize;
        private float m_DimensionTextFontSize;
        private float m_GridLineLabelTextFontSize;
        private float m_SectionSymbolLabelTextFontSize;
        private float m_DetailSymbolLabelTextFontSize;

        private float m_SawCutTextFontSize;
        private float m_ControlJointTextFontSize;

        private float m_FoundationTextFontSize;
        private float m_FloorSlabTextFontSize;

        private Color m_NodeColor;
        private int m_NodeColorIndex;
        private Color m_NodeDescriptionTextColor = Colors.Cyan;
        private int m_NodeDescriptionTextColorIndex;
        private Color m_MemberDescriptionTextColor = Colors.Beige;
        private int m_MemberDescriptionTextColorIndex;
        private Color m_DimensionTextColor = Colors.LightGreen;
        private int m_DimensionTextColorIndex;
        private Color m_DimensionLineColor = Colors.LightGreen;
        private int m_DimensionLineColorIndex;

        private Color m_GridLineLabelTextColor = Colors.Coral;
        private int m_GridLineLabelTextColorIndex;
        private Color m_GridLineColor = Colors.Coral;
        private int m_GridLineColorIndex;
        private int m_GridLinePatternType = (int)ELinePatternType.DASHDOTTED;
                
        private Color m_SectionSymbolLabelTextColor = Colors.Cyan;
        private int m_SectionSymbolLabelTextColorIndex;
        private Color m_SectionSymbolColor = Colors.Cyan;
        private int m_SectionSymbolColorIndex;

        private Color m_DetailSymbolLabelTextColor = Colors.LightPink;
        private Color? m_DetailSymbolLabelBackColor = Colors.White;
        private int m_DetailSymbolLabelTextColorIndex;
        private int m_DetailSymbolLabelBackColorIndex;
        private Color m_DetailSymbolColor = Colors.LightPink;
        private int m_DetailSymbolColorIndex;

        private Color m_SawCutTextColor = Colors.Goldenrod;
        private int m_SawCutTextColorIndex;
        private Color m_SawCutLineColor = Colors.Goldenrod;
        private int m_SawCutLineColorIndex;
        private int m_SawCutLinePatternType = (int)ELinePatternType.DOTTED;
                
        private Color m_ControlJointTextColor = Colors.BlueViolet;
        private int m_ControlJointTextColorIndex;
        private Color m_ControlJointLineColor = Colors.BlueViolet;
        private int m_ControlJointLineColorIndex;
        private int m_ControlJointLinePatternType = (int)ELinePatternType.DIVIDE;
                
        private Color m_FoundationTextColor = Colors.HotPink;
        private int m_FoundationTextColorIndex;
        private Color m_FloorSlabTextColor = Colors.HotPink;
        private int m_FloorSlabTextColorIndex;

        private Color m_FoundationColor = Colors.DarkGray;
        private int m_FoundationColorIndex;
        private Color m_FloorSlabColor = Colors.LightGray;
        private int m_FloorSlabColorIndex;
        private Color m_SlabRebateColor = Colors.OrangeRed;
        private int m_SlabRebateColorIndex;

        private int m_BackgroundColorIndex;
        private Color m_BackgroundColor = Colors.Black;

        private float m_MemberSolidModelOpacity = 0.8f;
        private float m_PlateSolidModelOpacity = 0.5f;
        private float m_ScrewSolidModelOpacity = 0.9f;
        private float m_AnchorSolidModelOpacity = 0.9f;
        private float m_FoundationSolidModelOpacity = 0.4f;
        private float m_ReinforcementBarSolidModelOpacity = 0.9f;
        private float m_FloorSlabSolidModelOpacity = 0.3f;
        private float m_SlabRebateSolidModelOpacity = 0.5f;

        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        
        public Color WireframeColor
        {
            get
            {
                return m_WireframeColor;
            }
            set
            {
                m_WireframeColor = value;
            }
        }

        public float WireFrameLineThickness
        {
            get
            {
                return m_WireFrameLineThickness;
            }

            set
            {
                m_WireFrameLineThickness = value;
                NotifyPropertyChanged("WireFrameLineThickness");
            }
        }

        public Color MemberCenterlineColor
        {
            get
            {
                return m_MemberCenterlineColor;
            }
            set
            {
                m_MemberCenterlineColor = value;
            }
        }

        public float MemberCenterlineThickness
        {
            get
            {
                return m_MemberCenterlineThickness;
            }

            set
            {
                m_MemberCenterlineThickness = value;
                NotifyPropertyChanged("MemberCenterlineThickness");
            }
        }

        public float NodeDescriptionTextFontSize
        {
            get
            {
                return m_NodeDescriptionTextFontSize;
            }

            set
            {
                m_NodeDescriptionTextFontSize = value;
                NotifyPropertyChanged("NodeDescriptionTextFontSize");
            }
        }

        public float MemberDescriptionTextFontSize
        {
            get
            {
                return m_MemberDescriptionTextFontSize;
            }

            set
            {
                m_MemberDescriptionTextFontSize = value;
                NotifyPropertyChanged("MemberDescriptionTextFontSize");
            }
        }

        public float DimensionTextFontSize
        {
            get
            {
                return m_DimensionTextFontSize;
            }

            set
            {
                m_DimensionTextFontSize = value;
                NotifyPropertyChanged("DimensionTextFontSize");
            }
        }

        public float GridLineLabelTextFontSize
        {
            get
            {
                return m_GridLineLabelTextFontSize;
            }

            set
            {
                m_GridLineLabelTextFontSize = value;
                NotifyPropertyChanged("GridLineLabelTextFontSize");
            }
        }

        public float SectionSymbolLabelTextFontSize
        {
            get
            {
                return m_SectionSymbolLabelTextFontSize;
            }

            set
            {
                m_SectionSymbolLabelTextFontSize = value;
                NotifyPropertyChanged("SectionSymbolLabelTextFontSize");
            }
        }

        public float DetailSymbolLabelTextFontSize
        {
            get
            {
                return m_DetailSymbolLabelTextFontSize;
            }

            set
            {
                m_DetailSymbolLabelTextFontSize = value;
                NotifyPropertyChanged("DetailSymbolLabelTextFontSize");
            }
        }

        public float SawCutTextFontSize
        {
            get
            {
                return m_SawCutTextFontSize;
            }

            set
            {
                m_SawCutTextFontSize = value;
                NotifyPropertyChanged("SawCutTextFontSize");
            }
        }

        public float ControlJointTextFontSize
        {
            get
            {
                return m_ControlJointTextFontSize;
            }

            set
            {
                m_ControlJointTextFontSize = value;
                NotifyPropertyChanged("ControlJointTextFontSize");
            }
        }

        public float FoundationTextFontSize
        {
            get
            {
                return m_FoundationTextFontSize;
            }

            set
            {
                m_FoundationTextFontSize = value;
                NotifyPropertyChanged("FoundationTextFontSize");
            }
        }

        public float FloorSlabTextFontSize
        {
            get
            {
                return m_FloorSlabTextFontSize;
            }

            set
            {
                m_FloorSlabTextFontSize = value;
                NotifyPropertyChanged("FloorSlabTextFontSize");
            }
        }

        public Color NodeColor
        {
            get
            {
                return m_NodeColor;
            }

            set
            {
                m_NodeColor = value;
                NotifyPropertyChanged("NodeColor");
            }
        }

        public Color NodeDescriptionTextColor
        {
            get
            {
                return m_NodeDescriptionTextColor;
            }

            set
            {
                m_NodeDescriptionTextColor = value;
                NotifyPropertyChanged("NodeDescriptionTextColor");
            }
        }

        public Color MemberDescriptionTextColor
        {
            get
            {
                return m_MemberDescriptionTextColor;
            }

            set
            {
                m_MemberDescriptionTextColor = value;
                NotifyPropertyChanged("MemberDescriptionTextColor");
            }
        }

        public Color DimensionTextColor
        {
            get
            {
                return m_DimensionTextColor;
            }

            set
            {
                m_DimensionTextColor = value;
                NotifyPropertyChanged("DimensionTextColor");
            }
        }

        public Color DimensionLineColor
        {
            get
            {
                return m_DimensionLineColor;
            }

            set
            {
                m_DimensionLineColor = value;
                NotifyPropertyChanged("DimensionLineColor");
            }
        }

        public Color GridLineLabelTextColor
        {
            get
            {
                return m_GridLineLabelTextColor;
            }

            set
            {
                m_GridLineLabelTextColor = value;
                NotifyPropertyChanged("GridLineLabelTextColor");
            }
        }

        public Color GridLineColor
        {
            get
            {
                return m_GridLineColor;
            }

            set
            {
                m_GridLineColor = value;
                NotifyPropertyChanged("GridLineColor");
            }
        }

        public int GridLinePatternType
        {
            get
            {
                return m_GridLinePatternType;
            }

            set
            {
                m_GridLinePatternType = value;
                NotifyPropertyChanged("GridLinePatternType");
            }
        }

        public Color SectionSymbolLabelTextColor
        {
            get
            {
                return m_SectionSymbolLabelTextColor;
            }

            set
            {
                m_SectionSymbolLabelTextColor = value;
                NotifyPropertyChanged("SectionSymbolLabelTextColor");
            }
        }

        public Color SectionSymbolColor
        {
            get
            {
                return m_SectionSymbolColor;
            }

            set
            {
                m_SectionSymbolColor = value;
                NotifyPropertyChanged("SectionSymbolColor");
            }
        }

        public Color DetailSymbolLabelTextColor
        {
            get
            {
                return m_DetailSymbolLabelTextColor;
            }

            set
            {
                m_DetailSymbolLabelTextColor = value;
                NotifyPropertyChanged("DetailSymbolLabelTextColor");
            }
        }
        public Color? DetailSymbolLabelBackColor
        {
            get
            {
                return m_DetailSymbolLabelBackColor;
            }

            set
            {
                m_DetailSymbolLabelBackColor = value;
                NotifyPropertyChanged("DetailSymbolLabelBackColor");
            }
        }

        public Color DetailSymbolColor
        {
            get
            {
                return m_DetailSymbolColor;
            }

            set
            {
                m_DetailSymbolColor = value;
                NotifyPropertyChanged("DetailSymbolColor");
            }
        }

        public Color SawCutTextColor
        {
            get
            {
                return m_SawCutTextColor;
            }

            set
            {
                m_SawCutTextColor = value;
                NotifyPropertyChanged("SawCutTextColor");
            }
        }

        public Color SawCutLineColor
        {
            get
            {
                return m_SawCutLineColor;
            }

            set
            {
                m_SawCutLineColor = value;
                NotifyPropertyChanged("SawCutLineColor");
            }
        }

        public int SawCutLinePatternType
        {
            get
            {
                return m_SawCutLinePatternType;
            }

            set
            {
                m_SawCutLinePatternType = value;
                NotifyPropertyChanged("SawCutLinePatternType");
            }
        }

        public Color ControlJointTextColor
        {
            get
            {
                return m_ControlJointTextColor;
            }

            set
            {
                m_ControlJointTextColor = value;
                NotifyPropertyChanged("ControlJointTextColor");
            }
        }

        public Color ControlJointLineColor
        {
            get
            {
                return m_ControlJointLineColor;
            }

            set
            {
                m_ControlJointLineColor = value;
                NotifyPropertyChanged("ControlJointLineColor");
            }
        }

        public int ControlJointLinePatternType
        {
            get
            {
                return m_ControlJointLinePatternType;
            }

            set
            {
                m_ControlJointLinePatternType = value;
                NotifyPropertyChanged("ControlJointLinePatternType");
            }
        }

        public Color FoundationTextColor
        {
            get
            {
                return m_FoundationTextColor;
            }

            set
            {
                m_FoundationTextColor = value;
                NotifyPropertyChanged("FoundationTextColor");
            }
        }

        public Color FloorSlabTextColor
        {
            get
            {
                return m_FloorSlabTextColor;
            }

            set
            {
                m_FloorSlabTextColor = value;
                NotifyPropertyChanged("FloorSlabTextColor");
            }
        }

        public Color FoundationColor
        {
            get
            {
                return m_FoundationColor;
            }

            set
            {
                m_FoundationColor = value;
                NotifyPropertyChanged("FoundationColor");
            }
        }

        public Color FloorSlabColor
        {
            get
            {
                return m_FloorSlabColor;
            }

            set
            {
                m_FloorSlabColor = value;
                NotifyPropertyChanged("FloorSlabColor");
            }
        }

        public Color SlabRebateColor
        {
            get
            {
                return m_SlabRebateColor;
            }

            set
            {
                m_SlabRebateColor = value;
                NotifyPropertyChanged("SlabRebateColor");
            }
        }

        public float MemberSolidModelOpacity
        {
            get
            {
                return m_MemberSolidModelOpacity;
            }

            set
            {
                m_MemberSolidModelOpacity = value;
                NotifyPropertyChanged("MemberSolidModelOpacity");
            }
        }

        public float PlateSolidModelOpacity
        {
            get
            {
                return m_PlateSolidModelOpacity;
            }

            set
            {
                m_PlateSolidModelOpacity = value;
                NotifyPropertyChanged("PlateSolidModelOpacity");
            }
        }

        public float ScrewSolidModelOpacity
        {
            get
            {
                return m_ScrewSolidModelOpacity;
            }

            set
            {
                m_ScrewSolidModelOpacity = value;
                NotifyPropertyChanged("ScrewSolidModelOpacity");
            }
        }

        public float AnchorSolidModelOpacity
        {
            get
            {
                return m_AnchorSolidModelOpacity;
            }

            set
            {
                m_AnchorSolidModelOpacity = value;
                NotifyPropertyChanged("AnchorSolidModelOpacity");
            }
        }

        public float FoundationSolidModelOpacity
        {
            get
            {
                return m_FoundationSolidModelOpacity;
            }

            set
            {
                m_FoundationSolidModelOpacity = value;
                NotifyPropertyChanged("FoundationSolidModelOpacity");
            }
        }

        public float ReinforcementBarSolidModelOpacity
        {
            get
            {
                return m_ReinforcementBarSolidModelOpacity;
            }

            set
            {
                m_ReinforcementBarSolidModelOpacity = value;
                NotifyPropertyChanged("ReinforcementBarSolidModelOpacity");
            }
        }

        public float FloorSlabSolidModelOpacity
        {
            get
            {
                return m_FloorSlabSolidModelOpacity;
            }

            set
            {
                m_FloorSlabSolidModelOpacity = value;
                NotifyPropertyChanged("FloorSlabSolidModelOpacity");
            }
        }

        public float SlabRebateSolidModelOpacity
        {
            get
            {
                return m_SlabRebateSolidModelOpacity;
            }

            set
            {
                m_SlabRebateSolidModelOpacity = value;
                NotifyPropertyChanged("SlabRebateSolidModelOpacity");
            }
        }

        public int WireframeColorIndex
        {
            get
            {
                return m_WireframeColorIndex;
            }

            set
            {
                m_WireframeColorIndex = value;
                
                WireframeColor = CComboBoxHelper.ColorList[m_WireframeColorIndex].Color.Value;

                NotifyPropertyChanged("WireframeColorIndex");
            }
        }

        public int MemberCenterlineColorIndex
        {
            get
            {
                return m_MemberCenterlineColorIndex;
            }

            set
            {
                m_MemberCenterlineColorIndex = value;

                MemberCenterlineColor = CComboBoxHelper.ColorList[m_MemberCenterlineColorIndex].Color.Value;

                NotifyPropertyChanged("MemberCenterlineColorIndex");
            }
        }

        public int NodeColorIndex
        {
            get
            {
                return m_NodeColorIndex;
            }

            set
            {
                m_NodeColorIndex = value;
                
                NodeColor = CComboBoxHelper.ColorList[m_NodeColorIndex].Color.Value;

                NotifyPropertyChanged("NodeColorIndex");
            }
        }

        public int NodeDescriptionTextColorIndex
        {
            get
            {
                return m_NodeDescriptionTextColorIndex;
            }

            set
            {
                m_NodeDescriptionTextColorIndex = value;
                
                NodeDescriptionTextColor = CComboBoxHelper.ColorList[m_NodeDescriptionTextColorIndex].Color.Value;

                NotifyPropertyChanged("NodeDescriptionTextColorIndex");
            }
        }

        public int MemberDescriptionTextColorIndex
        {
            get
            {
                return m_MemberDescriptionTextColorIndex;
            }

            set
            {
                m_MemberDescriptionTextColorIndex = value;

                MemberDescriptionTextColor = CComboBoxHelper.ColorList[m_MemberDescriptionTextColorIndex].Color.Value;

                NotifyPropertyChanged("MemberDescriptionTextColorIndex");
            }
        }

        public int DimensionTextColorIndex
        {
            get
            {
                return m_DimensionTextColorIndex;
            }

            set
            {
                m_DimensionTextColorIndex = value;

                DimensionTextColor = CComboBoxHelper.ColorList[m_DimensionTextColorIndex].Color.Value;

                NotifyPropertyChanged("DimensionTextColorIndex");
            }
        }

        public int DimensionLineColorIndex
        {
            get
            {
                return m_DimensionLineColorIndex;
            }

            set
            {
                m_DimensionLineColorIndex = value;

                DimensionLineColor = CComboBoxHelper.ColorList[m_DimensionLineColorIndex].Color.Value;

                NotifyPropertyChanged("DimensionLineColorIndex");
            }
        }

        public int GridLineLabelTextColorIndex
        {
            get
            {
                return m_GridLineLabelTextColorIndex;
            }

            set
            {
                m_GridLineLabelTextColorIndex = value;

                GridLineLabelTextColor = CComboBoxHelper.ColorList[m_GridLineLabelTextColorIndex].Color.Value;

                NotifyPropertyChanged("GridLineLabelTextColorIndex");
            }
        }

        public int GridLineColorIndex
        {
            get
            {
                return m_GridLineColorIndex;
            }

            set
            {
                m_GridLineColorIndex = value;
                
                GridLineColor = CComboBoxHelper.ColorList[m_GridLineColorIndex].Color.Value;

                NotifyPropertyChanged("GridLineColorIndex");
            }
        }
        public int SectionSymbolLabelTextColorIndex
        {
            get
            {
                return m_SectionSymbolLabelTextColorIndex;
            }

            set
            {
                m_SectionSymbolLabelTextColorIndex = value;
                SectionSymbolLabelTextColor = CComboBoxHelper.ColorList[m_SectionSymbolLabelTextColorIndex].Color.Value;

                NotifyPropertyChanged("SectionSymbolLabelTextColorIndex");
            }
        }
        public int SectionSymbolColorIndex
        {
            get
            {
                return m_SectionSymbolColorIndex;
            }

            set
            {
                m_SectionSymbolColorIndex = value;
                
                SectionSymbolColor = CComboBoxHelper.ColorList[m_SectionSymbolColorIndex].Color.Value;

                NotifyPropertyChanged("SectionSymbolColorIndex");
            }
        }

        public int DetailSymbolLabelTextColorIndex
        {
            get
            {
                return m_DetailSymbolLabelTextColorIndex;
            }

            set
            {
                m_DetailSymbolLabelTextColorIndex = value;
                
                DetailSymbolLabelTextColor = CComboBoxHelper.ColorList[m_DetailSymbolLabelTextColorIndex].Color.Value;

                NotifyPropertyChanged("DetailSymbolLabelTextColorIndex");
            }
        }
        public int DetailSymbolLabelBackColorIndex
        {
            get
            {
                return m_DetailSymbolLabelBackColorIndex;
            }

            set
            {
                m_DetailSymbolLabelBackColorIndex = value;

                DetailSymbolLabelBackColor = CComboBoxHelper.ColorListWithTransparent[m_DetailSymbolLabelBackColorIndex].Color;

                NotifyPropertyChanged("DetailSymbolLabelBackColorIndex");
            }
        }

        public int DetailSymbolColorIndex
        {
            get
            {
                return m_DetailSymbolColorIndex;
            }

            set
            {
                m_DetailSymbolColorIndex = value;

                DetailSymbolColor = CComboBoxHelper.ColorList[m_DetailSymbolColorIndex].Color.Value;

                NotifyPropertyChanged("DetailSymbolColorIndex");
            }
        }

        public int SawCutTextColorIndex
        {
            get
            {
                return m_SawCutTextColorIndex;
            }

            set
            {
                m_SawCutTextColorIndex = value;
                
                SawCutTextColor = CComboBoxHelper.ColorList[m_SawCutTextColorIndex].Color.Value;

                NotifyPropertyChanged("SawCutTextColorIndex");
            }
        }

        public int SawCutLineColorIndex
        {
            get
            {
                return m_SawCutLineColorIndex;
            }

            set
            {
                m_SawCutLineColorIndex = value;

                SawCutLineColor = CComboBoxHelper.ColorList[m_SawCutLineColorIndex].Color.Value;

                NotifyPropertyChanged("SawCutLineColorIndex");
            }
        }

        public int ControlJointTextColorIndex
        {
            get
            {
                return m_ControlJointTextColorIndex;
            }

            set
            {
                m_ControlJointTextColorIndex = value;

                ControlJointTextColor = CComboBoxHelper.ColorList[m_ControlJointTextColorIndex].Color.Value;

                NotifyPropertyChanged("ControlJointTextColorIndex");
            }
        }

        public int ControlJointLineColorIndex
        {
            get
            {
                return m_ControlJointLineColorIndex;
            }

            set
            {
                m_ControlJointLineColorIndex = value;

                ControlJointLineColor = CComboBoxHelper.ColorList[m_ControlJointLineColorIndex].Color.Value;

                NotifyPropertyChanged("ControlJointLineColorIndex");
            }
        }

        public int FoundationTextColorIndex
        {
            get
            {
                return m_FoundationTextColorIndex;
            }

            set
            {
                m_FoundationTextColorIndex = value;

                FoundationTextColor = CComboBoxHelper.ColorList[m_FoundationTextColorIndex].Color.Value;

                NotifyPropertyChanged("FoundationTextColorIndex");
            }
        }

        public int FloorSlabTextColorIndex
        {
            get
            {
                return m_FloorSlabTextColorIndex;
            }

            set
            {
                m_FloorSlabTextColorIndex = value;
                
                FloorSlabTextColor = CComboBoxHelper.ColorList[m_FloorSlabTextColorIndex].Color.Value;

                NotifyPropertyChanged("FloorSlabTextColorIndex");
            }
        }

        public int FoundationColorIndex
        {
            get
            {
                return m_FoundationColorIndex;
            }

            set
            {
                m_FoundationColorIndex = value;

                FoundationColor = CComboBoxHelper.ColorList[m_FoundationColorIndex].Color.Value;

                NotifyPropertyChanged("FoundationColorIndex");
            }
        }

        public int FloorSlabColorIndex
        {
            get
            {
                return m_FloorSlabColorIndex;
            }

            set
            {
                m_FloorSlabColorIndex = value;
                
                FloorSlabColor = CComboBoxHelper.ColorList[m_FloorSlabColorIndex].Color.Value;

                NotifyPropertyChanged("FloorSlabColorIndex");
            }
        }

        public int SlabRebateColorIndex
        {
            get
            {
                return m_SlabRebateColorIndex;
            }

            set
            {
                m_SlabRebateColorIndex = value;

                SlabRebateColor = CComboBoxHelper.ColorList[m_SlabRebateColorIndex].Color.Value;

                NotifyPropertyChanged("SlabRebateColorIndex");
            }
        }

        public List<CComboColor> ColorList
        {
            get
            {
                return CComboBoxHelper.ColorList;
            }
        }
        public List<CComboColor> ColorListWithTransparent
        {
            get
            {
                return CComboBoxHelper.ColorListWithTransparent;
            }
        }
        public List<ComboItem> LinePatternTypes
        {
            get
            {
                return new List<ComboItem>() { new ComboItem((int)ELinePatternType.CONTINUOUS, "Continuous"),
                    new ComboItem((int)ELinePatternType.DASHED, "Dashed"),
                    new ComboItem((int)ELinePatternType.DOTTED, "Dotted"),
                    new ComboItem((int)ELinePatternType.DASHDOTTED, "Dashdotted"),
                    new ComboItem((int)ELinePatternType.DIVIDE, "Divide")
                };
            }
        }

        public int BackgroundColorIndex
        {
            get
            {
                return m_BackgroundColorIndex;
            }

            set
            {
                m_BackgroundColorIndex = value;

                BackgroundColor = CComboBoxHelper.ColorList[m_BackgroundColorIndex].Color.Value;

                NotifyPropertyChanged("BackgroundColorIndex");
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return m_BackgroundColor;
            }

            set
            {
                m_BackgroundColor = value;
                NotifyPropertyChanged("BackgroundColor");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public DisplayOptionsViewModel()
        {
            IsSetFromCode = true;

            WireframeColorIndex = CComboBoxHelper.GetColorIndex(Colors.CadetBlue);
            WireFrameLineThickness = 2;

            MemberCenterlineColorIndex = CComboBoxHelper.GetColorIndex(Colors.WhiteSmoke);
            MemberCenterlineThickness = 2;

            NodeDescriptionTextFontSize = 12;
            MemberDescriptionTextFontSize = 12;
            DimensionTextFontSize = 12;
            GridLineLabelTextFontSize = 30;
            SectionSymbolLabelTextFontSize = 30;
            DetailSymbolLabelTextFontSize = 30;

            SawCutTextFontSize = 12;
            ControlJointTextFontSize = 12;

            FoundationTextFontSize = 12;
            FloorSlabTextFontSize = 12;

            NodeColorIndex = CComboBoxHelper.GetColorIndex(Colors.Cyan);
            NodeDescriptionTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Cyan);
            MemberDescriptionTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Beige);
            DimensionTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightGreen);
            DimensionLineColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightGreen);

            GridLineLabelTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Coral);
            GridLineColorIndex = CComboBoxHelper.GetColorIndex(Colors.Coral);
            GridLinePatternType = (int)ELinePatternType.DASHDOTTED;

            SectionSymbolLabelTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Cyan);
            SectionSymbolColorIndex = CComboBoxHelper.GetColorIndex(Colors.Cyan);

            DetailSymbolLabelTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightPink);
            DetailSymbolLabelBackColorIndex = CComboBoxHelper.GetColorIndexWithTransparent(Colors.White);
            DetailSymbolColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightPink);

            SawCutTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Goldenrod);
            SawCutLineColorIndex = CComboBoxHelper.GetColorIndex(Colors.Goldenrod);
            SawCutLinePatternType = (int)ELinePatternType.DOTTED;

            ControlJointTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.BlueViolet);
            ControlJointLineColorIndex = CComboBoxHelper.GetColorIndex(Colors.BlueViolet);
            ControlJointLinePatternType = (int)ELinePatternType.DIVIDE;

            FoundationTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.HotPink);
            FloorSlabTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.HotPink);

            FoundationColorIndex = CComboBoxHelper.GetColorIndex(Colors.DarkGray);
            FloorSlabColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightGray);
            SlabRebateColorIndex = CComboBoxHelper.GetColorIndex(Colors.DarkOrange);

            MemberSolidModelOpacity = 0.8f;
            PlateSolidModelOpacity = 0.5f;
            ScrewSolidModelOpacity = 0.9f;
            AnchorSolidModelOpacity = 0.9f;
            FoundationSolidModelOpacity = 0.4f;
            ReinforcementBarSolidModelOpacity = 0.9f;
            FloorSlabSolidModelOpacity = 0.3f;
            SlabRebateSolidModelOpacity = 0.3f;

            BackgroundColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);

            IsSetFromCode = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}