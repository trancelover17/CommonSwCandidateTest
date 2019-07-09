using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommonSwCandidateTest.Data;

namespace CommonSwCandidateTest
{
    // custom comparer for objects of 'BaseItem'
    public class BaseItemComparer : IComparer<BaseItem>
    {
        public int Compare(BaseItem a, BaseItem b)
        {
            // first, we compare by 'PlannedStart'
            int result = DateTime.Compare(a.PlannedStart, b.PlannedStart);
            if (result == 0) // if previous comparison returns '0' - dates are equal
            {
                result = DateTime.Compare(a.PlannedEnd, b.PlannedEnd); // then compare by 'PlannedEnd'
                if (result == 0) // if previous comparison returns '0' - dates are equal
                    result = string.Compare(a.Name, b.Name); // and then compare by 'Name'
            }            
            return result;
        }
    }
    public class CustomContainer
    {
        Dictionary<int, BaseItem> items_collection = new Dictionary<int, BaseItem>(); //private collection for storing 'BaseItem' elements
        BaseItemComparer bic = new BaseItemComparer(); // custom comparer for sorting 'BaseItem' values
        Stack<BaseItem> myStack = new Stack<BaseItem>(); // stack of 'BaseItem' elements for recalculation 'Completed' property in correct order
        string cvs_string = string.Empty; // string with values for writing to CVS file

        public void Add(int key, BaseItem value) // function for adding elements to private collection 'items_collection'
        {
            if (!items_collection.ContainsKey(key)) // check if item with 'key' exists in private dictionary
                items_collection.Add(key, value);
            else // item with this 'key' already exists in collection
                Console.WriteLine("Item with Id = {0} already exists in collection", value.Id);
        }
        /// <summary>
        /// Function for median calculation
        /// </summary>
        public void MedianCalculation()
        {
            double median = 0;
            List<byte> coml_vals = items_collection.Select(x => x.Value.Completed).ToList();
            coml_vals.Sort();
            if (coml_vals.Count % 2 != 0)
                median = (double)coml_vals[coml_vals.Count / 2];
            median = (double)(coml_vals[(coml_vals.Count - 1) / 2] + coml_vals[coml_vals.Count / 2]) / 2.0;
            Console.WriteLine("Median is: {0}", median);
        }
        /// <summary>
        /// Displaying hierarchical tree of elements
        /// </summary>
        public void DisplayHierarchicalTree()
        {
            Console.WriteLine("Hierarchical tree of items provided by ItemRepository.GetAllItems()"
                + " method from CommonSwCandidateTest.Data.dll.");

            var roots = items_collection.Values.Where(x => x.ParentId == null).ToList(); // selecting root-elements
            var items_lsit = items_collection.Values.ToList(); // create collection of all 'BaseItem' values

            foreach (var root_item in roots) // loop for every root-element
            {
                myStack.Push(root_item); // push root-element to stack for 'Completed' property recalculation
                ProcessSubItems(items_lsit, root_item, 1); // call method for processing all sub-items of root-element
            }
        }
        /// <summary>
        /// Processes all sub-items of element
        /// </summary>
        /// <param name="all_elements">List of all elements in collection</param>
        /// <param name="current">Item, which sub-items should be processed</param>
        /// <param name="level">Tree level counter for displaying and writing</param>
        void ProcessSubItems(List<BaseItem> all_elements, BaseItem current, int level)
        {
            Console.WriteLine(new string('-', level) + current.Id + " " + current.ParentId + " " + current.Name
                + " " + current.PlannedStart + " " + current.PlannedEnd);       

            var childrens = all_elements.Where(x => x.ParentId == current.Id).ToList(); // get all childrens of 'current' item

            // if item has any childs, it's 'Completed' property will be recalculated (even if the value not
            // changed - we need to recalculate it), so we write this information to file
            bool compl_recalc = childrens.Count > 0 ? true : false; 

            cvs_string += new string('-', level) + "Level: " + level + " Name: " + current.Name + " PlannedStart:"
                + current.PlannedStart + " PlannedEnd: " + current.PlannedEnd + " Completed: " + current.Completed
                + " Was completition calculated: " + compl_recalc + "\n";

            foreach (var child in childrens) // push all childrens to 'myStack' for 'Completed' property recalculation
                myStack.Push(child);

            childrens.Sort(bic); // sort all childrens by custom comparer
            ++level; // increase level by 1 (means we go deep by one level)
            foreach (var school in childrens) // recursive call (search all sub-items of all childrens of 'current' element)
                ProcessSubItems(all_elements, school, level);
        }
        /// <summary>
        /// Recalculation 'Completed' property of objects
        /// </summary>
        public void CompletedRecalculate()
        {
            int previous_parent_id = 0;     // store 'ParentId' of previous item in stack
            int completed_sum = 0;          // store sum of 'Completed' property of items with same 'ParentId'
            int counter = 0;                // stores number of items with same 'ParentId'
            while (myStack.Count > 0) // loop until stack is empty
            {
                BaseItem item = myStack.Pop(); // pop element from stack
                if (item.ParentId == null) // if true, this means it is root element
                {
                    if (completed_sum != 0 && counter != 0) // if true, this means we calculated value for root element and must save it
                        items_collection[previous_parent_id].Completed = (byte)(completed_sum / counter);
                    counter = 0;
                    completed_sum = 0;
                }
                // this item is refers to another parent, so 'Completed' property of previous parent should be saved
                else if (previous_parent_id != item.ParentId.GetValueOrDefault())
                {
                    // set completed property of parent by value 'completed_sum / counter'
                    if (completed_sum != 0 && counter != 0)
                        items_collection[previous_parent_id].Completed = (byte)(completed_sum / counter);
                    previous_parent_id = item.ParentId.GetValueOrDefault(); // now set current item ParentId as 'previous_parent_id'
                    completed_sum = item.Completed; // and 'completed_sum' will contain only value from current element
                    counter = 0; // reset counter
                }
                // item refers to the same parent
                else completed_sum += item.Completed; // add current 'Completed' value to 'completed_sum'
                counter++; // and increase counter
            }
        }
        public void WriteCSV()
        {
            var filepath = "C:\\dev\\hiararchical-Vyacheslav.csv";
            using (StreamWriter writer = new StreamWriter(new FileStream(filepath, FileMode.Create, FileAccess.Write)))
                writer.Write(cvs_string);
        }
    }
    class Program
    {
        static void Main()
        {
            ItemRepository it = new ItemRepository();               // object for call 'GetAllItems()' method
            IEnumerable<BaseItem> ItemsList = it.GetAllItems();     // getting all items
            CustomContainer myDict = new CustomContainer();         // creating object of 'CustomDictionaryWrapper' class

            foreach (var l in ItemsList) // add all items from 'ItemsList' to internal collection in 'myDict'
                myDict.Add(l.Id, l);

            myDict.DisplayHierarchicalTree();   // call method for displaying hierarchical tree of elements
            myDict.MedianCalculation();         // call method for calculation and print median value of 'Completed' property
            myDict.CompletedRecalculate();      // call method for recalculation 'Completed' property of objects
            myDict.WriteCSV();                  // call method for writing data to CSV file
        }
    }
}
