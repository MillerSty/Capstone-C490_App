﻿using C490_App.MVVM.Model;
using C490_App.MVVM.ViewModel;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace C490_App.Core
{
    public class FileHandler
    {

        /// <summary>
        /// Import from file function
        /// </summary>
        /// <param name="imexBool">If true, then its redudent ie: never false </param>
        /// <param name="ExperimentLocal"> Is the ExperimentStore</param>
        /// <param name="LedArrayViewModel"> Is used for updating LED values, should be done somewhere else</param>
        /// <returns>true for no errors, false for error</returns>
        public bool fileImport(ExperimentStore ExperimentLocal, LedArrayViewModel LedArrayViewModel, PotentiostatViewModel PotentiostatViewModel)
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

                    //Clear current selections
                    ExperimentLocal.initLedArray();
                    ExperimentLocal.pots = new();
                    ExperimentLocal.Model = new();


                    var experimentRecords = new List<ExperimentModelBase>();
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

                        if (string.IsNullOrEmpty(csv.GetField(0)) || csv.GetField(0).Equals(","))
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
                                string csvParsed = csv.Context.Parser.Record[0];
                                if (csvParsed.ToLower().Contains("dpv"))
                                {
                                    experimentRecords.Add(csv.GetRecord<DPVModel>());
                                }
                                else if (csvParsed.ToLower().Contains("ca"))
                                {
                                    experimentRecords.Add(csv.GetRecord<CAModel>());
                                }
                                else if (csvParsed.ToLower().Contains("cv"))
                                {
                                    experimentRecords.Add(csv.GetRecord<CVModel>());
                                }

                                else
                                {
                                    Trace.WriteLine("Not valid test");
                                }
                                break;

                            case "Pot":
                                int puase = 1000;

                                string s = csv.GetField(0);
                                ExperimentLocal.pots.Add(csv.GetField(0));
                                Trace.WriteLine($"Pot {s} added to experiment");
                                break;
                        }
                    }

                    foreach (var LED in LEDRecords)
                    {
                        //LED.IsSelected = true;
                        ExperimentLocal.ledParameters[int.Parse(LED.Name)].setParamsFromFile(LED);
                        LedArrayViewModel.isSelected[int.Parse(LED.Name)] = true;

                    }

                    //CRASH HERE FOR IM-CHANGE-IM-CRASH
                    //PotentiostatViewModel.potsActive = new();
                    bool check = ExperimentLocal.pots.Any(s => s.Equals(PotentiostatViewModel.potsActive));

                    foreach (var pot in ExperimentLocal.pots)
                    {
                        bool checky = false;
                        //run designated RelayCommand from Pot...ViewModel following methodology used in Pot...ViewModel
                        foreach (var potty in PotentiostatViewModel.potsActive)
                        {

                            if (potty == pot) //wtf is this doing 
                            {
                                checky = true;
                                int pause = 100;
                                break;
                            }
                            if (check)
                            {
                                break;
                            }



                        }
                        if (!checky)
                        {
                            PotentiostatViewModel.SelectedPotName = pot;

                            RelayCommand switchL = new RelayCommand(
                            o => PotentiostatViewModel.SwitchList(o, PotentiostatViewModel.potsActive, PotentiostatViewModel.potsInactive),
                            o => true
                            );
                            switchL.Execute(this);
                        }
                    }
                    //foreach (var pot in PotentiostatViewModel.SelectedPotName)
                    //{
                    //    Trace.WriteLine(pot);
                    //}
                    //system out of range for importing -> deselecting LED -> importing
                    if (experimentRecords.Count() > 0)
                    {
                        experimentRecords[0].setIsEnabled();
                        ExperimentLocal.Model = experimentRecords[0];
                    }
                }
                return true;

            }


            return false;
        }

        /// <summary>
        /// Export experimentStore to file
        /// </summary>
        /// <param name="ExperimentLocal"> Is the ExperimentStore</param>
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
                    //wrap in try catch
                    using (var writer = new StreamWriter(saveFileDialogClose.FileName))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {

                        csv.Context.RegisterClassMap<CVMap>();
                        csv.Context.RegisterClassMap<DPVMap>();
                        csv.Context.RegisterClassMap<CAMap>();
                        csv.Context.RegisterClassMap<LEDParameterMap>();

                        var random = ExperimentLocal.ledParameters;
                        List<LEDParameter> ledCulled = new List<LEDParameter>();
                        foreach (var param in random)
                        {
                            if (param.IsSelected == true)
                            {
                                ledCulled.Add(param);
                            }
                        }
                        var switchType = ExperimentLocal.Model.GetType().Name.ToString();
                        csv.WriteRecords<LEDParameter>(ledCulled);
                        csv.WriteField(",");
                        //csv.Flush();

                        switch (switchType)
                        {
                            case "DPVModel":

                                List<DPVModel> enumerableModelDPV = [(DPVModel)ExperimentLocal.Model];
                                csv.NextRecord();
                                //writer.Flush();
                                csv.WriteField("type");

                                csv.WriteHeader<DPVModel>();
                                csv.NextRecord();
                                // writer.Flush();

                                //TODO this could be split to just write DPV not DPVModel
                                csv.WriteField(enumerableModelDPV[0].GetType().Name.ToString());

                                csv.WriteRecords<DPVModel>(enumerableModelDPV);
                                csv.WriteField(",");

                                //writer.Flush();
                                enumerableModelDPV.Clear();
                                break;
                            case "CAModel":

                                List<CAModel> enumerableModelCA = [(CAModel)ExperimentLocal.Model];
                                csv.NextRecord();
                                csv.WriteField("type");

                                csv.WriteHeader<CAModel>();
                                csv.NextRecord();

                                //TODO this could be split to just write CA not CAModel
                                csv.WriteField(enumerableModelCA[0].GetType().Name.ToString());

                                csv.WriteRecords<CAModel>(enumerableModelCA);
                                csv.WriteField(",");
                                enumerableModelCA.Clear();
                                break;
                            case "CVModel":

                                List<CVModel> enumerableModelCV = [(CVModel)ExperimentLocal.Model];
                                csv.NextRecord();
                                csv.WriteField("type");

                                csv.WriteHeader<CVModel>();
                                csv.NextRecord();

                                //TODO this could be split to just write CV not CVModel
                                csv.WriteField(enumerableModelCV[0].GetType().Name.ToString());

                                csv.WriteRecords<CVModel>(enumerableModelCV);
                                csv.WriteField(",");
                                enumerableModelCV.Clear();
                                break;
                            default: break;
                        }
                        //Write Pots by hand 
                        csv.NextRecord();
                        csv.WriteField("Pot");
                        csv.NextRecord();

                        foreach (var pot in ExperimentLocal.pots)
                        {
                            csv.WriteField(pot);
                            csv.NextRecord();

                        }

                        csv.WriteField(",");

                    }

                }
            }

        }

        /// <summary>
        /// Is used to map LEDParameters for CSV
        /// </summary>
        public sealed class LEDParameterMap : ClassMap<LEDParameter>
        {
            public LEDParameterMap()
            {
                Map(m => m.Name).Name("name");
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
        /// <summary>
        /// Maps DPVModel for CSV
        /// </summary>
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
        /// <summary>
        /// Maps CVModel for CSV
        /// </summary>
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
        /// <summary>
        /// Maps CAModel for CSV
        /// </summary>
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

        public static ReadDataStructureModel ReadDataToArr(string filePath)
        {
            try
            {
                ReadDataStructureModel dataStructureModel = new ReadDataStructureModel();
                List<List<double>> xData = new List<List<double>>();
                List<List<double>> yData = new List<List<double>>();

                using (TextFieldParser parser = new TextFieldParser(filePath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    // Read the first row (Title and Date)
                    string[] headerFields = parser.ReadFields();
                    if (headerFields.Length >= 2)
                    {
                        dataStructureModel.Title = headerFields[0];
                        if (DateTime.TryParse(headerFields[1], out DateTime date))
                        {
                            dataStructureModel.Date = date;
                        }
                        else
                        {
                            throw new Exception("Error parsing date.");
                        }
                    }
                    else
                    {
                        throw new Exception("Error reading title and date fields.");
                    }
                    parser.SetDelimiters(",,"); // since uniqueIDs are separated by double commas
                    // Read the unique identifiers for each table (ex: 1,,2,,3,,4)
                    int[] tableID = Array.ConvertAll(parser.ReadFields(), int.Parse);
                    dataStructureModel.TableIdentifiers = new List<int>(tableID);

                    // Read the axis names
                    parser.SetDelimiters(",");
                    string[] axisNames = parser.ReadFields();
                    dataStructureModel.axisName = new List<string>(axisNames);

                    // Read the data points
                    // iterate through twice the number of unique IDs for each pair of x an y
                    for (int i = 0; i < tableID.Length; i++)
                    {
                        // Read each column of data
                        List<double> xDataColumn = new List<double>();
                        List<double> yDataColumn = new List<double>();

                        // Reset the parser position
                        parser.Close();

                        using (TextFieldParser resetParser = new TextFieldParser(filePath))
                        {
                            resetParser.TextFieldType = FieldType.Delimited;
                            resetParser.SetDelimiters(",");

                            // Skip the first three rows
                            for (int j = 0; j < 3; j++)
                            {
                                resetParser.ReadLine();
                            }

                            // Iterate through the rows
                            while (!resetParser.EndOfData)
                            {
                                string[] fields = resetParser.ReadFields();
                                if (fields.Length >= (i * 2) + 2)
                                {
                                    // Parse and add the values to the corresponding lists
                                    double xValue, yValue;

                                    // Handle nulls by breaking once reached
                                    if (double.TryParse(fields[(i * 2)], out xValue))
                                    {
                                        xDataColumn.Add(xValue);
                                    }
                                    else
                                    {
                                        break;
                                    }

                                    if (double.TryParse(fields[(i * 2) + 1], out yValue))
                                    {
                                        yDataColumn.Add(yValue);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    // Handle incomplete or invalid data row
                                    Console.WriteLine($"Error reading data row {i + 1}.");
                                }
                            }
                        }

                        // Add xDataColumn and yDataColumn to xData and yData lists
                        dataStructureModel.xData.Add(xDataColumn);
                        dataStructureModel.yData.Add(yDataColumn);
                    }
                    //Console.WriteLine("Data read complete.");
                }

                return dataStructureModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV file: {ex.Message}");
                return null;
            }
        }
    }
}
