using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Mock_Bucket: Bucket
{
    public float ReturnValue { get; private set; }

    public Mock_Bucket(float returnValue = 0f) 
    {
        ReturnValue = returnValue;
        LastCalculatedUtility = returnValue;
    }


    internal override float GetUtility(AiContext context)
    {
        LastCalculatedUtility = ReturnValue;
        return ReturnValue;
    }
}
