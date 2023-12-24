using C490_App.Core;
using C490_App.MVVM.Model;

namespace C490_App.MVVM.ViewModel
{
    public class HomeFrameViewModel
    {

        public PotentiostatViewModel PotentiostatViewModel { get; }

        public LedArrayViewModel LedArrayViewModel { get; }
        public CAModel _caModel { get; }
        public DPVModel _dpvModel { get; }
        public CVModel _cvModel { get; }
        public RelayCommand check { get; }



        public HomeFrameViewModel()
        {
            _caModel = new CAModel();
            _dpvModel = new DPVModel();
            _cvModel = new CVModel();
            PotentiostatViewModel = new PotentiostatViewModel();
            LedArrayViewModel = new LedArrayViewModel();

            check = new RelayCommand(o => newStore(), o => true);
        }
        public void newStore()
        {

            int check = 100;
        }


    }
}
