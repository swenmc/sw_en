namespace BaseClasses.CRSC
{
    public class CCrSc_3_06 : CCrSc_0_23
    {
      // Elliptical Hollow Section / Tube
      public CCrSc_3_06(float fa, float fb, float ft)
      {
          INoPointsIn = INoPointsOut = 72; // vykreslujeme ako n-uholnik, pocet bodov n
          Fa = fa;
          Fb = fb;
          Ft = ft;

          FAngle = 90f;

          // Radii
          Fr_out_major = Fa / 2f;
          Fr_in_major = Fa / 2f - Ft;

          Fr_out_minor = Fb / 2f;
          Fr_in_minor = Fb / 2f - Ft;

          if (Fr_in_major == Fr_out_major || Fr_in_minor == Fr_out_minor)
              return;

          // Create Array - allocate memory
          CrScPointsOut = new float[INoPointsOut, 2];
          CrScPointsIn = new float[INoPointsIn, 2];

          // Fill Array Data
          CalcCrSc_Coord();

          // Fill list of indices for drawing of surface - triangles edges
          loadCrScIndices();
      }

      protected override void loadCrScIndicesFrontSide()
      {
      }

      protected override void loadCrScIndicesShell()
      {
      }

      protected override void loadCrScIndicesBackSide()
      {
      }
    }
}
