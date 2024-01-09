namespace C490_App.MVVM.Model
{
    public class LEDParameter : Parameter
    {
        public string name { get; set; }
        public uint GIntensity { get => gIntensity; set => gIntensity = value; }
        public uint Address { get => address; set => address = value; }
        public bool IsSelected { get => isSelected; set => isSelected = value; }
        public uint GOnTime { get => gOnTime; set => gOnTime = value; }
        public uint GOffTime { get => gOffTime; set => gOffTime = value; }
        public uint ROnTime { get => rOnTime; set => rOnTime = value; }
        public uint ROffTime { get => rOffTime; set => rOffTime = value; }
        public uint RIntensity { get => rIntensity; set => rIntensity = value; }
        public uint BOnTime { get => bOnTime; set => bOnTime = value; }
        public uint BOffTime { get => bOffTime; set => bOffTime = value; }
        public uint BIntensity { get => bIntensity; set => bIntensity = value; }

        private UInt32 address;
        private bool isSelected;

        private UInt32 gOnTime;
        private UInt32 gOffTime;
        private UInt32 gIntensity;

        private UInt32 rOnTime;
        private UInt32 rOffTime;
        private UInt32 rIntensity;

        private UInt32 bOnTime;
        private UInt32 bOffTime;
        private UInt32 bIntensity;

        public LEDParameter()
        {
        }
        public LEDParameter(bool set, UInt32 addr, string name)
        {
            Address = addr;
            IsSelected = set;
            this.name = name;

            GIntensity = 0;
            GOnTime = 0;
            GOffTime = 0;

            RIntensity = 0;
            ROnTime = 0;
            ROffTime = 0;

            BIntensity = 0;
            BOnTime = 0;
            BOffTime = 0;
        }

        public void setParamsFromFile(LEDParameter record)
        {
            this.GOnTime = record.GOnTime;
            this.GOffTime = record.GOffTime;
            this.GIntensity = record.GIntensity;

            this.ROnTime = record.ROnTime;
            this.ROffTime = record.ROffTime;
            this.RIntensity = record.RIntensity;

            this.BOnTime = record.BOnTime;
            this.BOffTime = record.BOffTime;
            this.BIntensity = record.BIntensity;
            this.isSelected = true;

        }
    }
}