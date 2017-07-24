using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
	class State
	{
		//Enums
		public enum Category
		{
			Pawn, Knight, Bishop, Rook, Queen, King
		}

		//Gets
		public Move LastMove { get { return lastMove; } }
		public BitBoard AllPieces { get { return white | black; } }
		public bool Player { get { return player; } }
		

		//private variables
		Move lastMove;
		bool player;

		private BitBoard white;
		private BitBoard black;
		private BitBoard pawn;
		private BitBoard knight;
		private BitBoard bishop;
		private BitBoard rook;
		private BitBoard queen;
		private BitBoard king;

		//constructor
		public State()
		{
			lastMove = new Move(Category.Pawn, new Position(), new Position(), false);

			white = new BitBoard(new byte[8]
			{
				255, 255, 0, 0,
				0, 0, 0, 0
			});
			black = new BitBoard(new byte[8]
			{
				0, 0, 0, 0,
				0, 0, 255, 255
			});
			pawn = new BitBoard(new byte[8]
			{
				0, 255, 0, 0,
				0, 0, 255, 0
			});
			rook = new BitBoard(new byte[8]
			{
				129, 0, 0, 0,
				0, 0, 0, 129
			});
			knight = new BitBoard(new byte[8]
			{
				66, 0, 0, 0,
				0, 0, 0, 66
			});
			bishop = new BitBoard(new byte[8]
			{
				36, 0, 0, 0,
				0, 0, 0, 36
			});
			queen = new BitBoard(new byte[8]
			{
				8, 0, 0, 0,
				0, 0, 0, 8
			});
			king = new BitBoard(new byte[8]
			{
				16, 0, 0, 0,
				0, 0, 0, 16
			});
		}

		//public voids
		public void DoMove(Move _m)
		{
			lastMove = _m;
			if (Player)
			{   //black makes a move
				//remove a black colorPoint at start
				Console.WriteLine(black);
				black.Set(_m.Start, false);
				//removes a white colorPoint at End
				white.Set(_m.End, false);
				//writes a black Piece at End
				black.Set(_m.End, true);
			}
			else
			{	//white
				white.Set(_m.Start, false);
				black.Set(_m.End, false);
				white.Set(_m.End, true);
			}

			//remove killed piece
			if (_m.Kill)
			{
				if (pawn.Get(_m.End))	pawn.Set(_m.End, false);
				if (knight.Get(_m.End)) knight.Set(_m.End, false);
				if (bishop.Get(_m.End)) bishop.Set(_m.End, false);
				if (rook.Get(_m.End))	rook.Set(_m.End, false);
				if (queen.Get(_m.End))	queen.Set(_m.End, false);
				if (king.Get(_m.End))	king.Set(_m.End, false);
			}

			//remove and add moved piece
			switch (_m.Category)
			{
				case Category.Pawn:		pawn.Set(_m.Start, false);		pawn.Set(_m.End, true); break;
				case Category.Knight:	knight.Set(_m.Start, false);	knight.Set(_m.End, true); break;
				case Category.Bishop:	bishop.Set(_m.Start, false);	bishop.Set(_m.End, true); break;
				case Category.Rook:		rook.Set(_m.Start, false);		rook.Set(_m.End, true); break;
				case Category.Queen:	queen.Set(_m.Start, false);		queen.Set(_m.End, true); break;
				default:				king.Set(_m.Start, false);		king.Set(_m.End, true); break;
			}
			player = !player;
			
			//softMoves = SoftTest();
		}

		//public functions
		public string GetAllBitboards()
		{
			string ret = "";
			ret += "White:\n"		+ white.ToString();
			ret += "\nBlack:\n"		+ black.ToString();
			ret += "\nPawn:\n"		+ pawn.ToString();
			ret += "\nKnight:\n"	+ knight.ToString();
			ret += "\nBishop:\n"	+ bishop.ToString();
			ret += "\nRook:\n"		+ rook.ToString();
			ret += "\nQueen:\n"		+ queen.ToString();
			ret += "\nKing:\n"		+ king.ToString();

			return ret;
		}
		public byte[,] GetField()
		{
			byte[,] ret = new byte[8, 8];

			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					ret[x, y] = (byte)(AllPieces.Get(x, y)	? 0 : 16);
					ret[x, y] |= (byte)(white.Get(x, y)		? 0 : 8);
					ret[x, y] |= (byte)(pawn.Get(x, y)		? 0 : 0);
					ret[x, y] |= (byte)(knight.Get(x, y)	? 1 : 0);
					ret[x, y] |= (byte)(bishop.Get(x, y)	? 2 : 0);
					ret[x, y] |= (byte)(rook.Get(x, y)		? 3 : 0);
					ret[x, y] |= (byte)(queen.Get(x, y)		? 4 : 0);
					ret[x, y] |= (byte)(king.Get(x, y)		? 5 : 0);
				}
			}

			return ret;
		}
		//overrides
		public override string ToString()
		{
			return base.ToString();
		}
		public override int GetHashCode()
		{
			return AllPieces.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if (white	!=	((State)obj).white) return false;
			if (black	!=	((State)obj).black) return false;
			if (pawn	!=	((State)obj).pawn) return false;
			if (knight	!=	((State)obj).knight) return false;
			if (bishop	!=	((State)obj).bishop) return false;
			if (rook	!=	((State)obj).rook) return false;
			if (queen	!=	((State)obj).queen) return false;
			if (king	!=	((State)obj).king) return false;

			return true;
		}
	}
}
