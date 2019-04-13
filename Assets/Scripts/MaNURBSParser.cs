using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public static class MaNURBSParser
{
    //TODO Хоть как то пофиксить этот говнокод
    public static List<Curve> Parse(string fileName)
    {
        var curves = new List<Curve>();
        var streamReader = File.OpenText(fileName);
        var wpText = streamReader.ReadToEnd();
        wpText = wpText.Remove(wpText.IndexOf("select -ne")).Remove(0, wpText.IndexOf("createNode transform -n"));

        var nodes = wpText.Split(new[] {"createNode transform -n "}, StringSplitOptions.RemoveEmptyEntries);

        foreach (var node in nodes)
        {
            var curve = new Curve {Name = node.Substring(0, node.IndexOf(';')).Replace("\"", "")};

            var attrNodes = node.Substring(node.IndexOf("setAttr"))
                .Split(new[] {"setAttr"}, StringSplitOptions.RemoveEmptyEntries);

            var pointsNode = attrNodes.FirstOrDefault(s => s.Contains("nurbsCurve"));

            var start = pointsNode.IndexOf("2582\n") + "2582\n".Length;
            var coordinates = pointsNode.Substring(start, pointsNode.IndexOf(';') - start + 1);

            var coordinateLines = coordinates.Split('\n');

            curve.CreateCoordinateMassives(coordinateLines.Length);

            for (var index = 0; index < coordinateLines.Length - 1; index++)
            {
                var line = coordinateLines[index].Trim();
                var xyz = line.Split(' ');

                var x = float.Parse(xyz[0], CultureInfo.InvariantCulture.NumberFormat);
                var y = float.Parse(xyz[1], CultureInfo.InvariantCulture.NumberFormat);
                var z = float.Parse(xyz[2], CultureInfo.InvariantCulture.NumberFormat);

                curve.xyzPoints[index] = new Vector3(x, y, z);
            }

            curves.Add(curve);
        }

        return curves;
    }
}