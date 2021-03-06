﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommonSwCandidateTest.Data;

namespace CommonSwCandidateTest_2
{
    class CustomContainer
    {
        // TODO: Write your own path!!!
        private string filepath = @"C:\Users\vyacheslav.v.sorokin\Documents\!Разработки мои\Csharp\CommonSwCandidateTest\hiararchical-Vyacheslav.csv";

        Dictionary<int, BaseItem> items_collection = new Dictionary<int, BaseItem>(); //private collection for storing 'BaseItem' elements
        public static BaseItemComparer bic = new BaseItemComparer(); // custom comparer for sorting 'BaseItem' values
        Stack<BaseItem> myStack = new Stack<BaseItem>(); // stack of 'BaseItem' elements for recalculation 'Completed' property in correct order
        StringBuilder csv_string = new StringBuilder(); // string with values for writing to CVS file

        /// <summary>
        /// initializes new object of 'CustomContainer' by given collection
        /// </summary>
        /// <param name="key">Key of element in collection</param>
        /// <param name="value">Value of element in collection</param>
        public CustomContainer(IEnumerable<BaseItem> items)
        {
            // add all items from 'items' to internal collection in 'myDict'
            items_collection = items.ToDictionary(key => key.Id, value => value);
        }

        /// <summary>
        /// Function for median calculation
        /// </summary>
        public void MedianCalculation()
        {
            /* I'm using here standard 'Sort()' method for array sorting, which is worst-case O(n^2) complexity.
             * But, for more efficiency I might use here 'Median of medians-algorithm' which finds an approximate 
             * median in linear time only. But, in the task weren't any complexity requirements, so I use 
             * standard 'Sort()' for more easy and fast solution. But this is a good point to create a new feature
             */
            var median = 0; // median value
            byte[] coml_vals = items_collection.Select(x => x.Value.Completed).ToArray(); // get all 'Completed' values
            Array.Sort(coml_vals); // sort array
            if (coml_vals.Length % 2 != 0) // if count of elements is odd, median - is middle element
                median = coml_vals[coml_vals.Length / 2];
            // else median is half-sum of two middle elements
            else median = (coml_vals[(coml_vals.Length - 1) / 2] + coml_vals[coml_vals.Length / 2]) / 2;
            Console.WriteLine($"Median is: {median}");
        }

        /// <summary>
        /// Displaying hierarchical tree of elements
        /// </summary>
        public void DisplayHierarchicalTree(OutputType output)
        {
            if (output == OutputType.Console)
            {
                Console.WriteLine(@"Hierarchical tree of items provided by ItemRepository.GetAllItems() 
                    method from CommonSwCandidateTest.Data.dll.");
            }

            var roots = items_collection.Values.Where(x => x.ParentId == null).ToArray(); // selecting root-elements
            var items_lsit = items_collection.Values.ToArray(); // create collection of all 'BaseItem' values

            foreach (var root_item in roots) // loop for every root-element
            {
                myStack.Push(root_item); // push root-element to stack for 'Completed' property recalculation
                ProcessSubItems(items_lsit, root_item, 1, output); // call method for processing all sub-items of root-element
            }
        }

        /// <summary>
        /// Processes all sub-items of element
        /// </summary>
        /// <param name="all_elements">Array of all elements in collection</param>
        /// <param name="current">Item, which sub-items should be processed</param>
        /// <param name="level">Tree level counter for displaying and writing data</param>
        void ProcessSubItems(BaseItem[] all_elements, BaseItem current, int level, OutputType output)
        {
            if (output == OutputType.Console)
            {
                Console.WriteLine(new string('-', level) + current.Id + " " + current.ParentId + " " + current.Name
                + " " + current.PlannedStart + " " + current.PlannedEnd);
            }

            var childrens = all_elements.Where(x => x.ParentId == current.Id).ToArray(); // get all childrens of 'current' item

            if (output == OutputType.CSVFile)
            {
                // if item has any childs, it's 'Completed' property will be recalculated (even if the value not
                // changed - we need to recalculate it), so we write this information to file
                bool is_recalculated = childrens.Length > 0 ? true : false;

                csv_string.Append(('-', level) + "Level: " + level + " Name: " + current.Name + " PlannedStart:"
                    + current.PlannedStart + " PlannedEnd: " + current.PlannedEnd + " Completed: " + current.Completed
                    + " Was completition calculated: " + is_recalculated + "\n");
            }

            foreach (var child in childrens) // push all childrens to 'myStack' for 'Completed' property recalculation
                myStack.Push(child);

            if (output == OutputType.Console) Array.Sort(childrens, bic); // sort all childrens by custom comparer
            ++level; // increase level by 1 (means we go deep by one level)
            foreach (var school in childrens) // recursive call (search all sub-items of all childrens of 'current' element)
                ProcessSubItems(all_elements, school, level, output);
        }

        /// <summary>
        /// Recalculation 'Completed' property of objects
        /// </summary>
        public void CompletedRecalculate()
        {
            var previous_parent_id = 0; // store 'ParentId' of previous item in stack
            var completed_sum = 0; // store sum of 'Completed' property of items with same 'ParentId'
            var counter = 0; // stores number of items with same 'ParentId'
            while (myStack.Count > 0) // loop until stack is empty
            {
                BaseItem item = myStack.Pop(); // pop element from stack
                if (item.ParentId == null) // if true, this means it is root element
                {
                    if (completed_sum != 0 && counter != 0) // if true, this means we calculated value for root element and must save it
                        items_collection[previous_parent_id].Completed = (byte)(completed_sum / counter);
                    counter = 0; // reset value to 0
                    completed_sum = 0; // reset value to 0 
                }
                // this item is refers to another parent, so 'Completed' property of previous parent should be saved
                else if (previous_parent_id != item.ParentId.GetValueOrDefault())
                {
                    // set completed property of parent by value 'completed_sum / counter'
                    if (completed_sum != 0 && counter != 0)
                        items_collection[previous_parent_id].Completed = (byte)(completed_sum / counter);
                    previous_parent_id = item.ParentId.GetValueOrDefault(); // now set current item ParentId as 'previous_parent_id'
                    completed_sum = item.Completed; // and 'completed_sum' will contain only value from current element
                    counter = 1; // set counter to 0 
                }
                // item refers to the same parent
                else
                {
                    completed_sum += item.Completed; // add current 'Completed' value to 'completed_sum'
                    counter++; // and increase counter
                }
            }
        }

        /// <summary>
        /// Writes data to CSV file on disk
        /// </summary>
        public void WriteCSV()
        {
            // writing data using StreamWriter
            using (StreamWriter writer = new StreamWriter(new FileStream(filepath, FileMode.Create, FileAccess.Write)))
                writer.Write(csv_string);
        }
    }
}
