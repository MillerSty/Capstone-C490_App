using C490_App.MVVM.Model;
using System.Collections.ObjectModel;

namespace C490_App.Services
{

    //public interface IExperimentStore
    //{

    //    public ObservableCollection<LEDParameter> ledParameters { get; set; }

    //    ObservableCollection<LEDParameter> initLedArray();
    //}
    public class ExperimentStore
    {



        public ExperimentStore()
        {
            ledParameters = new ObservableCollection<LEDParameter>(initLedArray());
            model = new ExperimentModel();
        }



        private ExperimentModel model;
        public ObservableCollection<LEDParameter> ledParameters { get; set; } = new();

        public ExperimentModel getModel() { return model; }
        public void setModel(ExperimentModel model) { this.model = model; }
        public ObservableCollection<LEDParameter> initLedArray()
        {

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
