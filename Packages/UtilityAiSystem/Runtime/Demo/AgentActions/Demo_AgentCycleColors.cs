using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_AgentCycleColors : AgentAction
{
    private Renderer renderer;
    private int currentColorIndex = 0;
    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Color 1", Color.white),
            new Parameter("Color 2", Color.white),
            new Parameter("Color 3", Color.white),
            new Parameter("Color 4", Color.white),
            new Parameter("Color 5", Color.white),
        };
    }

    public override void OnGoing(AiContext context)
    {
        if (renderer == null)
        {
            //Debug.Log("Getting renderer for: " + context.Agent.Model.Name);
            renderer = GameObject.Find(context.Agent.Model.Name).GetComponent<Renderer>();
        }

        currentColorIndex++;
        if (currentColorIndex >= Parameters.Count)
        {
            currentColorIndex = 0;
        }
        renderer.material.SetColor("_Color", (Color)Parameters[currentColorIndex].Value);
    }
}