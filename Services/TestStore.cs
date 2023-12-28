using C490_App.MVVM.Model;

namespace C490_App.Services
{
    public class TestStore
    {
        private readonly List<LEDParameter> _leds;
        public IEnumerable<LEDParameter> LEDS => _leds;

        public TestStore()
        {
            _leds = new List<LEDParameter>();
        }

        public async Task Load()
        {


        }
    }
}
