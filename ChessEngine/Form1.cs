using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessEngine
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			BitBoard bb = new BitBoard(new byte[8]
			{
				255,
				129,
				129,
				129,
				129,
				129,
				129,
				255
			});

			Console.WriteLine(bb);
			bb.Move(1, -2);
			Console.WriteLine(bb);
		}
	}
}
