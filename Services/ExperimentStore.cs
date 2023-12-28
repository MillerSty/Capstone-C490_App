using C490_App.MVVM.Model;
using System.Collections.ObjectModel;

namespace C490_App.Services
{
    class ExperimentStore
    {


        ObservableCollection<LEDParameter> ledParameters { get; set; }
        public ExperimentStore()
        {
            ledParameters = new ObservableCollection<LEDParameter>(initLedArray());
        }



        public List<LEDParameter> initLedArray()
        {
            List<LEDParameter> ledParameters = new List<LEDParameter>();

            for (int i = 0; i < 50; i++)
            {
                ledParameters.Add(new LEDParameter(false, Convert.ToUInt32(i)));
            }
            //this.isSelected[42] = true; //this is like a control jawn atm
            UInt32 addr = ledParameters[16].Address;
            bool check = ledParameters[16].isSelected;
            return ledParameters;
        }
    }
}
