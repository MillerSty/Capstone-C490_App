using C490_App.Core;
using C490_App.MVVM.Model;
using System.Diagnostics;

namespace C490_App.MVVM.ViewModel
{
    internal class ExperimentParameterViewModel : ViewModelBase
    {
        DPVModel _dpvModel { get; set; }
        public DPVModel DpvModel
        {
            get
            {
                return _dpvModel;
            }
            set
            {
                _dpvModel = value;
                OnPropertyChanged();
            }

        }
        CVModel _cvModel { get; set; }
        public CVModel CvModel
        {
            get
            {
                return _cvModel;
            }
            set
            {
                _cvModel = value;
                OnPropertyChanged();
            }

        }
        CAModel _caModel { get; set; }
        public CAModel CaModel
        {
            get
            {
                return _caModel;
            }
            set
            {
                _caModel = value;
                OnPropertyChanged();
            }

        }
        public ExperimentModelBase experiment { get; set; }
        public RelayCommand Save { get; set; }

        public RelayCommand Cancel { get; set; }
        ExperimentStore ExperimentLocal;
        public ExperimentParameterViewModel(ExperimentStore ExperimentSingleton)
        {
            ExperimentLocal = ExperimentSingleton;
            switch (ExperimentLocal.Model.GetType().Name.ToString())
            {
                case "DPVModel":
                    DpvModel = (DPVModel)ExperimentLocal.Model; break;
                case "CAModel":
                    CaModel = (CAModel)ExperimentLocal.Model; break;
                case "CVModel":
                    CvModel = (CVModel)ExperimentLocal.Model; break;
                default:
                    DpvModel = new DPVModel();
                    CvModel = new CVModel();
                    CaModel = new CAModel(); break;
            }

            Save = new RelayCommand(o => save(), o => true);

            Cancel = new RelayCommand(o => cancel(), o => true);

        }

        public void save()
        {
            //ExperimentLocal.Model = experiment;
            Trace.WriteLine("Saving");
        }
        public void cancel() { Trace.WriteLine("Canceling"); }
    }
}
