namespace BaseClasses
{
    public abstract class CWeld : CConnectionComponentEntity3D
    {
        public CWeld()
        {
            BIsDisplayed = true;
        }

        public CWeld(bool bIsDisplayed)
        {
            BIsDisplayed = bIsDisplayed;
        }

        protected override void loadIndices()
        { }
    }
}