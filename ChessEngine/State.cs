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
			Pawn, Knight, Bishop, Rook, Queen, King, Empty
		}

		//Gets
		public Move LastMove { get { return lastMove; } }
		public BitBoard AllPieces { get { return white | black; } }
		public bool Player { get { return player; } }
		public List<Move> SoftMoves { get { return softMoves; } }
		public Position WhiteKingPos { get { return new Position((byte)Math.Log(white.Value & king.Value, 2)); } }
		public Position BlackKingPos { get { return new Position((byte)Math.Log(black.Value & king.Value, 2)); } }

		//const
		public static BitBoard WhitePawns = new BitBoard(new byte[8]
		{
			0, 0, 0, 255, 0, 0, 0, 0
		});
		public static BitBoard BlackPawns = new BitBoard(new byte[8]
		{	
			0, 0, 0, 0, 255, 0, 0, 0
		});

		//private variables
		private Move lastMove;
		private bool player;
		private bool movesProved;

		private bool BlackKingMoved;
		private bool WhiteKingMoved;
		private bool WhiteARookMoved;
		private bool WhiteHRookMoved;
		private bool BlackARookMoved;
		private bool BlackHRookMoved;

		private List<Move> softMoves;

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

			#region Standart 
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
			#endregion

			#region custom
//			white = new BitBoard(new byte[8]
//{
//				0, 1, 0, 0, 0, 0, 0, 0
//});
//			black = new BitBoard(new byte[8]{
//				0, 0, 0, 2, 0, 0, 0, 0
//			});
//			pawn = new BitBoard(new byte[8]
//			{
//				0, 1, 0, 2, 0, 0, 0, 0
//			});
//			knight = new BitBoard();
//			bishop = new BitBoard();
//			king = new BitBoard();
//			queen = new BitBoard();
//			rook = new BitBoard();
			#endregion


			CalcSoftMoves();
		}
		public State(State _s, Move _m)
		{
			lastMove = new Move(Category.Pawn, new Position(), new Position(), false);
			white = _s.white;
			black = _s.black;
			pawn = _s.pawn;
			knight = _s.knight;
			bishop = _s.bishop;
			rook = _s.rook;
			queen = _s.queen;
			king = _s.king;

			WhiteARookMoved = _s.WhiteARookMoved;
			WhiteHRookMoved = _s.WhiteHRookMoved;

			BlackARookMoved = _s.BlackARookMoved;
			BlackHRookMoved = _s.BlackHRookMoved;

			WhiteKingMoved = _s.WhiteKingMoved;
			BlackKingMoved = _s.BlackKingMoved;

			player = _s.player;
			DoMove(_m);
		}

		//public voids
		public void DoMove(Move _m)
		{
			//Console.WriteLine("Doing " + _m);
			
			if (Player)
			{   //black makes a move
				//remove a black colorPoint at start
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
				pawn.Set(_m.End, false);
				knight.Set(_m.End, false);
				bishop.Set(_m.End, false);
				rook.Set(_m.End, false);
				queen.Set(_m.End, false);
				king.Set(_m.End, false);

				//pawn								and moved 2										and	kill on rank 3 or 5		
				if(_m.Category == Category.Pawn && Math.Abs(lastMove.Start.Y - lastMove.End.Y) == 2 && (_m.End.Y == 2 || _m.End.Y == 5))
				{
					pawn.Set(new Position(_m.End.X, _m.End.Y + (Player ? 1 : -1)), false);
					if (player)	white.Set(new Position(_m.End.X, _m.End.Y + (Player ? 1 : -1)), false);
					else black.Set(new Position(_m.End.X, _m.End.Y + (Player ? 1 : -1)), false);
				}
			}

			//adding moved pieces
			if (_m.Start == new Position("a1") || _m.End == new Position("a1")) WhiteARookMoved = true;
			if (_m.Start == new Position("h1") || _m.End == new Position("h1")) WhiteHRookMoved = true;

			if (_m.Start == new Position("a8") || _m.End == new Position("a8")) BlackARookMoved = true;
			if (_m.Start == new Position("h8") || _m.End == new Position("h8")) BlackHRookMoved = true;

			//remove and add moved piece
			switch (_m.Category)
			{
				case Category.Pawn:
					pawn.Set(_m.Start, false);
					if(_m.FinalCategory != Category.Empty)
					{
						switch (_m.FinalCategory)
						{
							case Category.Knight:	knight.Set(_m.End, true); break;
							case Category.Bishop:	bishop.Set(_m.End, true); break;
							case Category.Rook:		rook.Set(_m.End, true); break;
							case Category.Queen:	queen.Set(_m.End, true); break;
						}
					}
					else pawn.Set(_m.End, true); 
					break;

				case Category.Knight:	knight.Set(_m.Start, false);	knight.Set(_m.End, true); break;
				case Category.Bishop:	bishop.Set(_m.Start, false);	bishop.Set(_m.End, true); break;
				case Category.Rook:		rook.Set(_m.Start, false);		rook.Set(_m.End, true); break;
				case Category.Queen:	queen.Set(_m.Start, false);		queen.Set(_m.End, true); break;
				default:				king.Set(_m.Start, false);		king.Set(_m.End, true);
					if (Player) BlackKingMoved = true;
					else WhiteKingMoved = true;
					break;
			}

			//castle
			if (_m.IsCastle)
			{
				if (player)
				{
					if(_m.End == new Position("c8"))
					{
						black.Set(new Position("a8"), false);
						black.Set(new Position("d8"), true);

						rook.Set(new Position("a8"), false);
						rook.Set(new Position("d8"), true);

						BlackARookMoved = true;
					}
					if (_m.End == new Position("g8"))
					{
						black.Set(new Position("h8"), false);
						black.Set(new Position("f8"), true);

						rook.Set(new Position("h8"), false);
						rook.Set(new Position("f8"), true);
						BlackHRookMoved = true;
					}
				}
				else
				{
					if (_m.End == new Position("c1"))
					{
						white.Set(new Position("a1"), false);
						white.Set(new Position("d1"), true);

						rook.Set(new Position("a1"), false);
						rook.Set(new Position("d1"), true);

						WhiteARookMoved = true;
					}
					if (_m.End == new Position("g1"))
					{
						white.Set(new Position("h1"), false);
						white.Set(new Position("f1"), true);

						rook.Set(new Position("h1"), false);
						rook.Set(new Position("f1"), true);

						WhiteHRookMoved = true;
					}
				}
			}

			lastMove = _m;
			player = !player;

			CalcSoftMoves();
		}
		public List<State> CheckMoves()
		{
			List<State> ret = new List<State>(softMoves.Count);

			for(int i = softMoves.Count - 1; i >= 0; i--)
			{
				ret.Add(new State(this, softMoves[i]));
				
				if (ret.Last().OpponentIsInCheck())
				{
					ret.RemoveAt(ret.Count -1);
					softMoves.RemoveAt(i);
				}
			}
			movesProved = true;
			ret.Reverse();
			return ret;
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
		public Category GetCategory(int x, int y)
		{
			if (AllPieces.Get(x, y))
			{
				if (pawn.Get(x, y)) return Category.Pawn;
				if (knight.Get(x, y)) return Category.Knight;
				if (bishop.Get(x, y)) return Category.Bishop;
				if (rook.Get(x, y)) return Category.Rook;
				if (queen.Get(x, y)) return Category.Queen;
				if (king.Get(x, y)) return Category.King;
			}

			return (Category)7;
			
		}
		public bool OpponentIsInCheck()
		{
			if (player) //BLACK
			{
				for (int i = 0; i < softMoves.Count; i++)
				{
					if(WhiteKingPos == softMoves[i].End) return true;
				}
			}
			else
			{
				for (int i = 0; i < softMoves.Count; i++)
				{
					if (BlackKingPos == softMoves[i].End) return true;
				}
			}

			//castle?
			if (lastMove.IsCastle)
			{
				for (int i = 0; i < softMoves.Count; i++)
				{
					//startpos?
					if (lastMove.Start == softMoves[i].End) return true;
					//moving?
					if (lastMove.Start.Offset((lastMove.End.X - lastMove.Start.X) / 2, 0) == softMoves[i].End) return true;
				}
			}
			return false;
		}

		//private voids
		public void CalcSoftMoves()
		{
			softMoves = new List<Move>();
			movesProved = false;
			BitBoard active;

			//Pawns
			PawnMoves(ref softMoves, pawn & (player ? black : white));


			//Knight
			TryMoveOrKill(ref softMoves, knight & (player ? black : white), 1, 2, Category.Knight);
			TryMoveOrKill(ref softMoves, knight & (player ? black : white), -1, 2, Category.Knight);
			TryMoveOrKill(ref softMoves, knight & (player ? black : white), 1, -2, Category.Knight);
			TryMoveOrKill(ref softMoves, knight & (player ? black : white), -1, -2, Category.Knight);

			TryMoveOrKill(ref softMoves, knight & (player ? black : white), 2, 1, Category.Knight);
			TryMoveOrKill(ref softMoves, knight & (player ? black : white), -2, 1, Category.Knight);
			TryMoveOrKill(ref softMoves, knight & (player ? black : white), 2, -1, Category.Knight);
			TryMoveOrKill(ref softMoves, knight & (player ? black : white), -2, -1, Category.Knight);

			//Bishop
			active = bishop & (player ? black : white);
			for (int i = 1; i < 8 && !active.Empty; i++) TryMoveOrKill(ref softMoves, ref active, i, i, Category.Bishop);
			active = bishop & (player ? black : white);
			for (int i = 1; i < 8 && !active.Empty; i++) TryMoveOrKill(ref softMoves, ref active, -i, i, Category.Bishop);
			active = bishop & (player ? black : white);
			for (int i = 1; i < 8 && !active.Empty; i++) TryMoveOrKill(ref softMoves, ref active, i, -i, Category.Bishop);
			active = bishop & (player ? black : white);
			for (int i = 1; i < 8 && !active.Empty; i++) TryMoveOrKill(ref softMoves, ref active, -i,- i, Category.Bishop);

			//Rook
			active = rook & (player ? black : white); //as ulong as there are active pieces test further
			for (int i = 1; i < 8 && !active.Empty; i++) TryMoveOrKill(ref softMoves, ref active, i, 0, Category.Rook);
			active = rook & (player ? black : white);
			for (int i = 1; i < 8 && !active.Empty; i++) TryMoveOrKill(ref softMoves, ref active, -i, 0, Category.Rook);
			active = rook & (player ? black : white);
			for (int i = 1; i < 8 && !active.Empty; i++) TryMoveOrKill(ref softMoves, ref active, 0, i, Category.Rook);
			active = rook & (player ? black : white);
			for (int i = 1; i < 8 && !active.Empty; i++) TryMoveOrKill(ref softMoves, ref active, 0, -i, Category.Rook);

			//Queen
			active = queen & (player ? black : white);
			for (int i = 1; i < 8 && !active.Empty; i++) TryMoveOrKill(ref softMoves, ref active, i, i, Category.Queen);
			active = queen & (player ? black : white);
			for (int i = 1; i < 8 && !active.Empty; i++) TryMoveOrKill(ref softMoves, ref active, -i, i, Category.Queen);
			active = queen & (player ? black : white);
			for (int i = 1; i < 8 && !active.Empty; i++) TryMoveOrKill(ref softMoves, ref active, -i, -i, Category.Queen);
			active = queen & (player ? black : white);
			for (int i = 1; i < 8 && !active.Empty; i++) TryMoveOrKill(ref softMoves, ref active, i, -i, Category.Queen);

			active = queen & (player ? black : white);
			for (int i = 1; i < 8 && !active.Empty; i++) TryMoveOrKill(ref softMoves, ref active, i, 0, Category.Queen);
			active = queen & (player ? black : white);
			for (int i = 1; i < 8 && !active.Empty; i++) TryMoveOrKill(ref softMoves, ref active, -i, 0, Category.Queen);
			active = queen & (player ? black : white);
			for (int i = 1; i < 8 && !active.Empty; i++) TryMoveOrKill(ref softMoves, ref active, 0, i, Category.Queen);
			active = queen & (player ? black : white);
			for (int i = 1; i < 8 && !active.Empty; i++) TryMoveOrKill(ref softMoves, ref active, 0, -i, Category.Queen);


			//king
			TryMoveOrKill(ref softMoves, king & (player ? black : white), 1, -1, Category.King);
			TryMoveOrKill(ref softMoves, king & (player ? black : white), -1, 1, Category.King);
			TryMoveOrKill(ref softMoves, king & (player ? black : white), -1, -1, Category.King);
			TryMoveOrKill(ref softMoves, king & (player ? black : white), 1, 1, Category.King);

			TryMoveOrKill(ref softMoves, king & (player ? black : white), 1, 0, Category.King);
			TryMoveOrKill(ref softMoves, king & (player ? black : white), -1, 0, Category.King);
			TryMoveOrKill(ref softMoves, king & (player ? black : white), 0, 1, Category.King);
			TryMoveOrKill(ref softMoves, king & (player ? black : white), 0, -1, Category.King);

			//castle
			if (Player)
			{
				if (!(BlackARookMoved || BlackKingMoved))
				{
					if (!(AllPieces.Get(new Position("b8")) || AllPieces.Get(new Position("c8")) || AllPieces.Get(new Position("d8"))))
					{
						softMoves.Add(new Move(Category.King, new Position("e8"), new Position("c8"), false));
					}
				}
	
				if (!(BlackHRookMoved || BlackKingMoved))
				{
					if (!(AllPieces.Get(new Position("f8")) || AllPieces.Get(new Position("g8"))))
					{
						softMoves.Add(new Move(Category.King, new Position("e8"), new Position("g8"), false));
					}
				}	
			}
			else
			{

				if (!(WhiteARookMoved || WhiteKingMoved))
				{
					if (!(AllPieces.Get(new Position("b1")) || AllPieces.Get(new Position("c1")) || AllPieces.Get(new Position("d1"))))
					{
						softMoves.Add(new Move(Category.King, new Position("e1"), new Position("c1"), false));
					}
				}

				if (!(WhiteHRookMoved || WhiteKingMoved))
				{
					if (!(AllPieces.Get(new Position("f1")) || AllPieces.Get(new Position("g1"))))
					{
						softMoves.Add(new Move(Category.King, new Position("e1"), new Position("g1"), false));
					}
				}

			}

		}

		//private functions
		private void TryMoveOrKill(ref List<Move> list, ref BitBoard active, int dx, int dy, Category c)
		{
			if(!active.Empty)
			{
				BitBoard coll, move, finalPos;
				//pieces that are out of bounce are no more active
				active.Move(dx, dy); active.Move(-dx, -dy);
				finalPos = active.Offset(dx, dy);
				//marks all  collisions points
				coll = finalPos & AllPieces;
				if (!coll.Empty)
				{
					//are there pieces that dont collide?				
					if (coll != finalPos)
					{   //add all pieces that are active and dont collide 
						list.AddRange(Move.GetMoves(c, dx, dy, finalPos & ~coll, false));
						//remove all not colliding pieces from collider
						coll.RemoveAt(finalPos & ~coll);
					}

					//colliding with opposite color?
					move = coll & (player ? white : black);
					if (!move.Empty)
					{
						//adding all pieces that kill another
						list.AddRange(Move.GetMoves(c, dx, dy, move, true));
						//pieces that collide here are no more active
						active.RemoveAt(move.Offset(-dx, -dy));
					}
					//pieces that collide with the same color are no more active
					active.RemoveAt((coll & (player ? black : white)).Offset(-dx, -dy));
				}
				else list.AddRange(Move.GetMoves(c, dx, dy, finalPos, false)); //no collision at all
			}
		}
		private void TryMoveOrKill(ref List<Move> list, BitBoard active, int dx, int dy, Category c)
		{
			if (!active.Empty)
			{
				BitBoard finalPos;
				//pieces that are out of bounce are no more active
				finalPos = active.Offset(dx, dy);

				list.AddRange(Move.GetMoves(c, dx, dy, finalPos & ~AllPieces, false));
				list.AddRange(Move.GetMoves(c, dx, dy, finalPos & (player ? white : black), true));
			}
		}
		private void PawnMoves(ref List<Move> list, BitBoard active)
		{
			int direction = player ? -1 : 1;

			//moving 1 
			BitBoard finalPos = active.Offset(0, direction) & ~AllPieces;
			list.AddRange(Move.GetMoves(Category.Pawn, 0, direction, finalPos, false));
			//moving 2
			finalPos.Move(0, direction);
			list.AddRange(Move.GetMoves(Category.Pawn, 0, direction * 2, finalPos & ~AllPieces & (player ? BlackPawns : WhitePawns), false));

			//kill right
			finalPos = active.Offset(1, direction);
			list.AddRange(Move.GetMoves(Category.Pawn, 1, direction, finalPos & (player ? white : black), true));
			//kill left
			finalPos = active.Offset(-1, direction);
			list.AddRange(Move.GetMoves(Category.Pawn, -1, direction, finalPos & (player ? white : black), true));

			//en passant
			if(lastMove.Category == Category.Pawn && Math.Abs(lastMove.Start.Y - lastMove.End.Y) == 2)
			{
				//kill right
				finalPos = active.Offset(1, direction) & new BitBoard(lastMove.End).Offset(0, direction);
				list.AddRange(Move.GetMoves(Category.Pawn, 1, direction, finalPos, true));
				//kill left
				finalPos = active.Offset(-1, direction) & new BitBoard(lastMove.End).Offset(0, direction);
				list.AddRange(Move.GetMoves(Category.Pawn, -1, direction, finalPos, true));
			}
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
