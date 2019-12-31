using System;
using System.Collections.Generic;
using CommonSwCandidateTest.Data;

namespace CommonSwCandidateTest_2
{
    public class BaseItemComparer : IComparer<BaseItem>
    {
        public int Compare(BaseItem a, BaseItem b)
        {
            // first, we compare by 'PlannedStart'
            int result = DateTime.Compare(a.PlannedStart, b.PlannedStart);
            if (result == 0)
            { // if previous comparison returns '0' - dates are equal
                result = DateTime.Compare(a.PlannedEnd, b.PlannedEnd); // then compare by 'PlannedEnd'
                if (result == 0) // if previous comparison returns '0' - dates are equal
                    result = string.Compare(a.Name, b.Name); // and then compare by 'Name'
            }
            return result;
        }
    }
}
