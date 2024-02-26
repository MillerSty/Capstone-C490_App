namespace C490_App.MVVM.Model
{
    public class LEDParameter : Parameter
    {
        public string Name { get; set; }
        public ushort GIntensity { get => gIntensity; set => gIntensity = value; }
        public ushort this[int Index] { get { return address[Index]; } set { address[Index] = value; } }
        public bool IsSelected { get => isSelected; set => isSelected = value; }
        public ushort GOnTime { get => gOnTime; set => gOnTime = value; }
        public ushort GOffTime { get => gOffTime; set => gOffTime = value; }
        public ushort ROnTime { get => rOnTime; set => rOnTime = value; }
        public ushort ROffTime { get => rOffTime; set => rOffTime = value; }
        public ushort RIntensity { get => rIntensity; set => rIntensity = value; }
        public ushort BOnTime { get => bOnTime; set => bOnTime = value; }
        public ushort BOffTime { get => bOffTime; set => bOffTime = value; }
        public ushort BIntensity { get => bIntensity; set => bIntensity = value; }

        private bool isSelected;

        private ushort[] address = new UInt16[2]; //Index 1: is row | Index 0: column

        private ushort gOnTime;
        private ushort gOffTime;
        private ushort gIntensity;

        private ushort rOnTime;
        private ushort rOffTime;
        private ushort rIntensity;

        private ushort bOnTime;
        private ushort bOffTime;
        private ushort bIntensity;

        public LEDParameter()
        {
        }
        public LEDParameter(bool set, string name)
        {
            IsSelected = set;
            this.Name = name;


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