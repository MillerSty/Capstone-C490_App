using C490_App.MVVM.Model;
using C490_App.MVVM.ViewModel;
using C490_App.Services;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace C490_App.Core
{
    public class FileHandler
    {
        public void readExperiment()
        {
            using (var reader = new StreamReader("P:/Git repos/C490_App/Resources/paramExport.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                csv.Read();

                //can probably use the raw data of csv object to iterate through things
                String sx = csv.GetField<String>("x");
                String sy = csv.GetField<String>("y");

            }


        }
        public void writeExperiment()
        {
            Trace.WriteLine("Export");
            // OpenFileDialog openFileDialogClose = new OpenFileDialog();
            SaveFileDialog saveFileDialogClose = new SaveFileDialog();
            saveFileDialogClose.DefaultExt = ".csv";
            if (saveFileDialogClose.ShowDialog() == true)
            {
                if (saveFileDialogClose.FileName != "")
                {
                    using (var writer = new StreamWriter(saveFileDialogClose.FileName))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        //Experiment (dateTime stamp)
                        csv.WriteComment("Experiment 2024");

                        csv.NextRecord();
                        //for each 'region x' in experiment 
                        for (int i = 0; i < 1; i++)
                        {
                            csv.WriteField("Pot " + i);
                            csv.WriteField("");
                        }
                        //end for each

                        //for each 'region x' in experimnt Z let x= z.xValue and y= z.yValue
                        csv.NextRecord();
                        for (int i = 0; i < 1; i++)
                        {
                            csv.WriteField("x");
                            csv.WriteField("y");
                        }
                        //csv.WriteField("X");
                        //csv.WriteField("Y");
                        csv.NextRecord();
                        //for each 'region x' in experiment, append value
                        csv.WriteField("1");
                        csv.WriteField("2");
                        //csv.WriteField("3");
                        //csv.WriteField("4");
                        //csv.WriteField("5");
                        //csv.WriteField("6");



                    }
                }
            }


        }

        public void appendExperiment()
        {
            // Append to the file.
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Don't write the header again.
                HasHeaderRecord = false,
            };
            using (var stream = File.Open("P:/Git repos/C490_App/Resources/paramExport.csv", FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.NextRecord();
                csv.WriteField("7");
                csv.WriteField("8");
            }


        }

        public bool fileImport(Object o, ExperimentStore ExperimentLocal, LedArrayViewModel LedArrayViewModel)
        {

            if (bool.Parse(o.ToString()))
            {
                Trace.WriteLine("Import params");
                OpenFileDialog openFileDialog = new OpenFileDialog();

                if (openFileDialog.ShowDialog() == true)
                {
                    var config = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);
                    config.MissingFieldFound = null;

                    using (var reader = new StreamReader(openFileDialog.FileName))

                    using (var csv = new CsvReader(reader, config))
                    {
                        csv.Context.RegisterClassMap<CVMap>();
                        csv.Context.RegisterClassMap<DPVMap>();
                        csv.Context.RegisterClassMap<CAMap>();
                        csv.Context.RegisterClassMap<LEDParameterMap>();


                        var experimentRecords = new List<ExperimentModel>();
                        var LEDRecords = new List<LEDParameter>();
                        var isHeader = true;

                        while (csv.Read())
                        {
                            if (isHeader)
                            {
                                csv.ReadHeader();
                                isHeader = false;
                                continue;
                            }

                            if (string.IsNullOrEmpty(csv.GetField(0)))
                            {
                                isHeader = true;
                                continue;
                            }

                            switch (csv.HeaderRecord[0])
                            {
                                case "name":
                                    LEDRecords.Add(csv.GetRecord<LEDParameter>());
                                    break;

                                case "type":
                                    string test = csv.Context.Parser.Record[0];
                                    if (test.ToLower().Contains("dpv"))
                                    {
                                        experimentRecords.Add(csv.GetRecord<DPVModel>());
                                    }
                                    else if (test.ToLower().Contains("ca"))
                                    {
                                        experimentRecords.Add(csv.GetRecord<CAModel>());
                                    }
                                    else if (test.ToLower().Contains("cv"))
                                    {
                                        experimentRecords.Add(csv.GetRecord<CVModel>());
                                    }

                                    else
                                    {
                                        Trace.WriteLine("Not valid test");
                                    }
                                    break;

                                case "pot"://add pots to the import param set-up
                                    break;
                            }
                        }

                        foreach (var LED in LEDRecords)
                        {
                            //LED.IsSelected = true;
                            ExperimentLocal.ledParameters[int.Parse(LED.name)].setParamsFromFile(LED);
                            LedArrayViewModel.isSelected[int.Parse(LED.name)] = true;
                        }

                        experimentRecords[0].setIsEnabled();
                        ExperimentLocal.Model = experimentRecords[0];

                    }
                    return true;

                }

            }
            return false;
        }


        public void fileExport(ExperimentStore ExperimentLocal)
        {
            Trace.WriteLine("Export");
            // OpenFileDialog openFileDialogClose = new OpenFileDialog();
            SaveFileDialog saveFileDialogClose = new SaveFileDialog();
            saveFileDialogClose.DefaultExt = ".csv";
            if (saveFileDialogClose.ShowDialog() == true)
            {
                if (saveFileDialogClose.FileName != "")
                {
                    using (var writer = new StreamWriter(saveFileDialogClose.FileName))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.Context.RegisterClassMap<CVMap>();
                        csv.Context.RegisterClassMap<DPVMap>();
                        csv.Context.RegisterClassMap<CAMap>();
                        csv.Context.RegisterClassMap<LEDParameterMap>();

                        var random = ExperimentLocal.ledParameters;
                        var switchType = ExperimentLocal.Model.getType();
                        csv.WriteRecords<LEDParameter>(random);
                        csv.Flush();

                        switch (switchType)
                        {
                            case "DPVModel":

                                List<DPVModel> enumerableModelDPV = [(DPVModel)ExperimentLocal.Model];
                                csv.NextRecord();
                                csv.WriteField("type");

                                csv.WriteHeader<DPVModel>();
                                csv.NextRecord();

                                csv.WriteField(enumerableModelDPV[0].getType());
                                csv.WriteRecords<DPVModel>(enumerableModelDPV);
                                enumerableModelDPV.Clear();
                                break;
                            case "ca":

                                List<CAModel> enumerableModelCA = [(CAModel)ExperimentLocal.Model];
                                csv.NextRecord();
                                csv.WriteField("type");

                                csv.WriteHeader<CAModel>();
                                csv.NextRecord();

                                csv.WriteField(enumerableModelCA[0].getType());
                                csv.WriteRecords<CAModel>(enumerableModelCA);
                                enumerableModelCA.Clear();
                                break;
                            case "cv":

                                List<CVModel> enumerableModelCV = [(CVModel)ExperimentLocal.Model];
                                csv.NextRecord();
                                csv.WriteField("type");

                                csv.WriteHeader<CVModel>();
                                csv.NextRecord();

                                csv.WriteField(enumerableModelCV[0].getType());
                                csv.WriteRecords<CVModel>(enumerableModelCV);
                                enumerableModelCV.Clear();
                                break;
                            default: break;
                        }


                    }

                }
            }

        }
        //probably deprecated
        public void fileOpen(Object o, ExperimentStore ExperimentLocal, bool dpvEnabled, bool caEnabled, bool cvEnabled, LedArrayViewModel LedArrayViewModel)
        {

            if (bool.Parse(o.ToString()))
            {
                Trace.WriteLine("Import params in class");
                OpenFileDialog openFileDialog = new OpenFileDialog();

                if (openFileDialog.ShowDialog() == true)
                {
                    var config = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);
                    config.MissingFieldFound = null;

                    using (var reader = new StreamReader(openFileDialog.FileName))

                    using (var csv = new CsvReader(reader, config))
                    {
                        csv.Context.RegisterClassMap<CVMap>();
                        csv.Context.RegisterClassMap<DPVMap>();
                        csv.Context.RegisterClassMap<CAMap>();
                        csv.Context.RegisterClassMap<LEDParameterMap>();


                        var experimentRecords = new List<ExperimentModel>();
                        var LEDRecords = new List<LEDParameter>();
                        var isHeader = true;

                        while (csv.Read())
                        {
                            if (isHeader)
                            {
                                csv.ReadHeader();
                                isHeader = false;
                                continue;
                            }

                            if (string.IsNullOrEmpty(csv.GetField(0)))
                            {
                                isHeader = true;
                                continue;
                            }
                            switch (csv.HeaderRecord[0])
                            {
                                case "name":
                                    LEDRecords.Add(csv.GetRecord<LEDParameter>());
                                    break;
                                case "type":
                                    string test = csv.Context.Parser.Record[0];
                                    if (test.ToLower().Contains("dpv"))
                                    {
                                        dpvEnabled = true;
                                        experimentRecords.Add(csv.GetRecord<DPVModel>());
                                    }
                                    else if (test.ToLower().Contains("ca"))
                                    {
                                        caEnabled = true;
                                        experimentRecords.Add(csv.GetRecord<CAModel>());
                                    }
                                    else if (test.ToLower().Contains("cv"))
                                    {
                                        cvEnabled = true;
                                        experimentRecords.Add(csv.GetRecord<CVModel>());
                                    }

                                    else
                                    {
                                        Trace.WriteLine("Not valid test");
                                    }
                                    break;
                                case "pot"://add pots to the import param set-up
                                    break;
                            }
                        }

                        foreach (var LED in LEDRecords)
                        {
                            //LED.IsSelected = true;
                            ExperimentLocal.ledParameters[int.Parse(LED.name)].setParamsFromFile(LED);
                            LedArrayViewModel.isSelected[int.Parse(LED.name)] = true;
                        }

                        experimentRecords[0].setIsEnabled();
                        ExperimentLocal.Model = experimentRecords[0];

                    }
                    int check = 1000; //used as breakpoint 

                }

            }
            else
            {
                Trace.WriteLine("Export in class");
                SaveFileDialog saveFileDialogClose = new SaveFileDialog();
                if (saveFileDialogClose.ShowDialog() == true)
                {
                    if (saveFileDialogClose.FileName != "")
                    {
                        using (var writer = new StreamWriter(saveFileDialogClose.FileName))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            csv.Context.RegisterClassMap<CVMap>();
                            csv.Context.RegisterClassMap<DPVMap>();
                            csv.Context.RegisterClassMap<CAMap>();
                            csv.Context.RegisterClassMap<LEDParameterMap>();

                            var random = ExperimentLocal.ledParameters;
                            var switchType = ExperimentLocal.Model.getType();
                            csv.WriteRecords<LEDParameter>(random);
                            csv.Flush();

                            switch (switchType)
                            {
                                case "DPVModel":

                                    List<DPVModel> enumerableModelDPV = [(DPVModel)ExperimentLocal.Model];
                                    csv.NextRecord();
                                    csv.WriteField("type");

                                    csv.WriteHeader<DPVModel>();
                                    csv.NextRecord();

                                    csv.WriteField(enumerableModelDPV[0].getType());
                                    csv.WriteRecords<DPVModel>(enumerableModelDPV);
                                    enumerableModelDPV.Clear();
                                    break;
                                case "ca":

                                    List<CAModel> enumerableModelCA = [(CAModel)ExperimentLocal.Model];
                                    csv.NextRecord();
                                    csv.WriteField("type");

                                    csv.WriteHeader<CAModel>();
                                    csv.NextRecord();

                                    csv.WriteField(enumerableModelCA[0].getType());
                                    csv.WriteRecords<CAModel>(enumerableModelCA);
                                    enumerableModelCA.Clear();
                                    break;
                                case "cv":

                                    List<CVModel> enumerableModelCV = [(CVModel)ExperimentLocal.Model];
                                    csv.NextRecord();
                                    csv.WriteField("type");

                                    csv.WriteHeader<CVModel>();
                                    csv.NextRecord();

                                    csv.WriteField(enumerableModelCV[0].getType());
                                    csv.WriteRecords<CVModel>(enumerableModelCV);
                                    enumerableModelCV.Clear();
                                    break;
                                default: break;
                            }


                        }

                    }
                }

            }


        }


        public sealed class LEDParameterMap : ClassMap<LEDParameter>
        {
            public LEDParameterMap()
            {
                Map(m => m.name).Name("name");
                Map(m => m.GOnTime);
                Map(m => m.GOffTime);
                Map(m => m.GIntensity);
                Map(m => m.ROnTime);
                Map(m => m.ROffTime);
                Map(m => m.RIntensity);
                Map(m => m.BOnTime);
                Map(m => m.BOffTime);
                Map(m => m.BIntensity);
                Map(m => m.IsSelected).Constant(true).Ignore();
            }
        }
        public sealed class DPVMap : ClassMap<DPVModel>
        {
            public DPVMap()
            {
                Map(m => m.startVoltage).Name("startVoltage");
                Map(m => m.endVoltage);
                Map(m => m.stepSize);
                Map(m => m.scanRate);
                Map(m => m.pulsePotential);
                Map(m => m.pulseTime);
            }
        }
        public sealed class CVMap : ClassMap<CVModel>
        {
            public CVMap()
            {
                Map(m => m.startVoltage).Name("startVoltage");
                Map(m => m.voltageThresholdOne);
                Map(m => m.voltageThresholdTwo);
                Map(m => m.stepSize);
                Map(m => m.scanRate);
                Map(m => m.numOfScans);
            }
        }
        public sealed class CAMap : ClassMap<CAModel>
        {
            public CAMap()
            {
                Map(m => m.voltageRangeStart).Name("voltageRangeStart");
                Map(m => m.voltageRangeEnd);
                Map(m => m.runTimeStart);
                Map(m => m.runTimeEnd);
                Map(m => m.sampleInterval);
            }
        }
    }
}
