using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
	class Root
	{
		public const int MaxDepth = 3;
		public Move LastMove { get { return state.LastMove; } }

		private Root[] root;
		private int depth;
		private State state;

		public Root(int _depth, State _s)
		{
			depth = _depth;
			state = _s;

			if(depth < MaxDepth)
			{
				List<State> list = state.CheckMoves();
				root = new Root[state.SoftMoves.Count];

				for(int i = 0; i < root.Length; i++)
				{
					root[i] = new Root(depth + 1, list[i]);
				}
			}
		}

		public float Evaluate()
		{
			if(depth == MaxDepth)
			{
				return state.Evaluate();
			}
			else
			{
				float extrema = 0;
				if (root.Length == 0) { Console.WriteLine("Found Mate in"); }
				else
				{
					extrema = root[0].Evaluate();
					for (int i = 1; i < root.Length; i++)
					{
						if (state.Player) extrema = Math.Min(extrema, root[i].Evaluate());
						else extrema = Math.Max(extrema, root[i].Evaluate());
					}

				}


				return extrema;
			}
		}
	}
}
