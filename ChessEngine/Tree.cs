using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
	class Tree
	{
		private State state;
		private Root[] root;
		
		public Tree()
		{
			state = new State();
			List<State> list = state.CheckMoves();
			root = new Root[state.SoftMoves.Count];

			Console.WriteLine("Start creating Tree");
			
			for(int i = 0; i < root.Length; i++)
			{
				root[i] = new Root(1, list[i]);
				Console.WriteLine(100 / root.Length * (i + 1) + "% done");
			}
		}

		public Move GetBestMove()
		{
			int index = 0;
			float value = root[0].Evaluate();
			float buffer;

			for(int i = 1; i < root.Length; i++)
			{
				buffer = root[i].Evaluate();
				if (state.Player)
				{
					if(buffer < value)
					{
						value = buffer;
						index = i;
					}
				}
				else
				{
					if(buffer > value)
					{
						value = buffer;
						index = i;
					}
				}
			}

			Console.WriteLine("The best for " + (state.Player ? "Black" : "White") + " is " + root[index].LastMove + " with " + value.ToString("0.00"));
			return root[index].LastMove;
		}
		public void DoMove(Move _m)
		{
			state.DoMove(_m);
			Console.WriteLine("Start recreating Tree");
			List<State> list = state.CheckMoves();
			root = new Root[state.SoftMoves.Count];
			for (int i = 0; i < root.Length; i++)
			{
				root[i] = new Root(1, list[i]);
				Console.WriteLine(100f / root.Length * (i + 1) + "% done");
			}
		}
	}
}
