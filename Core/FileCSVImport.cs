using C490_App.MVVM.Model;
using Microsoft.VisualBasic.FileIO;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Input;

namespace C490_App.Core
{
    public class FileCSVImport
    {
        // Function to read data from CSV file into the desired data structure
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
