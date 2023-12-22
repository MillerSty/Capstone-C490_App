namespace C490_App.MVVM.ViewModel
{
    public class HomeFrameViewModel
    {

        PotentiostatViewModel PotentiostatViewModel { get; set; }


        public HomeFrameViewModel()
        {
            PotentiostatViewModel = new PotentiostatViewModel();
        }


    }
}
