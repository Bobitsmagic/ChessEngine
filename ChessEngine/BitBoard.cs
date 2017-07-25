using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
	struct BitBoard
	{
		//Gets
		public ulong Value { get { return val; } }
		public int BitCount { get
			{
				ulong buffer = Value;
				buffer = buffer - ((buffer >> 1) & 0x5555555555555555);
				buffer = (buffer & 0x3333333333333333) + ((buffer >> 2) & 0x3333333333333333);
				return (int)((((buffer + (buffer >> 4)) & 0xF0F0F0F0F0F0F0F) * 0x101010101010101) >> 56);
			} }
		public bool Empty { get { return val == 0; } }

		//const
		private static ulong[] Column = new ulong[8]
		{
			72340172838076673,
			217020518514230019,
			506381209866536711,
			1085102592571150095,
			2242545357980376863,
			4557430888798830399,
			9187201950435737471,
			ulong.MaxValue
		};

		//data
		private ulong val;

		//constructor
		public BitBoard(ulong _val)
		{
			val = _val;
		}
		public BitBoard(byte[] _val)
		{
			val = BitConverter.ToUInt64(_val, 0);
		}
		public BitBoard(bool[] _bitArray)
		{
			val = 0;
			for(int i = 0; i < 64; i++)
			{
				if(_bitArray[i])	val |= (ulong)1 << i;
			}
		}
		public BitBoard(Position _pos)
		{
			val = (ulong)1 << _pos.Index;
		}

		//public voids
		public void Move(int _x, int _y)
		{
			int b = _x + (_y << 3);
			if (b >= 0)
				val = (val << b) & (_x > 0 ? ~Column[_x - 1] : Column[7 + _x]);
			else
				val = (val >> -b) & (_x > 0 ? ~Column[_x - 1] : Column[7 + _x]);
		}
		public void RemoveAt(ulong _val)
		{
			val &= ~_val;
		}
		public void RemoveAt(BitBoard _val)
		{
			this &= ~_val;
		}
		public void Set(int _x, int _y, bool _val)
		{
			if (_val) val |= (ulong)1 << (_x + (_y << 3));
			else val &= ~((ulong)1 << (_x + (_y << 3)));
		}
		public void Set(Position _p, bool _val)
		{
			if (_val) val |= (ulong)1 << (_p.X + (_p.Y << 3));
			else val &= ~((ulong)1 << (_p.X + (_p.Y << 3)));
		}

		//public Functions
		public BitBoard Offset(int _x, int _y)
		{
			int b = _x + (_y << 3);
			if (b >= 0)
				return new BitBoard((val << b) & (_x > 0 ? ~Column[_x - 1] : Column[7 + _x]));
			else
				return new BitBoard((val >> -b) & (_x > 0 ? ~Column[_x - 1] : Column[7 + _x]));
		}
		/// <summary>
		/// Return the total Number of set Bits
		/// </summary>
		public bool Get(int _x, int _y)
		{
			return (Value & ((ulong)1 << (_x + (_y << 3)))) != 0;
		}
		public bool Get(Position _p)
		{
			return (Value & ((ulong)1 << (_p.X + (_p.Y << 3)))) != 0;
		}

		//operator
		public static BitBoard operator &(BitBoard a, BitBoard b)
		{
			return new BitBoard(a.Value & b.Value);
		}
		public static BitBoard operator &(BitBoard a, ulong b)
		{
			return new BitBoard(a.Value & b);
		}
		public static BitBoard operator |(BitBoard a, BitBoard b)
		{
			return new BitBoard(a.Value | b.Value);
		}
		public static BitBoard operator |(BitBoard a, ulong b)
		{
			return new BitBoard(a.Value | b);
		}
		public static BitBoard operator ^(BitBoard a, BitBoard b)
		{
			return new BitBoard(a.Value ^ b.Value);
		}
		public static BitBoard operator ^(BitBoard a, ulong b)
		{
			return new BitBoard(a.Value ^ b);
		}

		public static BitBoard operator ~(BitBoard a)
		{
			return new BitBoard(~a.Value);
		}
		public static bool operator ==(BitBoard a, BitBoard b)
		{
			return a.Value == b.Value;
		}
		public static bool operator !=(BitBoard a, BitBoard b)
		{
			return a.Value != b.Value;
		}

		//override
		public override string ToString()
		{
			ulong buffer;
			string s = "";
			string[] a = new string[8];
			for (int i = 0; i < 64; i++)
			{
				buffer = Value >> i;
				a[i / 8] += (buffer & 1) > 0 ? "1" : "0";
			}
			for (int i = 7; i >= 0; i--)
			{
				s += a[i] + "\n";
			}
			return s;
		}
		public override int GetHashCode()
		{
			int ret = 0;
			for(int i = 0; i < 32; i++)
			{
				ret |= (int)(((Value << (i * 2)) ^ (Value << (i * 2 + 1))) >> i);
			}

			return ret;
		}
		public override bool Equals(object obj)
		{
			return this == (BitBoard)obj;
		}
	}
}
