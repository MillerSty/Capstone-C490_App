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
            Model = new ExperimentModel();
        }

        public ExperimentModel Model { get => model; set => model = value; }

        private ExperimentModel model;
        public ObservableCollection<LEDParameter> ledParameters { get; set; } = new();

        public ObservableCollection<String> ledNames { get; set; } = new();


        public ObservableCollection<LEDParameter> initLedArray()
        {

            for (int i = 0; i < 50; i++)
            {
                ledParameters.Add(new LEDParameter(false, Convert.ToUInt32(i), $"LED {i}"));
                ledNames.Add($"LED {i}");
            }
            return ledParameters;
        }

        public void UpdateLEDS(ObservableCollection<bool> ledsSelected)
        {
            for (int i = 0; i < 50; i++)
            {
                ledParameters[i].IsSelected = ledsSelected[i];
            }
        }



    }
}
