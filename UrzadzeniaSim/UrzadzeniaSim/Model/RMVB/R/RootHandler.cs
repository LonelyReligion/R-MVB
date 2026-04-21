namespace UrzadzeniaSim.Model.RMVB.R
{
    public interface RootHandler
    {
        RNode ProvideRoot ();

        void UpdateRoot (RNode root);
    }
}
