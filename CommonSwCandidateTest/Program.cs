using System.Collections.Generic;
using CommonSwCandidateTest.Data;

namespace CommonSwCandidateTest_2
{
    // this enum used as parameter for functions to determine data output - Console or CSV-file
    enum OutputType : byte { Console, CSVFile }

    class Program
    {
        static void Main()
        {
            CustomContainer myCont = new CustomContainer(new ItemRepository().GetAllItems()); // creating object of 'CustomDictionaryWrapper' class

            myCont.DisplayHierarchicalTree(OutputType.Console); // call method for displaying hierarchical tree of elements
            myCont.MedianCalculation(); // call method for calculation and print median value of 'Completed' property
            myCont.CompletedRecalculate(); // call method for recalculation 'Completed' property of objects
            myCont.DisplayHierarchicalTree(OutputType.CSVFile); // call method for creating data for writing to file
            myCont.WriteCSV(); // call method for writing data to CSV file
        }
    }
}
