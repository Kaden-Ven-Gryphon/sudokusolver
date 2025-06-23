using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SudokuSolver
{
	internal class BoardWpf
	{
		#region Properties
		public Grid BoardGrid { get; private set; }

		public int ValueFontSize { get; set; }
		public int PencilFontSize { get; set; }
		#endregion

		#region Private Atributes
		private int _rows;
		private int _columns;
		private List<Border> borderList;
		private List<TextBlock> cellTextList;
		private List<Grid> pencilGrids;
		private List<List<TextBlock>> pencilTextList;
		#endregion

		#region Constructors
		public BoardWpf()
		{
			BoardGrid = new Grid();
			borderList = new List<Border>();
			cellTextList = new List<TextBlock>();
			pencilGrids = new List<Grid>();
			pencilTextList = new List<List<TextBlock>>();
			ValueFontSize = 18;
			PencilFontSize = 7;

			initBoard(9, 9, 3);
		}

		public BoardWpf(int width, int hight, int bold)
		{
			BoardGrid = new Grid();
			borderList = new List<Border>();
			cellTextList = new List<TextBlock>();
			pencilGrids = new List<Grid>();
			pencilTextList = new List<List<TextBlock>>();
			ValueFontSize = 18;
			PencilFontSize = 7;

			initBoard(width, hight, bold);
		}

		// set the classes Board to a new grid filled
		// with the borders and textblocks
		private void initBoard(int width, int hight, int bold)
		{
			BoardGrid = new Grid();
			BoardGrid.Width = 200;
			BoardGrid.Height = 200;
			BoardGrid.Children.Clear();
			borderList = new List<Border>();
			cellTextList = new List<TextBlock>();

			for (int i = 0; i < width; i++)
			{
				BoardGrid.ColumnDefinitions.Add(new ColumnDefinition());
			}
			for (int i = 0; i < hight; i++)
			{
				BoardGrid.RowDefinitions.Add(new RowDefinition());
			}

			for (int i = 0; i < hight; i++)
			{
				for (int j = 0; j < width; j++)
				{
					// Make and add border for cell
					var cell = new Border();
					cell.BorderThickness = new Thickness(1);
					cell.BorderBrush = Brushes.Black;
					BoardGrid.Children.Add(cell);
					Grid.SetRow(cell, i);
					Grid.SetColumn(cell, j);
					borderList.Add(cell);


					// Make and add text block
					//var viewbox = new Viewbox();
					var celltext = new TextBlock();
					//viewbox.Child = celltext;
					//cell.Child = viewbox;
					cell.Child = celltext;
					cellTextList.Add(celltext);
					celltext.Text = i.ToString();

					// Make pencil grids
					var pencilGrid = new Grid();
					pencilGrid.ColumnDefinitions.Add(new ColumnDefinition());
					pencilGrid.ColumnDefinitions.Add(new ColumnDefinition());
					pencilGrid.ColumnDefinitions.Add(new ColumnDefinition());
					pencilGrid.RowDefinitions.Add(new RowDefinition());
					pencilGrid.RowDefinitions.Add(new RowDefinition());
					pencilGrid.RowDefinitions.Add(new RowDefinition());
					pencilGrid.Margin = new Thickness(1);
					pencilTextList.Add(new List<TextBlock>());

					for (int k = 0; k < 9; k++)
					{
						var pencilText = new TextBlock();
						pencilGrid.Children.Add(pencilText);
						Grid.SetRow(pencilText, k/3);
						Grid.SetColumn(pencilText, k%3);
						pencilTextList[pencilTextList.Count-1].Add(pencilText);
					}

					pencilGrids.Add(pencilGrid);
				}
			}

			for (int i = 0; i < hight / bold; i++)
			{
				for (int j = 0; j < width / bold; j++)
				{
					var cell = new Border();
					cell.BorderThickness = new Thickness(2);
					cell.BorderBrush = Brushes.Black;
					BoardGrid.Children.Add(cell);
					Grid.SetRow(cell, i * bold);
					Grid.SetColumn(cell, j * bold);
					Grid.SetRowSpan(cell, bold);
					Grid.SetColumnSpan(cell, bold);
				}
			}
		}
		#endregion


		#region Private Methods
		// Get 1D array index from row and colm
		private int getindex(int x, int y)
		{
			return x + (y * 9);
		}
		#endregion

		public void FillCellValue(int row, int col, int value)
		{
			int cellIndex = getindex(col, row);

			borderList[cellIndex].Child = cellTextList[cellIndex];

			cellTextList[cellIndex].TextAlignment = TextAlignment.Center;
			cellTextList[cellIndex].Text = value != 0 ? value.ToString() : "";
			cellTextList[cellIndex].VerticalAlignment = VerticalAlignment.Center;
			cellTextList[cellIndex].FontSize = ValueFontSize;
		}

		public void SetCellPencil(int row, int col, List<int> values)
		{
			if (values.Count == 0) { return; }

			int cellIndex = getindex(col, row);

			borderList[cellIndex].Child = pencilGrids[cellIndex];

			for (int i = 0; i < 9; i++)
			{
				pencilTextList[cellIndex][i].TextAlignment = TextAlignment.Center;
				pencilTextList[cellIndex][i].Text = values.Contains(i+1)? (i+1).ToString(): "";
				pencilTextList[cellIndex][i].VerticalAlignment = VerticalAlignment.Center;
				pencilTextList[cellIndex][i].FontSize = PencilFontSize;
			}
		}

		public void SetCellForegroundColor(int row, int col, Color color)
		{
			var index = getindex(col, row);
			var brush = new SolidColorBrush(color);
			cellTextList[index].Foreground =brush;

			for (int i = 0; i < 9; i++)
			{
				pencilTextList[index][i].Foreground = brush;
			}
		}

		public void SetCellBackgroundCollor(int row, int col, Color color)
		{
			var index = getindex(col, row);
			var brush = new SolidColorBrush(color);
			cellTextList[index].Background = brush;

			for (int i = 0; i < 9; i++)
			{
				pencilTextList[index][i].Background = brush;
			}
		}
	}
}
