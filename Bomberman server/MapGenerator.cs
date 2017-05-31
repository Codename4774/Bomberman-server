using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bomberman_client.GameClasses;
using System.Drawing;

namespace Bomberman_server
{
    public class MapGenerator
    {
        public enum LineDirection { Horizontal, Vertical }
        public static List<PhysicalObject> GenerateLineStaticWall(Point location, int count, LineDirection lineDirection, Size wallSize)
        {
            List<PhysicalObject> result = new List<PhysicalObject>();
            switch (lineDirection)
            {
                case LineDirection.Horizontal:
                    {
                        for (int i = 0; i < count; i++)
                        {
                            result.Add(new PhysicalObject(new Point(location.X + wallSize.Width * i, location.Y), wallSize));
                        }
                    }
                    break;
                case LineDirection.Vertical:
                    {
                        for (int i = 0; i < count; i++)
                        {
                            result.Add(new PhysicalObject(new Point(location.X, location.Y + wallSize.Height * i), wallSize));
                        }
                    }
                    break;
            }
            return result;
        }
        public static List<DynamicWall> GenerateLineDynamicWall(Point location, int count, LineDirection lineDirection, Size wallSize)
        {
            List<DynamicWall> result = new List<DynamicWall>();
            switch (lineDirection)
            {
                case LineDirection.Horizontal:
                    {
                        for (int i = 0; i < count; i++)
                        {
                            result.Add(new DynamicWall(new Point(location.X + wallSize.Width * i, location.Y), wallSize));
                        }
                    }
                    break;
                case LineDirection.Vertical:
                    {
                        for (int i = 0; i < count; i++)
                        {
                            result.Add(new DynamicWall(new Point(location.X, location.Y + wallSize.Height * i), wallSize));
                        }
                    }
                    break;
            }
            return result;
        }
    }
}
