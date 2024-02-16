using C490_App.Core;

namespace C490_App.MVVM.Model
{
    public class ExperimentModelBase : ObservableObject
    {
        public virtual void setIsEnabled() { }
        public virtual void runExperiment(ExperimentStore _serialPort) { }
    }
}
