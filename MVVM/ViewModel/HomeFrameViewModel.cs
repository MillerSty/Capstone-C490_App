namespace C490_App.MVVM.ViewModel
{
    public class HomeFrameViewModel
    {

        public PotentiostatViewModel PotentiostatViewModel { get; }
        public LedArrayViewModel LedArrayViewModel { get; }

        public HomeFrameViewModel()
        {
            PotentiostatViewModel = new PotentiostatViewModel();
            LedArrayViewModel = new LedArrayViewModel();
        }


    }
}
