using System;
using System.Collections.Generic;
using CommonSwCandidateTest.Data;

namespace CommonSwCandidateTest_2
{
    public class BaseItemComparer : IComparer<BaseItem>
    {
        public int Compare(BaseItem a, BaseItem b)
        {
            // first, we compare by 'PlannedStart', if it's comparison returns '0' - dates are equal
            var result = DateTime.Compare(a.PlannedStart, b.PlannedStart);
            if (result == 0)
            {
                // then compare by 'PlannedEnd'
                // if it's comparison returns '0' - dates are equal
                result = DateTime.Compare(a.PlannedEnd, b.PlannedEnd); 
                if (result == 0)
                {
                    // and then compare by 'Name'
                    result = string.Compare(a.Name, b.Name);
                }                   
            }
            return result;
        }
    }
}
