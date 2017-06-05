using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AtolFiscalPrinter;
using FiscalHelper;

namespace AtolFiscalPrinterTester
{
    public partial class MainForm : Form
    {
        Printer _printer;
        Printer _p
        {
            get { if (_printer == null) _printer = new Printer(); return _printer; }

        }

        public MainForm()
        {
            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            _printer = new Printer();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _p.ShowSettings();
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
            _p.PrintString("Hello world");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Check c = new Check();
            c.items.Add(new CheckItem());
            c.items[0].name = "Test";
            c.items[0].quantity = 1.123;
            c.items[0].price =3.68;

            c.checkMode = CheckMode.NonElectronic;
            c.checkType = CheckType.Income;
            _p.PrintCheck(c);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            _p.OpenSession();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            
        }
    }
}
