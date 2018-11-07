using System;

namespace BaseClasses
{
    [Serializable]
    public class CProductionInfo
    {
        private string MJobNumber;
        private string MCustomer;
        private int MAmount;
        private int MAmountRH;
        private int MAmountLH;


        public string JobNumber
        {
            get
            {
                return MJobNumber;
            }
        }

        public string Customer
        {
            get
            {
                return MCustomer;
            }
        }

        public int Amount
        {
            get
            {
                return MAmount;
            }
        }

        public int AmountRH
        {
            get
            {
                return MAmountRH;
            }
        }

        public int AmountLH
        {
            get
            {
                return MAmountLH;
            }
        }

        //----------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------
        public CProductionInfo(string jobNumber, string customer, int amount, int amountRH, int amountLH)
        {
            MJobNumber = jobNumber;
            MCustomer = customer;
            MAmount = amount;
            MAmountRH = amountRH;
            MAmountLH = amountLH;
        }



    }
}
