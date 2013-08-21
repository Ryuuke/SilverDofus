// By NightWolf

using System;
using System.Globalization;

namespace SilverGame.Models.Maps
{
    class PathFinding
    {
        private string _strPath;
        private readonly int _startCell;
        private readonly int _startDir;

        private readonly Map _map;

        public int Destination { get; private set; }

        public int Direction { get; private set; }

        public PathFinding(string path, Map map, int startCell, int startDir)
        {
            _strPath = path;
            _map = map;
            _startCell = startCell;
            _startDir = startDir;
        }

        public void UpdatePath(string path)
        {
            _strPath = path;
        }

        public string GetStartPath
        {
            get
            {
                return GetDirChar(_startDir) + GetCellChars(_startCell);
            }
        }

        public int GetCaseIDFromDirection(int caseId, char direction, bool fight)
        {
            switch (direction)
            {
                case 'a':
                    return fight ? -1 : caseId + 1;
                case 'b':
                    return caseId + _map.Width;
                case 'c':
                    return fight ? -1 : caseId + (_map.Width * 2 - 1);
                case 'd':
                    return caseId + (_map.Width - 1);
                case 'e':
                    return fight ? -1 : caseId - 1;
                case 'f':
                    return caseId - _map.Width;
                case 'g':
                    return fight ? -1 : caseId - (_map.Width * 2 - 1);
                case 'h':
                    return caseId - _map.Width + 1;
            }

            return -1;
        }

        public static int GetCellNum(string cellChars)
        {
            const string hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

            var numChar1 = hash.IndexOf(cellChars[0]) * hash.Length;
            var numChar2 = hash.IndexOf(cellChars[1]);

            return numChar1 + numChar2;
        }

        public static string GetCellChars(int cellNum)
        {
            const string hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

            var charCode2 = (cellNum % hash.Length);
            var charCode1 = (cellNum - charCode2) / hash.Length;

            return hash[charCode1].ToString(CultureInfo.InvariantCulture) + hash[charCode2].ToString(CultureInfo.InvariantCulture);
        }

        public static string GetDirChar(int dirNum)
        {
            const string hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

            if (dirNum >= hash.Length)
                return "";

            return hash[dirNum].ToString(CultureInfo.InvariantCulture);
        }

        public static int GetDirNum(string dirChar)
        {
            const string hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
            return hash.IndexOf(dirChar, StringComparison.Ordinal);
        }

        public bool InLine(int cell1, int cell2)
        {
            var isX = GetCellXCoord(cell1) == GetCellXCoord(cell2);
            var isY = GetCellYCoord(cell1) == GetCellYCoord(cell2);

            return isX || isY;
        }

        public int NextCell(int cell, int dir)
        {
            switch (dir)
            {
                case 0:
                    return cell + 1;

                case 1:
                    return cell + _map.Width;

                case 2:
                    return cell + (_map.Width * 2) - 1;

                case 3:
                    return cell + _map.Width - 1;

                case 4:
                    return cell - 1;

                case 5:
                    return cell - _map.Width;

                case 6:
                    return cell - (_map.Width * 2) + 1;

                case 7:
                    return cell - _map.Width + 1;

            }

            return -1;
        }

        public string RemakeLine(int lastCell, string cell)
        {
            var direction = GetDirNum(cell[0].ToString(CultureInfo.InvariantCulture));
            var toCell = GetCellNum(cell.Substring(1));

            var lenght = InLine(lastCell, toCell) ? GetEstimateDistanceBetween(lastCell, toCell) : int.Parse(Math.Truncate((GetEstimateDistanceBetween(lastCell, toCell) / 1.4)).ToString(CultureInfo.InvariantCulture));

            var actuelCell = lastCell;

            for (var i = 1; i <= lenght; i++)
            {
                actuelCell = NextCell(actuelCell, direction);
            }

            return cell + ",1";
        }

        public string RemakePath()
        {
            var newPath = string.Empty;
            var lastCell = _startCell;

            for (var i = 0; i <= _strPath.Length - 1; i += 3)
            {
                var actualCell = _strPath.Substring(i, 3);
                var lineData = RemakeLine(lastCell, actualCell).Split(',');
                newPath += lineData[0];

                if (lineData[1] == null)
                    return newPath;

                lastCell = GetCellNum(actualCell.Substring(1));
            }

            Destination = GetCellNum(_strPath.Substring(_strPath.Length - 2, 2));
            Direction = GetDirNum(_strPath.Substring(_strPath.Length - 3, 1));
 
            return newPath;
        }

        public int GetDistanceBetween(int id1, int id2)
        {
            if (id1 == id2 || _map == null) 
                return 0;
            
            var diffX = Math.Abs(GetCellXCoord(id1) - GetCellXCoord(id2));
            var diffY = Math.Abs(GetCellYCoord(id1) - GetCellYCoord(id2));

            return (diffX + diffY);
        }

        public int GetEstimateDistanceBetween(int id1, int id2)
        {
            if (id1 == id2 || _map == null)
                return 0;

            var diffX = Math.Abs(GetCellXCoord(id1) - GetCellXCoord(id2));
            var diffY = Math.Abs(GetCellYCoord(id1) - GetCellYCoord(id2));

            return int.Parse(Math.Truncate(Math.Sqrt(Math.Pow(diffX, 2) + Math.Pow(diffY, 2))).ToString(CultureInfo.InvariantCulture));
        }

        public int GetCellXCoord(int cellid)
        {
            var width = _map.Width;
            return ((cellid - (width - 1) * GetCellYCoord(cellid)) / width);
        }

        public int GetCellYCoord(int cellid)
        {
            var width = _map.Width;
            var loc5 = cellid / ((width * 2) - 1);
            var loc6 = cellid - loc5 * ((width * 2) - 1);
            var loc7 = loc6 % width;

            return (loc5 - loc7);
        }
    }
}
