using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiscalHelper
{

    public enum CheckMode { Electronic=0, NonElectronic=1 }
    public enum CheckType { Income=1,Outcome=4 }
    public enum PayType { Cash=0,Card=3}

    public class CheckItem
    {
        public string name;
        public double quantity, price;       
    }

    public class Check
    {
        public List<CheckItem> items;

        public CheckMode checkMode;
        public CheckType checkType;

        public string address;

        public string cassir;

        public Check()
        {
            checkMode = CheckMode.NonElectronic;
            checkType = CheckType.Outcome;
            items = new List<CheckItem>();
        }

        public double paySumma;
        public PayType payType;
        public double discount;



    }


    public enum FiscalError { ok=0, none=1,nullRef=2 }
    public enum FiscalType { atol=0 }

    public class FiscalException : Exception
    {

        public FiscalException(string message)
        : base(message)
        {

        }

    }

    public interface IFiscalPrinter
    {
        FiscalError PrintCheck(Check check);
        void ShowSettings();
        void OpenSession();
    }

    public interface IFiscalPrinterFactory
    {
       IFiscalPrinter create(FiscalType ft);
    }

    



}
