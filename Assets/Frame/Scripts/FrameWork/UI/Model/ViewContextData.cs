using UnityEngine;
using System.Collections;

namespace FrameWork.UI
{
    // Base window data context for Refresh window or show window
    public class ViewContextData
    {
        public object[] parameters = new object[3];
        public ViewContextData()
        {

        }
        public ViewContextData(object parameter)
        {
            parameters[0] = parameter;
        }

        public ViewContextData(object parameter, object parameter1)
        {
            parameters[0] = parameter;
            parameters[1] = parameter1;
        }
        public ViewContextData(object parameter, object parameter1, object parameter2)
        {
            parameters[0] = parameter;
            parameters[1] = parameter1;
            parameters[2] = parameter2;
        }
    }
}

// Example
// Rank window context data pass to UIRankWindow
//public class WindowContextData_Rank : BaseWindowContextData
//{
//}

// Skill window context data pass to UISkillWindow
//public class WindowContextData_Skill : BaseWindowContextData
//{
//}