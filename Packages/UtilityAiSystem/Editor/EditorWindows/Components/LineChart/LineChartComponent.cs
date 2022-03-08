using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UniRxExtension;

internal class LineChartComponent: IMGUIContainer
{
    private float marginLeft = 50f;
    private float marginRight = 50f;
    private float marginTop = 25f;
    private float marginBottom = 25f;
    private float marginTextBottom = -7.5f;
    private float marginTextLeft = 30f;
    private float textXAxisAdjuster = -3f;
    private float textYAxisAdjuster = 5f;
    private float screenHeight;
    private float screenWidth;
    private float graphHeight;
    private float graphWidth;
    private Vector3 graphOrigon;


    private float graphMinX = 45000f;
    private float graphMaxX = 50000f;
    private float graphRangeX => graphMaxX - graphMinX;

    private float graphMinY = 0f;
    private float graphMaxY = 1f;
    private float graphRangeY => graphMaxY - graphMinY;
    private float stepCountX = 10f;
    private float stepCountY = 10f;
    private List<Vector2> points = new List<Vector2>();

    private float TESTCalculateY(float x)
    {
        var xNormalized = (x - graphMinX) / graphRangeX;
        var y = 1 * xNormalized + 0;
        return y;
    }

    public void DrawCurve(List<Vector2> points, float min = 0, float max = 1, int steps = 100, int stepCountX = 10)
    {
        this.stepCountX = stepCountX;
        graphMinX = min;
        graphMaxX = max;
        this.points = points;

        MarkDirtyRepaint();
    }

    public void DrawCurve()
    {
        for (var i = 1; i < points.Count; i++)
        {
            var p1 = points[i - 1];
            var p2 = points[i];

            var screenCoordP1 = GraphToScreenCoordinates(p1);
            var screenCoordP2 = GraphToScreenCoordinates(p2);
            Handles.color = Color.green;
            Handles.DrawLine(screenCoordP1, screenCoordP2);
        }
    }

    public LineChartComponent()
    {
        style.flexGrow = 1;
        style.minHeight = 200;
        style.minWidth = 400;

        onGUIHandler = () =>
        {
            // Init
            screenHeight = resolvedStyle.height;
            screenWidth = resolvedStyle.width;
            graphHeight = screenHeight - marginTop - marginBottom;
            graphWidth = screenWidth - marginLeft - marginRight;
            graphOrigon = new Vector3(marginLeft, screenHeight - marginBottom, 0);

            //#region TEST
            //var demoSteps = 150;
            //var stepSize = graphRangeX / demoSteps;
            //points = new List<Vector2>();
            //for (var i = 0; i <= demoSteps; i++)
            //{
            //    var x = i * stepSize + graphMinX;
            //    points.Add(new Vector2(x, TESTCalculateY(x)));
            //}
            //#endregion
            DrawBaseGraph();
            
            DrawCurve();
        };
    }

    private void DrawBaseGraph()
    {
        // X labels
        Handles.color = Color.grey;
        var stepSizeX = graphRangeX / stepCountX;
        for (var i = 0; i <= stepCountX; i++)
        {
            var x = i * stepSizeX + graphMinX;
            var basePosition = GraphToScreenCoordinates(x, graphMinY);
            Handles.DrawLine(basePosition, GraphToScreenCoordinates(x, graphMaxY), 0.01f);

            var labelPosition = new Vector2(basePosition.x + textXAxisAdjuster, basePosition.y - marginTextBottom);
            Handles.Label(labelPosition, x.ToString("0.00"));
        }

        // Y labels
        Handles.color = Color.grey;
        var stepSizeY = graphRangeY/ stepCountY;
        for (var i = 0; i <= stepCountY; i++)
        {
            var y = i * stepSizeY + graphMinY;
            var basePosition = GraphToScreenCoordinates(graphMinX, y);
            Handles.DrawLine(basePosition, GraphToScreenCoordinates(graphMaxX, y), 0.01f);

            var labelPosition = new Vector2(basePosition.x - marginTextLeft, basePosition.y - textYAxisAdjuster);
            Handles.Label(labelPosition, y.ToString("0.0"));
        }


        // Base Lines
        Handles.color = Color.white;
        var xAxixEnd = GraphToScreenCoordinates(graphMaxX, graphMinY);
        var yAxixEnd = GraphToScreenCoordinates(graphMinX, graphMaxY);
        Handles.DrawLine(graphOrigon, xAxixEnd);
        Handles.DrawLine(graphOrigon, yAxixEnd);
    }

    private Vector3 GraphToScreenCoordinates(float graphX, float graphY)
    {
        return GraphToScreenCoordinates(new Vector2(graphX, graphY));
    }

    private Vector3 GraphToScreenCoordinates(Vector2 graphPos)
    {
        var x = graphOrigon.x + (graphPos.x - graphMinX) / graphRangeX * graphWidth;
        var y = graphOrigon.y - (graphPos.y - graphMinY) / graphRangeY * graphHeight;
        return new Vector3(x,y,0);
    }
}
