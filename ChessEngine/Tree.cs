using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace ChessEngine
{
	class Tree
	{
		private State state;
		private Root[] root;
		
		public Tree()
		{
			done = 0;
			state = new State();
			List<State> list = state.CheckMoves();
			root = new Root[state.SoftMoves.Count];


			for (int i = 0; i < root.Length; i++)
			{
				new Thread(new ParameterizedThreadStart(CreateTree)).Start(new RootData(list[i], i));
			}

			while(done < root.Length) { Thread.Sleep(100); }
		}

		float[] array;
		int done = 0;
		public Move GetBestMove()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			array = new float[root.Length];
			done = 0;


			for(int i = 0; i < root.Length; i++)
			{
				new Thread(new ParameterizedThreadStart(Eva)).Start(new EvaData(root[i], i));
			}
			while(done < root.Length) { Thread.Sleep(100); }

			int index = 0;

			float buffer, value;
			value = array[0];
			for (int i = 1; i < root.Length; i++)
			{
				buffer = array[i];
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
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			return root[index].LastMove;
		}
		public void DoMove(Move _m)
		{
			state.DoMove(_m);
			done = 0;
			List<State> list = state.CheckMoves();
			root = new Root[state.SoftMoves.Count];


			for (int i = 0; i < root.Length; i++)
			{
				new Thread(new ParameterizedThreadStart(CreateTree)).Start(new RootData(list[i], i));
			}

			while (done < root.Length) { Thread.Sleep(100); }
		}

		public void Eva(object _root)
		{
			array[((EvaData)_root).index] = ((EvaData)_root).r.Evaluate();
			done++;
		}
		public void CreateTree(object _state)
		{
			root[((RootData)_state).i] = new Root(1, ((RootData)_state).s);
			done++;
		}
	}

	struct EvaData
	{
		public Root r;
		public int index;

		public EvaData(Root _r, int _index)
		{
			r = _r;
			index = _index;
		}
	}
	struct RootData
	{
		public State s;
		public int i;
		
		public RootData(State _s, int _i)
		{
			s = _s;
			i = _i;
		}
	}
}
