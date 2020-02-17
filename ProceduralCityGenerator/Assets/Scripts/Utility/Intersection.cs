using System.Collections.Generic;

namespace Assets.Scripts.Utility
{
    public class Intersection
    {
        public List<Point> Points;

        public Intersection(List<Point> points)
        {
            Points = points;
        }

        public override bool Equals(object obj)
        {
            Intersection interObj = obj as Intersection;

            int countSim = 0;
            foreach (var point in Points)
            {
                if (interObj.Points.Exists(o => o == point))
                    countSim++;
            }

            if (countSim == Points.Count && countSim == interObj.Points.Count)
                return true;

            return false;
        }
    }
}

