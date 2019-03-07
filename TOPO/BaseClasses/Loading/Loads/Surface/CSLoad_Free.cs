using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace BaseClasses
{
    [Serializable]
    abstract public class CSLoad_Free : CLoad
    {
        //----------------------------------------------------------------------------
        //private int [] m_iSurfaceCollection;
        private ESLoadTypeDistr m_sLoadTypeDistr; // Type of external force distribution
        private ESLoadType m_sLoadType; // Type of external force
        private ELoadDirection m_eLoadDir;
        private float m_fRotationX_deg;
        private float m_fRotationY_deg;
        private float m_fRotationZ_deg;
        private Point3DCollection m_pSurfacePoints;
        private Point3DCollection m_pSurfacePoints_h;
        private Point3DCollection m_pSurfaceDefinitionPoints;
        private List<FreeSurfaceLoadsMemberData> m_listOfMemberData; // Tento zoznam by sme mali vyhodit - member moze byt pod roznymi plochami, takze to nie je jednoznacne
        private List<FreeSurfaceLoadsMemberTypeData> m_listOfLoadedMemberTypeData; // Tento zoznam by sme mali vyhodit - member moze byt pod roznymi plochami, takze to nie je jednoznacne

        private List<Point3D> m_PointsGCS;

        //----------------------------------------------------------------------------
        /*public int[] ISurfaceCollection
        {
            get { return m_iSurfaceCollection; }
            set { m_iSurfaceCollection = value; }
        }*/
        public ESLoadTypeDistr SLoadTypeDistr
        {
            get { return m_sLoadTypeDistr; }
            set { m_sLoadTypeDistr = value; }
        }
        public ESLoadType SLoadType
        {
            get { return m_sLoadType; }
            set { m_sLoadType = value; }
        }
        public ELoadDirection ELoadDir
        {
            get { return m_eLoadDir; }
            set { m_eLoadDir = value; }
        }

        public float RotationX_deg
        {
            get { return m_fRotationX_deg; }
            set { m_fRotationX_deg = value; }
        }

        public float RotationY_deg
        {
            get { return m_fRotationY_deg; }
            set { m_fRotationY_deg = value; }
        }

        public float RotationZ_deg
        {
            get { return m_fRotationZ_deg; }
            set { m_fRotationZ_deg = value; }
        }

        public Point3DCollection pSurfacePoints
        {
            get { return m_pSurfacePoints; }
            set { m_pSurfacePoints = value; }
        }
        public Point3DCollection pSurfacePoints_h
        {
            get { return m_pSurfacePoints_h; }
            set { m_pSurfacePoints_h = value; }
        }

        // Vymazat - member moze byt pod roznymi plochami, tak ze to nie je jednoznacne, musi to fungovat takze je zatazovacie plochy neobsahuju takyto zoznam, ale algoritmus vygeneruje CMLoad_21 alebo skupinu CMLoad_24 na jednom prute, pre pruty ktore su pod jednou plochou alebo pod viacerymi plochami
        public List<FreeSurfaceLoadsMemberData> listOfMemberData // Naplni sa automaticky v objekte podla rozmerov suradnic fiktivnej plochy
        {
            get { return m_listOfMemberData; }
            set { m_listOfMemberData = value; }
        }

        // Vymazat - member moze byt pod roznymi plochami, tak ze to nie je jednoznacne, musi to fungovat takze je zatazovacie plochy neobsahuju takyto zoznam, ale algoritmus vygeneruje CMLoad_21 alebo skupinu CMLoad_24 na jednom prute, pre pruty ktore su pod jednou plochou alebo pod viacerymi plochami
        public List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeData // Vstupny parameter konstruktora do objektu
        {
            get { return m_listOfLoadedMemberTypeData; }
            set { m_listOfLoadedMemberTypeData = value; }
        }

        public List<Point3D> PointsGCS
        {
            get
            {
                return m_PointsGCS;
            }

            set
            {
                m_PointsGCS = value;
            }
        }

        public Point3DCollection SurfaceDefinitionPoints
        {
            get
            {
                return m_pSurfaceDefinitionPoints;
            }

            set
            {
                m_pSurfaceDefinitionPoints = value;
            }
        }

        public float m_fOpacity;
        public Color m_Color = new Color(); // Default

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public CSLoad_Free(List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeData_temp, ELoadCoordSystem eLoadCS_temp, ELoadDirection eLoadDirection_temp, bool bIsDisplayed, float fTime)
        {
            listOfLoadedMemberTypeData = listOfLoadedMemberTypeData_temp;
            Displayin3DRatio = 0.001f;
            ELoadCS = eLoadCS_temp; // GCS or LCS surface load
            ELoadDir = eLoadDirection_temp;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;
        }

        public void FillListOfMemberData(CMember[] arrayOfMembersToSearchIn)
        {
            // For each item in listOfLoadedMemberTypeData
            foreach (FreeSurfaceLoadsMemberTypeData type in listOfLoadedMemberTypeData)
            {
                // Fill list of members and set loading area

                // Popis
                // Pre kazdy typ pruta (purlin, girt, ....), ktory ma byt zatazeny zatazenim definovanym v tomto objekte vyhladat v zozname vsetkych
                // prutov v modeli tie pruty ktore lezia v ploche definujucej zatazenie, pridat ich do zoznamu  vlastnosti prutov (member data, obsahuje prut a zatazovaciu plochu), ktore plocha zatazuje a zaroven im nastavit zatazovaciu plochu (sirku)

                // TODO No. 54 - Ondrej
                // teraz su suradnice pSurfacePoints urcene v LCS plochy, potrebujeme aby sa vyhladavalo podla suradnic plochy v GCS (to znamena pouzit suradnice tychto bodov po transformacii fiktivnej plochy do GCS)
                // Surdanice bodov pruta v GCS sa potom porovnavaju plochou definovanou suradnicami v GCS
                // je potrebne skontrolovat ci sa maju pouzit horne alebo spodne suradnice kvadra definujuceho objem (meni sa v objekte podla hodnoty zatazenia a strany plochy kam sa ma vykreslovat, nad alebo pod plochu,
                // asi sa to dotkne aj tychto dat - list pSurfacePoints - pozri  CSLoad_FreeUniform.cs, line 306 pSurfacePoints[i] = pBottom;
                // takze bud sa musi urcit ci sa ma pouzit zoznam pSurfacePoints alebo pSurfacePoints_h, 
                // pripadne si niekde stranou odlozit povodne suradnice bodov pSurfacePoints, ktore do plochy vstupili este pred tym nez sa modifikovali kvoli
                // vykresleniu kvadra plochy v grafike
                // na transformaciu sa pouzije public Transform3DGroup CreateTransformCoordGroup() z CSLoad_FreeUniform.cs
                // Pre generovanie zatazenia na prute CMLoad_21 prepocitat hodnotu zatazovacej sirky b ako b = zatazovacia plocha na jeden prut A / dlzka pruta L

                // Vymazat - member moze byt pod roznymi plochami, takze to nie je jednoznacne, musi to fungovat takze je zatazovacie plochy neobsahuju takyto zoznam, ale algoritmus vygeneruje CMLoad_21 alebo skupinu CMLoad_24 na jednom prute, pre pruty ktore su pod jednou plochou alebo pod viacerymi plochami
                foreach (CMember m in arrayOfMembersToSearchIn)
                {
                    if (m.EMemberType == type.memberType && Drawing3D.MemberLiesOnPlane(pSurfacePoints[0], pSurfacePoints[1], pSurfacePoints[2], m, 0.001))
                        m_listOfMemberData.Add(new FreeSurfaceLoadsMemberData(m, m.FLength * type.fLoadingWidth));
                }
            }
        }
    }
}
