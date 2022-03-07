using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LineChartComponent : IMGUIContainer
{
    public LineChartComponent()
    {
        this.style.flexGrow = 1;
        this.style.minHeight = 200;
        this.style.minWidth = 400;
        //this.style.backgroundColor = Color.green;
        onGUIHandler = () =>
        {

            var marginBottom = 25;
            var marginTop = 25;
            var marginLeft = 50;
            var marginRight = 50;
            var textMarginBottom = 5;
            var textMarginLeft = 25;
            var textAdjustmentY = 12;
            var graphHeight = this.resolvedStyle.height - marginTop - marginBottom;
            var graphWidth = this.resolvedStyle.width - marginLeft - marginRight;

            var origon = new Vector3(marginLeft, graphHeight, 0);
            var endX = new Vector3(graphWidth, graphHeight, 0);
            var endY = new Vector3(origon.x,  marginTop, 0);

            var labelsX = new List<ChartLabel>();
            var labelsY = new List<ChartLabel>();


            var minX = 0f;
            var maxX = 1f;
            var numberOfPointsX = 10;
            float xValueStepSize = maxX / numberOfPointsX;
            var xStepSize = (graphWidth - marginRight) / numberOfPointsX;

            for(var i = minX; i <= numberOfPointsX; i++)
            {
                var pos = new Vector3(origon.x + i * xStepSize, origon.y, 0);
                var labelPosition = new Vector3(pos.x, pos.y + textMarginBottom, 0);
                var label = new ChartLabel(labelPosition, (i * xValueStepSize).ToString());
                labelsX.Add(label);
                label.Draw();
                var linePositionStart = new Vector3(pos.x, pos.y, 0);
                var linePositionEnd = new Vector3(pos.x, endY.y, 0);
                Handles.DrawLine(linePositionStart, linePositionEnd);
            }

            var minY = 0f;
            var maxY = 1f;
            var numberOfPointsY = 10;
            float yValueStepSize = maxY / numberOfPointsY;
            var yStepSize = (graphHeight - marginTop) / numberOfPointsX;

            for(var i = minY; i <= numberOfPointsY; i++)
            {
                var pos = new Vector3(origon.x-textMarginLeft,origon.y-i*yStepSize, 0);
                var labelPosition = new Vector3(pos.x, pos.y + textMarginBottom - textAdjustmentY, 0);
                var label = new ChartLabel(labelPosition, (i * yValueStepSize).ToString());
                labelsY.Add(label);
                label.Draw();
                var linePositionStart = new Vector3(origon.x, pos.y, 0);
                var linePositionEnd = new Vector3(endX.x, pos.y, 0);
                Handles.DrawLine(linePositionStart, linePositionEnd);
            }

            //foreach (var label in labelsX)
            //{
            //    label.Draw();
            //    label.DrawVerticalLine(graphHeight);
            //}

            //foreach (var label in labelsY)
            //{
            //    label.Draw(false,true);
            //    label.DrawHorizontalLine(graphWidth);
            //}

            //if (GUI.Button(new Rect(10, 10, 50, 50), "text"))
            //{
            //    Debug.Log("H: " + graphHeight + " W: " + graphWidth);

            //}
        };
    }

    private class ChartLabel
    {
        public Vector3 Position;
        public string Text;
        private Vector3 adjusterX = new Vector3(5, 0, 0);
        private Vector3 adjusterY = new Vector3(0, 15, 0);

        public ChartLabel(Vector3 position, string text)
        {
            Position = position;
            Text = text;
        }

        public void Draw(bool isXAxis = false, bool isYAxis = false)
        {
            var p = Position;
            if (isXAxis)
            {
                p += adjusterX;
            }
            if (isYAxis)
            {
                p -= adjusterY;
            }
            Handles.Label(p, Text);
        }

        //public void DrawHorizontalLine(float length)
        //{
        //    Handles.DrawLine(Position, new Vector3(Position.x + length, Position.y, 0));
        //}

        //public void DrawVerticalLine(,float length)
        //{
        //    Handles.DrawLine(Position, new Vector3(Position.x, Position.y - length, 0));
        //}
    }
}
