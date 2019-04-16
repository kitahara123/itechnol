using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class MaNURBSParser
{
    // Не нашел парсер для ma, пришлось написать свой
    public static List<Curve> Parse(string fileName)
    {
        var curves = new List<Curve>();
        var streamReader = File.OpenText(fileName);
        var wpText = streamReader.ReadToEnd();
        
        Regex regex = new Regex(@"setAttr[^;]*nurbsCurve[^;]*");
        MatchCollection matches = regex.Matches(wpText);

        foreach (Match match in matches)
        {
            var curve = new Curve();

            var fullString = match.Value;
            var reg = new Regex(@"\t[0-9]* 0 1 2 3");
            var keyString = reg.Match(fullString).Value.Trim();
            var key = keyString.Substring(0, keyString.Length - " 0 1 2 3".Length);

            var coordinates = fullString.Substring(fullString.IndexOf("\t" + key + "\n\t") + key.Length + 1).Trim();

            var coordinateLines = coordinates.Split('\n');

            curve.CreateCoordinateMassive(coordinateLines.Length);

            for (var index = 0; index < coordinateLines.Length; index++)
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