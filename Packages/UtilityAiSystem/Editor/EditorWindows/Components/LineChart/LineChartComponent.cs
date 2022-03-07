using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

    private float graphMinY = 0.5f;
    private float graphMaxY = 1f;
    private float graphRangeY => graphMaxY - graphMinY;
    private float stepCountX = 10f;
    private float stepCountY = 10f;

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

            DrawBaseGraph();
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

            var labelPosition = new Vector3(basePosition.x + textXAxisAdjuster, basePosition.y - marginTextBottom, 0);
            Handles.Label(labelPosition, x.ToString());
        }

        // Y labels
        Handles.color = Color.grey;
        var stepSizeY = graphRangeY/ stepCountY;
        for (var i = 0; i <= stepCountY; i++)
        {
            var y = i * stepSizeY + graphMinY;
            var basePosition = GraphToScreenCoordinates(graphMinX, y);
            Handles.DrawLine(basePosition, GraphToScreenCoordinates(graphMaxX, y), 0.01f);

            var labelPosition = new Vector3(basePosition.x - marginTextLeft, basePosition.y - textYAxisAdjuster, 0);
            Handles.Label(labelPosition, y.ToString());
        }


        // Base Lines
        Handles.color = Color.white;
        var xAxixEnd = GraphToScreenCoordinates(graphMaxX, graphMinY);
        var yAxixEnd = GraphToScreenCoordinates(graphMinX, graphMaxY);
        Handles.DrawLine(graphOrigon, xAxixEnd);
        Handles.DrawLine(graphOrigon, yAxixEnd);

        Debug.Log("GraphOrigon: " + graphOrigon + " xAxisEnd: " + xAxixEnd + " yAxisEnd: " + yAxixEnd);
    }

    private Vector3 GraphToScreenCoordinates(float graphX, float graphY)
    {
        return GraphToScreenCoordinates(new Vector3(graphX, graphY, 0));
    }

    private Vector3 GraphToScreenCoordinates(Vector3 graphPos)
    {
        var x = graphOrigon.x + (graphPos.x - graphMinX) / graphRangeX * graphWidth;
        var y = graphOrigon.y - (graphPos.y - graphMinY) / graphRangeY * graphHeight;
        return new Vector3(x,y,0);
    }
}
