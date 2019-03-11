using BaseClasses;
using BaseClasses.GraphObj;
using M_EC1.AS_NZS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PFD
{
    public class CSurfaceLoadGenerator
    {
        float fH1_frame;
        float fH2_frame;
        float fW_frame;
        float fL_tot;
        float fRoofPitch_rad;
        float fDist_Purlin;
        float fDist_Girt;
        float fDist_FrontGirts;
        float fDist_BackGirts;
        float fDist_FrontColumns;
        float fDist_BackColumns;
        float fSlopeFactor;
        private CLoadCase[] m_arrLoadCases;
        CCalcul_1170_1 generalLoad;
        CCalcul_1170_2 wind;
        CCalcul_1170_3 snow;

        #region public List Properties
        public List<CSLoad_Free> surfaceDeadLoad;
        public List<CSLoad_Free> surfaceRoofImposedLoad;
        public List<CSLoad_Free> surfaceRoofSnowLoad_ULS_Nu_1;
        public List<CSLoad_Free> surfaceRoofSnowLoad_ULS_Nu_2_Left;
        public List<CSLoad_Free> surfaceRoofSnowLoad_ULS_Nu_2_Right;
        public List<CSLoad_Free> surfaceWindLoad_ULS_PlusX_Cpimin;
        public List<CSLoad_Free> surfaceWindLoad_ULS_MinusX_Cpimin;
        public List<CSLoad_Free> surfaceWindLoad_ULS_PlusY_Cpimin;
        public List<CSLoad_Free> surfaceWindLoad_ULS_MinusY_Cpimin;
        public List<CSLoad_Free> surfaceWindLoad_ULS_PlusX_Cpimax;
        public List<CSLoad_Free> surfaceWindLoad_ULS_MinusX_Cpimax;
        public List<CSLoad_Free> surfaceWindLoad_ULS_PlusY_Cpimax;
        public List<CSLoad_Free> surfaceWindLoad_ULS_MinusY_Cpimax;
        public List<CSLoad_Free> surfaceWindLoad_ULS_PlusX_Cpemin;
        public List<CSLoad_Free> surfaceWindLoad_ULS_MinusX_Cpemin;
        public List<CSLoad_Free> surfaceWindLoad_ULS_PlusY_Cpemin;
        public List<CSLoad_Free> surfaceWindLoad_ULS_MinusY_Cpemin;
        public List<CSLoad_Free> surfaceWindLoad_ULS_PlusX_Cpemax;
        public List<CSLoad_Free> surfaceWindLoad_ULS_MinusX_Cpemax;
        public List<CSLoad_Free> surfaceWindLoad_ULS_PlusY_Cpemax;
        public List<CSLoad_Free> surfaceWindLoad_ULS_MinusY_Cpemax;

        public List<CSLoad_Free> surfaceRoofSnowLoad_SLS_Nu_1;
        public List<CSLoad_Free> surfaceRoofSnowLoad_SLS_Nu_2_Left;
        public List<CSLoad_Free> surfaceRoofSnowLoad_SLS_Nu_2_Right;
        public List<CSLoad_Free> surfaceWindLoad_SLS_PlusX_Cpimin;
        public List<CSLoad_Free> surfaceWindLoad_SLS_MinusX_Cpimin;
        public List<CSLoad_Free> surfaceWindLoad_SLS_PlusY_Cpimin;
        public List<CSLoad_Free> surfaceWindLoad_SLS_MinusY_Cpimin;
        public List<CSLoad_Free> surfaceWindLoad_SLS_PlusX_Cpimax;
        public List<CSLoad_Free> surfaceWindLoad_SLS_MinusX_Cpimax;
        public List<CSLoad_Free> surfaceWindLoad_SLS_PlusY_Cpimax;
        public List<CSLoad_Free> surfaceWindLoad_SLS_MinusY_Cpimax;
        public List<CSLoad_Free> surfaceWindLoad_SLS_PlusX_Cpemin;
        public List<CSLoad_Free> surfaceWindLoad_SLS_MinusX_Cpemin;
        public List<CSLoad_Free> surfaceWindLoad_SLS_PlusY_Cpemin;
        public List<CSLoad_Free> surfaceWindLoad_SLS_MinusY_Cpemin;
        public List<CSLoad_Free> surfaceWindLoad_SLS_PlusX_Cpemax;
        public List<CSLoad_Free> surfaceWindLoad_SLS_MinusX_Cpemax;
        public List<CSLoad_Free> surfaceWindLoad_SLS_PlusY_Cpemax;
        public List<CSLoad_Free> surfaceWindLoad_SLS_MinusY_Cpemax;
        #endregion

        public CSurfaceLoadGenerator(float H1_frame, float H2_frame,
            float W_frame, float L_tot, float RoofPitch_rad, 
            float Dist_Purlin,
            float Dist_Girt,
            float Dist_FrontGirts,
            float Dist_BackGirts,
            float Dist_FrontColumns,
            float Dist_BackColumns,
            float SlopeFactor,
            CLoadCase[] arrLoadCases,
            CCalcul_1170_1 calc_generalLoad,
            CCalcul_1170_2 calc_wind,
            CCalcul_1170_3 calc_snow)
        {
            fH1_frame = H1_frame;
            fH2_frame = H2_frame;
            fW_frame = W_frame;
            fL_tot = L_tot;
            fRoofPitch_rad = RoofPitch_rad;
            fDist_Purlin = Dist_Purlin;
            fDist_Girt = Dist_Girt;
            fDist_FrontGirts = Dist_FrontGirts;
            fDist_BackGirts = Dist_BackGirts;
            fDist_FrontColumns = Dist_FrontColumns;
            fDist_BackColumns = Dist_BackColumns;
            fSlopeFactor = SlopeFactor;
            m_arrLoadCases = arrLoadCases;
            wind = calc_wind;
            snow = calc_snow;
            generalLoad = calc_generalLoad;
        }

        public void GenerateSurfaceLoads()
        {
            // Surface Free Loads
            // Roof Surface Geometry
            // Control Points
            CPoint pRoofFrontLeft = new CPoint(0, 0, 0, fH1_frame, 0);
            CPoint pRoofFrontApex = new CPoint(0, 0.5f * fW_frame, 0, fH2_frame, 0);
            CPoint pRoofFrontRight = new CPoint(0, fW_frame, 0, fH1_frame, 0);
            CPoint pRoofBackLeft = new CPoint(0, 0, fL_tot, fH1_frame, 0);
            CPoint pRoofBackApex = new CPoint(0, 0.5f * fW_frame, fL_tot, fH2_frame, 0);
            CPoint pRoofBackRight = new CPoint(0, fW_frame, fL_tot, fH1_frame, 0);

            // Dimensions
            float fRoof_X = fL_tot;
            float fRoof_Y = 0.5f * fW_frame / (float)Math.Cos(fRoofPitch_rad);

            // Wall Surface Geometry
            // Control Point
            CPoint pWallFrontLeft = new CPoint(0, 0, 0, 0, 0);
            CPoint pWallFrontRight = new CPoint(0, fW_frame, 0, 0, 0);
            CPoint pWallBackRight = new CPoint(0, fW_frame, fL_tot, 0, 0);
            CPoint pWallBackLeft = new CPoint(0, 0, fL_tot, 0, 0);

            // Dimensions
            float fWallLeftOrRight_X = fL_tot;
            float fWallLeftOrRight_Y = fH1_frame;

            // Dimensions
            float fWallFrontOrBack_X = fW_frame;
            float fWallFrontOrBack_Y1 = fH1_frame;
            float fWallFrontOrBack_Y2 = fH2_frame;

            // Types and loading widths of loaded members under free surface loads
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataRoof = new List<FreeSurfaceLoadsMemberTypeData>(2);
            listOfLoadedMemberTypeDataRoof.Add(new FreeSurfaceLoadsMemberTypeData(EMemberType_FS.eP, fDist_Purlin));
            listOfLoadedMemberTypeDataRoof.Add(new FreeSurfaceLoadsMemberTypeData(EMemberType_FS.eEP, 0.5f * fDist_Purlin));
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallLeftRight = new List<FreeSurfaceLoadsMemberTypeData>(2);
            listOfLoadedMemberTypeDataWallLeftRight.Add(new FreeSurfaceLoadsMemberTypeData(EMemberType_FS.eG, fDist_Girt));
            listOfLoadedMemberTypeDataWallLeftRight.Add(new FreeSurfaceLoadsMemberTypeData(EMemberType_FS.eEP, 0.5f * fDist_Girt));
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallFront = new List<FreeSurfaceLoadsMemberTypeData>(1);
            listOfLoadedMemberTypeDataWallFront.Add(new FreeSurfaceLoadsMemberTypeData(EMemberType_FS.eG, fDist_FrontGirts));
            listOfLoadedMemberTypeDataWallFront.Add(new FreeSurfaceLoadsMemberTypeData(EMemberType_FS.eC, fDist_FrontColumns));
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallBack = new List<FreeSurfaceLoadsMemberTypeData>(1);
            listOfLoadedMemberTypeDataWallBack.Add(new FreeSurfaceLoadsMemberTypeData(EMemberType_FS.eG, fDist_BackGirts));
            listOfLoadedMemberTypeDataWallBack.Add(new FreeSurfaceLoadsMemberTypeData(EMemberType_FS.eG, fDist_BackColumns));

            // Hodnota zatazenia v smere kladnej osi je kladna, hodnota zatazenia v smere zapornej osi je zaporna
            // Permanent load
            surfaceDeadLoad = new List<CSLoad_Free>(6);
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDirection.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, -generalLoad.fDeadLoadTotal_Roof, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, true, true, 0));
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDirection.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, -generalLoad.fDeadLoadTotal_Roof, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, true, true, 0));
            // Docasne zadavame dead load v lokalnom smere y plochy, kym nebude doriesena transformacia zatazenia zadaneho v GCS
            ELoadCoordSystem lcsPL = ELoadCoordSystem.eLCS; // ELoadCoordSystem.eGCS
            ELoadDirection ldirPL = ELoadDirection.eLD_Y; // ELoadDirection.eLD_Z

            surfaceDeadLoad.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, lcsPL, ldirPL, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, -generalLoad.fDeadLoadTotal_Wall, 90, 0, 90, Colors.DeepPink, true, true, true, 0));
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, lcsPL, ldirPL, pWallBackRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, -generalLoad.fDeadLoadTotal_Wall, 90, 0, 180 + 90, Colors.DeepPink, true, true, true, 0));
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallFront, lcsPL, ldirPL, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, -generalLoad.fDeadLoadTotal_Wall, 90, 0, 0, Colors.DeepPink, false, true, false, true, 0));
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallBack, lcsPL, ldirPL, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, -generalLoad.fDeadLoadTotal_Wall, 90, 0, 180, Colors.DeepPink, false, true, false, true, 0));

            // Imposed Load - Roof
            surfaceRoofImposedLoad = new List<CSLoad_Free>(2);
            surfaceRoofImposedLoad.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDirection.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, -generalLoad.fImposedLoadTotal_Roof, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.Red, false, true, true, 0));
            surfaceRoofImposedLoad.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDirection.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, -generalLoad.fImposedLoadTotal_Roof, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.Red, false, true, true, 0));

            // Snow Load - Roof
            float fsnowULS_Nu_1 = -snow.fs_ULS_Nu_1 * fSlopeFactor; // Design value (projection on roof)
            float fsnowULS_Nu_2 = -snow.fs_ULS_Nu_2 * fSlopeFactor;
            float fsnowSLS_Nu_1 = -snow.fs_SLS_Nu_1 * fSlopeFactor;
            float fsnowSLS_Nu_2 = -snow.fs_SLS_Nu_2 * fSlopeFactor;

            surfaceRoofSnowLoad_ULS_Nu_1 = new List<CSLoad_Free>(2);
            surfaceRoofSnowLoad_ULS_Nu_1.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDirection.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fsnowULS_Nu_1, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, false, true, true, 0));
            surfaceRoofSnowLoad_ULS_Nu_1.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDirection.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fsnowULS_Nu_1, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, false, true, true, 0));
            surfaceRoofSnowLoad_ULS_Nu_2_Left = new List<CSLoad_Free>(1);
            surfaceRoofSnowLoad_ULS_Nu_2_Left.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDirection.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fsnowULS_Nu_2, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, false, true, true, 0));
            surfaceRoofSnowLoad_ULS_Nu_2_Right = new List<CSLoad_Free>(1);
            surfaceRoofSnowLoad_ULS_Nu_2_Right.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDirection.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fsnowULS_Nu_2, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, false, true, true, 0));

            surfaceRoofSnowLoad_SLS_Nu_1 = new List<CSLoad_Free>(2);
            surfaceRoofSnowLoad_SLS_Nu_1.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDirection.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fsnowSLS_Nu_1, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, false, true, true, 0));
            surfaceRoofSnowLoad_SLS_Nu_1.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDirection.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fsnowSLS_Nu_1, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, false, true, true, 0));
            surfaceRoofSnowLoad_SLS_Nu_2_Left = new List<CSLoad_Free>(1);
            surfaceRoofSnowLoad_SLS_Nu_2_Left.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDirection.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fsnowSLS_Nu_2, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, false, true, true, 0));
            surfaceRoofSnowLoad_SLS_Nu_2_Right = new List<CSLoad_Free>(1);
            surfaceRoofSnowLoad_SLS_Nu_2_Right.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDirection.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fsnowSLS_Nu_2, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, false, true, true, 0));

            // Wind Load
            // Internal pressure
            // ULS
            // Cpi, min
            surfaceWindLoad_ULS_PlusX_Cpimin = new List<CSLoad_Free>(6);
            surfaceWindLoad_ULS_MinusX_Cpimin = new List<CSLoad_Free>(6);
            surfaceWindLoad_ULS_PlusY_Cpimin = new List<CSLoad_Free>(6);
            surfaceWindLoad_ULS_MinusY_Cpimin = new List<CSLoad_Free>(6);

            // Cpi, max
            surfaceWindLoad_ULS_PlusX_Cpimax = new List<CSLoad_Free>(6);
            surfaceWindLoad_ULS_MinusX_Cpimax = new List<CSLoad_Free>(6);
            surfaceWindLoad_ULS_PlusY_Cpimax = new List<CSLoad_Free>(6);
            surfaceWindLoad_ULS_MinusY_Cpimax = new List<CSLoad_Free>(6);

            SetSurfaceWindLoads_Cpi(
                listOfLoadedMemberTypeDataRoof,
                listOfLoadedMemberTypeDataWallLeftRight,
                listOfLoadedMemberTypeDataWallFront,
                listOfLoadedMemberTypeDataWallBack,
                ELSType.eLS_ULS,
                pRoofFrontApex,
                pRoofFrontRight,
                fRoof_X,
                fRoof_Y,
                pWallFrontLeft,
                pWallBackRight,
                fWallLeftOrRight_X,
                fWallLeftOrRight_Y,
                fWallFrontOrBack_X,
                fWallFrontOrBack_Y1,
                fWallFrontOrBack_Y2,
                wind,
                out surfaceWindLoad_ULS_PlusX_Cpimin,
                out surfaceWindLoad_ULS_MinusX_Cpimin,
                out surfaceWindLoad_ULS_PlusY_Cpimin,
                out surfaceWindLoad_ULS_MinusY_Cpimin,
                out surfaceWindLoad_ULS_PlusX_Cpimax,
                out surfaceWindLoad_ULS_MinusX_Cpimax,
                out surfaceWindLoad_ULS_PlusY_Cpimax,
                out surfaceWindLoad_ULS_MinusY_Cpimax
            );

            // SLS
            // Cpi, min
            surfaceWindLoad_SLS_PlusX_Cpimin = new List<CSLoad_Free>(6);
            surfaceWindLoad_SLS_MinusX_Cpimin = new List<CSLoad_Free>(6);
            surfaceWindLoad_SLS_PlusY_Cpimin = new List<CSLoad_Free>(6);
            surfaceWindLoad_SLS_MinusY_Cpimin = new List<CSLoad_Free>(6);

            // Cpi, max
            surfaceWindLoad_SLS_PlusX_Cpimax = new List<CSLoad_Free>(6);
            surfaceWindLoad_SLS_MinusX_Cpimax = new List<CSLoad_Free>(6);
            surfaceWindLoad_SLS_PlusY_Cpimax = new List<CSLoad_Free>(6);
            surfaceWindLoad_SLS_MinusY_Cpimax = new List<CSLoad_Free>(6);

            SetSurfaceWindLoads_Cpi(
                listOfLoadedMemberTypeDataRoof,
                listOfLoadedMemberTypeDataWallLeftRight,
                listOfLoadedMemberTypeDataWallFront,
                listOfLoadedMemberTypeDataWallBack,
                ELSType.eLS_SLS,
                pRoofFrontApex,
                pRoofFrontRight,
                fRoof_X,
                fRoof_Y,
                pWallFrontLeft,
                pWallBackRight,
                fWallLeftOrRight_X,
                fWallLeftOrRight_Y,
                fWallFrontOrBack_X,
                fWallFrontOrBack_Y1,
                fWallFrontOrBack_Y2,
                wind,
                out surfaceWindLoad_SLS_PlusX_Cpimin,
                out surfaceWindLoad_SLS_MinusX_Cpimin,
                out surfaceWindLoad_SLS_PlusY_Cpimin,
                out surfaceWindLoad_SLS_MinusY_Cpimin,
                out surfaceWindLoad_SLS_PlusX_Cpimax,
                out surfaceWindLoad_SLS_MinusX_Cpimax,
                out surfaceWindLoad_SLS_PlusY_Cpimax,
                out surfaceWindLoad_SLS_MinusY_Cpimax
            );

            // External presssure
            // ULS
            // Cpe, min
            surfaceWindLoad_ULS_PlusX_Cpemin = new List<CSLoad_Free>(6);
            surfaceWindLoad_ULS_MinusX_Cpemin = new List<CSLoad_Free>(6);
            surfaceWindLoad_ULS_PlusY_Cpemin = new List<CSLoad_Free>(6);
            surfaceWindLoad_ULS_MinusY_Cpemin = new List<CSLoad_Free>(6);

            // Cpe, max
            surfaceWindLoad_ULS_PlusX_Cpemax = new List<CSLoad_Free>(6);
            surfaceWindLoad_ULS_MinusX_Cpemax = new List<CSLoad_Free>(6);
            surfaceWindLoad_ULS_PlusY_Cpemax = new List<CSLoad_Free>(6);
            surfaceWindLoad_ULS_MinusY_Cpemax = new List<CSLoad_Free>(6);

            SetSurfaceWindLoads_Cpe(
                listOfLoadedMemberTypeDataRoof,
                listOfLoadedMemberTypeDataWallLeftRight,
                listOfLoadedMemberTypeDataWallFront,
                listOfLoadedMemberTypeDataWallBack,
                ELSType.eLS_ULS,
                pRoofFrontLeft,
                pRoofFrontApex,
                pRoofFrontRight,
                pRoofBackLeft,
                pRoofBackApex,
                pRoofBackRight,
                fRoof_X,
                fRoof_Y,
                pWallFrontLeft,
                pWallFrontRight,
                pWallBackRight,
                pWallBackLeft,
                fWallLeftOrRight_X,
                fWallLeftOrRight_Y,
                fWallFrontOrBack_X,
                fWallFrontOrBack_Y1,
                fWallFrontOrBack_Y2,
                wind,
                out surfaceWindLoad_ULS_PlusX_Cpemin,
                out surfaceWindLoad_ULS_MinusX_Cpemin,
                out surfaceWindLoad_ULS_PlusY_Cpemin,
                out surfaceWindLoad_ULS_MinusY_Cpemin,
                out surfaceWindLoad_ULS_PlusX_Cpemax,
                out surfaceWindLoad_ULS_MinusX_Cpemax,
                out surfaceWindLoad_ULS_PlusY_Cpemax,
                out surfaceWindLoad_ULS_MinusY_Cpemax
            );

            // SLS
            // Cpe, min
            surfaceWindLoad_SLS_PlusX_Cpemin = new List<CSLoad_Free>(6);
            surfaceWindLoad_SLS_MinusX_Cpemin = new List<CSLoad_Free>(6);
            surfaceWindLoad_SLS_PlusY_Cpemin = new List<CSLoad_Free>(6);
            surfaceWindLoad_SLS_MinusY_Cpemin = new List<CSLoad_Free>(6);

            // Cpe, max
            surfaceWindLoad_SLS_PlusX_Cpemax = new List<CSLoad_Free>(6);
            surfaceWindLoad_SLS_MinusX_Cpemax = new List<CSLoad_Free>(6);
            surfaceWindLoad_SLS_PlusY_Cpemax = new List<CSLoad_Free>(6);
            surfaceWindLoad_SLS_MinusY_Cpemax = new List<CSLoad_Free>(6);

            SetSurfaceWindLoads_Cpe(
                listOfLoadedMemberTypeDataRoof,
                listOfLoadedMemberTypeDataWallLeftRight,
                listOfLoadedMemberTypeDataWallFront,
                listOfLoadedMemberTypeDataWallBack,
                ELSType.eLS_SLS,
                pRoofFrontLeft,
                pRoofFrontApex,
                pRoofFrontRight,
                pRoofBackLeft,
                pRoofBackApex,
                pRoofBackRight,
                fRoof_X,
                fRoof_Y,
                pWallFrontLeft,
                pWallFrontRight,
                pWallBackRight,
                pWallBackLeft,
                fWallLeftOrRight_X,
                fWallLeftOrRight_Y,
                fWallFrontOrBack_X,
                fWallFrontOrBack_Y1,
                fWallFrontOrBack_Y2,
                wind,
                out surfaceWindLoad_SLS_PlusX_Cpemin,
                out surfaceWindLoad_SLS_MinusX_Cpemin,
                out surfaceWindLoad_SLS_PlusY_Cpemin,
                out surfaceWindLoad_SLS_MinusY_Cpemin,
                out surfaceWindLoad_SLS_PlusX_Cpemax,
                out surfaceWindLoad_SLS_MinusX_Cpemax,
                out surfaceWindLoad_SLS_PlusY_Cpemax,
                out surfaceWindLoad_SLS_MinusY_Cpemax
            );
            
            // Assign generated member loads to the load cases
            // Universal
            m_arrLoadCases[(int)ELCName.eDL_G].SurfaceLoadsList = surfaceDeadLoad;
            m_arrLoadCases[(int)ELCName.eIL_Q].SurfaceLoadsList = surfaceRoofImposedLoad;

            // ULS
            m_arrLoadCases[(int)ELCName.eSL_Su_Full].SurfaceLoadsList = surfaceRoofSnowLoad_ULS_Nu_1;
            m_arrLoadCases[(int)ELCName.eSL_Su_Left].SurfaceLoadsList = surfaceRoofSnowLoad_ULS_Nu_2_Left;
            m_arrLoadCases[(int)ELCName.eSL_Su_Right].SurfaceLoadsList = surfaceRoofSnowLoad_ULS_Nu_2_Right;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_min_Left_X_Plus].SurfaceLoadsList = surfaceWindLoad_ULS_PlusX_Cpimin;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_min_Right_X_Minus].SurfaceLoadsList = surfaceWindLoad_ULS_MinusX_Cpimin;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_min_Front_Y_Plus].SurfaceLoadsList = surfaceWindLoad_ULS_PlusY_Cpimin;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_min_Rear_Y_Minus].SurfaceLoadsList = surfaceWindLoad_ULS_MinusY_Cpimin;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_max_Left_X_Plus].SurfaceLoadsList = surfaceWindLoad_ULS_PlusX_Cpimax;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_max_Right_X_Minus].SurfaceLoadsList = surfaceWindLoad_ULS_MinusX_Cpimax;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_max_Front_Y_Plus].SurfaceLoadsList = surfaceWindLoad_ULS_PlusY_Cpimax;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_max_Rear_Y_Minus].SurfaceLoadsList = surfaceWindLoad_ULS_MinusY_Cpimax;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_min_Left_X_Plus].SurfaceLoadsList = surfaceWindLoad_ULS_PlusX_Cpemin; 
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_min_Right_X_Minus].SurfaceLoadsList = surfaceWindLoad_ULS_MinusX_Cpemin;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_min_Front_Y_Plus].SurfaceLoadsList = surfaceWindLoad_ULS_PlusY_Cpemin;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_min_Rear_Y_Minus].SurfaceLoadsList = surfaceWindLoad_ULS_MinusY_Cpemin;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_max_Left_X_Plus].SurfaceLoadsList = surfaceWindLoad_ULS_PlusX_Cpemax; 
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_max_Right_X_Minus].SurfaceLoadsList = surfaceWindLoad_ULS_MinusX_Cpemax;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_max_Front_Y_Plus].SurfaceLoadsList = surfaceWindLoad_ULS_PlusY_Cpemax;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_max_Rear_Y_Minus].SurfaceLoadsList = surfaceWindLoad_ULS_MinusY_Cpemax;
            // SLS
            m_arrLoadCases[(int)ELCName.eSL_Ss_Full].SurfaceLoadsList = surfaceRoofSnowLoad_SLS_Nu_1;
            m_arrLoadCases[(int)ELCName.eSL_Ss_Left].SurfaceLoadsList = surfaceRoofSnowLoad_SLS_Nu_2_Left;
            m_arrLoadCases[(int)ELCName.eSL_Ss_Right].SurfaceLoadsList = surfaceRoofSnowLoad_SLS_Nu_2_Right;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_min_Left_X_Plus].SurfaceLoadsList = surfaceWindLoad_SLS_PlusX_Cpimin;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_min_Right_X_Minus].SurfaceLoadsList = surfaceWindLoad_SLS_MinusX_Cpimin;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_min_Front_Y_Plus].SurfaceLoadsList = surfaceWindLoad_SLS_PlusY_Cpimin;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_min_Rear_Y_Minus].SurfaceLoadsList = surfaceWindLoad_SLS_MinusY_Cpimin;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_max_Left_X_Plus].SurfaceLoadsList = surfaceWindLoad_SLS_PlusX_Cpimax;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_max_Right_X_Minus].SurfaceLoadsList = surfaceWindLoad_SLS_MinusX_Cpimax;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_max_Front_Y_Plus].SurfaceLoadsList = surfaceWindLoad_SLS_PlusY_Cpimax;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_max_Rear_Y_Minus].SurfaceLoadsList = surfaceWindLoad_SLS_MinusY_Cpimax;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_min_Left_X_Plus].SurfaceLoadsList = surfaceWindLoad_SLS_PlusX_Cpemin; 
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_min_Right_X_Minus].SurfaceLoadsList = surfaceWindLoad_SLS_MinusX_Cpemin;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_min_Front_Y_Plus].SurfaceLoadsList = surfaceWindLoad_SLS_PlusY_Cpemin;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_min_Rear_Y_Minus].SurfaceLoadsList = surfaceWindLoad_SLS_MinusY_Cpemin;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_max_Left_X_Plus].SurfaceLoadsList = surfaceWindLoad_SLS_PlusX_Cpemax; 
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_max_Right_X_Minus].SurfaceLoadsList = surfaceWindLoad_SLS_MinusX_Cpemax;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_max_Front_Y_Plus].SurfaceLoadsList = surfaceWindLoad_SLS_PlusY_Cpemax;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_max_Rear_Y_Minus].SurfaceLoadsList = surfaceWindLoad_SLS_MinusY_Cpemax;
        }

        // Surface Loads
        private void SetSurfaceWindLoads_Cpi(
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataRoof,
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallLeftRight,
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallFront,
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallBack,
            ELSType eLSType,
            CPoint pRoofFrontApex,
            CPoint pRoofFrontRight,
            float fRoof_X,
            float fRoof_Y,
            CPoint pWallFrontLeft,
            CPoint pWallBackRight,
            float fWallLeftOrRight_X,
            float fWallLeftOrRight_Y,
            float fWallFrontOrBack_X,
            float fWallFrontOrBack_Y1,
            float fWallFrontOrBack_Y2,
            CCalcul_1170_2 wind,
            out List<CSLoad_Free> surfaceWindLoadPlusX_Cpimin,
            out List<CSLoad_Free> surfaceWindLoadMinusX_Cpimin,
            out List<CSLoad_Free> surfaceWindLoadPlusY_Cpimin,
            out List<CSLoad_Free> surfaceWindLoadMinusY_Cpimin,
            out List<CSLoad_Free> surfaceWindLoadPlusX_Cpimax,
            out List<CSLoad_Free> surfaceWindLoadMinusX_Cpimax,
            out List<CSLoad_Free> surfaceWindLoadPlusY_Cpimax,
            out List<CSLoad_Free> surfaceWindLoadMinusY_Cpimax
        )
        {
            // Cpi, min (underpressure, negative air pressure)
            SetSurfaceWindLoads_Cpi(
                listOfLoadedMemberTypeDataRoof,
                listOfLoadedMemberTypeDataWallLeftRight,
                listOfLoadedMemberTypeDataWallFront,
                listOfLoadedMemberTypeDataWallBack,
                eLSType,
                0,
                pRoofFrontApex,
                pRoofFrontRight,
                fRoof_X,
                fRoof_Y,
                pWallFrontLeft,
                pWallBackRight,
                fWallLeftOrRight_X,
                fWallLeftOrRight_Y,
                fWallFrontOrBack_X,
                fWallFrontOrBack_Y1,
                fWallFrontOrBack_Y2,
                wind,
                out surfaceWindLoadPlusX_Cpimin,
                out surfaceWindLoadMinusX_Cpimin,
                out surfaceWindLoadPlusY_Cpimin,
                out surfaceWindLoadMinusY_Cpimin
            );

            // Cpi, max (overpressure, possitive air pressure)
            SetSurfaceWindLoads_Cpi(
                listOfLoadedMemberTypeDataRoof,
                listOfLoadedMemberTypeDataWallLeftRight,
                listOfLoadedMemberTypeDataWallFront,
                listOfLoadedMemberTypeDataWallBack,
                eLSType,
                1,
                pRoofFrontApex,
                pRoofFrontRight,
                fRoof_X,
                fRoof_Y,
                pWallFrontLeft,
                pWallBackRight,
                fWallLeftOrRight_X,
                fWallLeftOrRight_Y,
                fWallFrontOrBack_X,
                fWallFrontOrBack_Y1,
                fWallFrontOrBack_Y2,
                wind,
                out surfaceWindLoadPlusX_Cpimax,
                out surfaceWindLoadMinusX_Cpimax,
                out surfaceWindLoadPlusY_Cpimax,
                out surfaceWindLoadMinusY_Cpimax
            );
        }

        private void SetSurfaceWindLoads_Cpi(
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataRoof,
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallLeftRight,
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallFront,
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallBack,
            ELSType eLSType,
            int iCodeForCpiMinMaxValue,
            CPoint pRoofFrontApex,
            CPoint pRoofFrontRight,
            float fRoof_X,
            float fRoof_Y,
            CPoint pWallFrontLeft,
            CPoint pWallBackRight,
            float fWallLeftOrRight_X,
            float fWallLeftOrRight_Y,
            float fWallFrontOrBack_X,
            float fWallFrontOrBack_Y1,
            float fWallFrontOrBack_Y2,
            CCalcul_1170_2 wind,
            out List<CSLoad_Free> surfaceWindLoadPlusX_Cpi,
            out List<CSLoad_Free> surfaceWindLoadMinusX_Cpi,
            out List<CSLoad_Free> surfaceWindLoadPlusY_Cpi,
            out List<CSLoad_Free> surfaceWindLoadMinusY_Cpi
        )
        {
            // Wind Load
            Color cColorWindWalls = Colors.Cyan;

            float[] fp_i_roof_Theta_4;

            float[] fp_i_W_wall_Theta_4;
            float[] fp_i_wall_Theta_4;

            if (eLSType == ELSType.eLS_ULS)
            {
                if (iCodeForCpiMinMaxValue == 0) // ULS - Cpi,min
                {
                    // Roof
                    fp_i_roof_Theta_4 = wind.fp_i_min_ULS_Theta_4;

                    // Walls
                    fp_i_W_wall_Theta_4 = wind.fp_i_min_ULS_Theta_4;
                    fp_i_wall_Theta_4 = wind.fp_i_min_ULS_Theta_4;
                }
                else // ULS - Cpi,max
                {
                    // Roof
                    fp_i_roof_Theta_4 = wind.fp_i_max_ULS_Theta_4;

                    // Walls
                    fp_i_W_wall_Theta_4 = wind.fp_i_max_ULS_Theta_4;
                    fp_i_wall_Theta_4 = wind.fp_i_max_ULS_Theta_4;
                }
            }
            else
            {
                if (iCodeForCpiMinMaxValue == 0)  // SLS - Cpi,min
                {
                    // Roof
                    fp_i_roof_Theta_4 = wind.fp_i_min_SLS_Theta_4;

                    // Walls
                    fp_i_W_wall_Theta_4 = wind.fp_i_min_SLS_Theta_4;
                    fp_i_wall_Theta_4 = wind.fp_i_min_SLS_Theta_4;
                }
                else  // SLS - Cpi,max
                {
                    // Roof
                    fp_i_roof_Theta_4 = wind.fp_i_max_SLS_Theta_4;

                    // Walls
                    fp_i_W_wall_Theta_4 = wind.fp_i_max_SLS_Theta_4;
                    fp_i_wall_Theta_4 = wind.fp_i_max_SLS_Theta_4;
                }
            }

            // Cpi
            surfaceWindLoadPlusX_Cpi = new List<CSLoad_Free>(6);
            surfaceWindLoadPlusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fp_i_roof_Theta_4[(int)ELCMainDirection.ePlusX], -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadPlusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fp_i_roof_Theta_4[(int)ELCMainDirection.ePlusX], fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadPlusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, -fp_i_W_wall_Theta_4[(int)ELCMainDirection.ePlusX], 90, 0, 90, cColorWindWalls, false, true, true, 0));
            surfaceWindLoadPlusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallBackRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, -fp_i_wall_Theta_4[(int)ELCMainDirection.ePlusX], 90, 0, 180 + 90, cColorWindWalls, false, true, true, 0)); // Right side
            surfaceWindLoadPlusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, fp_i_wall_Theta_4[(int)ELCMainDirection.ePlusX], 90, 0, 0, cColorWindWalls, false, false, false, true, 0));
            surfaceWindLoadPlusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, fp_i_wall_Theta_4[(int)ELCMainDirection.ePlusX], 90, 0, 180, cColorWindWalls, true, false, false, true, 0));

            surfaceWindLoadMinusX_Cpi = new List<CSLoad_Free>(6);
            surfaceWindLoadMinusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fp_i_roof_Theta_4[(int)ELCMainDirection.eMinusX], -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadMinusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fp_i_roof_Theta_4[(int)ELCMainDirection.eMinusX], fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadMinusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, -fp_i_wall_Theta_4[(int)ELCMainDirection.eMinusX], 90, 0, 90, cColorWindWalls, false, true, true, 0));
            surfaceWindLoadMinusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallBackRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, -fp_i_W_wall_Theta_4[(int)ELCMainDirection.eMinusX], 90, 0, 180 + 90, cColorWindWalls, false, true, true, 0));
            surfaceWindLoadMinusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, fp_i_wall_Theta_4[(int)ELCMainDirection.eMinusX], 90, 0, 0, cColorWindWalls, false, false, false, true, 0));
            surfaceWindLoadMinusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, fp_i_wall_Theta_4[(int)ELCMainDirection.eMinusX], 90, 0, 180, cColorWindWalls, true, false, false, true, 0));

            surfaceWindLoadPlusY_Cpi = new List<CSLoad_Free>(6);
            surfaceWindLoadPlusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fp_i_roof_Theta_4[(int)ELCMainDirection.ePlusY], -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadPlusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fp_i_roof_Theta_4[(int)ELCMainDirection.ePlusY], fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadPlusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, -fp_i_wall_Theta_4[(int)ELCMainDirection.ePlusY], 90, 0, 90, cColorWindWalls, false, true, true, 0));
            surfaceWindLoadPlusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallBackRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, -fp_i_wall_Theta_4[(int)ELCMainDirection.ePlusY], 90, 0, 180 + 90, cColorWindWalls, false, true, true, 0));
            surfaceWindLoadPlusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, fp_i_W_wall_Theta_4[(int)ELCMainDirection.ePlusY], 90, 0, 0, cColorWindWalls, false, false, false, true, 0));
            surfaceWindLoadPlusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, fp_i_wall_Theta_4[(int)ELCMainDirection.ePlusY], 90, 0, 180, cColorWindWalls, true, false, false, true, 0));

            surfaceWindLoadMinusY_Cpi = new List<CSLoad_Free>(6);
            surfaceWindLoadMinusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fp_i_roof_Theta_4[(int)ELCMainDirection.eMinusY], -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadMinusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fp_i_roof_Theta_4[(int)ELCMainDirection.eMinusY], fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadMinusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, -fp_i_wall_Theta_4[(int)ELCMainDirection.eMinusY], 90, 0, 90, cColorWindWalls, false, true, true, 0));
            surfaceWindLoadMinusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallBackRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, -fp_i_wall_Theta_4[(int)ELCMainDirection.eMinusY], 90, 0, 180 + 90, cColorWindWalls, false, true, true, 0));
            surfaceWindLoadMinusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, fp_i_wall_Theta_4[(int)ELCMainDirection.eMinusY], 90, 0, 0, cColorWindWalls, false, false, false, true, 0));
            surfaceWindLoadMinusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, fp_i_W_wall_Theta_4[(int)ELCMainDirection.eMinusY], 90, 0, 180, cColorWindWalls, true, false, false, true, 0));
        }

        private void SetSurfaceWindLoads_Cpe(
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataRoof,
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallLeftRight,
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallFront,
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallBack,
            ELSType eLSType,
            CPoint pRoofFrontLeft,
            CPoint pRoofFrontApex,
            CPoint pRoofFrontRight,
            CPoint pRoofBackLeft,
            CPoint pRoofBackApex,
            CPoint pRoofBackRight,
            float fRoof_X,
            float fRoof_Y,
            CPoint pWallFrontLeft,
            CPoint pWallFrontRight,
            CPoint pWallBackRight,
            CPoint pWallBackLeft,
            float fWallLeftOrRight_X,
            float fWallLeftOrRight_Y,
            float fWallFrontOrBack_X,
            float fWallFrontOrBack_Y1,
            float fWallFrontOrBack_Y2,
            CCalcul_1170_2 wind,
            out List<CSLoad_Free> surfaceWindLoadPlusX_Cpemin,
            out List<CSLoad_Free> surfaceWindLoadMinusX_Cpemin,
            out List<CSLoad_Free> surfaceWindLoadPlusY_Cpemin,
            out List<CSLoad_Free> surfaceWindLoadMinusY_Cpemin,
            out List<CSLoad_Free> surfaceWindLoadPlusX_Cpemax,
            out List<CSLoad_Free> surfaceWindLoadMinusX_Cpemax,
            out List<CSLoad_Free> surfaceWindLoadPlusY_Cpemax,
            out List<CSLoad_Free> surfaceWindLoadMinusY_Cpemax)
        {
            // Wind Load
            Color cColorWindPressure = Colors.DeepPink;
            Color cColorWindSagging = Colors.DarkCyan;

            float[,] fp_e_min_U_roof_Theta_4;
            float[,] fp_e_min_D_roof_Theta_4;
            float[,] fp_e_min_R_roof_Theta_4;
            float[,] fp_e_max_U_roof_Theta_4;
            float[,] fp_e_max_D_roof_Theta_4;
            float[,] fp_e_max_R_roof_Theta_4;

            float[] fp_e_W_wall_Theta_4;
            float[] fp_e_L_wall_Theta_4;
            float[,] fp_e_S_wall_Theta_4;

            if (eLSType == ELSType.eLS_ULS)
            {
                // Roof
                fp_e_min_U_roof_Theta_4 = wind.fp_e_min_U_roof_ULS_Theta_4;
                fp_e_min_D_roof_Theta_4 = wind.fp_e_min_D_roof_ULS_Theta_4;
                fp_e_min_R_roof_Theta_4 = wind.fp_e_min_R_roof_ULS_Theta_4;
                fp_e_max_U_roof_Theta_4 = wind.fp_e_max_U_roof_ULS_Theta_4;
                fp_e_max_D_roof_Theta_4 = wind.fp_e_max_D_roof_ULS_Theta_4;
                fp_e_max_R_roof_Theta_4 = wind.fp_e_max_R_roof_ULS_Theta_4;

                // Walls
                fp_e_W_wall_Theta_4 = wind.fp_e_W_wall_ULS_Theta_4;
                fp_e_L_wall_Theta_4 = wind.fp_e_L_wall_ULS_Theta_4;
                fp_e_S_wall_Theta_4 = wind.fp_e_S_wall_ULS_Theta_4;
            }
            else
            {
                // Roof
                fp_e_min_U_roof_Theta_4 = wind.fp_e_min_U_roof_SLS_Theta_4;
                fp_e_min_D_roof_Theta_4 = wind.fp_e_min_D_roof_SLS_Theta_4;
                fp_e_min_R_roof_Theta_4 = wind.fp_e_min_R_roof_SLS_Theta_4;
                fp_e_max_U_roof_Theta_4 = wind.fp_e_max_U_roof_SLS_Theta_4;
                fp_e_max_D_roof_Theta_4 = wind.fp_e_max_D_roof_SLS_Theta_4;
                fp_e_max_R_roof_Theta_4 = wind.fp_e_max_R_roof_SLS_Theta_4;

                // Walls
                fp_e_W_wall_Theta_4 = wind.fp_e_W_wall_SLS_Theta_4;
                fp_e_L_wall_Theta_4 = wind.fp_e_L_wall_SLS_Theta_4;
                fp_e_S_wall_Theta_4 = wind.fp_e_S_wall_SLS_Theta_4;
            }

            // Cpe, min
            SetSurfaceWindLoads_Cpe(
                listOfLoadedMemberTypeDataRoof,
                listOfLoadedMemberTypeDataWallLeftRight,
                listOfLoadedMemberTypeDataWallFront,
                listOfLoadedMemberTypeDataWallBack,
                eLSType,
                0,
                pRoofFrontLeft,
                pRoofFrontApex,
                pRoofFrontRight,
                pRoofBackLeft,
                pRoofBackApex,
                pRoofBackRight,
                fRoof_X,
                fRoof_Y,
                pWallFrontLeft,
                pWallFrontRight,
                pWallBackRight,
                pWallBackLeft,
                fWallLeftOrRight_X,
                fWallLeftOrRight_Y,
                fWallFrontOrBack_X,
                fWallFrontOrBack_Y1,
                fWallFrontOrBack_Y2,
                wind,
                out surfaceWindLoadPlusX_Cpemin,
                out surfaceWindLoadMinusX_Cpemin,
                out surfaceWindLoadPlusY_Cpemin,
                out surfaceWindLoadMinusY_Cpemin
            );

            // Cpe, max
            SetSurfaceWindLoads_Cpe(
                listOfLoadedMemberTypeDataRoof,
                listOfLoadedMemberTypeDataWallLeftRight,
                listOfLoadedMemberTypeDataWallFront,
                listOfLoadedMemberTypeDataWallBack,
                eLSType,
                1,
                pRoofFrontLeft,
                pRoofFrontApex,
                pRoofFrontRight,
                pRoofBackLeft,
                pRoofBackApex,
                pRoofBackRight,
                fRoof_X,
                fRoof_Y,
                pWallFrontLeft,
                pWallFrontRight,
                pWallBackRight,
                pWallBackLeft,
                fWallLeftOrRight_X,
                fWallLeftOrRight_Y,
                fWallFrontOrBack_X,
                fWallFrontOrBack_Y1,
                fWallFrontOrBack_Y2,
                wind,
                out surfaceWindLoadPlusX_Cpemax,
                out surfaceWindLoadMinusX_Cpemax,
                out surfaceWindLoadPlusY_Cpemax,
                out surfaceWindLoadMinusY_Cpemax
            );
        }

        private void SetSurfaceWindLoads_Cpe(
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataRoof,
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallLeftRight,
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallFront,
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallBack,
            ELSType eLSType,
            int iCodeForCpeMinMaxValue,
            CPoint pRoofFrontLeft,
            CPoint pRoofFrontApex,
            CPoint pRoofFrontRight,
            CPoint pRoofBackLeft,
            CPoint pRoofBackApex,
            CPoint pRoofBackRight,
            float fRoof_X,
            float fRoof_Y,
            CPoint pWallFrontLeft,
            CPoint pWallFrontRight,
            CPoint pWallBackRight,
            CPoint pWallBackLeft,
            float fWallLeftOrRight_X,
            float fWallLeftOrRight_Y,
            float fWallFrontOrBack_X,
            float fWallFrontOrBack_Y1,
            float fWallFrontOrBack_Y2,
            CCalcul_1170_2 wind,
            out List<CSLoad_Free> surfaceWindLoadPlusX_Cpe,
            out List<CSLoad_Free> surfaceWindLoadMinusX_Cpe,
            out List<CSLoad_Free> surfaceWindLoadPlusY_Cpe,
            out List<CSLoad_Free> surfaceWindLoadMinusY_Cpe
        )
        {
            // Wind Load
            Color cColorWindPressure = Colors.DeepPink;
            Color cColorWindSagging = Colors.DarkCyan;

            float[,] fp_e_U_roof_Theta_4;
            float[,] fp_e_D_roof_Theta_4;
            float[,] fp_e_R_roof_Theta_4;

            float[] fp_e_W_wall_Theta_4;
            float[] fp_e_L_wall_Theta_4;
            float[,] fp_e_S_wall_Theta_4;

            if (eLSType == ELSType.eLS_ULS)
            {
                // Roof
                if (iCodeForCpeMinMaxValue == 0)
                {
                    fp_e_U_roof_Theta_4 = wind.fp_e_min_U_roof_ULS_Theta_4;
                    fp_e_D_roof_Theta_4 = wind.fp_e_min_D_roof_ULS_Theta_4;
                    fp_e_R_roof_Theta_4 = wind.fp_e_min_R_roof_ULS_Theta_4;
                }
                else
                {
                    fp_e_U_roof_Theta_4 = wind.fp_e_max_U_roof_ULS_Theta_4;
                    fp_e_D_roof_Theta_4 = wind.fp_e_max_D_roof_ULS_Theta_4;
                    fp_e_R_roof_Theta_4 = wind.fp_e_max_R_roof_ULS_Theta_4;
                }

                // Walls
                fp_e_W_wall_Theta_4 = wind.fp_e_W_wall_ULS_Theta_4;
                fp_e_L_wall_Theta_4 = wind.fp_e_L_wall_ULS_Theta_4;
                fp_e_S_wall_Theta_4 = wind.fp_e_S_wall_ULS_Theta_4;
            }
            else
            {
                // Roof
                if (iCodeForCpeMinMaxValue == 0)
                {
                    fp_e_U_roof_Theta_4 = wind.fp_e_min_U_roof_SLS_Theta_4;
                    fp_e_D_roof_Theta_4 = wind.fp_e_min_D_roof_SLS_Theta_4;
                    fp_e_R_roof_Theta_4 = wind.fp_e_min_R_roof_SLS_Theta_4;
                }
                else
                {
                    fp_e_U_roof_Theta_4 = wind.fp_e_max_U_roof_SLS_Theta_4;
                    fp_e_D_roof_Theta_4 = wind.fp_e_max_D_roof_SLS_Theta_4;
                    fp_e_R_roof_Theta_4 = wind.fp_e_max_R_roof_SLS_Theta_4;
                }

                // Walls
                fp_e_W_wall_Theta_4 = wind.fp_e_W_wall_SLS_Theta_4;
                fp_e_L_wall_Theta_4 = wind.fp_e_L_wall_SLS_Theta_4;
                fp_e_S_wall_Theta_4 = wind.fp_e_S_wall_SLS_Theta_4;
            }

            float fLoadDirectionValueFactor_Roof = -1f;
            float fLoadDirectionValueFactor_Wall = 1f;
            float fLoadDirectionValueFactor_FrontWall = -1f;
            float fLoadDirectionValueFactor_BackWall = -1f;
            float fLoadDirectionValueFactor_LeftWall = -1f;
            float fLoadDirectionValueFactor_RightWall = -1f;

            // Cpe
            surfaceWindLoadPlusX_Cpe = new List<CSLoad_Free>(6);
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pRoofFrontLeft, wind.fC_pe_U_roof_dimensions, fRoof_Y, fRoof_X, ELCMainDirection.ePlusX, fp_e_U_roof_Theta_4, fLoadDirectionValueFactor_Roof, 0, -fRoofPitch_rad / (float)Math.PI * 180f, 0, true, false, true, 0));
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pRoofFrontApex, wind.fC_pe_D_roof_dimensions, fRoof_Y, fRoof_X, ELCMainDirection.ePlusX, fp_e_D_roof_Theta_4, fLoadDirectionValueFactor_Roof, 0, fRoofPitch_rad / (float)Math.PI * 180f, 0, true, false, true, 0, wind.iFirst_D_SegmentColorID));
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontLeft, wind.fC_pe_S_wall_dimensions, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, ELCMainDirection.ePlusX, fp_e_S_wall_Theta_4, fLoadDirectionValueFactor_FrontWall, 90, 0, 0, true, true, true, 0));
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallBackLeft, wind.fC_pe_S_wall_dimensions, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, ELCMainDirection.ePlusX, fp_e_S_wall_Theta_4, fLoadDirectionValueFactor_Wall, 90, 0, 0, true, true, true, 0));
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_e_W_wall_Theta_4[(int)ELCMainDirection.ePlusX], 90, 0, 90, cColorWindPressure, false, false, true, 0));
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, -fp_e_L_wall_Theta_4[(int)ELCMainDirection.ePlusX], 90, 0, 90, cColorWindSagging, true, true, true, 0));

            surfaceWindLoadMinusX_Cpe = new List<CSLoad_Free>(6);
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pRoofBackApex, wind.fC_pe_D_roof_dimensions, fRoof_Y, fRoof_X, ELCMainDirection.eMinusX, fp_e_D_roof_Theta_4, fLoadDirectionValueFactor_Roof, 0, fRoofPitch_rad / (float)Math.PI * 180f, 180, true, false, true, 0, wind.iFirst_D_SegmentColorID));
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pRoofBackRight, wind.fC_pe_U_roof_dimensions, fRoof_Y, fRoof_X, ELCMainDirection.eMinusX, fp_e_U_roof_Theta_4, fLoadDirectionValueFactor_Roof, 0, -fRoofPitch_rad / (float)Math.PI * 180f, 180, true, false, true, 0));
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontRight, wind.fC_pe_S_wall_dimensions, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, ELCMainDirection.eMinusX, fp_e_S_wall_Theta_4, fLoadDirectionValueFactor_Wall, 90, 0, 180, true, true, true, 0));
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallBackRight, wind.fC_pe_S_wall_dimensions, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, ELCMainDirection.eMinusX, fp_e_S_wall_Theta_4, fLoadDirectionValueFactor_BackWall, 90, 0, 180, true, true, true, 0));
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_e_L_wall_Theta_4[(int)ELCMainDirection.eMinusX], 90, 0, 90, cColorWindSagging, true, true, true, 0));
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, -fp_e_W_wall_Theta_4[(int)ELCMainDirection.eMinusX], 90, 0, 90, cColorWindPressure, false, false, true, 0));

            surfaceWindLoadPlusY_Cpe = new List<CSLoad_Free>(6);
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pRoofFrontApex, wind.fC_pe_R_roof_dimensions, fRoof_X, fRoof_Y, ELCMainDirection.ePlusY, fp_e_R_roof_Theta_4, fLoadDirectionValueFactor_Roof, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, true, false, true, 0));
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pRoofFrontRight, wind.fC_pe_R_roof_dimensions, fRoof_X, fRoof_Y, ELCMainDirection.ePlusY, fp_e_R_roof_Theta_4, fLoadDirectionValueFactor_Roof, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, true, false, true, 0));
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontLeft, wind.fC_pe_S_wall_dimensions, fWallLeftOrRight_X, fWallLeftOrRight_Y, ELCMainDirection.ePlusY, fp_e_S_wall_Theta_4, fLoadDirectionValueFactor_Wall, 90, 0, 90, true, true, true, 0));
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontRight, wind.fC_pe_S_wall_dimensions, fWallLeftOrRight_X, fWallLeftOrRight_Y, ELCMainDirection.ePlusY, fp_e_S_wall_Theta_4, fLoadDirectionValueFactor_RightWall, 90, 0, 90, true, true, true, 0));
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, -fp_e_W_wall_Theta_4[(int)ELCMainDirection.ePlusY], 90, 0, 0, cColorWindPressure, false, true, false, true, 0));
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, -fp_e_L_wall_Theta_4[(int)ELCMainDirection.ePlusY], 90, 0, 180, cColorWindSagging, true, false, false, true, 0));

            surfaceWindLoadMinusY_Cpe = new List<CSLoad_Free>(6);
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pRoofBackLeft, wind.fC_pe_R_roof_dimensions, fRoof_X, fRoof_Y, ELCMainDirection.eMinusY, fp_e_R_roof_Theta_4, fLoadDirectionValueFactor_Roof, fRoofPitch_rad / (float)Math.PI * 180f, 0, 180 + 90, true, false, true, 0));
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pRoofBackApex, wind.fC_pe_R_roof_dimensions, fRoof_X, fRoof_Y, ELCMainDirection.eMinusY, fp_e_R_roof_Theta_4, fLoadDirectionValueFactor_Roof, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 180 + 90, true, false, true, 0));
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallBackLeft, wind.fC_pe_S_wall_dimensions, fWallLeftOrRight_X, fWallLeftOrRight_Y, ELCMainDirection.eMinusY, fp_e_S_wall_Theta_4, fLoadDirectionValueFactor_LeftWall, 90, 0, 180 + 90, true, false, true, 0));
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallBackRight, wind.fC_pe_S_wall_dimensions, fWallLeftOrRight_X, fWallLeftOrRight_Y, ELCMainDirection.eMinusY, fp_e_S_wall_Theta_4, fLoadDirectionValueFactor_Wall, 90, 0, 180 + 90, true, true, true, 0));
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, -fp_e_L_wall_Theta_4[(int)ELCMainDirection.eMinusY], 90, 0, 0, cColorWindSagging, true, true, false, true, 0));
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, -fp_e_W_wall_Theta_4[(int)ELCMainDirection.eMinusY], 90, 0, 180, cColorWindPressure, true, false, false, true, 0));
        }
    }
}
