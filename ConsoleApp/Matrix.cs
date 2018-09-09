using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp
{
    public class Matrix
    {
        #region Construction
        public Matrix(int cols, params double[] elements) :
            this(cols > 0 ? (elements?.Length ?? 0) / cols : 0, cols, elements)
        {
        }

        public Matrix(IReadOnlyCollection<IReadOnlyCollection<double>> elements) :
            this(elements?.Count ?? 0, elements?.FirstOrDefault()?.Count ?? 0, elements?.SelectMany(x => x ?? Array.Empty<double>()))
        {
        }

        public Matrix(int rows, int cols, IEnumerable<double> elements)
        {
            if (rows < 0)
                throw new ArgumentOutOfRangeException($"{nameof(rows)} = {rows}");
            if (cols < 0)
                throw new ArgumentOutOfRangeException($"{nameof(cols)} = {cols}");
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            this.Rows = rows;
            this.Columns = cols;
            this.elements = new double[rows][];
            using (var enumerator = elements.GetEnumerator())
            {
                for (int r = 0; r < rows; r++)
                {
                    var row = new double[cols];
                    for (int c = 0; c < cols; c++)
                    {
                        enumerator.MoveNext();
                        row[c] = enumerator.Current;
                    }
                    this.elements[r] = row;
                }
            }
        }

        private Matrix(int rows, int cols, double[][] elements)
        {
            this.Rows = rows;
            this.Columns = cols;
            this.elements = elements;
        }
        #endregion

        #region Properties
        public int Rows { get; }

        public int Columns { get; }

        public IReadOnlyCollection<double> this[int row] => this.elements[row];

        public double this[int row, int column] => this.elements[row][column];
        #endregion

        #region Public methods
        public static Matrix operator +(Matrix left, Matrix right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            if (left.Rows != right.Rows || left.Columns != right.Columns)
                throw new ArgumentException($"Invalid matrix addition: {left.Rows}x{left.Columns} + {right.Rows}x{right.Columns}");

            var rows = left.Rows;
            var cols = right.Columns;
            var leftElements = left.elements;
            var rightElements = right.elements;

            var result = new double[rows][];

            for (int row = 0; row < rows; row++)
            {
                var leftRow = leftElements[row];
                var rightRow = rightElements[row];
                var resultRow = new double[cols];
                for (int col = 0; col < cols; col++)
                {
                    resultRow[col] = leftRow[col] + rightRow[col];
                }
                result[row] = resultRow;
            }

            return new Matrix(rows, cols, result);
        }

        public static Matrix operator +(Matrix left, double right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            var rows = left.Rows;
            var cols = left.Columns;
            var leftElements = left.elements;

            var result = new double[rows][];

            for (int row = 0; row < rows; row++)
            {
                var leftRow = leftElements[row];
                var resultRow = new double[cols];
                for (int col = 0; col < cols; col++)
                {
                    resultRow[col] = leftRow[col] + right;
                }
                result[row] = resultRow;
            }

            return new Matrix(rows, cols, result);
        }

        public static Matrix operator +(double left, Matrix right) => right + left;

        public static Matrix operator -(Matrix left, Matrix right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            if (left.Rows != right.Rows || left.Columns != right.Columns)
                throw new ArgumentException($"Invalid matrix subtraction: {left.Rows}x{left.Columns} - {right.Rows}x{right.Columns}");

            var rows = left.Rows;
            var cols = right.Columns;
            var leftElements = left.elements;
            var rightElements = right.elements;

            var result = new double[rows][];

            for (int row = 0; row < rows; row++)
            {
                var leftRow = leftElements[row];
                var rightRow = rightElements[row];
                var resultRow = new double[cols];
                for (int col = 0; col < cols; col++)
                {
                    resultRow[col] = leftRow[col] - rightRow[col];
                }
                result[row] = resultRow;
            }

            return new Matrix(rows, cols, result);
        }

        public static Matrix operator -(Matrix left, double right) => left + (right * -1);

        public static Matrix operator -(double left, Matrix right) => right - left;

        public static Matrix operator *(Matrix left, Matrix right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            if (left.Columns != right.Rows)
                throw new ArgumentException($"Invalid matrix multiplication: {left.Rows}x{left.Columns} * {right.Rows}x{right.Columns}");

            var rows = left.Rows;
            var cols = right.Columns;
            var common = left.Columns;
            var leftElements = left.elements;
            var rightElements = right.Transpose().elements;

            var result = new double[rows][];

            for (int row = 0; row < rows; row++)
            {
                var leftRow = leftElements[row];
                var resultRow = new double[cols];
                for (int col = 0; col < cols; col++)
                {
                    var rightRow = rightElements[col];
                    var resultRowCol = 0.0;
                    for (int x = 0; x < common; x++)
                    {
                        resultRowCol += leftRow[x] * rightRow[x];
                    }
                    resultRow[col] = resultRowCol;
                }
                result[row] = resultRow;
            }

            return new Matrix(rows, cols, result);
        }

        public static Matrix operator *(Matrix left, double right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            var rows = left.Rows;
            var cols = left.Columns;
            var leftElements = left.elements;

            var result = new double[rows][];

            for (int row = 0; row < rows; row++)
            {
                var leftRow = leftElements[row];
                var resultRow = new double[cols];
                for (int col = 0; col < cols; col++)
                {
                    resultRow[col] = leftRow[col] * right;
                }
                result[row] = resultRow;
            }

            return new Matrix(rows, cols, result);
        }

        public static Matrix operator *(double left, Matrix right) => right * left;

        public static Matrix operator /(Matrix left, double right) => left * (1 / right);

        public static Matrix operator /(double left, Matrix right)
        {
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            var rows = right.Rows;
            var cols = right.Columns;
            var rightElements = right.elements;

            var result = new double[rows][];

            for (int row = 0; row < rows; row++)
            {
                var rightRow = rightElements[row];
                var resultRow = new double[cols];
                for (int col = 0; col < cols; col++)
                {
                    resultRow[col] = left / rightRow[col];
                }
                result[row] = resultRow;
            }

            return new Matrix(rows, cols, result);
        }

        public Matrix Transpose()
        {
            var elements = new double[this.Columns][];
            for (int c = 0; c < this.Columns; c++)
            {
                var row = new double[this.Rows];
                for (int r = 0; r < this.Rows; r++)
                {
                    row[r] = this.elements[r][c];
                }
                elements[c] = row;
            }
            return new Matrix(this.Columns, this.Rows, elements);
        }
        #endregion

        #region Private fields and constants
        private readonly double[][] elements;
        #endregion
    }
}
