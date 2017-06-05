using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiscalHelper;

namespace AtolFiscalPrinter
{

    enum DriverMode { Select,Command,zReport=3  }
    struct Result { public int code; public string description;
        public override string ToString()
        {
            return "id: " + code.ToString() + " message: " + description;
        }
    }

    public class Printer:IFiscalPrinter
    {
        dynamic _driver = null;
        string objectName = "AddIn.FprnM45";
        string accessPassword = "0";
        string defaultPassword = "30";
        string cassir1Password = "1";
        string password = "30";

        void SetMode(DriverMode mode)
        {
            _driver.Mode = (int)mode;
            _driver.SetMode();
        }

        void Password()
        {
            _driver.AccessPassword = accessPassword;
            _driver.DefaultPassword = defaultPassword;

        }

        Result GetResult()
        {
            Result r;
            r.code = _driver.ResultCode;
            r.description = _driver.ResultDescription;
            return r;
        }

        public void GetRegister(int num)
        {
            
            _driver.DeviceEnabled = true;
            _driver.RegisterNumber = num;
            _driver.GetRegister();
            var r = GetResult();
            if (r.code != 0) throw new FiscalException(r.ToString());            
        }


        public int  OFD_NotSendedDocuments()
        {
            try
            {
                GetRegister(44);
                int i = _driver.Count;
                var r = GetResult();
                if (r.code != 0) throw new FiscalException(r.ToString());
                return i;
            }
            finally
            {
                _driver.DeviceEnabled = false;
            }
        }

        public DateTime OFD_FirstNotSended()
        {
            try
            {
                GetRegister(45);
                DateTime dt;
                try
                {
                    dt = new DateTime(_driver.Year, _driver.Month, _driver.Day, _driver.Hour, _driver.Minute, 0);
                }
                catch
                {
                    dt = new DateTime(_driver.Year,1,1);
                }
                return dt;
            }
            finally
            {
                _driver.DeviceEnabled = false;
            }
        }

        public DateTime OFD_LastSended()
        {
            try
            {
                GetRegister(51);
                DateTime dt;
                try
                {
                    dt = new DateTime(_driver.Year, _driver.Month, _driver.Day, _driver.Hour, _driver.Minute, 0);
                }
                catch
                {
                    dt = new DateTime(_driver.Year, 1, 1);
                }
                return dt;
            }
            finally
            {
                _driver.DeviceEnabled = false;
            }
        }

        

        public FiscalError PrintCheck(Check check)
        {
            try
            { 
                if (check == null) return FiscalError.nullRef;
                
                //Password();
                var r = GetResult();
                _driver.DeviceEnabled = true;
                r = GetResult();

                _driver.ResetMode();
                r = GetResult();
                if (r.code != 0) throw new FiscalException(r.ToString());
                

                _driver.OperatorPassword = cassir1Password;
                SetMode(DriverMode.Command);
                r = GetResult();
                _driver.NewDocument();
                r = GetResult();

                if (r.code != 0) throw new FiscalException(r.ToString());

                _driver.CheckType = (int)check.checkType;
                _driver.CheckMode = (int)check.checkMode;



                _driver.OpenCheck();

                if ((check.checkMode == CheckMode.Electronic) && (check.address != ""))
                {
                    _driver.AttrNumber = 1008;
                    _driver.AttrValue = check.address;
                    _driver.WriteAttribute();
                    r = GetResult();

                    if (r.code != 0) throw new FiscalException(r.ToString());
                }


                r = GetResult();
                for (var i = 0; i < check.items.Count; i++)
                {
                    _driver.Name = check.items[i].name;
                    _driver.Quantity = check.items[i].quantity;
                    _driver.Price = check.items[i].price;

                    if ((check.discount >= 0) && (check.discount <= 100))
                    {
                        _driver.DiscountType = 1;
                        _driver.DiscountValue = check.discount;

                    }

                    _driver.Registration();



                    r = GetResult();
                    if (r.code != 0) throw new FiscalException(r.ToString());
                }





                _driver.TypeClose = (int)check.payType;


                if (check.paySumma > 0)
            {
                _driver.Summ = check.paySumma;
                
                _driver.Payment();
            }
            _driver.CloseCheck();


            return FiscalError.none;
        }
            finally
            {
            _driver.DeviceEnabled=false;
            
            }


}




        public void ShowSettings() {
            if(_driver!=null)
               _driver.ShowProperties();
        }
        public Printer()
        {
            Type f = Type.GetTypeFromProgID(objectName, true);
            _driver = Activator.CreateInstance(f);
        }

        public void PrintString(string s)
        {
            
            _driver.DeviceEnabled = true;
            _driver.Caption = s;
            _driver.PrintString();

        }

        public void OpenSession()
        {
            Password();
            _driver.DeviceEnabled = true;
           // if (_driver.SessionOpened)
            {
                _driver.Password = password;
                SetMode(DriverMode.zReport);
                if (GetResult().code != 0)
                    throw new FiscalException(GetResult().ToString());
                _driver.ReportType = 1;
                _driver.Report();

            }


            _driver.Password = "1";
            _driver.DeviceEnabled = true;
            SetMode(DriverMode.Command);
            _driver.OpenSession();
            if (GetResult().code != 0)
                throw new FiscalException(GetResult().ToString());

        }

       
    }

    public class Factory:IFiscalPrinterFactory
    {
        public IFiscalPrinter create(FiscalType ft)
        {
            if (ft == FiscalType.atol)
                return (new Printer());
            else
                return null;
        }
    }


}
