namespace C490_App.MVVM.Model
{
    public class LEDParameter : Parameter
    {
        public string name;
        public UInt32 Address;
        public bool isSelected;

        public UInt32 gOnTime;
        public UInt32 gOffTime;
        public UInt32 gIntensity;

        public UInt32 rOnTime;
        public UInt32 rgOffTime;
        public UInt32 rIntensity;

        public UInt32 bOnTime;
        public UInt32 bOffTime;
        public UInt32 bIntensity;

        public LEDParameter(bool set, UInt32 addr, string name)
        {
            Address = addr;
            isSelected = set;
            this.name = name;
        }

        public void setIsSelected(bool set)
        {
            isSelected = set;
        }
        public void setAddress(UInt32 set)
        {
            Address = set;
        }


    }
}