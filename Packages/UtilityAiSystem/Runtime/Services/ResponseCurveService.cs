//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEditor;

//public class ResponseCurveService
//{
//    private static List<string> curveNames = new List<string>();
//    public static List<string> GetResponseCurveNames()
//    {
//        if (curveNames.Count <= 0)
//        {
//            var list = AssetDatabaseService.GetInstancesOfType<ResponseCurve>();
//            curveNames = list.Select(x => x.Name).ToList();
//        }
//        return curveNames;
//    }
//}