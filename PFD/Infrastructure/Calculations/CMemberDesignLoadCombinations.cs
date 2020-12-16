using BaseClasses;
using FEM_CALC_BASE;
using M_BASE;
using MATH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD.Infrastructure
{
    public class CMemberDesignLoadCombinations
    {
        //-------------------------------------------------------------------------------------------------------------------------------
        MemberDesignLoadCombinationsAsyncStub stub = null;
        public delegate void MemberDesignLoadCombinationsAsyncStub(CMember m, bool MShearDesignAccording334, bool MIgnoreWebStiffeners, int iNumberOfDesignSections, 
            CModel_PFD Model, bool MUseCRSCGeometricalAxes, bool MIsGableRoofModel,
            bool MUniformShearDistributionInAnchors, CalculationSettingsFoundation FootingCalcSettings,
            List<CMemberInternalForcesInLoadCombinations> MemberInternalForcesInLoadCombinations, List<CMemberDeflectionsInLoadCombinations> MemberDeflectionsInLoadCombinations);

        private Object theLock;

        public List<CMemberLoadCombinationRatio_ULS> MemberDesignResults_ULS;
        public List<CMemberLoadCombinationRatio_SLS> MemberDesignResults_SLS;
        public List<CJointLoadCombinationRatio_ULS> JointDesignResults_ULS;
        public List<CFootingLoadCombinationRatio_ULS> FootingDesignResults_ULS;


        //-------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------
        public CMemberDesignLoadCombinations(Object lockObject)
        {
            theLock = lockObject;
        }
        

        //-------------------------------------------------------------------------------------------------------------------------------
        public IAsyncResult BeginMemberDesignLoadCombinations(CMember m, bool MShearDesignAccording334, bool MIgnoreWebStiffeners, int iNumberOfDesignSections, 
            CModel_PFD Model, bool MUseCRSCGeometricalAxes, bool MIsGableRoofModel,
            bool MUniformShearDistributionInAnchors, CalculationSettingsFoundation FootingCalcSettings,
            List<CMemberInternalForcesInLoadCombinations> MemberInternalForcesInLoadCombinations, List<CMemberDeflectionsInLoadCombinations> MemberDeflectionsInLoadCombinations,
            AsyncCallback cb, object s)
        {
            stub = new MemberDesignLoadCombinationsAsyncStub(Calculate);
            //using delegate for asynchronous implementation
            return stub.BeginInvoke(m, MShearDesignAccording334, MIgnoreWebStiffeners, iNumberOfDesignSections, Model, MUseCRSCGeometricalAxes, MIsGableRoofModel,
                    MUniformShearDistributionInAnchors, FootingCalcSettings, MemberInternalForcesInLoadCombinations, MemberDeflectionsInLoadCombinations, cb, null);
        }

        //-------------------------------------------------------------------------------------------------------------------------------
        public void EndMemberDesignLoadCombinations(IAsyncResult call)
        {
            stub.EndInvoke(call);
        }

        public void Calculate(CMember m, bool MShearDesignAccording334, bool MIgnoreWebStiffeners, int iNumberOfDesignSections, CModel_PFD Model, bool MUseCRSCGeometricalAxes, bool MIsGableRoofModel,
            bool MUniformShearDistributionInAnchors, CalculationSettingsFoundation FootingCalcSettings,
            List<CMemberInternalForcesInLoadCombinations> MemberInternalForcesInLoadCombinations, List<CMemberDeflectionsInLoadCombinations> MemberDeflectionsInLoadCombinations)
        {
            MemberDesignResults_ULS = new List<CMemberLoadCombinationRatio_ULS>();
            MemberDesignResults_SLS = new List<CMemberLoadCombinationRatio_SLS>();
            JointDesignResults_ULS = new List<CJointLoadCombinationRatio_ULS>();
            FootingDesignResults_ULS = new List<CFootingLoadCombinationRatio_ULS>();

            if (!m.BIsGenerated) return;
            if (!m.BIsSelectedForDesign) return; // Only structural members (not auxiliary members or members with deactivated design)

            foreach (CLoadCombination lcomb in Model.m_arrLoadCombs)
            {
                if (lcomb.eLComType == ELSType.eLS_ULS) // Combination Type - ULS
                {
                    // Member design internal forces
                    designInternalForces[] sMemberDIF_x;

                    // Member Design
                    CMemberDesign memberDesignModel = new CMemberDesign();
                    // TODO - sBucklingLengthFactors_design  a sMomentValuesforCb_design nemaju byt priradene prutu ale segmentu pruta pre kazdy load case / load combination

                    // Set basic internal forces, buckling lengths and bending moments for determination of Cb for member and load combination
                    CMemberInternalForcesInLoadCombinations mInternal_forces_and_design_parameters = MemberInternalForcesInLoadCombinations.Find(i => i.Member.ID == m.ID && i.LoadCombination.ID == lcomb.ID);

                    // Design check procedure
                    memberDesignModel.SetDesignForcesAndMemberDesign_PFD(MUseCRSCGeometricalAxes,
                        MShearDesignAccording334,
                        MIgnoreWebStiffeners,
                        iNumberOfDesignSections,
                        m,
                        mInternal_forces_and_design_parameters.InternalForces, // TO Ondrej - toto by sme asi mohli predavat cele ako jeden parameter mInternal_forces_and_design_parameters namiesto troch
                        mInternal_forces_and_design_parameters.BucklingLengthFactors,
                        mInternal_forces_and_design_parameters.BendingMomentValues,
                        out sMemberDIF_x);

                    // Add design check results to the list
                    MemberDesignResults_ULS.Add(new CMemberLoadCombinationRatio_ULS(m, lcomb, memberDesignModel.fMaximumDesignRatio, sMemberDIF_x[memberDesignModel.fMaximumDesignRatioLocationID], mInternal_forces_and_design_parameters.BucklingLengthFactors[memberDesignModel.fMaximumDesignRatioLocationID], mInternal_forces_and_design_parameters.BendingMomentValues[memberDesignModel.fMaximumDesignRatioLocationID]));

                    // Joint Design
                    CJointDesign jointDesignModel = new CJointDesign();
                    designInternalForces sjointStartDIF_x;
                    designInternalForces sjointEndDIF_x;
                    CConnectionJointTypes jointStart;
                    CConnectionJointTypes jointEnd;

                    jointDesignModel.SetDesignForcesAndJointDesign_PFD(iNumberOfDesignSections, MUseCRSCGeometricalAxes, MShearDesignAccording334, MUniformShearDistributionInAnchors, Model, m, FootingCalcSettings, mInternal_forces_and_design_parameters.InternalForces, out jointStart, out jointEnd, out sjointStartDIF_x, out sjointEndDIF_x);

                    // Validation - Main member of joint must be defined
                    if (jointStart.m_MainMember == null)
                        throw new ArgumentNullException("Error" + "Joint No: " + jointStart.ID + " Main member is not defined.");
                    if (jointEnd.m_MainMember == null)
                        throw new ArgumentNullException("Error" + "Joint No: " + jointEnd.ID + " Main member is not defined.");

                    // Start Joint
                    JointDesignResults_ULS.Add(new CJointLoadCombinationRatio_ULS(m, jointStart, lcomb, jointDesignModel.fDesignRatio_Start, sjointStartDIF_x));

                    // End Joint
                    JointDesignResults_ULS.Add(new CJointLoadCombinationRatio_ULS(m, jointEnd, lcomb, jointDesignModel.fDesignRatio_End, sjointEndDIF_x));

                    FootingDesignResults_ULS.Add(new CFootingLoadCombinationRatio_ULS(m, jointDesignModel.footingJoint, jointDesignModel.footing, lcomb, jointDesignModel.fDesignRatio_footing, jointDesignModel.footingIF));

                    // Output (for debugging - member results)
                    bool bDebugging = false; // Testovacie ucely
                    if (bDebugging)
                        System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                          "Load Combination ID: " + lcomb.ID + "\t | " +
                                          "Design Ratio: " + Math.Round(memberDesignModel.fMaximumDesignRatio, 3).ToString() + "\n");

                    // Output (for debugging - member connection / joint results)
                    if (bDebugging)
                        System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                          "Joint ID: " + jointStart.ID + "\t | " +
                                          "Load Combination ID: " + lcomb.ID + "\t | " +
                                          "Design Ratio: " + Math.Round(jointDesignModel.fDesignRatio_Start, 3).ToString() + "\n");

                    if (bDebugging)
                        System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                          "Joint ID: " + jointEnd.ID + "\t | " +
                                          "Load Combination ID: " + lcomb.ID + "\t | " +
                                          "Design Ratio: " + Math.Round(jointDesignModel.fDesignRatio_End, 3).ToString() + "\n");
                }
                else // Combination Type - SLS (TODO - pre urcenie Ieff potrebujeme spocitat aj strength)
                {
                    // Member design deflections
                    designDeflections[] sMemberDDeflection_x;

                    // Member Design
                    CMemberDesign memberDesignModel = new CMemberDesign();

                    // Set basic deflections for member and load combination
                    CMemberDeflectionsInLoadCombinations mDeflections = MemberDeflectionsInLoadCombinations.Find(i => i.Member.ID == m.ID && i.LoadCombination.ID == lcomb.ID);

                    float fDeflectionLimitDenominator_Fraction = 1; // 25-1000
                    float fDeflectionLimit = 0f;

                    // Find group of current member (definition of member type)
                    CMemberGroup currentMemberTypeGroupOfMembers = Model.listOfModelMemberGroups.FirstOrDefault(gr => gr.MemberType_FS == m.EMemberType);

                    // To Mato - pozor ak su to dvere, okno, tak currentMemberTypeGroupOfMembers je null
                    // Ak je to aktualne, tak musime teda este doplnit funkcionalitu tak, aby sa pre pruty blokov tiez vytvorili skupiny

                    if (currentMemberTypeGroupOfMembers != null)
                    {
                        // Set deflection limit depending of member type and load combination type
                        if (lcomb.IsCombinationOfPermanentLoadCasesOnly())
                        {
                            fDeflectionLimitDenominator_Fraction = currentMemberTypeGroupOfMembers.DeflectionLimitFraction_Denominator_PermanentLoad;
                            fDeflectionLimit = currentMemberTypeGroupOfMembers.DeflectionLimit_PermanentLoad;
                        }
                        else
                        {
                            fDeflectionLimitDenominator_Fraction = currentMemberTypeGroupOfMembers.DeflectionLimitFraction_Denominator_Total;
                            fDeflectionLimit = currentMemberTypeGroupOfMembers.DeflectionLimit_Total;
                        }
                    }

                    // Design check procedure
                    memberDesignModel.SetDesignDeflections_PFD(MUseCRSCGeometricalAxes,
                        MIsGableRoofModel,
                        iNumberOfDesignSections,
                        m,
                        fDeflectionLimitDenominator_Fraction,
                        fDeflectionLimit,
                        mDeflections.Deflections,
                        out sMemberDDeflection_x);

                    // Add design check results to the list
                    MemberDesignResults_SLS.Add(new CMemberLoadCombinationRatio_SLS(m, lcomb, memberDesignModel.fMaximumDesignRatio, sMemberDDeflection_x[memberDesignModel.fMaximumDesignRatioLocationID]));

                    // Output (for debugging)
                    bool bDebugging = false; // Testovacie ucely
                    if (bDebugging)
                        System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                          "Load Combination ID: " + lcomb.ID + "\t | " +
                                          "Design Ratio: " + Math.Round(memberDesignModel.fMaximumDesignRatio, 3).ToString());                                        
                }
            }
        }

    }
}
