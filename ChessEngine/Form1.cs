using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace ChessEngine
{
	public partial class Form1 : Form
	{
		//Const
		const int pawn = 0;
		const int knight = 1;
		const int bishop = 2;
		const int rook = 3;
		const int queen = 4;
		const int king = 5;

		//Image Data
		Image[] wPieces;
		Image[] bPieces;
		State s = new State();
		Random rnd = new Random();
		Tree tree = new Tree();
		//constructor
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			wPieces = new Image[6];
			bPieces = new Image[6];

			wPieces[pawn] = Image.FromFile(Directory.GetCurrentDirectory() + "\\Pictures\\Pawn_White.png");
			bPieces[pawn] = Image.FromFile(Directory.GetCurrentDirectory() + "\\Pictures\\Pawn_Black.png");

			wPieces[rook] = Image.FromFile(Directory.GetCurrentDirectory() + "\\Pictures\\Rock_White.png");
			bPieces[rook] = Image.FromFile(Directory.GetCurrentDirectory() + "\\Pictures\\Rock_Black.png");

			wPieces[knight] = Image.FromFile(Directory.GetCurrentDirectory() + "\\Pictures\\Knight_White.png");
			bPieces[knight] = Image.FromFile(Directory.GetCurrentDirectory() + "\\Pictures\\Knight_Black.png");

			wPieces[bishop] = Image.FromFile(Directory.GetCurrentDirectory() + "\\Pictures\\Bishop_White.png");
			bPieces[bishop] = Image.FromFile(Directory.GetCurrentDirectory() + "\\Pictures\\Bishop_Black.png");

			wPieces[queen] = Image.FromFile(Directory.GetCurrentDirectory() + "\\Pictures\\Queen_White.png");
			bPieces[queen] = Image.FromFile(Directory.GetCurrentDirectory() + "\\Pictures\\Queen_Black.png");

			wPieces[king] = Image.FromFile(Directory.GetCurrentDirectory() + "\\Pictures\\King_White.png");
			bPieces[king] = Image.FromFile(Directory.GetCurrentDirectory() + "\\Pictures\\King_Black.png");
		}
		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			float sqx = 80;
			float sqy = 80;

			byte[,] field = s.GetField();
			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					e.Graphics.FillRectangle((x + y) % 2 == 1 ? Brushes.White : Brushes.DarkBlue,
							x * sqx, (7 - y) * sqy, sqx, sqy);

					if ((field[x, y] & 16) == 0)
					{
						e.Graphics.DrawImage(GetImage((State.Category)(field[x, y] & (1 | 2 | 4)), (field[x, y] & 8) == 0),
							x * sqx, (7 - y) * sqy, sqx, sqy);
					}

				}
			}
		}
		private Image GetImage(State.Category _cat, bool _color)
		{
			if (_color) return wPieces[(int)_cat];
			else return bPieces[(int)_cat];
		}

		private void button1_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < 30; i++)
			{
				s.DoMove(tree.GetBestMove());
				Refresh();
				tree.DoMove(s.LastMove);
			}
		}
	}
}
