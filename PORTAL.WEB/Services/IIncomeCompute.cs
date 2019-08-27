namespace PORTAL.WEB.Services
{
    public interface IIncomeCompute
    {
        void BayanihanIncomeCompute(string id);
        void DirectReferralCompute(string id);
        void UnilevelCompute(string id);
        void GeneologyIncomeCompute(string id);
        void ComputeNetIncome(string id);
    }
}
