using System;
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
            ItemRepository it = new ItemRepository(); // object for call 'GetAllItems()' method
            IEnumerable<BaseItem> ItemsList = it.GetAllItems(); // getting all items
            CustomContainer myCont = new CustomContainer(); // creating object of 'CustomDictionaryWrapper' class

            foreach (var l in ItemsList) // add all items from 'ItemsList' to internal collection in 'myDict'
                myCont.Add(l.Id, l);

            myCont.DisplayHierarchicalTree(OutputType.Console); // call method for displaying hierarchical tree of elements
            myCont.MedianCalculation(); // call method for calculation and print median value of 'Completed' property
            myCont.CompletedRecalculate(); // call method for recalculation 'Completed' property of objects
            myCont.DisplayHierarchicalTree(OutputType.CSVFile); // call method for creating data for writing to file
            myCont.WriteCSV(); // call method for writing data to CSV file

            Console.ReadKey();
        }
    }
}
