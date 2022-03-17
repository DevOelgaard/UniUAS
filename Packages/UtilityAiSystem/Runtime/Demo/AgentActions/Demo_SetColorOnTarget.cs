using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_SetColorOnTarget : AgentAction
{
    public Demo_SetColorOnTarget()
    {
        HelpText = "The parent must set a " + AiContextKey.CurrentTargetGameObject + " for it to act on";
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Color OnStart", Color.white),
            new Parameter("Set OnStart", true),
            new Parameter("Color OnGoing", Color.blue),
            new Parameter("Set OnGoing", true),
            new Parameter("Color OnEnd", Color.black),
            new Parameter("Set OnEnd", true),
        };
    }

    public override void OnStart(AiContext context)
    {
        base.OnGoing(context);
        if ((bool)Parameters[1].Value)
        {
            var targetRenderer = GetTargetRenderer(context);
            targetRenderer.material.SetColor("_Color", (Color)Parameters[0].Value);
        }
    }

    public override void OnGoing(AiContext context)
    {
        base.OnGoing(context);
        if ((bool)Parameters[3].Value)
        {
            var targetRenderer = GetTargetRenderer(context);
            targetRenderer.material.SetColor("_Color", (Color)Parameters[4].Value);
        }
    }

    public override void OnEnd(AiContext context)
    {
        base.OnGoing(context);
        if ((bool)Parameters[5].Value)
        {
            var targetRenderer = GetTargetRenderer(context);
            targetRenderer.material.SetColor("_Color", (Color)Parameters[6].Value);
        }
    }

    private Renderer GetTargetRenderer(AiContext context)
    {
        var address = context.LastSelectedDecision.GetContextAddress(context);
        return context.GetContext<GameObject>((address + AiContextKey.CurrentTargetGameObject)).GetComponent<Renderer>();
    }
}
