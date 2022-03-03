using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

public class ResponseCurveService
{
    static List<string> responseCurveNames = new List<string>
    {
        "Exponential",
        "Inverse Logistic",
        "Linear",
        "Logistic"
    };
    public static List<string> GetResponseCurveNames()
    {
        return responseCurveNames;
    }

    public static ResponseCurveModel GetResponseCurve(string name)
    {
        if (name == "Exponential")
            return new RCExponential();

        if (name == "Inverse Logistic")
            return new RCInverseLogistic();

        if (name == "Linear")
            return new RCLinear();

        if (name == "Logistic")
            return new RCLogistic();

        throw new Exception("Name: " + name + " does not exist"); 
    }
}