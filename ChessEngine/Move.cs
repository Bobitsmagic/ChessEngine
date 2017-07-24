using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
	class Move
	{
		public Position Start { get { return start; } }
		public Position End { get { return end; } }
		public bool Kill { get { return kill; } }
		public State.Category Category { get { return (State.Category)category; } }
		public bool Player { get { return player; } }

		private Position start;
		private Position end;
		private bool kill;
		private bool check;
		private bool player;

		byte category;

		public Move(State.Category _cat, Position _startPos, Position _endPos, bool _kill)
		{
			start = _startPos;
			end = _endPos;
			kill = _kill;
			check = false;
			category = (byte)_cat;
		}
		public static List<Move> GetMoves(State.Category _cat, int dx, int dy, ulong _endPos, bool _kill)
		{

			List<Position> buffer = GetPos(_endPos);
			List<Move> ret = new List<Move>(buffer.Count);

			for (int i = 0; i < buffer.Count; i++)
			{
				ret.Add(new Move(_cat, buffer[i].Offset(-dx, -dy), buffer[i], _kill));
			}

			return ret;
		}
		public override string ToString()
		{
			return ((State.Category)category).ToString() + " " + start.ToString() + (kill ? "x" : "-") + end.ToString();
		}

		public static List<Position> GetPos(ulong _val)
		{
			List<Position> ret = new List<Position>();

			for (int i = 0; i < 64; i++)
			{
				if (((_val >> i) & 1) > 0) ret.Add(new Position((byte)i));
			}

			return ret;
		}
	}

	public struct Position
	{                                       // = % 3
		public int X { get { return position % 8; } }
		public int Y { get { return position >> 3; } }
		public int Index { get { return position; } }

		private byte position;
		public Position(int _x, int _y)
		{
			position = (byte)(_x | (_y << 3));
		}
		public Position(byte _val)
		{
			position = _val;
		}

		public void Move(int _dx, int _dy)
		{
			position += (byte)(_dx + (_dy << 3));
		}
		public Position Offset(int _dx, int _dy)
		{
			return new Position(X + _dx, Y + _dy);
		}
		public bool TryMove(int _dx, int _dy)
		{
			return !(X + _dx > 7 || X + _dx < 0 || Y + _dy > 7 || Y + _dy < 0);
		}

		public override string ToString()
		{
			switch (X)
			{
				case 0: return "a" + (Y + 1).ToString();
				case 1: return "b" + (Y + 1).ToString();
				case 2: return "c" + (Y + 1).ToString();
				case 3: return "d" + (Y + 1).ToString();
				case 4: return "e" + (Y + 1).ToString();
				case 5: return "f" + (Y + 1).ToString();
				case 6: return "g" + (Y + 1).ToString();
				default: return "h" + (Y + 1).ToString();
			}
		}

		public static bool operator ==(Position a, Position b)
		{
			return a.position == b.position;
		}
		public static bool operator !=(Position a, Position b)
		{
			return a.position != b.position;
		}

		public override bool Equals(object obj)
		{
			return this == (Position)obj;
		}
		public override int GetHashCode()
		{
			return position;
		}
	}
}
